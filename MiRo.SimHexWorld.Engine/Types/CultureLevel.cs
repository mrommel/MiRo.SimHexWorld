using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class CultureLevel :AbstractNamedEntity
    {
        public int CityDefenseModifier { get; set; }
        public int Threshold { get; set; }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return null; }
        }

        public override string ImagePath
        {
            get { return ""; }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            return new List<Misc.MissingAsset>();
        }
    }
}
