using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MiRo.SimHexWorld.Helper;

namespace MiRo.SimHexWorld.Engine.World.Meshed
{
    /// <summary>
    /// class that creates a hexagon for the mesh and takes a textures of a 8x8 sprite
    /// </summary>
    public class HexagonMeshItem8X8 : IMeshObject
    {
        public enum Roughness { Plain, Hill, Peak }

        private const float S = 4f;
        static readonly float H = (float)Math.Sin(MathExtension.Deg2Rad(30)) * S; // 2
        static readonly float R = (float)Math.Cos(MathExtension.Deg2Rad(30)) * S; // 3.464
        static readonly float B = S + 2 * H; // 4 + 4 = 8

        static readonly Vector3 Center = new Vector3(0, 0, 0);
        static readonly Vector3 TopCenter = new Vector3(0, 0, -B / 2f);
        static readonly Vector3 TopRight = new Vector3(R, 0, -S / 2f);
        static readonly Vector3 BottomRight = new Vector3(R, 0, S / 2f);
        static readonly Vector3 BottomCenter = new Vector3(0, 0, B / 2f);
        static readonly Vector3 BottomLeft = new Vector3(-R, 0, S / 2f);
        static readonly Vector3 TopLeft = new Vector3(-R, 0, -S / 2f);

        private const float MountainOffset = 4f;
        private const float HillOffset = MountainOffset/2;

        static readonly HexagonTexture HexagonTexture = new HexagonTexture(0.02f, 0.5f, 0.98f, 0.02f, 0.27f, 0.5f, 0.73f, 0.98f, 8, 8);

        private readonly Vector3 _pos;
        readonly float[] _texturePoints;
        readonly Roughness _rough;

        public HexagonMeshItem8X8(Vector3 pos, int tileX, int tileY, Roughness rough = Roughness.Plain )
        {
            _pos = pos;
            _rough = rough;

            _texturePoints = HexagonTexture.GetPoints(tileX, tileY);
        }

        public HexagonMeshItem8X8(Vector3 pos, int index, Roughness rough = Roughness.Plain)
        {
            _pos = pos;
            _rough = rough;

            int tileX = index % 8;
            int tileY = index / 8;

            _texturePoints = HexagonTexture.GetPoints(tileX, tileY);
        }

        #region IDrawableObject Member

        public List<VertexPositionNormalTexture> Vertices
        {
            get
            {
                var vertices = new List<VertexPositionNormalTexture>();

                switch (_rough)
                { 
                    case Roughness.Plain:
                        vertices.Add(new VertexPositionNormalTexture(_pos + Center, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[5]))); // 0
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[3]))); // 1
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[4]))); // 2
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[6]))); // 3
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[7]))); // 4
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[6]))); // 5
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[4]))); // 6
                        break;
                    case Roughness.Hill:
                        vertices.Add(new VertexPositionNormalTexture(_pos + Center + new Vector3(0f,HillOffset,0f), Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[5]))); // 0
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[3]))); // 1
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[4]))); // 2
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[6]))); // 3
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[7]))); // 4
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[6]))); // 5
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[4]))); // 6
                        break;
                    case Roughness.Peak:
                        vertices.Add(new VertexPositionNormalTexture(_pos + Center + new Vector3(0f,MountainOffset,0f), Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[5]))); // 0
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[3]))); // 1
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[4]))); // 2
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[6]))); // 3
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[7]))); // 4
                        vertices.Add(new VertexPositionNormalTexture(_pos + BottomLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[6]))); // 5
                        vertices.Add(new VertexPositionNormalTexture(_pos + TopLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[4]))); // 6
                        break;

            }

                return vertices;
            }
        }

        public List<short> Indices
        {
            get
            {
                return new List<short>(new short[] {
				                         0,  1,  2,  
				                         0,  2,  3,
				                         0,  3,  4,  
				                         0,  4,  5,
				                         0,  5,  6,  
				                         0,  6,  1
				                });
            }
        }

        #endregion
    }
}