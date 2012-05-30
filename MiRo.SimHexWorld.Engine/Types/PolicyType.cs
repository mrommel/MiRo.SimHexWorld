using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Types.AI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class PolicyType : AbstractNamedEntity
    {
        public string Historical { get; set; }
        public string EraName { get; set; }

        [ContentSerializer(Optional = true)]
        public float BarbarianCombatModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public bool AlwaysSeeBarbCamps { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Yield> Yields { get; set; }
        
        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return null; }
        }

        public override string ImagePath
        {
            get { return ""; }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            // nothing to check so far
            return new List<MissingAsset>();
        }
    }
}
