using System;
using System.Collections.Generic;
using System.Linq;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class MapChangeArgs : EventArgs
    {
        readonly MapData _map;
        readonly List<HexPoint> _updatedTiles;

        public MapChangeArgs(MapData map)
        {
            _map = map;

            _updatedTiles = new List<HexPoint>();
        }

        public MapChangeArgs(MapData map, List<HexPoint> updatedTiles)
        {
            _map = map;

            _updatedTiles = updatedTiles;
        }

        public MapChangeArgs(MapData map, HexPoint updatedTile)
        {
            _map = map;

            _updatedTiles = new List<HexPoint>();
            _updatedTiles.Add(updatedTile);
        }

        public MapData Map
        {
            get
            {
                return _map;
            }
        }

        public IEnumerable<HexPoint> UpdatedTiles
        {
            get
            {
                return _updatedTiles;
            }
        }

        public override string ToString()
        {
            return "Update Map at: " + string.Join(", ", _updatedTiles.Select(a => a.ToString()));
        }
    }
}
