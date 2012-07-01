using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.UI.Controls.Helper;

namespace MiRo.SimHexWorld.Engine.UI.Controls
{
    public class Graph : Control
    {
        private List<float> values = new List<float>();

        VectorRenderer vRenderer;

        public Graph(Manager manager)
            : base(manager)
        {
            vRenderer = new VectorRenderer(manager.GraphicsDevice, manager.Content);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void AddValue(float value)
        {
            values.Add(value);

            UpdateValues();
        }

        private void UpdateValues()
        {
            vRenderer.Clear();

            float maxY = float.MinValue;
            float minY = float.MaxValue;

            foreach (float val in values)
            {
                maxY = Math.Max(maxY, val);
                minY = Math.Min(minY, val);
            }

            if (values.Count > 1)
            {
                for (int i = 1; i < values.Count; i++)
                {
                    Vector2 start = Normalize(i - 1, values[i - 1], minY, maxY);
                    Vector2 end = Normalize(i, values[i], minY, maxY);

                    vRenderer.AddLine(start, end, Color.White, Color.White);
                }
            }
        }

        private Vector2 Normalize(float x, float y, float minY, float maxY)
        {
            float dy = maxY - minY;

            if( dy == 0)
                return new Vector2(x / values.Count * Width, (y - minY) * Height);
            else
                return new Vector2(x / values.Count * Width, (y - minY) / (dy) * Height);
        }

        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            vRenderer.Render_ViewportSpace(Matrix.Identity);
        }
    }
}
