using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class FlavourData : AbstractNamedEntity
    {
        public override string ImagePath
        {
            get { return ""; }
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return null; }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            return new List<Misc.MissingAsset>();
        }
    }
}
