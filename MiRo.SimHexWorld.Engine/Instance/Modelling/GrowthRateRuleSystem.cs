using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class GrowthRateRuleSystem : AbstractRuleSystem
    {
        public static double FoodPerCitizen = 1;

        public GrowthRateRuleSystem(ModellingTile parent, double currentValue)
            : base(parent, RuleType.GrowthRate, currentValue)
        {
            _effects.Add(RuleType.Pollution);
        }

        public override void UpdateTarget()
        {
            double pollutionPenalty = (Math.Pow(1.02, _parent.Pollution) - 1.0);
            double starvationPenalty = _parent.Citizen * FoodPerCitizen - _parent.Food < 0 ? 0.1 : 0.0;
            _targetValue = Clamp(1.1 - pollutionPenalty - starvationPenalty - _parent.Immigration, 0.5, 1.5);
        }
    }
}
