using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class MapType : AbstractNamedEntity
    {
        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
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

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get
            {
                return "Content/Textures/Maptypes/" + ImageName;
            }
        }

        public int Order { get; set; }

        public float OceanRatio { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImagePath));

            return result;
        }
    }
}
