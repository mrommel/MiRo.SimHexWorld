using System;
using System.Linq;
using log4net;
using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World.Entities;
using NUnit.Framework;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.UI;
using MiRo.SimHexWorld.Engine.Instance.AI;
using MiRo.SimHexWorld.Engine.Locales;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public delegate void CivilizationHandler(Civilization civ);

    /// player ai and human
    public abstract class AbstractPlayerData : IDisposable
    {
        ILog _log = LogManager.GetLogger(typeof(AbstractPlayerData));

        #region Fields & proterties
        private bool _isHuman;
		
        public int Id { get; set; }
        public string Name { get; set; }

        protected Civilization _civilization;

        public List<Tech> Technologies = new List<Tech>();
        public List<Ressource> Ressources = new List<Ressource>();
        public Tech CurrentResearch = null;
        public float _science = 0;

        protected List<Unit> _units = new List<Unit>();
        protected List<City> _cities = new List<City>();

        protected List<DiplomaticStatus> _diplomaticStatuses = new List<DiplomaticStatus>();

        protected Era _era;

        protected List<Policy> _policies = new List<Policy>();
        protected List<PolicyType> _policyTypes = new List<PolicyType>();

        protected float _culture = 0L;
        private int adoptedPolicies = 0;

        // AI
        public InfluenceMap CityLocationMap;
        public InfluenceMap ImprovementLocationMap;

        // turn
        double _secondsToNextUpdate = 0f;
        double _secondsPerTurn = 2f;
        bool _isFirstRun = true;

        bool _needToUpdateInfluenceMaps = true;

        LeaderData _leader;

        public event CivilizationHandler FirstContact;

        public List<float> Scores = new List<float>();

        protected static Random rand = new Random();

        #endregion Fields & proterties      
		
        protected AbstractPlayerData( int id, Civilization tribe, bool isHuman )
        {
            Id = id;
            _isHuman = isHuman;
            _civilization = tribe;

            // set leader from tribe
            _leader = Provider.Instance.Leaders.FirstOrDefault(a => a.Value.CivilizationName == _civilization.Name).Value;

            Init();
			
            if( !_isHuman)
                StartAiThreads();
        }

        public virtual void Init() 
        {
            _era = Provider.GetEra("Ancient");
            Assert.NotNull(_era, "There must be at least the 'Ancient' era present");

            foreach (Tech tech in _civilization.StartingTechs)
                DiscoverTechnology(tech);
        }

        public LeaderData Leader
        {
            get
            {
                return _leader;
            }
        }

        public Civilization Civilization
        {
            get
            {
                return _civilization;
            }
        }

        public List<WonderData> Wonders
        {
            get
            {
                List<WonderData> wonders = new List<WonderData>();

                foreach (City city in _cities)
                    wonders.AddRange(city.Wonders);

                return wonders;
            }
        }
        
        public bool IsEarly
        {
            get
            {
                Assert.NotNull(_era, "Era cannot be null");
                return _era == Provider.GetEra("Ancient");
            }
        }

        public List<UnitData> AvailableUnits
        {
            get
            {
                List<UnitData> units = new List<UnitData>();

                foreach (UnitData unit in Provider.Instance.Units.Values)
                {
                    bool requiredRessourceAvailable = true;

                    foreach( string resStr in unit.RequiredRessourceNames )
                        if( !Ressources.Exists( a => a.Name == resStr ) )
                            requiredRessourceAvailable = false;

                    bool requiredTechAvailable = Technologies.Exists(a => a.Name == unit.RequiredTechName);

                    if (string.IsNullOrEmpty(unit.RequiredTechName))
                        requiredTechAvailable = true;

                    if ( requiredTechAvailable && requiredRessourceAvailable)
                        units.Add(unit);
                }

                return units;
            }
        }

        public DiplomaticStatus DiplomaticStatusTo(Civilization civ)
        {
            foreach (DiplomaticStatus ds in _diplomaticStatuses)
                if (ds.CivilizationName == civ.Name)
                    return ds;

            return null;
        }

        public List<Tech> PossibleTechnologies
        {
            get
            {
                List<Tech> techs = new List<Tech>();

                foreach (Tech tech in Provider.Instance.Techs.Values)
                {
                    bool requiredAnd = true;
                    foreach (Tech techAnd in tech.RequiredTech)
                        requiredAnd &= Technologies.Contains(techAnd);

                    if (requiredAnd && !Technologies.Contains(tech))
                    {
                        if (tech.Flavours.Count == 0)
                            throw new Exception("Tech without Flavours: " + tech.Name);

                        techs.Add(tech);
                    }
                }

                return techs;
            }
        }
		
        public bool IsHuman
        {
            get
            {
                return _isHuman;
            }
        }
		
        public bool IsArtificial
        {
            get
            {
                return !_isHuman;
            }
        }

        public List<City> Cities
        {
            get
            { return _cities; }
        }

        public List<Policy> Policies
        {
            get { return _policies; }
        }

        public List<Unit> Units
        {
            get
            {
                return _units;
            }
        }

        public List<Unit> GetUnitsAt(HexPoint pt)
        {
            List<Unit> units = new List<Unit>();

            foreach (Unit u in _units)
                if (u.Point == pt)
                    units.Add(u);

            return units;
        }

        public City GetCityAt(HexPoint pt)
        {
            foreach (City c in _cities)
                if (c.Point == pt)
                    return c;

            return null;
        }
		
        public abstract void StartAiThreads();

        public abstract void Dispose();

        public MapData Map
        {
            get
            {
                if (MainWindow.Game == null)
                    return null;

                return MainWindow.Game.Map;
            }
        }

        public City Capital
        {
            get
            {
                foreach (City c in _cities)
                    if (c.IsCapital)
                        return c;

                return null;
            }
        }

        public Era Era
        {
            get
            {
                return _era;
            }
        }

        public void DiscoverTechnology(Tech tech)
        {
            if (tech.Era > _era)
            {
                _era = tech.Era;

                GameFacade.getInstance().SendNotification( GameNotification.StartEra, this, _era);

                bool needToUpdateResources = false;
                // reveal resources for all players
                foreach (MapCell cell in Map.Tiles)
                {
                    if (!cell.RessourceRevealed && cell.Ressource != null && cell.Ressource.RequiredTechName == tech.Name)
                    {
                        cell.RessourceRevealed = true;
                        needToUpdateResources = true;
                    }
                }

                if( needToUpdateResources )
                    GameFacade.getInstance().SendNotification(GameNotification.UpdateResources, this);

                // evtl. enable policies
            }
        }

        public PlayerColor PlayerColor
        {
            get { return _civilization.PlayerColor; }
        }

        public virtual bool Update(GameTime time)
        {
            if (Map == null)
                return false;

            bool turned = false;
            _secondsToNextUpdate -= time.ElapsedGameTime.TotalSeconds;

            if (_secondsToNextUpdate <= 0f)
            {
                _secondsToNextUpdate = _secondsPerTurn;

                CalculateCulture();
                CalculateScience();

                UpdateScience();

                if( _needToUpdateInfluenceMaps )
                    UpdateInfluenceMaps();

                _isFirstRun = false;
                turned = true;
            }

            if (CityLocationMap != null)
            {
                foreach (Unit unit in _units)
                    unit.Update(time);

                _units.RemoveAll(u => u.Deleted);

                foreach (City city in _cities)
                    city.Update(time);
            }

            return turned;
        }

        private void UpdateScience()
        {
            if (CurrentResearch != null && _science > CurrentResearch.Cost)
            {
                GameFacade.getInstance().SendNotification(
                    GameNotification.Message, 
                    string.Format( Strings.TXT_KEY_NOTIFICATION_SCIENCE_DISCOVERED, Leader.Title, CurrentResearch.Title ), 
                    Civilization,
                    MessageFilter.Friends | MessageFilter.Self,
                    CurrentResearch);

                DiscoverTechnology(CurrentResearch);

                Technologies.Add(CurrentResearch);
                CurrentResearch = null;
                _science = 0;
            }

            if (CurrentResearch == null)
            {
                List<Tech> possibleTechnologies = PossibleTechnologies;

                // calculate flavours of city
                List<Flavour> flavours = _leader.Flavours;

                Tech best = null;
                float min = float.MaxValue;

                // calculate flavour difference for each unit/building
                foreach (Tech tech in possibleTechnologies)
                {
                    float dist = Flavours.Distance(tech.Flavours, flavours);

                    if (min > dist)
                    {
                        min = dist;
                        best = tech;
                    }
                }

                if (best == null)
                    throw new Exception("This can't be!");

                CurrentResearch = best;
            }
        }

        private void CalculateScience()
        {
            _science += ScienceSurplus;
        }

        public HexPoint StartLocation
        {
            get
            {
                StartLocation loc = Map.Extension.StartLocations.FirstOrDefault(a => a.CivilizationName == Civilization.Name);

                if (loc == null)
                    throw new Exception("No Start location for " + Civilization.Name);

                return new HexPoint(loc.X, loc.Y);
            }
        }

        private void UpdateInfluenceMaps()
        {
            //if (IsHuman)
            //    return;

            if (Map == null)
                return;

            if (CityLocationMap == null)
            {
                CityLocationMap = new InfluenceMap(Map.Width, Map.Height);
                ImprovementLocationMap = new InfluenceMap(Map.Width, Map.Height);
            }

            for (int x = 0; x < Map.Width; ++x)
            {
                for (int y = 0; y < Map.Height; ++y)
                {
                    HexPoint pt = new HexPoint(x, y);

                    #region city found map
                    MapCell cell = Map[x,y];
                    if (cell.IsSpotted(this))
                    {
                        float value = Map.GetValue(x, y, MapValueType.CityFoundValue);
                        CityLocationMap[x, y] = value;
                        ImprovementLocationMap[x, y] = value;

                        if (_cities.Count == 0)
                        {
                            int dist = HexPoint.GetDistance(pt, StartLocation);
                            if (dist < 2)
                                CityLocationMap[x, y] += 2;

                            if (dist < 1)
                                CityLocationMap[x, y] += 2;
                        }

                        if (GetCityAt(pt) != null)
                        {
                            CityLocationMap[pt] = 0;
                            ImprovementLocationMap[pt] = 0;
                        }

                        foreach (HexPoint n in pt.Neighbors)
                        {
                            if (GetCityAt(n) != null)
                            {
                                CityLocationMap[pt] = 0;
                                ImprovementLocationMap[x, y] *= 1.7f;
                            }
                        }
                    }
                    else
                        CityLocationMap[x, y] = 0;
                    #endregion city found map
                }
            }

            _needToUpdateInfluenceMaps = false;
        }

        public bool IsFirstRun
        {
            get
            {
                return _isFirstRun;
            }
        }

        protected void CalculateCulture()
        {
            _culture = 0;

            foreach (City city in _cities)
                _culture += city.Culture;
        }

        public float Culture
        {
            get
            {
                return _culture;
            }
        }

        public void AddUnit(string unitname, int x, int y)
        {
            UnitData data = Provider.GetUnit(unitname);
            Unit unit = new Unit(new HexPoint(x, y), this, data);

            //string script = MainApplication.ManagerInstance.Content.Load<string>("Content/Scripts/Marine");
            //unit.AssignScript(script);

            unit.Moved += new UnitMovedHandler(unit_Moved);

            unit.WorkFinished += delegate(Unit u, HexPoint pt, Improvement imp)
            {
                GameFacade.getInstance().SendNotification(
                    GameNotification.Message, 
                    string.Format(Strings.TXT_KEY_NOTIFICATION_BUILD_IMPROVEMENT, u.Player.Civilization.Title, imp.Title, pt), 
                    u.Player.Civilization,
                    MessageFilter.Friends | MessageFilter.Self,
                    imp);

                GameFacade.getInstance().SendNotification(GameNotification.UpdateImprovements);
            };

            _units.Add(unit);
        }

        void unit_Moved(Unit sender, HexPoint oldPosition, HexPoint newPosition)
        {
            _needToUpdateInfluenceMaps = true;
        }

        public void Draw(GameTime gameTime)
        {
            MainWindow mw = MainApplication.Instance.MainWindow as MainWindow;
            bool fogOfWarEnabled = mw.FogOfWarEnabled;

            Unit[] units = _units.ToArray();
            foreach (Unit unit in units)
            {
                if (!fogOfWarEnabled || Map[unit.Point].IsSpotted(MainWindow.Game.Human) )
                    unit.Draw(gameTime);
            }

            City[] cities = _cities.ToArray();
            foreach (City city in cities)
            {
                if (!fogOfWarEnabled || city.Player.IsHuman || Map[city.Point].IsSpotted(MainWindow.Game.Human) )
                    city.Draw(gameTime);
            }
        }

        Improvement road = Provider.GetImprovement("Road");
        public void AddCity(HexPoint point)
        {
            // check if this is really possible (controlled by must be -1 or Id)
            if (Map[point].ControlledBy != -1 && Map[point].ControlledBy != Id)
                throw new Exception("This should not have happend, cities can only be build on tiles that are free or controlled by yourself");

            if (!Map[point].Improvements.Contains(road))
                Map[point].Improvements.Add(road);

            string cityName = GetNextCityName();

            City c = new City(point, cityName, this);

            c.IsCapital = _cities.Count == 0;

            GameFacade.getInstance().SendNotification(
                GameNotification.Message, 
                string.Format(Strings.TXT_KEY_NOTIFICATION_FOUND_CITY, c.Player.Leader.Title, c.Name), 
                Civilization,
                MessageFilter.Self | MessageFilter.Friends,
                c);

            c.CityGrowth += delegate(City city, int from, int to) 
            { 
                GameFacade.getInstance().SendNotification(
                    GameNotification.Message, 
                    string.Format( Strings.TXT_KEY_NOTIFICATION_CITY_GREW, city.Name, c.Player.Leader.Title,to), 
                    city.Player.Civilization,
                    MessageFilter.Friends | MessageFilter.Self,
                    city); 
            };
            c.CityDecline += delegate(City city, int from, int to)
            {
                GameFacade.getInstance().SendNotification(
                    GameNotification.Message, 
                    string.Format(Strings.TXT_KEY_NOTIFICATION_CITY_DECLINE, city.Name, c.Player.Leader.Title,to),
                    city.Player.Civilization,
                    MessageFilter.Friends | MessageFilter.Self,
                    city);
            };
            c.CityBuild += delegate(City city, Building building)
            {
                GameFacade.getInstance().SendNotification(
                    GameNotification.Message, 
                    string.Format( Strings.TXT_KEY_NOTIFICATION_CITY_BUILDING, city.Name, c.Player.Leader.Title, building.Title ),
                    city.Player.Civilization,
                    MessageFilter.Self,
                    city);
            };
            c.UnitBuild += delegate(City city, UnitData unit)
            {
                GameFacade.getInstance().SendNotification(
                    GameNotification.Message, 
                    string.Format( Strings.TXT_KEY_NOTIFICATION_CITY_UNIT, city.Name, c.Player.Leader.Title, unit.Title ),
                    city.Player.Civilization,
                    MessageFilter.Self, 
                    city);
            };

            Map.SetControlled(point, Id, false, 1);

            _cities.Add(c);
        }

        public string GetNextCityName()
        {
            if (_cities.Count == 0)
            {
                if (_civilization.Capital.StartsWith("TXT_KEY_"))
                    return Provider.Instance.Translate(_civilization.Capital);
                else
                    return _civilization.Capital;
            }

            List<string> possibleNames = _civilization.Cities;

            // translate if needed
            possibleNames = possibleNames.Select(a => a.StartsWith("TXT_KEY_") ? Provider.Instance.Translate(a) : a).ToList();

            if (possibleNames.Count == 0)
                throw new Exception("No Citylist found for " + _civilization.Name);

            foreach (City city in _cities)
            {
                if( possibleNames.Contains(city.Name))
                    possibleNames.Remove(city.Name);
            }

            if( possibleNames.Count == 0 )
                return "Futurename";

            if (possibleNames.First().StartsWith("TXT_KEY_"))
                return Provider.Instance.Translate(possibleNames.First());
            else
                return possibleNames.First();
        }

        public void DeleteUnit(Unit unit)
        {
            _units.Remove(unit);
        }

        public IList<Policy> SelectedPolicies 
        {
            get
            {
                return _policies.AsReadOnly();
            }
        }

        public IList<PolicyType> AdoptedPolicyTypes
        {
            get
            {
                return _policyTypes.AsReadOnly();
            }
        }

        public bool AdoptPolicyType(PolicyType type)
        {
            if (!_policyTypes.Contains(type))
            {
                // check if enough culture is there
                float cultureNeededForChange = CultureNeededForChange;

                if (_culture < cultureNeededForChange)
                    return false;

                adoptedPolicies++;

                // check other types and unadopt them id needed 

                // send notification

                // finally add it
                _policyTypes.Add(type);

                return true;
            }

            return false;
        }


        public bool AdoptPolicy(Policy policy)
        {
            if (!_policies.Contains(policy))
            {
                // check if enough culture is there
                float cultureNeededForChange = CultureNeededForChange;

                if (_culture < cultureNeededForChange)
                    return false;

                adoptedPolicies++;

                _policies.Add(policy);

                return true;
            }

            return false;
        }

        public float SciencePerScientist
        {
            get
            {
                float science = 2f;

                foreach (Policy p in SelectedPolicies)
                    science += p.SciencePerScientist;

                return science;
            }
        }

        public float CultureNeededForChange
        {
            get
            {
                float culture = 25 + (float)Math.Pow(6 * adoptedPolicies, 1.7);

                culture *= MainWindow.Game.Handicap.CultureModifier;

                if(  MainWindow.Game.Map != null )
                    culture += MainWindow.Game.Map.Size.CultureModifierPerCity * Cities.Count;

                // game speed

                return culture;
            }
        }

        public int HappyCities
        {
            get
            {
                int happy = 0;

                foreach (City c in _cities)
                    if (c.Happiness >= c.Unhappiness)
                        happy++;

                return happy;
            }
        }

        public int UnhappyCities
        {
            get
            {
                int unhappy = 0;

                foreach (City c in _cities)
                    if (c.Happiness < c.Unhappiness)
                        unhappy++;

                return unhappy;
            }
        }

        public float ScienceSurplus
        {
            get
            {
                float science = 0f;

                foreach (City c in _cities)
                    science += c.ScienceSurplus;

                return science;
            }
        }

        public float ScienceReady
        {
            get
            {
                if (CurrentResearch != null)
                    return _science / CurrentResearch.Cost;

                return 0f; 
            }
        }

        private static float SCORE_CITY_MULTIPLIER = 8;
        private static float SCORE_POPULATION_MULTIPLIER = 4;
        private static float SCORE_LAND_MULTIPLIER = 1;
        private static float SCORE_WONDER_MULTIPLIER = 25;
        private static float SCORE_TECH_MULTIPLIER = 4;
        private static float SCORE_FUTURE_TECH_MULTIPLIER = 10;

        public float Score
        {
            get 
            {
                float score = 0;

                if (Map == null)
                    return 0;

                score += _cities.Count * SCORE_CITY_MULTIPLIER;
                score += Cities.Sum(a => a.Citizen) * SCORE_POPULATION_MULTIPLIER;
                score += Map.Tiles.Count(a => a.ControlledBy == Id) * SCORE_LAND_MULTIPLIER;
                score += Cities.Sum(a => a.Wonders.Count) * SCORE_WONDER_MULTIPLIER;
                score += Technologies.Count * SCORE_TECH_MULTIPLIER;

                return score;
            }
        }
    }
}