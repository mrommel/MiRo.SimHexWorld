using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class AgricultureSystem : AbstractRuleSystem
    {
        public AgricultureSystem(ModellingTile modellingTile, double initialValue)
            : base(modellingTile, RuleType.AgricultureLevel, initialValue)
        {
            
        }

        public override void UpdateTarget()
        {
            
        }
    }
}
