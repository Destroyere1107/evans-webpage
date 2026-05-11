using EvansWebpage.Models;
using Markdig;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection; // Required for reading embedded resources

namespace EvansWebpage.Services
{
    public class HappeningsService
    {
        private readonly IWebHostEnvironment _env;

        public HappeningsService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public List<NewsPost> GetAllPosts()
        {
            var posts = new List<NewsPost>();
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            
            var assembly = Assembly.GetExecutingAssembly();
            
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(n => n.Contains(".Data.happenings.posts.") && n.EndsWith(".md"));

            foreach (var resourceName in resourceNames)
            {
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                using StreamReader reader = new StreamReader(stream);
                string rawText = reader.ReadToEnd();
                
                var sections = rawText.Split("---", StringSplitOptions.RemoveEmptyEntries);

                if (sections.Length >= 2)
                {
                    var metaLines = sections[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    string title = "Untitled";
                    DateTime postDate = DateTime.MinValue;

                    foreach (var line in metaLines)
                    {
                        if (line.StartsWith("Title:")) title = line.Substring(6).Trim();
                        if (line.StartsWith("Date:")) DateTime.TryParse(line.Substring(5).Trim(), out postDate);
                    }

                    string htmlBody = Markdown.ToHtml(sections[1], pipeline);

                    posts.Add(new NewsPost
                    {
                        Title = title,
                        Date = postDate,
                        HtmlContent = htmlBody
                    });
                }
            }

            return posts.OrderByDescending(p => p.Date).ToList();
        }
    }
}