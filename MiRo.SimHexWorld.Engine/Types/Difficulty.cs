using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Difficulty : AbstractNamedEntity
    {
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
            get { return "Content/Textures/Difficulties/" + ImageName; }
        }

        public int Order { get; set; }
        public float AIGrowthModifier { get; set; }
        public float CultureModifier { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImageName));

            return result;
        }
    }
}
