using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class TechInfoButton : ImageBox
    {
        public Tech Tech { get; set; }

        private Rectangle _techIconRect = new Rectangle(0, 0, 64, 64);
        private Rectangle _techTitleRect = new Rectangle(0, 0, 96, 16);
        private static Texture2D _backgroundNoReach, _backgroundPossible, _backgroundActive, _backgroundReached;
        private static SpriteFont _font;

        public TechInfoButton(Manager manager)
            : base(manager)
        {
            if (_backgroundNoReach == null)
                _backgroundNoReach = manager.Content.Load<Texture2D>("Content//Textures//UI//ScienceView//TechNoReach");

            if (_backgroundActive == null)
                _backgroundActive = manager.Content.Load<Texture2D>("Content//Textures//UI//ScienceView//TechActive");

            if (_backgroundPossible == null)
                _backgroundPossible = manager.Content.Load<Texture2D>("Content//Textures//UI//ScienceView//TechPossible");

            if (_backgroundReached == null)
                _backgroundReached = manager.Content.Load<Texture2D>("Content//Textures//UI//ScienceView//TechReached");

            if( _font == null)
                _font = manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            if( MainWindow.Game.Human.Technologies.Contains( Tech ))
                renderer.Draw(_backgroundReached, rect, Color.White);
            else if( MainWindow.Game.Human.CurrentResearch == Tech )
                renderer.Draw(_backgroundActive, rect, Color.White);
            else if( MainWindow.Game.Human.PossibleTechnologies.Contains( Tech ) )
                renderer.Draw(_backgroundPossible, rect, Color.White);
            else
                renderer.Draw(_backgroundNoReach, rect, Color.White);

            if( Tech != null )
            {
                _techIconRect.X = rect.X + 2;
                _techIconRect.Y = rect.Y;
                renderer.Draw(Tech.Image, _techIconRect, Color.White);

                _techTitleRect.X = rect.X + 70;
                _techTitleRect.Y = rect.Y + 5;
                renderer.DrawString(_font, Tech.Title,_techTitleRect, Color.White,Alignment.MiddleLeft );
            }
        }
    }
}
