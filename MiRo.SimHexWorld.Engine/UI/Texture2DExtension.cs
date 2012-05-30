using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Graphics;
using MiRo.SimHexWorld;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld
{
    public static class Texture2DExtension
    {
        public static Texture2D GetThumbnail(this Texture2D source, int width, int height)
        {
            return GetResized(source,width,height);
        }

        public static Texture2D GetResized(Texture2D sourceImage, int width, int height)
        {
            SpriteBatch batch = new SpriteBatch(MainApplication.ManagerInstance.GraphicsDevice);

            RenderTarget2D renderTarget = new RenderTarget2D(MainApplication.ManagerInstance.GraphicsDevice, width, height);

            Rectangle destinationRectangle = new Rectangle(0, 0, width, height);

            MainApplication.ManagerInstance.GraphicsDevice.SetRenderTarget(renderTarget);

            MainApplication.ManagerInstance.GraphicsDevice.Clear(Color.Transparent);

            batch.Begin();
            batch.Draw(sourceImage, destinationRectangle, Color.White);
            batch.End();

            MainApplication.ManagerInstance.GraphicsDevice.SetRenderTarget(null);

            return renderTarget;
        }
    }
}
