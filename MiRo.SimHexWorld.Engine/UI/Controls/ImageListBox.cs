using System;
using System.Linq;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class ImageListBox : ListBox
    {
        #region Fields
        ////////////////////////////////////////////////////////////////////////////
        private readonly ScrollBar _sbVert = null;
        private readonly ClipBox _pane = null;
        ////////////////////////////////////////////////////////////////////////////
        #endregion

        #region constructor
        ////////////////////////////////////////////////////////////////////////////
        public ImageListBox(Manager manager)
            : base(manager)
        {
            _sbVert = Controls.ToArray()[0] as ScrollBar;
            _pane = Controls.ToArray()[1] as ClipBox;

            DrawEventHandler oldDraw = GetDrawEventHandler(_pane, "Draw");

            _pane.Draw -= oldDraw;
            _pane.Draw += new DrawEventHandler(DrawImagePane);
        }
        ////////////////////////////////////////////////////////////////////////////
        #endregion

        #region methods
        ////////////////////////////////////////////////////////////////////////////
        static DrawEventHandler GetDrawEventHandler(object classInstance, string eventName)
        {
            Type classType = classInstance.GetType();
            //FieldInfo[] fi = classType.BaseType.GetFields(BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo eventField = classType.BaseType.GetField(eventName, BindingFlags.GetField
                                                                                                               | BindingFlags.NonPublic
                                                                                                               | BindingFlags.Instance);

            DrawEventHandler eventDelegate = (DrawEventHandler)eventField.GetValue(classInstance);

            // eventDelegate will be null if no listeners are attached to the event
            if (eventDelegate == null)
            {
                return null;
            }

            return eventDelegate;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public override int ItemHeight
        {
            get
            {
                return HeadingHeight + TextHeight;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public int HeadingHeight
        {
            get
            {
                try
                {
                    SkinText fontHeading = Skin.Layers["ListBox.Selection"].Text;
                    float headingHeight = fontHeading.Font.Height;
                    return (int)(headingHeight);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public int TextHeight
        {
            get
            {
                try
                {
                    SkinText fontText = Skin.Layers["ListBox.Text"].Text;

                    if (fontText.Font.Resource == null)
                        fontText.Font.Resource = Manager.Content.Load<SpriteFont>("Content/Fonts/Text");

                    float textHeight = fontText.Font.Height;

                    return (int)(textHeight * 2);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private void DrawImagePane(object sender, DrawEventArgs e)
        {
            try
            {
                if (Items != null && Items.Count > 0)
                {
                    //SkinText font = Skin.Layers["Control"].Text;
                    SkinLayer sel = Skin.Layers["ListBox.Selection"];
                    SkinLayer txt = Skin.Layers["ListBox.Text"];

                    int h = ItemHeight;
                    int v = (_sbVert.Value / 10);
                    int p = (_sbVert.PageSize / 10);
                    int d = (int)(((_sbVert.Value % 10) / 10f) * h);
                    int c = Items.Count;
                    int s = ItemIndex;

                    for (int i = v; i <= v + p + 1; i++)
                    {
                        if (i < c)
                        {
                            Texture2D image = Items[i] as AbstractNamedEntity != null ? (Items[i] as AbstractNamedEntity).Image : null;
                            string desc = Items[i] as AbstractNamedEntity != null ? AbstractNamedEntity.LimitTextLength((Items[i] as AbstractNamedEntity).Description, 200 ) : "";

                            if (image != null)
                                e.Renderer.Draw(image, new Rectangle(e.Rectangle.Left + 2, e.Rectangle.Top - d + ((i - v) * h) + 2, ItemHeight - 4, ItemHeight - 4), Color.White);

                            e.Renderer.DrawString(this, Skin.Layers["Control"], Items[i].ToString(), new Rectangle(e.Rectangle.Left + ItemHeight, e.Rectangle.Top - d + ((i - v) * h), e.Rectangle.Width, HeadingHeight), false);
                            e.Renderer.DrawString(this, txt, desc, new Rectangle(e.Rectangle.Left + ItemHeight, e.Rectangle.Top - d + ((i - v) * h) + HeadingHeight, e.Rectangle.Width, TextHeight), false);
                        }
                    }
                    if (s >= 0 && s < c && (Focused || !HideSelection))
                    {
                        int pos = -d + ((s - v) * h);
                        if (pos > -h && pos < (p + 1) * h)
                        {
                            e.Renderer.DrawLayer(this, sel, new Rectangle(e.Rectangle.Left, e.Rectangle.Top + pos, e.Rectangle.Width, h));

                            Texture2D image = Items[s] as AbstractNamedEntity != null ? (Items[s] as AbstractNamedEntity).Image : null;
                            string desc = Items[s] as AbstractNamedEntity != null ? AbstractNamedEntity.LimitTextLength((Items[s] as AbstractNamedEntity).Description, 200) : "";

                            if (image != null)
                                e.Renderer.Draw(image, new Rectangle(e.Rectangle.Left + 2, e.Rectangle.Top + pos + 2, ItemHeight - 4, ItemHeight - 4), Color.White);

                            e.Renderer.DrawString(this, sel, Items[s].ToString(), new Rectangle(e.Rectangle.Left + ItemHeight, e.Rectangle.Top + pos, e.Rectangle.Width, HeadingHeight), false);
                            e.Renderer.DrawString(this, txt, desc, new Rectangle(e.Rectangle.Left + ItemHeight, e.Rectangle.Top + pos + HeadingHeight, e.Rectangle.Width, TextHeight), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private void TrackItem(int x, int y)
        {
            if (Items != null && Items.Count > 0 && (_pane.ControlRect.Contains(new Point(x, y))))
            {

                int h = ItemHeight;
                int d = (int)(((_sbVert.Value % 10) / 10f) * h);
                int i = (int)Math.Floor((_sbVert.Value / 10f) + ((float)y / h));
                if (i >= 0 && i < Items.Count && i >= (int)Math.Floor((float)_sbVert.Value / 10f) && i < (int)Math.Ceiling((float)(_sbVert.Value + _sbVert.PageSize) / 10f)) ItemIndex = i;
                Focused = true;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        public string SelectedItemName
        {
            get 
            {
                if (ItemIndex >= 0 && ItemIndex < Items.Count)
                {
                    object obj = Items[ItemIndex];

                    if (obj as AbstractNamedEntity != null)
                        return (obj as AbstractNamedEntity).Name;
                    else
                        return obj.ToString();                        
                }
                else
                    return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (HotTrack)
            {
                TrackItem(e.Position.X, e.Position.Y);
            }
        }
        ////////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
