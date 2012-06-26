using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class ImageButton : ImageBox
    {
        public string Text { get; set; }

        private Texture2D _background;
        private Texture2D _backgroundDisabled;
        private static SpriteFont _font;

        public ImageButton(Manager manager)
            : base(manager)
        {
            if (_background == null)
                _background = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_background");

            if (_backgroundDisabled == null)
                _backgroundDisabled = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//ScienceView//button_backgroundDisabled");

            if (_font == null)
                _font = manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        protected override void DrawControl(Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if( Enabled )
                renderer.Draw(_background, rect, Color.White);
            else
                renderer.Draw(_backgroundDisabled, rect, Color.White);

            renderer.DrawString(_font, Text, rect, TextColor, Alignment.MiddleCenter);
        }
    }
}
