using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types.AI;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class CitySpecialization : AbstractNamedEntity
    {
        public override string ImagePath
        { 
            get {return ""; }
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return null; }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            // check flavours
            if (Flavours != null)
            {
                foreach (Flavour f in Flavours)
                {
                    if (f.Data == null)
                        result.Add(new MissingAsset(this, "Flavour", f.Name));
                }
            }
            else
                result.Add(new MissingAsset(this, "Flavour (no Flavours)", ""));

            return result;
        }

        public string Yield { get; set; }
        public List<Flavour> Flavours { get; set; }

        [ContentSerializer(Optional = true)]
        public bool MustBeCoastal { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsDefault { get; set; }
    }
}
