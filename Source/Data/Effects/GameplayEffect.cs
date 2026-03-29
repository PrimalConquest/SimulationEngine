using SimulationEngine.Source.Data.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Effects
{
    internal abstract class GameplayEffect : IEffect
    {
        public void Apply(object instigator, object reciever)
        {
            StatSheet source = (StatSheet)instigator;
            StatSheet target = (StatSheet)reciever;
            if (source == null || target == null) return;
            OnApply(source, target);
        }

        public abstract void OnApply(StatSheet source, StatSheet target);

    }
}
