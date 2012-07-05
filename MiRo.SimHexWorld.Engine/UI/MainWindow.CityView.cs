using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.UI.Controls;
using MiRo.SimHexWorld.Engine.UI.Dialogs;
using MiRo.SimHexWorld.Engine.Types;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.Locales;

namespace MiRo.SimHexWorld.Engine.UI
{
    partial class MainWindow
    {
        public City CurrentCity { get; set; }

        Texture2D _cityProductionBackTexture;
        Texture2D _cityProductionFrameTexture;
        Texture2D _cityProductionMeterTexture;

        SpriteFont _cityTitleFont, _cityMenu;

        Rectangle cityOwnerIconLocation = new Rectangle( 0, 0, 42, 42);

        private void InitCityControls()
        {
            _cityProductionBackTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//productionpanelback");
            _cityProductionFrameTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//productionpanelframe");
            _cityProductionMeterTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//productionpanelmeter");

            _cityTitleFont = Manager.Content.Load<SpriteFont>("Content//Fonts//ArialS");
            _cityMenu = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        public void LblBuildings_Draw(object sender, DrawEventArgs e)
        {
            //e.Renderer.Draw(_citySeperatorTexture, e.Rectangle, new Rectangle(0,0,44,44), Color.White);

            if (GetControl("BuildingsList").Visible)
                e.Renderer.DrawString(_cityMenu, Strings.TXT_KEY_UI_CITYVIEW_BUILDINGS_COLLAPSE, e.Rectangle, Color.White, Alignment.MiddleLeft);
            else
                e.Renderer.DrawString(_cityMenu, Strings.TXT_KEY_UI_CITYVIEW_BUILDINGS_EXPAND, e.Rectangle, Color.White, Alignment.MiddleLeft);
        }

        public void LblCitynameTribe_Draw(object sender, DrawEventArgs e)
        {
            cityOwnerIconLocation.X = e.Rectangle.X + 106;
            cityOwnerIconLocation.Y = e.Rectangle.Y + 10;
            e.Renderer.Draw(CurrentCity.Player.Civilization.Image, cityOwnerIconLocation, Color.White);
        }

        public void BtnCityExit_Draw(object sender, DrawEventArgs e)
        {
            e.Renderer.DrawString(_cityTitleFont, Strings.TXT_KEY_UI_CITYVIEW_RETURN_MAP, e.Rectangle, Color.White, Alignment.MiddleCenter);
        }

        public void LblCityname_Draw(object sender, DrawEventArgs e)
        {
            e.Rectangle.Height = 36;
            e.Renderer.DrawString(_cityTitleFont, CurrentCity.Name, e.Rectangle, Color.White, Alignment.MiddleCenter);
        }

        public void LblProductionMeter_Draw(object sender, DrawEventArgs e)
        {
            Texture2D _cityProductionMeterModTexture = new Texture2D(Manager.GraphicsDevice,256,256);

            if (CurrentCity.CurrentBuildingTarget != null)
            {
                float radReady = CurrentCity.ProductionReady * (float)Math.PI * 2f - (float)Math.PI;

                // remove not present slice
                Color[] colors = new Color[256 * 256];
                _cityProductionMeterTexture.GetData<Color>(colors);

                for (int x = 0; x < 256; x++)
                {
                    for (int y = 0; y < 256; ++y)
                    {
                        int i = y * 256 + x;

                        if (Math.Atan2(x - 128, y - 128) < radReady)
                            colors[i] = Microsoft.Xna.Framework.Color.Transparent;
                    }
                }

                _cityProductionMeterModTexture.SetData<Color>(colors);
            }

            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 256, 256);

            e.Renderer.Draw(_cityProductionMeterModTexture, r, Color.White);
            e.Renderer.Draw(_cityProductionFrameTexture, r, Color.White);

            if (CurrentCity.CurrentBuildingTarget != null && CurrentCity.CurrentBuildingTarget.Image != null)
                e.Renderer.Draw(CurrentCity.CurrentBuildingTarget.Image, r, Color.White);
        }

        public void CitySidebar_Draw(object sender, DrawEventArgs e)
        {
            e.Renderer.Draw(IconProvider.Pixel, e.Rectangle, Microsoft.Xna.Framework.Color.Black);

            Rectangle r2 = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 11, e.Rectangle.Height);
            e.Renderer.Draw(_overviewSideTextureLeft, r2, Color.White);
        }
    }
}
