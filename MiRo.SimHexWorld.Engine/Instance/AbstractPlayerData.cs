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
        double _secondsPerTurn = 0.5f;
        bool _isFirstRun = true;

        bool _needToUpdateInfluenceMaps = true;

        LeaderData _leader;

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

            Technologies = _civilization.StartingTechs;
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
                _era = tech.Era;
        }

        public virtual bool Update(GameTime time)
        {          
            _secondsToNextUpdate -= time.ElapsedGameTime.TotalSeconds;

            if (_secondsToNextUpdate <= 0f)
            {
                _secondsToNextUpdate = _secondsPerTurn;

                CalculateCulture();
                CalculateScience();

                UpdateScience();

                if( _needToUpdateInfluenceMaps )
                    UpdateInfluenceMaps();

                foreach (Unit unit in _units)
                    unit.Update(time);

                _units.RemoveAll(u => u.Deleted);

                foreach (City city in _cities)
                    city.Update(time);

                _isFirstRun = false;

                return true;
            }

            return false;
        }

        private void UpdateScience()
        {
            if (CurrentResearch != null && _science > CurrentResearch.Cost)
            {
                GameFacade.getInstance().SendNotification(GameNotification.Message, _civilization.Name + " has discovered " + CurrentResearch.Name, CurrentResearch);
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
                        CityLocationMap[x, y] = Map.GetValue(x, y, MapValueType.CityFoundValue);
                        ImprovementLocationMap[x, y] = Map.GetValue(x, y, MapValueType.CityFoundValue);

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

                        foreach(HexPoint n in pt.Neighbors )
                            if (GetCityAt(n) != null)
                            {
                                CityLocationMap[n] = 0;
                                ImprovementLocationMap[x, y] *= 1.7f;
                            }
                    }
                    else
                        CityLocationMap[x, y] = 0;
                    #endregion city found map
                }
            }
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
                GameFacade.getInstance().SendNotification(GameNotification.Message, string.Format(Strings.TXT_KEY_NOTIFICATION_BUILD_IMPROVEMENT, u.Player.Civilization.Title, imp.Title, pt), imp);

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
                if (Map[unit.Point].IsSpotted(MainWindow.Game.Human) || !fogOfWarEnabled)
                    unit.Draw(gameTime);
            }

            City[] cities = _cities.ToArray();
            foreach (City city in cities)
            {
                if (Map[city.Point].IsSpotted(MainWindow.Game.Human) || !fogOfWarEnabled)
                    city.Draw(gameTime);
            }
        }

        public void AddCity(HexPoint point)
        {
            // check if this is really possible (controlled by must be -1 or Id)
            if (Map[point].ControlledBy != -1 && Map[point].ControlledBy != Id)
                throw new Exception("This should not have happend, cities can only be build on tiles that are free or controlled by yourself");

            string cityName = GetNextCityName();

            City c = new City(point, cityName, this);

            c.IsCapital = _cities.Count == 0;

            GameFacade.getInstance().SendNotification(GameNotification.Message, string.Format(Strings.TXT_KEY_NOTIFICATION_FOUND_CITY, c.Player.Leader.Title, c.Name), c);

            c.CityGrowth += delegate(City city, int from, int to) 
            { 
                GameFacade.getInstance().SendNotification(GameNotification.Message, "City " + city.Name + " grew from " + from + " to " + to, city); 
            };
            c.CityDecline += delegate(City city, int from, int to)
            {
                GameFacade.getInstance().SendNotification(GameNotification.Message, "City " + city.Name + " declined from " + from + " to " + to, city);
            };
            c.CityBuild += delegate(City city, Building building)
            {
                GameFacade.getInstance().SendNotification(GameNotification.Message, "City " + city.Name + " build " + building.Title, city);
            };
            c.UnitBuild += delegate(City city, UnitData unit)
            {
                GameFacade.getInstance().SendNotification(GameNotification.Message, "Unit " + unit.Title + " build in " + city.Name, city);
            };

            Map.SetControlled(point, Id, false, 1);

            _cities.Add(c);
        }

        public string GetNextCityName()
        {
            if (_cities.Count == 0)
                return _civilization.Capital;

            List<string> possibleName = _civilization.Cities;

            if (possibleName.Count == 0)
                throw new Exception("No Citylist found for " + _civilization.Name);

            foreach (City city in _cities)
            {
                if( possibleName.Contains(city.Name))
                    possibleName.Remove(city.Name);
            }

            if( possibleName.Count == 0 )
                return "Futurename";

            return possibleName.First();
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
    }
}