using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class MidIncomePeopleRuleSystem : AbstractRuleSystem
    {
        public MidIncomePeopleRuleSystem(ModellingTile modellingTile)
            : base(modellingTile, RuleType.MidIncomePeople, 0.6)
        {
        }

        public override void UpdateTarget()
        {
            _targetValue = 0.6 
                + 0.27 - (1.07 * ModellingTile.GetTaxValue(_parent.IncomeTaxRate)) 
                - (0.18 * ModellingTile.GetTaxValue(_parent.InheritanceTaxRate))
                - (0.32 * ModellingTile.GetTaxValue(_parent.PropertyTaxRate));

            _targetValue = Clamp(_targetValue, 0, 1);
        }
    }
}
