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
    public class PolicyButton : ImageBox
    {
        public Policy Policy { get; set; }

        static Texture2D _button44, _button44gold;
        Rectangle _policyRect = new Rectangle(0, 0, 44, 44);

        public PolicyButton(Manager manager)
            : base(manager)
        {
            if( _button44 == null )
                _button44 = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44");

            if( _button44gold == null )
                _button44gold = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44gold");
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            _policyRect.X = rect.X;
            _policyRect.Y = rect.Y;

            if (Policy != null)
            {
                if (MainWindow.Game.Human.Policies.Contains(Policy))
                    renderer.Draw(_button44gold, _policyRect, Color.White);
                else if (MainWindow.Game.Human.PoliciesInReach.Contains(Policy))
                    renderer.Draw(_button44, _policyRect, Color.Gray);
                else
                    renderer.Draw(_button44, _policyRect, Color.White);

                renderer.Draw(Policy.Image, _policyRect, Color.White);
            }            
        }
    }
}
