using Microsoft.AspNetCore.Mvc.RazorPages;
using EvansWebpage.Models;
using EvansWebpage.Services;
using System.Collections.Generic;

namespace EvansWebpage.Pages
{
    public class HappeningsModel : PageModel
    {
        private readonly HappeningsService _happeningsService;

        // This property holds the list of posts for the .cshtml page to loop through
        public List<NewsPost> Posts { get; set; } = new();

        // Inject the service created in the previous step
        public HappeningsModel(HappeningsService happeningsService)
        {
            _happeningsService = happeningsService;
        }

        public void OnGet()
        {
            // Call the service to grab all posts, already sorted by date
            Posts = _happeningsService.GetAllPosts();
        }
    }
}