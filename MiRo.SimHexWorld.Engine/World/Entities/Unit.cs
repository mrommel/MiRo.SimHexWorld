using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.AI;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.UI;
using System.Windows.Forms;
using MiRo.SimHexWorld.Engine.World.Helper;
using MiRo.SimHexWorld.Engine.UI.Entities;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using MiRoSimHexWorld.Engine.World;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.UI.Controls;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public interface IScriptable
    {
        void ScriptCallback(string message, object body); 
    }

    // events
    public delegate void UnitMovedHandler(Unit sender, HexPoint oldPosition, HexPoint newPosition);

    public partial class Unit //: IScriptable
    {
        AbstractPlayerData _player;
        HexPoint _point;

        //PythonEngine _behaviour;
        UnitData _data;

        ModelEntity _entity;

        private UnitAI _unitAI;
        private UnitAction _action = UnitAction.Idle;
        private WayPoints _path = null;

        public event UnitMovedHandler Moved;

        List<UnitActionTransition> _transitions;

        BillboardSystem<UnitAction> _unitActionBillboard;

        private bool _deleted;

        public Unit(HexPoint point, AbstractPlayerData player, UnitData data)
        {
            _point = point;
            _player = player;
            _data = data;

            _unitAI = _data.DefaultUnitAI;

            _entity = new ModelEntity(player, this, _data.ModelName);
            _entity.Point = point;
            _entity.Scale = new Vector3(_data.ModelScale);

            UpdateSpotting();

            InitTransitions();

            _unitActionBillboard = new BillboardSystem<UnitAction>(MainApplication.Instance.GraphicsDevice, MainApplication.Instance.Content);
            _unitActionBillboard.AddEntity(UnitAction.Idle, Provider.GetAtlas("UnitActionAtlas").GetTexture("Idle"), new Vector2(2, 2));
            _unitActionBillboard.AddEntity(UnitAction.Move, Provider.GetAtlas("UnitActionAtlas").GetTexture("Move"), new Vector2(2, 2));
            _unitActionBillboard.AddEntity(UnitAction.BuildFarm, Provider.GetAtlas("UnitActionAtlas").GetTexture("BuildFarm"), new Vector2(2, 2));
            _unitActionBillboard.AddEntity(UnitAction.BuildRoad, Provider.GetAtlas("UnitActionAtlas").GetTexture("BuildRoad"), new Vector2(2, 2));
            _unitActionBillboard.AddEntity(UnitAction.Found, Provider.GetAtlas("UnitActionAtlas").GetTexture("Found"), new Vector2(2, 2));

            UpdateUnitAction();
        }

        public WayPoints Path
        {
            get
            {
                return _path;
            }
        }

        public void UpdateUnitAction()
        {
            _unitActionBillboard.ResetPositions();
            _unitActionBillboard.AddPosition(_action, MapData.GetWorldPosition(Point) + new Vector3(0, 5, 0));
            _unitActionBillboard.Build();
        }

        protected void UpdateSpotting()
        {
            if (Point != null && MainWindow.Game.Map != null)
            {
                MainWindow.Game.Map[Point].SetSpotted(_player, true);

                foreach (HexPoint pt in Point.Neighbors)
                    if( MainWindow.Game.Map.IsValid(pt))
                        MainWindow.Game.Map[pt].SetSpotted(_player, true);
            }
        }

        public MapData Map
        {
            get
            {
                return MainWindow.Game.Map;
            }
        }

        public void SetTarget(HexPoint target)
        {
            _path = WayPoints.FromPath( Map.FindPath(this, target) );

            Assert.AreEqual(Point, _path.Peek);
            Assert.AreEqual(target, _path.Goal);

            // remove the current position
            _path.Pop();
        }

        public void Move(HexPoint target)
        {
            if (Moved != null)
                Moved(this, Point, target);

            _point = target;

            if (_path.Finished)
                _path = null;

            UpdateSpotting();
        }

        public List<HexPoint> TilesInSight
        {
            get
            {
                return Point.GetNeighborhood(Data.Sight);               
            }
        }

        public List<HexPoint> GetTilesOfInterest( int sight )
        {
            return Point.GetNeighborhood(sight);
        }

        public bool Deleted
        {
            get
            {
                return _deleted;
            }
        }

        public UnitData Data
        {
            get { return _data; }
        }

        public HexPoint Point
        {
            get { return _point; }
        }

        public AbstractPlayerData Player
        {
            get { return _player; }
        }

        TimeSpan updateEvery = TimeSpan.FromSeconds(1); // from setting ???
        TimeSpan currentTime = TimeSpan.FromSeconds(0);
        public void Update(GameTime time)
        {
            if (_deleted)
                return;

            currentTime -= time.ElapsedGameTime;
            if (currentTime.TotalMilliseconds <= 0)
            {
                if( !_player.IsHuman || _action == UnitAction.Idle )
                    UpdateAI();

                UpdateWork();
                // reset timer
                currentTime = updateEvery;

                _entity.Update(time);
            }
        }

        private void UpdateWork()
        {
            if (_currentWork != null)
            {
                _currentWorkProgress++;

                if (_currentWork.Cost < _currentWorkProgress)
                {
                    Map[Point].Improvements.Add(_currentWork);

                    if (WorkFinished != null)
                        WorkFinished(this, Point, _currentWork);

                    _currentWork = null;
                    _currentWorkProgress = 0;
                }
            }
        }

        public void Draw(GameTime time)
        {
            if (_deleted)
                return;

            _unitActionBillboard.Draw(GameMapBox.Camera.View, GameMapBox.Camera.Projection, GameMapBox.Camera.Position, GameMapBox.Camera.Up, GameMapBox.Camera.Right);

            _entity.Draw(time);
        }

        public override string ToString()
        {
            if (_deleted)
                return "deleted";

            return "[Unit: " + Data.Name + " at " + Point + "]";
        }
    }
}
