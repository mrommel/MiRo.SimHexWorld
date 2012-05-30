using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Policy : AbstractNamedEntity
    {
        public string Historical { get; set; }
        public string PolicyTypeName { get; set; }

        public List<string> RequiredPolicyNames { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Yield> Yields { get; set; }

        #region bonuses

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

        #endregion bonuses

        public List<Flavour> Flavours { get; set; }

        [ContentSerializerIgnore]
        public int YieldsHappiness
        {
            get
            {
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
                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Science);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return null; }
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
