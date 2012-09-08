using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class HappinessSystem : AbstractRuleSystem
    {
        public HappinessSystem(ModellingTile tile, double initialValue)
            : base(tile, RuleType.Happiness, initialValue)
        {
            _effects.Add(RuleType.Crime);
        }

        public override void UpdateTarget()
        {
            _targetValue = -0.13 * _parent.Crime;
        }
    }
}
