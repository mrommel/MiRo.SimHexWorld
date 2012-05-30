using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.UI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class PromotionData : AbstractNamedEntity
    {
        public class UnitClassModification
        {
            public UnitClass Type { get; set; }
            public int Modifier { get; set; } // in percent
        }

        public override string ImagePath
        {
            get
            {
                return "Content/Textures/Promotions/" + ImageName;
            }
        }

        private Texture2D _image;

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get
            {
                if (_image == null)
                {
                    try
                    {
                        _image = MainApplication.ManagerInstance.Content.Load<Texture2D>(ImagePath);
                    }
                    catch { }
                }

                return _image;
            }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImagePath));

            // check technology
            if (!string.IsNullOrEmpty(RequiredTechnology) && !Provider.Instance.Techs.ContainsKey(RequiredTechnology))
                result.Add(new MissingAsset(this, "Technology (Required)", RequiredTechnology));

            return result;
        }

        [ContentSerializer(Optional = true)]
        public string RequiredTechnology { get; set; }

        [ContentSerializer(Optional = true)]
        public int CityAttack { get; set; }

        [ContentSerializer(Optional = true)]
        public int DefenseMod { get; set; }

        [ContentSerializer(Optional = true)]
        public bool NoDefensiveBonus { get; set; }

        [ContentSerializer(Optional = true)]
        public int VisibilityChange { get; set; }

        [ContentSerializer(Optional = true)]
        public bool Recon { get; set; }

        [ContentSerializer(Optional = true)]
        public int MovesChange { get; set; }

        [ContentSerializer(Optional = true)]
        public List<UnitClass> Types { get; set; }

        [ContentSerializer(Optional = true)]
        public List<UnitClassModification> Modifications { get; set; }
    }
}
