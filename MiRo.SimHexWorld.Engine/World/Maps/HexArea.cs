using System;
using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.World.Maps;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class HexArea : List<HexPoint>
    {
        readonly MapData _map;

        public HexArea(MapData map)
        {
            _map = map;
        }

        public HexArea(MapData map, int x, int y, int w, int h)
        {
            _map = map;

            for( int i = 0; i < w; ++i )
                for( int j = 0; j < h; ++j )
                    Add( new HexPoint(x + i, y + j) );
        }

        public float Sum(Func<MapCell, float> func)
        {
            return this.Where(pt => _map.IsValid(pt)).Sum(pt => func(_map[pt]));
        }

        public HexPoint BestPosition(Func<MapCell, float> func)
        {
            float maxValue = float.MinValue;
            var maxPosition = new HexPoint();

            foreach (HexPoint pt in this)
            {
                if( _map.IsValid(pt))
                {
                    float v = func(_map[pt]);

                    if (v > maxValue)
                    {
                        maxValue = v;
                        maxPosition = pt;
                    }
                }
            }

            return maxPosition;
        }
    }
}
