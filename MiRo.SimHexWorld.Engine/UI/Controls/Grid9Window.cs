using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.UI.Controls.Helper;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class Grid9Window : AssetWindow
    {
        RenderTarget2D _backgroundBuffer;
        Texture2D Background;

        public Grid9Window(Manager manager, string asset)
            : base(manager, asset)
        {
            this.BorderVisible = false;
            this.BackColor = Color.Transparent;
            this.Color = Color.Transparent;
            this.Background = Manager.Content.Load<Texture2D>("Content//Textures//UI//Grid9//grid9detailthree140");
            this.Alpha = 255;
            this.Shadow = false;
            this.DesignMode = false;
        }

        private void UpdateBackground(int buttonWidth, int buttonHeight)
        {
            if (buttonWidth <= 0 || buttonHeight <= 0)
                return;

            if (Background == null)
                return;

            _backgroundBuffer = Grid9Helper.CreateBuffer(Background, buttonWidth, buttonHeight);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            UpdateBackground(e.Width, e.Height);
        }

        Color AlphaColor;
        public override byte Alpha
        {
            get
            {
                return base.Alpha;
            }
            set
            {
                //log.InfoFormat("Alpha {0}", value);
                base.Alpha = value;
                AlphaColor = new Color(value, value, value, value);
                Invalidate();
            }
        }

        public override void PrepareTexture(Renderer renderer, GameTime gameTime)
        {
            base.PrepareTexture(renderer, gameTime);
        }

        public override void Render(Renderer renderer, GameTime gameTime)
        {
            if (_backgroundBuffer == null)
                UpdateBackground(Width, Height);

            // if buffer is still empty, something wrong
            if (_backgroundBuffer == null)
                return;

            base.Render(renderer, gameTime);

            if (Visible)
            {
                renderer.Begin(BlendingMode.Default);
                renderer.Draw(_backgroundBuffer, this.AbsoluteRect, AlphaColor);
                renderer.End();
            }
        }

        public override List<Instance.GameNotification> NotificationInterests
        {
            get { throw new NotImplementedException(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
