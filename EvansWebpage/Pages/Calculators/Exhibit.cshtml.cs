using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Markdig;
using EvansWebpage.Models;

namespace EvansWebpage.Pages.Calculators
{
    public class ExhibitModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public ExhibitModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        public Exhibit Exhibit { get; set; }
        public string DescriptionHtml { get; set; }
        public string NotesHtml { get; set; }
        public string SpecimensHtml { get; set; }

        public IActionResult OnGet(string id)
        {
            var jsonPath = Path.Combine(_env.ContentRootPath, "Data", "calcs", "calcs.json");
            
            if (!System.IO.File.Exists(jsonPath)) 
            {
                return NotFound();
            }

            var jsonString = System.IO.File.ReadAllText(jsonPath);
            
            var allCalculators = JsonSerializer.Deserialize<List<Exhibit>>(jsonString, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Exhibit = allCalculators?.FirstOrDefault(c => c.Id == id);

            if (Exhibit == null) 
            {
                return NotFound();
            }
            
            var targetDirectory = Path.Combine(
                _env.ContentRootPath, 
                "Data", "calcs", "md", 
                Exhibit.ManufacturerSlug, 
                Exhibit.ModelSlug
            );

            DescriptionHtml = ParseMarkdownFile(targetDirectory, "description.md");
            NotesHtml = ParseMarkdownFile(targetDirectory, "notes.md");
            SpecimensHtml = ParseMarkdownFile(targetDirectory, "specimens.md");

            return Page();
        }

        private string ParseMarkdownFile(string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName);

            if (!System.IO.File.Exists(filePath)) return "<p><i>Content coming soon.</i></p>";
            var markdownText = System.IO.File.ReadAllText(filePath);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdownText, pipeline);

        }
    }
}