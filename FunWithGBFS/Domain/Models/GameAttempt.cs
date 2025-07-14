using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Core.Models
{
    public class GameAttempt
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Score { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
