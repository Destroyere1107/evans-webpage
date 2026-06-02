using EvansWebpage.Models;
using EvansWebpage.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvansWebpage.Pages;

public class CalculatorMuseumModel : PageModel
{
    private readonly ExhibitService _exhibitService;
    private readonly MarkdownService _markdownService;

    public CalculatorMuseumModel(ExhibitService exhibitService, MarkdownService markdownService)
    {
        _exhibitService = exhibitService;
        _markdownService = markdownService;
    }

    // --- Collection Stats ---
    public int TotalCalculators { get; set; }
    public int TotalManufacturers { get; set; }
    public string YearRange { get; set; } = "";
    public int TotalSpecimens { get; set; }
    public Exhibit? OldestCalculator { get; set; }
    public Exhibit? NewestCalculator { get; set; }
    public (Exhibit Exhibit, Specimen Specimen)? OldestSpecimen { get; set; }
    public (Exhibit Exhibit, Specimen Specimen)? NewestSpecimen { get; set; }
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

    public void OnGet()
    {
        // 1. Pull pre-computed stats from the singleton service
        TotalCalculators = _exhibitService.TotalCalculators;
        TotalManufacturers = _exhibitService.TotalManufacturers;
        TotalSpecimens = _exhibitService.TotalSpecimens;
        TotalPhotos = _exhibitService.TotalPhotos;
        DocumentedSpecimens = _exhibitService.DocumentedSpecimens;
        YearRange = _exhibitService.YearRange;
        OldestCalculator = _exhibitService.OldestExhibit;
        NewestCalculator = _exhibitService.NewestExhibit;
        OldestSpecimen = _exhibitService.OldestSpecimen;
        NewestSpecimen = _exhibitService.NewestSpecimen;

        // 2. Fetch manual featured exhibits (O(1) dictionary lookups)
        ExhibitOfTheMonth = _exhibitService.GetById(ExhibitOfTheMonthId);
        ExhibitOfTheMonthDescription = LoadDescriptionSnippet(ExhibitOfTheMonth);

        CuratorsChoice = _exhibitService.GetById(CuratorsChoiceId);
        CuratorsChoiceDescription = LoadDescriptionSnippet(CuratorsChoice);

        NewestAddition = _exhibitService.GetById(NewestAdditionId);
        NewestAdditionDescription = LoadDescriptionSnippet(NewestAddition);

        // 3. Pick a random exhibit, excluding any manual picks to avoid duplication
        var candidates = _exhibitService.GetExcluding(
            [ExhibitOfTheMonthId, CuratorsChoiceId, NewestAdditionId]);
        if (candidates.Count > 0)
        {
            RandomExhibit = candidates[Random.Shared.Next(candidates.Count)];
            RandomExhibitDescription = LoadDescriptionSnippet(RandomExhibit);
        }
    }

    private string? LoadDescriptionSnippet(Exhibit? exhibit)
    {
        if (exhibit == null) return null;

        var mdPath = $"calcs/md/{exhibit.ManufacturerId}/{exhibit.ModelSlug}/description.md";
        return _markdownService.GetSnippet(mdPath);
    }
}
