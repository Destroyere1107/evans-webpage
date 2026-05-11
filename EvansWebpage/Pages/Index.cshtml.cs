using Microsoft.AspNetCore.Mvc.RazorPages;
using EvansWebpage.Models;
using EvansWebpage.Services;

namespace EvansWebpage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HappeningsService _happeningsService;
        
        public List<NewsPost> RecentPosts { get; set; } = new();

        public IndexModel(HappeningsService happeningsService)
        {
            _happeningsService = happeningsService;
        }

        public void OnGet()
        {
            // Fetch all posts, take the top 3, and save to the RecentPosts list
            RecentPosts = _happeningsService.GetAllPosts().Take(3).ToList();
        }
    }
}