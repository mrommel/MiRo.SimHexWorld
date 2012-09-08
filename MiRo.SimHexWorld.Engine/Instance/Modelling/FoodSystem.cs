using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class FoodSystem : AbstractRuleSystem
    {
        public FoodSystem(ModellingTile modellingTile, double initialValue)
            : base( modellingTile,RuleType.Food,initialValue)
        {
        }

        public override void UpdateTarget()
        {
            _targetValue = _parent.Productivity * 0.5 + _parent.AgricultureLevel;
        }
    }
}
