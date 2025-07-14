using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Core.Models
{
    //TODO: move to Models folder + separate classes into their own files
    public class Station
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int BikesAvailable { get; set; }
    }

    // Station Info (static data)
    public class StationInfoRoot
    {
        public StationInfoData data { get; set; }
    }

    public class StationInfoData
    {
        public List<StationInfo> stations { get; set; }
    }

    public class StationInfo
    {
        public string station_id { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
    }

    // Station Status (dynamic data)
    public class StationStatusRoot
    {
        public StationStatusData data { get; set; }
    }

    public class StationStatusData
    {
        public List<StationStatus> stations { get; set; }
    }

    public class StationStatus
    {
        public string station_id { get; set; }
        public int num_bikes_available { get; set; }
    }
}
