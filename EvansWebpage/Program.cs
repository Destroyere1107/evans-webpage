using System.ServiceModel.Syndication;
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

        var app = builder.Build();

        if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

        // app.UseStaticFiles(); used to be here, but packaging wwwroot into the binary with the functionality added
// by Microsoft.Extensions.FileProviders.Embedded requires this
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new ManifestEmbeddedFileProvider(
                typeof(Program).Assembly, "wwwroot"
            )
        });
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
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });
    
            var rssFormatter = new Rss20FeedFormatter(feed, serializeExtensionsAsAtom: false);
            rssFormatter.WriteTo(xmlWriter);
            xmlWriter.Flush();

            // 5. Return as the correct MIME type so browsers and feed readers recognize it
            return Results.Text(stringWriter.ToString(), "application/rss+xml");
        });

        app.MapRazorPages();

        app.Run();
    }
}