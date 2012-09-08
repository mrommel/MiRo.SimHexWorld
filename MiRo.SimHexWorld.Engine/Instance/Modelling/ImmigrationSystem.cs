using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class ImmigrationRuleSystem : AbstractRuleSystem
    {
        public ImmigrationRuleSystem(ModellingTile tile)
            : base(tile, RuleType.Immigration, 0)
        {
            _description = "Immigration measures the number of people entering this country, both legally and illegally, with the intent of making this their new home. Immigration is generally caused by a strong economy, as those living in poorer countries seek out employment and a higher standard of living. Immigration can be reduced by border controls, and specific measures to set immigration quotas. Too much immigration too fast can lead to racial tensions developing.";
        }

        public override void UpdateTarget()
        {
            _targetValue = 0.63 * (Math.Pow(_parent.GrossDomesticProduct, 4));
        }
    }
}
