using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.Instance;
using MiRo.SimHexWorld.Engine.Misc;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class PolicyChooseDialog : Window
    {
        Texture2D _clipImg;

        SpriteFont _policyTitleFont;

        ImageBox _traditionBox;
        Texture2D _traditionImg;
        ImageBox _btnAdoptTradition;
        ImageBox _btnTraditionAristocracy;

        ImageBox _libertyBox;
        Texture2D _libertyImg;
        ImageBox _btnAdoptLiberty;

        ImageBox _btnClose;

        public PolicyChooseDialog(Manager manager)
            : base(manager)
        {
            Init();

            Text = "Policy Choose";

            Width = 192 * 5 + 10;
            Height = 292 * 2 + 120;

            Visible = true;

            InitControls();
        }

        private void InitControls()
        {
            _clipImg = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//socialpoliciestrim");
            _traditionImg = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//socialpoliciestradition");
            _libertyImg = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//socialpoliciesliberty");

            _policyTitleFont = Manager.Content.Load<SpriteFont>("Content//Fonts//Default");

            #region tradition
            ToolTip _traditionToolTip = new TomShane.Neoforce.Controls.ToolTip(Manager);
            _traditionToolTip.Init();
            _traditionToolTip.Width = 200;
            _traditionToolTip.Height = 150;
            _traditionToolTip.Text = Provider.GetPolicyType("Tradition").Description;
            Add(_traditionToolTip);

            _traditionBox = new ImageBox(Manager);
            _traditionBox.Init();
            _traditionBox.Top = 10;
            _traditionBox.Left = 5;
            _traditionBox.Width = 192;
            _traditionBox.Height = 292;
            _traditionBox.Image = _traditionImg;
            _traditionBox.Draw += new DrawEventHandler(_tradionBox_Draw);
            _traditionBox.StayOnBack = true;
            _traditionBox.ToolTip = _traditionToolTip;
            Add(_traditionBox);

            _btnAdoptTradition = new ImageBox(Manager);
            _btnAdoptTradition.Init();
            _btnAdoptTradition.Top = _traditionBox.Top + 40;
            _btnAdoptTradition.Left = _traditionBox.Left + ( _traditionBox.Width - 125 ) / 2;
            _btnAdoptTradition.Width = 125;
            _btnAdoptTradition.Height = 32;
            _btnAdoptTradition.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button");
            _btnAdoptTradition.SizeMode = SizeMode.Stretched;
            _btnAdoptTradition.Draw += new DrawEventHandler(_btnAdopt_Draw);
            _btnAdoptTradition.StayOnBack = false;
            _btnAdoptTradition.Click += new TomShane.Neoforce.Controls.EventHandler(_btnAdoptTradition_Click);
            _btnAdoptTradition.Visible = !HasAdoptedTradition;
            Add(_btnAdoptTradition);

            _btnTraditionAristocracy = new ImageBox(Manager);
            _btnTraditionAristocracy.Init();
            _btnTraditionAristocracy.Top = _traditionBox.Top + 40;
            _btnTraditionAristocracy.Left = _traditionBox.Left + 20;
            _btnTraditionAristocracy.Width = 44;
            _btnTraditionAristocracy.Height = 44;
            _btnTraditionAristocracy.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button44");
            _btnTraditionAristocracy.Draw += new DrawEventHandler(_btnTraditionAristocracy_Draw);
            _btnTraditionAristocracy.StayOnBack = false;
            _btnTraditionAristocracy.Click += new TomShane.Neoforce.Controls.EventHandler(_btnTraditionAristocracy_Click);
            _btnTraditionAristocracy.Visible = HasAdoptedTradition;
            Add(_btnTraditionAristocracy);

            #endregion tradition

            #region liberty
            _libertyBox = new ImageBox(Manager);
            _libertyBox.Init();
            _libertyBox.Top = 10;
            _libertyBox.Left = 5 + _traditionBox.Width;
            _libertyBox.Width = 192;
            _libertyBox.Height = 292;
            _libertyBox.Image = _libertyImg;
            _libertyBox.Draw += new DrawEventHandler(_libertyBox_Draw);
            _libertyBox.StayOnBack = true;
            Add(_libertyBox);

            _btnAdoptLiberty = new ImageBox(Manager);
            _btnAdoptLiberty.Init();
            _btnAdoptLiberty.Top = _libertyBox.Top + 40;
            _btnAdoptLiberty.Left = _libertyBox.Left + +(_libertyBox.Width - 125) / 2;
            _btnAdoptLiberty.Width = 125;
            _btnAdoptLiberty.Height = 32;
            _btnAdoptLiberty.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button");
            _btnAdoptLiberty.SizeMode = SizeMode.Stretched;
            _btnAdoptLiberty.Draw += new DrawEventHandler(_btnAdopt_Draw);
            _btnAdoptLiberty.StayOnBack = false;
            _btnAdoptLiberty.Click += new TomShane.Neoforce.Controls.EventHandler(_btnAdoptLiberty_Click);
            _btnAdoptLiberty.Visible = !HasAdoptedLiberty;
            Add(_btnAdoptLiberty);
            #endregion liberty

            _btnClose = new ImageBox(Manager);
            _btnClose.Init();
            _btnClose.Top = Height - 80;
            _btnClose.Left = 5;
            _btnClose.Width = 250;
            _btnClose.Height = 43;
            _btnClose.Image = Manager.Content.Load<Texture2D>("Content//Textures//UI//PolicyView//button");
            _btnClose.Draw += new DrawEventHandler(_btnClose_Draw);
            _btnClose.Click += new TomShane.Neoforce.Controls.EventHandler(_btnClose_Click);
            Add(_btnClose);
        }

        void _btnTraditionAristocracy_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (!Player.AdoptPolicy(Provider.GetPolicy("Aristocracy")))
            {
                //Close();
                MessageBox.Show(Manager, "It was not possible to adopt Aristocracy, because there were to little culture.", "Adopt Aristocracy");
            }
        }

        void _btnTraditionAristocracy_Draw(object sender, DrawEventArgs e)
        {
            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 44, 44);
            e.Renderer.Draw(Provider.GetAtlas("PolicyAtlas").GetTexture("Aristocracy"), r, Color.White);
        }

        void _btnAdoptLiberty_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (!Player.AdoptPolicyType(Provider.GetPolicyType("Liberty")))
            {
                MessageBox.Show(Manager, "It was not possible to adopt Liberty, because there were to little culture.", "Adopt Liberty");
                return;
            }

            _btnAdoptLiberty.Visible = !HasAdoptedLiberty;
        }

        void _btnAdoptTradition_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (!Player.AdoptPolicyType(Provider.GetPolicyType("Tradition")))
            {
                MessageBox.Show(Manager, "It was not possible to adopt Tradition, because there were to little culture.", "Adopt Tradition");
                return;
            }

            _btnAdoptTradition.Visible = !HasAdoptedTradition;

            _btnTraditionAristocracy.Visible = HasAdoptedTradition;
        }

        void _btnAdopt_Draw(object sender, DrawEventArgs e)
        {
            Rectangle r = new Rectangle(e.Rectangle.X, e.Rectangle.Y, 125, 32);
            e.Renderer.DrawString(_policyTitleFont, "Adopt", r, Color.White, Alignment.MiddleCenter);
        }

        void _btnClose_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
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

        bool HasAdoptedTradition
        {
            get
            { return Player.AdoptedPolicyTypes.Contains(Provider.GetPolicyType("Tradtion")); }
        }

        bool HasAdoptedLiberty
        {
            get
            { return Player.AdoptedPolicyTypes.Contains(Provider.GetPolicyType("Liberty")); }
        }

        void _libertyBox_Draw(object sender, DrawEventArgs e)
        {
            Rectangle r = new Rectangle(e.Rectangle.X + 1, e.Rectangle.Y + 1, 190, 290);

            //if( Player.AdoptedPolicyTypes 
            e.Renderer.Draw(_clipImg, r, Color.White);

            r.Y += 5;
            e.Renderer.DrawString(_policyTitleFont, "Liberty", r, Color.White, Alignment.TopCenter);
        }

        void _tradionBox_Draw(object sender, DrawEventArgs e)
        {
            Rectangle r = new Rectangle( e.Rectangle.X + 1, e.Rectangle.Y + 1, 190, 290 );
            e.Renderer.Draw(_clipImg, r, Color.White);

            r.Y += 5;
            e.Renderer.DrawString(_policyTitleFont, "Tradition", r,Color.White,Alignment.TopCenter);
        }
    }
}
