using System.Text.Json;
using EvansWebpage.Models;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage.Services;

/// <summary>
/// Singleton service that loads the calculator exhibit catalog once at startup
/// and exposes pre-computed data for all museum pages.
/// </summary>
public class ExhibitService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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

    // Pre-grouped directory data: Category -> Manufacturer -> List<Exhibit>
    private readonly Dictionary<string, Dictionary<string, List<Exhibit>>> _groupedDirectory;

    public ExhibitService([FromKeyedServices("DataFiles")] IFileProvider dataFiles)
    {
        var fileInfo = dataFiles.GetFileInfo("calcs/calcs.json");

        if (!fileInfo.Exists)
        {
            _exhibits = [];
            _byId = new Dictionary<string, Exhibit>();
            _groupedDirectory = new Dictionary<string, Dictionary<string, List<Exhibit>>>();
            YearRange = "";
            return;
        }

        // Load once synchronously — data is embedded in the assembly, not disk I/O
        using var stream = fileInfo.CreateReadStream();
        var allExhibits = JsonSerializer.Deserialize<List<Exhibit>>(stream, JsonOptions) ?? [];

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
