using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class LowIncomeRuleSystem : AbstractRuleSystem
    {
        public static int BASEINCOME = 5000;
        public static int SALESVALUE = 2000;
        public static int INCOMEVALUE = 2000;

        public LowIncomeRuleSystem(ModellingTile tile)
            : base(tile,RuleType.LowIncome,1000)
        {
            _description = "The effective income of those people who are on low (or no) earnings in our society. If this value rises high enough (through benefits and tax exemptions for the poor), then more people will move out of poverty and in to the middle income group. Progressive taxation, combined with minimum income guarantees and state benefits are the most popular ways to 'lift people' out of poverty.";
        }

        public override void UpdateTarget()
        {
            _targetValue = BASEINCOME
                - ModellingTile.GetTaxValue(_parent.SalesTaxRate) * SALESVALUE
                - ModellingTile.GetTaxValue(_parent.IncomeTaxRate) * INCOMEVALUE;
        }
    }
}
