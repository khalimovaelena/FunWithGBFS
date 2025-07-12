using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Models
{
    public class Provider
    {
        public string Name { get; set; }
        public string GbfsUrl { get; set; }

        public Provider(string name, string gbfsUrl)
        {
            Name = name;
            GbfsUrl = gbfsUrl;
        }
    }
}
