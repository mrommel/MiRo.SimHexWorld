using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public class ModellingTile
    {
        private HexPoint _location;

        protected Dictionary<RuleType, AbstractRuleSystem> _systems = new Dictionary<RuleType, AbstractRuleSystem>();
        //protected Dictionary<PolicyType, bool> _policies = new Dictionary<PolicyType, bool>();

        public ModellingTile(HexPoint pt)
        {
            _location = pt;

            // init rules
            _systems.Add(RuleType.Citizen, new CitizenRuleSystem(this,500));
            _systems.Add(RuleType.GrowthRate, new GrowthRateRuleSystem(this,1.1));
            _systems.Add(RuleType.Pollution, new PollutionRuleSystem(this, 0.0));
            _systems.Add(RuleType.GDP, new GrossDomesticProduct(this));
            _systems.Add(RuleType.Productivity, new ProductivitySystem(this, 0.0));

            _systems.Add(RuleType.Food, new FoodSystem(this, 0.0));
            _systems.Add(RuleType.CorporateTaxRate, new CorporateTaxRateSystem(this, 0.0));
            _systems.Add(RuleType.Crime, new CrimeSystem(this, 0.0));
            _systems.Add(RuleType.AgricultureLevel, new AgricultureSystem(this, 0.0));
            _systems.Add(RuleType.EnergyCost, new EnergyCostSystem(this));

            _systems.Add(RuleType.Immigration, new ImmigrationRuleSystem(this));
            _systems.Add(RuleType.Poverty, new PovertyRuleSystem(this));

            // people
            _systems.Add(RuleType.LowIncomePeople, new LowIncomePeopleRuleSystem(this));
            _systems.Add(RuleType.MidIncomePeople, new MidIncomePeopleRuleSystem(this));
            _systems.Add(RuleType.HighIncomePeople, new HighIncomePeopleRuleSystem(this));

            // money
            _systems.Add(RuleType.LowIncome, new LowIncomeRuleSystem(this));
            _systems.Add(RuleType.MidIncome, new MidIncomeRuleSystem(this));
            _systems.Add(RuleType.HighIncome, new HighIncomeRuleSystem(this));

            //// Tradition
            //_policies.Add(PolicyType.Aristocracy, false);
            //_policies.Add(PolicyType.Oligarchy, false);
            //_policies.Add(PolicyType.Legalism, false);
            //_policies.Add(PolicyType.LandedElite, false);
            //_policies.Add(PolicyType.Monarchy, false);

            //// Liberty
            //_policies.Add(PolicyType.CollectiveRule, false);
            //_policies.Add(PolicyType.Citizenship, false);
            //_policies.Add(PolicyType.Republic, false);
            //_policies.Add(PolicyType.Representation, false);
            //_policies.Add(PolicyType.Meritocracy, false);
        }

        public void Update()
        {
            // update targets
            foreach (AbstractRuleSystem system in _systems.Values)
                system.UpdateTarget();

            // apply values
            foreach (AbstractRuleSystem system in _systems.Values)
                system.UpdateCurrent();

            // normalize people distribution
            AbstractRuleSystem highIncomePeople = _systems.FirstOrDefault(a => a.Key == RuleType.HighIncomePeople).Value;
            AbstractRuleSystem midIncomePeople = _systems.FirstOrDefault(a => a.Key == RuleType.MidIncomePeople).Value;
            AbstractRuleSystem lowIncomePeople = _systems.FirstOrDefault(a => a.Key == RuleType.LowIncomePeople).Value;

            double sum = highIncomePeople.Current + midIncomePeople.Current + lowIncomePeople.Current;
            highIncomePeople.Current /= sum;
            midIncomePeople.Current /= sum;
            lowIncomePeople.Current /= sum;
        }

        public double Food { get { return GetValue(RuleType.Food); } }
        public int Citizen { get { return (int)GetValue(RuleType.Citizen); } }
        public double Growth { get { return GetValue(RuleType.GrowthRate); } }
        public double Pollution { get { return GetValue(RuleType.Pollution); } }
        public double RacialTension { get { return GetValue(RuleType.RacialTension); } }

        public double Productivity { get { return GetValue(RuleType.Productivity); } }
        public double Crime { get { return GetValue(RuleType.Crime); } }
        public double AgricultureLevel { get { return GetValue(RuleType.AgricultureLevel); } }
        public double Immigration { get { return GetValue(RuleType.Immigration); } }
        public double EnergyCost { get { return GetValue(RuleType.EnergyCost); } }

        public double Poverty { get { return GetValue(RuleType.Poverty); } }
        public double GrossDomesticProduct { get { return GetValue(RuleType.GDP); } }

        public double LowIncome { get { return GetValue(RuleType.LowIncome); } }
        public double MidIncome { get { return GetValue(RuleType.MidIncome); } }
        public double HighIncome { get { return GetValue(RuleType.HighIncome); } }

        public double LowIncomePeople { get { return GetValue(RuleType.LowIncomePeople); } }
        public double MidIncomePeople { get { return GetValue(RuleType.MidIncomePeople); } }
        public double HighIncomePeople { get { return GetValue(RuleType.HighIncomePeople); } }

        public Infrastructure Infrastructure { get; set; }

        public double Revenue
        {
            get
            {
                return TaxRevenue
                    - Infrastructure.GetValue();
                    // - police
                    // - justice
                    // - palace (only capital)
            }
        }

        public double GetValue(RuleType type)
        {
            return _systems.FirstOrDefault(a => a.Key == type).Value.Current;
        }

        // values to tweak
        public enum TaxRate 
        { 
            NOMINAL, 
            VERY_LOW, 
            LOW,
            VERY_FAIR, 
            FAIR,
            REASONABLE,
            AVERAGE,
            HIGH,
            VERY_HIGH,
            UNFAIR,
            PUNITIVE,
            SCANDALOUS,
            OUTRAGEOUS
        };

        public TaxRate InheritanceTaxRate { get; set; }
        public TaxRate PropertyTaxRate { get; set; }
        public TaxRate CorporateTaxRate { get; set; }
        public TaxRate SalesTaxRate { get; set; }
        public TaxRate IncomeTaxRate { get; set; }

        /// <summary>
        /// maps <paramref name="rate"/> from enum to 0..1
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static double GetTaxValue(TaxRate rate)
        {
            double num = Enum.GetValues(typeof(TaxRate)).Length;
            int index = 0;
            foreach (TaxRate r in Enum.GetValues(typeof(TaxRate)))
            {
                if (r == rate)
                    return index / num;

                index++;
            }

            return 0;
        }

        public double SalesTaxRevenue
        {
            get
            {
                return ModellingTile.GetTaxValue(SalesTaxRate) * LowIncomeRuleSystem.SALESVALUE * LowIncomePeople
                    + ModellingTile.GetTaxValue(SalesTaxRate) * MidIncomeRuleSystem.SALESVALUE * MidIncomePeople
                    + ModellingTile.GetTaxValue(SalesTaxRate) * HighIncomeRuleSystem.SALESVALUE * HighIncomePeople;
            }
        }

        public double IncomeTaxRevenue
        {
            get
            {
                return ModellingTile.GetTaxValue(IncomeTaxRate) * LowIncomeRuleSystem.INCOMEVALUE * LowIncomePeople
                    + ModellingTile.GetTaxValue(IncomeTaxRate) * MidIncomeRuleSystem.INCOMEVALUE * MidIncomePeople
                    + ModellingTile.GetTaxValue(IncomeTaxRate) * HighIncomeRuleSystem.INCOMEVALUE * HighIncomePeople;
            }
        }

        public double PropertyTaxRevenue
        {
            get
            {
                return ModellingTile.GetTaxValue(PropertyTaxRate) * MidIncomeRuleSystem.PROPERTYVALUE * MidIncomePeople
                    + ModellingTile.GetTaxValue(PropertyTaxRate) * HighIncomeRuleSystem.PROPERTYVALUE * HighIncomePeople;
            }
        }

        public double InheritanceTaxRevenue
        {
            get
            {
                return ModellingTile.GetTaxValue(InheritanceTaxRate) * MidIncomeRuleSystem.INHERITANCEVALUE * MidIncomePeople
                    + ModellingTile.GetTaxValue(InheritanceTaxRate) * HighIncomeRuleSystem.INHERITANCEVALUE * HighIncomePeople;
            }
        }

        public double TaxRevenue
        { 
            get 
            {
                return SalesTaxRevenue
                    + IncomeTaxRevenue
                    + PropertyTaxRevenue
                    + InheritanceTaxRevenue;
            } 
        }
    }
}
