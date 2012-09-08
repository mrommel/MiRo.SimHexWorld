using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class PollutionRuleSystem : AbstractRuleSystem
    {
        double maxPopulation = 1000.0;

        public PollutionRuleSystem(ModellingTile parent, double currentValue)
            : base(parent, RuleType.Pollution, currentValue)
        {
            _effects.Add(RuleType.Citizen);
        }

        public override void UpdateTarget()
        {
            _targetValue = Math.Max(_parent.Citizen - maxPopulation, 0) * 0.1;
        }
    }
}
