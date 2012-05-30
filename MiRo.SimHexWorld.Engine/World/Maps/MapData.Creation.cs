using System;
using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.AiXGame.Maps;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.World.Helper;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Entities;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    partial class MapData
    {
        public enum HexCorner
        {
            North,
            NorthEast,
            SouthEast,
            South,
            SouthWest,
            NorthWest
        }

        public void Create(GameStartupSettings settings)
        {
            Random rnd = new Random();

            log.DebugFormat("Create( {0} )", settings);
            HeightMap heightmap = CreateHeightMap(settings);

            float seaLevel = heightmap.FindHeightLevel(settings.OceanRatio * 1.5f, 0.05f); // <-- Values from settings
            float shoreLevel = heightmap.FindHeightLevel(settings.OceanRatio, 0.05f); // <-- Values from settings
            float hillLevel = heightmap.FindHeightLevel(0.1f, 0.01f); // <-- Values from settings
            float mountainLevel = heightmap.FindHeightLevel(settings.PeakRatio, 0.01f); // <-- Values from settings

            OnProgress(new ProgressNotificationEventArgs(string.Format("Min: {0}, Sea: {1}, shoreLevel: {2}, mountain: {3}, max: {4}", heightmap.Minimum, seaLevel, shoreLevel, mountainLevel, heightmap.Maximum), 1f, 0.1f));

            Init(settings.Width, settings.Height);

            // assign coords
            for (int i = 0; i < settings.Width; ++i)
            {
                for (int j = 0; j < settings.Height; ++j)
                {
                    this[i, j].Height = heightmap[i, j];
                }
            }

            Terrain ocean = Provider.Instance.Terrains["Ocean"];
            Terrain shore = Provider.Instance.Terrains["Shore"]; // Shore
            Terrain coast = Provider.Instance.Terrains["Coast"];
            Terrain grassland = Provider.Instance.Terrains["Grassland"];

            Feature hills = Provider.Instance.Features["Hills"];
            Feature mountains = Provider.Instance.Features["Mountains"];

            // settle ground plains (ocean, grassland, mountain)
            RangeConversion<float, Terrain> conv = new RangeConversion<float, Terrain>(grassland);

            conv.AddMapping(heightmap.Minimum, seaLevel, ocean);
            conv.AddMapping(seaLevel, shoreLevel, shore);
            conv.AddMapping(shoreLevel, hillLevel, grassland);
            //conv.AddMapping(hillLevel, mountainLevel, hill);
            //conv.AddMapping(mountainLevel, heightmap.Maximum, mountain);

            for (int i = 0; i < settings.Width; ++i)
            {
                for (int j = 0; j < settings.Height; ++j)
                {
                    this[i, j].Terrain = conv.Find(heightmap[i, j]);

                    if(hillLevel <= heightmap[i,j] && heightmap[i,j] < mountainLevel)
                        this[i, j].Features.Add(hills);
                    else if(mountainLevel <= heightmap[i,j])
                        this[i, j].Features.Add(mountains);
                }
            }

            OnProgress(new ProgressNotificationEventArgs("Basic Terrain Assignment", 1f, 0.2f));

            // find coasts
            for (int i = 0; i < settings.Width; ++i)
            {
                for (int j = 0; j < settings.Height; ++j)
                {
                    if (this[i, j].IsOcean)
                    {
                        if (ShouldBeCoast(i, j))
                            this[i, j].Terrain = coast;
                    }
                }
            }

            OnProgress(new ProgressNotificationEventArgs("Find Coasts: " + GetArea(a => a.IsOcean).Count + " ocean tiles, " + GetArea(a => a.IsLand).Count + " land tiles", 1f, 0.3f));

            // find river tiles
            List<MapCell> hillsOrMountains = GetArea(a => a.IsSpring);

            int currentRiverId = 0;

            int numOfRivers = Math.Min(hillsOrMountains.Count, settings.NumOfRivers);
            Array corners = Enum.GetValues(typeof(HexCorner));
            while( currentRiverId < numOfRivers)
            {
                MapCell spring = hillsOrMountains.PickRandom();

                HexCorner randomCorner = (HexCorner)corners.GetValue(rnd.Next(corners.Length));

                HexPointCorner hxc = new HexPointCorner { Corner = randomCorner, Point = spring.Point };
                HexPointFlow[] flows = GetFlowByCorner(hxc);

                River currentriver = new River(string.Format("River{0}", currentRiverId));
                Rivers.Add(currentriver);
                DoRiver(flows[rnd.Next(3)], hxc, currentriver, 0);

                currentRiverId++;
            }

            OnProgress(new ProgressNotificationEventArgs("Add Rivers: " + GetArea(a => a.IsRiver).Count + " tiles", 1f, 0.3f));
        }

        #region river generation

        private void DoRiver(HexPointFlow hexPointFlow, HexPointCorner hxc, River river, int iteration)
        {
            if (river.Points.Count > 20 || iteration > 30) // paranoia
                return;

            if (!IsValid(hxc.Point))
                return;

            if( !river.Points.Contains(hxc.Point) )
                river.Points.Add(hxc.Point);

            this[hxc.Point].SetRiver(hexPointFlow.Flow);

            if (this[hxc.Point].IsOcean)
                return;

            List<HexPointFlow> flows = new List<HexPointFlow>(GetFlowByCorner(hxc));

            // remove not valid and current flow (resp. reverse flow)
            for (int i = 2; i > -1; i-- )
            {
                if (!IsValid(flows[i].Corner.Point) || Reverse(flows[i].Flow) == hexPointFlow.Flow || flows[i].Flow == hexPointFlow.Flow)
                    flows.RemoveAt(i);
            }

            if (flows.Count == 0)
                return;

            if( flows.Count == 1) 
            {
                DoRiver( new HexPointFlow { Corner = flows[0].Corner, Flow = flows[0].Flow }, flows[0].Corner, river, iteration + 1);
                return;
            }

            float height1 = GetHeightAtCorner( flows[0].Corner.Point.X, flows[0].Corner.Point.Y, flows[0].Corner.Corner);
            float height2 = GetHeightAtCorner( flows[1].Corner.Point.X, flows[1].Corner.Point.Y, flows[1].Corner.Corner);

            if(height1 < height2)
                DoRiver(new HexPointFlow { Corner = flows[0].Corner, Flow = flows[0].Flow }, flows[0].Corner, river, iteration + 1);
            else
                DoRiver(new HexPointFlow { Corner = flows[1].Corner, Flow = flows[1].Flow }, flows[1].Corner, river, iteration + 1);
        }

        public enum TurnType
        {
            Right, Left
        }

        public MapCell.FlowDirectionType Reverse(MapCell.FlowDirectionType type)
        {
            switch (type)
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
                    throw new Exception("Unknown flow " + type.ToString());
            }
        }

        public MapCell.FlowDirectionType TurnFlow(MapCell.FlowDirectionType type, TurnType turn)
        {
            switch (type)
            {
                case MapCell.FlowDirectionType.North:
                    return turn == TurnType.Right
                               ? MapCell.FlowDirectionType.NorthEast
                               : MapCell.FlowDirectionType.NorthWest;
                case MapCell.FlowDirectionType.NorthEast:
                    return turn == TurnType.Right
                               ? MapCell.FlowDirectionType.SouthEast
                               : MapCell.FlowDirectionType.North;
                case MapCell.FlowDirectionType.SouthEast:
                    return turn == TurnType.Right
                               ? MapCell.FlowDirectionType.South
                               : MapCell.FlowDirectionType.NorthEast;
                case MapCell.FlowDirectionType.South:
                    return turn == TurnType.Right
                               ? MapCell.FlowDirectionType.SouthWest
                               : MapCell.FlowDirectionType.SouthEast;
                case MapCell.FlowDirectionType.SouthWest:
                    return turn == TurnType.Right
                              ? MapCell.FlowDirectionType.NorthWest
                              : MapCell.FlowDirectionType.South;
                case MapCell.FlowDirectionType.NorthWest:
                    return turn == TurnType.Right
                               ? MapCell.FlowDirectionType.North
                               : MapCell.FlowDirectionType.SouthWest;
            }

            return MapCell.FlowDirectionType.NoFlowdirection;
        }

        private HexPointFlow[] GetFlowByCorner(HexPointCorner corner)
        {
            switch (corner.Corner)
            {
                //     0
                //     ^
                //     |
                //    (X) <= corner
                //    / \
                //   v   v
                //  1     2
                //  |  T  |
                case HexCorner.North:
                    return new HexPointFlow[]
                               {
                                   new HexPointFlow
                                       {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthWest), 
                                                            Corner = HexCorner.NorthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.North
                                       }, // 0
                                   new HexPointFlow 
                                       { 
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthWest),
                                                            Corner = HexCorner.South
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.SouthWest 
                                       }, // 1
                                   new HexPointFlow
                                       {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthEast), 
                                                            Corner = HexCorner.South
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.SouthEast
                                       } // 2
                               };
                //      |
                //      +       +
                //     / 0     1
                //    /   ^   ^
                //   /     \ /
                //  +      (X) <= corner
                //  |   T   |
                //  |       v
                //  |       2
                case HexCorner.NorthEast:
                    return new HexPointFlow[]
                               {
                                   new HexPointFlow 
                                       {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthEast),
                                                            Corner = HexCorner.SouthWest
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.NorthWest 
                                       }, // 0
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthEast),
                                                            Corner = HexCorner.SouthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.SouthWest 
                                   }, // 1
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point,
                                                            Corner = HexCorner.SouthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.South 
                                   } // 2
                               };
                //  |       0   
                //  |       ^
                //  |   T   |
                //  +      (X) <= corner
                //   \     / \
                //    \   v   v 
                //     \ 2     1 
                //      +       +
                case HexCorner.SouthEast:
                    return new HexPointFlow[]
                               {
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point,
                                                            Corner = HexCorner.NorthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.North 
                                   }, // 0
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.East),
                                                            Corner = HexCorner.South
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.NorthWest 
                                   }, // 1
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point,
                                                            Corner = HexCorner.South
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.SouthWest 
                                   } // 2
                               };
                //  |   T   |
                //  +       +
                //   2     0
                //    ^   ^ 
                //     \ /  
                //     (X) <= corner
                //      |
                //      v
                //      1
                case HexCorner.South:
                    return new HexPointFlow[]
                               {
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point,
                                                            Corner = HexCorner.SouthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.NorthEast 
                                   }, // 0
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.SouthWest),
                                                            Corner = HexCorner.SouthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.South 
                                   }, // 1
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point,
                                                            Corner = HexCorner.SouthWest
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.NorthWest 
                                   } // 2
                               };
                //  |       0   
                //  |       ^
                //  |       |    T
                //  +      (X) <= corner
                //   \     / \
                //    \   v   v 
                //     \ 2     1 
                //      +       +
                case HexCorner.SouthWest:
                    return new HexPointFlow[]
                               {
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.West),
                                                            Corner = HexCorner.NorthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.North 
                                   }, // 0
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point,
                                                            Corner = HexCorner.South
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.SouthEast 
                                   }, // 1
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.West),
                                                            Corner = HexCorner.South
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.SouthWest 
                                   } // 2
                               };
                //      |
                //      +       +
                //     / 0     1
                //    /   ^   ^
                //   /     \ /
                //  +      (X) <= corner
                //  |       |    T
                //  |       v
                //  |       2
                case HexCorner.NorthWest:
                    return new HexPointFlow[]
                               {
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthWest),
                                                            Corner = HexCorner.SouthWest
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.NorthWest 
                                   }, // 0
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.NorthWest),
                                                            Corner = HexCorner.SouthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.NorthEast 
                                   }, // 1
                                   new HexPointFlow 
                                   {
                                           Corner = new HexPointCorner
                                                        {
                                                            Point = corner.Point.Neighbor(HexDirection.West),
                                                            Corner = HexCorner.SouthEast
                                                        }, 
                                           Flow = MapCell.FlowDirectionType.South 
                                   } // 2
                               };
                default:
                    throw new Exception();
            }
        }

        private float GetHeightAtCorner(int x, int y, HexCorner corner)
        {
            HexPoint pt = new HexPoint(x, y);

            switch (corner)
            {
                case HexCorner.SouthEast:
                    {
                        HexPoint se = pt.Neighbor(HexDirection.SouthEast);
                        HexPoint ea = pt.Neighbor(HexDirection.East);

                        if (IsValid(se) && IsValid(ea))
                            return (this[se].Height + this[ea].Height + Height) / 3.0f;

                        if (IsValid(se))
                            return (this[se].Height + Height) / 2.0f;

                        if (IsValid(ea))
                            return (this[ea].Height + Height) / 2.0f;

                        return Height;
                    }
                case HexCorner.NorthEast:
                    {
                        HexPoint ne = pt.Neighbor(HexDirection.NorthEast);
                        HexPoint ea = pt.Neighbor(HexDirection.East);

                        if (IsValid(ne) && IsValid(ea))
                            return (this[ne].Height + this[ea].Height + Height) / 3.0f;

                        if (IsValid(ne))
                            return (this[ne].Height + Height) / 2.0f;

                        if (IsValid(ea))
                            return (this[ea].Height + Height) / 2.0f;

                        return Height;
                    }
                case HexCorner.North:
                    {
                        HexPoint ne = pt.Neighbor(HexDirection.NorthEast);
                        HexPoint nw = pt.Neighbor(HexDirection.NorthWest);

                        if (IsValid(ne) && IsValid(nw))
                            return (this[ne].Height + this[nw].Height + Height) / 3.0f;

                        if (IsValid(ne))
                            return (this[ne].Height + Height) / 2.0f;

                        if (IsValid(nw))
                            return (this[nw].Height + Height) / 2.0f;

                        return Height;
                    }
                case HexCorner.SouthWest:
                    {
                        HexPoint sw = pt.Neighbor(HexDirection.SouthWest);
                        HexPoint we = pt.Neighbor(HexDirection.West);

                        if (IsValid(sw) && IsValid(we))
                            return (this[sw].Height + this[we].Height + Height) / 3.0f;

                        if (IsValid(sw))
                            return (this[sw].Height + Height) / 2.0f;

                        if (IsValid(we))
                            return (this[we].Height + Height) / 2.0f;

                        return Height;
                    }
                case HexCorner.NorthWest:
                    {
                        HexPoint nw = pt.Neighbor(HexDirection.NorthWest);
                        HexPoint we = pt.Neighbor(HexDirection.West);

                        if (IsValid(nw) && IsValid(we))
                            return (this[nw].Height + this[we].Height + Height) / 3.0f;

                        if (IsValid(nw))
                            return (this[nw].Height + Height) / 2.0f;

                        if (IsValid(we))
                            return (this[we].Height + Height) / 2.0f;

                        return Height;
                    }
                case HexCorner.South:
                    {
                        HexPoint se = pt.Neighbor(HexDirection.SouthEast);
                        HexPoint sw = pt.Neighbor(HexDirection.SouthWest);

                        if (IsValid(se) && IsValid(sw))
                            return (this[se].Height + this[sw].Height + Height) / 3.0f;

                        if (IsValid(se))
                            return (this[se].Height + Height) / 2.0f;

                        if (IsValid(sw))
                            return (this[sw].Height + Height) / 2.0f;

                        return Height;
                    }
            }

            return 0f;
        }

        private class HexPointCorner
        {
            public HexPoint Point;
            public HexCorner Corner;
        }

        private class HexPointFlow
        {
            public HexPointCorner Corner;
            public MapCell.FlowDirectionType Flow;
        }

        #endregion river generation

        private HeightMap CreateHeightMap(GameStartupSettings settings)
        {
            HeightMap heightmap = new HeightMap(256, HeightMap.Overlap / 2, HeightMap.Overlap / 2);

            heightmap.MakeFlat(0.1f);
            heightmap.SetNoise(0.22f); // 0.3 makes smaller structure, 0.1 makes blobs
            heightmap.Erode(10);
            heightmap.Smoothen();
            //heightmap.Smoothen();

            return heightmap;
        }

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event ProgressNotificationEventHandler ProgressNotificationChanged;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnProgress(ProgressNotificationEventArgs e)
        {
            if (ProgressNotificationChanged != null)
                ProgressNotificationChanged(this, e);
        }

        internal string GetRegionNames(HexPoint pt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (MapRegion mr in Extension.Regions)
                if (mr.IsInside(pt) && this[pt].IsOcean == mr.IsOcean)
                    sb.Append(mr.Name + ", ");

            return sb.ToString();
        }

        public float GetValue(int x, int y, MapValueType mapValueType)
        {
            if( !IsValid(x,y))
                return 0f;

            switch (mapValueType)
            {
                case MapValueType.CityFoundValue:
                    float sum = 0f;

                    HexPoint center = new HexPoint(x,y);
                    foreach( HexPoint pt in center.Neighbors )
                        if( IsValid(pt) )
                            sum += this[pt].Food;

                    return this[x, y].Food + sum / 2;
            }

            return 0f;
        }

        static Pathfinder finder;
        public Path FindPath(Unit unit, HexPoint pt)
        {
            if (finder == null)
                finder = new Pathfinder(this);

            return finder.CalculatePath(unit, unit.Point, pt);
        }

        public ICollection<IGridCell> GetNeighbourCells(IGridCell iGridCell)
        {
            List<IGridCell> cells = new List<IGridCell>();

            HexPoint cellPoint = new HexPoint(iGridCell.X,iGridCell.Y);
            foreach (HexPoint pt in cellPoint.Neighbors)
            {
                if (IsValid(pt))
                    cells.Add(this[pt]);
            }

            return cells;
        }
    }

    public class ProgressNotificationEventArgs : EventArgs
    {
        public float OverallProgress { get; set; }
        public float DetailProgress { get; set; }
        public string DetailName { get; set; }

        public ProgressNotificationEventArgs(string name, float progress, float allProgress)
        {
            DetailName = name;
            DetailProgress = progress;
            OverallProgress = allProgress;
        }
    }

    // A delegate type for hooking up change notifications.
    public delegate void ProgressNotificationEventHandler(object sender, ProgressNotificationEventArgs e);
}
