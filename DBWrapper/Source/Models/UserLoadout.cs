using System.ComponentModel.DataAnnotations.Schema;

namespace DBWrapper.Source.Models
{
    public class UserLoadout
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;

        public string CommanderId { get; set; } = "";
        public string OfficerIdsRaw { get; set; } = "";

        [NotMapped]
        public List<string> OfficerIds
        {
            get => string.IsNullOrWhiteSpace(OfficerIdsRaw)
                   ? new List<string>()
                   : OfficerIdsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            set => OfficerIdsRaw = string.Join(',', value);
        }
    }
}
