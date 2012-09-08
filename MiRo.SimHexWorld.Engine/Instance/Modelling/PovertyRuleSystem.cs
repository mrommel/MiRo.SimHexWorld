using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class PovertyRuleSystem : AbstractRuleSystem
    {
        public PovertyRuleSystem(ModellingTile tile)
            : base(tile, RuleType.Poverty,0)
        {}

        public override void UpdateTarget()
        {
            _targetValue = 0.00 + (0.20 * ModellingTile.GetTaxValue(_parent.SalesTaxRate));
        }
    }
}
