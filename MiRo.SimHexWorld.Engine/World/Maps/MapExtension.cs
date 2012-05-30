using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class MapRegion
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsOcean { get; set; }

        public List<string> RessourceExcludes { get; set; }

        public bool IsInside(HexPoint pt)
        {
            return X <= pt.X
                && pt.X <= (X + Width)
                && Y <= pt.Y
                && pt.Y <= (Y + Height);
        }
    }

    public class StartLocation
    {
        public string CivilizationName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class MapExtension
    {
        public string MapName { get; set; }
        public List<MapRegion> Regions { get; set; }
        public List<StartLocation> StartLocations { get; set; }
    }
}
