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
    public partial class MainWindow : AssetWindow
    {
        public enum MapView { Main, City }

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////       
        private readonly Texture2D _defaultbg;

        FpsCounter fpsCounter;
        SpriteFont _messageFont;

        //GameMapBox _mapBox;
        //ContextMenu _ctxUnitMenu;

        public MapView View { get; set; }

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

        //SideBar _sidebar;
        //SideBarPanel _pnlRes;
        //Button _btnCreate;
        //Button _btnLoad;
        //Button _btnCheck;
        //Button _btnExit;

        //ImageBox _lblTerrainIcon;
        //Label _lblPosition;
        //Label _lblTerrainName;
        //Label _lblFeatures;
        //Label _lblResource;

        //ImageBox _imgFood, _imgCommercial, _imgProduction;
        //Label _lblFood, _lblCommercial, _lblProduction;

        //Label _lblUnitName, _lblRegion;

        // unit
        //ImageBox _lblUnit;

        // zoom
        //ImageBox _imgZoomIn, _imgZoomOut;

        public HexPoint Focus { get; set; }

        readonly GameStartupSettings _startupSettings = new GameStartupSettings();

        private static readonly GameData _game = new GameData();

        ////////////////////////////////////////////////////////////////////////////       

        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////
        public MainWindow(Manager manager)
            : base(manager, "Content//Controls//MainWindow")
        {
            // Tell the resource manager what language to use when loading strings.
            Strings.Culture = CultureInfo.CurrentCulture;

            View = MapView.Main;

            _defaultbg = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\bg_body3");

            var application = Manager.Game as Application;
            if (application != null)
                application.BackgroundImage = _defaultbg;

            Icon = IconProvider.ApplicationIcon;

            _messageFont = Manager.Content.Load<SpriteFont>("Content\\Fonts\\ArialL");

            InitMainControls();
            InitCityControls();
            InitUnitControls();
            InitScienceControls();
            InitOverviewControls();
            InitMessages();
            InitRightTop();

            Manager.SetSkin("Default");

            _startupSettings.MapSize = "Tiny";
            _startupSettings.MapType = "Pangaea";

            fpsCounter = new FpsCounter();

            Text = Strings.Title;
            CaptionVisible = true;
            BorderVisible = true;

            Manager.Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PrepareGraphicsDevice);
        }

        protected void PrepareGraphicsDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        //void _btnCityExit_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    ToogleView();
        //}

        private void InitMainControls()
        {
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

        TimeSpan lastFpsUpdate;
        protected override void Update(GameTime gameTime)
        {
            fpsCounter.Update(gameTime);

            base.Update(gameTime);

            if (_needToUpdateOverview)
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
                this.Text += ", Cursor: " + Focus;
                lastFpsUpdate = TimeSpan.FromSeconds(3);
            }

            //if (View == MapView.City)
            //    UpdateCityControls();

            UpdateMessages();

            UpdateHeadNotificationBar();
        }

        private void UpdateHeadNotificationBar()
        {
            _lblCulture.Text = string.Format("{0:0}", ( Game.Human.CultureNeededForChange - Game.Human.Culture ) / Game.Human.CulturePerTurn);
            _lblHappiness.Text = Game.Human.HappyCities + "/" + Game.Human.UnhappyCities;
            _lblScience.Text = string.Format("{0:##.#}", Game.Human.ScienceSurplus);
        }

        public GameMapBox MapBox
        {
            get { return GetControl("MapBox") as GameMapBox; }
        }

        //GameMapBox.ZoomState _oldZoomState;
        //public void ToogleView()
        //{
        //    switch (View)
        //    {
        //        case MapView.Main:
        //            ShowMainControls(false);

        //            // city
        //            ShowCityControls(true);

        //            _oldZoomState = MapBox.Zoom;
        //            MapBox.Zoom = GameMapBox.ZoomState.VeryNear;
        //            CurrentCity.InDetailView = true;

        //            View = MapView.City;
        //            break;
        //        case MapView.City:
        //            ShowMainControls(true);

        //            // city
        //            ShowCityControls(false);

        //            MapBox.Zoom = _oldZoomState;

        //            View = MapView.Main;
        //            CurrentCity.InDetailView = false;
        //            CurrentCity = null;
        //            break;
        //    }
        //}

        //private void ShowMainControls(bool visible)
        //{
        //    //_imgZoomIn.Visible = visible;
        //    //_imgZoomOut.Visible = visible;

        //    //_sidebar.Visible = visible;

        //    //_lblUnit.Visible = visible;

        //    GetControl("OverviewTop").Visible = visible;
        //    GetControl("OverviewBottomRight").Visible = visible;
        //    GetControl("OverviewMap").Visible = visible;

        //    //_lblUnit.Visible = visible;
        //    //for (int i = 0; i < _actionButtons.Length; ++i)
        //    //    _actionButtons[i].Visible = visible;

        //    GetControl("LeftTopCorner").Visible = visible;
        //    GetControl("ResearchProgress").Visible = visible;
        //    GetControl("ScienceDetail").Visible = visible;
        //}

        public bool FogOfWarEnabled
        {
            get { return MapBox.FogOfWarEnabled; }
        }

        //void MapBox_FocusChanged(MapChangeArgs args)
        //{
        //    if (args.Map == null)
        //        return;

        //    if (_lblPosition != null)
        //    {
        //        HexPoint pt = args.UpdatedTiles.First();
        //        Focus = pt;

        //        if (MapBox.Map.IsValid(pt))
        //        {
        //            MapCell cell = MapBox.Map[pt];
        //            River river = MapBox.Map.GetRiverAt(pt);

        //            _lblPosition.Text = "Pos: " + pt; // +" " + (cell.IsCoast ? _mapBox.Map.GetCoastalTileIndex(pt.X, pt.Y).ToString() : "");
        //            _lblTerrainName.Text = cell.Terrain.Name;
        //            if (cell.Terrain.Image != null)
        //                _lblTerrainIcon.Image = cell.Terrain.Image.GetThumbnail(64, 64);
        //            else
        //                _lblTerrainIcon.Image = IconProvider.DefaultTerrainIcon.GetThumbnail(64, 64);
        //            _lblFeatures.Text = "Feat: " + cell.FeatureStr;
        //            _lblResource.Text = "Res:" + cell.RessourceStr + " " + (river == null ? "no river" : river.ToString());

        //            _lblFood.Text = cell.Food.ToString();
        //            _lblCommercial.Text = cell.Commerce.ToString();
        //            _lblProduction.Text = cell.Production.ToString();

        //            List<Unit> units = MainWindow.Game.GetUnitsAt(pt);
        //            City city = MainWindow.Game.GetCityAt(pt);
        //            if (units.Count > 0 || city != null)
        //            {
        //                StringBuilder sb = new StringBuilder();

        //                foreach (Unit u in units)
        //                    sb.Append("Unit: " + u.Data.Title + " " + u.UnitAI + " " + u.Action + ",\n");

        //                if (city != null)
        //                    sb.Append("City: " + city.Name + ",\n");

        //                _lblUnitName.Text = sb.ToString().TrimEnd(',');
        //            }
        //            else
        //            {
        //                _lblUnitName.Text = "";
        //            }

        //            _lblRegion.Text = MapBox.Map.GetRegionNames(pt);

        //            // influence
        //            foreach (AbstractPlayerData pl in MainWindow.Game.Players)
        //            {
        //                // only AI Players:
        //                if (pl.CityLocationMap != null)
        //                    _lblRegion.Text += "\n" + pl.Civilization.Name + "=>" + pl.CityLocationMap[pt] + "=>" + pl.CityLocationMap.IsLocalMaximum(pt);
        //            }
        //        }
        //        else
        //        {
        //            _lblPosition.Text = "Pos: " + pt;
        //            _lblTerrainName.Text = "(no terrain)";
        //            _lblTerrainIcon.Image = IconProvider.DefaultTerrainIcon.GetThumbnail(64, 64);
        //            _lblFeatures.Text = "(no feature)";
        //            _lblResource.Text = "(no resource)";
        //            _lblFood.Text = "-";
        //            _lblCommercial.Text = "-";
        //            _lblProduction.Text = "-";
        //            _lblUnitName.Text = "";
        //            _lblRegion.Text = "";
        //        }
        //    }
        //}

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

            //_btnCreate.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_START;
            //_btnCheck.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_CHECK;
            //_btnLoad.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_LOAD;
            //_btnExit.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_EXIT;
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

        public override void HandleNotification(INotification notification)
        {
            switch ((GameNotification)System.Enum.Parse(typeof(GameNotification), notification.Name))
            {
                case GameNotification.CreateMapSuccess:
                    _game.Map = notification.Body as MapData;
                    MapBox.Map = _game.Map;

                    _game.Initialize();

                    break;
                case GameNotification.LoadMapSuccess:
                    _game.Map = notification.Body as MapData;
                    MapBox.Map = _game.Map;

                    _game.Initialize();
                    break;
                case GameNotification.Message:
                    {
                        List<object> objs = notification.Body as List<object>;

                        NotificationType type = (NotificationType)Enum.Parse(typeof(NotificationType), objs[0].ToString());
                        string txt = objs[1] as string;
                        Civilization civSender = objs[2] as Civilization;
                        MessageFilter filter = (MessageFilter)objs[3];
                        object obj = objs[4];

                        if ((civSender.Name == Game.Human.Civilization.Name && (IsSet(filter, MessageFilter.Self))) ||
                            IsValidMessage(Game.Human.DiplomaticStatusTo(civSender), filter))
                            _messages.Add(new ScreenNotification(type, txt, DateTime.Now.AddSeconds(10), obj));
                    }
                    break;
                case GameNotification.UpdateSpotting:
                    _needToUpdateOverview = true;
                    break;
                case GameNotification.StartEra:
                    {
                        List<object> objs = notification.Body as List<object>;

                        AbstractPlayerData player = objs[0] as AbstractPlayerData;
                        Era era = objs[1] as Era;

                        // show window if human
                        if (player.IsHuman)
                            NewEraWindow.Show(Manager, era, Strings.TXT_KEY_UI_NEWERA_TITLE);
                    }
                    break;
                case GameNotification.ShowScoreHistory:

                    break;
                default:
                    throw new System.Exception(notification.Name + " notification not handled");
            }
        }
    }
}
