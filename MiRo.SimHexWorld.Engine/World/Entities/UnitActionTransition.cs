using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public delegate bool UnitFunction();
    public delegate void UnitProcedure();

    public class UnitActionTransition
    {
        UnitAI _ai;
        UnitAction _from;
        UnitAction _to;
        UnitFunction _precondition;
        double _propability;

        public UnitActionTransition(UnitAI ai, UnitAction from, UnitAction to, UnitFunction precondition, double propability = 1.0)
        {
            _ai = ai;
            _from = from;
            _to = to;
            _precondition = precondition;
            _propability = propability;
        }

        public UnitAI UnitAI
        {
            get
            {
                return _ai;
            }
        }

        public UnitAction From
        {
            get
            {
                return _from;
            }
        }
        public UnitAction To
        {
            get
            {
                return _to;
            }
        }

        public UnitFunction Precondition
        {
            get
            {
                return _precondition;
            }
        }

        public double Propability
        {
            get
            {
                return _propability;
            }
        }
    }
}
