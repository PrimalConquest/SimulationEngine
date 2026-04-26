namespace DBWrapper.Source.Models
{
    public class UserStats
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;

        public int RankPoints { get; set; } = 0;
    }
}
