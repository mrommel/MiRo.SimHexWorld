using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using NUnit.Framework;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    [Serializable]
    public class HexPoint : IComparable<HexPoint>, ICloneable
    {
        private const int MaximalPathLength = 100;

        private const int SerializeConstant = 1024;

        public static HexPoint Zero
        {
            get { return new HexPoint(0, 0); }
        }

        public HexPoint() { }

        public HexPoint(int nx, int ny)
        {
            X = nx;
            Y = ny;
        }

        public static HexPoint FromReal(int x, int y)
        {
            int hx, hy;
            XyToHex(x, y, out hx, out hy);

            return new HexPoint(hx, hy);
        }

        [XmlIgnore()]
        public int RealX
        {
            get
            {
                int rx, ry;
                HexToXy(X, Y, out rx, out ry);
                return rx;
            }
        }

        [XmlIgnore()]
        public int RealY
        {
            get
            {
                int rx, ry;
                HexToXy(X, Y, out rx, out ry);
                return ry;
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public HexPoint Neighbor(HexDirection dir)
        {
            HexPoint neighbor = Clone();

            neighbor.MoveDir(dir);

            Assert.False(ReferenceEquals(this, neighbor), "references must be different");
            Assert.AreNotEqual(neighbor, this, "Neighbor cannot be equal to current object");

            return neighbor;
        }

        [XmlIgnore()]
        [ContentSerializer(Optional = true)]
        public List<HexPoint> Neighbors
        {
            get
            {
                //return HexDirection.All.Select(Neighbor).ToList();
                List<HexPoint> neighbors = new List<HexPoint>();

                foreach (HexDirection dir in HexDirection.All)
                {
                    HexPoint pt = Clone();
                    pt.MoveDir(dir);
                    neighbors.Add(pt);
                }

                return neighbors;
            }
        }

        public void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move(int x, int y)
        {
            X += x;
            Y += y;
        }

        public void MoveDir(HexDirection dir)
        {
            if (Y % 2 == 1) 
            {
                switch (dir)
                {
                    case HexDirection.NorthEast:
                        Move(0, -1);
                        break;
                    case HexDirection.East:
                        Move(1, 0);
                        break;
                    case HexDirection.SouthEast:
                        Move(0, 1);
                        break;
                    case HexDirection.SouthWest:
                        Move(-1, 1);
                        break;
                    case HexDirection.West:
                        Move(-1, 0);
                        break;
                    case HexDirection.NorthWest:
                        Move(-1, -1);
                        break;
                    default:
                        throw new Exception("Error");
                }
            }
            else
                switch (dir)
                {
                    case HexDirection.NorthEast:
                        Move(1, -1);
                        break;
                    case HexDirection.East:
                        Move(1, 0);
                        break;
                    case HexDirection.SouthEast:
                        Move(1, 1);
                        break;
                    case HexDirection.SouthWest:
                        Move(0, 1);
                        break;
                    case HexDirection.West:
                        Move(-1, 0);
                        break;
                    case HexDirection.NorthWest:
                        Move(0, -1);
                        break;
                    default:
                        throw new Exception("Error");
                }
        }

        public bool IsNeighbor(HexPoint pt)
        {
            return IsNeighbor(pt, HexDirection.NorthEast) ||
                   IsNeighbor(pt, HexDirection.East) ||
                   IsNeighbor(pt, HexDirection.SouthEast) ||
                   IsNeighbor(pt, HexDirection.SouthWest) ||
                   IsNeighbor(pt, HexDirection.West) ||
                   IsNeighbor(pt, HexDirection.NorthWest);
        }

        private bool IsNeighbor(HexPoint pt, HexDirection dir)
        {
            var tmp = new HexPoint(X, Y);

            tmp.MoveDir(dir);

            return pt == tmp;
        }

        // move rad
        private void Move(double theta)
        {
            int ny = RealY + (int)(Math.Sin(theta) * 34) + 27;

            int newx, newy;
            XyToHex(RealX + (int)(Math.Cos(theta) * 30) + 17, ny, out newx, out newy);

            X = newx;
            Y = newy;
        }

        // move deg
        private void Move(int theta)
        {
            int ny = RealY + (int)(Math.Sin(Deg2Rad(theta)) * 34) + 27;

            int newx, newy;
            XyToHex(RealX + (int)(Math.Cos(Deg2Rad(theta)) * 30) + 17, ny, out newx, out newy);

            X = newx;
            Y = newy;
        }

        /// <summary>
        /// estimates the distance between <para>hexPoint</para> and <para>pt</para>
        /// </summary>
        /// <param name="hexPoint">first point</param>
        /// <param name="pt">second point</param>
        /// <returns>estimated distance between the points</returns>
        public static int GetDistance(HexPoint hexPoint, HexPoint pt)
        {
            return GetDistance(hexPoint.X, hexPoint.Y, pt.X, pt.Y);
        }

        public static int GetDistance(int x1, int y1, int x2, int y2)
        {
            //int dist = 0;
            //var tmp = new HexPoint(x1, y1);
            //var toggled = new HexPoint(y2, x2);

            //while ((tmp.X != x2 && tmp.Y != y2) && (dist < MaximalPathLength))
            //{
            //    int theta = tmp.Angle(toggled) / 60 * 60 + 30;
            //    tmp.Move(theta);
            //    dist++;
            //}

            //return dist;
            int dx = x2 - x1;
            int dy = y2 - y1;
            if (Math.Sign(dx) == Math.Sign(dy))
            {    // this is (1); see first paragraph
                return Math.Max(Math.Abs(dx), Math.Abs(dy));
            }
            else
            {
                return Math.Abs(dx) + Math.Abs(dy);
            }
        }

        public static double Rad2Deg(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        public static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public HexPoint MoveTowards(HexPoint to)
        {
            double angle = Angle(to);

            Move(angle);

            return this;
        }

        private static int ReduceAngle(int ang)
        {
            while (ang >= 360) ang = ang - 360;
            while (ang < 0) ang = ang + 360;

            return ang;
        }

        public int Angle(HexPoint pt)
        {
            double dx = pt.RealX - RealX;
            double dy = pt.RealY - RealY;

            if (dy == 0)
                return ReduceAngle((dx >= 0) ? 0 : 180);
            else
            {
                double t = Rad2Deg(Math.Atan2(dx, dy));
                if (t >= 0)
                    return ReduceAngle((dy > 0) ? (int)(t + 270) : (int)(t - 90));
                else
                    return ReduceAngle((int)t + 270);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y); ;
        }

        public static bool operator ==(HexPoint pt1, HexPoint pt2)
        {
            if (ReferenceEquals(pt1, pt2))
                return true;

            return pt1.CompareTo(pt2) == 0;
        }

        public static bool operator !=(HexPoint pt1, HexPoint pt2)
        {
            if (ReferenceEquals(pt1, pt2))
                return false;

            if (ReferenceEquals(pt1, null) && ReferenceEquals(pt2, null))
                return false;

            if (ReferenceEquals(pt1, null))
                return true;

            return pt1.CompareTo(pt2) != 0;
        }

        public override int GetHashCode()
        {
            return Serialize();
        }

        #region IComparable Member

        public int Serialize()
        {
            return X + Y * SerializeConstant;
        }

        public int CompareTo(HexPoint hexPoint)
        {
            if (ReferenceEquals(null, hexPoint))
                return 1;

            return Serialize() - hexPoint.Serialize();
        }

        public int CompareTo(object obj)
        {
            if (obj is HexPoint)
                return CompareTo((HexPoint)obj);

            return 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return CompareTo(obj) == 0;
        }

        #endregion

        public bool Check(int w, int h)
        {
            return X >= 0 && Y >= 0 && X < w && Y < h;
        }

        public static void HexToXy(int hx, int hy, out int rx, out int ry)
        {
            if (HexPointSetting.Orientation == HexPointOrientation.Toggled)
            {
                rx = hx * HexPointSetting.Width;
                ry = hy * HexPointSetting.Height + ((hx % 2 == 0 ? 1 : 0) * HexPointSetting.ToogleOffset - HexPointSetting.Offset);
            }
            else
            {
                rx = hx * HexPointSetting.Width + ((hy % 2 == 0 ? 1 : 0) * HexPointSetting.ToogleOffset);
                ry = hy * HexPointSetting.Height - HexPointSetting.Offset;
            }

            return;
        }

        private static readonly double Sqrt3 = Math.Sqrt(3.0);
        public static void XyToHex(int rx, int ry, out int hx, out int hy)
        {
            if (HexPointSetting.Orientation == HexPointOrientation.Toggled)
            {
                int deltaHeight = HexPointSetting.Height / 2;
                int ergy;
                bool moved;

                //GrobRasterung
                int ergx = rx / HexPointSetting.Width;
                if (ergx % 2 == 1)
                {
                    moved = false;
                    ergy = ry / HexPointSetting.Height;
                }
                else
                {
                    moved = true;
                    ergy = (ry - deltaHeight) / HexPointSetting.Height;
                }

                // +-+--+--
                // |/   |\ <-F I
                // X    | x
                // |\ R |/ <-F II
                // +-+--+--
                //FehlerKorrektur 
                int crossPoint; //X
                if (moved)
                    crossPoint = ergy * HexPointSetting.Height + deltaHeight + (HexPointSetting.Height / 2);
                else
                    crossPoint = ergy * HexPointSetting.Height + (HexPointSetting.Height / 2);
                //Fehler I
                if ((ry < -Sqrt3 * (rx - ergx * HexPointSetting.Width) + crossPoint) && (moved))
                {
                    ergx--;
                }
                if ((ry < -Sqrt3 * (rx - ergx * HexPointSetting.Width) + crossPoint) && (!moved))
                {
                    ergx--;
                    ergy--;
                }
                //Fehler II
                if ((ry > Sqrt3 * (rx - ergx * HexPointSetting.Width) + crossPoint) && (moved))
                {
                    ergx--;
                    ergy++;
                }
                if ((ry > Sqrt3 * (rx - ergx * HexPointSetting.Width) + crossPoint) && (!moved))
                {
                    ergx--;
                }

                //Realisierung
                hx = (ergx < 0) ? 0 : ergx;
                hy = (ergy < 0) ? 0 : ergy;

                return;
            }
            else
            {
                //  FI  FII
                //+---+---+
                //|  / \  |
                //|/     \|
                //+       +
                //|       |
                //+       +
                //|\     /|
                //|  \ /  |
                //+---+---+

                int deltaWidth = HexPointSetting.Width / 2;
                int ergx;
                bool moved;

                ry -= 10;

                //GrobRasterung
                int ergy = ry / HexPointSetting.Height;
                if (ergy % 2 == 1)
                {
                    moved = false;
                    ergx = rx / HexPointSetting.Width;
                }
                else
                {
                    moved = true;
                    ergx = (rx - deltaWidth) / HexPointSetting.Width;
                }

                //FehlerKorrektur 
                int crossPoint; //X
                if (moved)
                    crossPoint = ergx * HexPointSetting.Width + deltaWidth;
                else
                    crossPoint = ergx * HexPointSetting.Width;

                //Fehler I
                double tmp = -(22.0 / 8.0) * (ry - ergy * HexPointSetting.Height) + crossPoint;

                if (((rx - deltaWidth) < tmp) && (moved))
                {
                    ergy--;
                }
                if (((rx - deltaWidth) < tmp) && (!moved))
                {
                    ergx--;
                    ergy--;
                }

                //Fehler II
                tmp = (22.0 / 8.0) * (ry - ergy * HexPointSetting.Height) + crossPoint;
                if (((rx - deltaWidth) > tmp) && (moved))
                {
                    ergy--;
                    ergx++;
                }
                if (((rx - deltaWidth) > tmp) && (!moved))
                {
                    ergy--;
                }

                //Realisierung
                hx = (ergx < 0) ? -1 : ergx;
                hy = (ergy < 0) ? -1 : ergy;

                //if( hx == 0 && hy == 0 && rx < 34 && ry < 40)
                //    logger.DebugFormat("test: {0}, {1} => {2}, {3} - {4}, {5} - {6}, {7} => {8}", rx, ry, ergx, ergy, crossPoint, tmp, hx, hy, (rx < tmp));
            }
        }

        #region ICloneable Member

        object ICloneable.Clone()
        {
            return null;
        }

        public HexPoint Clone()
        {
            return new HexPoint(X, Y);
        }

        #endregion

        public int DistanceTo(HexPoint loc)
        {
            return HexPoint.GetDistance(this, loc);
        }

        public List<HexPoint> GetNeighborhood(int size)
        {
            var neighborhood = new List<HexPoint>();

            if (size > -1)
                neighborhood.Add(this);

            if (size > 0)
                neighborhood.AddRange(Neighbors);

            while (size-- > 1)
            {
                IEnumerable<HexPoint> list = HexPoint.AddHexBorder(neighborhood);

                foreach (HexPoint pt in list)
                    if( !neighborhood.Contains(pt))
                        neighborhood.Add(pt);
            }

            return neighborhood;
        }

        public static IEnumerable<HexPoint> AddHexBorder(IEnumerable<HexPoint> list)
        {
            var result = new List<HexPoint>();

            foreach (HexPoint pt in list)
            {
                var successors = pt.Neighbors;

                foreach (HexPoint successor in successors)
                    if (!result.Contains(successor))
                        result.Add(successor);
            }

            return result;
        }

        private const double Angle01 = 20;
        private const double Angle02 = 90;
        private const double Angle03 = 160;
        private const double Angle04 = 200;
        private const double Angle05 = 270;
        private const double Angle06 = 340;

        public HexDirection GetDirection(HexPoint pt)
        {
            double angle = Angle(pt);

            angle %= 360;

            if (Angle01 < angle && angle <= Angle02)
                return HexDirection.NorthEast;
            if (Angle02 < angle && angle <= Angle03)
                return HexDirection.NorthWest;
            if (Angle03 < angle && angle <= Angle04)
                return HexDirection.West;
            if (Angle04 < angle && angle <= Angle05)
                return HexDirection.SouthWest;
            if (Angle05 < angle && angle <= Angle06)
                return HexDirection.SouthEast;
            return HexDirection.East;
        }

        public static HexPoint operator +(HexPoint pt1, HexPoint pt2)
        {
            return new HexPoint(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }
    }
}
