using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class SimpleEntity : AbstractNamedEntity
    {
        public SimpleEntity(string name, string desc, string imagepath)
        {
            Name = name;
            Description = desc;
            _imagepath = imagepath;
        }

        private readonly string _imagepath;
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
            get { return _imagepath; }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            return new List<Misc.MissingAsset>();
        }
    }
}
