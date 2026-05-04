namespace BattleComunication
{
    public class GameStateDTO
    {
        public int              ActivePlayer { get; set; }
        public PlayerStateDTO[] Players      { get; set; } = System.Array.Empty<PlayerStateDTO>();
    }
}
