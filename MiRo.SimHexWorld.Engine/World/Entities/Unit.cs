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

        PythonEngine _behaviour;
        UnitData _data;

        ModelEntity _entity;

        private UnitAI _unitAI;
        private UnitAction _action = UnitAction.Idle;
        private Path _path = null;

        public event UnitMovedHandler Moved;

        List<UnitActionTransition> _transitions;

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
        }

        public Path Path
        {
            get
            {
                return _path;
            }
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

        public void MoveTarget(HexPoint target)
        {
            _path = Map.FindPath(this, target);
            //_path = Path.Create(Point, target);
        }

        public void Move(HexPoint target)
        {
            if (Moved != null)
                Moved(this, Point, target);

            _point = target;

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

        public void Update(GameTime time)
        {
            if (_deleted)
                return;

            if( !_player.IsHuman )
                UpdateAI();

            UpdateWork();

            _entity.Update(time);
        }

        private void UpdateWork()
        {
            if (_currentWork != null)
            {
                _currentWorkProgress++;
            }
        }

        public void Draw(GameTime time)
        {
            if (_deleted)
                return;
                
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
