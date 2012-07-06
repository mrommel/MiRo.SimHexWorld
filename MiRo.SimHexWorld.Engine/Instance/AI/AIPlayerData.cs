using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.Instance.AI
{
    public class AIPlayerData : AbstractPlayerData
    {
        #region grand strategy weights
        //const int AI_GRAND_STRATEGY_NUM_TURNS_STRATEGY_MUST_BE_ACTIVE = 0;
        //const int AI_GRAND_STRATEGY_RAND_ROLL = 50;
        const int AI_GRAND_STRATEGY_CURRENT_STRATEGY_WEIGHT = 50;
        const int AI_GRAND_STRATEGY_GUESS_NO_CLUE_WEIGHT = 40;
        const int AI_GRAND_STRATEGY_GUESS_POSITIVE_THRESHOLD = 120;
        const int AI_GRAND_STRATEGY_GUESS_LIKELY_THRESHOLD = 70;
        //const int AI_GRAND_STRATEGY_OTHER_PLAYERS_GRAND_STRATEGY_MULTIPLIER = 40;

        const int AI_GRAND_STRATEGY_CONQUEST_NOBODY_MET_FIRST_TURN = 20;
        const int AI_GRAND_STRATEGY_CONQUEST_NOBODY_MET_WEIGHT = -50;
        const int AI_GRAND_STRATEGY_CONQUEST_AT_WAR_WEIGHT = 10;
        const int AI_GRAND_STRATEGY_CONQUEST_MILITARY_STRENGTH_FIRST_TURN = 60;
        const int AI_GRAND_STRATEGY_CONQUEST_POWER_RATIO_MULTIPLIER = 100;
        const int AI_GRAND_STRATEGY_CONQUEST_CRAMPED_WEIGHT = 20;
        const int AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MINOR_ATTACKED = 5;
        const int AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MINOR_CONQUERED = 10;
        const int AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MAJOR_ATTACKED = 10;
        const int AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MAJOR_CONQUERED = 15;

        const int AI_GRAND_STRATEGY_CULTURE_RATIO_MULTIPLIER = 150;
        const int AI_GRAND_STRATEGY_CULTURE_RATIO_EARLY_TURN_THRESHOLD = 50;
        const int AI_GRAND_STRATEGY_CULTURE_RATIO_EARLY_DIVISOR = 2;
        const int AI_GRAND_STRATEGY_CULTURE_MAX_CITIES = 4;

        const int AI_GRAND_STRATEGY_UN_EACH_MINOR_ATTACKED_WEIGHT = -40;
        const int AI_GRAND_STRATEGY_UN_SECURED_VOTE_MOD = 300;

        const int AI_GRAND_STRATEGY_SS_HAS_APOLLO_PROGRAM = 150;
        const int AI_GRAND_STRATEGY_SS_TECH_PROGRESS_MOD = 300;
        #endregion grand strategy weights

        #region city specialization weights
        const int AI_CITY_SPECIALIZATION_EARLIEST_TURN = 25;
        const int AI_CITY_SPECIALIZATION_REEVALUATION_INTERVAL = 10000;

        const int AI_CITY_SPECIALIZATION_GENERAL_ECONOMIC_WEIGHT = 200;

        const int AI_CITY_SPECIALIZATION_FOOD_WEIGHT_FLAVOR_EXPANSION = 5;
        const int AI_CITY_SPECIALIZATION_FOOD_WEIGHT_PERCENT_CONTINENT_UNOWNED = 5;
        const int AI_CITY_SPECIALIZATION_FOOD_WEIGHT_NUM_CITIES = -50;
        const int AI_CITY_SPECIALIZATION_FOOD_WEIGHT_NUM_SETTLERS = -40;
        const int AI_CITY_SPECIALIZATION_FOOD_WEIGHT_EARLY_EXPANSION = 500;

        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_OPERATIONAL_UNITS_REQUESTED = 10;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_CIVS_AT_WAR_WITH = 100;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_WAR_MOBILIZATION = 300;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_EMPIRE_DEFENSE = 300;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_EMPIRE_DEFENSE_CRITICAL = 1000;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_CAPITAL_THREAT = 50;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_NEED_NAVAL_UNITS = 50;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_NEED_NAVAL_UNITS_CRITICAL = 250;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_FLAVOR_WONDER = 200;
        const float AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_NEXT_WONDER =  0.2f;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_WEIGHT_FLAVOR_SPACESHIP = 5;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_TRAINING_PER_OFFENSE = 10;
        const int AI_CITY_SPECIALIZATION_PRODUCTION_TRAINING_PER_PERSONALITY = 10;

        const int AI_CITY_SPECIALIZATION_GOLD_WEIGHT_FLAVOR_GOLD = 20;
        const int AI_CITY_SPECIALIZATION_GOLD_WEIGHT_LAND_DISPUTE = 10;

        const int AI_CITY_SPECIALIZATION_SCIENCE_WEIGHT_FLAVOR_SCIENCE = 20;
        const int AI_CITY_SPECIALIZATION_SCIENCE_WEIGHT_FLAVOR_SPACESHIP = 10;

        const int AI_CITY_SPECIALIZATION_YIELD_WEIGHT_FIRST_RING = 8;
        const int AI_CITY_SPECIALIZATION_YIELD_WEIGHT_SECOND_RING = 5;
        const int AI_CITY_SPECIALIZATION_YIELD_WEIGHT_THIRD_RING = 2;
        const int AI_CITY_SPECIALIZATION_YIELD_NUM_TILES_CONSIDERED = 18;
        #endregion city specialization weights

        GrandStrategyData _grandStrategy;
        Dictionary<GrandStrategyData, bool> _everHadStrategy = new Dictionary<GrandStrategyData, bool>();
        Flavours _grandStrategyFlavours;

        //EconomicStratedyData _economicStategy;
        List<MilitaryStrategyData> _militaryStrategies = new List<MilitaryStrategyData>();
        Flavours _militaryStrategyFlavours = new Flavours();

        public AIPlayerData(int id, Civilization tribe)
            : base(id, tribe, false)
        {
            // determine grand strategy
            DetermineGrandStragegy();
            _grandStrategyFlavours = Flavours.FromGrandStrategy(_grandStrategy);

            // add all military strategies to propability map
            foreach (MilitaryStrategyData mStrategy in Provider.Instance.MilitayStrategies.Values)
                _militaryStrategies.Add(mStrategy);
        }

        public void DetermineGrandStragegy()
        {
            PropabilityMap<GrandStrategyData> map = new PropabilityMap<GrandStrategyData>();

            #region conquest strategy
            GrandStrategyData conquestStrategy = Provider.GetGrandStrategy("Conquest");

            if (conquestStrategy != null)
            {
                float conquestWeight = _diplomaticStatuses.Count == 0 && IsFirstRun ? AI_GRAND_STRATEGY_CONQUEST_NOBODY_MET_FIRST_TURN : 0;
                conquestWeight += _diplomaticStatuses.Count == 0 && !IsFirstRun ? AI_GRAND_STRATEGY_CONQUEST_NOBODY_MET_WEIGHT : 0;
                conquestWeight += _diplomaticStatuses.Exists(a => a.Status == BilateralStatus.AtWar) ? AI_GRAND_STRATEGY_CONQUEST_AT_WAR_WEIGHT : 0;

                conquestWeight += IsFirstRun ? AI_GRAND_STRATEGY_CONQUEST_MILITARY_STRENGTH_FIRST_TURN : 0;
                conquestWeight += AI_GRAND_STRATEGY_CONQUEST_POWER_RATIO_MULTIPLIER * MilitaryPowerValue;
                conquestWeight += AI_GRAND_STRATEGY_CONQUEST_CRAMPED_WEIGHT * CrampedValue;

                foreach (DiplomaticStatus status in _diplomaticStatuses)
                {
                    conquestWeight += status.HasBeenAttacked && !status.IsMinor ? AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MAJOR_ATTACKED : 0;
                    conquestWeight += status.HasBeenConquered && !status.IsMinor ? AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MAJOR_CONQUERED : 0;
                    conquestWeight += status.HasBeenAttacked && status.IsMinor ? AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MINOR_ATTACKED : 0;
                    conquestWeight += status.HasBeenConquered && status.IsMinor ? AI_GRAND_STRATEGY_CONQUEST_WEIGHT_PER_MINOR_ATTACKED : 0;
                }

                if( _grandStrategy == conquestStrategy )
                    conquestWeight += AI_GRAND_STRATEGY_CURRENT_STRATEGY_WEIGHT;

                if( !( _everHadStrategy.ContainsKey( conquestStrategy ) && _everHadStrategy[conquestStrategy] ) )
                    conquestWeight += AI_GRAND_STRATEGY_GUESS_NO_CLUE_WEIGHT;

                map.AddItem(conquestStrategy, conquestWeight);
            }
            #endregion conquest strategy

            #region culture strategy
            GrandStrategyData cultureStrategy = Provider.GetGrandStrategy("Culture");

            if (cultureStrategy != null)
            {
                float cultureWeight = CultureRatio * AI_GRAND_STRATEGY_CULTURE_RATIO_MULTIPLIER;

                if (IsEarly)
                {
                    cultureWeight /= AI_GRAND_STRATEGY_CULTURE_RATIO_EARLY_DIVISOR;

                    if (cultureWeight < AI_GRAND_STRATEGY_CULTURE_RATIO_EARLY_TURN_THRESHOLD)
                        cultureWeight = 0f;
                }

                if (_cities.Count > AI_GRAND_STRATEGY_CULTURE_MAX_CITIES)
                    cultureWeight = 0f;

                if( _grandStrategy == cultureStrategy )
                    cultureWeight += AI_GRAND_STRATEGY_CURRENT_STRATEGY_WEIGHT;

                if( !( _everHadStrategy.ContainsKey( cultureStrategy ) && _everHadStrategy[cultureStrategy] ) )
                    cultureWeight += AI_GRAND_STRATEGY_GUESS_NO_CLUE_WEIGHT;

                map.AddItem(cultureStrategy, cultureWeight);
            }
            #endregion culture strategy

            #region spaceship strategy
            GrandStrategyData spaceshipStrategy = Provider.GetGrandStrategy("Spaceship");

            if (spaceshipStrategy != null)
            {
                float spaceshipWeight = Wonders.FirstOrDefault( a => a.Name == "ApolloProgram" ) != null ? AI_GRAND_STRATEGY_SS_HAS_APOLLO_PROGRAM : 0;
                spaceshipWeight += AI_GRAND_STRATEGY_SS_TECH_PROGRESS_MOD * SpaceshipTechProgress;

                if( _grandStrategy == spaceshipStrategy )
                    spaceshipWeight += AI_GRAND_STRATEGY_CURRENT_STRATEGY_WEIGHT;

                if( !( _everHadStrategy.ContainsKey( spaceshipStrategy ) && _everHadStrategy[spaceshipStrategy] ) )
                    spaceshipWeight += AI_GRAND_STRATEGY_GUESS_NO_CLUE_WEIGHT;

                map.AddItem(spaceshipStrategy, spaceshipWeight);
            }
            #endregion spaceship strategy

            #region unitednations strategy
            GrandStrategyData unitednationsStrategy = Provider.GetGrandStrategy("UnitedNations");

            if (unitednationsStrategy != null)
            {
                float unitednationsWeight = 0f;

                foreach (DiplomaticStatus status in _diplomaticStatuses)
                {
                    unitednationsWeight += status.IsMinor && status.HasBeenAttacked ? AI_GRAND_STRATEGY_UN_EACH_MINOR_ATTACKED_WEIGHT : 0;
                    unitednationsWeight += status.Status == BilateralStatus.VoteForUN ? AI_GRAND_STRATEGY_UN_SECURED_VOTE_MOD : 0;
                }

                if( _grandStrategy == unitednationsStrategy )
                    unitednationsWeight += AI_GRAND_STRATEGY_CURRENT_STRATEGY_WEIGHT;

                if( !( _everHadStrategy.ContainsKey( unitednationsStrategy ) && _everHadStrategy[unitednationsStrategy] ) )
                    unitednationsWeight += AI_GRAND_STRATEGY_GUESS_NO_CLUE_WEIGHT;

                map.AddItem(unitednationsStrategy, unitednationsWeight);
            }
            #endregion unitednations strategy

            GrandStrategyData newStrategy = map.Random;

            if (newStrategy != _grandStrategy)
            {
                // grand stragety is set on first run or by 40% chance
                if( _grandStrategy == null || rand.Next(100) < 40)
                    ChangeGrandStrategy(newStrategy);
            }
        }

        private void ChangeGrandStrategy(GrandStrategyData newStrategy)
        {
            if (_everHadStrategy.ContainsKey(newStrategy))
                _everHadStrategy[newStrategy] = true;
            else
                _everHadStrategy.Add(newStrategy, true);

            _grandStrategy = newStrategy;
            _grandStrategyFlavours = Flavours.FromGrandStrategy(_grandStrategy);
        }

        public virtual Flavours Flavours
        {
            get 
            {
                return base.Flavours + _grandStrategyFlavours + _militaryStrategyFlavours;
            }
        }

        public float MilitaryPowerValue
        {
            get
            {
                return 0f;
            }
        }

        public float SpaceshipTechProgress
        {
            get
            {
                return 0f;
            }
        }

        public float CrampedValue
        {
            get
            {
                return 0f;
            }
        }

        public float CultureRatio
        {
            get
            {
                if (MainWindow.Game == null)
                    return 0f;

                float total = MainWindow.Game.TotalCulture;

                if (total == 0L)
                    return 0f;

                return (float)Culture / total;
            }
        }

        public override bool Update(GameTime time)
        {
            bool turned = base.Update(time);

            if( turned)
            {
                DetermineGrandStragegy();
                UpdateMilitaryStragetyFlavourWeights();
            }
            //string playerStr = _flavours.ToString();
            //string unitStr = unitFlavours.ToString();

            return turned;
        }

        private static float AI_MILITARY_THREAT_WEIGHT_MINOR = 1;
        private static float AI_MILITARY_THREAT_WEIGHT_MAJOR = 3;
        private static float AI_STRATEGY_DEFEND_MY_LANDS_UNITS_PER_CITY = 1;
        private static float AI_STRATEGY_DEFEND_MY_LANDS_BASE_UNITS = 3;

        private void UpdateMilitaryStragetyFlavourWeights()
        {
            _militaryStrategyFlavours.Clear();
            float sum = 0;

            // helper values
            int defenseUnits = _units.Count(a => a.Data.UnitType == UnitClass.Melee || a.Data.UnitType == UnitClass.Archery);
            float targetDefenseUnit = AI_STRATEGY_DEFEND_MY_LANDS_BASE_UNITS + AI_STRATEGY_DEFEND_MY_LANDS_UNITS_PER_CITY * _cities.Count;

            foreach (MilitaryStrategyData strategy in _militaryStrategies)
            {
                float weight = 0f;

                switch (strategy.Name)
                {
                    case "AtWar":
                        weight = _diplomaticStatuses.Count(a => a.Status == BilateralStatus.AtWar && a.IsMinor) > 0 ? AI_MILITARY_THREAT_WEIGHT_MINOR : 0;
                        weight = Math.Max(weight, _diplomaticStatuses.Count(a => a.Status == BilateralStatus.AtWar && !a.IsMinor) > 0 ? AI_MILITARY_THREAT_WEIGHT_MAJOR : 0);
                        break;
                    case "EmpireDefense":
                        weight = Math.Max(0, targetDefenseUnit / defenseUnits - 1);
                        weight = MathHelper.Clamp(weight, 0f, 1f);
                        break;
                    case "EmpireDefenseCritical":
                        weight = 0f;
                        break;
                    case "EnoughMilitaryUnits":
                        weight = Math.Max(0, targetDefenseUnit / defenseUnits - 1);
                        weight = 1f - MathHelper.Clamp(weight, 0f, 1f);
                        break;
                    case "EnoughNavalUnits":
                        weight = 0f;
                        break;
                    case "EnoughRanged":
                        weight = 0f;
                        break;
                    case "EradicateBarbarians":
                        // distance of next barbarian camp / num of units
                        weight = 0.2f;
                        break;
                    case "LoosingWars":
                        weight = 0f;
                        break;
                    case "MinorCivGeneralDefense":
                        weight = 0f;
                        break;
                    case "MinorCivThreatCritical":
                        weight = 0f;
                        break;
                    case "MinorCivThreatElevated":
                        weight = 0f;
                        break;
                    case "NeedNavalUnits":
                        weight = 0f;
                        break;
                    case "NeedRanged":
                        weight = 0f;
                        break;
                    case "WarMobilization":
                        weight = 0f;
                        break;
                    case "WinningWars":
                        weight = 0f;
                        break;
                    default:
                        throw new Exception("No Weight handler for Military Strategy: " + strategy.Name);
                }

                _militaryStrategyFlavours += strategy.Flavours * weight;
                sum += weight;
            }

            if (sum != 0)
                _militaryStrategyFlavours /= sum;
        }

        public Flavours UnitFlavours
        {
            get
            {
                Flavours unitFlavours = new Flavours();

                foreach (Unit unit in _units)
                    unitFlavours += unit.Data.Flavours;

                return unitFlavours;
            }
        }

        public override void StartAiThreads()
        {

        }

        public override void Dispose()
        {

        }
    }
}
