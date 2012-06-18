using System;
using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.UI;
using log4net;
using MiRo.SimHexWorld.Engine.World.Maps;
using System.IO;
using MiRo.SimHexWorld.Engine.Locales;

namespace MiRo.SimHexWorld.Engine.Misc
{
    public class Provider
    {
        readonly ILog _log = LogManager.GetLogger(typeof(Provider));

        public readonly Dictionary<string, Terrain> Terrains;
        public readonly Dictionary<string, Ressource> Ressources;
        public readonly Dictionary<string, Feature> Features;
        public readonly Dictionary<string, UnitData> Units;
        public readonly Dictionary<string, Era> Eras;
        public readonly Dictionary<string, Tech> Techs;
        public readonly Dictionary<string, Civilization> Civilizations;
        public readonly Dictionary<string, Improvement> Improvements;
        public readonly Dictionary<string, Policy> Policies;
        public readonly Dictionary<string, PolicyType> PolicyTypes;
        public readonly Dictionary<string, Building> Buildings;
        public readonly Dictionary<string, CultureLevel> CultureLevels;
        public readonly Dictionary<string, MapSize> MapSizes;
        public readonly Dictionary<string, MapType> MapTypes;
        public readonly Dictionary<string, Difficulty> Difficulties;
        public readonly Dictionary<string, Pace> Paces;

        public readonly Dictionary<string, FlavourData> Flavours;
        public readonly Dictionary<string, LeaderData> Leaders;
        public readonly Dictionary<string, LeaderAbility> LeaderAbilities;
        public readonly Dictionary<string, GrandStrategyData> GrandStrategies;
        public readonly Dictionary<string, MilitayStrategyData> MilitayStrategies;
        public readonly Dictionary<string, CitySpecialization> CitySpecializations;
        public readonly Dictionary<string, PromotionData> Promotions;

        public readonly Dictionary<string, Civ5Map> Maps;

        public readonly Dictionary<string, TextureAtlas> Atlases;

        private Provider()
        {
            Terrains = MainApplication.ManagerInstance.Content.LoadContent<Terrain>("Content\\Terrains");
            Ressources = MainApplication.ManagerInstance.Content.LoadContent<Ressource>("Content\\Ressources");
            Features = MainApplication.ManagerInstance.Content.LoadContent<Feature>("Content\\Features");

            Units = MainApplication.ManagerInstance.Content.LoadContent<UnitData>("Content\\Units");
            Eras = MainApplication.ManagerInstance.Content.LoadContent<Era>("Content\\Eras");
            Techs = MainApplication.ManagerInstance.Content.LoadContent<Tech>("Content\\Techs");
            Civilizations = MainApplication.ManagerInstance.Content.LoadContent<Civilization>("Content\\Types\\Civilizations");
            Improvements = MainApplication.ManagerInstance.Content.LoadContent<Improvement>("Content\\Improvements");
            Policies = MainApplication.ManagerInstance.Content.LoadContent<Policy>("Content\\Policies", "*.*", true);         
            PolicyTypes = MainApplication.ManagerInstance.Content.LoadContent<PolicyType>("Content\\PolicyTypes");
            Buildings = MainApplication.ManagerInstance.Content.LoadContent<Building>("Content\\Buildings");
            CultureLevels = MainApplication.ManagerInstance.Content.LoadContent<CultureLevel>("Content\\CultureLevels");
            MapSizes = MainApplication.ManagerInstance.Content.LoadContent<MapSize>("Content\\MapSizes");
            MapTypes = MainApplication.ManagerInstance.Content.LoadContent<MapType>("Content\\MapTypes");
            Difficulties = MainApplication.ManagerInstance.Content.LoadContent<Difficulty>("Content\\Difficulties");

            Flavours = MainApplication.ManagerInstance.Content.LoadContent<FlavourData>("Content\\Types\\Flavours");
            Leaders = MainApplication.ManagerInstance.Content.LoadContent<LeaderData>("Content\\Types\\Leaders");
            LeaderAbilities = MainApplication.ManagerInstance.Content.LoadContent<LeaderAbility>("Content\\Types\\LeaderAbilities");
            MilitayStrategies = MainApplication.ManagerInstance.Content.LoadContent<MilitayStrategyData>("Content\\Types\\MilitayStrategies");
            GrandStrategies = MainApplication.ManagerInstance.Content.LoadContent<GrandStrategyData>("Content\\Types\\GrandStrategies");
            CitySpecializations = MainApplication.ManagerInstance.Content.LoadContent<CitySpecialization>("Content\\Types\\CitySpecializations");
            Promotions = MainApplication.ManagerInstance.Content.LoadContent<PromotionData>("Content\\Types\\Promotions");

            //Paces = MainApplication.ManagerInstance.Content.LoadContent<Pace>("Content\\Paces");
            Maps = MainApplication.ManagerInstance.Content.LoadContent<Civ5Map>("Content\\Maps");

            UpdateMaps();

            Atlases = MainApplication.ManagerInstance.Content.LoadContent<TextureAtlas>("Content\\Textures\\Atlases", "*Atlas.*");

            _log.InfoFormat("Terrains:      {0}", Terrains.Count);
            _log.InfoFormat("Ressources:    {0}", Ressources.Count);
            _log.InfoFormat("Features:      {0}", Features.Count);
            _log.InfoFormat("Units:         {0}", Units.Count);
            _log.InfoFormat("Eras:          {0}", Eras.Count);
            _log.InfoFormat("Techs:         {0}", Techs.Count);
            _log.InfoFormat("Civilizations: {0}", Civilizations.Count);
            _log.InfoFormat("Improvements:  {0}", Improvements.Count);
            _log.InfoFormat("Policies:      {0}", Policies.Count);
            _log.InfoFormat("PolicyTypes:   {0}", PolicyTypes.Count);
            _log.InfoFormat("Buildings:     {0}", Buildings.Count);
            _log.InfoFormat("CultureLevels: {0}", CultureLevels.Count);
            _log.InfoFormat("MapSizes:      {0}", MapSizes.Count);
            _log.InfoFormat("MapTypes:      {0}", MapTypes.Count);
            _log.InfoFormat("Difficulties:  {0}", Difficulties.Count);
            _log.InfoFormat("Flavours:  {0}", Flavours.Count);
            //log.InfoFormat("Paces:         {0}", Paces.Count);
            _log.InfoFormat("Maps:          {0}", Maps.Count);
        }

