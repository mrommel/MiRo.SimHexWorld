using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;

namespace MiRo.SimHexWorld.Engine.Types
{
    public enum RessourceType { Strategic, Luxury, Bonus };

    public class Ressource : AbstractNamedEntity
    {
        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                return Provider.GetAtlas("ResourceAtlas").GetTexture(ImageName);
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get
            {
                return "";
            }
        }

        public RessourceType RessourceType { get; set; }
        public Bonus Bonus { get; set; }

        public List<string> Improvements { get; set; }
        public string RequiredTechName { get; set; }

        [ContentSerializerIgnore]
        public Tech RequiredTech
        {
            get
            {
                if (string.IsNullOrEmpty(RequiredTechName))
                    return null;

                return Provider.GetTech(RequiredTechName);
            }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Provider.GetAtlas("ResourceAtlas") == null)
                result.Add(new MissingAsset(this, "Atlas", "ResourceAtlas"));
            else if (!Provider.GetAtlas("ResourceAtlas").HasIdentifier(ImageName))
                result.Add(new MissingAsset(this, "AtlasEntry in 'ResourceAtlas'", ImageName));

            if (!string.IsNullOrEmpty(RequiredTechName) && !Provider.Instance.Techs.ContainsKey(RequiredTechName))
                result.Add(new MissingAsset(this, "Technology", RequiredTechName));

            foreach( string impr in Improvements )
                if( !Provider.Instance.Improvements.ContainsKey(impr) )
                    result.Add(new MissingAsset(this, "Improvement", impr));

            return result;
        }
    }  
}
