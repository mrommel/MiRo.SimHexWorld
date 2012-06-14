using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using MiRo.SimHexWorld.Engine.Misc;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class WindowSet
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ControlSet Controls { get; set; }

        public void Init(GameWindow gameWindow, Manager manager)
        {
            gameWindow.Text = Title;
            gameWindow.Width = Width;
            gameWindow.Height = Height;

            Controls.Init(gameWindow, manager);
        }
    }

    public class ControlSet : List<ControlItem>
    {
        public void Init(Window parent, Manager manager)
        {
            foreach (ControlItem item in this)
                item.Init(parent, manager);
        }
    }

    public class AtlasAsset
    {
        public string Atlas { get; set; }
        public string Name { get; set; }
    }

    public class ControlItem
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [ContentSerializer(Optional = true)]
        public string Parent { get; set; }

        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [ContentSerializer(Optional = true)]
        public AtlasAsset AtlasAsset { get; set; }

        [ContentSerializer(Optional = true)]
        public string ImageAsset { get; set; }

        [ContentSerializer(Optional = true)]
        public SizeMode ImageMode { get; set; }

        [ContentSerializer(Optional = true)]
        public string Text { get; set; }

        [ContentSerializer(Optional = true)]
        public string Click { get; set; }

        [ContentSerializerIgnore]
        MethodInfo EventFieldClick;

        [ContentSerializer(Optional = true)]
        public string Draw { get; set; }

        [ContentSerializerIgnore]
        MethodInfo EventFieldDraw;

        public void Init(Window parent, Manager manager)
        {
            switch (Type)
            {
                case "Container":
                    Container container = new Container(manager);
                    container.Init();

                    if( !string.IsNullOrEmpty(Parent))
                        container.Parent = parent.GetControl(Parent);
              
                    container.Name = Name;
                    container.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    container.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    container.Width = Width;
                    container.Height = Height;

                    container.BackColor = Color.Cyan;
                    container.Color = Color.Cornsilk;

                    if (string.IsNullOrEmpty(Parent))
                        parent.Add(container);
                    break;
                case "ImageBox":
                    ImageBox box = new ImageBox(manager);
                    box.Init();

                    if (!string.IsNullOrEmpty(Parent))
                        box.Parent = parent.GetControl(Parent);

                    box.Name = Name;
                    box.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    box.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    box.Width = Width;
                    box.Height = Height;

                    if (!string.IsNullOrEmpty(ImageAsset))
                        box.Image = manager.Content.Load<Texture2D>(ImageAsset);
                    else if (AtlasAsset != null)
                        box.Image = Provider.GetAtlas(AtlasAsset.Atlas).GetTexture(AtlasAsset.Name);

                    box.SizeMode = ImageMode;

                    if (!string.IsNullOrEmpty(Click))
                        box.Click += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                        {
                            if (EventFieldClick == null)
                            {
                                Type classType = parent.GetType();
                                EventFieldClick = classType.GetMethod(Click);
                            }

                            if (EventFieldClick == null)
                                throw new Exception("Could not find: " + Draw + " method");

                            EventFieldClick.Invoke(parent, new object[] { sender, e });
                        };

                    if (!string.IsNullOrEmpty(Draw))
                        box.Draw += delegate(object sender, DrawEventArgs e)
                        {
                            if (EventFieldDraw == null)
                            {
                                Type classType = parent.GetType();
                                EventFieldDraw = classType.GetMethod(Draw);
                            }

                            if (EventFieldDraw == null)
                                throw new Exception("Could not find: " + Draw + " method");

                            EventFieldDraw.Invoke(parent, new object[] { sender, e });
                        };

                    if (string.IsNullOrEmpty(Parent))
                        parent.Add(box);
                    break;
                case "Label":
                    Label label = new Label(manager);
                    label.Init();

                    if (!string.IsNullOrEmpty(Parent))
                        label.Parent = parent.GetControl(Parent);

                    label.Name = Name;
                    label.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    label.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    label.Width = Width;
                    label.Height = Height;

                    label.Text = Text;
                    label.TextColor = Color.Black;

                    if (string.IsNullOrEmpty(Parent))
                        parent.Add(label);
                    break;
                default:
                    throw new Exception("No handling for " + Type);
            }
        }

        //static TomShane.Neoforce.Controls.EventHandler GetEventHandler(object classInstance, string eventName)
        //{
        //    Type classType = classInstance.GetType();
        //    MethodInfo eventField = classType.GetMethod(eventName);
        //    eventField.Invoke(classInstance,
        //    //ParameterInfo[] a = eventField.GetParameters();
        //    //TomShane.Neoforce.Controls.EventHandler eventDelegate = eventField.Invoke(
        //    classInstance.
        //    return (TomShane.Neoforce.Controls.EventHandler)eventField.;
        //}

    }
}
