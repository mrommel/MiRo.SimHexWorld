using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using NUnit.Framework;
using MiRo.SimHexWorld.World.Maps;
using MiRo.SimHexWorld.Engine.Instance.AI;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Instance
{
    [TestFixture]
    public class GameData : IDisposable
    {
        MapData _map;
        MapPace _pace;

        int _currentTurn = 0;
        int _currentPlayer = 0;

        List<AbstractPlayerData> _players = new List<AbstractPlayerData>();

        Globals _globals;

        Difficulty _handicap;

        // TODO: Load from XML (standard)
        private int firstYear = -4000;
        private int[] standardTurns = new int[] { 75, 60, 25, 50, 60, 50, 120, 60 }; // Summe: 500
        private float[] standardYears = new float[] { 40, 25, 20, 10, 5, 2, 1, 0.5f };

        public event TurnHandler Turned;
        public event PlayerChangedHandler PlayerChanged;
        public event TurnHandler LastTurn;
        public event MapUpdateHandler MapUpdated;

        public GameData()
        {
            _currentTurn = 0;
            _pace = MapPace.Standard;

            _players.Add(new HumanPlayerData(0, Provider.GetCivilization("English")));
            _players.Add(new AIPlayerData(1, Provider.GetCivilization("German")));

            _handicap = Provider.GetHandicap("Settler");
        }

        public void Initialize()
        {
            if (Map.Extension.StartLocations.Count > 0)
            {
                foreach (AbstractPlayerData pl in _players)
                {
                    HexPoint loc = pl.StartLocation;

                    pl.AddUnit("Settler", loc.X, loc.Y);
                    pl.AddUnit("Worker", loc.X, loc.Y);
                }
            }
        }

        public MapData Map
        {
            get
            {
                return _map;
            }
            set
            {
                _map = value;

                if (MapUpdated != null)
                    MapUpdated(new MapChangeArgs(_map));
            }
        }

        public Globals Globals
        {
            get
            { return _globals; }
        }

        public Difficulty Handicap
        {
            get
            { return _handicap; }
        }

        public List<Unit> GetUnitsAt(HexPoint pt)
        {
            List<Unit> units = new List<Unit>();

            foreach (AbstractPlayerData pl in _players)
                units.AddRange(pl.GetUnitsAt(pt));

            return units;
        }

        public City GetCityAt(HexPoint pt)
        {
            foreach (AbstractPlayerData pl in _players)
            {
                City c = pl.GetCityAt(pt);

                if (c != null)
                    return c;
            }

            return null;
        }

        public void Update(GameTime time)
        {
            Turn();

            foreach (AbstractPlayerData pl in _players)
                pl.Update(time);
        }

        public List<AbstractPlayerData> Players
        {
            get
            {
                return _players;
            }
        }

        public void Turn()
        {
            _currentTurn++;

            //if (Turned != null)
            //    Turned(this, new TurnEventArgs(oldYear, Year));

            //AbstractPlayerData oldPlayer = Player;
            //GameYear oldYear = Year;

            //oldPlayer.OnTurnEnd();

            //// turn?
            //if (_currentPlayer + 1 >= players.Count)
            //{
            //    if (Year == new GameYear(2050) && LastTurn != null)
            //        LastTurn(this, new TurnEventArgs(oldYear, Year));

            //    _currentPlayer = 0;
            //    _currentTurn++;

            //    if (Turned != null)
            //        Turned(this, new TurnEventArgs(oldYear, Year));
            //}
            //else
            //    _currentPlayer++;

            //if (PlayerChanged != null)
            //    PlayerChanged(this, new PlayerChangedEventArgs(oldPlayer, Player));

            //Player.OnTurnStart();
        }

        public AbstractPlayerData Human
        {
            get
            {
                Assert.True(_players.Count > 0);
                //Assert.True(0 <= _currentPlayer);
                //Assert.True(_currentPlayer < players.Count);

                return _players.FirstOrDefault(a => a.IsHuman);
            }
        }

        public float TotalCulture
        {
            get
            {
                float culture = 0L;

                foreach (AbstractPlayerData pl in _players)
                    culture += pl.Culture;

                return culture;
            }
        }

        //EraTypes getCurrentEra();

        //int countReligionLevels(ReligionTypes eReligion);
        //int calculateReligionPercent(ReligionTypes eReligion);
        //City getHolyCity(ReligionTypes eIndex);
        //void setHolyCity(ReligionTypes eIndex, City pNewValue, bool bAnnounce);

        //int getAdjustedPopulationPercent(VictoryTypes eVictory); 
        //int getAdjustedLandPercent(VictoryTypes eVictory);

        //public void UpdateCitySight(bool bIncrement)
        //{
        //    foreach (AbstractPlayerData pl in players)
        //        pl.UpdateCitySight(bIncrement);
        //}

        //public void UpdateTradeRoutes()
        //{
        //    foreach (AbstractPlayerData pl in players)
        //        pl.UpdateTradeRoutes();
        //}

        //void updateScore(bool bForce = false);

        //void assignStartingPlots();
        //void normalizeStartingPlots();

        public int CurrentTurn
        {
            get
            {
                return _currentTurn;
            }
        }

        public GameYear Year
        {
            get
            {
                Assert.GreaterOrEqual(_currentTurn, 0, "Current Turn must be possitive");

                switch (_pace)
                {
                    case MapPace.Quick:
                        return new GameYear(_currentTurn);
                    case MapPace.Standard:
                        // -4000 - -1000 | 3000 = 480 * 75 / 12
                        // -1000 - 500 | 1500 = 300 * 60 / 12
                        // 500 - 1000 | 500 = 240 * 25 / 12
                        // 1000 - 1500 | 500 = 120 * 50 / 12
                        // 1500 - 1800 | 300 = 60 * 60 / 12
                        // 1800 - 1900 | 100 = 24 * 50 / 12
                        // 1900 - 2020 | 120 = 12 * 120 / 12
                        // 2020 - 2050 | 30 = 6 * 60 / 12

                        int currentYear = firstYear;
                        int turnsLeft = _currentTurn;

                        for (int i = 0; i < standardTurns.Length; ++i)
                        {
                            if ((turnsLeft - standardTurns[i]) < 0)
                            {
                                return new GameYear(currentYear + (int)(turnsLeft * standardYears[i]));
                            }
                            else
                            {
                                turnsLeft -= standardTurns[i];
                                currentYear += (int)(standardTurns[i] * standardYears[i]);
                            }
                        }

                        return new GameYear(2050);
                    default:
                        return new GameYear(-4000);
                }
            }
        }

        #region tests

        [Test]
        public static void TestStandardPaceFirst()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 0;
            Assert.AreEqual(new GameYear(-4000).ToString(), d1.Year.ToString(), "Startyear must be 4000 BC");
        }

        [Test]
        public static void TestStandardPaceSecond()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 2;
            Assert.AreEqual(new GameYear(-3920).ToString(), d1.Year.ToString(), "3rd Year must be 3920 BC");
        }

        [Test]
        public static void TestStandardPaceThird()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 2;
            Assert.AreEqual(new GameYear(-3920).ToString(), d1.Year.ToString(), "3rd Year must be 3920 BC");
        }

        [Test]
        public static void TestStandardPace100()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 100;
            Assert.AreEqual(new GameYear(-375).ToString(), d1.Year.ToString(), "100 turn must be 375 AD");
        }

        [Test]
        public static void TestStandardPace200()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 200;
            Assert.AreEqual(new GameYear(1400).ToString(), d1.Year.ToString(), "200 turn must be 1400 AD");
        }

        [Test]
        public static void TestStandardPace300()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 300;
            Assert.AreEqual(new GameYear(1860).ToString(), d1.Year.ToString(), "300 turn must be 1860 AD");
        }

        [Test]
        public static void TestStandardPace400()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 400;
            Assert.AreEqual(new GameYear(1980).ToString(), d1.Year.ToString(), "400 turn must be 1980 AD");
        }

        [Test]
        public static void TestStandardPace499()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 499;
            Assert.AreEqual(new GameYear(2049).ToString(), d1.Year.ToString(), "Pre Endyear must be 2049 AD");
        }

        [Test]
        public static void TestStandardPaceLast()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 500;
            Assert.AreEqual(new GameYear(2050).ToString(), d1.Year.ToString(), "Endyear must be 2050 AD");
        }

        [Test]
        public static void TestTurnedHandling()
        {
            GameData d1 = new GameData();
            d1._pace = MapPace.Standard;

            d1._currentTurn = 0;
            d1._players.Add(new HumanPlayerData(0, Provider.GetCivilization("German")));

            bool fired = false;
            GameYear oldYear = new GameYear();
            GameYear newYear = new GameYear();

            d1.Turned += new TurnHandler(
                delegate(GameData game, TurnEventArgs args)
                {
                    oldYear = args.OldYear;
                    newYear = args.NewYear;
                    fired = true;
                });

            d1.Turn();

            Assert.True(fired, "Event Turned not fired");
            Assert.AreEqual(new GameYear(-4000), oldYear, "Old year must be 4000 BC");
            Assert.AreEqual(new GameYear(-3960), newYear, "New year must be 3960 BC");
        }

        #endregion tests

        public void Dispose()
        {
            foreach (AbstractPlayerData pl in _players)
                pl.Dispose();
        }
    }
}
