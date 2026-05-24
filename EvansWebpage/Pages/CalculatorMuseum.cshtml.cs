using System.Text.Json;
using System.Text.RegularExpressions;
using EvansWebpage.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage.Pages;

public class CalculatorMuseumModel : PageModel
{
    private readonly IFileProvider _dataFiles;

    public CalculatorMuseumModel([FromKeyedServices("DataFiles")] IFileProvider dataFiles)
    {
        _dataFiles = dataFiles;
    }

    // --- Collection Stats ---
    public int TotalCalculators { get; set; }
    public int TotalManufacturers { get; set; }
    public string YearRange { get; set; } = "";
    public int TotalSpecimens { get; set; }
    public Exhibit? OldestCalculator { get; set; }
    public Exhibit? NewestCalculator { get; set; }
    public int TotalPhotos { get; set; }
    public int DocumentedSpecimens { get; set; }

    // --- Featured Exhibits (Manual & Random) ---
    // Change these IDs to swap the manually-curated featured cards.
    public static readonly string ExhibitOfTheMonthId = "28";        // HP 28 Series
    private const string CuratorsChoiceId    = "50g";       // HP 50g
    private const string NewestAdditionId    = "nspire-cx"; // TI-Nspire CX Series

    public Exhibit? ExhibitOfTheMonth { get; set; }
    public string? ExhibitOfTheMonthDescription { get; set; }

    public Exhibit? CuratorsChoice { get; set; }
    public string? CuratorsChoiceDescription { get; set; }

    public Exhibit? NewestAddition { get; set; }
    public string? NewestAdditionDescription { get; set; }

    public Exhibit? RandomExhibit { get; set; }
    public string? RandomExhibitDescription { get; set; }

    public async Task OnGetAsync()
    {
        var fileInfo = _dataFiles.GetFileInfo("calcs/calcs.json");

        if (!fileInfo.Exists) return;

        using var stream = fileInfo.CreateReadStream();
        var allExhibits = await JsonSerializer.DeserializeAsync<List<Exhibit>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (allExhibits == null || !allExhibits.Any()) return;

        // 1. Compute Collection Stats
        TotalCalculators = allExhibits.Count;
        TotalManufacturers = allExhibits.Select(e => e.Manufacturer).Distinct().Count();
        TotalSpecimens = allExhibits.Sum(e => e.Specimens?.Count ?? 0);

        // Calculate visual photo assets in the museum catalog
        TotalPhotos = allExhibits.Sum(e =>
            (!string.IsNullOrEmpty(e.MainImageUrl) && !e.MainImageUrl.Equals("[coming soon]", StringComparison.OrdinalIgnoreCase) ? 1 : 0) +
            (e.Gallery?.Count ?? 0) +
            (e.Specimens?.Count(s => !string.IsNullOrEmpty(s.ImageUrl) && !s.ImageUrl.Equals("[coming soon]", StringComparison.OrdinalIgnoreCase)) ?? 0)
        );

        // Count specimens that have been photographed (non-empty, non-placeholder image)
        DocumentedSpecimens = allExhibits.Sum(e =>
            e.Specimens?.Count(s => !string.IsNullOrEmpty(s.ImageUrl) && !s.ImageUrl.Equals("[coming soon]", StringComparison.OrdinalIgnoreCase)) ?? 0
        );

        var withYears = allExhibits.Where(e => e.YearIntroduced.HasValue).ToList();
        if (withYears.Any())
        {
            var minYear = withYears.Min(e => e.YearIntroduced!.Value);
            var maxYear = withYears.Max(e => e.YearIntroduced!.Value);
            YearRange = $"{minYear}–{maxYear}";

            OldestCalculator = withYears.OrderBy(e => e.YearIntroduced).First();
            NewestCalculator = withYears.OrderByDescending(e => e.YearIntroduced).First();
        }

        // 2. Fetch manual featured exhibits
        ExhibitOfTheMonth = allExhibits.FirstOrDefault(e => e.Id == ExhibitOfTheMonthId);
        ExhibitOfTheMonthDescription = await LoadDescriptionSnippetAsync(ExhibitOfTheMonth);

        CuratorsChoice = allExhibits.FirstOrDefault(e => e.Id == CuratorsChoiceId);
        CuratorsChoiceDescription = await LoadDescriptionSnippetAsync(CuratorsChoice);

        NewestAddition = allExhibits.FirstOrDefault(e => e.Id == NewestAdditionId);
        NewestAdditionDescription = await LoadDescriptionSnippetAsync(NewestAddition);

        // 3. Pick a random exhibit, excluding any manual picks to avoid duplication
        var manualIds = new HashSet<string> { ExhibitOfTheMonthId, CuratorsChoiceId, NewestAdditionId };
        var candidates = allExhibits.Where(e => !manualIds.Contains(e.Id)).ToList();
        if (candidates.Any())
        {
            RandomExhibit = candidates[Random.Shared.Next(candidates.Count)];
            RandomExhibitDescription = await LoadDescriptionSnippetAsync(RandomExhibit);
        }
    }

    private async Task<string?> LoadDescriptionSnippetAsync(Exhibit? exhibit)
    {
        if (exhibit == null) return null;

        var mdPath = $"calcs/md/{exhibit.ManufacturerSlug}/{exhibit.ModelSlug}/description.md";
        var fileInfo = _dataFiles.GetFileInfo(mdPath);

        if (!fileInfo.Exists) return null;

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        var rawMarkdown = await reader.ReadToEndAsync();

        var cleanText = StripMarkdown(rawMarkdown);

        if (cleanText.Length > 200)
        {
            // Find last space before the 200-char limit to avoid cutting mid-word
            var cutoff = cleanText.LastIndexOf(' ', 200);
            if (cutoff <= 0) cutoff = 200;
            return cleanText.Substring(0, cutoff).Trim() + "…";
        }

        return cleanText;
    }

    private string StripMarkdown(string markdown)
    {
        if (string.IsNullOrEmpty(markdown)) return "";

        // Strip markdown images: ![alt](url)
        var text = Regex.Replace(markdown, @"!\[.*?\]\(.*?\)", "");

        // Replace markdown links [text](url) with just the text
        text = Regex.Replace(text, @"\[(.*?)\]\(.*?\)", "$1");

        // Remove inline formatting symbols like asterisks, underscores, backticks, hashes
        text = Regex.Replace(text, @"[\*_`#]", "");

        // Normalize spaces and newlines
        text = Regex.Replace(text, @"\s+", " ").Trim();

        return text;
    }
}
