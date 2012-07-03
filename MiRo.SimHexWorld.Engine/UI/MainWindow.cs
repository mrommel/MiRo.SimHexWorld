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

        TimeSpan lastFpsUpdate;
        FpsCounter fpsCounter;

        public MapView View { get; set; }

        public Locales CurrentLocale = Locales.de_DE;

        private static readonly GameOptions _config = new GameOptions();

        public HexPoint Focus { get; set; }

        readonly GameStartupSettings _startupSettings = new GameStartupSettings();

        private static readonly GameData _game = new GameData();

        private Dictionary<string, AssetWindow> _windows = new Dictionary<string, AssetWindow>();

        ////////////////////////////////////////////////////////////////////////////       

        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////
        public MainWindow(Manager manager)
            : base(manager, "Content//Controls//MainWindow")
        {
            _config.Load();

            // Tell the resource manager what language to use when loading strings.
            Strings.Culture = CultureInfo.CurrentCulture;

            View = MapView.Main;

            _defaultbg = Manager.Content.Load<Texture2D>("Content\\Textures\\UI\\bg_body3");

            var application = Manager.Game as Application;
            if (application != null)
                application.BackgroundImage = _defaultbg;

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

            ShowMainDialog();
        }

        protected void PrepareGraphicsDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        public void CreateWindow(string name, string asset, bool visible)
        {
            if( _windows.ContainsKey(name))
                return;

            GenericWindow win = new GenericWindow(Manager, asset);
            win.Visible = visible;
            Manager.Add(win);

            _windows.Add(name, win);
        }

        public AssetWindow GetWindow(string name)
        {
            if( _windows.ContainsKey(name))
                return _windows[name];

            return null;
        }

        public void FocusPreviousCity()
        {
            if (CurrentCity != null)
            {
                int index = Game.Human.Cities.IndexOf(CurrentCity);

                if (index != -1)
                {
                    index--;
                    if (index < 0) 
                        index = Game.Human.Cities.Count - 1;

                    CurrentCity = Game.Human.Cities[index];
                }
            }
        }

        public void FocusNextCity()
        {
            if (CurrentCity != null)
            {
                int index = Game.Human.Cities.IndexOf(CurrentCity);

                if (index != -1)
                {
                    index++;
                    if (index >= Game.Human.Cities.Count - 1)
                        index = 0;

                    CurrentCity = Game.Human.Cities[index];
                }
            }
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
                    MapLoadDialog tmp = new MapLoadDialog(Manager);
                    tmp.ShowModal();
                    Manager.Add(tmp);
                    break;
                case MainOptionChoises.Check:
                    break;
                case MainOptionChoises.Exit:
                    Close();
                    break;
            }
        }

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

                _engine.Invoke("window", "Update", this, gameTime);
                //UpdateHeadNotificationBar();
                UpdateRanking();
            }

            UpdateMessages();
        }

        private void UpdateRanking()
        {
            foreach (AbstractPlayerData player in Game.Players)
            {
                RankingRow row = GetControl("Ranking" + player.Id) as RankingRow;

                if (row != null)
                {
                    row.Visible = _config.ShowRanking;

                    if (_config.ShowRanking)
                        row.Player = player;
                }
            }           
        }

        //private void UpdateHeadNotificationBar()
        //{
        //    _lblCulture.Text = string.Format("{0:0}", ( Game.Human.CultureNeededForChange - Game.Human.Culture ) / Game.Human.CulturePerTurn);
        //    _lblHappiness.Text = Game.Human.HappyCities + "/" + Game.Human.UnhappyCities;
        //    _lblScience.Text = string.Format("{0:##.#}", Game.Human.ScienceSurplus);
        //}

        public GameMapBox MapBox
        {
            get { return GetControl("MapBox") as GameMapBox; }
        }

        public bool FogOfWarEnabled
        {
            get { return MapBox.FogOfWarEnabled; }
        }

        //void LblTurnClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    _game.Turn();

        //    _lblCurrentTurn.Text = string.Format(Strings.TXT_KEY_UI_MAINAPPLICATION_TURN_PATTERN, _game.CurrentTurn, _game.Year);
        //}

        
        //void ImgLocaleDoubleClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    switch (CurrentLocale)
        //    {
        //        case Locales.de_de:
        //            Strings.Culture = CultureInfo.GetCultureInfo("en-US");

        //            CurrentLocale = Locales.en_us;
        //            _imgLocale.Image = IconProvider.GetLocaleIcon("en-us");
        //            break;
        //        default:
        //            Strings.Culture = CultureInfo.GetCultureInfo("de-DE");

        //            CurrentLocale = Locales.de_de;
        //            _imgLocale.Image = IconProvider.GetLocaleIcon("de-de");
        //            break;
        //    }

        //    //_btnCreate.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_START;
        //    //_btnCheck.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_CHECK;
        //    //_btnLoad.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_LOAD;
        //    //_btnExit.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_EXIT;
        //    //_lblCurrentTurn.Text = string.Format(Strings.TXT_KEY_UI_MAINAPPLICATION_TURN_PATTERN, _game.CurrentTurn, _game.Year);
        //    //_btnTurn.Text = Strings.TXT_KEY_UI_MAINAPPLICATION_TURN;
        //}

        //void lblHappiness_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

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

        public static GameOptions Config
        {
            get
            {
                return _config;
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
            MapLoadDialog tmp = new MapLoadDialog(Manager);
            //tmp.Closed += SelectMapLoadDialogClosed;
            tmp.Init();
            Manager.Add(tmp);
            tmp.ShowModal();

        }

        //void SelectMapLoadDialogClosed(object sender, WindowClosedEventArgs e)
        //{
        //    if (sender == null)
        //        return;

        //    Dialog dialog = sender as Dialog;
        //    if (dialog != null && dialog.ModalResult == TomShane.Neoforce.Controls.ModalResult.Ok)
        //    {
        //        //GameFacade.getInstance().SendNotification(GameNotification.LoadMap, (sender as MapLoadDialog).SelectedItem);
        //    }

        //    e.Dispose = true;
        //}
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
                            Messages.Add(new ScreenNotification(type, txt, DateTime.Now.AddSeconds(10), obj));
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
