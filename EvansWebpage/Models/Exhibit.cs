namespace EvansWebpage.Models
{
    public class Exhibit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerLogo { get; set; }
        public string MainImageUrl { get; set; }
        public string Model { get; set; }
        public int? YearIntroduced { get; set; }
        public string Type { get; set; }
        public string MyCalcsSearchId { get; set; } = null!;
        public string ManufacturerSlug { get; set; }
        public string ModelSlug { get; set; }
        public List<Specimen> Specimens { get; set; } = new List<Specimen>();
    }

    public class Specimen
    {
        public int Number { get; set; }
        public string Variant { get; set; }
        public string SerialNumber { get; set; }
        public string Condition { get; set; }
        public string ImageUrl { get; set; }
    }
}