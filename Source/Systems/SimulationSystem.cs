using Newtonsoft.Json.Linq;
using SimulationEngine.Source.Data.Commands;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Enums.Stats;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Logistic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SimulationEngine.Source.Systems
{
    internal static class SimulationSystem
    {
       

        private static string TroopId = "Troop";

        private static Random _rng = new Random();

        private static uint _currentId = 0;

        public static Game ActiveGame { get; private set; }

        public static HashSet<Cell> CheckForMatchPositions { get; set; }

        public static Dictionary<Cell, HashSet<Cell>> PositionsToActivate { get; private set; }

        public static HashSet<int> RefillColumnsIndexes { get; private set; }

        // Enemy special units that need gravity applied after an attack this turn
        public static List<uint> HitEnemySpecialUnits { get; private set; }

        public static int Seed
        {
            get;
            set
            {
                field = value;
                _rng = new Random(field);
            }
        }

        public static int RandomInt() => _rng.Next();
        public static uint NextId() => ++_currentId;

        public static void Init(int seed, uint currentId, Game game)
        {
            Seed = seed;
            _currentId = currentId;
            ActiveGame = game;
        }

        static SimulationSystem()
        {
            Seed = new Random().Next();
            ActiveGame = new(0, new Cell { x = 0, y = 0 });
            CheckForMatchPositions = new();
            PositionsToActivate = new();
            RefillColumnsIndexes = new();
            HitEnemySpecialUnits = new();
        }

        public static Unit? SpawnUnit(string unitId, Player owner)
        {
            Unit? unit = UnitFactory.GetUnit(unitId, owner);
            if (unit == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"SimulationSystem.SpawnUnit Cannot spawn with id: {unitId} for player[{owner.Id}]");
                return null;
            }
            unit.Id = NextId();
            return unit;
        }


        public static void CheckStateChain()
        {
            while (true)
            {
                if (CheckForMatchPositions.Count > 0)
                {
                    CheckForMatches();
                    continue;
                }
                if (PositionsToActivate.Count > 0)
                {
                    MatchPositions();
                    continue;
                }
                if (RefillColumnsIndexes.Count > 0)
                {
                    RefillBoard();
                    continue;
                }
                break;
            }
            ApplyEnemyGravity();
        }

        private static Dictionary<Unit, (uint column, Move move)>? MoveStep(Board board, Unit movingUnit, Move move, bool restrictToEmpty = false)
        {
            if (!movingUnit.CanMove)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"SimulationSystem.MoveStep - Trying to move unit {movingUnit.Position} that cannot move");
                return null;
            }
            var bounds = movingUnit.Ocupation.GetWall(move.Direction);
            Cell anchor = movingUnit.Position;
            Cell extend = movingUnit.Ocupation.Extend;
            Cell unitExtend = anchor + extend;

            HashSet<Unit> baseUnits = board.GetAllInBoundsInclusive(anchor, unitExtend + new Cell(-1,-1));
            if (!baseUnits.Contains(movingUnit))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"SimulationSystem.MoveStep - Trying to move unit {movingUnit.Position} that doesnt contain itself");
                return null;
            }

            Cell movementVector = move.Direction.ToVector();
            Cell wallStart = anchor + bounds.startOffset + movementVector;
            Cell wallEnd = anchor + bounds.endOffset + movementVector;
            Cell wallStep = bounds.step;

            uint unitRelativeSize = (extend * movementVector).MagnitudeAbs();
            uint actualMoveStep = move.Step;

            int boardLenght = (int)(board.Extend * movementVector).MagnitudeAbs();
            Dictionary<Unit, (uint column, Move move)> moveResult = new();

            for (uint i = 0; i < actualMoveStep; i++)
            {
                for (Cell it = wallStart; it != wallEnd + wallStep; it += wallStep)
                {
                    Cell next = (it + movementVector * i);
                    if (!board.IsInBounds(next)) return null;

                    Unit? currentUnit = board.Get(next);

                    if(restrictToEmpty && currentUnit != null)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"SimulationSystem.MoveStep - Trying to move unit {movingUnit.Position} but cell is not empty while restriction is up");
                        return null;
                    }

                    if (currentUnit == null || baseUnits.Contains(currentUnit) || moveResult.ContainsKey(currentUnit)) continue;

                    if (!currentUnit.CanMove)
                    {
                        LogSystem.Log(ELogCategory.Debug, ELogLevel.Display, $"SimulationSystem.MoveStep - Trying to move unit {movingUnit.Position} but unit {currentUnit.Position} cannot move");
                        return null;
                    }

                    moveResult.Add(currentUnit, ((next*movementVector).MagnitudeAbs(), new(unitRelativeSize , move.Direction.Opposite())));

                    uint currentUnitExtend = i + (currentUnit.Ocupation.Extend * movementVector).MagnitudeAbs();

                    if (currentUnitExtend > actualMoveStep) actualMoveStep = currentUnitExtend;
                }
            }

            Cell finalDestination = anchor + movementVector * actualMoveStep;

            if(!board.RemoveUnit(anchor, movingUnit)) return null;
            if (!board.PlaceUnit(finalDestination, movingUnit)) return null;

            return moveResult;
        }

        // GattherMoveStack and ApplyMoveStack are left as-is — not yet updated.
        public static MoveStack? GattherMoveStack(Board board, Unit initialUnit, EDirection initialDirection)
        {
            MoveStack moveStack = new();

            Dictionary<EDirection, List<List<(Unit, Move)>>> moveQueues = new();
            int boardLenght = (int)(board.Extend * initialDirection.ToVector()).MagnitudeAbs();
            moveQueues.Add(initialDirection, new());
            moveQueues.Add(initialDirection.Opposite(), new(boardLenght));

            foreach (var dir in moveQueues)
            {
                for (int i = 0; i < boardLenght; i++)
                {
                    List<(Unit, Move)> column = new();
                    moveQueues[dir.Key].Add(column);
                }
            }

            moveQueues[initialDirection][0].Add((initialUnit,new(1,initialDirection)));

            int timeSkipped = 0;
            for (EDirection direction = initialDirection; timeSkipped < moveQueues.Count; direction = direction.Opposite())
            {
                List<List<(Unit, Move)>> moveQueue = moveQueues[direction];

                int moveDir = direction.ToVector().Magnitude(); 

                (int start, int end) = (moveDir == -1) ? (0, boardLenght-1) : (boardLenght-1, 0);
                int step = 0-moveDir;
                List<(Unit, Move)>? currentMoveList = null;
                for (int i = start; i != end+step; i += step)
                {
                    if(moveQueue[i].Count > 0)
                    {
                        currentMoveList = new(moveQueue[i]);
                        moveQueue[i] = new();
                        break;
                    }
                }

                if (currentMoveList == null)
                {
                    timeSkipped++;
                    continue;
                }
                timeSkipped = 0;
                moveStack.NextTimeStep();

                Dictionary<Unit, (uint column, Move move)> collectiveMoveResult = new();
                foreach ((Unit unit, Move move) moveEntry in currentMoveList)
                {
                    Cell prevPos = moveEntry.unit.Position;
                    Dictionary<Unit, (uint column, Move move)>? moveResult = MoveStep(board, moveEntry.unit, moveEntry.move);
                    if (moveResult == null) return null;

                    moveStack.AddMoveInCurrentTimeStep(moveEntry.unit, prevPos, moveEntry.unit.Position);

                    foreach (KeyValuePair<Unit, (uint column, Move move)> entry in moveResult)
                    {
                        if(!collectiveMoveResult.ContainsKey(entry.Key)) collectiveMoveResult.Add(entry.Key, entry.Value);

                        (uint column, Move move) currentEntry = collectiveMoveResult[entry.Key];
                        if (currentEntry.column != entry.Value.column) return null;
                        Move currentMove = currentEntry.move;
                        Move move = entry.Value.move;
                        if (currentMove.Direction != move.Direction) return null;
                        uint realMovestep = (currentMove.Step > move.Step) ? currentMove.Step : move.Step;

                        collectiveMoveResult[entry.Key] = (currentEntry.column, new(realMovestep, move.Direction));
                    }
                }

                foreach (KeyValuePair<Unit, (uint column, Move move)> entry in collectiveMoveResult)
                {
                    moveQueues[entry.Value.move.Direction][(int)entry.Value.column].Add((entry.Key, entry.Value.move));
                }

            }

            return moveStack;
        }


        private static void CheckForMatches()
        {
            Player player = ActiveGame.CurrentPlayer;
            Board board = player.Board;

            List<Cell> alreadyMatched = new();
            List<Cell> toCheck = new(CheckForMatchPositions);
            CheckForMatchPositions.Clear();

            Cell[] cardinals = {
                Cell.GetMoveDirection(EDirection.Up),
                Cell.GetMoveDirection(EDirection.Down),
                Cell.GetMoveDirection(EDirection.Left),
                Cell.GetMoveDirection(EDirection.Right)
            };

            (int, int)[] diagPairs = { (0, 2), (0, 3), (1, 2), (1, 3) };

            foreach (Cell origin in toCheck)
            {
                if (alreadyMatched.Contains(origin)) continue;
                if (!board.IsInBounds(origin)) continue;

                Unit? originUnit = board.Get(origin);
                if (originUnit == null) continue;

                EColor color = originUnit.Color;

                HashSet<Cell>[] arms = new HashSet<Cell>[4];
                for (int i = 0; i < 4; i++)
                {
                    arms[i] = new();
                    Cell cursor = origin + cardinals[i];
                    while (board.IsInBounds(cursor))
                    {
                        Unit? u = board.Get(cursor);
                        if (u == null || u.Color != color) break;
                        arms[i].Add(cursor);
                        cursor = cursor + cardinals[i];
                    }
                }

                KeyValuePair<Cell, HashSet<Cell>> group = new(origin, new());

                foreach ((int a, int b) in diagPairs)
                {
                    if (arms[a].Count == 0 || arms[b].Count == 0) continue;
                    Cell diag = origin + cardinals[a] + cardinals[b];
                    if (!board.IsInBounds(diag)) continue;
                    Unit? diagUnit = board.Get(diag);
                    if (diagUnit == null || diagUnit.Color != color) continue;
                    group.Value.Add(diag);
                    group.Value.Add(origin + cardinals[a]);
                    group.Value.Add(origin + cardinals[b]);
                }

                if (arms[0].Count + arms[1].Count > 1)
                {
                    foreach (Cell c in arms[0]) group.Value.Add(c);
                    foreach (Cell c in arms[1]) group.Value.Add(c);
                }
                if (arms[2].Count + arms[3].Count > 1)
                {
                    foreach (Cell c in arms[2]) group.Value.Add(c);
                    foreach (Cell c in arms[3]) group.Value.Add(c);
                }

                if (group.Value.Count < 2) continue;

                alreadyMatched.Add(group.Key);
                alreadyMatched.AddRange(group.Value);

                PositionsToActivate.Add(group.Key, group.Value);
            }
        }

        private static void MatchPositions()
        {
            Player player = ActiveGame.CurrentPlayer;
            Board board = player.Board;

            HashSet<Unit> activated = new();
            Dictionary<Cell, HashSet<Cell>> groups = new(PositionsToActivate);
            PositionsToActivate.Clear();

            foreach (KeyValuePair<Cell, HashSet<Cell>> group in groups)
            {
                foreach (Cell cell in group.Value)
                {
                    Unit? unit = board.Get(cell);
                    if (unit == null || activated.Contains(unit)) continue;

                    unit.UnitEventBus.Raise(EUnitEvent.TryActivate, new EventPayload());
                    activated.Add(unit);
                }

                Unit? originUnit = board.Get(group.Key);
                if (originUnit == null || activated.Contains(originUnit)) continue;

                if (group.Value.Count > 2)
                    originUnit.UnitEventBus.Raise(EUnitEvent.Promote, new ValuePayload<int>(group.Value.Count - 3));
                else
                    originUnit.UnitEventBus.Raise(EUnitEvent.Activate, new EventPayload());

                activated.Add(originUnit);
            }
        }

        public static void SetupBoard(Player owner)
        {
            Board board = owner.Board;
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    Cell pos = new Cell { x = i, y = j };
                    Unit? newTroop = SpawnUnit(TroopId, owner);
                    if (newTroop == null) continue;
                    owner.BoardUnits.Add(newTroop);
                    newTroop.Position = pos;
                    board.Set(pos, newTroop);
                }
            }
        }

        private static void RefillBoard()
        {
            Player player = ActiveGame.CurrentPlayer;
            Board board = player.Board;

            HashSet<int> columns = new(RefillColumnsIndexes);
            RefillColumnsIndexes.Clear();

            foreach (int col in columns)
            {
                List<(Cell prevPos, Unit unit)> units = new();
                for (int y = 0; y < board.Height; y++)
                {
                    Cell pos = new Cell { x = col, y = y };
                    Unit? fallingUnit = board.Get(pos);
                    if (fallingUnit == null) continue;
                    if (!fallingUnit.CanFall) continue;

                    units.Add((pos, fallingUnit));
                    for (int i = 0; i < fallingUnit.Ocupation.Height; i++)
                        board.Set(new Cell { x = pos.x, y = pos.y + i }, null);
                }

                for (int y = 0; y < board.Height; y++)
                {
                    Cell pos = new Cell { x = col, y = y };
                    if (board.Get(pos) != null) continue;

                    CheckForMatchPositions.Add(pos);

                    // may be slow to remove from the start since rearranging the whole list
                    if (units.Count > 0)
                    {
                        (Cell prevPos, Unit unit) = units[0];
                        board.Set(pos, unit);
                        unit.UnitEventBus.Raise(EUnitEvent.Fall, new ValueChangedPayload<Cell>(pos, prevPos));
                        units.RemoveAt(0);
                        continue;
                    }

                    Unit? newTroop = SpawnUnit(TroopId, player);
                    if (newTroop == null) continue;
                    player.BoardUnits.Add(newTroop);
                    newTroop.Position = pos;
                    board.Set(pos, newTroop);
                }
            }
        }

        private static void ApplyEnemyGravity()
        {
            /*Player enemy = ActiveGame.OtherPlayer;
            Board board = enemy.Board;

            List<uint> units = new(HitEnemySpecialUnits);
            HitEnemySpecialUnits.Clear();

            units.RemoveAll(id => !enemy.BoardUnits.Contains(id));

            // Process units closest to y=0 first so they don't block units behind them.
            units = units.OrderBy(u =>
            {
                enemy.BoardUnits.TryGetValue(u, out Unit unit);
                return unit.Position.y;
            }).ToList();

            foreach (uint unitId in units)
            {
                if (!GattherMoveStack(board, enemy.BoardUnits, unitId, EDirection.Down, 1, moveStack, false))
                    continue;
 
                ApplyMoveStack(board, moveStack);
            }*/
        }
    }
}