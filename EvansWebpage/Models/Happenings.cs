namespace EvansWebpage.Models
{
    public class NewsPost
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string HtmlContent { get; set; } = string.Empty;
    }
}