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
        bool _isCommander;
        uint _unitId;
        Cell _pos;

        public PlaceSpecialUnit(Player player, uint unitId, Cell pos)
        {
            _player = player;
            _isCommander = unitId == player.CommanderId;
            _unitId = unitId;
            _pos = pos;
        }

        public bool CanExecute()
        {
            if (SimulationSystem.ActiveGame.CurrentPlayer != _player) return false;
            if (_player.BoardUnits.ContainsKey(_unitId)) return false;
            if (!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return false;
            if (!unit.CanBeDrafted) return false;

            Board board = _player.Board;

            for(int x = 0; x<unit.Ocupation.Width; x++)
            {
                for (int y = 0; y < unit.Ocupation.Height; y++)
                {
                    Cell cell = _pos + new Cell { x=x, y=y};
                    if (!board.IsInBounds(cell)) return false;

                    uint occupantId = board.Get(cell);
                    if (occupantId == 0) continue;

                    if (!_player.BoardUnits.TryGetValue(occupantId, out Unit occupant) || !occupant.CanBeOverriden)
                        return false;
                }
            }

            return true;
        }

        public void Execute()
        {
            if (!_player.SpecialUnits.TryGetValue(_unitId, out Unit unit)) return;

            unit.Position = _pos;
            Board board = _player.Board;

            for (int x = 0; x < unit.Ocupation.Width; x++)
            {
                for (int y = 0; y < unit.Ocupation.Height; y++)
                {
                    Cell cell = _pos + new Cell { x = x, y = y };

                    uint overriddenId = _player.Board.Get(cell);

                    _player.Board.Set(cell, _unitId);

                    if (_player.BoardUnits.TryGetValue(overriddenId, out Unit overridden))
                    {
                        overridden.UnitEventBus.Raise(EUnitEvent.Override, new EventPayload());
                        _player.BoardUnits.Remove(overriddenId);
                    }
                }
            }

            _player.BoardUnits.Add(_unitId, unit);
            unit.UnitEventBus.Raise(EUnitEvent.Draft, new DraftPayload(_pos));

            SimulationSystem.CheckStateChain();
        }
    }
}
