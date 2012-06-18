using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.Instance.AI
{
    public class PropabilityMap<T> 
    {
        private static Random rnd = new Random();
        protected List<T> _items = new List<T>();
        protected List<float> _propabilities = new List<float>();

        public PropabilityMap()
        {
        }

        public void AddItem(T item, float propability)
        {
            _items.Add(item);

            // limit propability
            if (propability < 0f)
                propability = 0f;

            _propabilities.Add(propability);
        }

        public List<T> Items
        {
            get { return _items; }
        }

        public T Random
        {
            get
            {
                float sum = _propabilities.Sum();

                float rndVal = (float)rnd.NextDouble() * sum;

                for (int i = 0; i < _items.Count; ++i)
                {
                    if (rndVal <= _propabilities[i])
                        return _items[i];

                    rndVal -= _propabilities[i];
                }

                return default(T);
            }
        }

        public T Best
        {
            get
            {
                Sort();
                return _items.First();
            }
        }

        public T RandomOfBest3
        {
            get 
            {
                Sort();

                int index = rnd.Next(Math.Min(_items.Count, 3));

                return _items[index];
            }
        }

        private void Sort()
        {
            for (int i = 0; i < _items.Count; ++i)
            {
                for (int j = i; j < _items.Count; ++j)
                {
                    if (_propabilities[i] > _propabilities[j])
                    {
                        float tmpF = _propabilities[i];
                        _propabilities[i] = _propabilities[j];
                        _propabilities[j] = tmpF;

                        T tmpI = _items[i];
                        _items[i] = _items[j];
                        _items[j] = tmpI;
                    }
                }
            }

        }
    }
}
