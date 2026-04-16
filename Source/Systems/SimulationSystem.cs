using SimulationEngine.Source.Data.Commands;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationEngine.Source.Systems
{
    internal static class SimulationSystem
    {

        private static string TroopId = "Troop";

        private static Random _rng = new Random();

        private static uint _currentId = 0;

        public static Game ActiveGame { get; private set; }

        public static HashSet<Cell> CheckForMatchPositions { get; set; }

        public static Dictionary<Cell,HashSet<Cell>> PositionsToActivate { get; private set; }

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
            ActiveGame = new(0, new Cell{ x=0,y=0});
            CheckForMatchPositions = new();
            PositionsToActivate = new();
            RefillColumnsIndexes = new();
            HitEnemySpecialUnits = new();
        }

        public static KeyValuePair<uint, Unit>? SpawnUnit(string unitId, Player owner)
        {
            Unit? unit = UnitFactory.GetUnit(unitId, owner);
            if (unit == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"SimulationSystem.SpawnUnit Cannot spawn with id: {unitId} for player[{owner.Id}]");
                return null;
            }
            uint simId = NextId();
            return new KeyValuePair<uint, Unit>(simId, unit);
        }


        public static void CheckStateChain()
        {
            while(true){
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

       


        public static bool GattherMoveStack(Board board, Dictionary<uint, Unit> boardUnits, uint unitId, EDirection direction, uint stepCount, 
            Dictionary<Unit, (EDirection, uint)> moveStack, bool shouldFreeFall = false)
        {

            Console.WriteLine($"TRIGGER: {unitId}");

            if (!boardUnits.TryGetValue(unitId, out Unit unit))
            {
                Console.WriteLine($"eeerrm {unitId}");
                return false;
            }

            if (moveStack.ContainsKey(unit))
            {
                if (moveStack[unit].Item1 != direction || moveStack[unit].Item2 >= stepCount)
                {
                    Console.WriteLine($"SHORT _ Move Stack: {moveStack.Count}");
                    return true;
                }
                moveStack[unit] = (direction,stepCount);
            }
            else
            {
                moveStack.Add(unit, (direction, stepCount));
            }

           

            Cell anchor = unit.Position;
            (Cell, Cell, Cell) bounds = unit.Ocupation.GetWall(direction);
            Cell start = anchor + bounds.Item1;
            Cell end = anchor + bounds.Item2;
            Cell step = bounds.Item3;
            Cell movementVector = direction.ToVector();

            for (Cell it = start; it != end+step; it+=step)
            {

                for(int i=1; i<stepCount+1; i++)
                {
                    if (!board.IsInBounds(it + movementVector*i))
                    {
                        moveStack.Clear();
                        Console.WriteLine($"babag {unitId}");
                        return false;
                    }

                    if (!GattherMoveStack(board, boardUnits, board.Get(it + movementVector*i), direction.Opposite(), (uint)(unit.Ocupation.Extend() * movementVector).MagnitudeAbs(), moveStack, shouldFreeFall))
                    {
                        moveStack.Clear();
                        Console.WriteLine($"fortnie {unitId}");
                        return false;
                    }

                }

            }
            Console.WriteLine($"Move Stack: {moveStack.Count}");

            return true;
        }

        public static void ApplyMoveStack(Board board, Dictionary<Unit, (EDirection, uint)> moveStack)
        {
            List<(Unit, Cell, Cell, EDirection, uint)> shapshot = new();
            foreach(KeyValuePair<Unit, (EDirection, uint)> move in moveStack)
            {

                shapshot.Add((move.Key, move.Key.Position, move.Key.Ocupation.Extend(), move.Value.Item1, move.Value.Item2));
            }


            foreach((Unit, Cell, Cell, EDirection, uint) move in shapshot)
            {
                uint unitId = board.Get(move.Item1.Position);

                move.Item1.Position = move.Item2 + move.Item4.ToVector() * (int)move.Item5;

                for (int x = 0; x < move.Item3.x; x++)
                {
                    for (int y = 0; y < move.Item3.y; y++)
                    {
                        board.Set(move.Item1.Position + new Cell { x = x, y = y }, unitId);
                    }
                }
            }

           
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


            // Each pair of perpendicular arms whose shared corner is a diagonal
            (int, int)[] diagPairs = { (0, 2), (0, 3), (1, 2), (1, 3) };

            foreach (Cell origin in toCheck)
            {
                if(alreadyMatched.Contains(origin)) continue;

                if (!board.IsInBounds(origin)) continue;

                uint originId = board.Get(origin);
                if (originId == 0) continue;
                if (!player.BoardUnits.TryGetValue(originId, out Unit originUnit)) continue;

                EColor color = originUnit.Color;

                // Scan each arm until a different color or empty cell is hit
                HashSet<Cell>[] arms = new HashSet<Cell>[4];
                for (int i = 0; i < 4; i++)
                {
                    arms[i] = new();
                    Cell cursor = origin + cardinals[i];
                    while (board.IsInBounds(cursor))
                    {
                        uint id = board.Get(cursor);
                        if (id == 0) break;
                        if (!player.BoardUnits.TryGetValue(id, out Unit u) || u.Color != color) break;
                        arms[i].Add(cursor);
                        cursor = cursor + cardinals[i];
                    }
                }

                KeyValuePair<Cell,HashSet<Cell>> group = new(origin, new());


                // Add the diagonal cell when both adjacent arms have at least one match
                foreach ((int a, int b) in diagPairs)
                {
                    if (arms[a].Count == 0 || arms[b].Count == 0) continue;
                    Cell diag = origin + cardinals[a] + cardinals[b];
                    if (!board.IsInBounds(diag)) continue;
                    uint diagId = board.Get(diag);
                    if (diagId == 0) continue;
                    if (!player.BoardUnits.TryGetValue(diagId, out Unit diagUnit) || diagUnit.Color != color) continue;
                    group.Value.Add(diag);
                    group.Value.Add(origin + cardinals[a]);
                    group.Value.Add(origin + cardinals[b]);
                }


                if (arms[0].Count + arms[1].Count > 1)
                {
                    foreach (Cell c in arms[0])
                        group.Value.Add(c);
                    foreach (Cell c in arms[1])
                        group.Value.Add(c);
                }
                if (arms[2].Count + arms[3].Count > 1)
                {
                    foreach (Cell c in arms[2])
                        group.Value.Add(c);
                    foreach (Cell c in arms[3])
                        group.Value.Add(c);
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

            HashSet<uint> activated = new HashSet<uint>();
            Dictionary<Cell, HashSet<Cell>> groups = new(PositionsToActivate);
            PositionsToActivate.Clear();

            foreach (KeyValuePair<Cell, HashSet<Cell>> group in groups)
            {
                uint id;
                Unit unit;
                foreach (Cell cell in group.Value)
                {
                    id = board.Get(cell);
                    if (id == 0 || activated.Contains(id)) continue;
                    if (!player.BoardUnits.TryGetValue(id, out unit)) continue;

                    unit.UnitEventBus.Raise(EUnitEvent.TryActivate, new EventPayload());
                    activated.Add(id);
                }

                id = board.Get(group.Key);
                if (id == 0 || activated.Contains(id)) continue;
                if (!player.BoardUnits.TryGetValue(id, out unit)) continue;

                if (group.Value.Count > 2)
                {
                    unit.UnitEventBus.Raise(EUnitEvent.Promote, new ValuePayload<int>(group.Value.Count - 3));
                }else
                {
                    unit.UnitEventBus.Raise(EUnitEvent.Promote, new EventPayload());
                }
                activated.Add(id);
            }
        }

        public static void SetupBoard(Player owner)
        {
            Board board = owner.Board;
            for(int i=0; i< board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    Cell pos = new Cell { x = i, y = j };
                    KeyValuePair<uint, Unit>? newTroop = SpawnUnit(TroopId, owner);
                    if (newTroop == null) continue;
                    owner.BoardUnits.Add(newTroop.Value.Key, newTroop.Value.Value);
                    newTroop.Value.Value.Position = pos;
                    board.Set(pos, newTroop.Value.Key);
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
                List<(uint id, Cell prevPos, Unit unit)> units = new();
                for (int y = 0; y < board.Height; y++)
                {
                    Cell pos = new Cell { x = col, y = y };
                    uint id = board.Get(pos);
                    if (id == 0) continue;

                    if (!player.BoardUnits.TryGetValue(id, out Unit fallingUnit)) continue;

                    if (!fallingUnit.CanFall) continue;

                    units.Add((id, pos, fallingUnit));
                    for(int i =0; i<fallingUnit.Ocupation.Height; i++)
                    { 
                        board.Set(new Cell {x=pos.x, y = pos.y+i }, 0);
                    }
                }

                for (int y = 0; y < board.Height; y++)
                {
                    Cell pos = new Cell { x = col, y = y };
                    if (board.Get(pos) != 0) continue;   

                    CheckForMatchPositions.Add(pos);

                    //may be slow to remiove from the start since rearanging the whole array
                    if (units.Count > 0)
                    {
                        (uint id, Cell prevPos, Unit unit) = units[0];
                        board.Set(pos, id);
                        unit.UnitEventBus.Raise(EUnitEvent.Fall, new ValueChangedPayload<Cell>(pos, prevPos));
                        units.RemoveAt(0);
                        continue;
                    }

                    KeyValuePair<uint, Unit>? newTroop = SpawnUnit(TroopId, player);
                    if (newTroop == null) continue;
                    player.BoardUnits.Add(newTroop.Value.Key, newTroop.Value.Value);
                    newTroop.Value.Value.Position = pos;
                    board.Set(pos, newTroop.Value.Key);
                }
            }
        }

        private static void ApplyEnemyGravity()
        {
            Player enemy = ActiveGame.OtherPlayer;
            Board board = enemy.Board;

            List<uint> units = new(HitEnemySpecialUnits);
            HitEnemySpecialUnits.Clear();

            //Cell down = Cell.GetMoveDirection(); 

            units.RemoveAll(id => !enemy.BoardUnits.ContainsKey(id));

            // Process units closest to y=0 first so they don't block units behind them.
            units = units.OrderBy(u =>
            {
                enemy.BoardUnits.TryGetValue(u, out Unit unit);
                
                return unit.Position.y;
            }).ToList();

            Dictionary<Unit, (EDirection, uint)> moveStack = new();

            foreach (uint unitId in units)
            {
                if (!GattherMoveStack(board, enemy.BoardUnits, unitId, EDirection.Down, 1, moveStack, false))
                    continue;

                ApplyMoveStack(board, moveStack);
            }
        }
    }
}
