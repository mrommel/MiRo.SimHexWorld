﻿using MiRo.SimHexWorld.Engine.World;
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
using MiRo.SimHexWorld.Engine.UI.Controls.Helper;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class GameMapBox : GameControl
    {
        Effect _effect;

        Texture2D _cloudMap;
        Model _skyDome;

        MapData _map;
        readonly MapRenderer _mapRenderer;

        private Zooming zooming;
        public static ArcBallCamera _camera;
        const float CameraDistance = 50;

        public event MapUpdateHandler FocusChanged;
        public event CityOpenHandler CityOpened;
        public event CityOpenHandler CitySelected;
        public event UnitSelectHandler HumanUnitsSelected;
        public event UnitSelectHandler EnemyUnitsSelected;
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

            // zooming
            zooming = new Zooming("Middle", "VeryFar", 2f, "Far", 1.3f, "Middle", 1.0f, "Near", 0.7f, "VeryNear", 0.5f, "Detail", 0.2f);
            zooming.ZoomChanged += delegate(float zoom) { _camera.Distance = CameraDistance * zoom; };

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
            controlViewport.X = 4;
            controlViewport.Y = 27;
            controlViewport.Width = Manager.GraphicsDevice.Viewport.Width - 8;
            controlViewport.Height = Manager.GraphicsDevice.Viewport.Height - 32;
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();
            HandleMouse();

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
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.F) && !_oldKeyState.IsKeyDown(Keys.F))
                _mapRenderer.FogOfWarEnabled = !_mapRenderer.FogOfWarEnabled;

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

                        CenterAt(new HexPoint(loc.X,loc.Y));
                    }
                }
            }

            if (keyState.IsKeyDown(Keys.P) && !_oldKeyState.IsKeyDown(Keys.P))
            {
                ScoreDialog.Show(Manager, MainWindow.Game.Scores, "Scores");
            }

            _oldKeyState = keyState;
        }

        private void HandleMouse()
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue > _oldMouseState.ScrollWheelValue)
                ZoomIn();
            else if (mouseState.ScrollWheelValue < _oldMouseState.ScrollWheelValue)
                ZoomOut();

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
                    Unit unit = MainWindow.Game.GetUnitAt(_dragStart);

                    if (unit != null)
                    {
                        unit.Action = UnitAction.Move;
                        unit.SetTarget(_mapRenderer.Cursor);
                    }
                }
                else
                {
                    Unit unit = MainWindow.Game.GetUnitAt(_mapRenderer.Cursor);

                    // select unit
                    if (unit != null)
                    {
                        if (unit.Player.IsHuman && HumanUnitsSelected != null)
                            HumanUnitsSelected(unit);
                        else if (!unit.Player.IsHuman && EnemyUnitsSelected != null)
                            EnemyUnitsSelected(unit);
                    }
                    else if (UnitsUnselected != null)
                        UnitsUnselected();
                }
            }
          
            _oldMouseState = mouseState;
        }

        public void CenterAt(HexPoint loc)
        {
            if (Map != null && Map.IsValid(loc))
            {
                _mapCenter.X = loc.X;
                _mapCenter.Y = loc.Y;
                _camera.Target = MapData.GetWorldPosition(_mapCenter);
                _mapRenderer.Center = _mapCenter;
            }
        }

        public void MoveCenter(int dx, int dy)
        {
            CenterAt(_mapCenter + new HexPoint( dx, dy));
        }

        private void UpdateCursor()
        {
            // Get the new keyboard and mouse state
            MouseState mouseState = Mouse.GetState();

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
        public string Zoom
        {
            get 
            {
                return zooming.CurrentZoomName;
            }
            set
            {
                zooming.CurrentZoomName = value;
            }
        }

        public void ZoomOut()
        {
            zooming.ZoomOut();
        }

        public void ZoomIn()
        {
            zooming.ZoomIn();
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
                    GameNotification.UpdateMapControlling,
                    GameNotification.UpdateResources
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
                case GameNotification.UpdateResources:
                    {
                        _mapRenderer.OnMapUpdateResources();
                    }
                    break;
            }
        }
    }
}