        private void UpdateMaps()
        {
            foreach (KeyValuePair<string, Civ5Map> mapPair in Maps)
            {
                mapPair.Value.ImageName = mapPair.Key;
                mapPair.Value.Name += ("_" + mapPair.Key).ToUpper();
                mapPair.Value.Description += ("_" + mapPair.Key).ToUpper();
                mapPair.Value.FileName = mapPair.Key;
            }
        }

        private static Provider _instance = null;
        public static Provider Instance
        {
            get { return _instance ?? (_instance = new Provider()); }
        }

        public static bool CanTranslate
        {
            get { return MainApplication.IsManagerReady; }
        }

        public string Translate(string key)
        {
            string str =  Strings.ResourceManager.GetString(key);
            if (string.IsNullOrEmpty(str))
                return key;
            else
                return str;
        }

        public string Check()
        {
            List<MissingAsset> assets = new List<MissingAsset>();

            Terrains.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Ressources.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Features.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Units.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Eras.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Techs.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Civilizations.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Improvements.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Policies.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            PolicyTypes.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Buildings.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            CultureLevels.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            MapSizes.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            MapTypes.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Difficulties.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));

            Flavours.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Leaders.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            LeaderAbilities.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            MilitayStrategies.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            CitySpecializations.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            Promotions.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            // Paces.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));
            //Maps.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));

            //Atlases.Values.ToList().ForEach(a => assets.AddRange(a.CheckIntegrity()));

            return assets.Aggregate("", (current, ass) => current + (ass + "\n"));
        }

        public static Era GetEra(string name)
        {
            KeyValuePair<string,Era> pair = Instance.Eras.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static GrandStrategyData GetGrandStrategy(string name)
        {
            KeyValuePair<string, GrandStrategyData> pair = Instance.GrandStrategies.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static PolicyType GetPolicyType(string name)
        {
            KeyValuePair<string, PolicyType> pair = Instance.PolicyTypes.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Policy GetPolicy(string name)
        {
            KeyValuePair<string, Policy> pair = Instance.Policies.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Tech GetTech(string name)
        {
            KeyValuePair<string, Tech> pair = Instance.Techs.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Ressource GetRessource(string name)
        {
            KeyValuePair<string, Ressource> pair = Instance.Ressources.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Civilization GetCivilization(string name)
        {
            KeyValuePair<string, Civilization> pair = Instance.Civilizations.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Difficulty GetHandicap(string name)
        {
            KeyValuePair<string, Difficulty> pair = Instance.Difficulties.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static UnitData GetUnit(string name)
        {
            KeyValuePair<string, UnitData> pair = Instance.Units.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Improvement GetImprovement(string name)
        {
            KeyValuePair<string, Improvement> pair = Instance.Improvements.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static Building GetBuilding(string name)
        {
            KeyValuePair<string, Building> pair = Instance.Buildings.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static CitySpecialization GetCitySpecialization(string name)
        {
            KeyValuePair<string, CitySpecialization> pair = Instance.CitySpecializations.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }

        public static CitySpecialization GetCitySpecializationDefault()
        {
            KeyValuePair<string, CitySpecialization> pair = Instance.CitySpecializations.FirstOrDefault(a => a.Value.IsDefault);

            if (pair.Value != null)
                return pair.Value;

            throw new Exception("There must be at least one CitySpecialization which is default! Found none!");
        }

        public static TextureAtlas GetAtlas(string name )
        {
            KeyValuePair<string, TextureAtlas> pair = Instance.Atlases.FirstOrDefault(a => a.Key == name);

            if (pair.Value != null)
                return pair.Value;

            return null;
        }
    }
}
