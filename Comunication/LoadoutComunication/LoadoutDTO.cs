using System.Collections.Generic;

namespace LoadoutComunication
{
    public class LoadoutDTO
    {
        public string CommanderId { get; set; } = "";
        public List<string> OfficerIds { get; set; } = new();
    }
}
