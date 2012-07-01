using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Misc;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
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
        public bool Enabled { get; set; }

        private Color Color { get; set; }

        [ContentSerializer(Optional = true)]
        public string ColorName
        {
            get { return ColorConversion.FromColor(Color); }
            set
            {
                if (value != null)
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
        
        [ContentSerializer(Optional = true)]
        public string CheckedChanged { get; set; }

        [ContentSerializer(Optional = true)]
        public string TechName { get; set; }

        [ContentSerializer(Optional = true)]
        public string ToolTip { get; set; }

        [ContentSerializer(Optional = true)]
        public string Import { get; set; }

        public ControlItem()
        {
            Visible = true;
            Enabled = true;
        }

        private void SetProperties(Control c, Control parent, Manager manager)
        {
            c.Name = Name;
            //c.Top = Top < 0 ? manager.GraphicsDevice.Viewport.Height + Top : Top;
            //c.Left = Left < 0 ? manager.GraphicsDevice.Viewport.Width + Left : Left;
            if (!string.IsNullOrEmpty(Parent))
            {
                c.Top = Top < 0 ? parent.GetControl(Parent).Height + Top : Top;
                c.Left = Left < 0 ? parent.GetControl(Parent).Width + Left : Left;
            }
            else
            {
                c.Top = Top < 0 ? parent.Height + Top : Top;
                c.Left = Left < 0 ? parent.Width + Left : Left;
            }

            c.Width = Width;
            c.Height = Height;

            c.StayOnBack = StayOnBack;
            c.StayOnTop = StayOnTop;
            c.Passive = Passive;
            c.Enabled = Enabled;

            if (TextColor != Color.Transparent)
                c.TextColor = TextColor;
            else
                c.TextColor = Color.White;

            if (BackColor != Color.Transparent)
                c.BackColor = BackColor;

            if (Color != Color.Transparent)
                c.Color = Color;

            c.Tag = this;
            c.Visible = Visible;

            if (!string.IsNullOrEmpty(ToolTip))
            {
                ToolTip = ToolTip.Trim();
                c.ToolTipType = typeof(EnhancedToolTip);
                c.ToolTip.Text = ToolTip;
                c.ToolTip.MaximumWidth = 240;
                c.ToolTip.MinimumWidth = 240;
                c.ToolTip.Height = ToolTip.Split(new char[] { '\n' }).Length * 20;
            }

            if (!string.IsNullOrEmpty(Parent))
                c.Parent = parent.GetControl(Parent);
            else
                parent.Add(c);
        }

        public void Init(Control parent, Manager manager)
        {
            switch (Type)
            {
                case "Container":
                    Container container = new Container(manager);
                    container.Init();

                    SetProperties(container, parent, manager);

                    container.AutoScroll = true;
                    break;
                case "ImageBox":
                    ImageBox box = new ImageBox(manager);
                    box.Init();

                    SetProperties(box, parent, manager);

                    box.SizeMode = ImageMode;

                    if (!string.IsNullOrEmpty(ImageAsset))
                        box.Image = manager.Content.Load<Texture2D>(ImageAsset);
                    else if (AtlasAsset != null)
                        box.Image = Provider.GetAtlas(AtlasAsset.Atlas).GetTexture(AtlasAsset.Name);

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
                    break;
                case "Label":
                    Label label = new Label(manager);
                    label.Init();

                    SetProperties(label, parent, manager);

                    label.Text = Text.StartsWith("TXT_KEY_") ? Provider.Instance.Translate(Text) : Text;

                    break;
                case "GameMapBox":
                    GameMapBox mapBox = new GameMapBox(manager);
                    mapBox.Init();

                    SetProperties(mapBox, parent, manager);

                    break;
                case "SideBar":
                    SideBar sideBar = new SideBar(manager);
                    sideBar.Init();

                    SetProperties(sideBar, parent, manager);

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

                    break;
                case "ContextMenu":
                    ContextMenu contextMenu = new ContextMenu(manager);
                    contextMenu.Init();

                    contextMenu.Name = Name;
                    contextMenu.Tag = this;
                    contextMenu.Passive = Passive;
                    contextMenu.Enabled = Enabled;

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

                    SetProperties(listBox, parent, manager);

                    listBox.HideSelection = HideSelection;

                    if( !string.IsNullOrEmpty(ContextMenu))
                        listBox.ContextMenu = parent.GetControl(ContextMenu) as ContextMenu;

                    break;

                case "TechInfoButton":
                    TechInfoButton techInfo = new TechInfoButton(manager);
                    techInfo.Init();

                    SetProperties(techInfo, parent, manager);

                    if( !string.IsNullOrEmpty(TechName))
                        techInfo.Tech = Provider.GetTech(TechName);

                    break;
                case "ImageButton":
                    ImageButton button = new ImageButton(manager);
                    button.Init();

                    SetProperties(button, parent, manager);

                    button.Text = Text;
                    break;
                case "ProgressBar":
                    ProgressBar progress = new ProgressBar(manager);
                    progress.Init();

                    SetProperties(progress, parent, manager);
                    break;
                case "CheckBox":
                    CheckBox check = new CheckBox(manager);
                    check.Init();

                    SetProperties(check, parent, manager);

                    check.Text = Text;
                    break;
                case "Graph":
                    Graph graph = new Graph(manager);
                    graph.Init();

                    SetProperties(graph, parent, manager);

                    break;
                case "RankingRow":
                    RankingRow rank = new RankingRow(manager);
                    rank.Init();

                    SetProperties(rank, parent, manager);
                    break;
                case "Include":
                    List<ControlItem> children = manager.Content.Load<List<ControlItem>>(Import);

                    foreach (ControlItem item in children)
                        item.Init(parent, manager);
                    break;
                default:
                    throw new Exception("No handling for " + Type);
            }
        }
    }
}
