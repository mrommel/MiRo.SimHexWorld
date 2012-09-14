using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.UI.Controls.Helper
{
    public static class Grid9Helper
    {
        /// <summary>
        /// 
        ///    0   x1                x2  width
        ///  0 +---+-----------------+---+
        ///    |   |                 |   |
        /// y1 +---+-----------------+---+
        ///    |   |                 |   |
        ///    |   |                 |   |
        ///    |   |                 |   |
        /// y2 +---+-----------------+---+
        ///    |   |                 |   |
        ///  h +---+-----------------+---+
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buttonWidth"></param>
        /// <param name="buttonHeight"></param>
        /// <returns></returns>
        public static RenderTarget2D CreateBuffer(Texture2D source, int buttonWidth, int buttonHeight)
        {
            int w = source.Width / 3;
            int h = source.Height / 3;

            int x1 = w;
            int x2 = buttonWidth - w;

            int y1 = Math.Min(h, buttonHeight / 2);
            int y2 = buttonHeight - Math.Min(h, buttonHeight / 2);

            RenderTarget2D target = new RenderTarget2D(MainApplication.ManagerInstance.GraphicsDevice, buttonWidth, buttonHeight);
            MainApplication.ManagerInstance.GraphicsDevice.SetRenderTarget(target);

            MainApplication.ManagerInstance.GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = new SpriteBatch(MainApplication.ManagerInstance.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);

            // first line
            spriteBatch.Draw(source, new Rectangle(0, 0, w, h), new Rectangle(0, 0, w, h), Color.White);
            spriteBatch.Draw(source, new Rectangle(x1, 0, x2 - x1, h), new Rectangle(w, 0, w, h), Color.White);
            spriteBatch.Draw(source, new Rectangle(x2, 0, w, h), new Rectangle(w << 1, 0, w, h), Color.White);

            // second line
            spriteBatch.Draw(source, new Rectangle(0, h, w, y2 - y1), new Rectangle(0, h, w, h), Color.White);
            spriteBatch.Draw(source, new Rectangle(x1, h, x2 - x1, y2 - y1), new Rectangle(w, h, w, h), Color.White);
            spriteBatch.Draw(source, new Rectangle(x2, h, w, y2 - y1), new Rectangle(w << 1, h, w, h), Color.White);

            // third line
            spriteBatch.Draw(source, new Rectangle(0, y2, w, h), new Rectangle(0, h << 1, w, h), Color.White);
            spriteBatch.Draw(source, new Rectangle(x1, y2, x2 - x1, h), new Rectangle(w, h << 1, w, h), Color.White);
            spriteBatch.Draw(source, new Rectangle(x2, y2, w, h), new Rectangle(w << 1, h << 1, w, h), Color.White);

            spriteBatch.End();

            MainApplication.ManagerInstance.GraphicsDevice.SetRenderTarget(null);

            return target;
        }
    }
}
