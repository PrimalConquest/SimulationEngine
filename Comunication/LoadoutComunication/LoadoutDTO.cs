using System;
using System.Collections.Generic;
using System.Text;

namespace LoadoutComunication
{
    public class LoadoutDTO
    {
        public int PlayerId { get; set; }
        public string CommanderId { get; set; } = "";
        public List<string> OfficerIds{ get; set; } = new();
    }
}
