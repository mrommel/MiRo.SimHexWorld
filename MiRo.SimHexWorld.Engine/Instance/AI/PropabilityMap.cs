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
        protected List<KeyValuePair<T, float>> _items = new List<KeyValuePair<T, float>>();

        public PropabilityMap()
        {
        }

        public void AddItem(T item, float propability)
        {
            // limit propability
            if (propability < 0f)
                propability = 0f;

            _items.Add(new KeyValuePair<T, float>(item, propability));
        }

        public List<KeyValuePair<T, float>> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public T Random
        {
            get
            {
                float sum = _items.Select( a => a.Value ).Sum();

                float rndVal = (float)rnd.NextDouble() * sum;

                for (int i = 0; i < _items.Count; ++i)
                {
                    if (rndVal <= _items[i].Value)
                        return _items[i].Key;

                    rndVal -= _items[i].Value;
                }

                return default(T);
            }
        }

        public T Best
        {
            get
            {
                Sort();
                return _items.First().Key;
            }
        }

        public T RandomOfBest3
        {
            get 
            {
                Sort();

                int index = rnd.Next(Math.Min(_items.Count, 3));

                return _items[index].Key;
            }
        }

        private void Sort()
        {
            for (int i = 0; i < _items.Count; ++i)
            {
                for (int j = i; j < _items.Count; ++j)
                {
                    if (_items[i].Value > _items[j].Value)
                    {
                        KeyValuePair<T, float> tmpF = _items[i];
                        _items[i] = _items[j];
                        _items[j] = tmpF;
                    }
                }
            }

        }
    }
}
