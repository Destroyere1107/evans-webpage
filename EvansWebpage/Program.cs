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

        app.MapRazorPages();

        app.Run();
    }
}