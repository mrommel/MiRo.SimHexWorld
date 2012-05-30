using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class CityData : AbstractNamedEntity
    {
         public string Civilization { get; set; }

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
            get { return "Content/Textures/Cities/" + ImageName; }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            throw new NotImplementedException();
        }
    }
}
