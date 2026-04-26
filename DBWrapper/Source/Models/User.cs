using Microsoft.AspNetCore.Identity;

namespace DBWrapper.Source.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = true;

        public UserLoadout? Loadout { get; set; }
        public UserStats? Stats { get; set; }
    }
}
