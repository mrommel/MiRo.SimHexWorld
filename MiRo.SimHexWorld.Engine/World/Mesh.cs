using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.World.Helper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace MiRo.SimHexWorld.Engine.World
{
    public class Mesh : IDisposable
    {
        private static Effect _dEffect;
        private static GraphicsDevice _dDevice;

        private VertexBuffer _vertexBuffer;

        private List<short> _indices = new List<short>();
        private IndexBuffer _indexBuffer;

        private List<VertexPositionNormalTexture> _vertices = new List<VertexPositionNormalTexture>();

        private Texture2D _texture;
        readonly string _textureName;

        public Mesh(GraphicsDevice device, string textureName)
        {
            RenderSolid = false;
            _dDevice = device;
            _textureName = textureName;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            if (_dEffect == null)
                _dEffect = content.Load<Effect>("Content/Effects/ShaderFog");

            _texture = TextureManager.Instance[_textureName];
        }

        public bool RenderSolid { private get; set; }

        public void AddObject(IMeshObject obj, bool updateBuffers = true )
        {
            if (_vertices.Count == 0)
            {
                _vertices = obj.Vertices;
                _indices = obj.Indices;
            }
            else
            {
                var maxVertexIndex = (short)_vertices.Count;

                _vertices.AddRange(obj.Vertices);

                obj.Indices.ForEach(idx => _indices.Add((short)(idx + maxVertexIndex)));
            }

            if (updateBuffers) 
                UpdateBuffers();
        }

        public void Clear()
        {
            _indices = new List<short>();
            _vertices = new List<VertexPositionNormalTexture>();
        }

        public void UpdateBuffers()
        {
            if (_vertices.Count > 0)
            {
                if (_vertexBuffer != null)
                    _vertexBuffer.Dispose();

                if (_indexBuffer != null)
                    _indexBuffer.Dispose();

                try
                {
                    // Create the vertex buffer
                    _vertexBuffer = new VertexBuffer(_dDevice, typeof(VertexPositionNormalTexture), _vertices.Count, BufferUsage.WriteOnly);
                    _vertexBuffer.SetData(_vertices.ToArray());

                    // Create the index buffer
                    _indexBuffer = new IndexBuffer(_dDevice, typeof(short), _indices.Count, BufferUsage.WriteOnly);
                    _indexBuffer.SetData(_indices.ToArray());
                }
                catch (Exception ex)
                { }
            }
        }

        public void Draw(GameTime gameTime, Matrix viewMatrix, Matrix projectionMatrix, Vector3 position)
        {
            if (_vertexBuffer != null && _vertices.Count > 0)
            {
                _dEffect.Parameters["World"].SetValue(Matrix.Identity);
                _dEffect.Parameters["WorldViewProject"].SetValue(Matrix.Identity * viewMatrix * projectionMatrix);
                _dEffect.Parameters["thisTexture"].SetValue(_texture);
                _dEffect.Parameters["EyePosition"].SetValue(position);

                // Prepare the graphics device
                _dDevice.SetVertexBuffer(_vertexBuffer);
                _dDevice.Indices = _indexBuffer;

                // enable alpha blending
                BlendState bs = _dDevice.BlendState;
                _dDevice.BlendState = RenderSolid ? BlendState.Opaque : BlendState.AlphaBlend;

                RasterizerState rs = _dDevice.RasterizerState;

                var rasterizerState1 = new RasterizerState();
                rasterizerState1.CullMode = CullMode.None;
                rasterizerState1.FillMode = FillMode.Solid;
                rasterizerState1.MultiSampleAntiAlias = true;
                _dDevice.RasterizerState = rasterizerState1;

                // Start drawing
                foreach (var pass in _dEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _dDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, _vertices.Count, 0, _indices.Count / 3 );
                }

                _dDevice.BlendState = bs;
                _dDevice.RasterizerState = rs;
            }
        }

        public void Update(GameTime gameTime)
        {
            
        }

        ~Mesh()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_vertexBuffer != null)
                _vertexBuffer.Dispose();

            if (_indexBuffer != null)
                _indexBuffer.Dispose();

            //if (_texture != null)
            //    _texture.Dispose();
        }



        public bool HasObjects
        {
            get
            {
                return _vertices.Count > 0;
            }
        }
    }
}
