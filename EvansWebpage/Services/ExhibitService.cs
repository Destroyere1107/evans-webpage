using EvansWebpage.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage.Services;

/// <summary>
/// Singleton service that loads the calculator exhibit catalog once at startup
/// from an embedded SQLite database and exposes pre-computed data for all museum pages.
/// </summary>
public class ExhibitService
{
    private readonly IReadOnlyList<Exhibit> _exhibits;
    private readonly Dictionary<string, Exhibit> _byId;

    // Pre-computed stats
    public int TotalCalculators { get; }
    public int TotalManufacturers { get; }
    public int TotalSpecimens { get; }
    public int TotalPhotos { get; }
    public int DocumentedSpecimens { get; }
    public string YearRange { get; }
    public Exhibit? OldestExhibit { get; }
    public Exhibit? NewestExhibit { get; }
    public (Exhibit Exhibit, Specimen Specimen)? OldestSpecimen { get; }
    public (Exhibit Exhibit, Specimen Specimen)? NewestSpecimen { get; }

    // Pre-grouped directory data: Category -> Manufacturer -> List<Exhibit>
    private readonly Dictionary<string, Dictionary<string, List<Exhibit>>> _groupedDirectory;

    public ExhibitService([FromKeyedServices("DataFiles")] IFileProvider dataFiles)
    {
        var fileInfo = dataFiles.GetFileInfo("calcs/calcs.db");

        if (!fileInfo.Exists)
        {
            _exhibits = [];
            _byId = new Dictionary<string, Exhibit>();
            _groupedDirectory = new Dictionary<string, Dictionary<string, List<Exhibit>>>();
            YearRange = "";
            return;
        }

        // Extract the embedded .db to a temp file so SQLite can open it
        var tempPath = Path.GetTempFileName();
        try
        {
            using (var embeddedStream = fileInfo.CreateReadStream())
            using (var tempFile = File.Create(tempPath))
            {
                embeddedStream.CopyTo(tempFile);
            }

            var allExhibits = LoadFromDatabase(tempPath);

            _exhibits = allExhibits.AsReadOnly();
            _byId = allExhibits.ToDictionary(e => e.Id, e => e);

            // --- Pre-compute collection stats ---

            TotalCalculators = allExhibits.Count;
            TotalManufacturers = allExhibits.Select(e => e.Manufacturer).Distinct().Count();
            TotalSpecimens = allExhibits.Sum(e => e.Specimens?.Count ?? 0);

            TotalPhotos = allExhibits.Sum(e =>
                (!string.IsNullOrEmpty(e.MainImageUrl) &&
                 !e.MainImageUrl.Equals("[coming soon]", StringComparison.OrdinalIgnoreCase)
                    ? 1
                    : 0) +
                (e.Gallery?.Count ?? 0) +
                (e.Specimens?.Count(s =>
                    !string.IsNullOrEmpty(s.ImageUrl) &&
                    !s.ImageUrl.Equals("[coming soon]", StringComparison.OrdinalIgnoreCase)) ?? 0)
            );

            DocumentedSpecimens = allExhibits.Sum(e =>
                e.Specimens?.Count(s =>
                    !string.IsNullOrEmpty(s.ImageUrl) &&
                    !s.ImageUrl.Equals("[coming soon]", StringComparison.OrdinalIgnoreCase)) ?? 0
            );

            var withYears = allExhibits.Where(e => e.YearIntroduced.HasValue).ToList();
            if (withYears.Count > 0)
            {
                var minYear = withYears.Min(e => e.YearIntroduced!.Value);
                var maxYear = withYears.Max(e => e.YearIntroduced!.Value);
                YearRange = $"{minYear}–{maxYear}";
                OldestExhibit = withYears.OrderBy(e => e.YearIntroduced).First();
                NewestExhibit = withYears.OrderByDescending(e => e.YearIntroduced).First();
            }
            else
            {
                YearRange = "";
            }

            var specimensWithYears = allExhibits
                .Where(e => e.Specimens != null)
                .SelectMany(e => e.Specimens.Select(s => (Exhibit: e, Specimen: s)))
                .Where(x => x.Specimen.ManufactureYear.HasValue)
                .ToList();

            if (specimensWithYears.Count > 0)
            {
                OldestSpecimen = specimensWithYears.OrderBy(x => x.Specimen.ManufactureYear).First();
                NewestSpecimen = specimensWithYears.OrderByDescending(x => x.Specimen.ManufactureYear).First();
            }

            // --- Pre-group for Directory page ---

            _groupedDirectory = allExhibits
                .GroupBy(e => string.IsNullOrEmpty(e.Category) ? "Other" : e.Category)
                .ToDictionary(
                    catGroup => catGroup.Key,
                    catGroup => catGroup
                        .GroupBy(e => string.IsNullOrEmpty(e.Manufacturer) ? "Unknown" : e.Manufacturer)
                        .ToDictionary(
                            mfgGroup => mfgGroup.Key,
                            mfgGroup => mfgGroup.OrderBy(e => e.Model).ToList()
                        )
                );
        }
        finally
        {
            // Clean up the temp file
            try { File.Delete(tempPath); } catch { /* best-effort cleanup */ }
        }
    }

