using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Enums.EventTypes;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Events.Payloads;
using SimulationEngine.Source.Factories;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    public class Player
    {
        public uint Id { get; private set; }
        public Board Board { get; private set; }
        public Dictionary<uint, Unit> Commanders { get; private set; }
        public Dictionary<uint, Unit> Officers { get; private set; }
        public Dictionary<uint, Unit> Units { get; set; }

        public Player(uint id, Dictionary<string, Cell> commanderIds, Dictionary<uint, Unit> officers, Board board)
        {
            Id = id;
            Board = board;
            Commanders = new();
            Officers = officers;
            Units = new();

            /*oreach (KeyValuePair<string, Cell> commanderId in commanderIds)
            {
                KeyValuePair<uint, Unit>? commander = SpawnUnit(commanderId.Key);
                if (commander == null) { continue; }
                commander.Value.Value.Position = commanderId.Value;
                Commanders.Add(commander.Value.Key, commander.Value.Value);

                Board.Set(commanderId.Value, commander.Value.Key);
            }*/

        }

        bool CanPlaceUnit(Shape unitShape, Cell position)
        {
            if(Board.Get(position) != 0) return false;

            if (unitShape.Offsets == null) return true;

            foreach(Cell extend in unitShape.Offsets)
            {
                if (Board.Get(position) != 0) return false;
            }

            return true;
        }

        /*public void PlaceOfficer(int officerIndex, Cell position)
        {
            if(officerIndex >= OfficerIds.Count)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"Player.SpawnOfficer - officer index '{officerIndex}' out of bounds");
                return;
            }
            string officerId = OfficerIds[officerIndex];
            KeyValuePair<uint, Unit>? officer = SpawnUnit(officerId);
            if(officer == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"Player.SpawnOfficer - cannot spawn officer with id '{officerId}'");
                return;
            }
            PlaceUnit(officer.Value, position);
        }

        void PlaceUnit(KeyValuePair<uint, Unit> unit, Cell position)
        {
            Shape shape = unit.Value.Ocupation;
            if(!CanPlaceUnit(shape, position))
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, $"Player.PlaceUnit - cannot place unit at {position.ToString()}");
                return;
            }

            Board.Set(position, unit.Key);
            if (shape.Offsets == null) return;
            foreach (Cell extend in shape.Offsets)
            {
                Board.Set(extend, unit.Key);
            }
            unit.Value.UnitEventBus.Raise(EUnitEvent.Draft, new DraftPayload(position));
        }

        KeyValuePair<uint, Unit>? SpawnUnit(string unitId)
        {
            Unit? unit = UnitFactory.GetUnit(unitId, this);
            if (unit == null) 
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Error, $"ShapeFactory:GetShape Player[{Id}] cannot spawn with id: {unitId}");
                return null;
            }
            uint simId = SimulationSystem.NextId();
            return new KeyValuePair<uint, Unit>( simId, unit );
        }*/
    }
}
