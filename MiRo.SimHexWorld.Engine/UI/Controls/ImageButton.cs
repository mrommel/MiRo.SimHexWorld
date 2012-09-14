using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class ImageButton : Grid9Control
    {
        private string _text;
        public override string Text 
        {
            get 
            {
                if (_text.StartsWith("TXT_KEY_") && Provider.CanTranslate)
                    return Provider.Instance.Translate(_text);

                return _text;
            }
            set { _text = value; }
        }

        private static SpriteFont _font;

        public ImageButton(Manager manager)
            : base(manager)
        {
            if (Background == null)
                Background = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_background");

            if (BackgroundGlow == null)
                BackgroundGlow = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_backgroundGlow");

            if (BackgroundDisabled == null)
                BackgroundDisabled = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_backgroundDisabled");

            if (_font == null)
                _font = manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            base.DrawControl(renderer, rect, gameTime);

            renderer.DrawString(_font, Text, rect, TextColor, Alignment.MiddleCenter);
        }
    }
}
