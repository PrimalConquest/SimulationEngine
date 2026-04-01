using SimulationEngine.Source.Data.Stats;
using SimulationEngine.Source.Enums.Logging;
using SimulationEngine.Source.Interfaces;
using SimulationEngine.Source.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationEngine.Source.Data.Effects
{
    internal abstract class Effect : IAppliable
    {
        public void Apply(object source, object target)
        {
            StatSheet sourceSheet = (StatSheet)source;
            StatSheet targetSheet = (StatSheet)target;

            if (sourceSheet == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, "Effect:Apply - source is not StatSheet");
                return;
            }
            if(targetSheet == null)
            {
                LogSystem.Log(ELogCategory.Debug, ELogLevel.Warning, "Effect:Apply - target is not StatSheet");
                return;
            }

            OnApply(sourceSheet, targetSheet);
        }

        public abstract void OnApply(StatSheet source, StatSheet target);

    }
}
