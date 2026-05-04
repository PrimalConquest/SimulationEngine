namespace BattleComunication
{
    public class PlayerStateDTO
    {
        public uint                  GameId       { get; set; }
        public string                Name         { get; set; } = "";
        public int                   CurrentMoves { get; set; }
        public UnitStateDTO[]        BoardUnits   { get; set; } = System.Array.Empty<UnitStateDTO>();
        public SpecialUnitStateDTO[] SpecialUnits { get; set; } = System.Array.Empty<SpecialUnitStateDTO>();
    }
}
