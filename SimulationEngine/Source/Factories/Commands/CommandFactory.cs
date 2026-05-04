using SimulationEngine.Source.Data.Commands;
using SimulationEngine.Source.Factories.Commands.CommandInfos;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Factories.Commands
{
    public static class CommandFactory
    {
        public static IGameCommand? Get(Player player, ICommandInfo commandInfo) => commandInfo switch
        {
            MoveCommandInfo            m => new MoveCommand(player, m),
            ActivateSpecialCommandInfo a => new ActivateSpecial(player, a),
            PlaceSpecialCommandInfo    p => new PlaceSpecialUnit(player, p),
            EndTurnCommandInfo         e => new EndTurn(player, e),
            _ => null
        };
    }
}
