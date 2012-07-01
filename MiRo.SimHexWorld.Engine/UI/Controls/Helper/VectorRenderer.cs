using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.UI.Controls.Helper
{
    /// <summary>
    /// Batches line "draw" calls from the game, and renders them at one time.
    /// </summary>
    public class VectorRenderer
    {
        GraphicsDevice _graphics;
        ContentManager _content;
        Effect _effect;

        const int _maxPrimitives = 0x1000;
        VertexPositionColorTexture[] vertices;
        Texture2D _texture;
        short[] _indices;
        int currentIndex;
        int lineCount;

        public VectorRenderer(GraphicsDevice g, ContentManager c)
        {
            _graphics = g;
            _content = c;
            _effect = _content.Load<Effect>("Content//Effects//VectorEffect");

            Color[] data = new Color[] { Color.White };
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData<Color>(data);

            Clear();
        }

        public void Clear()
        {
            // create the vertex and indices array
            this.vertices = new VertexPositionColorTexture[_maxPrimitives * 2];
            _indices = createIndexBuffer(_maxPrimitives);
            currentIndex = 0;
            lineCount = 0;
        }

        private short[] createIndexBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 2];
            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 2] = (short)(i * 2);
                indices[i * 2 + 1] = (short)(i * 2 + 1);
            }
            return indices;
        }

        public void AddDotWorld(Vector3 origin, Color color)
        {
            AddLine(origin + new Vector3(0, -.01f, 0), origin + new Vector3(0, .01f, 0), color);
            AddLine(origin + new Vector3(-.01f, 0, 0), origin + new Vector3(.01f, 0, 0), color);
        }

        /// <summary>
        /// Draw a line from one point to another with different colors at each end.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="startColor">The color at the starting point.</param>
        /// <param name="endColor">The color at the ending point.</param>
        public void AddLine(Vector2 start, Vector2 end,
            Color startColor, Color endColor)
        {
            AddLine(
                new VertexPositionColorTexture(new Vector3(start, 0f), startColor, new Vector2()),
                new VertexPositionColorTexture(new Vector3(end, 0f), endColor, new Vector2()));
        }

        /// <summary>
        /// Draw a line from one point to another with the same color.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="color">The color throughout the line.</param>
        public void AddLine(Vector3 start, Vector3 end, Color color)
        {
            AddLine(
                new VertexPositionColorTexture(start, color, new Vector2()),
                new VertexPositionColorTexture(end, color, new Vector2()));
        }


        /// <summary>
        /// Draw a line from one point to another with different colors at each end.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="startColor">The color at the starting point.</param>
        /// <param name="endColor">The color at the ending point.</param>
        public void AddLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
        {
            AddLine(
                new VertexPositionColorTexture(start, startColor, new Vector2()),
                new VertexPositionColorTexture(end, endColor, new Vector2()));
        }


        /// <summary>
        /// Draws a line from one vertex to another.
        /// </summary>
        /// <param name="start">The starting vertex.</param>
        /// <param name="end">The ending vertex.</param>
        public void AddLine(VertexPositionColorTexture start, VertexPositionColorTexture end)
        {
            if (lineCount >= _maxPrimitives)
                throw new Exception("Raster graphics count has exceeded limit.");

            vertices[currentIndex++] = start;
            vertices[currentIndex++] = end;

            lineCount++;
        }


        /// <summary>
        /// Draws the given polygon.
        /// </summary>
        /// <param name="polygon">The polygon to render.</param>
        /// <param name="color">The color to use when drawing the polygon.</param>
        public void AddPolygon(VectorPolygon polygon, Color color)
        {
            AddPolygon(polygon, color, false);
        }

        /// <summary>
        /// Draws the given polygon.
        /// </summary>
        /// <param name="polygon">The polygon to render.</param>
        /// <param name="color">The color to use when drawing the polygon.</param>
        /// <param name="dashed">If true, the polygon will be "dashed".</param>
        public void AddPolygon(VectorPolygon polygon, Color color, bool dashed)
        {
            if (polygon == null)
                return;

            int step = (dashed == true) ? 2 : 1;
            int length = polygon.Points.Length + ((polygon.IsClosed) ? 0 : -1);
            for (int i = 0; i < length; i += step)
            {
                if (lineCount >= _maxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                vertices[currentIndex].Position = polygon.Points[i % polygon.Points.Length];
                vertices[currentIndex++].Color = color;
                vertices[currentIndex].Position = polygon.Points[(i + 1) % polygon.Points.Length];
                vertices[currentIndex++].Color = color;
                lineCount++;
            }
        }

        public void AddCircle(Vector2 origin, float radius, float z, Color color)
        {
            int numPoints = 20;
            float radiansPerPoint = ((float)Math.PI * 2) / numPoints;
            Vector3[] points = new Vector3[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                points[i] = new Vector3(
                    origin.X + (float)Math.Cos(radiansPerPoint * i) * radius,
                    origin.Y + (float)Math.Sin(radiansPerPoint * i) * radius,
                    z);
            }
            AddPolygon(new VectorPolygon(points, true), color);
        }

        //public void Render_WorldSpace(Matrix projection,Vector2 rotation, float zoom)
        //{
        //    // if we don't have any vertices, then we can exit early
        //    if (currentIndex == 0)
        //        return;

        //    float zOffset = (zoom > 1000) ? (zoom - 1000) : 0;

        //    Matrix projection = Matrix.CreateRotationY(rotation.X) * Matrix.CreateRotationX(rotation.Y) * Matrix.CreateTranslation(0, 0, -zOffset) * projection;
        //    _effect.Parameters["ProjectionMatrix"].SetValue(projection);
        //    _effect.Parameters["WorldMatrix"].SetValue(Matrix.CreateScale(zoom * .99f));
        //    _effect.Parameters["Viewport"].SetValue(new Vector2(_graphics.Viewport.Width, _graphics.Viewport.Height));

        //    _effect.CurrentTechnique.Passes[0].Apply();

        //    _graphics.Textures[0] = _texture;
        //    _graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, vertices, 0, currentIndex, _indices, 0, lineCount);

        //    currentIndex = 0;
        //    lineCount = 0;
        //}

        public void Render_ViewportSpace(Matrix projection)
        {
            // if we don't have any vertices, then we can exit early
            if (currentIndex == 0)
                return;

            _graphics.BlendState = BlendState.AlphaBlend;
            _graphics.DepthStencilState = DepthStencilState.None;
            _graphics.SamplerStates[0] = SamplerState.PointClamp;
            _graphics.RasterizerState = RasterizerState.CullNone;

            _effect.Parameters["ProjectionMatrix"].SetValue(projection);
            _effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            _effect.Parameters["Viewport"].SetValue(new Vector2(_graphics.Viewport.Width, _graphics.Viewport.Height));

            _effect.CurrentTechnique.Passes[0].Apply();

            _graphics.Textures[0] = _texture;
            _graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, vertices, 0, currentIndex, _indices, 0, lineCount);
        }
    }
}
