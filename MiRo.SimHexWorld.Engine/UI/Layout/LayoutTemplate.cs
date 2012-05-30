using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MS.Internal.Xml.XPath;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using System.Reflection;

namespace MiRo.SimHexWorld.Engine.UI.Layout
{
    public class LayoutTemplate
    {
        /// <summary>
        /// class that hoĺds the general information about the layout
        /// </summary>
        public class InfoTemplate
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string Version { get; set; }
        }

        /// <summary>
        /// class that holds the information about the controls
        /// </summary>
        public class ControlTemplate
        {
            public static ControlTemplate FromControl(Control control)
            {
                ControlTemplate ct = new ControlTemplate();

                ct.Name = control.Name;
                ct.Class = control.ToString();
                ct.Properties = new List<PropertyTemplate>
                                             {
                                                 new PropertyTemplate("Left", control.Left),
                                                 new PropertyTemplate("Top", control.Top),
                                                 new PropertyTemplate("MinimumWidth", control.MinimumWidth),
                                                 new PropertyTemplate("Height", control.Height),
                                                 new PropertyTemplate("Width", control.Width),
                                                 new PropertyTemplate("Anchor", (int) control.Anchor),                                                                             
                                             };

                if (control.Text != null)
                    ct.Properties.Add(new PropertyTemplate("Text", control.Text));

                if (control.StayOnBack)
                    ct.Properties.Add(new PropertyTemplate("StayOnBack", control.StayOnBack));

                if (control.Passive)
                    ct.Properties.Add(new PropertyTemplate("Passive", control.Passive));

                if (control is Label)
                    ct.Properties.Add(new PropertyTemplate("Alignment", (int) (control as Label).Alignment));

                //if (control is ImageBox)
                //    ct.Properties.Add(new PropertyTemplate("Image", (control as ImageBox).Image));

                ct.Controls = new List<ControlTemplate>();

                foreach (var c in control.Controls)
                    ct.Controls.Add(FromControl(c));

                return ct;
            }
            public string Name { get; set; }
            public string Class { get; set; }

            [ContentSerializer(Optional = true)]
            public Orientation Orientation { get; set; }

            [ContentSerializer(ElementName = "Properties", CollectionItemName = "Property", Optional = true)]
            public List<PropertyTemplate> Properties { get; set; }

            [ContentSerializer(ElementName = "Controls", CollectionItemName = "Control", Optional = true)]
            public List<ControlTemplate> Controls { get; set; }
        }

        /// <summary>
        /// class the holds one property information of controls
        /// </summary>
        public class PropertyTemplate
        {
            public string Name { get; set; }
            public string Value { get; set; }

            //[ContentSerializer(Optional = true)]
            //private Texture2D TextureValue { get; set; }

            public PropertyTemplate()
            {
            }

            public PropertyTemplate(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public PropertyTemplate(string name, int value)
            {
                Name = name;
                Value = value.ToString();
            }

            public PropertyTemplate(string name, bool value)
            {
                Name = name;
                Value = value.ToString();
            }

            //public PropertyTemplate(string name, Texture2D texture2D)
            //{
            //    Name = name;
            //    TextureValue = texture2D;
            //}
        }

        public InfoTemplate Info { get; set; }

        [ContentSerializer(ElementName = "Controls", CollectionItemName = "Control", Optional = true)]
        public List<ControlTemplate> Controls = new List<ControlTemplate>();

        public Container CreateContainer(Manager manager)
        {
            Container window = null;

            ControlTemplate c = Controls.First();

            Type type = null;

            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            foreach (var assembly in assemblies)
            {
                // try to load directly from full name
                type = assembly.GetType(c.Class);

                if (type != null)
                    break;

                // load from short name (should not happen)
                type = assembly.GetType("TomShane.Neoforce.Controls." + c.Class);

                if (type != null)
                    break;
            }

            if (type == null)
                throw new Exception("Cannot load '" + c.Class + "'");

            window = (Container)LoadControl(manager, c, type, null);
            window.Name = Info.Name;

            return window;
        }

        ////////////////////////////////////////////////////////////////////////////
        private static Control LoadControl(Manager manager, ControlTemplate controlTemplate, Type type, Control parent)
        {
            Control c = null;

            Object[] args;

            if (controlTemplate.Class.StartsWith("TomShane.Neoforce.Controls.ScrollBar"))
                args = new Object[] { manager, controlTemplate.Orientation };
            else
                args = new Object[] { manager };

            c = (Control)type.InvokeMember(null, BindingFlags.CreateInstance, null, null, args);
            c.Init();

            if (parent != null) 
                c.Parent = parent;

            c.Name = controlTemplate.Name;

            if (controlTemplate.Properties.Count >= 0)
            {
                LoadProperties(controlTemplate.Properties, c);
            }

            foreach (ControlTemplate controlTemplateChild in controlTemplate.Controls)
            {
                Type t = Type.GetType(controlTemplateChild.Class);

                if (t == null)
                {
                    List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

                    foreach (var assembly in assemblies)
                    {
                        // try to load directly from full name
                        t = assembly.GetType(controlTemplateChild.Class);

                        if (t != null)
                            break;

                        // load from short name (should not happen)
                        t = assembly.GetType("TomShane.Neoforce.Controls." + controlTemplateChild.Class);

                        if (t != null)
                            break;
                    }
                }

                if (t == null)
                    throw new Exception("Cannot load '" + controlTemplateChild.Class + "'");

                LoadControl(manager, controlTemplateChild, t, c);
            }

            return c;
        }

        private static void LoadProperties(IEnumerable<PropertyTemplate> list, Control c)
        {
            foreach (PropertyTemplate e in list)
            {
                string name = e.Name;
                string val = e.Value;

                PropertyInfo i = c.GetType().GetProperty(name);

                if (i != null)
                {
                    {
                        try
                        {
                            i.SetValue(c, Convert.ChangeType(val, i.PropertyType, null), null);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        private static LayoutTemplate CreateLayoutTemplate(Container container, string name, string description = "", string author = "")
        {
            LayoutTemplate lt = new LayoutTemplate();

            InfoTemplate it = new InfoTemplate();

            it.Name = name;
            it.Author = author;
            it.Description = description;
            it.Version = "0.7";

            lt.Info = it;

            lt.Controls = new List<ControlTemplate> { ControlTemplate.FromControl(container) };

            return lt;
        }

        public static void SaveLayoutTemplate(string filename, Container container, string name, string description = "", string author = "")
        {
            LayoutTemplate lt = CreateLayoutTemplate(container, name, description, author);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize(writer, lt, null);
            }
        }
    }
}
