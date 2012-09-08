using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class ProductivitySystem : AbstractRuleSystem
    {
        public ProductivitySystem(ModellingTile parent, double currentValue)
            : base(parent, RuleType.Productivity, currentValue)
        {
        }

        public override void UpdateTarget()
            {
                //_targetValue = Clamp( , -0.3, 0.3);
            }
    }
}
