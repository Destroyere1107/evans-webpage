using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using EvansWebpage.Services;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();

        // Register embedded file provider for Data folder so page models can
        // read data files from the published single-file binary.
        builder.Services.AddKeyedSingleton<IFileProvider>("DataFiles",
            new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "Data"));
        
        builder.Services.AddTransient<EvansWebpage.Services.HappeningsService>();
        builder.Services.AddSingleton<EvansWebpage.Services.ExhibitService>();
        builder.Services.AddSingleton<EvansWebpage.Services.MarkdownService>();

        // Set WebRootFileProvider so Tag Helpers (like asp-append-version) resolve hashes correctly
        builder.Environment.WebRootFileProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "wwwroot");

        var app = builder.Build();

        if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

        app.UseStaticFiles();
        app.UseRouting();

// Backwards compatibility: redirect /index.html to /
// I'm basically 100% sure that no one has bookmarked my site yet, but might as well.
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.Value?.Equals("/index.html", StringComparison.OrdinalIgnoreCase) == true)
            {
                context.Response.Redirect("/", true);
                return;
            }

            await next();
        });
        
        app.MapGet("/rss.xml", async (HappeningsService happeningsService) =>
        {
            var posts = await happeningsService.GetAllPostsAsync();
            
            var feed = new SyndicationFeed(
                "Destroyere1107's Happenings",
                "Miscellaneous news, thoughts, and technical updates.",
                new Uri("https://destroyere.yugoslavia.dev/Happenings") 
            );

            var items = new List<SyndicationItem>();
            
            foreach (var post in posts)
            {
                string slug = post.Title.ToLower().Replace(" ", "-").Replace("'", "");
                var itemUri = new Uri($"https://destroyere.yugoslavia.dev/Happenings#{slug}");

                var item = new SyndicationItem(
                    post.Title,
                    SyndicationContent.CreateHtmlContent(post.HtmlContent),
                    itemUri,
                    slug, 
                    new DateTimeOffset(post.Date)
                )
                {
                    PublishDate = new DateTimeOffset(post.Date)
                };
        
                items.Add(item);
            }

            feed.Items = items;

            // 4. Serialize to XML
            using var stringWriter = new StringWriter();
            using var stream = new MemoryStream();
            var settings = new XmlWriterSettings 
            { 
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                var rssFormatter = new Rss20FeedFormatter(feed, false);
                rssFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }

            return Results.File(stream.ToArray(), "application/rss+xml");
        });

        app.MapRazorPages();

        app.Run();
    }
}