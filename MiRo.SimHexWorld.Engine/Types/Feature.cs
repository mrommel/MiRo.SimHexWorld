using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class TerrainBonus
    {
        public string Terrain { get; set; }
        public Bonus Bonus { get; set; }
    }

    public class Feature : AbstractNamedEntity
    {
        public string Civ5Name { get; set; }

        public string AtlasName { get; set; }
        public string AtlasIdentifier { get; set; }

        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                return Provider.GetAtlas("FeatureAtlas").GetTexture(AtlasIdentifier);
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

        public List<TerrainBonus> ImprovesTerrains { get; set; }

        public List<string> PossibleRessources { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsForest { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsRiver { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsSpring { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            //if (Image == null)
            //    result.Add( new MissingAsset( this, "Image", ImageName ));
            if (Provider.GetAtlas(AtlasName) == null)
                result.Add(new MissingAsset(this, "Atlas", AtlasName));
            else if (!Provider.GetAtlas(AtlasName).HasIdentifier(AtlasIdentifier))
                result.Add(new MissingAsset(this, "AtlasEntry in '" + AtlasName + "'", AtlasIdentifier));

            foreach (string ressourceName in PossibleRessources)
                if (!Provider.Instance.Ressources.ContainsKey(ressourceName))
                    result.Add(new MissingAsset(this, "Ressource", ressourceName));

            return result;
        }
    }
}
