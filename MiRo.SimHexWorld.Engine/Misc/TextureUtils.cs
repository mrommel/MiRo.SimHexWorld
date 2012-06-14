using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.Misc
{
    public class TextureUtils
    {
        /// <summary>
        /// both sprites must be of same size
        /// </summary>
        /// <param name="spriteOne"></param>
        /// <param name="spriteTwo"></param>
        /// <returns></returns>
        public static Texture2D Combine(GraphicsDevice gd, Texture2D spriteOne, Texture2D spriteTwo)
        {
            if (spriteOne.Width != spriteTwo.Width || spriteOne.Width != spriteTwo.Width)
                throw new Exception("Sprites must be of same size!");

            //The result array
            Color[] CombinedColorData = new Color[spriteOne.Width * spriteOne.Height];

            //The array that will be combined with the array above
            Color[] TempColorData = new Color[spriteOne.Width * spriteOne.Height];

            //Fill up the first array with the first sprite
            spriteOne.GetData(CombinedColorData);

            //Fill up the second array with the second sprite
            spriteTwo.GetData(TempColorData);

            //Here is where the magic happens,
            //Walk through the entire CombinedColorData array (what means you go through every pixel)
            for (int i = 0; i < CombinedColorData.Length; i++)
            {
                //If the pixel at the current position is not transparent (of the second sprite).
                //Override the pixel from the first sprite with the second sprite.
                if (TempColorData[i].A != 0)
                {
                    float alpha = TempColorData[i].A / 255f;
                    CombinedColorData[i].R = (byte)(CombinedColorData[i].R * (1f - alpha) + TempColorData[i].R * alpha);
                    CombinedColorData[i].G = (byte)(CombinedColorData[i].G * (1f - alpha) + TempColorData[i].G * alpha);
                    CombinedColorData[i].B = (byte)(CombinedColorData[i].B * (1f - alpha) + TempColorData[i].B * alpha);
                }
            }

            Texture2D returnTexture = new Texture2D(gd, spriteOne.Width, spriteOne.Height);
            returnTexture.SetData(CombinedColorData);

            return returnTexture;
        }

    }
}
