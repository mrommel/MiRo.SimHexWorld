using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class HighIncomePeopleRuleSystem : AbstractRuleSystem
    {
        public HighIncomePeopleRuleSystem(ModellingTile tile)
            : base(tile,RuleType.HighIncomePeople,0.2)
        { 
        }

        public override void UpdateTarget()
        {
            _targetValue = Clamp( 0.2, 0, 1);
        }
    }
}
