namespace FunWithGBFS.Core.Models
{
    public class Provider
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string SourceUrl { get; set; } // Not Gbfs-specific
        public string Type { get; set; } = "gbfs"; // Optional, allows future switching
    }
}
