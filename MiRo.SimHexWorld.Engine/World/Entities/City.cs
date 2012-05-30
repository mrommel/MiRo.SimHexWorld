using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.AI;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.UI.Entities;
using MiRo.SimHexWorld.Engine.UI;
using MiRoSimHexWorld.Engine.World;
using MiRo.SimHexWorld.Engine.UI.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Instance.AI;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public enum WorkType { Unemployed, Land, Sea, Artists, Merchants, Scientists, Engineers }

    public class CitizenWork : IEquatable<CitizenWork>
    {
        public HexPoint Point;
        public WorkType Type = WorkType.Unemployed;

        public bool Equals(CitizenWork other)
        {
            return Type == other.Type && Point == other.Point;
        }
    }

    public delegate void CityGrowthHandler(City city, int fromCitizen, int toCitizen);
    public delegate void CityBuildHandler(City city, Building building);
    public delegate void CityUnitBuildHandler( City city, UnitData unit );

    public class City
    {
        private HexPoint _point;
        private string _name;
        AbstractPlayerData _player;
        CitySpecialization _specialization;

        CityEntity _entity;

        List<Building> _buildings = new List<Building>();
        List<WonderData> _wonders = new List<WonderData>();

        const int AI_CITYSTRATEGY_SMALL_CITY_POP_THRESHOLD = 2;
        const int AI_CITYSTRATEGY_MEDIUM_CITY_POP_THRESHOLD = 8;
        const int AI_CITYSTRATEGY_LARGE_CITY_POP_THRESHOLD = 15;

        public enum CitySize { Tiny, Small, Medium, Large }

        protected float _food = 10f;
        private static float _foodPerCitizen = 1f;
        protected float _population = 1000f;

        protected float _science, _production, _gold, _culture;

        List<CitizenWork> _work = new List<CitizenWork>();

        UnitData _currentUnit;
        Building _currentBuilding = null;
        float _currentBuildingProgress = 0;

        int neighborHood = 1;

        //static PythonEngine _behaviour;
        static BillboardSystem<Civilization> _civilizationFlagBillboards;
        BillboardSystem<string> _cityNameBillboard;
        BillboardSystem<float> _cityFoodBillboard;
        BillboardSystem<float> _cityProductionBillboard;
        BillboardSystem<float> _cityGoldBillboard;

        // events
        public event CityGrowthHandler CityGrowth;
        public event CityGrowthHandler CityDecline;
        public event CityBuildHandler CityBuild;
        public event CityUnitBuildHandler UnitBuild;

        public City(HexPoint point, string name, AbstractPlayerData player)
        {
            _name = name;
            _player = player;
            _point = point;

            _specialization = Provider.GetCitySpecializationDefault();

            _entity = new CityEntity(this);
            _entity.Rotation.X = -MathHelper.PiOver2;

            if (_civilizationFlagBillboards == null)
            {
                _civilizationFlagBillboards = new BillboardSystem<Civilization>(MainApplication.Instance.GraphicsDevice, MainApplication.Instance.Content);

                foreach (Civilization civ in Provider.Instance.Civilizations.Values)
                    _civilizationFlagBillboards.AddEntity(civ, civ.Image, new Vector2(1, 1));
            }

            _civilizationFlagBillboards.AddPosition(_player.Civilization, _entity.Position + new Vector3(0, 3, 0));
            _civilizationFlagBillboards.Build();

            _cityNameBillboard = new BillboardSystem<string>(MainApplication.Instance.GraphicsDevice, MainApplication.Instance.Content);
            _cityNameBillboard.AddEntity(Name, TextTexture, new Vector2(5, 1));
            _cityNameBillboard.AddPosition(Name, _entity.Position + new Vector3(0, 1, 0));
            _cityNameBillboard.Build();

            _cityFoodBillboard = new BillboardSystem<float>(MainApplication.Instance.GraphicsDevice, MainApplication.Instance.Content);
            _cityFoodBillboard.AddEntity(1, Provider.GetAtlas("YieldAtlas").GetTexture("Food1"), new Vector2(1, 1));
            _cityFoodBillboard.AddEntity(2, Provider.GetAtlas("YieldAtlas").GetTexture("Food2"), new Vector2(1, 1));
            _cityFoodBillboard.AddEntity(3, Provider.GetAtlas("YieldAtlas").GetTexture("Food3"), new Vector2(1, 1));
            _cityFoodBillboard.AddEntity(4, Provider.GetAtlas("YieldAtlas").GetTexture("Food4"), new Vector2(1, 1));
            _cityFoodBillboard.AddEntity(5, Provider.GetAtlas("YieldAtlas").GetTexture("Food5"), new Vector2(1, 1));

            _cityProductionBillboard = new BillboardSystem<float>(MainApplication.Instance.GraphicsDevice, MainApplication.Instance.Content);
            _cityProductionBillboard.AddEntity(1, Provider.GetAtlas("YieldAtlas").GetTexture("Prod1"), new Vector2(1, 1));
            _cityProductionBillboard.AddEntity(2, Provider.GetAtlas("YieldAtlas").GetTexture("Prod2"), new Vector2(1, 1));
            _cityProductionBillboard.AddEntity(3, Provider.GetAtlas("YieldAtlas").GetTexture("Prod3"), new Vector2(1, 1));
            _cityProductionBillboard.AddEntity(4, Provider.GetAtlas("YieldAtlas").GetTexture("Prod4"), new Vector2(1, 1));
            _cityProductionBillboard.AddEntity(5, Provider.GetAtlas("YieldAtlas").GetTexture("Prod5"), new Vector2(1, 1));

            _cityGoldBillboard = new BillboardSystem<float>(MainApplication.Instance.GraphicsDevice, MainApplication.Instance.Content);
            _cityGoldBillboard.AddEntity(1, Provider.GetAtlas("YieldAtlas").GetTexture("Gold1"), new Vector2(1, 1));
            _cityGoldBillboard.AddEntity(2, Provider.GetAtlas("YieldAtlas").GetTexture("Gold2"), new Vector2(1, 1));
            _cityGoldBillboard.AddEntity(3, Provider.GetAtlas("YieldAtlas").GetTexture("Gold3"), new Vector2(1, 1));
            _cityGoldBillboard.AddEntity(4, Provider.GetAtlas("YieldAtlas").GetTexture("Gold4"), new Vector2(1, 1));
            _cityGoldBillboard.AddEntity(5, Provider.GetAtlas("YieldAtlas").GetTexture("Gold5"), new Vector2(1, 1));
        }

        private Texture2D TextTexture
        {
            get
            {
                SpriteFont font = MainApplication.Instance.Content.Load<SpriteFont>("Content//Fonts//ArialS");
                Vector2 fontSize = font.MeasureString(Citizen + " " + Name);
                Vector2 pos = new Vector2((128 - fontSize.X) / 2, (32 - fontSize.Y) / 2);

                RenderTarget2D target = new RenderTarget2D(MainApplication.Instance.GraphicsDevice, 128, 32);
                MainApplication.Instance.GraphicsDevice.SetRenderTarget(target);// Now the spriteBatch will render to the RenderTarget2D

                MainApplication.Instance.GraphicsDevice.Clear(Color.Transparent);

                SpriteBatch spriteBatch = new SpriteBatch(MainApplication.Instance.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.DrawString(font, Citizen + " " + Name, pos, Color.White);//Do your stuff here
                spriteBatch.End();

                MainApplication.Instance.GraphicsDevice.SetRenderTarget(null);//This will set the spriteBatch to render to the screen again.

                return target;
            }
        }

        bool _isCapital;
        public bool IsCapital
        {
            get
            { return _isCapital; }
            set
            {
                _isCapital = value;

                // this is the capital, we have the palace
                if (_isCapital)
                    _buildings.Add(Provider.GetBuilding("Palace"));
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public HexPoint Point
        {
            get
            {
                return _point;
            }
        }

        public List<Building> Buildings
        {
            get
            { return _buildings; }
        }

        public List<WonderData> Wonders
        {
            get
            {
                return _wonders;
            }
        }

        public AbstractPlayerData Player
        {
            get
            {
                return _player;
            }
        }

        public CitySize Size
        {
            get
            {
                if (_population < AI_CITYSTRATEGY_SMALL_CITY_POP_THRESHOLD)
                    return CitySize.Tiny;
                else if (_population < AI_CITYSTRATEGY_MEDIUM_CITY_POP_THRESHOLD)
                    return CitySize.Small;
                else if (_population < AI_CITYSTRATEGY_LARGE_CITY_POP_THRESHOLD)
                    return CitySize.Medium;
                else
                    return CitySize.Large;
            }
        }

        int[] _populationLimits = new int[]
        {
            1000,
            6000,
            21000,
            48000,
            90000,
            150000,
            232000,
            337000,
            469000,
            630000,
            823000,
            1051000,
            1315000,
            1618000,
            1963000,
            2352000,
            2787000,
            3271000,
            3806000,
            4394000,
            5037000,
            5738000,
            6498000,
            7321000,
            8207000,
            9160000,
            10181000,
            11273000,
            12436000,
            13675000,
            14990000,
            16383000,
            17858000,
            19415000,
            21056000,
            22784000,
            24601000,
            26509000,
            28508000,
            30603000
        };

        public int Citizen
        {
            get
            {
                for (int i = 0; i < _populationLimits.Length; ++i)
                {
                    if (_population < _populationLimits[i])
                        return i;
                }

                return _populationLimits.Length;
            }
        }

        public int Happiness
        {
            get
            {
                int happiness = 0;

                // connected to capital (not sieged)

                // social policies
                _food += _player.Policies.Sum(a => a.YieldsHappiness);

                // buildings
                foreach (Building b in _buildings)
                    happiness += b.Happiness;

                // connected to luxuries

                return happiness;
            }
        }

        public int Unhappiness
        {
            get
            {
                return Citizen * 3;
            }
        }

        TimeSpan updateEvery = TimeSpan.FromSeconds(0.5); // from setting ???
        TimeSpan currentTime = TimeSpan.FromSeconds(0);
        public void Update(GameTime time)
        {
            currentTime -= time.ElapsedGameTime;
            if (currentTime.TotalMilliseconds <= 0)
            {
                UpdatePopulation();
                UpdateWork();
                UpdateProduction();

                // reset timer
                currentTime = updateEvery;
            }

            if (!_player.IsHuman)
                UpdateAI();

            _entity.Update(time);
        }

        public int Population
        {
            get
            {
                return (int)_population;
            }
        }

        private void UpdatePopulation()
        {
            _food -= Citizen * _foodPerCitizen;
            bool enoughFood = _food > 0;

            float growthRate = enoughFood ? 1.02f : 0.98f;

            // apply handicap/difficulty
            if (!_player.IsHuman)
                growthRate = (float)Math.Pow(growthRate, MainWindow.Game.Handicap.AIGrowthModifier);

            // apply era
            growthRate = (float)Math.Pow(growthRate, _player.Era.GrowthModifier);

            // happiness
            if (Happiness < Unhappiness)
                growthRate = Math.Max(growthRate, 1.01f);

            int oldCitizen = Citizen;

            _population = (float)Math.Pow(_population, growthRate);

            int newCitizen = Citizen;

            if (newCitizen > oldCitizen)
            {
                // update city text
                _cityNameBillboard.Reset();
                _cityNameBillboard.AddEntity(Name, TextTexture, new Vector2(5, 1));
                _cityNameBillboard.AddPosition(Name, _entity.Position + new Vector3(0, 1, 0));
                _cityNameBillboard.Build();

                if (CityGrowth != null)
                    CityGrowth(this, oldCitizen, newCitizen);
            }
            else if (newCitizen < oldCitizen)
            {
                // update city text
                _cityNameBillboard.Reset();
                _cityNameBillboard.AddEntity(Name, TextTexture, new Vector2(5, 1));
                _cityNameBillboard.AddPosition(Name, _entity.Position + new Vector3(0, 1, 0));
                _cityNameBillboard.Build();

                if (CityDecline != null)
                    CityDecline(this, oldCitizen, newCitizen);
            }
        }

        private void UpdateWork()
        {
            bool changed = false;
            _work.RemoveAll(a => a.Type == WorkType.Unemployed);

            // are there too much worker -> reduce
            while (Citizen > _work.Count)
            {
                AddBestEfficientWorker();
                changed = true;
            }

            // are there any citizen without work -> find best place
            while (Citizen < _work.Count)
            {
                RemoveLeastEfficientWorker();
                changed = true;
            }

            if (changed)
            {
                _cityFoodBillboard.ResetPositions();
                _cityProductionBillboard.ResetPositions();
                _cityGoldBillboard.ResetPositions();

                foreach (CitizenWork work in _work)
                {
                    if (work.Type == WorkType.Land || work.Type == WorkType.Sea)
                    {
                        Gain gain = GetGain(work);
                        _cityFoodBillboard.AddPosition(gain.Food, MapData.GetWorldPosition(work.Point) + new Vector3(0, 1, 0));
                        _cityProductionBillboard.AddPosition(gain.Production, MapData.GetWorldPosition(work.Point) + new Vector3(0, 1.5f, 0));
                        _cityGoldBillboard.AddPosition(gain.Gold, MapData.GetWorldPosition(work.Point) + new Vector3(0, 2, 0));
                    }
                }

                _cityFoodBillboard.Build();
                _cityProductionBillboard.Build();
                _cityGoldBillboard.Build();
            }
        }

        private void AddBestEfficientWorker()
        {
            List<CitizenWork> possibleWorks = new List<CitizenWork>();

            foreach (HexPoint pt in Point.GetNeighborhood(neighborHood))
            {
                if (Map.IsValid(pt) && Map[pt].IsSpotted(_player) && Map[pt].ControlledBy == _player.Id)
                {
                    CitizenWork work = new CitizenWork();
                    work.Point = pt;
                    work.Type = Map[pt].IsOcean ? WorkType.Sea : WorkType.Land;
                    possibleWorks.Add(work);
                }
            }

            foreach (Building b in _buildings)
            {
                for (int i = 0; i < b.SpecialistSlots; i++)
                {
                    CitizenWork work = new CitizenWork();

                    switch (b.SpecialistType)
                    {
                        case SpecialistType.Scientist:
                            work.Type = WorkType.Scientists;
                            break;
                        case SpecialistType.Merchant:
                            work.Type = WorkType.Merchants;
                            break;
                        default:
                            throw new Exception("No Handle for " + b.SpecialistType);
                    }

                    possibleWorks.Add(work);
                }
            }

            // remove current work
            foreach (CitizenWork current in _work)
                possibleWorks.RemoveAll(a => a.Equals(current));

            possibleWorks.Sort(SortWork);

            if (possibleWorks.Count > 0)
                _work.Add(possibleWorks.First());
            else
                _work.Add(new CitizenWork());
        }

        private int SortWork(CitizenWork w1, CitizenWork w2)
        {
            return (int)(GetGain(w2).Sum - GetGain(w1).Sum);
        }

        public struct Gain
        {
            public float Food, Production, Gold, Science, Culture;

            public float Sum
            {
                get
                {
                    // food is more important
                    return 2 * Food + Production + Gold + Science + Culture;
                }
            }
        }

        public Gain GetGain(CitizenWork work)
        {
            Gain gain = new Gain();

            switch (work.Type)
            {
                case WorkType.Unemployed:
                    gain.Food = 0;
                    gain.Production = 0.3f; // ??? settings
                    gain.Gold = 0;
                    gain.Science = 0;
                    gain.Culture = 0;
                    break;
                case WorkType.Sea:
                case WorkType.Land:
                    gain.Food = Map[work.Point].Food;
                    gain.Production = Map[work.Point].Production;
                    gain.Gold = Map[work.Point].Commerce;
                    gain.Science = 0;
                    gain.Culture = 0;
                    break;
                case WorkType.Scientists:
                    gain.Food = 0;
                    gain.Production = 0;
                    gain.Gold = 0;
                    gain.Science = 2;
                    gain.Culture = 0;
                    break;
                default:
                    throw new Exception("Handler for " + work.Type + " not implemented");
            }

            return gain;
        }

        private void RemoveLeastEfficientWorker()
        {
            if (_work.Count == 0)
                return;

            _work.Sort(SortWork);

            _work.RemoveAt(_work.Count - 1);
        }

        public Building CurrentBuilding
        {
            get
            {
                return _currentBuilding;
            }
        }

        private void UpdateProduction()
        {
            // increment current building
            if (_currentBuilding == null && _currentUnit == null)
                ChooseCurrentProductionTarget();

            // receive food, production, commerce
            _currentBuildingProgress += ProductionSurplus;
            _food += FoodSurplus;
            _gold += GoldSurplus;
            _science += ScienceSurplus;
            _culture += CultureSurplus;

            if (_currentBuilding != null && _currentBuilding.Cost <= _currentBuildingProgress)
            {
                // hurra we build building xy
                _buildings.Add(_currentBuilding);

                if (CityBuild != null)
                    CityBuild(this, _currentBuilding);

                _currentBuilding = null;
                _currentBuildingProgress = 0;
            }
            else if (_currentUnit != null && _currentUnit.Cost <= _currentBuildingProgress)
            {
                // hurra we build unit xy
                _player.AddUnit(_currentUnit.Name, Point.X, Point.Y);

                if (UnitBuild != null)
                    UnitBuild(this, _currentUnit);

                _currentUnit = null;
                _currentBuildingProgress = 0;
            }
            // wonders ????
        }

        private void ChooseCurrentProductionTarget()
        {
            switch (_specialization.Name)
            {
                case "GeneralEconomic":
                    {
                        List<Building> possibleBuildings = PossibleBuildings;
                        List<UnitData> possibleUnits = PossibleUnits;

                        // calculate flavours of city
                        List<Flavour> flavours = _player.Leader.Flavours;

                        Building bestBuilding = null;
                        UnitData bestUnit = null;
                        float minBuilding = float.MaxValue;
                        float minUnit = float.MaxValue;

                        // calculate flavour difference for each unit/building
                        foreach (Building b in possibleBuildings)
                        {
                            float dist = Flavours.Distance(b.Flavours, flavours);

                            if (minBuilding > dist)
                            {
                                minBuilding = dist;
                                bestBuilding = b;
                            }
                        }

                        // fetch units from surrounding
                        Flavours unitFlavours = new Flavours();
                        foreach (HexPoint pt in Point.Neighbors)
                        {
                            if (Map.IsValid(pt))
                            {
                                foreach (Unit unit in MainWindow.Game.GetUnitsAt(pt))
                                    unitFlavours += (unit.Data.Flavours / 10f);
                            }
                        }

                        foreach (Unit unit in MainWindow.Game.GetUnitsAt(Point))
                            unitFlavours += (unit.Data.Flavours / 3f);

                        foreach (UnitData u in possibleUnits)
                        {
                            // evtl. nearby units zur aktuellen unit addieren
                            float dist = Flavours.Distance(u.Flavours + unitFlavours, flavours);

                            if (minUnit > dist)
                            {
                                minUnit = dist;
                                bestUnit = u;
                            }
                        }

                        if (minBuilding < minUnit)
                            _currentBuilding = bestBuilding;
                        else
                            _currentUnit = bestUnit;
                    }
                    break;
                default:
                    throw new Exception("No handle for: " + _specialization.Name);
                    break;
            }
        }

        public Flavours Flavours
        {
            get
            {
                Flavours flavours = new Flavours();

                // buildings
                foreach (Building b in _buildings)
                    flavours += b.Flavours;

                // wonders

                return flavours;
            }
        }

        private MapData Map
        {
            get
            {
                return MainWindow.Game.Map;
            }
        }

        private void UpdateAI()
        {
            // select city specialization, etc
        }

        public void Draw(GameTime time)
        {
            _entity.Draw(time);

            // draw tribe flag  
            _civilizationFlagBillboards.Draw(GameMapBox.Camera.View, GameMapBox.Camera.Projection, GameMapBox.Camera.Position, GameMapBox.Camera.Up, GameMapBox.Camera.Right);

            // draw name, population (citizen)
            _cityNameBillboard.Draw(GameMapBox.Camera.View, GameMapBox.Camera.Projection, GameMapBox.Camera.Position, GameMapBox.Camera.Up, GameMapBox.Camera.Right);

            // draw food
            if (InDetailView)
            {
                _cityFoodBillboard.Draw(GameMapBox.Camera.View, GameMapBox.Camera.Projection, GameMapBox.Camera.Position, GameMapBox.Camera.Up, GameMapBox.Camera.Right);
                _cityProductionBillboard.Draw(GameMapBox.Camera.View, GameMapBox.Camera.Projection, GameMapBox.Camera.Position, GameMapBox.Camera.Up, GameMapBox.Camera.Right);
                _cityGoldBillboard.Draw(GameMapBox.Camera.View, GameMapBox.Camera.Projection, GameMapBox.Camera.Position, GameMapBox.Camera.Up, GameMapBox.Camera.Right);

            }
        }

        public int FoodSurplus
        {
            get
            {
                int food = 0;

                // buildings
                foreach (Building building in _buildings)
                    food += building.YieldsFood;

                // citizen
                foreach (CitizenWork work in _work)
                {
                    if (work.Type == WorkType.Land || work.Type == WorkType.Sea)
                    {
                        food += Map[work.Point].Food;
                    }
                }

                // social policies
                if (IsCapital)
                    food += _player.Policies.Sum(a => a.YieldsFoodCapital);

                _food += _player.Policies.Sum(a => a.YieldsFood);

                return food;
            }
        }

        public float ProductionSurplus
        {
            get
            {
                float production = 0;
                float modifier = 1f;

                // citizen
                foreach (CitizenWork work in _work)
                {
                    if (work.Type == WorkType.Land || work.Type == WorkType.Sea)
                    {
                        production += Map[work.Point].Production;
                    }
                }

                // buildings
                foreach (Building building in _buildings)
                {
                    production += building.YieldsProduction;
                    modifier += building.ProductionModifier;
                }

                production *= modifier;

                return production;
            }
        }

        public int GoldSurplus
        {
            get
            {
                return 0;
            }
        }

        public bool InDetailView { get; set; }

        public float ScienceSurplus
        {
            get
            {
                float science = 0;
                float modifier = 1f;

                foreach (Building building in _buildings)
                {
                    science += building.YieldsScience;
                    science += building.SciencePerCitizen * Citizen;
                    modifier += building.ScienceModifier;
                }

                foreach (WonderData wonder in _wonders)
                {
                    science += wonder.YieldsScience;
                    modifier += wonder.ScienceModifier;
                }

                foreach (CitizenWork work in _work)
                {
                    if (work.Type == WorkType.Scientists)
                        science += _player.SciencePerScientist;
                }

                foreach (Policy policy in _player.SelectedPolicies)
                {
                    science += policy.YieldsScience;
                    modifier += policy.ScienceModifier;
                }

                science += Citizen;

                science *= modifier;

                return science;
            }
        }

        public int CultureSurplus
        {
            get
            {
                int surplus = 0;

                foreach (Building building in _buildings)
                    surplus += building.Culture;

                return surplus;
            }
        }

        public void Demolish(Building buildingToDemolish)
        {
            _buildings.Remove(buildingToDemolish);

            // reward half the building cost
            _gold += buildingToDemolish.Cost / 2;
        }

        public float Culture { get { return _culture; } }

        public List<Building> PossibleBuildings
        {
            get
            {
                List<Building> buildings = new List<Building>();

                foreach (Building b in Provider.Instance.Buildings.Values)
                {
                    if (!_buildings.Contains(b))
                    {
                        // techs
                        if ((b.RequiredTech == null || _player.Technologies.Contains(b.RequiredTech)) &&
                            ((b.InCapitalOnly == IsCapital) || IsCapital))
                            buildings.Add(b);
                    }
                }

                return buildings;
            }
        }

        public List<UnitData> PossibleUnits
        {
            get
            {
                List<UnitData> units = new List<UnitData>();

                foreach (UnitData u in Provider.Instance.Units.Values)
                {
                    // resources
                    bool resources = true;

                    if( u.RequiredRessourceNames != null )
                    {
                        foreach(Ressource r in u.RequiredRessources )
                            resources &= _player.Ressources.Contains(r);
                    }

                    // techs
                    bool tech = ((u.RequiredTech == null || _player.Technologies.Contains(u.RequiredTech)) &&
                        (u.ObsoleteTech == null || !_player.Technologies.Contains(u.ObsoleteTech)));
                      
                    if( resources && tech )
                        units.Add(u);
                }

                return units;
            }
        }

        public float ProductionReady
        {
            get
            {
                if (_currentBuilding != null)
                    return _currentBuildingProgress / _currentBuilding.Cost;

                if (_currentUnit != null)
                    return _currentBuildingProgress / _currentUnit.Cost;

                return 0f;
            }
        }
    }
}
