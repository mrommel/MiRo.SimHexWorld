using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;
using System;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class FlowPoint
    {
        [ContentSerializerIgnore]
        public HexPoint Point { get; set; }

        [ContentSerializerIgnore]
        public MapCell.FlowDirectionType Flow { get; set; }

        public FlowPoint(HexPoint pt, MapCell.FlowDirectionType flow)
        {
            Point = pt;
            Flow = flow;
        }

        public FlowPoint()
        {
        }
    }

    public class River
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        public List<FlowPoint> Points { get; set; }

        public River()
        { }

        public River(string name)
        {
            Name = name;
            Points = new List<FlowPoint>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}