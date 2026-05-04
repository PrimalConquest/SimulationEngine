using BattleComunication;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Logistic;
using System.Linq;

namespace BattleServer.Source.Mappers
{
    public static class GameStateMapper
    {
        public static GameStateDTO From(Game game) => new()
        {
            ActivePlayer = game.ActivePlayer,
            Players      = game.Players.Select(kv => MapPlayer(kv.Key, kv.Value)).ToArray()
        };

        static PlayerStateDTO MapPlayer(Player p, string name) => new()
        {
            GameId       = p.Id,
            Name         = name,
            CurrentMoves = p.CurrentMoves,
            BoardUnits   = p.BoardUnits.Select(MapUnit).ToArray(),
            SpecialUnits = p.SpecialUnits.Select(kv => MapSpecial(kv.Key, kv.Value, p.Board)).ToArray()
        };

        static UnitStateDTO MapUnit(SimulationEngine.Source.Data.Units.Unit u) => new()
        {
            Id      = u.Id,
            X       = u.X,
            Y       = u.Y,
            Color   = (int)u.Color,
            Health  = SafeGetStat(u, EStat.Health),
            CanMove = u.CanMove
        };

        static SpecialUnitStateDTO MapSpecial(string key, SimulationEngine.Source.Data.Units.Unit u, SimulationEngine.Source.Logistic.Board board) => new()
        {
            Key            = key,
            Id             = u.Id,
            X              = u.X,
            Y              = u.Y,
            IsOnBoard      = board.Get(u.Position) == u,
            Energy         = SafeGetStat(u, EStat.Energy),
            MaxEnergy      = SafeGetStat(u, EStat.MaxEnergy),
            ActivationCost = SafeGetStat(u, EStat.ActivationCost),
            CanActivate    = u.CanActivate
        };

        static int SafeGetStat(SimulationEngine.Source.Data.Units.Unit u, EStat stat)
        {
            try   { return u.GetStat(stat); }
            catch { return 0; }
        }
    }
}
