using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using SimulationEngine.Source.Systems;

namespace SimulationEngine.Source.Data.Commands
{
    public class MoveCommand : IGameCommand
    {
        Player _player;
        Unit? _movingUnit;
        EDirection _direction;
        int _moveCost;
        int _maxRepetitions;
        MoveStack? moveStack;


        public MoveCommand(Player player, Cell pos, EDirection direction)
        {
            _player = player;
            _movingUnit = _player.Board.Get(pos);
            _direction = (direction);
            if (_movingUnit == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error,
                    $"Move - Trying to move pos[x:{pos.x}, y:{pos.y}] unit[{_movingUnit}] that is not on the board");
                return;
            }
            //_maxRepetitions = unit.GetStat(EStat.MoveSpeed);
            _maxRepetitions = 1;//////////
            //_moveCost = unit.GetStat(EStat.MoveCost);
        }

        public bool CanExecute()
        {
            //if (SimulationSystem.ActiveGame.CurrentPlayer != _player) return false;
            //if (_player.CurrentMoves < _moveCost) return false;

            //_player.BoardUnits.TryGetValue(_movingUnit, out Unit unit);
            if (_movingUnit == null) return false;
            //if (!unit.CanMove) return false;

            Dictionary<Unit, Cell> tempPositions = new();

            foreach (KeyValuePair<uint, Unit> _unit in _player.BoardUnits)
            {
                tempPositions.Add(_unit.Value, _unit.Value.Position);
            }

            MoveStack? moveStack = SimulationSystem.GattherMoveStack(_player.Board, _movingUnit, _direction);
            
            if(moveStack != null)
            {
                Console.WriteLine($"Move stack: \n{moveStack}");
                return true;
            }
            Console.WriteLine("Cannot move");
            return false;
        }

        public void Execute()
        {


            var boardSnapshot = _player.Board.SnapshotPositions();

            //_player.CurrentMoves -= _moveCost;
            _moveCost = 0;

            //_player.BoardUnits.TryGetValue(_movingUnit, out Unit unit);
            //if (unit == null) return;

            //SimulationSystem.ApplyMoveStack(_player.Board, moveStack);

            _maxRepetitions -= 1;

            if (_maxRepetitions > 0 && CanExecute()) Execute();
            else SimulationSystem.CheckStateChain();
        }
    }
}
