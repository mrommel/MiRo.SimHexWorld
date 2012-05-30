using System;
using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.World.Maps;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    /// <summary>
    /// list of continents
    /// </summary>
    public class ContinentList : List<Continent>
    {
        private MapData _map;
        private short[,] _continentIds;
        public const short NoContinent = 255;
        private const short NotAnalizedContinent = 254;
        private static readonly Random Rnd = new Random();

        public static ContinentList Create(MapData map)
        {
            ContinentList cl = new ContinentList();

            cl._map = map;

            cl.CreateList();
            cl.CountContinentSizes();
            cl.ResetContinentNames();

            cl.FindGoodTiles();

            cl.SetContinents();

            return cl;
        }

        private void SetContinents()
        {
            for (int nx = 0; nx < _map.Width; nx++)
            {
                for (int ny = 0; ny < _map.Height; ny++)
                {
                    MapCell tile = _map[nx, ny];
                    tile.Continent = this[_continentIds[nx,ny]];
                    tile.Continent.Add(tile.Point);
                }
            }
        }

        public Continent this[int x, int y]
        {
            get
            {
                short id = _continentIds[x, y];

                if (Valid(id))
                    return this[id];

                return null;
            }
        }

        public Continent this[HexPoint pt]
        {
            get
            {
                return this[pt.X, pt.Y];
            }
        }

        public short GetId(int x, int y)
        {
            return _continentIds[x, y];
        }

        private void Fill(int size)
        {
            for (int i = 0; i < size; ++i)
                Add(new Continent(_map, string.Format("Continent{0}", i), 0));
        }

        private static bool Valid(short value)
        {
            return value != NoContinent && value != NotAnalizedContinent;
        }

        private void CreateList()
        {
            // fill list
            Fill(256);

            // init array
            _continentIds = new short[_map.Width, _map.Height];

            for (int i = 0; i < _map.Width; i++)
                for (int j = 0; j < _map.Height; j++)
                    _continentIds[i, j] = NotAnalizedContinent;

            for (int nx = 0; nx < _map.Width; nx++)
            {
                for (int ny = 0; ny < _map.Height; ny++)
                {
                    MapCell tile = _map[nx, ny];
                    HexPoint pt = tile.Point;

                    if (tile.IsLand)
                    {
                        HexPoint p0 = pt.Neighbor(HexDirection.NorthWest);
                        HexPoint p4 = pt.Neighbor(HexDirection.West);
                        HexPoint p1 = pt.Neighbor(HexDirection.NorthEast);
                        HexPoint p5 = pt.Neighbor(HexDirection.SouthWest);

                        short c0 = (p0.Check(_map.Width, _map.Height)) ? _continentIds[p0.X, p0.Y] : NotAnalizedContinent;
                        short c1 = (p1.Check(_map.Width, _map.Height)) ? _continentIds[p1.X, p1.Y] : NotAnalizedContinent;
                        short c4 = (p4.Check(_map.Width, _map.Height)) ? _continentIds[p4.X, p4.Y] : NotAnalizedContinent;
                        short c5 = (p5.Check(_map.Width, _map.Height)) ? _continentIds[p5.X, p5.Y] : NotAnalizedContinent;

                        if (Valid(c0))
                            _continentIds[pt.X, pt.Y] = c0;
                        else if (Valid(c1))
                            _continentIds[pt.X, pt.Y] = c1;
                        else if (Valid(c5))
                            _continentIds[pt.X, pt.Y] = c5;
                        else if (Valid(c4))
                            _continentIds[pt.X, pt.Y] = c4;
                        else
                            _continentIds[pt.X, pt.Y] = GetFreeContinent();

                        //Error
                        if (Valid(c1) && Valid(c5) && (c1 != c5))
                            ReplaceContinent(c5, c1);
                        if (Valid(c4) && Valid(c0) && (c4 != c0))
                            ReplaceContinent(c4, c0);
                        if (Valid(c4) && Valid(c1) && (c4 != c1))
                            ReplaceContinent(c4, c1);
                        if (Valid(c0) && Valid(c5) && (c0 != c5))
                            ReplaceContinent(c5, c0);
                        if (Valid(c0) && Valid(c1) && (c0 != c1))
                            ReplaceContinent(c0, c1);
                        if (Valid(c4) && Valid(c5) && (c4 != c5))
                            ReplaceContinent(c4, c5);
                    }
                    else
                        _continentIds[pt.X, pt.Y] = NoContinent;
                }
            }

            for (int nx = 0; nx < _map.Width; nx++)
            {
                for (int ny = 0; ny < _map.Height; ny++)
                {
                    MapCell tile = _map[nx, ny];
                    HexPoint pt = tile.Point;

                    if (tile.IsCoast)
                        _continentIds[pt.X, pt.Y] = GetContinentIdNearby(pt);
                }
            }
        }

        private short GetContinentIdNearby(HexPoint shore)
        {
            foreach (HexPoint pt in shore.Neighbors)
            {
                if (_map.IsValid(pt))
                {
                    if (_map[pt].IsLand)
                        return _continentIds[pt.X, pt.Y];
                }
            }

            return NoContinent;
        }

        /// <summary>
        /// replace <para>a</para> with <para>b</para> in <para>_continentIds</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void ReplaceContinent(short a, short b)
        {
            for (int nx = 0; nx < _map.Width; nx++)
                for (int ny = 0; ny < _map.Height; ny++)
                    if (_continentIds[nx, ny] == a)
                        _continentIds[nx, ny] = b;

            return;
        }

        /// <summary>
        /// get first free continent id
        /// </summary>
        /// <returns></returns>
        private short GetFreeContinent()
        {
            bool[] free = new bool[Count];

            // initialisieren
            for (int i = 0; i < Count; i++)
                free[i] = true;

            // Kontinent suchen
            for (int nx = 0; nx < _map.Width; nx++)
            {
                for (int ny = 0; ny < _map.Height; ny++)
                {
                    short c = _continentIds[nx, ny];
                    if (c >= 0 && c < Count)
                        free[c] = false;
                }
            }

            // ergebnis setzen
            for (short u = 0; u < Count; u++)
                if (free[u])
                    return u;

            return NoContinent;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CountContinentSizes()
        {
            ////for (int u = 0; u < Count; u++)
            ////    this[u].Size = 0;

            //for (int nx = 0; nx < _map.Width; nx++)
            //{
            //    for (int ny = 0; ny < _map.Height; ny++)
            //    {
            //        int c = _continentIds[nx, ny];

            //        if (c < Count && c >= 0)
            //            this[c].Size++;
            //    }
            //}

            //Sort();

            return;
        }

        private void ResetContinentNames()
        {
            for (int u = 0; u < Count; u++)
                this[u].Name = String.Format("Continent{0}", u);

            return;
        }

        public Continent GetRandomBySize(int size)
        {
            int borderIndex = 0;

            for (int i = 0; i < Count; ++i)
            {
                if (this[i].Size < size)
                    break;
                borderIndex = i;
            }

            return this[Rnd.Next(borderIndex)];
        }

        public List<Continent> BySize(int size)
        {
            List<Continent> list = new List<Continent>();

            for (int i = 0; i < Count; ++i)
            {
                if (this[i].Size > size)
                    list.Add(this[i]);
            }

            return list;
        }

        private void FindGoodTiles()
        {
            for (int nx = 0; nx < _map.Width; nx++)
            {
                for (int ny = 0; ny < _map.Height; ny++)
                {
                    Continent c = this[nx, ny];

                    if (c != null)
                    {
                        MapCell tile = _map[nx, ny];

                        int food = tile.Food;

                        tile.Point.Neighbors.ForEach(n =>
                                                         {
                                                             if (_map.IsValid(n))
                                                                 food += _map[n].Food;
                                                         });

                        c.AddGoodTileLocation(food, tile.Point);
                    }
                }
            }
        }
    }
}