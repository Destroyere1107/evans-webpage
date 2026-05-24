using EvansWebpage.Models;

namespace EvansWebpage.Pages.Shared;

/// <summary>
/// View model passed to _ExhibitCardPartial to render a single featured exhibit card.
/// </summary>
public class ExhibitCardViewModel
{
    public Exhibit? Exhibit { get; set; }
    public string? Description { get; set; }
    public string BadgeText { get; set; } = "";
    public string BadgeClass { get; set; } = "";
    public string BadgeIcon { get; set; } = "";
}
