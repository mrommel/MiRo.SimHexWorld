using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class CitizenRuleSystem : AbstractRuleSystem
    {
        public CitizenRuleSystem(ModellingTile parent, int currentValue)
            : base(parent, RuleType.Citizen, currentValue)
        {
            _effects.Add(RuleType.GrowthRate);
        }

        public override void UpdateTarget()
        {
            _targetValue = _currentValue * _parent.Growth;
        }
    }
}
