using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Types.AI;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class PolicyType : AbstractNamedEntity
    {
        public string Historical { get; set; }
        public string EraName { get; set; }
        
        public string FreePolicyName { get; set; }

        [ContentSerializerIgnore]
        public Policy FreePolicy
        {
            get
            {
                return Provider.GetPolicy(FreePolicyName);
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
        public Flavours Flavours
        {
            get 
            {
                Flavours f = new Flavours();

                if (Provider.CanTranslate) 
                {
                    Policy free = Provider.GetPolicy(FreePolicyName);

                    if (free == null)
                        throw new Exception("Cannot find: " + FreePolicyName);

                    float factor = 1f;
                    f += free.Flavours;

                    foreach (Policy p in Policies)
                    {
                        f += (p.Flavours / 3f);
                        factor += 0.333f;
                    }

                    f /= factor;
                }

                return f;
            }
        }

        [ContentSerializerIgnore]
        public List<Policy> Policies
        {
            get 
            {
                return Provider.Instance.Policies.Values.Where(a => a.PolicyTypeName == Name).ToList();
            }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            // nothing to check so far
            return new List<MissingAsset>();
        }
    }
}
