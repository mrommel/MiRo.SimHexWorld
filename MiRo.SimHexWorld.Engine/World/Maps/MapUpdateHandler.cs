using System;
using MiRo.SimHexWorld.Engine.World.Entities;
using System.Collections.Generic;
namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public delegate void MapUpdateHandler(MapChangeArgs args);
    public delegate void MapSpottingHandler(MapSpottingArgs args);
    public delegate void MapControllingHandler(MapControllingArgs args);

    public delegate void CityOpenHandler(City city );
    public delegate void UnitsSelectHandler(List<Unit> unit);

    public class MapSpottingArgs : EventArgs
    {
        readonly MapData _map;
        readonly HexPoint _tile;
        readonly bool _spotting;

        public MapSpottingArgs(MapData map, HexPoint point, bool spotting)
        {
            _map = map;
            _tile = point;
            _spotting = spotting;
        }

        public MapData Map
        {
            get
            {
                return _map;
            }
        }

        public override string ToString()
        {
            return "";
        }
    }

    public class MapControllingArgs : EventArgs
    {
        readonly MapData _map;
        readonly HexPoint _tile;
        readonly int _controller;

        public MapControllingArgs(MapData map, HexPoint point, int controller)
        {
            _map = map;
            _tile = point;
            _controller = controller;
        }

        public MapData Map
        {
            get
            {
                return _map;
            }
        }

        public override string ToString()
        {
            return "";
        }
    }
}