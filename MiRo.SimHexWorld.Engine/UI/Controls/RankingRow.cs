using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.Instance;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class RankingRow : Control
    {
        private static Texture2D bar340x2;
        private static SpriteFont _font;

        public AbstractPlayerData Player { get; set; }

        private Rectangle _iconRect = new Rectangle(0, 0, 20, 20);
        private Rectangle _seperatorRect = new Rectangle(0, 0, 340, 2);

        public RankingRow(Manager manager)
            : base(manager)
        {
            if (bar340x2 == null)
                bar340x2 = manager.Content.Load<Texture2D>("Content//Textures//UI//MainView//bar340x2");

            if (_font == null)
                _font = manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            if (Player != null)
            {
                _iconRect.X = rect.X;
                _iconRect.Y = rect.Y;

                renderer.Draw(Player.Leader.Image, _iconRect, Color.White);

                renderer.DrawString(_font, Player.Leader.Title, rect.X + 24, rect.Y, Color.White);

                renderer.DrawString(_font, Player.Score.ToString(), rect.X + rect.Width - 20, rect.Y, Color.White);
            }

            _seperatorRect.X = rect.X;
            _seperatorRect.Y = rect.Y + rect.Height - 2;

            renderer.Draw(bar340x2, _seperatorRect, Color.White);
        }
    }
}
