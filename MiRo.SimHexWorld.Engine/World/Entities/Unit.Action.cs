using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.Instance.AI;
using MiRo.SimHexWorld.Engine.UI;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public delegate void UnitWorkHandler(Unit unit, HexPoint point, Improvement improvement);

    partial class Unit
    {
        static Improvement road, farm;
        static Random rnd = new Random();

        Improvement _currentWork = null;
        int _currentWorkProgress = 0;

        public event UnitWorkHandler WorkFinished;

        private void InitTransitions()
        {
            _transitions = new List<UnitActionTransition>
            {
                // Work
                new UnitActionTransition(UnitAI.Work, UnitAction.Idle, UnitAction.Idle, CanIdle, 0.5),

                new UnitActionTransition(UnitAI.Work, UnitAction.Idle, UnitAction.Move, CanMove),
                new UnitActionTransition(UnitAI.Work, UnitAction.Move, UnitAction.Idle, TargetReached),

                new UnitActionTransition(UnitAI.Work, UnitAction.Idle, UnitAction.BuildRoad, CanBuildRoad),
                new UnitActionTransition(UnitAI.Work, UnitAction.BuildRoad, UnitAction.Idle, IsWorkFinished),

                new UnitActionTransition(UnitAI.Work, UnitAction.Idle, UnitAction.BuildFarm, CanBuildFarm),
                new UnitActionTransition(UnitAI.Work, UnitAction.BuildFarm, UnitAction.Idle, IsWorkFinished),

                // Settle
                new UnitActionTransition(UnitAI.Settle, UnitAction.Idle, UnitAction.Idle, CanIdle, 0.5),

                new UnitActionTransition(UnitAI.Settle, UnitAction.Idle, UnitAction.Move, CanMove),
                new UnitActionTransition(UnitAI.Settle, UnitAction.Move, UnitAction.Idle, TargetReached),

                new UnitActionTransition(UnitAI.Settle, UnitAction.Idle, UnitAction.Found, CanFound, 4f),

                // Explore
                new UnitActionTransition(UnitAI.Explore, UnitAction.Idle, UnitAction.Idle, CanIdle, 0.5),

                new UnitActionTransition(UnitAI.Explore, UnitAction.Idle, UnitAction.Move, CanMove),
                new UnitActionTransition(UnitAI.Explore, UnitAction.Move, UnitAction.Idle, TargetReached),

                // Attack
                new UnitActionTransition(UnitAI.Attack, UnitAction.Idle, UnitAction.Idle, CanIdle, 0.5),

                new UnitActionTransition(UnitAI.Attack, UnitAction.Idle, UnitAction.Move, CanMove),
                new UnitActionTransition(UnitAI.Attack, UnitAction.Move, UnitAction.Idle, TargetReached),
            };
        }

        public List<UnitAction> Actions
        {
            get
            {
                List<UnitAction> actions = new List<UnitAction>();

                foreach (UnitActionTransition trans in _transitions)
                {
                    if (trans.UnitAI == UnitAI)
                    {
                        if( trans.From != UnitAction.Idle && trans.From != UnitAction.Move)
                            actions.Add(trans.From);

                        if (trans.To != UnitAction.Idle && trans.To != UnitAction.Move)
                            actions.Add(trans.To);
                    }
                }

                return actions;
            }
        }

        public UnitAI UnitAI
        {
            get
            {
                return _unitAI;
            }
        }

        public UnitAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public bool CanIdle()
        {
            return true;
        }

        public bool CanMove()
        {
            bool canEnterAnything = false;

            // are there tiles, where the unit can walk to?
            foreach (HexPoint pt in Point.Neighbors)
                canEnterAnything |= Map[pt].CanEnter(this);

            return canEnterAnything;
        }

        private static int _settlerInterest = 3;

        public void SelectTarget()
        {
            switch (_unitAI)
            {
                case Types.UnitAI.Work:
                    PropabilityMap<WayPoints> propsWork = new PropabilityMap<WayPoints>();

                    foreach (HexPoint pt in Point.Neighbors)
                    {
                        if (Map[pt].CanEnter(this))
                            propsWork.AddItem(WayPoints.FromPath(Map.FindPath(this, pt)), _player.ImprovementLocationMap[pt] * (_player.ImprovementLocationMap.IsLocalMaximum(pt) ? 1.0f : 0.2f));
                    }

                    _path = propsWork.Random;
                    break;
                case Types.UnitAI.Settle:
                    PropabilityMap<WayPoints> propsSettle = new PropabilityMap<WayPoints>();

                    List<HexPoint> interest = GetTilesOfInterest(_settlerInterest);
                    foreach (HexPoint pt in interest)
                    {
                        Path path = Map.FindPath(this, pt);
                        if (path != null && path.IsValid)
                        {
                            propsSettle.AddItem(WayPoints.FromPath( path ), _player.CityLocationMap[pt] * (_player.CityLocationMap.IsLocalMaximum(pt) ? 1.0f : 0.2f));
                        }
                    }

                    _path = propsSettle.Random;
                    break;
                case Types.UnitAI.Attack:
                case Types.UnitAI.Explore:
                    PropabilityMap<WayPoints> propsExplore = new PropabilityMap<WayPoints>();

                    foreach (HexPoint pt in Point.Neighbors)
                    {
                        if (Map[pt].CanEnter(this))
                            propsExplore.AddItem(WayPoints.FromPath(Map.FindPath(this, pt)), 1);
                    }

                    _path = propsExplore.Random;
                    break;
                default:
                    throw new Exception("SelectTarget not prepared " + _unitAI);
            }

            return;
        }

        public bool TargetReached()
        {
            if (_path == null)
                return true;

            return _path.Finished;
        }

        public bool IsWorkFinished()
        {
            return _currentWork == null;
        }

        public bool CanBuild(Improvement imp)
        {
            if (MainWindow.Game == null)
                return false;

            if (MainWindow.Game.Map == null)
                return false;

            if (Map[Point].Improvements.Contains(imp))
                return false;

            if (!imp.Terrains.Contains(Map[Point].Terrain))
                return false;

            if (Map[Point].Features.Count > 0)
            {
                bool hasFeature = false;

                foreach (Feature feature in Map[Point].Features)
                    hasFeature |= imp.FeatureNames.Contains(feature.Name);

                if (!hasFeature)
                    return false;
            }

            if (!string.IsNullOrEmpty(imp.RequiredTech))
            {
                if (!Player.Technologies.Contains(Provider.GetTech(imp.RequiredTech)))
                    return false;
            }

            // look at road map
            return true;
        }

        public bool CanBuildRoad()
        {
            if (road == null)
                road = Provider.GetImprovement("Road");

            return CanBuild(road);
        }

        public bool CanBuildFarm()
        {
            if (farm == null)
                farm = Provider.GetImprovement("Farm");

            return CanBuild(farm);
        }

        public bool CanFound()
        {
            if (MainWindow.Game == null)
                return false;

            if (MainWindow.Game.Map == null)
                return false;

            if (_player.CityLocationMap == null)
                return false;

            if( _player.CityLocationMap[Point] <= 0 )
                return false;

            return _player.CityLocationMap.IsLocalMaximum(Point);
        }

        public void Found()
        {
            if( MainWindow.Game.GetCityAt(Point) == null )
                _player.AddCity(Point);

            // unit need to be removed - later on (we are probably in a foreach)
            _deleted = true;
            return;
        }

        private void UpdateAI()
        {
            List<UnitActionTransition> possible = _transitions.Where(a => a.UnitAI == _unitAI && a.From == _action && a.Precondition()).ToList();

            if (possible.Count == 0)
                return;

            double sum = possible.Sum(a => a.Propability);

            double weighted = rnd.NextDouble() * sum;

            foreach (UnitActionTransition transition in possible)
            {
                if (weighted < transition.Propability)
                {
                    if (transition.To != _action)
                    {
                        _action = transition.To;

                        Execute(_action);
                        return;
                    }
                }

                weighted -= transition.Propability;
            }
        }

        public void Build(Improvement imp)
        {
            if (imp == null)
                throw new NullReferenceException();

            _currentWork = imp;
            _currentWorkProgress = 0;
        }

        public void Execute(UnitAction _action)
        {
            switch (_action)
            {
                case UnitAction.Idle:
                    break;
                case UnitAction.Found:
                    Found();
                    break;
                case UnitAction.Move:
                    SelectTarget();
                    break;
                case UnitAction.BuildRoad:
                    if (road == null)
                        road = Provider.GetImprovement("Road");

                    Build(road);
                    break;
                case UnitAction.BuildFarm:
                    if (farm == null)
                        farm = Provider.GetImprovement("Farm");

                    Build(farm);
                    break;
                default:
                    throw new Exception("No handle for " + _action); 
            }

            UpdateUnitAction();
        }

        public bool IsAutomated { get; set; }
    }
}
