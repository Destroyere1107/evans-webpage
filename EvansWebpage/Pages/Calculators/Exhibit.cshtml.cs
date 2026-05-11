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
    
    public async Task<IActionResult> OnGetAsync(string id)
    {
        var fileInfo = _dataFiles.GetFileInfo("calcs/calcs.json");

        if (!fileInfo.Exists) return NotFound();

        using var stream = fileInfo.CreateReadStream();
        
        var allCalculators = await JsonSerializer.DeserializeAsync<List<Exhibit>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Exhibit = allCalculators?.FirstOrDefault(c => c.Id == id);

        if (Exhibit == null) return NotFound();

        var mdBasePath = $"calcs/md/{Exhibit.ManufacturerSlug}/{Exhibit.ModelSlug}";

        // 3. Await the updated helper method
        DescriptionHtml = await ParseMarkdownFileAsync(mdBasePath, "description.md", "<p><i>Content coming soon.</i></p>");
        NotesHtml = await ParseMarkdownFileAsync(mdBasePath, "notes.md", null);
        SpecimensHtml = await ParseMarkdownFileAsync(mdBasePath, "specimens.md", "<p><i>Content coming soon.</i></p>");

        return Page();
    }
    
    private async Task<string?> ParseMarkdownFileAsync(string basePath, string fileName, string? fallback)
    {
        var fileInfo = _dataFiles.GetFileInfo($"{basePath}/{fileName}");

        if (!fileInfo.Exists) return fallback;

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        
        var markdownText = await reader.ReadToEndAsync();
        
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdownText, pipeline);
    }
}