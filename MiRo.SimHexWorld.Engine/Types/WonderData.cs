using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Types.AI;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class WonderData : AbstractNamedEntity
    {
        public int Cost { get; set; }
        public int Maintenance { get; set; }

        public string RequiredTechName { get; set; }

        [ContentSerializerIgnore]
        public Tech RequiredTech
        {
            get
            {
                if (string.IsNullOrEmpty(RequiredTechName))
                    return null;

                return Provider.GetTech(RequiredTechName);
            }
        }

        public List<Flavour> Flavours { get; set; }

        [ContentSerializer(Optional = true)]
        public float ScienceModifier { get; set; }

        [ContentSerializer(Optional = true)]
        public List<Yield> Yields { get; set; }

        [ContentSerializerIgnore]
        public int YieldsFood
        {
            get
            {
                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Food);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
        }

        [ContentSerializerIgnore]
        public int YieldsScience
        {
            get
            {
                Yield yield = Yields.FirstOrDefault(a => a.Type == YieldType.Science);

                if (yield != null)
                    return yield.Amount;

                return 0;
            }
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
                        _image = Provider.GetAtlas("WonderAtlas").GetTexture(ImageName);
                }
                catch { }

                return _image;
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return ""; }
        }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            return new List<Misc.MissingAsset>();
        }
    }
}
