using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class CrimeSystem : AbstractRuleSystem
    {
        public CrimeSystem(ModellingTile modellingTile, double initialValue)
            : base(modellingTile, RuleType.Crime, initialValue)
        {
            _description = "An indicator of the level of general non violent crime in your nation. This includes crimes such as car crime, burglary etc, but also covers fraud and other similar crimes";
            _effects.Add(RuleType.Poverty);
        }

        public override void UpdateTarget()
        {
            _targetValue = _parent.Poverty * 0.2;
        }
    }
}
