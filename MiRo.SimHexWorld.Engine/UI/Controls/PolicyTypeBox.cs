using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.Types;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class PolicyTypeBox : ImageBox
    {
        public PolicyType PolicyType { get; set; }

        static Texture2D _clipImg;
        static SpriteFont _policyTitleFont;

        Rectangle clipRect = new Rectangle(0, 0, 190, 290);

        public PolicyTypeBox(Manager manager)
            : base(manager)
        {
            if( _clipImg == null )
                _clipImg = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//socialpoliciestrim");

            if( _policyTitleFont == null )
                _policyTitleFont = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            base.DrawControl(renderer, rect, gameTime);

            clipRect.X = rect.X + 1;
            clipRect.Y = rect.Y + 1;
            renderer.Draw(_clipImg, clipRect, Color.White);

            clipRect.Y += 5;
            renderer.DrawString(_policyTitleFont, PolicyType.Title, clipRect, Color.White, Alignment.TopCenter); 
        }
    }
}
