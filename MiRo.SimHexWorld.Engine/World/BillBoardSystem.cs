using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MiRoSimHexWorld.Engine.World
{
    public class BillboardSystem<T>
    {
        readonly GraphicsDevice _graphicsDevice;
        readonly ContentManager _content;
        readonly Dictionary<T, BillboardSystemEntity> _entities = new Dictionary<T, BillboardSystemEntity>();
        readonly Dictionary<T, List<Vector3>> _positions = new Dictionary<T, List<Vector3>>();

        public BillboardSystem(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;
            _content = content;
        }

        public void AddEntity(T t, Texture2D texture, Vector2 size)
        {
            _entities.Add(t, new BillboardSystemEntity(_graphicsDevice, _content, texture, size));
        }

        public void AddEntity(T t,string textureName, Vector2 size)
        { 
            _entities.Add(t,new BillboardSystemEntity(_graphicsDevice,_content,_content.Load<Texture2D>(textureName),size));
        }

        public void Reset()
        {
            _entities.Clear();
            _positions.Clear();
        }

        public void ResetPositions()
        {
            _positions.Clear();
        }

        public void AddPosition(T t, Vector3 pos)
        {
            if( !_positions.ContainsKey(t))
                _positions.Add(t, new List<Vector3>());

            _positions[t].Add(pos);
        }

        public void Build()
        {
            foreach (T t in _entities.Keys)
            {
                if (_positions.ContainsKey(t) && _positions[t].Count > 0)
                    _entities[t].GenerateParticles(_positions[t].ToArray());
                else
                    _entities[t].Initialized = false;
            }
        }

        public void Draw(Matrix view, Matrix projection, Vector3 camPos, Vector3 up, Vector3 right)
        {
            foreach (T t in _entities.Keys)
            {
                if( _entities[t].Initialized)
                    _entities[t].Draw(view,projection,camPos,up,right);
            }
        }

        public class BillboardSystemEntity
        {
            // Vertex buffer and index buffer, particle
            // and index arrays
            VertexBuffer _verts;
            IndexBuffer _ints;
            VertexPositionTexture[] _particles;
            short[] _indices;

            // Billboard settings
            int _nBillboards;
            readonly Vector2 _billboardSize;
            readonly Texture2D _texture;

            // GraphicsDevice and Effect
            readonly GraphicsDevice _graphicsDevice;
            readonly Effect _effect;

            public BillboardSystemEntity(GraphicsDevice graphicsDevice,
                ContentManager content, Texture2D texture,
                Vector2 billboardSize, Vector3[] particlePositions)
            {
                _nBillboards = particlePositions.Length;
                _billboardSize = billboardSize;
                _graphicsDevice = graphicsDevice;
                _texture = texture;

                _effect = content.Load<Effect>("Content/Effects/BillboardEffect");

                GenerateParticles(particlePositions);
            }

            public BillboardSystemEntity(GraphicsDevice graphicsDevice,
               ContentManager content, Texture2D texture,
               Vector2 billboardSize)
            {
                _billboardSize = billboardSize;
                _graphicsDevice = graphicsDevice;
                _texture = texture;

                _effect = content.Load<Effect>("Content/Effects/BillboardEffect");
            }

            public void GenerateParticles(Vector3[] particlePositions)
            {
                _nBillboards = particlePositions.Length;

                // Create vertex and index arrays
                _particles = new VertexPositionTexture[_nBillboards * 4];
                _indices = new short[_nBillboards * 6];
                var x = 0;
                // For each billboard...
                for (int i = 0; i < _nBillboards * 4; i += 4)
                {
                    Vector3 pos = particlePositions[i / 4];

                    // Add 4 vertices at the billboard's position
                    _particles[i + 0] = new VertexPositionTexture(pos, new Vector2(0, 0));
                    _particles[i + 1] = new VertexPositionTexture(pos, new Vector2(0, 1));
                    _particles[i + 2] = new VertexPositionTexture(pos, new Vector2(1, 1));
                    _particles[i + 3] = new VertexPositionTexture(pos, new Vector2(1, 0));

                    // Add 6 indices to form two triangles
                    _indices[x++] = (short)(i + 0);
                    _indices[x++] = (short)(i + 3);
                    _indices[x++] = (short)(i + 2);
                    _indices[x++] = (short)(i + 2);
                    _indices[x++] = (short)(i + 1);
                    _indices[x++] = (short)(i + 0);
                }
                // Create and set the vertex buffer
                _verts = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTexture), _nBillboards * 4, BufferUsage.WriteOnly);
                _verts.SetData<VertexPositionTexture>(_particles);

                // Create and set the index buffer
                _ints = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _nBillboards * 6, BufferUsage.WriteOnly);
                _ints.SetData<short>(_indices);
            }

            void SetEffectParameters(Matrix view, Matrix projection, Vector3 camPos, Vector3 up, Vector3 right)
            {
                _effect.Parameters["ParticleTexture"].SetValue(_texture);
                _effect.Parameters["View"].SetValue(view);
                _effect.Parameters["Projection"].SetValue(projection);
                _effect.Parameters["Size"].SetValue(_billboardSize / 2f);
                _effect.Parameters["Up"].SetValue(up);
                _effect.Parameters["Side"].SetValue(right);

                _effect.CurrentTechnique.Passes[0].Apply();
            }

            public void Draw(Matrix view, Matrix projection, Vector3 camPos, Vector3 up, Vector3 right)
            {
                // Set the vertex and index buffer to the graphics card
                _graphicsDevice.SetVertexBuffer(_verts);
                _graphicsDevice.Indices = _ints;
                _graphicsDevice.BlendState = BlendState.AlphaBlend;

                SetEffectParameters(view, projection, camPos, up, right);

                _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                _effect.Parameters["AlphaTest"].SetValue(false);
                DrawBillboards();

                // Reset render states
                _graphicsDevice.BlendState = BlendState.Opaque;
                _graphicsDevice.DepthStencilState = DepthStencilState.Default;

                // Un-set the vertex and index buffer
                _graphicsDevice.SetVertexBuffer(null);
                _graphicsDevice.Indices = null;
            }

            void DrawBillboards()
            {
                _effect.CurrentTechnique.Passes[0].Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4 * _nBillboards, 0, _nBillboards * 2);
            }

            public bool Initialized
            {
                get
                {
                    return _nBillboards > 0;
                }
                set
                {
                    if (value == false)
                        _nBillboards = 0;
                }
            }
        }

        public bool HasPositions { get { return _positions.Count > 0; } }
    }    
}
