using EvansWebpage.Models;
using EvansWebpage.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvansWebpage.Pages.Calculators;

public class DirectoryModel : PageModel
{
    private readonly ExhibitService _exhibitService;

    public DirectoryModel(ExhibitService exhibitService)
    {
        _exhibitService = exhibitService;
    }

    // Data structure: Category -> Manufacturer -> List of Exhibits
    public Dictionary<string, Dictionary<string, List<Exhibit>>> DirectoryData { get; set; } = new();

    public void OnGet()
    {
        DirectoryData = _exhibitService.GetGroupedDirectory();
    }
}
