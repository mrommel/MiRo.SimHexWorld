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
        static Texture2D _pixel;
        public static Texture2D Pixel
        {
            get
            {
                if (_pixel == null)
                    _pixel = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\UI\\pixel");

                return _pixel;
            }
        }

        static Texture2D _exitIcon;
        public static Texture2D ExitIcon
        {
            get
            {
                if (_exitIcon == null)
                    _exitIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\UI\\exit");

                return _exitIcon;
            }
        }

        static Texture2D _scienceIcon;
        public static Texture2D ScienceIcon
        {
            get
            {
                if (_scienceIcon == null)
                    _scienceIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\research");

                return _scienceIcon;
            }
        }

        static Texture2D _goldIcon;
        public static Texture2D GoldIcon
        {
            get
            {
                if (_goldIcon == null)
                    _goldIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\gold");

                return _goldIcon;
            }
        }

        static Texture2D _happinessIcon;
        public static Texture2D HappinessIcon
        {
            get
            {
                if (_happinessIcon == null)
                    _happinessIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\happiness");

                return _happinessIcon;
            }
        }

        static Texture2D _greatPeopleIcon;
        public static Texture2D GreatPeopleIcon
        {
            get
            {
                if (_greatPeopleIcon == null)
                    _greatPeopleIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\greatpeople");

                return _greatPeopleIcon;
            }
        }

        static Texture2D _cultureIcon;
        public static Texture2D CultureIcon
        {
            get
            {
                if (_cultureIcon == null)
                    _cultureIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\culture");

                return _cultureIcon;
            }
        }

        static Texture2D _foodIcon;
        public static Texture2D FoodIcon
        {
            get
            {
                if (_foodIcon == null)
                    _foodIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\food");

                return _foodIcon;
            }
        }

        static Texture2D _productionIcon;
        public static Texture2D ProductionIcon
        {
            get
            {
                if (_productionIcon == null)
                    _productionIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Goods\\production");

                return _productionIcon;
            }
        }

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
                {
                    //_defaultTerrainIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\Terrains\\Default");
                    _defaultTerrainIcon = Provider.GetAtlas("TerrainAtlas").GetDefaultTexture();
                }

                return _defaultTerrainIcon;
            }
        }

        static Texture2D _applicationIcon;
        public static Texture2D ApplicationIcon
        {
            get
            {
                if( _applicationIcon == null )
                    _applicationIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\UI\\Globe_Icon_Small");

                return _applicationIcon;
            }
        }

        static Texture2D _zoomInIcon;
        public static Texture2D ZoomInIcon
        {
            get
            {
                if (_zoomInIcon == null)
                    _zoomInIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\UI\\plus");

                return _zoomInIcon;
            }
        }

        static Texture2D _zoomOutIcon;
        public static Texture2D ZoomOutIcon
        {
            get
            {
                if (_zoomOutIcon == null)
                    _zoomOutIcon = MainApplication.ManagerInstance.Content.Load<Texture2D>("Content\\Textures\\UI\\minus");

                return _zoomOutIcon;
            }
        }
    }
}
