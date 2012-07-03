using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.UI
{
    public class IconProvider
    {
        private static Dictionary<string, Texture2D> _buffer = new Dictionary<string, Texture2D>();

        private static Dictionary<string, string> _assets = new Dictionary<string, string>();

        private IconProvider()
        {           
        }

        private static void Init()
        {
            if (_assets.Count == 0)
            {
                _assets.Add("Pixel", "Content\\Textures\\UI\\pixel");
                _assets.Add("Exit", "Content\\Textures\\UI\\exit");
                _assets.Add("ScienceIcon", "Content\\Textures\\Goods\\research");
                _assets.Add("Gold", "Content\\Textures\\Goods\\gold");
                _assets.Add("Happiness", "Content\\Textures\\Goods\\happiness");

                _assets.Add("GreatPeople", "Content\\Textures\\Goods\\greatpeople");
                _assets.Add("Culture", "Content\\Textures\\Goods\\culture");
                _assets.Add("Food", "Content\\Textures\\Goods\\food");
                _assets.Add("Production", "Content\\Textures\\Goods\\production");
                _assets.Add("Application", "Content\\Textures\\UI\\Globe_Icon_Small");

                _assets.Add("ZoomIn", "Content\\Textures\\UI\\plus");
                _assets.Add("ZoomOut", "Content\\Textures\\UI\\minus");
                _assets.Add("Capital", "Content\\Textures\\Goods\\capital");
            }
        }

        private static Texture2D FetchOrLoad(string assetName)
        {
            Init();

            if (!_buffer.ContainsKey(assetName))
                _buffer.Add(assetName, MainApplication.ManagerInstance.Content.Load<Texture2D>(_assets[assetName]));

            return _buffer[assetName];
        }

        public static Texture2D Pixel { get { return FetchOrLoad("Pixel"); } }
        public static Texture2D ExitIcon { get { return FetchOrLoad("Exit"); } }
        public static Texture2D ScienceIcon { get { return FetchOrLoad("ScienceIcon"); } }
        public static Texture2D GoldIcon { get { return FetchOrLoad("Gold"); } }
        public static Texture2D HappinessIcon { get { return FetchOrLoad("Happiness"); } }
        public static Texture2D GreatPeopleIcon { get { return FetchOrLoad("GreatPeople"); } }
        public static Texture2D CultureIcon { get { return FetchOrLoad("Culture"); } }
        public static Texture2D FoodIcon { get { return FetchOrLoad("Food"); } }
        public static Texture2D ProductionIcon { get { return FetchOrLoad("Production"); } }

        static Dictionary<string,Texture2D> _localeIcons = new Dictionary<string,Texture2D>();
        public static Texture2D GetLocaleIcon(string locale)
        {
            if (!_localeIcons.ContainsKey(locale))
                _localeIcons.Add(locale, MainApplication.ManagerInstance.Content.LoadLocalizedAsset<Texture2D>("Content\\Textures\\Locales\\flag"));

            return _localeIcons[locale];           
        }

        static Texture2D _defaultTerrainIcon;
        public static Texture2D DefaultTerrainIcon
        {
            get
            {
                if (_defaultTerrainIcon == null)
                    _defaultTerrainIcon = Provider.GetAtlas("TerrainAtlas").GetDefaultTexture();

                return _defaultTerrainIcon;
            }
        }

        public static Texture2D ApplicationIcon { get { return FetchOrLoad("Application"); } }
        public static Texture2D ZoomInIcon { get { return FetchOrLoad("ZoomIn"); } }
        public static Texture2D ZoomOutIcon { get { return FetchOrLoad("ZoomOut"); } }
        public static Texture2D CapitalIcon { get { return FetchOrLoad("Capital"); } }

        public static Texture2D GetByName(string name)
        {
            return FetchOrLoad(name);
        }
    }
}
