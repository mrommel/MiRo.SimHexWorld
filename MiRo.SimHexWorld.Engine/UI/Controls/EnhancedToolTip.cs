using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class EnhancedToolTip : ToolTip
    {
        private static Dictionary<string, Texture2D> _iconDict = new Dictionary<string, Texture2D>();

        public class IconPosition
        {
            public string IconName { get; set; }
            public Vector2 Location { get; set; }

            private Rectangle _rect = Rectangle.Empty;
            public Rectangle Rect
            {
                get
                {
                    if (_rect == Rectangle.Empty)
                        _rect = new Rectangle((int)Location.X, (int)Location.Y, 16, 16);

                    return _rect;
                }
            }
        }

        public class LinkPosition
        {
            public string Link { get; set; }
            public string Target { get; set; }
            public Vector2 Location { get; set; }

            private Rectangle _rect = Rectangle.Empty;
            public Rectangle Rect
            {
                get
                {
                    if (_rect == Rectangle.Empty)
                        _rect = new Rectangle((int)Location.X, (int)Location.Y, 32, 12);

                    return _rect;
                }
            }
        }

        public class LineInfo
        {
            public string Text { get; set; } // original text with markup
            public string StrippedText { get; set; } // without markup
            public int LineHeight { get; set; }
            public int LineWidth { get; set; }

            public List<IconPosition> Icons { get; set; }
            public List<LinkPosition> Links { get; set; }

            public LineInfo(SkinControl skin, int offset, string text)
            {
                Icons = new List<IconPosition>();
                Links = new List<LinkPosition>();

                Text = text;
                StrippedText = Strip(skin, offset, text);

                Vector2 size = skin.Layers[0].Text.Font.Resource.MeasureString(StrippedText);

                LineHeight = (int)size.Y;
                LineWidth = (int)size.X;
            }

            private string Strip(SkinControl skin, int offset, string text)
            {
                string result = "";
                bool linkStarted = false;
                foreach (string word in text.Split(' '))
                {
                    if (_iconDict.ContainsKey(word))
                    {
                        Vector2 tmp = skin.Layers[0].Text.Font.Resource.MeasureString(result);

                        IconPosition pos = new IconPosition();                       
                        pos.Location = new Vector2(13 + tmp.X, offset);
                        pos.IconName = word;
                        Icons.Add(pos);

                        result += "--- "; 
                    }
                    else if (linkStarted && word != "[/link]")
                    {
                        Links.Last().Link += word + " ";
                        result += word + " ";
                    }
                    else if(word.StartsWith("[link="))
                    {
                        Vector2 tmp = skin.Layers[0].Text.Font.Resource.MeasureString(result);

                        LinkPosition link = new LinkPosition();
                        link.Link = "";
                        link.Target = word.Replace("[link=", "").Replace("]", "");                        
                        link.Location = new Vector2(13 + tmp.X, offset);
                        Links.Add(link);

                        linkStarted = true;
                    }
                    else if (word == "[/link]")
                    {
                        // do nothing
                        linkStarted = false;

                        // update link width
                    }
                    else
                        result += word + " ";
                }

                return result.Trim();
            }
        }

        // the stripped data
        List<LineInfo> lines = new List<LineInfo>();
        
        // for rendering
        Rectangle rLine = new Rectangle();

        bool inDisableMode = false;
        TimeSpan disableIn = TimeSpan.Zero;

        public EnhancedToolTip(Manager manager)
            : base(manager)
        {
            if (_iconDict.Count == 0)
            {
                _iconDict.Add("[Food]", IconProvider.FoodIcon);
                _iconDict.Add("[Happiness]", IconProvider.HappinessIcon);
                _iconDict.Add("[Science]", IconProvider.ScienceIcon);
                _iconDict.Add("[Production]", IconProvider.ProductionIcon);
                _iconDict.Add("[Gold]", IconProvider.GoldIcon);
                _iconDict.Add("[Capital]", IconProvider.CapitalIcon);
                _iconDict.Add("[Culture]", IconProvider.CultureIcon);
            }

            MouseDown += new MouseEventHandler(EnhancedToolTip_MouseDown);
        }

        ////////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            base.Init();
            Passive = false;
            CanFocus = true;
        }
        ////////////////////////////////////////////////////////////////////////////   

        void EnhancedToolTip_MouseDown(object sender, MouseEventArgs e)
        {
            Point pt = e.Position;
            pt.X -= 5;
            pt.Y -= 5;

            foreach (LineInfo li in lines)
            {
                foreach (LinkPosition lp in li.Links)
                {
                    if (lp.Rect.Contains(pt))
                    {
                        MainWindow window = this.Manager.Controls.FirstOrDefault(a => a.Name == "MainWindow") as MainWindow;
                        window.Execute("window", lp.Target, this, sender, e);
                    }
                }
            }
        }

        private Vector2 MeasuredSize
        {
            get
            {
                if (lines == null)
                    return new Vector2();

                Vector2 result = new Vector2();

                foreach (LineInfo line in lines)
                {
                    result.Y += line.LineHeight;
                    result.X = Math.Max(result.X, line.LineWidth);
                }

                return result;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                int offset = 0;
                lines.Clear();
                foreach (string line in value.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        string lineStr = line.Trim();

                        LineInfo li = new LineInfo(Skin, offset, lineStr);
                        lines.Add(li);

                        offset += li.LineHeight;
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (inDisableMode)
            {
                disableIn -= gameTime.ElapsedGameTime;

                if (disableIn.TotalSeconds <= 0)
                {
                    inDisableMode = false;
                    base.Visible = false;
                }
            }
        }

        public override bool Visible
        {
            set
            {
                if (value && Text != null && Text != "" && Skin != null && Skin.Layers[0] != null)
                {
                    Vector2 size = MeasuredSize;
                    Width = (int)size.X + Skin.Layers[0].ContentMargins.Horizontal + 40;
                    Height = (int)size.Y + Skin.Layers[0].ContentMargins.Vertical;
                    Left = Mouse.GetState().X;
                    Top = Mouse.GetState().Y + 24 - 100;

                    rLine.Width = (int)size.X + 40;

                    base.Visible = true;
                }
                else
                {
                    if (base.Visible)
                    {
                        inDisableMode = true;
                        disableIn = TimeSpan.FromSeconds(2);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            if (Left + Width > 1600)
                Left = 1600 - Width;

            if (Top + Height + 40 > 850)
                Top = 850 - Height - 20;

            rect.Width = Math.Max(rect.Width, rLine.Width + 20);
            renderer.DrawLayer(this, Skin.Layers[0], rect);
            
            if (lines != null)
            {
                int y = 5;

                foreach (LineInfo line in lines)
                {
                    // prepare bounding rect
                    rLine.X = rect.X + 5;
                    rLine.Y = rect.Y + y;
                    rLine.Height = line.LineHeight;

                    renderer.DrawString(this, Skin.Layers[0], line.StrippedText, rLine, true);

                    foreach (IconPosition icon in line.Icons)
                        renderer.Draw(_iconDict[icon.IconName], icon.Rect, Color.White);

                    foreach (LinkPosition link in line.Links)
                        renderer.DrawString(Skin.Layers[0].Text.Font.Resource, link.Link, link.Rect.X + 2, link.Rect.Y + y - 3, Color.DarkRed);

                    y += line.LineHeight;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////     
    }
}
