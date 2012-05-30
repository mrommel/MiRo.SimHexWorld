using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Screens;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class MapSize : AbstractNamedEntity
    {
        public Size Size { get; set; }
        public int Players  { get; set; }
        public int CityStates { get; set; }
        public float CultureModifierPerCity { get; set; }

        //[ContentSerializer(Optional = true)]
        //public MapDescription Settings { get; set; }

        //[ContentSerializer(Optional = true)]
        //public MapData Data { get; set; }

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
                return "Content/Textures/MapSizes/" + ImageName;
            }
        }

        public int Order { get; set; }
        public int NumOfRivers { get; set; }
        public int CitiesPerPlayer { get; set; }

        public override List<MissingAsset> CheckIntegrity()
        {
            List<MissingAsset> result = new List<MissingAsset>();

            if (Image == null)
                result.Add(new MissingAsset(this, "Image", ImagePath));

            //if (Settings == null && Data == null)
            //    result.Add(new MissingAsset(this, "MapDescription or MapData", "one must be set"));

            return result;
        }
    }
}
