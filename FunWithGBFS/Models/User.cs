using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public List<GameAttempt> Attempts { get; set; } = new();
    }
}
