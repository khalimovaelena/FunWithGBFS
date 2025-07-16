namespace FunWithGBFS.Domain.Models
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
