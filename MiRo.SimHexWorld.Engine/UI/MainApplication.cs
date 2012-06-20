using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using TomShane.Neoforce.Controls;
using System;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.UI.Layout;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World;
using MiRoSimHexWorld.Engine.World.Helper;
using MiRo.SimHexWorld.Engine.World.Maps;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace MiRo.SimHexWorld.Engine.UI
{
    public class MainApplication : Application
    {
        //static ArcBallCamera _camera;
        //const float CameraDistance = 50;

        private static Manager _manager;
        public static Manager ManagerInstance
        {
            get
            {
                if (_manager == null)
                    throw new Exception("Manager not initialized yet!");

                return _manager;
            }
        }

        public static bool IsManagerReady
        {
            get
            {
                return _manager != null;
            }
        }

        private static MainApplication _instance;

        public static MainApplication Instance 
        { 
            get
            {
                if( _instance == null)
                    _instance = new MainApplication();

                return _instance;
            }
        }

        private MainWindow _main;

        private MainApplication()
            : base("Default", true)
        {
            Initilaize();
        }

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////
        private void Initilaize()
        {
            Graphics.PreferredBackBufferWidth = 1600;
            Graphics.PreferredBackBufferHeight = 850;
            Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(Graphics_PreparingDeviceSettings);

            // Setting up the shared skins directory
            Manager.SkinDirectory = @"Content\Skins\";
            Manager.LayoutDirectory = @"Content\Layouts\";

            SystemBorder = false;
            FullScreenBorder = false;
            ClearBackground = false;
            ExitConfirmation = false;
            Manager.TargetFrames = 30;

            _manager = Manager;
        }

        void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents; 
        }
        ////////////////////////////////////////////////////////////////////////////
        //protected override void Draw(GameTime gameTime)
        //{
        //    //if (renderer != null)
        //    //    renderer.Draw(gameTime, _camera);

        //    base.Draw(gameTime);
        //}

        //protected override void Update(GameTime gameTime)
        //{
        //    if (renderer != null)
        //        renderer.Update(gameTime);

        //    base.Update(gameTime);
        //}

        ////////////////////////////////////////////////////////////////////////////
        protected override Window CreateMainWindow()
        {
            if (_main == null)
            {
                Manager.SetSkin("Default");

                _main = new MainWindow(Manager);
                GameFacade.getInstance().RegisterMediator(_main);

                //_main.GameInstance.MapUpdated += new World.Maps.MapUpdateHandler(GameInstance_MapUpdated);
            }

            return _main;
        }

        //MapRenderer renderer;
        //void GameInstance_MapUpdated(World.Maps.MapChangeArgs args)
        //{
        //    // rendering
        //    _camera = new ArcBallCamera(MapData.GetWorldPosition(20, 20), 0, MathHelper.PiOver2 / 2, 0, MathHelper.PiOver2, CameraDistance, 30, 100, _manager.GraphicsDevice);

        //    renderer = new MapRenderer(_manager);
        //    renderer.Initialize();
        //    renderer.LoadContent();
        //    renderer.Map = args.Map;
        //}
        ////////////////////////////////////////////////////////////////////////////

        protected override void Dispose(bool disposing)
        {
            if (_main != null)
                _main.Dispose();

            try
            {
                base.Dispose(disposing);
            }
            catch { }
        }

        #endregion
    }
}
