using System.Linq;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Game;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.UI.Controls;
using PureMVC.Interfaces;
using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.UI.Dialogs;
using Microsoft.Xna.Framework;
using System;
using MiRo.SimHexWorld.Engine.World.Entities;
using System.Text;
using MiRo.SimHexWorld.Engine.Locales;
using System.Globalization;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.UI
{
    public partial class MainWindow : GameWindow
    {
        enum MapView { Main, City }

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////       
        private readonly Texture2D _defaultbg;

        FpsCounter fpsCounter;
        SpriteFont _messageFont;

        GameMapBox _mapBox;

        MapView _view = MapView.Main;

        SideBar _topBar;
        ImageBox _imgScience;
        Label _lblScience;
        ImageBox _imgGold;
        Label _lblGold;
        ImageBox _imgHappiness;
        Label _lblHappiness;
        ImageBox _imgGreatPeople;
        Label _lblGreatPeople;
        ImageBox _imgCulture;
        Label _lblCulture;

        Label _lblCurrentTurn;
        Button _btnTurn;

        ImageBox _imgLocale;

        SideBar _sidebar;
        SideBarPanel _pnlRes;
        Button _btnCreate;
        Button _btnLoad;
        Button _btnCheck;
        Button _btnExit;

        ImageBox _lblTerrainIcon;
        Label _lblPosition;
        Label _lblTerrainName;
        Label _lblFeatures;
        Label _lblResource;

        ImageBox _imgFood, _imgCommercial, _imgProduction;
        Label _lblFood, _lblCommercial, _lblProduction;

        Label _lblUnitName, _lblRegion;

        // unit
        ImageBox _lblUnit;

        // zoom
        ImageBox _imgZoomIn, _imgZoomOut;

        readonly GameStartupSettings _startupSettings = new GameStartupSettings();

        private static readonly GameData _game = new GameData();

        ////////////////////////////////////////////////////////////////////////////       

        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////
        public MainWindow(Manager manager)
            : base(manager)
        {
            // Tell the resource manager what language to use when loading strings.
            Strings.Culture = CultureInfo.CurrentCulture;

            _defaultbg = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\bg_body3");

            var application = Manager.Game as Application;
            if (application != null) 
                application.BackgroundImage = _defaultbg;

            Icon = IconProvider.ApplicationIcon;

            _messageFont = Manager.Content.Load<SpriteFont>("Content\\Fonts\\ArialL");

            InitMainControls();
            InitCityControls();
            InitMessages();
            InitUnitControls();
            InitScienceControls();
            InitOverviewControls();

            Manager.SetSkin("Default");

            _startupSettings.MapSize = "Tiny";
            _startupSettings.MapType = "Pangaea";

            fpsCounter = new FpsCounter();

            Text = Strings.Title;
            CaptionVisible = true;
            BorderVisible = true;

            Manager.Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PrepareGraphicsDevice); 

            // debugging
            //foreach (TextureAtlas at in Provider.Instance.Atlases.Values)
            //{
            //    at.SaveAsPNG(at.Name + ".png");
            //}

            //this.Draw += new DrawEventHandler(MainWindow_Draw);
        }

        protected void PrepareGraphicsDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        void _btnCityExit_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ToogleView();
        }

        private void InitMainControls()
        {
            ////////////////////////////////////////////////
            _mapBox = new GameMapBox(Manager);
            _mapBox.Init();
            _mapBox.StayOnBack = false;
            _mapBox.Anchor = Anchors.All;
            _mapBox.Top = 22;
            _mapBox.Left = 2;
            _mapBox.Width = Manager.GraphicsDevice.Viewport.Width - 4;
            _mapBox.Height = Manager.GraphicsDevice.Viewport.Height - 32;
            _mapBox.FocusChanged += MapBox_FocusChanged;
            _mapBox.CityOpened += MapBox_CityOpened;
            _mapBox.CitySelected += MapBox_CitySelected;
            _mapBox.HumanUnitsSelected += MapBox_HumanUnitsSelected;
            _mapBox.UnitsUnselected += new UnselectHandler(MapBox_UnitsUnselected);
            Add(_mapBox);

            // top ////////////////////////////////////////////////

            _topBar = new SideBar(Manager);
            _topBar.Init();
            _topBar.StayOnBack = true;
            _topBar.Passive = true;
            _topBar.Width = Manager.GraphicsDevice.Viewport.Width;
            _topBar.Height = 20;
            _topBar.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            Add(_topBar);

            _imgScience = new ImageBox(Manager);
            _imgScience.Init();
            _imgScience.Image = IconProvider.ScienceIcon;
            _imgScience.Parent = _topBar;
            _imgScience.Left = 5;
            _imgScience.Width = 20;

            _lblScience = new Label(Manager);
            _lblScience.Init();
            _lblScience.Text = "+0";
            _lblScience.Left = 5 + _imgScience.Width + 5;
            _lblScience.Width = 40;
            _lblScience.Top = 2;
            _lblScience.Parent = _topBar;

            _imgGold = new ImageBox(Manager);
            _imgGold.Init();
            _imgGold.Image = IconProvider.GoldIcon;
            _imgGold.Parent = _topBar;
            _imgGold.Left = 5 + _imgScience.Width + 5 + _lblScience.Width;
            _imgGold.Width = 20;

            _lblGold = new Label(Manager);
            _lblGold.Init();
            _lblGold.Text = "100 (+0)";
            _lblGold.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width;
            _lblGold.Width = 70;
            _lblGold.Top = 2;
            _lblGold.Parent = _topBar;

            _imgHappiness = new ImageBox(Manager);
            _imgHappiness.Init();
            _imgHappiness.Image = IconProvider.HappinessIcon;
            _imgHappiness.Parent = _topBar;
            _imgHappiness.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width + 5 + _lblGold.Width;
            _imgHappiness.Width = 20;
            _imgHappiness.DoubleClick += lblHappiness_Click;

            _lblHappiness = new Label(Manager);
            _lblHappiness.Init();
            _lblHappiness.Text = "0";
            _lblHappiness.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width + 5 + _lblGold.Width + 5 + _imgHappiness.Width;
            _lblHappiness.Width = 20;
            _lblHappiness.Top = 2;
            _lblHappiness.Parent = _topBar;
            _lblHappiness.DoubleClick += lblHappiness_Click;

            _imgGreatPeople = new ImageBox(Manager);
            _imgGreatPeople.Init();
            _imgGreatPeople.Image = IconProvider.GreatPeopleIcon;
            _imgGreatPeople.Parent = _topBar;
            _imgGreatPeople.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width + 5 + _lblGold.Width + 5 + _imgHappiness.Width + 5 + _lblHappiness.Width;
            _imgGreatPeople.Width = 20;

            _lblGreatPeople = new Label(Manager);
            _lblGreatPeople.Init();
            _lblGreatPeople.Text = "0/0";
            _lblGreatPeople.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width + 5 + _lblGold.Width + 5 + _imgHappiness.Width + 5 + _lblHappiness.Width + 5 + _imgGreatPeople.Width;
            _lblGreatPeople.Width = 50;
            _lblGreatPeople.Top = 2;
            _lblGreatPeople.Parent = _topBar;

            _imgCulture = new ImageBox(Manager);
            _imgCulture.Init();
            _imgCulture.Image = IconProvider.CultureIcon;
            _imgCulture.Parent = _topBar;
            _imgCulture.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width + 5 + _lblGold.Width + 5 + _imgHappiness.Width + 5 + _lblHappiness.Width + 5 + _imgGreatPeople.Width + 5 + _lblGreatPeople.Width;
            _imgCulture.Width = 20;

            _lblCulture = new Label(Manager);
            _lblCulture.Init();
            _lblCulture.Text = "0/0 (+0)";
            _lblCulture.Left = 5 + _imgScience.Width + 5 + _lblScience.Width + 5 + _imgGold.Width + 5 + _lblGold.Width + 5 + _imgHappiness.Width + 5 + _lblHappiness.Width + 5 + _imgGreatPeople.Width + 5 + _lblGreatPeople.Width + 5 + _imgCulture.Width;
            _lblCulture.Width = 70;
            _lblCulture.Top = 2;
            _lblCulture.Parent = _topBar;

            // from the right ///////////////////////////////////////////////

            _imgLocale = new ImageBox(Manager);
            _imgLocale.Init();
            _imgLocale.Anchor = Anchors.Right;
            _imgLocale.Image = IconProvider.GetLocaleIcon("de-de");
            _imgLocale.Parent = _topBar;
            _imgLocale.Left = Manager.GraphicsDevice.Viewport.Width - 25;
            _imgLocale.Width = 20;
            _imgLocale.DoubleClick += ImgLocaleDoubleClick;

            Label lblSpacer1 = new Label(Manager);
            lblSpacer1.Init();
            lblSpacer1.Text = "|";
            lblSpacer1.Anchor = Anchors.Right;
            lblSpacer1.Parent = _topBar;
            lblSpacer1.Width = 5;
            lblSpacer1.Left = Manager.GraphicsDevice.Viewport.Width - lblSpacer1.Width - 5 - _imgLocale.Width - 5;

            _btnTurn = new Button(Manager);
            _btnTurn.Init();
            _btnTurn.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_TURN;
            _btnTurn.Anchor = Anchors.Right;
            _btnTurn.Parent = _topBar;
            _btnTurn.Width = 45;
            _btnTurn.Left = Manager.GraphicsDevice.Viewport.Width - _btnTurn.Width - 5 - lblSpacer1.Width - 5 - _imgLocale.Width - 5;
            _btnTurn.Click += LblTurnClick;

            Label lblSpacer2 = new Label(Manager);
            lblSpacer2.Init();
            lblSpacer2.Text = "|";
            lblSpacer2.Anchor = Anchors.Right;
            lblSpacer2.Parent = _topBar;
            lblSpacer2.Width = 5;
            lblSpacer2.Left = Manager.GraphicsDevice.Viewport.Width - lblSpacer2.Width - 5 - _btnTurn.Width - 5 - lblSpacer1.Width - 5 - _imgLocale.Width - 5;

            _lblCurrentTurn = new Label(Manager);
            _lblCurrentTurn.Init();
            _lblCurrentTurn.Text = string.Format(Strings.TXT_KEY_UI_MAINAPPLICATION_TURN_PATTERN, _game.CurrentTurn, _game.Year);
            _lblCurrentTurn.Anchor = Anchors.Right;
            _lblCurrentTurn.Parent = _topBar;
            _lblCurrentTurn.Width = 110;
            _lblCurrentTurn.Alignment = Alignment.MiddleRight;
            _lblCurrentTurn.Left = Manager.GraphicsDevice.Viewport.Width - lblSpacer2.Width - 5 - _btnTurn.Width - 5 - lblSpacer1.Width - 5 - _lblCurrentTurn.Width - 5 - _imgLocale.Width - 5;       

            // side ////////////////////////////////////////////////

            

            ////////////////////////////////////////////////////////////////////////////////////////////

            _sidebar = new SideBar(Manager);
            _sidebar.Init();
            _sidebar.StayOnBack = true;
            _sidebar.Passive = true;
            _sidebar.Top = _topBar.Height;
            _sidebar.Width = 200;
            _sidebar.Height = Manager.GraphicsDevice.Viewport.Height - _topBar.Height;
            _sidebar.Anchor = Anchors.Left | Anchors.Top | Anchors.Bottom;
            _sidebar.Visible = false;
            Add(_sidebar);

            _pnlRes = new SideBarPanel(Manager);
            _pnlRes.Init();
            _pnlRes.Passive = true;
            _pnlRes.Parent = _sidebar;
            _pnlRes.Left = 16;
            _pnlRes.Top = 16;
            _pnlRes.Width = _sidebar.Width - _pnlRes.Left * 2;
            _pnlRes.Height = 150;
            _pnlRes.CanFocus = false;
            _pnlRes.Draw += DrawOverview;

            _btnCreate = new Button(Manager);
            _btnCreate.Init();
            _btnCreate.Width = 80;
            _btnCreate.Parent = _sidebar;
            _btnCreate.Left = _pnlRes.Left;
            _btnCreate.Top = _pnlRes.Top + _pnlRes.Height + 8;
            _btnCreate.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_START; // "Start";
            _btnCreate.Click += BtnCreateClick;

            _btnExit = new Button(Manager);
            _btnExit.Init();
            _btnExit.Width = 80;
            _btnExit.Parent = _sidebar;
            _btnExit.Left = _btnCreate.Left + _btnCreate.Width + 8;
            _btnExit.Top = _pnlRes.Top + _pnlRes.Height + 8;
            _btnExit.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_EXIT;  // "Exit";
            _btnExit.Click += BtnExitClick;

            _btnCheck = new Button(Manager);
            _btnCheck.Init();
            _btnCheck.Width = 80;
            _btnCheck.Parent = _sidebar;
            _btnCheck.Left = _pnlRes.Left;
            _btnCheck.Top = _pnlRes.Top + _pnlRes.Height + 8 + _btnCreate.Height + 8;
            _btnCheck.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_CHECK; // "Check";
            _btnCheck.Click += BtnCheckClick;

            _btnLoad = new Button(Manager);
            _btnLoad.Init();
            _btnLoad.Width = 80;
            _btnLoad.Parent = _sidebar;
            _btnLoad.Left = _pnlRes.Left + _btnCreate.Width + 8;
            _btnLoad.Top = _pnlRes.Top + _pnlRes.Height + 8 + _btnCreate.Height + 8;
            _btnLoad.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_LOAD; // "Check";
            _btnLoad.Click += BtnLoadClick;

            _lblTerrainIcon = new ImageBox(Manager);
            _lblTerrainIcon.Init();
            _lblTerrainIcon.Image = IconProvider.DefaultTerrainIcon.GetThumbnail(64, 64);
            _lblTerrainIcon.Anchor = Anchors.Left | Anchors.Top;
            _lblTerrainIcon.Width = 64;
            _lblTerrainIcon.Height = 64;           
            _lblTerrainIcon.Left = _pnlRes.Left;
            _lblTerrainIcon.Top = _btnCheck.Top + _btnCheck.Height + 8;          
            _lblTerrainIcon.BackColor = Microsoft.Xna.Framework.Color.White;
            _lblTerrainIcon.Parent = _sidebar;

            _lblPosition = new Label(Manager);
            _lblPosition.Init();
            _lblPosition.Width = 90;
            _lblPosition.Parent = _sidebar;
            _lblPosition.Left = _pnlRes.Left + _lblTerrainIcon.Width + 16;
            _lblPosition.Top = _btnCheck.Top + _btnCheck.Height + 8;
            _lblPosition.Text = "Pos:";

            _lblTerrainName = new Label(Manager);
            _lblTerrainName.Init();
            _lblTerrainName.Width = 90;
            _lblTerrainName.Parent = _sidebar;
            _lblTerrainName.Left = _pnlRes.Left + _lblTerrainIcon.Width + 16;
            _lblTerrainName.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height;
            _lblTerrainName.Text = "None";

            _lblFeatures = new Label(Manager);
            _lblFeatures.Init();
            _lblFeatures.Width = 90;
            _lblFeatures.Parent = _sidebar;
            _lblFeatures.Left = _pnlRes.Left + _lblTerrainIcon.Width + 16;
            _lblFeatures.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height;
            _lblFeatures.Text = "Feat";

            _lblResource = new Label(Manager);
            _lblResource.Init();
            _lblResource.Width = 90;
            _lblResource.Parent = _sidebar;
            _lblResource.Left = _pnlRes.Left + _lblTerrainIcon.Width + 16;
            _lblResource.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height;
            _lblResource.Text = "Reso";

            // Food Commercial Production
            _imgFood = new ImageBox(Manager);
            _imgFood.Init();
            _imgFood.Image = IconProvider.FoodIcon.GetThumbnail(16, 16);
            _imgFood.Anchor = Anchors.Left | Anchors.Top;
            _imgFood.Width = 16;
            _imgFood.Height = 16;
            _imgFood.Left = _pnlRes.Left;
            _imgFood.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height;
            _imgFood.BackColor = Microsoft.Xna.Framework.Color.White;
            _imgFood.Parent = _sidebar;

            _lblFood = new Label(Manager);
            _lblFood.Init();
            _lblFood.Width = 12;
            _lblFood.Parent = _sidebar;
            _lblFood.Left = _pnlRes.Left + _imgFood.Width + 4;
            _lblFood.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height;
            _lblFood.Text = "0";

            _imgCommercial = new ImageBox(Manager);
            _imgCommercial.Init();
            _imgCommercial.Image = IconProvider.GoldIcon.GetThumbnail(16, 16);
            _imgCommercial.Anchor = Anchors.Left | Anchors.Top;
            _imgCommercial.Width = 16;
            _imgCommercial.Height = 16;
            _imgCommercial.Left = _pnlRes.Left + _imgFood.Width + 4 + _lblFood.Width;
            _imgCommercial.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height;
            _imgCommercial.BackColor = Microsoft.Xna.Framework.Color.White;
            _imgCommercial.Parent = _sidebar;

            _lblCommercial = new Label(Manager);
            _lblCommercial.Init();
            _lblCommercial.Width = 12;
            _lblCommercial.Parent = _sidebar;
            _lblCommercial.Left = _pnlRes.Left + _imgFood.Width + 4 + _lblFood.Width + _imgCommercial.Width + 4;
            _lblCommercial.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height;
            _lblCommercial.Text = "0";

            _imgProduction = new ImageBox(Manager);
            _imgProduction.Init();
            _imgProduction.Image = IconProvider.ProductionIcon.GetThumbnail(16, 16);
            _imgProduction.Anchor = Anchors.Left | Anchors.Top;
            _imgProduction.Width = 16;
            _imgProduction.Height = 16;
            _imgProduction.Left = _pnlRes.Left + _imgFood.Width + 4 + _lblFood.Width + _imgCommercial.Width + 4 + _lblCommercial.Width;
            _imgProduction.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height;
            _imgProduction.BackColor = Microsoft.Xna.Framework.Color.White;
            _imgProduction.Parent = _sidebar;

            _lblProduction = new Label(Manager);
            _lblProduction.Init();
            _lblProduction.Width = 12;
            _lblProduction.Parent = _sidebar;
            _lblProduction.Left = _pnlRes.Left + _imgFood.Width + 4 + _lblFood.Width + _imgCommercial.Width + 4 + _lblCommercial.Width + _imgProduction.Width + 4;
            _lblProduction.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height;
            _lblProduction.Text = "0";

            _lblUnitName = new Label(Manager);
            _lblUnitName.Init();
            _lblUnitName.Width = 150;
            _lblUnitName.Height = 100;
            _lblUnitName.Parent = _sidebar;
            _lblUnitName.Left = _pnlRes.Left;
            _lblUnitName.Top = _btnCheck.Top + _btnCheck.Height + 8 + _lblPosition.Height + 8 + _lblTerrainName.Height + 8 + _lblFeatures.Height + 8 + _lblResource.Height + 8 + _lblFood.Height;
            _lblUnitName.Text = "";

            _lblRegion = new Label(Manager);
            _lblRegion.Init();
            _lblRegion.Width = 150;
            _lblRegion.Height = 50;
            _lblRegion.Parent = _sidebar;
            _lblRegion.Left = _pnlRes.Left;
            _lblRegion.Top = _lblUnitName.Top + _lblUnitName.Height + 8;
            _lblRegion.Text = "";

            // /////////////////////
            // ui
            _imgZoomIn = new ImageBox(Manager);
            _imgZoomIn.Init();
            _imgZoomIn.Width = 50;
            _imgZoomIn.Left = Manager.GraphicsDevice.Viewport.Width - 90;
            _imgZoomIn.Top = Manager.GraphicsDevice.Viewport.Height - 120;
            _imgZoomIn.Image = IconProvider.ZoomInIcon.GetThumbnail(50,50);
            _imgZoomIn.Click += ImgZoomInClick;
            _imgZoomIn.ToolTip = new TomShane.Neoforce.Controls.ToolTip(Manager);
            _imgZoomIn.ToolTip.Text = "Zoom in";
            Add(_imgZoomIn);

            _imgZoomOut = new ImageBox(Manager);
            _imgZoomOut.Width = 50;
            _imgZoomOut.Left = Manager.GraphicsDevice.Viewport.Width - 90;
            _imgZoomOut.Top = Manager.GraphicsDevice.Viewport.Height - 180;
            _imgZoomOut.Image = IconProvider.ZoomOutIcon.GetThumbnail(50, 50);
            _imgZoomOut.Click += ImgZoomOutClick;
            _imgZoomOut.ToolTip = new TomShane.Neoforce.Controls.ToolTip(Manager);
            _imgZoomOut.ToolTip.Text = "Zoom out";
            Add(_imgZoomOut);

            Manager.Add(this);

            ShowMainDialog();
        }


        enum MainOptionChoises { New, Load, Check, Exit };
        private void ShowMainDialog()
        {
            ChooseEnumDialog ced = new ChooseEnumDialog();
            ced.ShowChooseEnumDialog(Manager, "What do you want?", typeof(MainOptionChoises), MainChoose_Click);
        }

        void MainChoose_Click(object sender, EnumChooseEventArgs e)
        {
            switch ((MainOptionChoises)e.SelectedEnum)
            {
                case MainOptionChoises.New:
                    GameFacade.getInstance().SendNotification(GameNotification.CreateMap, _startupSettings);
                    break;
                case MainOptionChoises.Load:
                    SelectMapLoadDialog tmp = new SelectMapLoadDialog(Manager);
                    tmp.Closed += SelectMapLoadDialogClosed;
                    tmp.Init();
                    Manager.Add(tmp);
                    tmp.ShowModal(); 
                    break;
                case MainOptionChoises.Check:
                    break;
                case MainOptionChoises.Exit:
                    Close();
                    break;
            }
        }

        

        void DrawOverview(object sender, DrawEventArgs e)
        {
        }

        TimeSpan lastFpsUpdate;
        protected override void Update(GameTime gameTime)
        {
            fpsCounter.Update(gameTime);

            base.Update(gameTime);

            if(_needToUpdateOverview)
            {
                UpdateOverviewControls();
                _needToUpdateOverview = false;
            }

            Game.Update(gameTime);

            // display fps
            lastFpsUpdate -= gameTime.ElapsedGameTime;
            if (lastFpsUpdate.TotalSeconds <= 0)
            {
                this.Text = Strings.Title + ": " + fpsCounter.FrameRate + " FPS, mem: " + String.Format(new FileSizeFormatProvider(), "{0:fs}", GC.GetTotalMemory(false));
                lastFpsUpdate = TimeSpan.FromSeconds(3);
            }

            if (_view == MapView.City)
                UpdateCityControls();

            UpdateMessages();

            UpdateHeadNotificationBar();
        }

        private void UpdateHeadNotificationBar()
        {
            _lblCulture.Text = string.Format("{0:0.0}/{0:0.0} ({0:0.0})", Game.Human.Culture, Game.Human.CultureNeededForChange, /*Game.Human.CultureSurplus*/ 0);
            _lblHappiness.Text = Game.Human.HappyCities + "/" + Game.Human.UnhappyCities;
            _lblScience.Text = string.Format("{0:##.#}", Game.Human.ScienceSurplus);
        }

        void ImgZoomOutClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            _mapBox.ZoomOut();
        }

        void ImgZoomInClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            _mapBox.ZoomIn();
        }

        GameMapBox.ZoomState _oldState;
        void ToogleView()
        {
            switch (_view)
            {
                case MapView.Main:
                    ShowMainControls(false);

                    // city
                    ShowCityControls(true);

                    _oldState = _mapBox.Zoom;
                    _mapBox.Zoom = GameMapBox.ZoomState.VeryNear;
                    _currentCity.InDetailView = true;

                    _view = MapView.City;
                    break;
                case MapView.City:
                    ShowMainControls(true);

                    // city
                    ShowCityControls(false);

                    _mapBox.Zoom = _oldState;

                    _view = MapView.Main;
                    _currentCity.InDetailView = false;
                    _currentCity = null;
                    break;
            }
        }

        private void ShowMainControls(bool visible)
        {
            _imgZoomIn.Visible = visible;
            _imgZoomOut.Visible = visible;

            //_sidebar.Visible = visible;

            _lblUnit.Visible = visible;

            _lblOverview.Visible = visible;
            _lblBottomRight.Visible = visible;

            _lblUnit.Visible = visible;
            for (int i = 0; i < _actionButtons.Length; ++i)
                _actionButtons[i].Visible = visible;
        }

        public bool FogOfWarEnabled
        {
            get { return _mapBox.FogOfWarEnabled; }
        }

        void MapBox_FocusChanged(MapChangeArgs args)
        {
            if (args.Map == null)
                return;

            if (_lblPosition != null)
            {
                HexPoint pt = args.UpdatedTiles.First();

                if (_mapBox.Map.IsValid(pt))
                {
                    MapCell cell = _mapBox.Map[pt];
                    River river = _mapBox.Map.GetRiverAt(pt);

                    _lblPosition.Text = "Pos: " + pt; // +" " + (cell.IsCoast ? _mapBox.Map.GetCoastalTileIndex(pt.X, pt.Y).ToString() : "");
                    _lblTerrainName.Text = cell.Terrain.Name;
                    if( cell.Terrain.Image != null )
                        _lblTerrainIcon.Image = cell.Terrain.Image.GetThumbnail(64, 64);
                    else
                        _lblTerrainIcon.Image = IconProvider.DefaultTerrainIcon.GetThumbnail(64, 64);
                    _lblFeatures.Text = "Feat: " + cell.FeatureStr;
                    _lblResource.Text = "Res:" + cell.RessourceStr + " " + (river == null ? "no river" : river.ToString());
                    
                    _lblFood.Text = cell.Food.ToString();
                    _lblCommercial.Text = cell.Commerce.ToString();
                    _lblProduction.Text = cell.Production.ToString();

                    List<Unit> units = MainWindow.Game.GetUnitsAt(pt);
                    City city = MainWindow.Game.GetCityAt(pt);
                    if (units.Count > 0 || city != null)
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (Unit u in units)
                            sb.Append("Unit: " + u.Data.Title + " " + u.UnitAI + " " + u.Action + ",\n");

                       if( city != null )
                            sb.Append("City: " + city.Name + ",\n");

                        _lblUnitName.Text = sb.ToString().TrimEnd(',');
                    }
                    else
                    {
                        _lblUnitName.Text = "";
                    }

                    _lblRegion.Text = _mapBox.Map.GetRegionNames(pt);

                    // influence
                    foreach (AbstractPlayerData pl in MainWindow.Game.Players)
                    {
                        // only AI Players:
                        if( pl.CityLocationMap != null )
                            _lblRegion.Text += "\n" + pl.Civilization.Name + "=>" + pl.CityLocationMap[pt] + "=>" + pl.CityLocationMap.IsLocalMaximum(pt);
                    }
                }
                else
                {
                    _lblPosition.Text = "Pos: " + pt;
                    _lblTerrainName.Text = "(no terrain)";
                    _lblTerrainIcon.Image = IconProvider.DefaultTerrainIcon.GetThumbnail(64, 64);
                    _lblFeatures.Text = "(no feature)";
                    _lblResource.Text = "(no resource)";
                    _lblFood.Text = "-";
                    _lblCommercial.Text = "-";
                    _lblProduction.Text = "-";
                    _lblUnitName.Text = "";
                    _lblRegion.Text = "";
                }
            }
        }

        void LblTurnClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            _game.Turn();

            _lblCurrentTurn.Text = string.Format(Strings.TXT_KEY_UI_MAINAPPLICATION_TURN_PATTERN, _game.CurrentTurn, _game.Year);
        }

        Locales _currentLocale = Locales.de_de;
        void ImgLocaleDoubleClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            switch (_currentLocale)
            {
                case Locales.de_de:
                    Strings.Culture = CultureInfo.GetCultureInfo("en-US");

                    _currentLocale = Locales.en_us;
                    _imgLocale.Image = IconProvider.GetLocaleIcon("en-us");
                    break;
                default:
                    Strings.Culture = CultureInfo.GetCultureInfo("de-DE");

                    _currentLocale = Locales.de_de;
                    _imgLocale.Image = IconProvider.GetLocaleIcon("de-de");
                    break;
            }

            _btnCreate.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_START;
            _btnCheck.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_CHECK;
            _btnLoad.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_LOAD;
            _btnExit.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_EXIT;
            _lblCurrentTurn.Text = string.Format(Strings.TXT_KEY_UI_MAINAPPLICATION_TURN_PATTERN, _game.CurrentTurn, _game.Year);
            _btnTurn.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_TURN;
        }

        void lblHappiness_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //throw new NotImplementedException();
        }

        ////////////////////////////////////////////////////////////////////////////

        #endregion

        ////////////////////////////////////////////////////////////////////////////   
        public static GameData Game
        {
            get
            {
                return _game;
            }
        }

        public GameData GameInstance
        {
            get
            {
                return _game;
            }
        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////    
        void BtnExitClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Close();
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void BtnCreateClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //InitPlayerNumberDialog(); 
            GameFacade.getInstance().SendNotification(GameNotification.CreateMap, _startupSettings);
        }
        ////////////////////////////////////////////////////////////////////////////  

        //////////////////////////////////////////////////////////////////////////// 
        void BtnLoadClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            SelectMapLoadDialog tmp = new SelectMapLoadDialog(Manager);
            tmp.Closed += SelectMapLoadDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal(); 
            
        }

        void SelectMapLoadDialogClosed(object sender, WindowClosedEventArgs e)
        {
            if (sender == null)
                return;

            Dialog dialog = sender as Dialog;
            if (dialog != null && dialog.ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
            {
                GameFacade.getInstance().SendNotification(GameNotification.LoadMap, (sender as SelectMapLoadDialog).SelectedItem);
            }

            e.Dispose = true;
        }
        //////////////////////////////////////////////////////////////////////////// 
        void BtnCheckClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            string checkResult = Provider.Instance.Check();

            Dialog d = new Dialog(Manager);
            d.Init();
            d.Icon = Icon;
            d.Caption.Text = "Check result";
            d.Description.Text = checkResult;
            d.Description.Height = 400;
            d.TopPanel.Height = 400;
            d.BottomPanel.Top = 400;
            Manager.Add(d);
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////   
        void InitPlayerNumberDialog()
        {
            SelectPlayerNumberDialog tmp = new SelectPlayerNumberDialog(Manager);
            tmp.Closed += SelectPlayerNumberDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal();   
        }

        void SelectPlayerNumberDialogClosed(object sender, WindowClosedEventArgs e)
        {
            if ((sender as Dialog).ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
            {
                _startupSettings.PlayerNumber = (sender as SelectPlayerNumberDialog).SelectetItem;

                InitMapSizeDialog();
            }

            e.Dispose = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////     
        void InitMapSizeDialog()
        {
            SelectMapSizeDialog tmp = new SelectMapSizeDialog(Manager);
            tmp.Closed += SelectMapSizeDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal();   
        }

        void SelectMapSizeDialogClosed(object sender, WindowClosedEventArgs e)
        {
            if ((sender as Dialog).ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
            {
                _startupSettings.MapSize = (sender as SelectMapSizeDialog).SelectetItem;

                InitMapTypeDialog();
            }

            e.Dispose = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////     
        void InitMapTypeDialog()
        {
            SelectMapTypeDialog tmp = new SelectMapTypeDialog(Manager);
            tmp.Closed += SelectMapTypeDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal();
        }

        void SelectMapTypeDialogClosed(object sender, WindowClosedEventArgs e)
        {
            if ((sender as Dialog).ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
            {
                _startupSettings.MapType = (sender as SelectMapTypeDialog).SelectetItem;

                InitDifficultyDialog();
            }

            e.Dispose = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////     
        void InitDifficultyDialog()
        {
            SelectDifficultyDialog tmp = new SelectDifficultyDialog(Manager);
            tmp.Closed += SelectDifficultyDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal();
        }

        void SelectDifficultyDialogClosed(object sender, WindowClosedEventArgs e)
        {
            if ((sender as Dialog).ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
            {
                _startupSettings.Difficulty = (sender as SelectDifficultyDialog).SelectetItem;

                InitCivilizationDialog();
            }

            e.Dispose = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////      
        void InitCivilizationDialog()
        {
            SelectCivilizationDialog tmp = new SelectCivilizationDialog(Manager);
            tmp.Closed += SelectCivilizationDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal();
        }

        void SelectCivilizationDialogClosed(object sender, WindowClosedEventArgs e)
        {
            if ((sender as Dialog).ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
            {
                _startupSettings.CivilizationName = (sender as SelectCivilizationDialog).SelectetItem;

                Dialog d = new Dialog(Manager);
                d.Description.Text = _startupSettings.ToString();
                d.Init();
                Manager.Add(d);
                d.Show();
            }

            e.Dispose = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        protected override void Dispose(bool disposing)
        {
            if (_game != null)
                _game.Dispose();

            base.Dispose(disposing);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public override List<GameNotification> NotificationInterests
        {
            get
            {
                return new List<GameNotification>
                           {
                               GameNotification.CreateMapSuccess, 
                               GameNotification.LoadMapSuccess,
                               GameNotification.Message,
                               GameNotification.UpdateSpotting,
                               GameNotification.StartEra
                           };
            }
        }
        ////////////////////////////////////////////////////////////////////////////
    }
}
