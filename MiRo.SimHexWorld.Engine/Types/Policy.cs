using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types.AI;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Policy : AbstractNamedEntity
    {
        public string Historical { get; set; }
        public string PolicyTypeName { get; set; }

        public List<string> RequiredPolicyNames { get; set; }

        #region bonuses

        [ContentSerializer(Optional = true)]
        public float BarbarianCombatModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public bool AlwaysSeeBarbarianCamps { get; set; }

        [ContentSerializer(Optional = true)]
        public float SettlerProductionModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public int FreeFoodBox { get; set; }

        [ContentSerializer(Optional = true)]
        public int MinorFriendshipDecayMod { get; set; }

        [ContentSerializer(Optional = true)]
        public int ExtraHappiness { get; set; }

        [ContentSerializer(Optional = true)]
        public float CapitalUnhappinessMod { get; set; }

        [ContentSerializer(Optional = true)]
        public int PlotGoldCostMod { get; set; }

        [ContentSerializer(Optional = true)]
        public int CulturePerCity { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Yield> Yields { get; set; }

        [ContentSerializer(Optional = true)]
        public float ScienceModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public int SciencePerScientist { get; set; }

        [ContentSerializer(Optional = true)]
        public float WonderProductionModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public float GrowthRateCapitalModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public string FreePromotionName { get; set; }

        [ContentSerializer(Optional = true)]
        public string FreeUnitName { get; set; }

        [ContentSerializer(Optional = true)]
        public float ExperienceModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public int HappinessPerGarrisonedUnit { get; set; }

        [ContentSerializer(Optional = true)]
        public float UnitUpgradeCostModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public int HappinessPerTradeRoute { get; set; }

        #endregion bonuses

        public Flavours Flavours { get; set; }

        [ContentSerializerIgnore]
        public int YieldsHappiness
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Happiness && !a.CapitalOnly);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializerIgnore]
        public int YieldsFood
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Food && !a.CapitalOnly);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializerIgnore]
        public int YieldsFoodCapital
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Food && a.CapitalOnly );

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializerIgnore]
        public int YieldsScience
        {
            get
            {
                if (Yields == null)
                    return 0;

                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Science);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return Provider.GetAtlas("PolicyAtlas").GetTexture(ImageName); }
        }

        public override string ImagePath
        {
            get { return ""; }
        }

        [ContentSerializerIgnore]
        public PolicyType PolicyType
        {
            get
            {
                if (!string.IsNullOrEmpty(PolicyTypeName))
                    return Provider.Instance.PolicyTypes[PolicyTypeName];

                return null;
            }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (!Provider.Instance.PolicyTypes.ContainsKey(PolicyTypeName))
                result.Add(new MissingAsset(this, "PolicyType", PolicyTypeName));

            if (!string.IsNullOrEmpty(FreePromotionName) && !Provider.Instance.Promotions.ContainsKey(FreePromotionName))
                result.Add(new MissingAsset(this, "Promotion", FreePromotionName));

            if (RequiredPolicyNames != null)
            {
                foreach (string policyName in RequiredPolicyNames)
                {
                    if (!string.IsNullOrEmpty(policyName) && !Provider.Instance.Policies.ContainsKey(policyName))
                        result.Add(new MissingAsset(this, "Policy", policyName));
                }
            }

            return result;
        }
    }
}
