var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Backwards compatibility: redirect /index.html to /
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value?.Equals("/index.html", StringComparison.OrdinalIgnoreCase) == true)
    {
        context.Response.Redirect("/", permanent: true);
        return;
    }
    await next();
});

app.MapRazorPages();

app.Run();
