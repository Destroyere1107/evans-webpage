namespace EvansWebpage.Models
{
    public class Exhibit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } 
        public string Manufacturer { get; set; }
        public string ManufacturerLogo { get; set; }
        public string ManufacturerSlug { get; set; }
        public string Model { get; set; }
        public string ModelSlug { get; set; }
        public string Type { get; set; }
        public int? YearIntroduced { get; set; }
        public string MainImageUrl { get; set; } 
        public bool UnderConstruction { get; set; }
        
        public List<MyCalcsLink> MyCalcsLinks { get; set; } = new List<MyCalcsLink>(); 
        public List<Specimen> Specimens { get; set; } = new List<Specimen>();
        public List<GalleryImage> Gallery { get; set; } = new List<GalleryImage>();
    }
    
    public class MyCalcsLink
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class GalleryImage
    {
        public string Url { get; set; }
        public string AltText { get; set; }
        public string Caption { get; set; }
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