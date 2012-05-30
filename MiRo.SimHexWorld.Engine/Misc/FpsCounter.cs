using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.Misc
{
    public class FpsCounter
    {
        private float elapsed;
        private float frameRate;
        private float frames;

        public void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsed > 1.0f)
            {
                elapsed -= 1.0f;
                frameRate = frames;
                frames = 0;
            }
            else
            {
                frames += 1;
            }
        }

        public string FrameRate
        {
            get
            {
                return frameRate.ToString("0.00");
            }
        }
    }
}
