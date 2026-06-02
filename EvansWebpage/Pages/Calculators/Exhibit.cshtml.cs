using EvansWebpage.Models;
using EvansWebpage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvansWebpage.Pages.Calculators;

public class ExhibitModel : PageModel
{
    private readonly ExhibitService _exhibitService;
    private readonly MarkdownService _markdownService;

    public ExhibitModel(ExhibitService exhibitService, MarkdownService markdownService)
    {
        _exhibitService = exhibitService;
        _markdownService = markdownService;
    }

    public Exhibit Exhibit { get; set; }
    public string? DescriptionHtml { get; set; }
    public string? NotesHtml { get; set; }
    public string? SpecimensHtml { get; set; }
    
    public IActionResult OnGet(string id)
    {
        Exhibit = _exhibitService.GetById(id);

        if (Exhibit == null) return NotFound();

        var mdBasePath = $"calcs/md/{Exhibit.ManufacturerId}/{Exhibit.ModelSlug}";

        DescriptionHtml = _markdownService.GetHtml($"{mdBasePath}/description.md", "<p><i>Content coming soon.</i></p>");
        NotesHtml = _markdownService.GetHtml($"{mdBasePath}/notes.md");
        SpecimensHtml = _markdownService.GetHtml($"{mdBasePath}/specimens.md", "<p><i>Content coming soon.</i></p>");

        return Page();
    }
}