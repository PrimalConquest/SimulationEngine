using Newtonsoft.Json.Linq;

namespace SimulationEngine.Source.Helpers.Abilities
{
    internal class AbilityData
    {
        public string  Class       { get; set; } = "";
        public int     Priority    { get; set; } = 5;
        public string  TargetingId { get; set; } = "";
        public JObject Specific    { get; set; } = new();
    }
}
