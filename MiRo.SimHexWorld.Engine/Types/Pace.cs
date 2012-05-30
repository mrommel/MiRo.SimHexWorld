using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class Pace : AbstractNamedEntity
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
                catch (Exception ex)
                {
                    string str = ex.Message;
                }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return "Content/Textures/Paces/" + ImageName; }
        }

        public override List<MiRo.SimHexWorld.Engine.Misc.MissingAsset> CheckIntegrity()
        {
            throw new NotImplementedException();
        }
    }
}
