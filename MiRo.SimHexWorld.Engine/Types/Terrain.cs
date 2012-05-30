using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;
using MiRo.SimHexWorld.Engine.World;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Terrain : AbstractNamedEntity
    {
        public string Civ5Name { get; set; }

        public string AtlasName { get; set; }
        public string AtlasIdentifier { get; set; }

        public TileType TileType  { get; set; }

        public TileHeight TileHeight { get; set; }

        //private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                return Provider.GetAtlas("TerrainAtlas").GetTexture(AtlasIdentifier);
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return ""; }
        }

        public Bonus Bonus { get; set; }
        public double CombatModifiers { get; set; }
        public int MovementCost { get; set; }

        public List<string> PossibleFeatures { get; set; }
        public List<string> PossibleRessources { get; set; }

        public bool CanFound { get; set; }
        public int InfluenceCost { get; set; }
        public int BuildModifier { get; set; }

        public List<MoveRateType> MoveRateTypes { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            //if (Image == null)
            //    result.Add(new MissingAsset(this, "Image", ImageName));
            if (Provider.GetAtlas(AtlasName) == null)
                result.Add(new MissingAsset(this, "Atlas", AtlasName));
            else if (!Provider.GetAtlas(AtlasName).HasIdentifier(AtlasIdentifier))
                result.Add(new MissingAsset(this, "AtlasEntry in '" + AtlasName + "'", AtlasIdentifier));

            foreach (string featureName in PossibleFeatures)
                if (!Provider.Instance.Features.ContainsKey(featureName))
                    result.Add(new MissingAsset(this, "Feature", featureName));
                    
            foreach (string ressourceName in PossibleRessources)
                if (!Provider.Instance.Ressources.ContainsKey(ressourceName))
                    result.Add(new MissingAsset(this, "Ressource", ressourceName));

            return result;
        }
    }
}
