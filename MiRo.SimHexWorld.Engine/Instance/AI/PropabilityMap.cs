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
        public List<T> items = new List<T>();
        public List<float> propabilities = new List<float>();

        public PropabilityMap()
        {
        }

        public void AddItem(T item, float propability)
        {
            items.Add(item);

            // limit propability
            if (propability < 0f)
                propability = 0f;

            propabilities.Add(propability);
        }

        public T Random
        {
            get
            {
                float sum = propabilities.Sum();

                float rndVal = (float)rnd.NextDouble() * sum;

                for (int i = 0; i < items.Count; ++i)
                {
                    if (rndVal <= propabilities[i])
                        return items[i];

                    rndVal -= propabilities[i];
                }

                return default(T);
            }
        }
    }
}
