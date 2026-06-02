using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Markdig;
using Microsoft.Extensions.FileProviders;

namespace EvansWebpage.Services;

/// <summary>
/// Singleton service that reads, parses, and caches embedded markdown files.
/// Each file is read and rendered at most once for the lifetime of the process.
/// </summary>
public partial class MarkdownService
{
    private readonly IFileProvider _dataFiles;
    private readonly MarkdownPipeline _pipeline;

    // Caches: path -> rendered result (null means "file not found, use fallback")
    private readonly ConcurrentDictionary<string, string?> _htmlCache = new();
    private readonly ConcurrentDictionary<string, string?> _snippetCache = new();

    public MarkdownService([FromKeyedServices("DataFiles")] IFileProvider dataFiles)
    {
        _dataFiles = dataFiles;
        _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    /// <summary>
    /// Returns the full HTML rendering of a markdown file, cached.
    /// </summary>
    /// <param name="path">Relative path within the Data file provider (e.g. "calcs/md/hp/50g/description.md")</param>
    /// <param name="fallback">Value to return if the file doesn't exist</param>
    public string? GetHtml(string path, string? fallback = null)
    {
        return _htmlCache.GetOrAdd(path, _ =>
        {
            var raw = ReadFile(path);
            return raw != null ? Markdown.ToHtml(raw, _pipeline) : fallback;
        });
    }

    /// <summary>
    /// Returns a plain-text snippet (first N characters) of a markdown file, cached.
    /// Strips all markdown formatting before truncating.
    /// </summary>
    /// <param name="path">Relative path within the Data file provider</param>
    /// <param name="maxLength">Maximum character length of the snippet</param>
    public string? GetSnippet(string path, int maxLength = 200)
    {
        var cacheKey = $"{path}::{maxLength}";
        return _snippetCache.GetOrAdd(cacheKey, _ =>
        {
            var raw = ReadFile(path);
            if (raw == null) return null;

            var cleanText = StripMarkdown(raw);

            if (cleanText.Length <= maxLength) return cleanText;

            // Find last space before the limit to avoid cutting mid-word
            var cutoff = cleanText.LastIndexOf(' ', maxLength);
            if (cutoff <= 0) cutoff = maxLength;
            return cleanText[..cutoff].Trim() + "…";
        });
    }

    private string? ReadFile(string path)
    {
        var fileInfo = _dataFiles.GetFileInfo(path);
        if (!fileInfo.Exists) return null;

        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string StripMarkdown(string markdown)
    {
        if (string.IsNullOrEmpty(markdown)) return "";

        // Strip markdown images: ![alt](url)
        var text = ImagePattern().Replace(markdown, "");

        // Replace markdown links [text](url) with just the text
        text = LinkPattern().Replace(text, "$1");

        // Remove inline formatting symbols like asterisks, underscores, backticks, hashes
        text = FormattingPattern().Replace(text, "");

        // Normalize spaces and newlines
        text = WhitespacePattern().Replace(text, " ").Trim();

        return text;
    }

    // Pre-compiled regex patterns via source generation
    [GeneratedRegex(@"!\[.*?\]\(.*?\)")]
    private static partial Regex ImagePattern();

    [GeneratedRegex(@"\[(.*?)\]\(.*?\)")]
    private static partial Regex LinkPattern();

    [GeneratedRegex(@"[\*_`#]")]
    private static partial Regex FormattingPattern();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
