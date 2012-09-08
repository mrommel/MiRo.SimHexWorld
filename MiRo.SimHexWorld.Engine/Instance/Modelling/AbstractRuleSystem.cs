using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance.Modelling
{
    public abstract class AbstractRuleSystem
    {
        protected RuleType _type;
        protected ModellingTile _parent;
        protected double _currentValue;
        protected double _targetValue;
        protected String _description = "";

        protected List<RuleType> _effects = new List<RuleType>();

        public AbstractRuleSystem(ModellingTile parent, RuleType type, double currentValue)
        {
            _parent = parent;
            _type = type;
            _currentValue = currentValue;
        }

        public abstract void UpdateTarget();

        public double UpdateCurrent()
        {
            _currentValue = Lerp(_currentValue, _targetValue, 0.8);
            return _currentValue;
        }

        public double Current
        {
            get { return _currentValue; }
            set { _currentValue = value; }
        }

        public List<RuleType> Effects
        {
            get { return _effects; }
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value > max)
                return max;

            if (value < min)
                return min;

            return value;
        }

        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }
    }
}
