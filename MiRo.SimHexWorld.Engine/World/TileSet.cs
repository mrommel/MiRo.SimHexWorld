using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.World
{
    public class TileSet
    {
        public readonly List<string> Tiles = new List<string>();

        [XmlIgnore()]
        [ContentSerializerIgnore]
        public int TilesetIndex { get; set; }

        public TileSet()
        {
            TilesetIndex = TileMatchPattern.Noindex;
        }
    }
}