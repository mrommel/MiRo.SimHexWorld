using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;
using MiRo.SimHexWorld.World.Maps;

namespace MiRo.SimHexWorld.World.Maps
{
    public class NamedList<T> : List<T> where T : INamed
    {
        public T this[string name]
        {
            get
            {
                return this.FirstOrDefault(a => a.Name == name);
            }
        }

        static Random rnd = new Random();

        [XmlIgnore()]
        [ContentSerializerIgnore]
        public T RandomItem
        {
            get
            {
                return this[rnd.Next() % Count];
            }
        }

        public bool IsSubset(IList<T> test)
        {
            bool subset = true;

            foreach (T t in test)
                if (!Contains(t))
                    subset = false;

            return subset;
        }
    }
}
