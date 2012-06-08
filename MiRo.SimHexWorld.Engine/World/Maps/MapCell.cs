using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Collections;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Instance;
using NUnit.Framework;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World.Entities;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    //public enum Climate { None, A, B, C, D, E };
    //public enum ClimateModifier { humid, semihumig, semiarid, arid };
    //public enum HeightLevel { Peak, High, Medium, Flat, Shore, Ocean };

    /// <summary>
    /// A grid cell of a 2D grid map.
    /// </summary>
    public interface IGridCell
    {
        /// <summary>
        /// X coordinate in 2d grid space (cell index)
        /// </summary>
        int X { get; }

        /// <summary>
        /// Y coordinate in 2d grid space (cell index)
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Weight of this cell. Standard value should be 0.
        /// Can be higher to account for difficult terrain or
        /// occupied cells. To make sure the cell won't be 
        /// part of a path the weight should be very high
        /// compared to the standard or usual weights used.
        /// </summary>
        int Weight(Unit unit);

        /// <summary>
        /// Determines whether this cell can be walked and therefore
        /// should be considered for path calculations.
        /// </summary>
        bool IsWalkable(Unit unit);

        /// <summary>
        /// Determines whether the point p corresponds to
        /// this cell, that is its X and Y coordinates are equal
        /// to the cell coordinates X and Y.
        /// </summary>
        /// <param name="p">A point of grid coordinates</param>
        /// <returns>True if the cell matches the given coordinates</returns>
        bool Matches(HexPoint p);

        /// <summary>
        /// The 3D position of this grid cell. This should be the point
        /// right in the middle of the cell at the height of the grid.
        /// </summary>
        /// <remarks>Is not used by Pathfinder itself. This is just a convenience
        /// property for other users of the interface. If you don't need the
        /// 3D position of the cell (because your game is 2D only), you don't
        /// have to return a defined value. Just return Vector3.Zero or anything 
        /// else, it doesn't matter.</remarks>
        Vector3 Position3D { get; }
    }

    /// <summary>
    /// map cell
    /// </summary>
    public class MapCell : IGridCell
    {
        HexPoint _pt = new HexPoint();
        Terrain _terrain;
        readonly List<Feature> _features = new List<Feature>();
        Ressource _ressource;
        byte _river;

        readonly BitArray _spotted; // set of Player IDs
        readonly BitArray _discovered;
        private int _controlledBy = -1, _exploitedBy = -1;
        private const int MaxPlayerId = 8;
        List<Improvement> _improvements;
        private bool _ressourceRevealed;

        //HeightLevel _hLevel;
        //Climate _climate = Climate.None;

        public string Text { get; set; }

        public string Info { get; set; }
        public MapCell()
        {
        }

        public MapCell(Terrain terrain)
        {
            Continent = null;
            _pt.X = 0;
            _pt.Y = 0;
            _terrain = terrain;
            _improvements = new List<Improvement>();

            _spotted = new BitArray(MaxPlayerId, false);
            _discovered = new BitArray(MaxPlayerId, false);
        }

        public MapCell(int x, int y, Terrain terrain)
        {
            Continent = null;
            _pt.X = x;
            _pt.Y = y;
            _terrain = terrain;
            _improvements = new List<Improvement>();

            _spotted = new BitArray(MaxPlayerId, false);
            _discovered = new BitArray(MaxPlayerId, false);
        }

        #region terrain

        public Terrain Terrain
        {
            get { return _terrain; }
            set { _terrain = value; }
        }

        public bool IsCoast
        {
            get
            {
                return _terrain.Name == "Coast";
            }
        }

        public bool IsOcean
        {
            get
            {
                if (_terrain == null)
                    return true;

                switch (_terrain.Name)
                {
                    case "Ocean":
                    case "Shore":
                    case "Coast":
                        return true;
                }

                return false;
            }
        }

        public bool IsLand
        {
            get
            {
                return !IsOcean;
            }
        }

        #endregion terrain

        #region features

        public List<Feature> Features
        {
            get
            {
                return _features;
            }
        }

        public string FeatureStr
        {
            get
            {
                return _features.Count.ToString(CultureInfo.InvariantCulture) + " " + string.Join(", ", _features.Select(a => a.Name)).Trim().Trim(new char[]{','});
            }
        }

        public bool IsForest
        {
            get 
            {
                return _features.Exists( a => a.IsForest);
            }
			set
            {
                Feature forestFeature = Provider.Instance.Features.FirstOrDefault(a => a.Value.IsForest && a.Value.ImprovesTerrains.Exists( b => b.Terrain == Terrain.Name )).Value;

				if (forestFeature == null)
                    throw new Exception("Forest not found for '" + Terrain + "'!");

                if( !IsForest )
                    _features.Add(forestFeature);
            }
        }

        public bool IsSpring
        {
            get { return _features.Exists(a => a.IsSpring); }
        }

        #endregion

        #region river

        public byte River
        {
            get { return _river; }
            set { _river = value; }
        }

        public bool IsRiver
        {
            get
            {
                return _river > 0;
            }
        }

		public enum FlowDirectionType : int 
        {
			NoFlowdirection = 0,

            North = 1,
            South = 2,     
            SouthEast = 4,
            NorthWest = 8,
            SouthWest = 16,
            NorthEast = 32,
           
            //NORTH_MASK,
            //SOUTH_MASK,
            //SOUTHEAST_MASK,
            //NORTHWEST_MASK,
            //SOUTHWEST_MASK,
            //NORTHEAST_MASK,
		}

        public static string BinaryString(byte val)
        {
            string str = "";

            for (int i = 0; i < 6; i++)
            {
                if (val % 2 == 1)
                    str = "1" + str;
                else
                    str = "0" + str;

                val = (byte)(val / 2);
            }

            return str;
        }

        public string FlowString
        {
            get { return BinaryString(River); }
        }

        public void SetRiver(FlowDirectionType flowDir)
        {
            switch(flowDir)
            {
                case FlowDirectionType.South:
                case FlowDirectionType.North:
                    SetWOfRiver(true,flowDir);
                    break;
                case FlowDirectionType.SouthEast:
                case FlowDirectionType.NorthWest:
                    SetNEOfRiver(true, flowDir);
                    break;
                case FlowDirectionType.SouthWest:
                case FlowDirectionType.NorthEast:
                    SetNWOfRiver(true, flowDir);
                    break;
            }
        }

        /// http://wiki.2kgames.com/civ5/index.php/River_system_overview
        public void SetWOfRiver(bool river, FlowDirectionType flowDir)
        {
            if (flowDir != FlowDirectionType.North && flowDir != FlowDirectionType.South)
                throw new Exception("West of the plot can only flow the river from north to south and vice versa, not " + flowDir);

            int riverMask = (63 - (int) flowDir);
            River = (byte) (River & riverMask);
            River += river ? (byte)flowDir : (byte)0;
        }

        public bool IsWOfRiver
        {
            get
            {
                if ((River & (int)FlowDirectionType.North) > 0)
                    return true;

                if ((River & (int)FlowDirectionType.South) > 0)
                    return true;

                return false;
            }
        }

        public void SetNWOfRiver(bool river, FlowDirectionType flowDir)
        {
            if (flowDir != FlowDirectionType.NorthEast && flowDir != FlowDirectionType.SouthWest)
                throw new Exception("West of the plot can only flow the river from northeast to southwest and vice versa, not " + flowDir);

            int riverMask = (63 - (int)flowDir);
            River = (byte)(River & riverMask);
            River += river ? (byte)flowDir : (byte)0;
        }

        public bool IsNWOfRiver
        {
            get
            {
                if ((River & (int)FlowDirectionType.NorthEast) > 0)
                    return true;

                if ((River & (int)FlowDirectionType.SouthWest) > 0)
                    return true;

                return false;
            }
        }

        public void SetNEOfRiver(bool river, FlowDirectionType flowDir)
        {
            if (flowDir != FlowDirectionType.NorthWest && flowDir != FlowDirectionType.SouthEast)
                throw new Exception("West of the plot can only flow the river from northwest to southeast and vice versa, not " + flowDir);

            int riverMask = (63 - (int)flowDir);
            River = (byte)(River & riverMask);
            River += river ? (byte)flowDir : (byte)0;
        }

        public bool IsNEOfRiver
        {
            get
            {
                if ((River & (int)FlowDirectionType.NorthWest) > 0)
                    return true;

                if ((River & (int)FlowDirectionType.SouthEast) > 0)
                    return true;

                return false;
            }
        }

        public byte RiverTileValue
        {
            get
            {
                byte riverFlow = 0;
              
                riverFlow += (byte)(IsWOfRiver ? 2 : 0);
                riverFlow += (byte)(IsNWOfRiver ? 4 : 0);
                riverFlow += (byte)(IsNEOfRiver ? 8 : 0); 

                return riverFlow;
            }
        }

        public string RiverFlowString { 
            get
            {
                string str = "";
                if (IsWOfRiver)
                    str += "S/N,";

                if (IsNEOfRiver)
                   str += "NW/SE,";

                if (IsNWOfRiver)
                    str += "NE/SW,";

                return str.TrimEnd(',');
            }
        }

        #endregion features

        #region resources

        public Ressource Ressource
        {
            get { return _ressource; }
        }

        public string RessourceStr 
        {
            get
            {
                if (_ressource == null)
                    return "";

                return _ressource.Name;
            }
        }

        #endregion resources

        #region spotting

        public bool IsSpotted(AbstractPlayerData player)
        {
            return IsSpotted(player.Id);
        }

        public bool IsSpotted(int playerId)
        {
            if (playerId < 0 || playerId >= MaxPlayerId)
                return false;

            return _spotted[playerId];
        }

        public void SetSpotted(AbstractPlayerData player, bool spotted = true)
        {
            SetSpotted(player.Id, spotted);
        }

        public void SetSpotted(int playerId, bool spotted = true)
        {
            if (IsSpotted(playerId) != spotted)
            {
                MainWindow.Game.Map.OnMapSpotting(new MapSpottingArgs(MainWindow.Game.Map, _pt, spotted));
            }

            _spotted.Set(playerId, spotted);
        }

        #endregion spotting

        #region position

        public HexPoint Point
        {
            get { return _pt; }
            set { _pt = value; }
        }

        public int X
        {
            get { return _pt.X; }
            set { _pt.X = value; }
        }

        public int Y
        {
            get { return _pt.Y; }
            set { _pt.Y = value; }
        }

        #endregion position

        #region goods

        public int Commerce
        {
            get
            {
                return Terrain.Bonus.Commercial + 
                    _features.Sum(feature => feature.Bonus.Commercial);
            }
        }

        public int Food
        {
            get
            {
                int food = Terrain.Bonus.Food;

                food += _features.Sum(feature => feature.Bonus.Food);

                foreach (Improvement imp in _improvements)
                {
                    food += imp.Bonus.Food;

                    if (imp.ImprovesResources != null)
                    {
                        foreach (ResourceBonus rb in imp.ImprovesResources)
                        {
                            if (_ressource != null && _ressource.Name == rb.Ressource)
                                food += rb.Bonus.Food;
                        }
                    }

                    if (ControlledBy != -1)
                    {
                        AbstractPlayerData controller = MainWindow.Game.Players[ControlledBy];

                        if (imp.TechnologyBonuses != null)
                        {
                            foreach (TechnologyBonus tb in imp.TechnologyBonuses)
                            {
                                if (tb.IsValid(controller.Technologies))
                                    if (tb.Yield.Type == Types.AI.YieldType.Food)
                                        food += tb.Yield.Amount;
                            }
                        }
                    }
                }

                return food;
            }
        }

        public int Production
        {
            get
            {
                return Terrain.Bonus.Production + 
                    _features.Sum(feature => feature.Bonus.Production);
            }
        }

        #endregion goods

        #region continent

        public Continent Continent { get; set; }

        #endregion continent

        #region height

        public float Height { get; set; }
        public TileHeight TileHeight
        {
            get
            {
                if (Features.FirstOrDefault(a => a.Name == "Hills") != null)
                    return World.TileHeight.Hill;

                if (Features.FirstOrDefault(a => a.Name == "Mountains") != null)
                    return World.TileHeight.Peak;

                return _terrain.TileHeight;
            }
        }

        #endregion height

        public TileType TileType
        {
            get
            {
                if (Features.FirstOrDefault(a => a.Name == "Hills") != null)
                    return TileType.Rock;

                if (Features.FirstOrDefault(a => a.Name == "Mountains") != null)
                    return TileType.Snow;

                return _terrain.TileType;
            }
        }

        #region tests

        [Test]
        public static void TestNoRiver()
        {
            Terrain terrain = new Terrain();
            MapCell d1 = new MapCell(terrain);
            d1.River = 0;

            Assert.IsFalse(d1.IsNEOfRiver, "Should be no river NE");
            Assert.IsFalse(d1.IsNWOfRiver, "Should be no river NW");
            Assert.IsFalse(d1.IsWOfRiver, "Should be no river W");
            Assert.AreEqual("000000", d1.FlowString, "FlowString should be 000000");
        }

        [Test]
        public static void TestRiverFlowsNorth()
        {
            Terrain terrain = new Terrain();
            MapCell d1 = new MapCell(terrain);

            d1.SetWOfRiver(true,FlowDirectionType.North);

            Assert.IsFalse(d1.IsNEOfRiver, "Should be no river NE");
            Assert.IsFalse(d1.IsNWOfRiver, "Should be no river NW");
            Assert.IsTrue(d1.IsWOfRiver, "Should be river W");
            Assert.AreEqual("000001", d1.FlowString, "FlowString should be 000001");
        }

        [Test]
        public static void TestRiverFlowsSouth()
        {
            Terrain terrain = new Terrain();
            MapCell d1 = new MapCell(terrain);

            d1.SetWOfRiver(true, FlowDirectionType.South);

            Assert.IsFalse(d1.IsNEOfRiver, "Should be no river NE");
            Assert.IsFalse(d1.IsNWOfRiver, "Should be no river NW");
            Assert.IsTrue(d1.IsWOfRiver, "Should be river W");
            Assert.AreEqual("000010", d1.FlowString, "FlowString should be 000010");
        }

        [Test]
        public static void TestRiverFlowsNorthAndSouth()
        {
            Terrain terrain = new Terrain();
            MapCell d1 = new MapCell(terrain);

            d1.SetWOfRiver(true, FlowDirectionType.North);
            d1.SetWOfRiver(true, FlowDirectionType.South);

            Assert.IsFalse(d1.IsNEOfRiver, "Should be no river NE");
            Assert.IsFalse(d1.IsNWOfRiver, "Should be no river NW");
            Assert.IsTrue(d1.IsWOfRiver, "Should be river W");
            Assert.AreEqual("000011", d1.FlowString, "FlowString should be 000011");
        }

        [Test]
        public static void TestRiverFlowsNorthWest()
        {
            Terrain terrain = new Terrain();
            MapCell d1 = new MapCell(terrain);

            d1.SetNEOfRiver(true, FlowDirectionType.NorthWest);

            Assert.IsTrue(d1.IsNEOfRiver, "Should be river NE");
            Assert.IsFalse(d1.IsNWOfRiver, "Should be no river NW");
            Assert.IsFalse(d1.IsWOfRiver, "Should be no river W");
        }

        [Test]
        public static void TestRiverFlowSouthEast()
        {
            Terrain terrain = new Terrain();
            MapCell d1 = new MapCell(terrain);

            d1.SetNEOfRiver(true, FlowDirectionType.SouthEast);

            Assert.IsTrue(d1.IsNEOfRiver, "Should be river NE");
            Assert.IsFalse(d1.IsNWOfRiver, "Should be no river NW");
            Assert.IsFalse(d1.IsWOfRiver, "Should be no river W");
        }

        #endregion tests

        public bool CanEnter(Entities.Unit unit)
        {
            return Terrain.MoveRateTypes.Contains(unit.Data.MoveRateType);
        }

        public int ControlledBy
        {
            get
            {
                return _controlledBy;
            }
            set
            {
                _controlledBy = value;

                MainWindow.Game.Map.OnMapControlling(new MapControllingArgs(MainWindow.Game.Map, _pt, _controlledBy));
            }
        }

        public int ExploitedBy
        {
            get
            {
                return _exploitedBy;
            }
            set
            {
                _exploitedBy = value;

                MainWindow.Game.Map.OnMapExploiting(new MapControllingArgs(MainWindow.Game.Map, _pt, _exploitedBy));
            }
        }

        public bool Unexploited
        {
            get { return _exploitedBy == -1; }
            set 
            {
                if (value == false)
                    throw new Exception("You need to call ExploitedBy with the new exploiter");

                _exploitedBy = -1; 
            }
        }

        public IList<Improvement> Improvements
        {
            get { return _improvements; }
        }

        public int Weight(Unit unit)
        {
            return 1;
        }

        public bool IsWalkable(Unit unit)
        {
            return CanEnter(unit) && IsSpotted(unit.Player);
        }

        public bool Matches(HexPoint p)
        {
            return p == Point;
        }

        public Vector3 Position3D
        {
            get
            {
                return MapData.GetWorldPosition(Point);
            }
        }

        internal void SetRessources(Ressource ressource)
        {
            _ressource = ressource;
        }

        public bool RessourceRevealed
        {
            get
            {
                return _ressourceRevealed;
            }
            set
            {
                _ressourceRevealed = value;
            }
        }
    }
}
