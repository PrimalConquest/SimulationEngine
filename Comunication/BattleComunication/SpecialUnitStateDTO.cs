namespace BattleComunication
{
    public class SpecialUnitStateDTO
    {
        public string Key            { get; set; } = "";
        public uint   Id             { get; set; }
        public int    X              { get; set; }
        public int    Y              { get; set; }
        public bool   IsOnBoard      { get; set; }
        public int    Energy         { get; set; }
        public int    MaxEnergy      { get; set; }
        public int    ActivationCost { get; set; }
        public bool   CanActivate    { get; set; }
    }
}
