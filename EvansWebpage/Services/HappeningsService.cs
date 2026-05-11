using EvansWebpage.Models;
using Markdig;
using Microsoft.AspNetCore.Hosting;

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
            string postsDirectory = Path.Combine(_env.ContentRootPath, "Data", "happenings", "posts");

            if (!Directory.Exists(postsDirectory)) return posts;

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string[] files = Directory.GetFiles(postsDirectory, "*.md");

            foreach (var file in files)
            {
                string rawText = File.ReadAllText(file);
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

            // Return the list sorted from newest to oldest
            return posts.OrderByDescending(p => p.Date).ToList();
        }
    }
}