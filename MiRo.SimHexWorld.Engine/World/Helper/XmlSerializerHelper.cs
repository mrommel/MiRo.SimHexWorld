using System;
using System.IO;
using System.Xml.Serialization;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public class XmlSerializerHelper<T>
    {
        private readonly Type _type;

        public XmlSerializerHelper()
        {
            _type = typeof(T);
        }

        public void Save(string path, object obj, XmlAttributeOverrides overrides = null)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                XmlSerializer serializer = (overrides != null) ? new XmlSerializer(_type, overrides) : new XmlSerializer(_type);
                serializer.Serialize(textWriter, obj);
            }
        }

        public T Read(string path, XmlAttributeOverrides overrides = null)
        {
            T result;
            using (TextReader textReader = new StreamReader(path))
            {
                XmlSerializer deserializer = (overrides != null) ? new XmlSerializer(_type, overrides) : new XmlSerializer(_type);
                result = (T)deserializer.Deserialize(textReader);
            }
            return result;
        }
    }
}
