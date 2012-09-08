using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class EnergyCostSystem : AbstractRuleSystem
    {
        public EnergyCostSystem(ModellingTile tile)
            : base(tile,RuleType.EnergyCost,0)
        {
        }

        public override void UpdateTarget()
        {
            
        }
    }
}
