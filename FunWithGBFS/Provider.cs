using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS
{
    public class Provider
    {
        public string City { get; set; }
        public string GbfsUrl { get; set; }

        public Provider(string city, string gbfsUrl)
        {
            City = city;
            GbfsUrl = gbfsUrl;
        }
    }
}
