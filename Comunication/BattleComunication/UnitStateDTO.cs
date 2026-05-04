namespace BattleComunication
{
    public class UnitStateDTO
    {
        public uint Id      { get; set; }
        public int  X       { get; set; }
        public int  Y       { get; set; }
        public int  Color   { get; set; }  // EColor cast to int
        public int  Health  { get; set; }
        public bool CanMove { get; set; }
    }
}
