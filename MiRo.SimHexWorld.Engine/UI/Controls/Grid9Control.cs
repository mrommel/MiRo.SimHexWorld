using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld.Engine.UI.Animations;
using log4net;
using MiRo.SimHexWorld.Engine.UI.Controls.Helper;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class Grid9Control : ImageBox
    {
        ILog log = LogManager.GetLogger(typeof(Grid9Control));

        public Grid9Control(Manager manager)
            : base(manager)
        {
            Alpha = 255;
        }

        public Texture2D Background { get; set; }
        public Texture2D BackgroundGlow { get; set; }
        public Texture2D BackgroundDisabled { get; set; }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            UpdateBackground(e.Width, e.Height);
        }

        private Animation _animation;

        private RenderTarget2D _buffer;
        private RenderTarget2D _bufferGlow;
        private RenderTarget2D _bufferDisabled;

        /// <summary>
        ///
        /// </summary>
        /// <param name="buttonWidth"></param>
        /// <param name="buttonHeight"></param>
        private void UpdateBackground(int buttonWidth, int buttonHeight)
        {
            if (buttonWidth <= 0 || buttonHeight <= 0)
                return;

            _buffer = Grid9Helper.CreateBuffer(Background, buttonWidth, buttonHeight);
            _bufferGlow = Grid9Helper.CreateBuffer(BackgroundGlow, buttonWidth, buttonHeight);
            _bufferDisabled = Grid9Helper.CreateBuffer(BackgroundDisabled, buttonWidth, buttonHeight);
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

        public void startAnimation(Animation animation)
        {
            _animation = animation;
            _animation.Start(this);
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            if (_buffer == null)
                UpdateBackground(rect.Width, rect.Height);

            // if buffer is still empty, something wrong
            if (_buffer == null)
                return;

            // completely transparent, don't draw anything
            if (Alpha == 0)
                return;

            if (Enabled)
            {
                if (Hovered)
                    renderer.Draw(_bufferGlow, rect, AlphaColor);
                else
                    renderer.Draw(_buffer, rect, AlphaColor);
            }
            else
                renderer.Draw(_bufferDisabled, rect, AlphaColor);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_animation != null)
            {
                _animation.Update(gameTime);

                if (_animation.IsFinished)
                    _animation = null;
            }
        }
    }
}
