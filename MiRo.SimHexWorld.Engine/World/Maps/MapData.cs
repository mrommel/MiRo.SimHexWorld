using System;
using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.World.Helper;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using log4net;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Instance;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    public enum MapValueType { CityFoundValue };

    public partial class MapData
    {
        ILog log = LogManager.GetLogger(typeof(MapData));
        
        int _width, _height;
        private ContinentList _continents = null;
        private MapSize _size;

        public MapExtension Extension = new MapExtension();

        // events
        public event MapUpdateHandler MapUpdate;

        /// <summary>
        /// The name of the world.
        /// </summary>
        private string _name;

        /// <summary>
        /// Spatial array for the ground tiles for this map.
        /// </summary>
        private MapCell[] _tiles;

        List<River> Rivers = new List<River>(); 

        internal MapData()
        {
        }

        public MapData(MapSize size)
        {
            _size = size;
            Init(size.Size.Width, size.Size.Height);
        }

        private void Init(int width, int height)
        {
            _width = width;
            _height = height;

            if (_size == null)
            {
                MapSize bestMapSize = new MapSize();
                int bestValue = int.MaxValue;

                foreach (MapSize ms in Provider.Instance.MapSizes.Values)
                {
                    int dist = (ms.Size.Width - Width) * (ms.Size.Width - Width) + (ms.Size.Height - Height) * (ms.Size.Height - Height);
                    if (dist < bestValue)
                    {
                        bestValue = dist;
                        bestMapSize = ms;
                    }
                }
             
                _size = bestMapSize;
            }

            _tiles = new MapCell[_width * _height];

            Fill("Ocean");
        }

        public void Fill(string terrainName)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    this[x, y] = new MapCell(x,y,Provider.Instance.Terrains.FirstOrDefault(a => a.Key == terrainName).Value);
                }
            }
        }

        #region Description

        /// <summary>
        /// The name of this section of the world.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Dimensions

        public MapSize Size
        {
            get
            { return _size; }
        }

        public Point Dimensions
        {
            get
            {
                return new Point(_width, _height);
            }
            set
            {
                _width = value.X;
                _height = value.Y;
            }
        }

        [XmlIgnore]
        public int Width
        {
            get
            {
                return _width;
            }
        }

        [XmlIgnore]
        public int Height
        {
            get
            {
                return _height;
            }
        }

        #endregion dimensions

        public void UpdateContinents()
        {
            _continents = ContinentList.Create(this);
        }

        #region Tiles

        /// <summary>
        /// Retrieves the base layer value for the given map position.
        /// </summary>
        public MapCell this[int x, int y]
        {
            get
            {
                // check the parameter
                if ((x < 0) || (x >= _width))
                    throw new ArgumentOutOfRangeException(String.Format("x: {0} not in ({1},{2})", x, 0, _width));

                if ((y < 0) || (y >= _height))
                    throw new ArgumentOutOfRangeException(String.Format("y: {0} not in ({1},{2})", y, 0, _height));

                return _tiles[y * _width + x];
            }

            set
            {
                // check the parameter
                if ((x < 0) || (x >= _width))
                    throw new ArgumentOutOfRangeException(String.Format("x: {0} not in ({1},{2})", x, 0, _width));

                if ((y < 0) || (y >= _height))
                    throw new ArgumentOutOfRangeException(String.Format("y: {0} not in ({1},{2})", y, 0, _height));

                _tiles[y * _width + x] = value;

                if (MapUpdate != null)
                    MapUpdate(new MapChangeArgs(this, new HexPoint(x,y)));
            }
        }

        public MapCell this[HexPoint pt]
        {
            get 
            {
                return this[pt.X, pt.Y];
            }
        }

        public MapCell[] Tiles
        {
            get
            {
                return _tiles;
            }
        }

        #endregion Tiles    

        #region river

        public River GetRiverAt(HexPoint point)
        {
            if (!IsValid(point))
                return null;

            if( this[point].IsRiver)
                return Rivers.FirstOrDefault(river => river.Points.Select( a => a.Point ).Contains(point));

            HexPoint neighbor = point.Neighbor(HexDirection.NorthEast);
            if (IsValid(neighbor) && this[neighbor].IsNEOfRiver)
                return Rivers.FirstOrDefault(river => river.Points.Select(a => a.Point).Contains(neighbor));

            neighbor = point.Neighbor(HexDirection.West);
            if (IsValid(neighbor) && this[neighbor].IsWOfRiver)
                return Rivers.FirstOrDefault(river => river.Points.Select(a => a.Point).Contains(neighbor));

            neighbor = point.Neighbor(HexDirection.NorthWest);
            if (IsValid(neighbor) && this[neighbor].IsNWOfRiver)
                return Rivers.FirstOrDefault(river => river.Points.Select(a => a.Point).Contains(neighbor));

            return null;
        }

        #endregion river

        #region area handling

        public List<MapCell> GetArea(Func<MapCell, bool> func)
        {
            List<MapCell> set = new List<MapCell>();

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if (func(this[i, j]))
                        set.Add(this[i, j]);
                }
            }

            return set;
        }

        public float[] GetSerilizedData(Func<MapCell, float> func)
        {
            float[] data = new float[_width * _height];

            int c = 0;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    data[c++] = func(this[i,j]);
                }
            }

            return data;
        }

        #endregion area handling

        static float sx = 0.157f; 
        static float sy = 0.273f; 
        static readonly Matrix MapProjection = Matrix.CreateScale(new Vector3(sx, 0, sy));
        static readonly Matrix MapUnprojection = Matrix.CreateScale(new Vector3(1f / sx, 0, 1f / sy));

        /// <summary>
        /// 3D position from tile position
        /// </summary>
        /// <param name="tilex"></param>
        /// <param name="tiley"></param>
        /// <returns></returns>
        public static Vector3 GetWorldPosition(int tilex, int tiley)
        {
            int rx, ry;
            HexPoint.HexToXy(tilex, tiley, out rx, out ry);

            return Vector3.Transform(new Vector3(rx, 0, ry), MapProjection);
        }

        public static Vector3 GetWorldPosition(HexPoint pt)
        {
            int rx, ry;
            HexPoint.HexToXy(pt.X, pt.Y, out rx, out ry);

            return Vector3.Transform(new Vector3(rx, 0, ry), MapProjection);
        }

        public static HexPoint GetMapPosition(Vector3 position)
        {
            Vector3 tmp = Vector3.Transform(position, MapUnprojection);
            int hx, hy;
            HexPoint.XyToHex((int)tmp.X, (int)tmp.Z, out hx, out hy);
            return new HexPoint(hx, hy);
        }

        public int GetCoastalTileIndex(int i, int j)
        {
            // return rnd.Next(32);

            int index = 0, c = 1;
            var loc = new HexPoint(i, j);

            foreach (HexDirection dir in HexDirection.All /*.Shift(6)*/)
            {
                HexPoint pt = loc.Neighbor(dir);
                if (IsValid(pt))
                {
                    if ((this[pt].IsLand)) // may coast should be removed
                        index += c;
                }

                c *= 2;
            }

            return index;
        }
        
        public int GetBorderTileIndex(int i, int j)
        {
            int currentController = this[i,j].ControlledBy;

            if (currentController == -1)
                return 0;

            int index = 0, c = 1;
            var loc = new HexPoint(i, j);
            
            foreach (HexDirection dir in HexDirection.All)
            {
                HexPoint pt = loc.Neighbor(dir);
                if (IsValid(pt))
                {
                    if ((this[pt].ControlledBy != currentController)) 
                        index += c;
                }

                c *= 2;
            }

            return index;
        }

        Improvement road = Provider.GetImprovement("Road");
        public int GetRoadTileIndex(int i, int j)
        {
            if (!this[i,j].Improvements.Contains(road))
                return -1;

            int index = 0, c = 1;
            var loc = new HexPoint(i, j);

            foreach (HexDirection dir in HexDirection.All)
            {
                HexPoint pt = loc.Neighbor(dir);
                if (IsValid(pt))
                {
                    if (this[pt].Improvements.Contains(road))
                        index += c;
                }

                c *= 2;
            }

            return index;
        }

        public bool ShouldBeCoast(int i, int j)
        {
            HexPoint pt = new HexPoint(i, j);
            bool hasOcean = false, hasLand = false;

            foreach (HexPoint p in pt.Neighbors)
            {
                if (IsValid(p))
                {
                    if (this[p].IsOcean)
                        hasOcean = true;

                    if (this[p].IsLand)
                        hasLand = true;
                }
            }

            return hasOcean && hasLand;
        }

        public bool IsValid(HexPoint pt)
        {
            return IsValid(pt.X, pt.Y);
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        public void OnMapSpotting(MapSpottingArgs args)
        {
            GameFacade.getInstance().SendNotification(GameNotification.UpdateSpotting, args);
        }

        public void OnMapControlling(MapControllingArgs args)
        {
            GameFacade.getInstance().SendNotification(GameNotification.UpdateMapControlling, args);
        }

        public void OnMapExploiting(MapControllingArgs args)
        {
            GameFacade.getInstance().SendNotification(GameNotification.UpdateMapExploiting, args);
        }

        public void SetControlled(HexPoint point, int id, bool doOverride = false, int neighborhood = 0)
        {
            this[point].ControlledBy = id;

            if (neighborhood > 0)
            {
                List<HexPoint> pts = point.GetNeighborhood(neighborhood);

                foreach (HexPoint pt in pts)
                {
                    if (IsValid(pt))
                    {
                        if (!doOverride)
                        {
                            if (this[pt].ControlledBy == -1)
                                this[pt].ControlledBy = id;
                        }
                        else
                            this[pt].ControlledBy = id;
                    }

                }
            }
        }

        public MapData Clone()
        {
            MapData clone = new MapData();

            clone.Name = Name;
            clone.Init(Width, Height);

            clone._tiles = _tiles.Clone() as MapCell[];

            return clone;
        }
    }
}
