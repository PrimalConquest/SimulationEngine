using SimulationEngine.Source.Data.Units;
using SimulationEngine.Source.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Logistic
{
    internal class Player
    {
        uint _id;
        Unit? _commander;
        //Board _board;

        public Player(uint id, string commanderId)
        {
            _id = id;
            _commander = UnitFactory.GetUnit(commanderId);
        }
    }
}
