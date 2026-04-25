namespace DBWrapper.Source.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int RankPoints { get; set; }

        public string CommanderId { get; set; } = "";

        public string OfficerIdsRaw { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> OfficerIds
        {
            get => string.IsNullOrWhiteSpace(OfficerIdsRaw)
                   ? new List<string>()
                   : OfficerIdsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            set => OfficerIdsRaw = string.Join(',', value);
        }
    }
}
