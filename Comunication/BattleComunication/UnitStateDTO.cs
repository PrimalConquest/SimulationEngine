namespace BattleComunication
{
    public class UnitStateDTO
    {
        public uint Id         { get; set; }
        public int  X          { get; set; }
        public int  Y          { get; set; }
        public int  Color      { get; set; }
        public int  Health     { get; set; }
        public bool CanMove    { get; set; }
        public int  ShapeWidth  { get; set; } = 1;
        public int  ShapeHeight { get; set; } = 1;
    }
}
