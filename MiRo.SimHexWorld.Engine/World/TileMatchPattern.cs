using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.World.Maps;

namespace MiRo.SimHexWorld.Engine.World
{
    public class TileMatchPattern : INamed
    {
        public const int Noindex = -1;
        private string _name;
        private string _terrainname;
        private string[] _terrainNameAdjacent;
        private List<TileSet> _tiles;
        private int _score;

        [ContentSerializer(Optional = true)]
        public string Name 
        {
            get
            {
                if (_name == null || _name == "empty")
                    Load();

                if (_name == null)
                    return "empty";

                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [ContentSerializer(Optional = true)]
        public string TerrainName 
        {
            get
            {
                if (_terrainname == null || _terrainname == "empty")
                    Load();

                if (_terrainname == null)
                    return "empty";

                return _terrainname;
            }
            set
            {
                _terrainname = value;
            }
        }

        [ContentSerializer(Optional = true)]
        public string[] TerrainNameAdjacent 
        {
            get
            {
                if (_terrainNameAdjacent == null || _terrainNameAdjacent.Length == 0)
                    Load();

                if (_terrainNameAdjacent == null)
                    return new string[] { };

                return _terrainNameAdjacent;
            }
            set
            {
                _terrainNameAdjacent = value;
            }
        }

        [ContentSerializer(Optional = true)]
        public List<TileSet> Tiles
        {
            get
            {
                if (_tiles == null || _tiles.Count == 0)
                    Load();

                if (_tiles == null)
                    return new List<TileSet>();

                return _tiles;
            }
            set
            {
                _tiles = value;
            }
        }

        [ContentSerializer(Optional = true)]
        public string Include { get; set; }

        [ContentSerializerIgnore]
        public List<int> TilesetIndices
        {
            get
            {
                return Tiles.Select(a => a.TilesetIndex).ToList();
            }
        }

        [ContentSerializerIgnore]
        public int Score
        {
            get;
            set;
        }

        private void Load()
        {
            if (MainApplication.IsManagerReady)
            {
                TileMatchPattern pattern = MainApplication.ManagerInstance.Content.Load<TileMatchPattern>("Content/Data/" + Include);

                _name = pattern.Name;
                _terrainname = pattern.TerrainName;
                _terrainNameAdjacent = pattern.TerrainNameAdjacent;
                _tiles = pattern.Tiles;
                _score = pattern.Score;
            }
        }

        public TileMatchPattern()
        {
        }
    }
}
