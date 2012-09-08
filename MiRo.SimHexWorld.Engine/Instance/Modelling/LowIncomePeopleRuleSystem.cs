using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class LowIncomePeopleRuleSystem : AbstractRuleSystem
    {
        public LowIncomePeopleRuleSystem(ModellingTile modellingTile)
            : base(modellingTile, RuleType.LowIncomePeople, 0.3)
        {
        }

        public override void UpdateTarget()
        {
            _targetValue = Clamp( 0.3 + 3.0 * _parent.Poverty, 0, 1);
        }
    }
}
