using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class HighIncomeRuleSystem : AbstractRuleSystem
    {
        public static int BASEVALUE = 50000;
        public static int SALESVALUE = 4000;
        public static int INCOMEVALUE = 15000;
        public static int INHERITANCEVALUE = 5000;
        public static int PROPERTYVALUE = 4500;

        public HighIncomeRuleSystem(ModellingTile tile)
            : base(tile, RuleType.HighIncome, 30000)
        {
            _description = "The effective income of the top earners in society. It is easy to reduce this by punitive taxes on the wealthy, their property, inheritance and activities such as air travel that they often indulge in. It is also possible to benefit them by offering tax exemptions and loopholes for the rich, or by favouring regressive taxation such as sales tax, rather than progressive income taxes.";
        }

        public override void UpdateTarget()
        {
            _targetValue = BASEVALUE
                - ModellingTile.GetTaxValue(_parent.SalesTaxRate) * SALESVALUE
                - ModellingTile.GetTaxValue(_parent.IncomeTaxRate) * INCOMEVALUE
                - ModellingTile.GetTaxValue(_parent.InheritanceTaxRate) * INHERITANCEVALUE
                - ModellingTile.GetTaxValue(_parent.PropertyTaxRate) * PROPERTYVALUE;
        }
    }
}
