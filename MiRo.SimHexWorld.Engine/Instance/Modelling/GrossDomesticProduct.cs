using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class GrossDomesticProduct : AbstractRuleSystem
    {
        public GrossDomesticProduct(ModellingTile parent)
            : base(parent, RuleType.GDP, 0.4)
        {
            _effects.Add(RuleType.Productivity);
            _effects.Add(RuleType.CorporateTaxRate);
            _effects.Add(RuleType.Crime);
            _effects.Add(RuleType.EnergyCost);
            _effects.Add(RuleType.Immigration);
        }

        public override void UpdateTarget()
        {
            _targetValue = - 0.18 + _parent.Productivity * 0.36
                - ModellingTile.GetTaxValue(_parent.CorporateTaxRate)
                - _parent.Crime * 0.08 
                + 0.22 - _parent.EnergyCost * 0.4
                - 0.035 + (0.035 * _parent.Immigration);
        }
    }
}
