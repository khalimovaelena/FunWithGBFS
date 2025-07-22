namespace FunWithGBFS.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public List<GameAttempt> Attempts { get; set; } = new();

        public int Level { get; set; } = 0;
    }
}
