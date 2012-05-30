using System.Collections.Generic;
using MiRo.SimHexWorld.Engine.World.Helper;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using MiRo.SimHexWorld.Engine.World.Meshed;
using MiRo.SimHexWorld.Engine.UI;
using MiRo.SimHexWorld.Engine.UI.Controls;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Engine.World
{
    public class HexMesh : IDisposable
    {
        private static Effect _dEffect;
        private static GraphicsDevice _dDevice;

        private VertexBuffer _vertexBuffer;

        private List<short> _indices = new List<short>();
        private IndexBuffer _indexBuffer;

        private List<VertexMultitextured> _vertices = new List<VertexMultitextured>();

        private Texture2D _texture1;
        private Texture2D _texture2;
        private Texture2D _texture3;
        private Texture2D _texture4;
        private Texture2D waterBumpMap;

        const float waterHeight = -0.49f;
        //RenderTarget2D refractionRenderTarget;
        //Texture2D refractionMap;

        //Matrix reflectionViewMatrix;
        //RenderTarget2D reflectionRenderTarget;
        //Texture2D reflectionMap;

        VertexBuffer waterVertexBuffer;
        VertexDeclaration waterVertexDeclaration;
        private IndexBuffer _waterIndexBuffer;

        Texture2D _cloudMap;
        Model _skyDome;

        Vector3 windDirection = new Vector3(-1, 0, 0);

        RenderTarget2D cloudsRenderTarget;
        Texture2D cloudStaticMap;

        VertexBuffer fullScreenBuffer;
        VertexDeclaration fullScreenVertexDeclaration;

        public HexMesh(GraphicsDevice device)
        {
            RenderSolid = false;
            _dDevice = device;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            if (_dEffect == null)
                _dEffect = content.Load<Effect>("Content/Effects/Series4Effects");

            _texture1 = content.Load<Texture2D>("Content/Textures/Ground/sand");
            _texture2 = content.Load<Texture2D>("Content/Textures/Ground/grass");
            _texture3 = content.Load<Texture2D>("Content/Textures/Ground/rock");
            _texture4 = content.Load<Texture2D>("Content/Textures/Ground/snow");

            _skyDome = content.Load<Model>("Content/Models/dome");
            _skyDome.Meshes[0].MeshParts[0].Effect = _dEffect.Clone();

            _cloudMap = content.Load<Texture2D>("Content/Models/cloudMap");

            //PresentationParameters pp = _dDevice.PresentationParameters;
            //refractionRenderTarget = new RenderTarget2D(_dDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            //reflectionRenderTarget = new RenderTarget2D(_dDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            SetUpWaterVertices();

            waterBumpMap = content.Load<Texture2D>("Content/Textures/Ground/water");

            VertexPositionTexture[] fullScreenVertices = SetUpFullscreenVertices();
            //fullScreenVertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);

            VertexDeclaration vertexDeclaration = new VertexDeclaration(VertexMultitextured.VertexElements);
            fullScreenBuffer = new VertexBuffer(_dDevice, vertexDeclaration, fullScreenVertices.Length, BufferUsage.WriteOnly);
            fullScreenBuffer.SetData(fullScreenVertices);

            fullScreenVertexDeclaration = new VertexDeclaration(VertexPositionTexture.VertexDeclaration.GetVertexElements());
        }

        public bool RenderSolid { private get; set; }

        private VertexPositionTexture[] SetUpFullscreenVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];

            vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0f), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0f), new Vector2(1, 1));
            vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0f), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0f), new Vector2(1, 0));

            return vertices;
        }

        private void SetUpWaterVertices()
        {
            float terrainWidth = 200f;
            float terrainLength = 200f;

            if (MainWindow.Game.Map != null)
            {
                HexPoint topLeft = new HexPoint(0, 0);
                HexPoint bottomRight = new HexPoint(MainWindow.Game.Map.Width, MainWindow.Game.Map.Height);

                Vector3 topLeftPt = MapData.GetWorldPosition(topLeft);
                Vector3 bottomRightPt = MapData.GetWorldPosition(bottomRight);

                terrainWidth = Math.Max( topLeftPt.X, bottomRightPt.X ) + 5f;
                terrainLength = Math.Max(topLeftPt.Z, bottomRightPt.Z) + 5f;
            }

            VertexPositionNormalTexture[] waterVertices = new VertexPositionNormalTexture[6];

            waterVertices[0] = new VertexPositionNormalTexture(new Vector3(0, waterHeight - 1f, 0), Vector3.Up, new Vector2(0, 1));
            waterVertices[2] = new VertexPositionNormalTexture(new Vector3(terrainWidth, waterHeight - 1f, terrainLength), Vector3.Up, new Vector2(1, 0));
            waterVertices[1] = new VertexPositionNormalTexture(new Vector3(0, waterHeight - 1f, terrainLength), Vector3.Up, new Vector2(0, 0));

            waterVertices[3] = new VertexPositionNormalTexture(new Vector3(0, waterHeight - 1f, 0), Vector3.Up, new Vector2(0, 1));
            waterVertices[5] = new VertexPositionNormalTexture(new Vector3(terrainWidth, waterHeight - 1f, 0), Vector3.Up, new Vector2(1, 1));
            waterVertices[4] = new VertexPositionNormalTexture(new Vector3(terrainWidth, waterHeight - 1f, terrainLength), Vector3.Up, new Vector2(1, 0));

            waterVertexBuffer = new VertexBuffer(_dDevice, VertexPositionNormalTexture.VertexDeclaration, waterVertices.Length, BufferUsage.WriteOnly);

            waterVertexBuffer.SetData(waterVertices);

            short[] waterIndexData = new short[6];
            waterIndexData[0] = 0;
            waterIndexData[1] = 1; 
            waterIndexData[2] = 2;
            waterIndexData[3] = 3;
            waterIndexData[4] = 4;
            waterIndexData[5] = 5;

            _waterIndexBuffer = new IndexBuffer(_dDevice, typeof(short), 6, BufferUsage.WriteOnly);
            _waterIndexBuffer.SetData(waterIndexData);
        }

        public void AddHexagon(Hexagon obj, bool updateBuffers = true )
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
            _vertices = new List<VertexMultitextured>();
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
                    _vertexBuffer = new VertexBuffer(_dDevice, VertexMultitextured.VertexDeclaration, _vertices.Count, BufferUsage.WriteOnly);
                    _vertexBuffer.SetData(_vertices.ToArray());

                    // Create the index buffer
                    _indexBuffer = new IndexBuffer(_dDevice, typeof(short), _indices.Count, BufferUsage.WriteOnly);
                    _indexBuffer.SetData(_indices.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                SetUpWaterVertices();
            }
        }

        public enum TerrainClipping { None, Above, Below }
        protected void DrawTerrain(GameTime gameTime, Matrix viewMatrix, Matrix projectionMatrix, Vector3 position, TerrainClipping clip = TerrainClipping.None)
        {
            if (_vertexBuffer != null && _vertices.Count > 0)
            {
                _dEffect.CurrentTechnique = _dEffect.Techniques["MultiTextured"];

                if (clip == TerrainClipping.Above)
                {
                    Plane reflectionPlane = CreatePlane(waterHeight, new Vector3(0, -1, 0), viewMatrix, projectionMatrix, true);

                    _dEffect.Parameters["ClipPlane0"].SetValue(new Vector4(reflectionPlane.Normal, reflectionPlane.D));
                    _dEffect.Parameters["Clipping"].SetValue(true);
                } 
                else if (clip == TerrainClipping.Below)
                {
                    Plane reflectionPlane = CreatePlane(waterHeight, new Vector3(0, -1, 0), viewMatrix, projectionMatrix, false);

                    _dEffect.Parameters["ClipPlane0"].SetValue(new Vector4(reflectionPlane.Normal, reflectionPlane.D));
                    _dEffect.Parameters["Clipping"].SetValue(true);
                }
                else
                    _dEffect.Parameters["Clipping"].SetValue(false);

                _dEffect.Parameters["xTexture0"].SetValue(_texture1);
                _dEffect.Parameters["xTexture1"].SetValue(_texture2);
                _dEffect.Parameters["xTexture2"].SetValue(_texture3);
                _dEffect.Parameters["xTexture3"].SetValue(_texture4);

                Matrix worldMatrix = Matrix.Identity;
                _dEffect.Parameters["xWorld"].SetValue(worldMatrix);
                _dEffect.Parameters["xView"].SetValue(viewMatrix);
                _dEffect.Parameters["xProjection"].SetValue(projectionMatrix);

                _dEffect.Parameters["xEnableLighting"].SetValue(true);
                _dEffect.Parameters["xAmbient"].SetValue(0.4f);
                _dEffect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

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

                    _dDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Count, 0, _indices.Count / 3);
                }

                _dDevice.BlendState = bs;
                _dDevice.RasterizerState = rs;
            }
        }

        public void Draw(GameTime gameTime, Matrix viewMatrix, Matrix projectionMatrix, Vector3 position)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            _dDevice.RasterizerState = rs;

            DrawSkyDome(gameTime, viewMatrix, projectionMatrix, position);
            DrawTerrain(gameTime, viewMatrix,projectionMatrix,position, TerrainClipping.Below);
            DrawWater(gameTime, viewMatrix, projectionMatrix);
            DrawTerrain(gameTime, viewMatrix, projectionMatrix, position, TerrainClipping.Above);
        }

        private void DrawWater(GameTime gameTime, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _dEffect.CurrentTechnique = _dEffect.Techniques["Textured"];

            _dEffect.Parameters["xTexture"].SetValue(waterBumpMap);

            Matrix worldMatrix = Matrix.Identity;
            _dEffect.Parameters["xWorld"].SetValue(worldMatrix);
            _dEffect.Parameters["xView"].SetValue(viewMatrix);
            _dEffect.Parameters["xProjection"].SetValue(projectionMatrix);

            _dEffect.Parameters["xEnableLighting"].SetValue(true);
            _dEffect.Parameters["xAmbient"].SetValue(0.01f);
            _dEffect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            // Prepare the graphics device
            _dDevice.SetVertexBuffer(waterVertexBuffer);
            _dDevice.Indices = _waterIndexBuffer;

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

                _dDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, waterVertexBuffer.VertexCount, 0, _waterIndexBuffer.IndexCount / 3);
            }
        }

        private void DrawSkyDome(GameTime gameTime, Matrix viewMatrix, Matrix projectionMatrix, Vector3 position)
        {
            _dDevice.DepthStencilState = DepthStencilState.None;
            Matrix[] modelTransforms = new Matrix[_skyDome.Bones.Count];
            _skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);
            Matrix wMatrix = Matrix.CreateTranslation(0, -0.3f, 0) * Matrix.CreateScale(100) * Matrix.CreateTranslation(position);
            foreach (ModelMesh mesh in _skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(_cloudMap);
                    currentEffect.Parameters["xEnableLighting"].SetValue(false);
                }
                mesh.Draw();
            }
            _dDevice.BlendState = BlendState.Opaque;
            _dDevice.DepthStencilState = DepthStencilState.Default;
        }

        private Plane CreatePlane(float height, Vector3 planeNormalDirection, Matrix currentViewMatrix, Matrix projectionMatrix, bool clipSide)
        {
            planeNormalDirection.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormalDirection, height);
            if (clipSide) 
                planeCoeffs *= -1;
            Plane finalPlane = new Plane(planeCoeffs);
            return finalPlane;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        ~HexMesh()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_vertexBuffer != null)
                _vertexBuffer.Dispose();

            if (_indexBuffer != null)
                _indexBuffer.Dispose();
        }    
    }
}
