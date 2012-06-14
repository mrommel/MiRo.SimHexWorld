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
        City _currentCity = null;

        // city view
        SideBar _citySidebar;
        Label _lblCitizen;
        ImageBox _lblBuildings;
        ImageListBox _lstBuildings;
        MenuItem _miBuildingDelete;
        Label _lblCurrentBuilding;
        ImageBox _btnCityExit;

        ImageBox _lblCitynameTribe;
        ImageBox _lblCityname;

        ImageBox _lblProductionMeter;

        Texture2D _citySideBarTexture;

        Texture2D _cityProductionBackTexture;
        Texture2D _cityProductionFrameTexture;
        Texture2D _cityProductionMeterTexture;
        Texture2D _citySeperatorTexture;

        SpriteFont _cityTitleFont, _cityMenu;

        Rectangle cityOwnerIconLocation = new Rectangle( 0, 0, 42, 42);

        private void InitCityControls()
        {
            _citySideBarTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//sidebar");

            _cityProductionBackTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//productionpanelback");
            _cityProductionFrameTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//productionpanelframe");
            _cityProductionMeterTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//productionpanelmeter");

            _citySeperatorTexture = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//frametopbottom44");

            _cityTitleFont = Manager.Content.Load<SpriteFont>("Content//Fonts//ArialS");
            _cityMenu = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");

            _lblCitynameTribe = new ImageBox(Manager);
            _lblCitynameTribe.Init();
            _lblCitynameTribe.Visible = false;
            _lblCitynameTribe.Anchor = Anchors.All;
            _lblCitynameTribe.Top = 20;
            _lblCitynameTribe.Left = ( Manager.GraphicsDevice.Viewport.Width - 256 ) / 2;
            _lblCitynameTribe.Width = 256;
            _lblCitynameTribe.Height = 64;
            _lblCitynameTribe.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//tribeHeading");
            _lblCitynameTribe.Draw += new DrawEventHandler(LblCitynameTribe_Draw);
            Add(_lblCitynameTribe);

            _lblCityname = new ImageBox(Manager);
            _lblCityname.Init();
            _lblCityname.Visible = false;
            _lblCityname.Anchor = Anchors.All;
            _lblCityname.Top = 20 + _lblCitynameTribe.Height;
            _lblCityname.Left = ( Manager.GraphicsDevice.Viewport.Width - 464 ) / 2;
            _lblCityname.Width = 464;
            _lblCityname.Height = 36;
            _lblCityname.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//title");
            _lblCityname.Draw += new DrawEventHandler(LblCityname_Draw);
            Add(_lblCityname);

            _citySidebar = new SideBar(Manager);
            _citySidebar.Init();
            _citySidebar.StayOnBack = true;
            _citySidebar.Passive = true;
            _citySidebar.Left = Manager.GraphicsDevice.Viewport.Width - 260;
            _citySidebar.Top = _topBar.Height;
            _citySidebar.Width = 230;
            _citySidebar.Height = Manager.GraphicsDevice.Viewport.Height - _topBar.Height;
            _citySidebar.Anchor = Anchors.Right | Anchors.Top | Anchors.Bottom;
            _citySidebar.Visible = false;
            _citySidebar.Draw += new DrawEventHandler(CitySidebar_Draw);
            Add(_citySidebar);

            _lblCitizen = new Label(Manager);
            _lblCitizen.Init();
            _lblCitizen.Passive = false;
            _lblCitizen.Parent = _citySidebar;
            _lblCitizen.Top = 8;
            _lblCitizen.Left = 12;
            _lblCitizen.Width = _citySidebar.Width - 16 - 8;
            _lblCitizen.Height = 16;
            _lblCitizen.Text = "Citizen: 1";

            _lblBuildings = new ImageBox(Manager);
            _lblBuildings.Init();
            _lblBuildings.Passive = false;
            _lblBuildings.Parent = _citySidebar;
            _lblBuildings.Top = _lblCitizen.Top + _lblCitizen.Height + 8;
            _lblBuildings.Left = 10;
            _lblBuildings.Width = _citySidebar.Width - 16;
            _lblBuildings.Height = 22;
            _lblBuildings.Image = _citySeperatorTexture;
            _lblBuildings.SizeMode = SizeMode.Stretched;
            _lblBuildings.Draw += new DrawEventHandler(LblBuildings_Draw);
            _lblBuildings.Click += new TomShane.Neoforce.Controls.EventHandler(_lblBuildings_Click);

            ContextMenu ctxBuildings = new ContextMenu(Manager);
            _miBuildingDelete = new MenuItem("Delete");
            _miBuildingDelete.Click += new TomShane.Neoforce.Controls.EventHandler(ctxBuildingDelete_Click);
            _miBuildingDelete.Enabled = false;
            ctxBuildings.Items.Add(_miBuildingDelete);

            _lstBuildings = new ImageListBox(Manager);
            _lstBuildings.Init();
            _lstBuildings.Parent = _citySidebar;
            _lstBuildings.Top = _lblBuildings.Top + _lblBuildings.Height + 8;
            _lstBuildings.Left = 10;
            _lstBuildings.Width = _citySidebar.Width - 24;
            _lstBuildings.Height = 200;
            _lstBuildings.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            _lstBuildings.HideSelection = false;
            _lstBuildings.ItemIndexChanged += new TomShane.Neoforce.Controls.EventHandler(_lstBuildings_ItemIndexChanged);
            _lstBuildings.ContextMenu = ctxBuildings;

            // left
            _lblCurrentBuilding = new Label(Manager);
            _lblCurrentBuilding.Visible = false;
            _lblCurrentBuilding.Anchor = Anchors.All;
            _lblCurrentBuilding.Top = Manager.GraphicsDevice.Viewport.Height - 256 - 22 - 24;
            _lblCurrentBuilding.Left = 0;
            _lblCurrentBuilding.Width = 256;
            _lblCurrentBuilding.Height = 24;
            Add(_lblCurrentBuilding);

            _lblProductionMeter = new ImageBox(Manager);
            _lblProductionMeter.Init();
            _lblProductionMeter.Visible = false;
            _lblProductionMeter.Anchor = Anchors.All;
            _lblProductionMeter.Top = Manager.GraphicsDevice.Viewport.Height - 256 - 70;
            _lblProductionMeter.Left = 0;
            _lblProductionMeter.Image = _cityProductionBackTexture;
            _lblProductionMeter.Width = 256;
            _lblProductionMeter.Height = 256;
            _lblProductionMeter.SizeMode = SizeMode.Normal;
            _lblProductionMeter.Draw += new DrawEventHandler(LblProductionMeter_Draw);
            Add(_lblProductionMeter);

            _btnCityExit = new ImageBox(Manager);
            _btnCityExit.Init();
            _btnCityExit.Width = 250;
            _btnCityExit.Height = 44;
            _btnCityExit.Visible = false;
            _btnCityExit.Left = ( Manager.GraphicsDevice.Viewport.Width - _btnCityExit.Width ) / 2;
            _btnCityExit.Top = Manager.GraphicsDevice.Viewport.Height - 120;
            _btnCityExit.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//CityView//grid9blueframe");
            _btnCityExit.SizeMode = SizeMode.Stretched;
            _btnCityExit.Draw += new DrawEventHandler(BtnCityExit_Draw);
            _btnCityExit.Click += new TomShane.Neoforce.Controls.EventHandler(_btnCityExit_Click);
            Add(_btnCityExit);
        }

        void LblBuildings_Draw(object sender, DrawEventArgs e)
        {
            //e.Renderer.Draw(_citySeperatorTexture, e.Rectangle, new Rectangle(0,0,44,44), Color.White);

            if( _lstBuildings.Visible )
                e.Renderer.DrawString(_cityMenu, Strings.TXT_KEY_UI_CITYVIEW_BUILDINGS_COLLAPSE, e.Rectangle, Color.White, Alignment.MiddleLeft);
            else
                e.Renderer.DrawString(_cityMenu, Strings.TXT_KEY_UI_CITYVIEW_BUILDINGS_EXPAND, e.Rectangle, Color.White, Alignment.MiddleLeft);
        }

        void LblCitynameTribe_Draw(object sender, DrawEventArgs e)
        {
            cityOwnerIconLocation.X = e.Rectangle.X + 106;
            cityOwnerIconLocation.Y = e.Rectangle.Y + 10;
            e.Renderer.Draw(_currentCity.Player.Civilization.Image, cityOwnerIconLocation, Color.White);
        }

        void BtnCityExit_Draw(object sender, DrawEventArgs e)
        {
            e.Renderer.DrawString(_cityTitleFont, Strings.TXT_KEY_UI_CITYVIEW_RETURN_MAP, e.Rectangle, Color.White, Alignment.MiddleCenter);
        }

        void LblCityname_Draw(object sender, DrawEventArgs e)
        {
            e.Rectangle.Height = 36;
            e.Renderer.DrawString(_cityTitleFont, _currentCity.Name, e.Rectangle, Color.White, Alignment.MiddleCenter);
        }

        void LblProductionMeter_Draw(object sender, DrawEventArgs e)
        {
            Texture2D _cityProductionMeterModTexture = new Texture2D(Manager.GraphicsDevice,256,256);

            if (_currentCity.CurrentBuildingTarget != null)
            {
                float radReady = _currentCity.ProductionReady * (float)Math.PI * 2f - (float)Math.PI;

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

            if (_currentCity.CurrentBuildingTarget != null && _currentCity.CurrentBuildingTarget.Image != null)
                e.Renderer.Draw(_currentCity.CurrentBuildingTarget.Image, r, Color.White);
        }

        public void UpdateCityControls()
        {
            if (_currentCity == null)
                return;

            _lblCitizen.Text = "Citizen: " + _currentCity.Citizen + " (" + _currentCity.Population + ")";

            if (_currentCity.CurrentBuildingTarget != null)
                _lblCurrentBuilding.Text = _currentCity.CurrentBuildingTarget.Title + "(" + (int)(_currentCity.ProductionReady * 100) + "%)";
            else
                _lblCurrentBuilding.Text = "";

            _lstBuildings.Items.Clear();
            foreach (Building building in _currentCity.Buildings)
                _lstBuildings.Items.Add(building);
        }

        private void ShowCityControls(bool show)
        {
            _lblCurrentBuilding.Visible = show;
            _btnCityExit.Visible = show;
            _citySidebar.Visible = show;
            _lblCityname.Visible = show;
            _lblCitynameTribe.Visible = show;
            _lblProductionMeter.Visible = show;
        }

        void MapBox_CityOpened(City city)
        {
            _currentCity = city;
            
            if (_view == MapView.Main)
                ToogleView();

            UpdateCityControls();
        }

        void MapBox_CitySelected(City city)
        {
        }

        void CitySidebar_Draw(object sender, DrawEventArgs e)
        {
            e.Renderer.Draw(IconProvider.Pixel, e.Rectangle, Microsoft.Xna.Framework.Color.Black);

            Rectangle r2 = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 11, e.Rectangle.Height);
            e.Renderer.Draw(_overviewSideTextureLeft, r2, Color.White);
        }

        void _lstBuildings_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            _miBuildingDelete.Enabled = _lstBuildings.ItemIndex != -1;
        }

        enum YesNoChoises { Yes, No };
        Building _buildingToDemolish;
        void ctxBuildingDelete_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            string building = _lstBuildings.Items[_lstBuildings.ItemIndex].ToString();
            _buildingToDemolish = _lstBuildings.Items[_lstBuildings.ItemIndex] as Building;
            ChooseEnumDialog ced = new ChooseEnumDialog();
            ced.ShowChooseEnumDialog(Manager, string.Format("Really delete {0}?", building), typeof(YesNoChoises), ctxBuildings_Delete_Click);
        }

        void ctxBuildings_Delete_Click(object sender, EnumChooseEventArgs e)
        {
            if ((YesNoChoises)e.SelectedEnum == YesNoChoises.Yes)
            {
                _currentCity.Demolish(_buildingToDemolish);
            }
        }

        void _lblBuildings_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //_lstBuildings.Height = _lstBuildings.Height == 0 ? 200 : 0;
            _lstBuildings.Visible = !_lstBuildings.Visible;

            UpdateCityControlsHeight();
        }

        private void UpdateCityControlsHeight()
        {
            
        }
    }
}
