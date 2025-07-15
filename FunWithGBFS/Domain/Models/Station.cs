namespace FunWithGBFS.Core.Models
{
    public class Station
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int BikesAvailable { get; set; }

        public string City { get; set; } = default!;
    }
}
