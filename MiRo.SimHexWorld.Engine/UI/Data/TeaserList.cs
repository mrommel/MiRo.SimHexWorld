using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld;

namespace MiRo.SimHexWorld.Engine.UI.Data
{
    public class TeaserList
    {
        public enum TeaserMode { TitleText, TitleTextImage };

        public class TeaserItem
        {
            public TeaserItem() { }

            public TeaserItem(string title, string text, string imageName = "")
            {
                Title = title;
                Text = text;
                ImageName = imageName;
            }

            public string Title { get; set; }
            public string Text { get; set; }

            public string ImageName { get; set; }

            private Texture2D _image;

            public Texture2D Image
            {
                get
                {
                    if (_image == null && !string.IsNullOrEmpty(ImageName))
                        _image = MainApplication.ManagerInstance.Content.Load<Texture2D>(ImageName);

                    return _image;
                }

                set
                {
                    _image = value;
                }
            }
        }
    }
}
