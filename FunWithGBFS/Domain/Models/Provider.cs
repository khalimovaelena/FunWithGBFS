using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Core.Models
{
    public class Provider
    {
        public string Name { get; set; }
        public string SourceUrl { get; set; } // Not Gbfs-specific
        public string Type { get; set; } = "gbfs"; // Optional, allows future switching
    }
}