    private static List<Exhibit> LoadFromDatabase(string dbPath)
    {
        var exhibits = new Dictionary<string, Exhibit>();

        using var conn = new SqliteConnection($"Data Source={dbPath};Mode=ReadOnly");
        conn.Open();

        // Load exhibits
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Exhibits";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var exhibit = new Exhibit
                {
                    Id = reader.GetString(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Category = GetStringOrEmpty(reader, "Category"),
                    Manufacturer = GetStringOrEmpty(reader, "Manufacturer"),
                    ManufacturerLogo = GetStringOrEmpty(reader, "ManufacturerLogo"),
                    ManufacturerSlug = GetStringOrEmpty(reader, "ManufacturerSlug"),
                    Model = GetStringOrEmpty(reader, "Model"),
                    ModelSlug = GetStringOrEmpty(reader, "ModelSlug"),
                    Type = GetStringOrEmpty(reader, "Type"),
                    YearIntroduced = reader.IsDBNull(reader.GetOrdinal("YearIntroduced"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("YearIntroduced")),
                    MainImageUrl = GetStringOrEmpty(reader, "MainImageUrl"),
                    UnderConstruction = reader.GetInt32(reader.GetOrdinal("UnderConstruction")) != 0,
                    HasCas = reader.GetInt32(reader.GetOrdinal("HasCas")) != 0,
                    HasGraphing = reader.GetInt32(reader.GetOrdinal("HasGraphing")) != 0,
                    HasColor = reader.GetInt32(reader.GetOrdinal("HasColor")) != 0,
                };
                exhibits[exhibit.Id] = exhibit;
            }
        }

        // Load specimens
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Specimens ORDER BY ExhibitId, Number";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var exhibitId = reader.GetString(reader.GetOrdinal("ExhibitId"));
                if (!exhibits.TryGetValue(exhibitId, out var exhibit)) continue;

                exhibit.Specimens.Add(new Specimen
                {
                    Number = reader.GetInt32(reader.GetOrdinal("Number")),
                    Variant = GetStringOrEmpty(reader, "Variant"),
                    SerialNumber = GetStringOrEmpty(reader, "SerialNumber"),
                    Condition = GetStringOrEmpty(reader, "Condition"),
                    ImageUrl = GetStringOrEmpty(reader, "ImageUrl"),
                    ManufactureDate = GetStringOrEmpty(reader, "ManufactureDate"),
                    Datecode = GetStringOrEmpty(reader, "Datecode"),
                    CountryOfManufacture = GetStringOrEmpty(reader, "CountryOfManufacture"),
                    HardwareRevision = GetStringOrEmpty(reader, "HardwareRevision"),
                    AcquisitionDate = GetStringOrEmpty(reader, "AcquisitionDate"),
                });
            }
        }

        // Load gallery images
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM GalleryImages ORDER BY ExhibitId, Id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var exhibitId = reader.GetString(reader.GetOrdinal("ExhibitId"));
                if (!exhibits.TryGetValue(exhibitId, out var exhibit)) continue;

                exhibit.Gallery.Add(new GalleryImage
                {
                    Url = GetStringOrEmpty(reader, "Url"),
                    AltText = GetStringOrEmpty(reader, "AltText"),
                    Caption = GetStringOrEmpty(reader, "Caption"),
                });
            }
        }

        // Load MyCalcsLinks
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM MyCalcsLinks ORDER BY ExhibitId, Id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var exhibitId = reader.GetString(reader.GetOrdinal("ExhibitId"));
                if (!exhibits.TryGetValue(exhibitId, out var exhibit)) continue;

                exhibit.MyCalcsLinks.Add(new MyCalcsLink
                {
                    Id = GetStringOrEmpty(reader, "LinkId"),
                    Name = GetStringOrEmpty(reader, "Name"),
                });
            }
        }

        return exhibits.Values.ToList();
    }

    private static string GetStringOrEmpty(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? "" : reader.GetString(ordinal);
    }

    /// <summary>Returns all exhibits in the catalog.</summary>
    public IReadOnlyList<Exhibit> GetAll() => _exhibits;

    /// <summary>Looks up a single exhibit by ID. O(1) dictionary lookup.</summary>
    public Exhibit? GetById(string id) =>
        _byId.TryGetValue(id, out var exhibit) ? exhibit : null;

    /// <summary>
    /// Returns the pre-grouped directory data: Category → Manufacturer → Exhibits.
    /// </summary>
    public Dictionary<string, Dictionary<string, List<Exhibit>>> GetGroupedDirectory() =>
        _groupedDirectory;

    /// <summary>
    /// Returns all exhibits except those with the given IDs.
    /// Used for picking a random exhibit that doesn't duplicate manual picks.
    /// </summary>
    public IReadOnlyList<Exhibit> GetExcluding(IEnumerable<string> excludeIds)
    {
        var excluded = new HashSet<string>(excludeIds);
        return _exhibits.Where(e => !excluded.Contains(e.Id)).ToList();
    }
}
