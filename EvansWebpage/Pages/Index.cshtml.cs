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

        public async Task OnGetAsync()
        {
            // Fetch all posts, take the top 3, and save to the RecentPosts list
            var allPosts = await _happeningsService.GetAllPostsAsync();
            RecentPosts = allPosts.Take(3).ToList();
        }
    }
}