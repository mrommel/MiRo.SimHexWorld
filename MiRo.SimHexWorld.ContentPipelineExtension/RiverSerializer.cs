using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using MiRo.SimHexWorld.Engine.World.Maps;
using Microsoft.Xna.Framework.Content;
using System.Xml;

namespace MiRo.SimHexWorld.ContentPipelineExtension
{
    [ContentTypeSerializer]
    class RiverSerializer : ContentTypeSerializer<River>
    {
        protected override void Serialize(IntermediateWriter output, River value, ContentSerializerAttribute format)
        {
            output.Xml.WriteStartElement("Name");
            output.Xml.WriteString(value.Name);
            output.Xml.WriteEndElement();

            output.Xml.WriteStartElement("Points");
            output.Xml.WriteAttributeString("Count", value.Points.Count.ToString());

            foreach (FlowPoint fp in value.Points)
            {
                int x = fp.Point.X;
                int y = fp.Point.Y;
                string flow = fp.Flow.ToString();

                output.Xml.WriteStartElement("Item");
                output.Xml.WriteString(string.Format("{0} {1} {2}", x, y, flow));
                output.Xml.WriteEndElement();
            }

            output.Xml.WriteEndElement();
        }

        /// <summary>
        /// Count occurrences of strings.
        /// </summary>
        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        private int ReadAttributeInt(XmlReader reader, string attributeName)
        {
            reader.MoveToAttribute(attributeName);
            return int.Parse(reader.Value);
        }

        protected override River Deserialize(IntermediateReader input, ContentSerializerAttribute format, River existingInstance)
        {
            River river = existingInstance ?? new River();

            input.ReadObject(nameFormat, river.Name);

            river.Points = new List<FlowPoint>();

            //input.Xml.ReadStartElement("Points");
            ReadToNextElement(input.Xml);

            //string innerXml = input.Xml.ReadInnerXml();
            //int pts = CountStringOccurrences(innerXml, "Item");
            int pts =  ReadAttributeInt(input.Xml, "Count");

            //while( input.Xml.Name == "Item" )
            for( int i = 0; i < pts; i++ )
            {
                //input.Xml.ReadStartElement();
                ReadToNextElement(input.Xml);
                string str = input.Xml.ReadString();

                string[] parts = str.Split(' ');

                if (parts.Length != 3)
                    throw new Exception("Wrong number of parts: " + parts.Length);

                FlowPoint fp = new FlowPoint();
                fp.Point = new HexPoint(int.Parse(parts[0]), int.Parse(parts[1]));
                switch (parts[2])
                {
                    case "S":
                        fp.Flow = MapCell.FlowDirectionType.South;
                        break;
                    case "SW":
                        fp.Flow = MapCell.FlowDirectionType.SouthWest;
                        break;
                    case "SE":
                        fp.Flow = MapCell.FlowDirectionType.SouthEast;
                        break;
                    default:
                        fp.Flow = (MapCell.FlowDirectionType)Enum.Parse(typeof(MapCell.FlowDirectionType), parts[2]);
                        break;
                }

                river.Points.Add(fp);

                input.Xml.ReadEndElement();
            }

            input.Xml.ReadEndElement();

            return river;
        }

        static ContentSerializerAttribute nameFormat = new ContentSerializerAttribute()
        {
            ElementName = "Name"
        };

        static ContentSerializerAttribute pointsFormat = new ContentSerializerAttribute()
        {
            ElementName = "Points"
        };

        private void ReadToNextElement(XmlReader reader)
        {
            reader.Read();

            while (reader.NodeType != System.Xml.XmlNodeType.Element)
            {
                if (!reader.Read())
                {
                    return;
                }
            }
        }

        private void ReadToEndElement(XmlReader reader)
        {
            reader.Read();

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.Read();
            }
        }
    }
}
