using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;

namespace SimulationEngine.Source.Data.Commands
{
    public class PlaceSpecialUnit : IGameCommand
    {
        Player _player;
        string _unitId;
        public Cell Position { get; set; }

        public PlaceSpecialUnit(Player player, string unitId, Cell pos)
        {
            _player = player;
            _unitId = unitId;
            Position = pos;
        }

        public bool CanExecute()
        {
            if (SimulationSystem.ActiveGame.CurrentPlayer != _player) return false;

            if(!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return false;

            if (!unit.CanBeDrafted) return false;

            Board board = _player.Board;
            Cell unitExtend = Position + unit.Ocupation.Extend;
            board.IsInBounds(unitExtend + new Cell(-1, -1));

            for (int x = Position.x; x < unitExtend.x; x++)
            {
                for (int y = Position.y; y < unitExtend.y; y++)
                {
                    Unit? occupant = board.Get(new(x,y));
                    if (occupant == null) continue;
                    if (!occupant.CanBeOverriden) return false;
                }
            }

            return true;
        }

        public void Execute()
        {
            if (!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return;
            Board board = _player.Board;
            Cell unitExtend = Position + unit.Ocupation.Extend;

            HashSet<Unit> signaled = new();

            for (int x = Position.x; x < unitExtend.x; x++)
            {
                for (int y = Position.y; y < unitExtend.y; y++)
                {
                    Unit? overridden = board.Get(new(x, y));
                    if (overridden == null) continue;
                    if (signaled.Contains(overridden)) continue;

                    overridden.UnitEventBus.Raise(EUnitEvent.Override, new EventPayload());
                    signaled.Add(overridden);
                }
            }

            board.PlaceUnit(Position, unit);

            unit.UnitEventBus.Raise(EUnitEvent.Draft, new DraftPayload(Position));

            SimulationSystem.CheckStateChain();
        }
    }
}