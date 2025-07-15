namespace FunWithGBFS.Domain.Models
{
    public class Vehicle
    {
        public string Id { get; set; } = default!;
        public string ProviderName { get; set; } = default!;

        public string Type { get; set; } = default!;
        public string City { get; set; } = default!;
        public bool IsReserved { get; set; }
        public bool IsDisabled { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
