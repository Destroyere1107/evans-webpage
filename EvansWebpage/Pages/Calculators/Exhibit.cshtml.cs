using System.Text.Json;
using EvansWebpage.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage.Pages.Calculators;

public class ExhibitModel : PageModel
{
    private readonly IFileProvider _dataFiles;

    public ExhibitModel([FromKeyedServices("DataFiles")] IFileProvider dataFiles)
    {
        _dataFiles = dataFiles;
    }

    public Exhibit Exhibit { get; set; }
    public string? DescriptionHtml { get; set; }
    public string? NotesHtml { get; set; }
    public string? SpecimensHtml { get; set; }

    public IActionResult OnGet(string id)
    {
        var fileInfo = _dataFiles.GetFileInfo("calcs/calcs.json");

        if (!fileInfo.Exists) return NotFound();

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        var jsonString = reader.ReadToEnd();

        var allCalculators = JsonSerializer.Deserialize<List<Exhibit>>(jsonString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Exhibit = allCalculators?.FirstOrDefault(c => c.Id == id);

        if (Exhibit == null) return NotFound();

        var mdBasePath = $"calcs/md/{Exhibit.ManufacturerSlug}/{Exhibit.ModelSlug}";

        DescriptionHtml = ParseMarkdownFile(mdBasePath, "description.md", "<p><i>Content coming soon.</i></p>");
        NotesHtml = ParseMarkdownFile(mdBasePath, "notes.md", null);
        SpecimensHtml = ParseMarkdownFile(mdBasePath, "specimens.md", "<p><i>Content coming soon.</i></p>");

        return Page();
    }

    private string? ParseMarkdownFile(string basePath, string fileName, string? fallback)
    {
        var fileInfo = _dataFiles.GetFileInfo($"{basePath}/{fileName}");

        if (!fileInfo.Exists) return fallback;

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        var markdownText = reader.ReadToEnd();
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdownText, pipeline);
    }
}