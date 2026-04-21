using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EvansWebpage.Models;

namespace EvansWebpage.Pages.Calculators
{
    public class DirectoryModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public DirectoryModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Data structure: Category -> Manufacturer -> List of Exhibits
        public Dictionary<string, Dictionary<string, List<Exhibit>>> DirectoryData { get; set; } = new();

        public void OnGet()
        {
            var jsonPath = Path.Combine(_env.ContentRootPath, "Data", "calcs", "calcs.json");
            
            if (!System.IO.File.Exists(jsonPath))
            {
                return;
            }

            var jsonString = System.IO.File.ReadAllText(jsonPath);
            var allExhibits = JsonSerializer.Deserialize<List<Exhibit>>(jsonString, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (allExhibits != null && allExhibits.Any())
            {
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
    }
}