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
    public class ResourceBonus
    {
        public string Ressource { get; set; }
        public Bonus Bonus { get; set; }
    }

    public class Improvement : AbstractNamedEntity
    {
        public Improvement()
        {
            if( Cost == 0 )
                Cost = 50;
        }

        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                try
                {
                    if (_image == null)
                        _image = MainApplication.ManagerInstance.Content.Load<Texture2D>(ImagePath);
                }
                catch { }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return "Content/Textures/Improvements/" + ImageName; }
        }

        [ContentSerializer(Optional = true)]
        public int Cost { get; set; }

        public Bonus Bonus { get; set; }
        public List<ResourceBonus> ImprovesResources { get; set; }
        public string RequiredTech { get; set; }

        public List<string> TerrainNames { get; set; }
        public List<string> FeatureNames { get; set; }

        [ContentSerializerIgnore]
        public List<Terrain> Terrains
        {
            get
            {
                List<Terrain> terrains = new List<Terrain>();

                foreach (string terrainName in TerrainNames)
                {
                    if (Provider.Instance.Terrains.ContainsKey(terrainName))
                        terrains.Add(Provider.Instance.Terrains[terrainName]);
                }

                return terrains;
            }
        }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImageName));

            foreach( ResourceBonus rb in ImprovesResources )
                if (!string.IsNullOrEmpty(rb.Ressource) && !Provider.Instance.Ressources.ContainsKey(rb.Ressource))
                    result.Add(new MissingAsset(this, "Ressource", rb.Ressource));

            if (!string.IsNullOrEmpty(RequiredTech) && !Provider.Instance.Techs.ContainsKey(RequiredTech))
                result.Add(new MissingAsset(this, "Technology", RequiredTech));

            if (Terrains.Count == 0)
                result.Add(new MissingAsset(this, "Terrain", "no"));

            foreach (string terrainName in TerrainNames)
                    if (!Provider.Instance.Terrains.ContainsKey(terrainName))
                        result.Add(new MissingAsset(this, "Terrain", terrainName));

            foreach( string featureName in FeatureNames )
                if( !Provider.Instance.Features.ContainsKey( featureName ) )
                    result.Add(new MissingAsset(this, "Feature", featureName));

            return result;
        }
    }
}
