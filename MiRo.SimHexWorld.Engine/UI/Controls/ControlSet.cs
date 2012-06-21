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

    public class MenuItemEntry
    {
        public string Title { get; set; }
        public string Click { get; set; }
        public bool Enabled { get; set; }
    }

    public class ControlItem
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [ContentSerializer(Optional = true)]
        public string Parent { get; set; }

        [ContentSerializer(Optional = true)]
        public int Top { get; set; }

        [ContentSerializer(Optional = true)]
        public int Left { get; set; }

        [ContentSerializer(Optional = true)]
        public int Width { get; set; }

        [ContentSerializer(Optional = true)]
        public int Height { get; set; }

        [ContentSerializer(Optional = true)]
        public bool StayOnBack { get; set; }

        [ContentSerializer(Optional = true)]
        public bool StayOnTop { get; set; }

        [ContentSerializer(Optional = true)]
        public bool Passive { get; set; }

        [ContentSerializer(Optional = true)]
        public bool Visible { get; set; }

        [ContentSerializer(Optional = true)]
        public bool Enable { get; set; }

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

        [ContentSerializer(Optional = true)]
        public List<MenuItemEntry> Items { get; set; }

        [ContentSerializer(Optional = true)]
        public bool HideSelection { get; set; }

        [ContentSerializer(Optional = true)]
        public string ContextMenu { get; set; }

        [ContentSerializer(Optional = true)]
        public string ItemIndexChanged { get; set; }

        public ControlItem()
        {
            Visible = true;
            Enable = true;
        }

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
                    container.Passive = Passive;

                    if (TextColor != Color.Transparent)
                        container.TextColor = TextColor;

                    if (BackColor != Color.Transparent)
                        container.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        container.Color = Color;

                    container.Visible = Visible;

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
                    box.Passive = Passive;

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

                    box.Visible = Visible;

                    box.Tag = this;

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
                    label.Passive = Passive;

                    label.Visible = Visible;

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
                    mapBox.Passive = Passive;

                    mapBox.Tag = this;

                    mapBox.Visible = Visible;

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
                case "SideBar":
                    SideBar sideBar = new SideBar(manager);
                    sideBar.Init();

                    sideBar.Name = Name;
                    sideBar.Tag = this;
                    sideBar.Enabled = Enable;

                    sideBar.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    sideBar.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    sideBar.Width = Width;
                    sideBar.Height = Height;

                    sideBar.StayOnBack = StayOnBack;
                    sideBar.StayOnTop = StayOnTop;
                    sideBar.Passive = Passive;
                    sideBar.Visible = Visible;

                    if (BackColor != Color.Transparent)
                        sideBar.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        sideBar.Color = Color;

                    if (TextColor != Color.Transparent)
                        sideBar.TextColor = TextColor;
                    else
                        sideBar.TextColor = Color.Black;

                    if (!string.IsNullOrEmpty(Draw))
                        sideBar.Draw += delegate(object sender, DrawEventArgs e)
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

                    if (!string.IsNullOrEmpty(Parent))
                        sideBar.Parent = parent.GetControl(Parent);
                    else
                        parent.Add(sideBar);

                    break;
                case "ContextMenu":
                    ContextMenu contextMenu = new ContextMenu(manager);
                    contextMenu.Init();

                    contextMenu.Name = Name;
                    contextMenu.Tag = this;
                    contextMenu.Passive = Passive;
                    contextMenu.Enabled = Enable;

                    foreach (MenuItemEntry entry in Items)
                    {
                        MenuItem menuItem = new MenuItem(entry.Title);
                        menuItem.Enabled = entry.Enabled;

                        contextMenu.Items.Add(menuItem);
                    }

                    if (!string.IsNullOrEmpty(Parent))
                        contextMenu.Parent = parent.GetControl(Parent);
                    else
                        parent.Add(contextMenu);

                    break;
                case "ImageListBox":
                    ImageListBox listBox = new ImageListBox(manager);
                    listBox.Init();

                    listBox.Name = Name;

                    listBox.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
                    listBox.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
                    listBox.Width = Width;
                    listBox.Height = Height;

                    listBox.StayOnBack = StayOnBack;
                    listBox.StayOnTop = StayOnTop;
                    listBox.Passive = Passive;
                    listBox.Enabled = Enable;
                    listBox.Tag = this;

                    listBox.Visible = Visible;

                    if (BackColor != Color.Transparent)
                        listBox.BackColor = BackColor;

                    if (Color != Color.Transparent)
                        listBox.Color = Color;

                    if (TextColor != Color.Transparent)
                        listBox.TextColor = TextColor;
                    else
                        listBox.TextColor = Color.Black;

                    listBox.HideSelection = HideSelection;
                    //listBox.ItemIndexChanged = ItemIndexChanged;
                    listBox.ContextMenu = parent.GetControl(ContextMenu) as ContextMenu;

                    if (!string.IsNullOrEmpty(Parent))
                        listBox.Parent = parent.GetControl(Parent);
                    else
                        parent.Add(listBox);
                    break;
                default:
                    throw new Exception("No handling for " + Type);
            }
        }
    }
}
