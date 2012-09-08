using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class MidIncomeRuleSystem : AbstractRuleSystem
    {
        public static int BASEINCOME = 10000;
        public static int SALESVALUE = 4000;
        public static int INCOMEVALUE = 4000;
        public static int INHERITANCEVALUE = 100;
        public static int PROPERTYVALUE = 100;

        public MidIncomeRuleSystem(ModellingTile tile)
            : base(tile, RuleType.MidIncome, 6000)
        {
            _description = "The effective income of those people on average earnings, not wealthy or poor. This is generally the largest group of people in the economy, so policies which affect this value can be big vote deciders. Middle income earners are expected to own their own homes (or be buying them), and often own one or more cars. They probably take holidays abroad at least once a year. Taxes that affect any of these will hit these people hardest, and may push some of them down into poverty";
        }

        public override void UpdateTarget()
        {
            _targetValue = BASEINCOME
                - ModellingTile.GetTaxValue(_parent.SalesTaxRate) * SALESVALUE
                - ModellingTile.GetTaxValue(_parent.IncomeTaxRate) * INCOMEVALUE
                - ModellingTile.GetTaxValue(_parent.InheritanceTaxRate) * INHERITANCEVALUE
                - ModellingTile.GetTaxValue(_parent.PropertyTaxRate) * PROPERTYVALUE;
        }
    }
}
