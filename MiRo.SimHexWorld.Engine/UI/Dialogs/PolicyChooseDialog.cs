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
    public class PolicyChooseDialog : GameWindow
    {
        Texture2D _clipImg;

        SpriteFont _policyTitleFont;

        Texture2D _button44, _button44gold;

        ImageBox _btnClose;

        public PolicyChooseDialog(Manager manager)
            : base(manager)
        {
            Init();

            Text = "Policy Choose";

            Width = 192 * 5 + 25;
            Height = 292 * 2 + 150;

            Visible = true;

            InitControls();
        }

        private void InitControls()
        {
            _clipImg = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//socialpoliciestrim");
            _button44 = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44");
            _button44gold = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44gold");

            _policyTitleFont = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");

            LoadControls("Content//Controls//PolicyChooseDialog//Tradition");
            LoadControls("Content//Controls//PolicyChooseDialog//Liberty");
            LoadControls("Content//Controls//PolicyChooseDialog//Honor");
            LoadControls("Content//Controls//PolicyChooseDialog//Piety");
            LoadControls("Content//Controls//PolicyChooseDialog//Patronage");

            LoadControls("Content//Controls//PolicyChooseDialog//Order");
            LoadControls("Content//Controls//PolicyChooseDialog//Autocracy");

            UpdateVisibility();

            _btnClose = new ImageBox(Manager);
            _btnClose.Init();
            _btnClose.Top = Height - 80;
            _btnClose.Left = 5;
            _btnClose.Width = 250;
            _btnClose.Height = 43;
            _btnClose.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button");
            _btnClose.Draw += new DrawEventHandler(_btnClose_Draw);
            _btnClose.Click += new TomShane.Neoforce.Controls.EventHandler(Close_Click);
            Add(_btnClose);
        }

        private void UpdateVisibility()
        {
            bool hasAdoptedTradition = HasAdoptedTradition;
            GetControl("AdoptTraditionButton").Visible = !hasAdoptedTradition;
            GetControl("AristocracyButton").Visible = hasAdoptedTradition;
            GetControl("LandedEliteButton").Visible = hasAdoptedTradition;
            GetControl("LegalismButton").Visible = hasAdoptedTradition;
            GetControl("MonarchyButton").Visible = hasAdoptedTradition;
            GetControl("OligarchyButton").Visible = hasAdoptedTradition;

            bool hasAdoptedLiberty = HasAdoptedLiberty;
            GetControl("AdoptLibertyButton").Visible = !hasAdoptedLiberty;
            GetControl("CollectiveRuleButton").Visible = hasAdoptedLiberty;
            GetControl("CitizenshipButton").Visible = hasAdoptedLiberty;
            GetControl("MeritocracyButton").Visible = hasAdoptedLiberty;
            GetControl("RepresentationButton").Visible = hasAdoptedLiberty;
            GetControl("RepublicButton").Visible = hasAdoptedLiberty;
        }

        public void Choose_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ImageBox box = sender as ImageBox;
            string name = box.Name.Replace("Button", "");

            if (!Player.AdoptPolicy(Provider.GetPolicy(name)))
            {
                MessageBox.Show(Manager, "It was not possible to adopt " + name + ", because there were to little culture.\n\n" + Player.Culture + " of culture present, but " + Player.CultureNeededForChange + " needed.", "Adopt " + name);
            }

            Close();
        }

        public void Button_Draw(object sender, DrawEventArgs e)
        {
            ImageBox box = sender as ImageBox;

            Policy p = Provider.GetPolicy(box.Name.Replace("Button", ""));

            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 44, 44);

            if( Player.Policies.Contains(p))
                e.Renderer.Draw(_button44gold, r, Color.White);
            else
                e.Renderer.Draw(_button44, r, Color.White);

            e.Renderer.Draw(p.Image, r, Color.White);
        }

        public void Adopt_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ImageBox box = sender as ImageBox;
            string name = box.Name.Replace("Adopt", "").Replace("Button","");

            if (!Player.AdoptPolicyType(Provider.GetPolicyType(name)))
            {
                MessageBox.Show(Manager, "It was not possible to adopt " + name + ", because there were to little culture.\n\n" + Player.Culture + " of culture present, but " + Player.CultureNeededForChange + " needed.", "Adopt " + name);
                return;
            }

            UpdateVisibility();
            Close();
        }

        public void Adopt_Draw(object sender, DrawEventArgs e)
        {
            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 125, 32);
            e.Renderer.DrawString(_policyTitleFont, "Adopt", r, Color.White, Alignment.MiddleCenter);
        }

        public void Close_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Close();
        }

        void _btnClose_Draw(object sender, DrawEventArgs e)
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

        public bool HasAdoptedTradition
        {
            get { return Player.AdoptedPolicyTypes.Contains(Provider.GetPolicyType("Tradition")); }
        }

        public bool HasAdoptedLiberty
        {
            get { return Player.AdoptedPolicyTypes.Contains(Provider.GetPolicyType("Liberty")); }
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
