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
    public class ImageButton : ImageBox
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

        private Texture2D _background;
        private Texture2D _backgroundGlow;
        private Texture2D _backgroundDisabled;
        private static SpriteFont _font;

        public ImageButton(Manager manager)
            : base(manager)
        {
            if (_background == null)
                _background = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_background");

            if (_backgroundGlow == null)
                _backgroundGlow = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_backgroundGlow");

            if (_backgroundDisabled == null)
                _backgroundDisabled = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_backgroundDisabled");

            if (_font == null)
                _font = manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        protected override void DrawControl(Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
        {

            if (Enabled)
            {
                renderer.Draw(_background, rect, Color.White);

                 if( Hovered )
                     renderer.Draw(_backgroundGlow, rect, Color.White);
            }
            else
                renderer.Draw(_backgroundDisabled, rect, Color.White);

            renderer.DrawString(_font, Text, rect, TextColor, Alignment.MiddleCenter);
        }
    }
}
