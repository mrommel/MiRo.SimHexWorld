using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class MapRegion
    {
        private string _name;
        public string Name 
        {
            get
            {
                if (_name.StartsWith("TXT_KEY_") && Provider.CanTranslate)
                    return Provider.Instance.Translate(_name);
                else
                    return _name;
            }
            set
            { 
                _name = value; 
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsOcean { get; set; }

        public List<string> RessourceExcludes { get; set; }

        public bool IsInside(HexPoint pt)
        {
            return IsInside(pt.X, pt.Y);
        }

        internal bool IsInside(int x, int y)
        {
            return X <= x
                 && x <= (X + Width)
                 && Y <= y
                 && y <= (Y + Height);
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
        public List<River> Rivers { get; set; }
        public List<StartLocation> StartLocations { get; set; }
    }
}
