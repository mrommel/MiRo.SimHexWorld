using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class PolicyChooseDialog : AssetWindow
    {
        Texture2D _clipImg;

        SpriteFont _policyTitleFont;

        Texture2D _button44, _button44gold;
        Rectangle _policyRect = new Rectangle(0, 0, 44, 44);
        Rectangle _adoptRect = new Rectangle(0, 0, 125, 32);

        public PolicyChooseDialog(Manager manager)
            : base(manager,"Content//Controls//PolicyChooseDialog")
        {
            InitControls();
        }

        private void InitControls()
        {
            _clipImg = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//socialpoliciestrim");
            _button44 = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44");
            _button44gold = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44gold");

            _policyTitleFont = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");
        }

        public void Button_Draw(object sender, DrawEventArgs e)
        {
            ImageBox box = sender as ImageBox;

            Policy p = Provider.GetPolicy(box.Name.Replace("Button", ""));

            _policyRect.X = e.Rectangle.X;
            _policyRect.Y = e.Rectangle.Y;

            if( Player.Policies.Contains(p))
                e.Renderer.Draw(_button44gold, _policyRect, Color.White);
            else if (Player.PoliciesInReach.Contains(p))
                e.Renderer.Draw(_button44, _policyRect, Color.Gray);
            else
                e.Renderer.Draw(_button44, _policyRect, Color.White);

            e.Renderer.Draw(p.Image, _policyRect, Color.White);
        }

        public void Adopt_Draw(object sender, DrawEventArgs e)
        {
            _adoptRect.X = e.Rectangle.X;
            _adoptRect.Y = e.Rectangle.Y;
            e.Renderer.DrawString(_policyTitleFont, "Adopt", _adoptRect, Color.White, Alignment.MiddleCenter);
        }

        public void Close_Draw(object sender, DrawEventArgs e)
        {
            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 250, 43);
            e.Renderer.DrawString(_policyTitleFont, "Close", r, Color.White, Alignment.MiddleCenter);
        }

        AbstractPlayerData Player
        {
            get
            {
                return MainWindow.Game.Human;
            }
        }

        public void Box_Draw(object sender, DrawEventArgs e)
        {
            ImageBox box = sender as ImageBox;
            string name = box.Name.Replace("Box","");

            PolicyType type = Provider.GetPolicyType(name);

            Rectangle r = new Rectangle(e.Rectangle.X + 1, e.Rectangle.Y + 1, 190, 290);

            e.Renderer.Draw(_clipImg, r, Color.White);

            r.Y += 5;
            e.Renderer.DrawString(_policyTitleFont, type.Title, r, Color.White, Alignment.TopCenter);
        }

        public override List<GameNotification> NotificationInterests
        {
            get { return new List<GameNotification>(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
