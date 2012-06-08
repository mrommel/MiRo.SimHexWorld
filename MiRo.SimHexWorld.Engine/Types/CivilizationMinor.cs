using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Instance.AI;
using MiRo.SimHexWorld.Engine.UI;

namespace MiRo.SimHexWorld.Engine.Types
{
    public class CivilizationMinor : AbstractNamedEntity
    {
        private Texture2D _image;

        [ContentSerializerIgnore]
        public override Texture2D Image
        {
            get
            {
                return Provider.GetAtlas("CivilizationAtlas").GetTexture(ImageName);
            }
        }

        [ContentSerializerIgnore]
        public override string ImagePath
        {
            get { return ""; }
        }

        public string PlayerColorName { get; set; }

        [ContentSerializerIgnore]
        public PlayerColor PlayerColor { get { return MainWindow.Game.PlayerColors.FirstOrDefault( a => a.Name == PlayerColorName); } }

        public Flavours Flavours { get; set; }

        public override List<Misc.MissingAsset> CheckIntegrity()
        {
            throw new NotImplementedException();
        }
    }
}
