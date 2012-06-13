using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;
using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using System.Globalization;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public class FlowPoint : IEquatable<FlowPoint>
    {
        public HexPoint Point { get; set; }
        public MapCell.FlowDirectionType Flow { get; set; }

        public FlowPoint(HexPoint pt, MapCell.FlowDirectionType flow)
        {
            Point = pt;
            Flow = flow;
        }

        public FlowPoint()
        {
            Point = new HexPoint();
            Flow = MapCell.FlowDirectionType.NoFlowdirection;
        }

        public FlowPoint Right
        {
            get 
            {
                switch( Flow )
                {
                    case MapCell.FlowDirectionType.North:
                        return new FlowPoint(Point.Neighbor(HexDirection.NorthEast), MapCell.FlowDirectionType.NorthEast);
                    case MapCell.FlowDirectionType.South:
                        return new FlowPoint(Point, MapCell.FlowDirectionType.SouthWest);
                    case MapCell.FlowDirectionType.NorthEast:
                        return new FlowPoint(Point.Neighbor(HexDirection.East), MapCell.FlowDirectionType.SouthEast);
                    case MapCell.FlowDirectionType.SouthWest:
                        return new FlowPoint(Point, MapCell.FlowDirectionType.NorthWest);
                    case MapCell.FlowDirectionType.NorthWest:
                        return new FlowPoint(Point.Neighbor(HexDirection.West), MapCell.FlowDirectionType.North);
                    case MapCell.FlowDirectionType.SouthEast:
                        return new FlowPoint(Point.Neighbor(HexDirection.SouthWest), MapCell.FlowDirectionType.South);
                    default:
                        return new FlowPoint(Point, MapCell.FlowDirectionType.NoFlowdirection);
                }
            }
        }

        public FlowPoint Left
        {
            get
            {
                switch (Flow)
                {
                    case MapCell.FlowDirectionType.North:
                        return new FlowPoint(Point.Neighbor(HexDirection.NorthEast), MapCell.FlowDirectionType.NorthWest);
                    case MapCell.FlowDirectionType.South:
                        return new FlowPoint(Point.Neighbor(HexDirection.East), MapCell.FlowDirectionType.SouthEast);
                    case MapCell.FlowDirectionType.NorthEast:
                        return new FlowPoint(Point, MapCell.FlowDirectionType.North);
                    case MapCell.FlowDirectionType.SouthWest:
                        return new FlowPoint(Point.Neighbor(HexDirection.SouthWest), MapCell.FlowDirectionType.South);
                    case MapCell.FlowDirectionType.NorthWest:
                        return new FlowPoint(Point.Neighbor(HexDirection.West), MapCell.FlowDirectionType.SouthWest);
                    case MapCell.FlowDirectionType.SouthEast:
                        return new FlowPoint(Point, MapCell.FlowDirectionType.NorthEast);
                    default:
                        return new FlowPoint(Point, MapCell.FlowDirectionType.NoFlowdirection);
                }
            }
        }

        public static MapCell.FlowDirectionType Reverse(MapCell.FlowDirectionType v)
        {
            switch (v)
            {
                case MapCell.FlowDirectionType.North:
                    return MapCell.FlowDirectionType.South;
                case MapCell.FlowDirectionType.South:
                    return MapCell.FlowDirectionType.North;
                case MapCell.FlowDirectionType.SouthEast:
                    return MapCell.FlowDirectionType.NorthWest;
                case MapCell.FlowDirectionType.NorthWest:
                    return MapCell.FlowDirectionType.SouthEast;
                case MapCell.FlowDirectionType.SouthWest:
                    return MapCell.FlowDirectionType.NorthEast;
                case MapCell.FlowDirectionType.NorthEast:
                    return MapCell.FlowDirectionType.SouthWest;
                default:
                    return MapCell.FlowDirectionType.NoFlowdirection;
            }
        }

        public bool Equals(FlowPoint other)
        {
            return Point == other.Point && ( Flow == other.Flow || Reverse(Flow) == other.Flow);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(FlowPoint fp1, FlowPoint fp2)
        {
            return fp1.Equals(fp2);
        }

        public static bool operator !=(FlowPoint fp1, FlowPoint fp2)
        {
            return !fp1.Equals(fp2);
        }
    }

    public class River
    {
        public string Name { get; set; }

        [ContentSerializerIgnore]
        public List<FlowPoint> Points { get; set; }

        public River()
        { }

        public River(string name)
        {
            Name = name;
            Points = new List<FlowPoint>();
        }

        public List<string> Data 
        { 
            get 
            {
                List<string> data = new List<string>();

                foreach (FlowPoint pt in Points)
                    data.Add(string.Format("{0} {1} {2}",pt.Point.X, pt.Point.Y, pt.Flow));

                return data;
            }
            set
            {
                Points = new List<FlowPoint>();

                foreach (string line in value)
                {
                    string[] parts = line.Split(' ');

                    FlowPoint fp = new FlowPoint();

                    fp.Point = new HexPoint(int.Parse(parts[0]), int.Parse(parts[1]));
                    fp.Flow = (MapCell.FlowDirectionType)Enum.Parse(typeof(MapCell.FlowDirectionType), parts[2]);

                    Points.Add(fp);
                }
            }
        }

        public override string ToString()
        {
            return Name + " => " + Points.Count;
        }

        public bool IsConnected(FlowPoint fpToTest)
        {
            foreach (FlowPoint fpInner in Points)
            {
                if (fpInner.Right == fpToTest)
                    return true;

                if (fpInner.Left == fpToTest)
                    return true;

                if (fpToTest.Right == fpInner)
                    return true;

                if (fpToTest.Left == fpInner)
                    return true;
            }

            return false;
        }

        public bool IsConnected(River rInner)
        {
            foreach (FlowPoint fp in Points)
                if (rInner.IsConnected(fp))
                    return true;

            return false;
        }

        public void Join(River r2)
        {
            Points.AddRange(r2.Points);
        }

        public class BinRiverTreeItem
        {
            public FlowPoint Data { get; set; }
            public BinRiverTreeItem Left { get; set; }
            public BinRiverTreeItem Right { get; set; }

            public int Length
            {
                get { return 1 + Math.Max( Left != null ? Left.Length : 0, Right != null ? Right.Length : 0); }
            }

            public bool Insert(FlowPoint item)
            {
                if (Data.Right == item)
                {
                    Right = new BinRiverTreeItem();
                    Right.Data = item;
                    return true;
                }

                if (Data.Left == item)
                {
                    Left = new BinRiverTreeItem();
                    Left.Data = item;
                    return true;
                }

                if (Left != null && Left.Insert(item))
                    return true;

                if (Right != null && Right.Insert(item))
                    return true;

                return false;
            }
        }

        public class BinRiverTree
        {
            private BinRiverTreeItem root;

            public bool Insert(FlowPoint item)
            {
                if (root == null)
                {
                    root = new BinRiverTreeItem();
                    root.Data = item;
                    return true;
                }

                if (item.Right == root.Data)
                {
                    BinRiverTreeItem newRoot = new BinRiverTreeItem();
                    newRoot.Data = item;
                    newRoot.Right = root;
                    root = newRoot;
                    return true;
                }

                if (item.Left == root.Data)
                {
                    BinRiverTreeItem newRoot = new BinRiverTreeItem();
                    newRoot.Data = item;
                    newRoot.Left = root;
                    root = newRoot;
                    return true;
                }

                if (root.Insert(item))
                    return true;

                return false;
            }

            public List<FlowPoint> Points 
            {
                get
                {
                    List<FlowPoint> pts = new List<FlowPoint>();

                    BinRiverTreeItem current = root;

                    while (current != null)
                    {
                        pts.Add(current.Data);

                        int rightLength = current.Right != null ? current.Right.Length : 0;
                        int leftLength = current.Left != null ? current.Left.Length : 0;

                        if (current.Right != null && leftLength < rightLength)
                            current = current.Right;
                        else if (current.Left != null && leftLength >= rightLength)
                            current = current.Left;
                        else
                            current = null;
                    }

                    return pts;
                }
            }
        }

        static Random rnd = new Random();
        public void Clean()
        {
            BinRiverTree binRiver = new BinRiverTree();
            int count = 100;

            while (Points.Count > 0 && count > 0)
            {
                FlowPoint item = Points[rnd.Next(Points.Count)];

                if (binRiver.Insert(item))
                    Points.Remove(item);

                count--;
            }

            Points = binRiver.Points;
        }

        public int Length { get { return Points.Count; } }
    }
}