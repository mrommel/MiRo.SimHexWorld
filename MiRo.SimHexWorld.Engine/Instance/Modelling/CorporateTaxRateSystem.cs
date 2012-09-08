using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    class CorporateTaxRateSystem : AbstractRuleSystem
    {
       public CorporateTaxRateSystem(ModellingTile modellingTile, double initialValue)
           : base(modellingTile, RuleType.CorporateTaxRate, initialValue)
        {
        }

       public override void UpdateTarget()
       {
       }
    }
}
