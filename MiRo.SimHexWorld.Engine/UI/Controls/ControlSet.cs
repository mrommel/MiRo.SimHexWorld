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
using MiRo.SimHexWorld.Engine.Locales;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class WindowSet
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ControlSet Controls { get; set; }

        public void Init(GameWindow gameWindow, Manager manager)
        {
            gameWindow.Text = Title;
            gameWindow.Name = Name;
            gameWindow.Icon = manager.Content.Load<Texture2D>(Icon);
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
            {
                item.Init(parent, manager);
            }
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
        public bool StayOnBack { get; set; }

        [ContentSerializer(Optional = true)]
        public bool StayOnTop { get; set; }

        private Color Color { get; set; }

        [ContentSerializer(Optional = true)]
        public string ColorName
        {
            get { return ColorConversion.FromColor(Color); }
            set 
            {
                if( value != null )
                    Color = ColorConversion.FromName(value); 
            }
        }

        private Color BackColor { get; set; }

        [ContentSerializer(Optional = true)]
        public string BackColorName
        {
            get { return ColorConversion.FromColor(BackColor); }
            set 
            { 
                if (value != null)                
                    BackColor = ColorConversion.FromName(value); 
            }
        }

        private Color TextColor { get; set; }

        [ContentSerializer(Optional = true)]
        public string TextColorName
        {
            get { return ColorConversion.FromColor(TextColor); }
            set 
            {
                if (value != null)
                    TextColor = ColorConversion.FromName(value); 
            }
        }

        [ContentSerializer(Optional = true)]
        public AtlasAsset AtlasAsset { get; set; }

        [ContentSerializer(Optional = true)]
        public string ImageAsset { get; set; }

        [ContentSerializer(Optional = true)]
        public SizeMode ImageMode { get; set; }

        [ContentSerializer(Optional = true)]
        public string Text { get; set; }

        #region events

        [ContentSerializer(Optional = true)]
        public string Click { get; set; }

        [ContentSerializer(Optional = true)]
        public string FocusChanged { get; set; }

        [ContentSerializer(Optional = true)]
        public string CityOpened { get; set; }

        [ContentSerializer(Optional = true)]
        public string CitySelected { get; set; }

        [ContentSerializer(Optional = true)]
        public string HumanUnitSelected { get; set; }

        [ContentSerializer(Optional = true)]
        public string UnitUnselected { get; set; }

        #endregion events

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

                    if (!string.IsNullOrEmpty(Parent))
                        container.Parent = parent.GetControl(Parent);

                    container.Name = Name;
                    container.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    container.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    container.Width = Width;
                    container.Height = Height;

                    container.StayOnBack = StayOnBack;
                    container.StayOnTop = StayOnTop;

                    if (TextColor != Color.Transparent)
                        container.TextColor = TextColor;

                    if (BackColor != Color.Transparent)
                        container.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        container.Color = Color;

                    container.AutoScroll = true;

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

                    box.StayOnBack = StayOnBack;
                    box.StayOnTop = StayOnTop;

                    if (BackColor != Color.Transparent)
                        box.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        box.Color = Color;

                    if (TextColor != Color.Transparent)
                        box.TextColor = TextColor;

                    if (!string.IsNullOrEmpty(ImageAsset))
                        box.Image = manager.Content.Load<Texture2D>(ImageAsset);
                    else if (AtlasAsset != null)
                        box.Image = Provider.GetAtlas(AtlasAsset.Atlas).GetTexture(AtlasAsset.Name);

                    box.SizeMode = ImageMode;

                    box.Tag = this;
                    //if (!string.IsNullOrEmpty(Click))
                    //    box.Click += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                    //    {
                    //        //if (EventFieldClick == null)
                    //        //{
                    //        //    Type classType = parent.GetType();
                    //        //    EventFieldClick = classType.GetMethod(Click);
                    //        //}

                    //        //if (EventFieldClick == null)
                    //        //    throw new Exception("Could not find: " + Draw + " method");

                    //        //EventFieldClick.Invoke(parent, new object[] { sender, e });
                    //    };

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

                    label.StayOnBack = StayOnBack;
                    label.StayOnTop = StayOnTop;

                    if (BackColor != Color.Transparent)
                        label.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        label.Color = Color;

                    if (TextColor != Color.Transparent)
                        label.TextColor = TextColor;
                    else
                        label.TextColor = Color.Black;

                    label.Text = Text.StartsWith("TXT_KEY_") ? Provider.Instance.Translate(Text) : Text;

                    if (string.IsNullOrEmpty(Parent))
                        parent.Add(label);
                    break;
                case "GameMapBox":
                    GameMapBox mapBox = new GameMapBox(manager);
                    mapBox.Init();

                    if (!string.IsNullOrEmpty(Parent))
                        mapBox.Parent = parent.GetControl(Parent);

                    mapBox.Name = Name;
                    mapBox.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    mapBox.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    mapBox.Width = Width;
                    mapBox.Height = Height;

                    mapBox.StayOnBack = StayOnBack;
                    mapBox.StayOnTop = StayOnTop;

                    mapBox.Tag = this;

                    if (BackColor != Color.Transparent)
                        mapBox.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        mapBox.Color = Color;

                    if (TextColor != Color.Transparent)
                        mapBox.TextColor = TextColor;
                    else
                        mapBox.TextColor = Color.Black;

                    if (string.IsNullOrEmpty(Parent))
                        parent.Add(mapBox);

                    break;
                default:
                    throw new Exception("No handling for " + Type);
            }
        }
    }
}
