using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class LeaderData : AbstractNamedEntity
    {
        public string CivilizationName { get; set; }

        [ContentSerializerIgnore]
        public Civilization Civilization
        {
            get
            {
                return Provider.GetCivilization(CivilizationName);
            }
        }

        public string SpecialAbilityName { get; set; }
        public string Lived { get; set; }
        public List<string> Titles { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Flavour> Flavours;

        public override string ImagePath
        {
            get { return ""; }
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return Provider.GetAtlas("LeaderAtlas").GetTexture(ImageName); }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (string.IsNullOrEmpty(CivilizationName))
                result.Add(new MissingAsset(this, "Civilization (empty)", CivilizationName));
            else if (Civilization == null)
                result.Add(new MissingAsset(this, "Civilization", CivilizationName));

            // check flavours
            if (Flavours != null)
            {
                foreach (Flavour f in Flavours)
                {
                    if (f.Data == null)
                        result.Add(new MissingAsset(this, "Flavour", f.Name));
                }
            }

            return result;
        }
    }
}
