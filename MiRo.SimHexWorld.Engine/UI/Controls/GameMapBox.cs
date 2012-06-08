using MiRo.SimHexWorld.Engine.World;
using MiRo.SimHexWorld.Engine.World.Maps;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiRo.SimHexWorld.World.Maps;
using MiRo.SimHexWorld.Engine.Types;
using MiRoSimHexWorld.Engine.World.Helper;
using MiRoSimHexWorld.Engine.World.Scenarios;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.UI.Entities;
using MiRo.SimHexWorld.Engine.World.Entities;
using MiRo.SimHexWorld.Engine.Misc;
using System;
using MiRo.SimHexWorld.Engine.UI.Dialogs;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class GameMapBox : GameControl
    {
        Effect _effect;

        Texture2D _cloudMap;
        Model _skyDome;

        MapData _map;
        readonly MapRenderer _mapRenderer;

        public static ArcBallCamera _camera;
        const float CameraDistance = 50;

        public event MapUpdateHandler FocusChanged;
        public event CityOpenHandler CityOpened;
        public event CityOpenHandler CitySelected;
        public event UnitsSelectHandler HumanUnitsSelected;
        public event UnitsSelectHandler EnemyUnitsSelected;
        public event UnselectHandler UnitsUnselected;

        readonly HexPoint _mapCenter = new HexPoint(20, 20);
        KeyboardState _oldKeyState;
        MouseState _oldMouseState;

        static readonly Plane GroundPlane = new Plane(Vector3.Up, 0f);

        Viewport defaultViewport;
        Viewport controlViewport;

        public GameMapBox(Manager manager)
            : base(manager)
        {
            _mapRenderer = new MapRenderer(manager);

            Initialize();
        }

        private void Initialize()
        {
            // create Camera
            _camera = new ArcBallCamera(MapData.GetWorldPosition(20, 20), 0, MathHelper.PiOver2 * 0.5f * 0.8f * 0.8f, 0, MathHelper.PiOver2, CameraDistance, 30, 100, Manager.GraphicsDevice);

            _effect = Manager.Content.Load<Effect>("Content/Effects/Series4Effects");

            Mouse.SetPosition(Manager.GraphicsDevice.Viewport.Width / 2, Manager.GraphicsDevice.Viewport.Height / 2);

            _skyDome = Manager.Content.Load<Model>("Content/Models/dome");
            _skyDome.Meshes[0].MeshParts[0].Effect = _effect.Clone();

            _cloudMap = Manager.Content.Load<Texture2D>("Content/Models/cloudMap");

            _mapRenderer.Initialize();
            _mapRenderer.LoadContent();

            // init complete view
            defaultViewport = new Viewport();
            defaultViewport.X = 0;
            defaultViewport.Y = 0;
            defaultViewport.Width = Manager.GraphicsDevice.Viewport.Width;
            defaultViewport.Height = Manager.GraphicsDevice.Viewport.Height;

            // init control view
            controlViewport = new Viewport();
            controlViewport.X = 7;
            controlViewport.Y = 27;
            controlViewport.Width = Manager.GraphicsDevice.Viewport.Width - 14;
            controlViewport.Height = Manager.GraphicsDevice.Viewport.Height - 32;
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            _mapRenderer.Update(gameTime);

            // Update the camera
            _camera.Update();

            UpdateCursor();
        }

        public bool FogOfWarEnabled
        {
            get { return _mapRenderer.FogOfWarEnabled; }
        }

        HexPoint _dragStart;
        private void HandleKeyboard()
        {
            int dx = 0, dy = 0;

            KeyboardState keyState = Keyboard.GetState();
            if ((keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W)) &&
                !(_oldKeyState.IsKeyDown(Keys.Up) || _oldKeyState.IsKeyDown(Keys.W)))
                dy = -1;
            if ((keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S)) &&
                !(_oldKeyState.IsKeyDown(Keys.Down) || _oldKeyState.IsKeyDown(Keys.S)))
                dy = 1;
            if ((keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D)) &&
                !(_oldKeyState.IsKeyDown(Keys.Right) || _oldKeyState.IsKeyDown(Keys.D)))
                dx = 1;
            if ((keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A)) &&
                !(_oldKeyState.IsKeyDown(Keys.Left) || _oldKeyState.IsKeyDown(Keys.A)))
                dx = -1;

            if (keyState.IsKeyDown(Keys.F) && !_oldKeyState.IsKeyDown(Keys.F))
                _mapRenderer.FogOfWarEnabled = !_mapRenderer.FogOfWarEnabled;

            if (dx != 0 || dy != 0)
            {
                _mapCenter.X += dx;
                _mapCenter.Y += dy;
                _mapRenderer.Center = _mapCenter;
                _camera.Target = MapData.GetWorldPosition(_mapCenter);
            }

            if (keyState.IsKeyDown(Keys.K) && !_oldKeyState.IsKeyDown(Keys.K))
                _camera.RotationX += 0.2f;

            if (keyState.IsKeyDown(Keys.OemPlus) && !_oldKeyState.IsKeyDown(Keys.OemPlus))
                ZoomIn();

            if (keyState.IsKeyDown(Keys.OemMinus) && !_oldKeyState.IsKeyDown(Keys.OemMinus))
                ZoomOut();

            if (keyState.IsKeyDown(Keys.N) && !_oldKeyState.IsKeyDown(Keys.N))
                _camera.RotationY *= 0.8f;

            if (keyState.IsKeyDown(Keys.M) && !_oldKeyState.IsKeyDown(Keys.M))
                _camera.RotationY *= 1.2f;

            if (keyState.IsKeyDown(Keys.C) && !_oldKeyState.IsKeyDown(Keys.C))
            {

                City capital = MainWindow.Game.Human.Capital;

                if (capital != null)
                {
                    _mapCenter.X = capital.Point.X;
                    _mapCenter.Y = capital.Point.Y;
                    _camera.Target = MapData.GetWorldPosition(_mapCenter);
                    _mapRenderer.Center = _mapCenter;
                }
                else
                {
                    if (Map != null && Map.Extension != null)
                    {
                        StartLocation loc = Map.Extension.StartLocations.FirstOrDefault(a => a.CivilizationName == MainWindow.Game.Human.Civilization.Name);

                        if (loc == null)
                            throw new Exception("No Start location for " + MainWindow.Game.Human.Civilization.Name);

                        _mapCenter.X = loc.X;
                        _mapCenter.Y = loc.Y;
                        _camera.Target = MapData.GetWorldPosition(_mapCenter);
                        _mapRenderer.Center = _mapCenter;
                    }
                }
            }

            if (keyState.IsKeyDown(Keys.P) && !_oldKeyState.IsKeyDown(Keys.P))
            {
                //PolicyChooseDialog pcd = new PolicyChooseDialog(Manager);
                //pcd.Left = 100;
                //pcd.Top = 100;

                //Manager.Add(pcd);

                //pcd.ShowModal();
                ScoreWindow.Show(Manager, MainWindow.Game.Scores, "Scores");
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Released && _oldMouseState.RightButton == ButtonState.Pressed)
            {
                City city = MainWindow.Game.GetCityAt(_mapRenderer.Cursor);

                // select unit city
                if (city != null)
                {
                    if (CityOpened != null)
                        CityOpened(city);
                }
            }
            else if (mouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                _dragStart = _mapRenderer.Cursor;
            }
            else if (mouseState.LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_dragStart != _mapRenderer.Cursor)
                {
                    // drag action
                    List<Unit> units = MainWindow.Game.GetUnitsAt(_dragStart);

                    Unit unit = units.FirstOrDefault();

                    if (unit != null)
                    {
                        unit.Action = UnitAction.Move;
                        unit.SetTarget(_mapRenderer.Cursor);
                    }
                }
                else
                {
                    //City city = MainWindow.Game.GetCityAt(_mapRenderer.Cursor);
                    List<Unit> units = MainWindow.Game.GetUnitsAt(_mapRenderer.Cursor);

                    // select unit city
                    //if (city != null)
                    //{
                    //    if (CitySelected != null)
                    //        CitySelected(city);
                    //}

                    // select unit
                    if (units.Count > 0)
                    {
                        Unit first = units.First();

                        if (first.Player.IsHuman && HumanUnitsSelected != null)
                            HumanUnitsSelected(units);
                        else if (!first.Player.IsHuman && EnemyUnitsSelected != null)
                            EnemyUnitsSelected(units);
                    }
                    else if (UnitsUnselected != null)
                        UnitsUnselected();
                }
            }

            _oldKeyState = keyState;
            _oldMouseState = mouseState;
        }

        private void UpdateCursor()
        {
            // Get the new keyboard and mouse state
            MouseState mouseState = Mouse.GetState();

            //Vector3 mousePos = Manager.GraphicsDevice.Viewport.Unproject(new Vector3(mouseState.X + Left, mouseState.Y + Top, 0), camera.Projection, camera.View, Matrix.Identity);
            Vector3 mousePos = Manager.GraphicsDevice.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 0), _camera.Projection, _camera.View, Matrix.Identity);

            // get views intersection with ground plane
            Vector3 direction = mousePos - _camera.Position;
            var r = new Ray(_camera.Position, direction);
            float? intersection = r.Intersects(GroundPlane);

            if (intersection.HasValue)
            {
                Vector3 groundIntersection = _camera.Position + direction * intersection.Value;
                HexPoint pt = MapData.GetMapPosition(groundIntersection);

                pt.MoveDir(HexDirection.SouthEast);

                if (FocusChanged != null)
                    FocusChanged(new MapChangeArgs(Map, pt));

                _mapRenderer.Cursor = pt;
            }
        }

        public static ArcBallCamera Camera
        {
            get { return _camera; }
        }

        #region zoom handling
        public enum ZoomState { VeryNear, Near, Middle, Far, VeryFar }

        ZoomState _zoom = ZoomState.Middle;

        private float ZoomFactor
        {
            get
            {
                switch (_zoom)
                {
                    case ZoomState.VeryFar:
                        return 2.0f;
                    case ZoomState.Far:
                        return 1.3f;
                    case ZoomState.Middle:
                        return 1.0f;
                    case ZoomState.Near:
                        return 0.7f;
                    case ZoomState.VeryNear:
                        return 0.5f;
                }

                return 1.0f;
            }
        }

        public ZoomState Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                _camera.Distance = CameraDistance * ZoomFactor;
            }
        }

        public void ZoomOut()
        {
            switch (_zoom)
            {
                case ZoomState.VeryNear:
                    _zoom = ZoomState.Near;
                    break;
                case ZoomState.Near:
                    _zoom = ZoomState.Middle;
                    break;
                case ZoomState.Middle:
                    _zoom = ZoomState.Far;
                    break;
                case ZoomState.Far:
                    _zoom = ZoomState.VeryFar;
                    break;
            }

            _camera.Distance = CameraDistance * ZoomFactor;
        }

        public void ZoomIn()
        {
            switch (_zoom)
            {
                case ZoomState.Near:
                    _zoom = ZoomState.VeryNear;
                    break;
                case ZoomState.Middle:
                    _zoom = ZoomState.Near;
                    break;
                case ZoomState.Far:
                    _zoom = ZoomState.Middle;
                    break;
                case ZoomState.VeryFar:
                    _zoom = ZoomState.Far;
                    break;
            }

            _camera.Distance = CameraDistance * ZoomFactor;
        }
        #endregion zoom handling

        public MapData Map
        {
            set
            {
                _mapRenderer.Map = value;
                _map = value;
            }
            get
            {
                return _map;
            }
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            Manager.GraphicsDevice.Viewport = controlViewport;
            _mapRenderer.Draw(gameTime, _camera);

            Manager.GraphicsDevice.Viewport = defaultViewport;
        }

        public override System.Collections.Generic.List<Instance.GameNotification> NotificationInterests
        {
            get
            {
                return new List<GameNotification> 
                { 
                    GameNotification.UpdateImprovements,
                    GameNotification.UpdateSpotting,
                    GameNotification.UpdateMapControlling
                };
            }
        }

        public override void HandleNotification(INotification notification)
        {
            switch ((GameNotification)System.Enum.Parse(typeof(GameNotification), notification.Name))
            {
                case GameNotification.UpdateImprovements:
                    _mapRenderer.UpdateImprovements();
                    break;
                case GameNotification.UpdateSpotting:
                    {
                        MapSpottingArgs args = notification.Body as MapSpottingArgs;
                        _mapRenderer.OnMapSpotting(args);
                    }
                    break;
                case GameNotification.UpdateMapControlling:
                    {
                        MapControllingArgs args = notification.Body as MapControllingArgs;
                        _mapRenderer.OnUpdateBorders(args);
                    }
                    break;
            }
        }
    }
}
