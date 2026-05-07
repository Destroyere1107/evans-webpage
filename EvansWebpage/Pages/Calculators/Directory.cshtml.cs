using System.Text.Json;
using EvansWebpage.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage.Pages.Calculators;

public class DirectoryModel : PageModel
{
    private readonly IFileProvider _dataFiles;

    public DirectoryModel([FromKeyedServices("DataFiles")] IFileProvider dataFiles)
    {
        _dataFiles = dataFiles;
    }

    // Data structure: Category -> Manufacturer -> List of Exhibits
    public Dictionary<string, Dictionary<string, List<Exhibit>>> DirectoryData { get; set; } = new();

    public void OnGet()
    {
        var fileInfo = _dataFiles.GetFileInfo("calcs/calcs.json");

        if (!fileInfo.Exists) return;

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        var jsonString = reader.ReadToEnd();
        var allExhibits = JsonSerializer.Deserialize<List<Exhibit>>(jsonString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (allExhibits != null && allExhibits.Any())
            // Use LINQ to group the data cleanly
            DirectoryData = allExhibits
                .GroupBy(e => string.IsNullOrEmpty(e.Category) ? "Other" : e.Category)
                .ToDictionary(
                    catGroup => catGroup.Key,
                    catGroup => catGroup
                        .GroupBy(e => string.IsNullOrEmpty(e.Manufacturer) ? "Unknown" : e.Manufacturer)
                        .ToDictionary(
                            mfgGroup => mfgGroup.Key,
                            mfgGroup => mfgGroup.OrderBy(e => e.Model).ToList() // Alphabetize models
                        )
                );
    }
}