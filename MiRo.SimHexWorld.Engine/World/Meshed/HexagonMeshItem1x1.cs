using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MiRo.SimHexWorld.Helper;

namespace MiRo.SimHexWorld.Engine.World.Meshed
{
    public class HexagonMeshItem1X1 : IMeshObject
    {
        private const float S = 4f;
        static readonly float H = (float)Math.Sin(MathExtension.Deg2Rad(30)) * S; // 2
        static readonly float R = (float)Math.Cos(MathExtension.Deg2Rad(30)) * S; // 3.464
        static readonly float B = S + 2 * H; // 4 + 4 = 8
        //static float a = 2 * r; // 6.928

        static readonly Vector3 Center = new Vector3(0, 0, 0);
        static readonly Vector3 TopCenter = new Vector3(0, 0, -B / 2f);
        static readonly Vector3 TopRight = new Vector3(R, 0, -S / 2f);
        static readonly Vector3 BottomRight = new Vector3(R, 0, S / 2f);
        static readonly Vector3 BottomCenter = new Vector3(0, 0, B / 2f);
        static readonly Vector3 BottomLeft = new Vector3(-R, 0, S / 2f);
        static readonly Vector3 TopLeft = new Vector3(-R, 0, -S / 2f);

        static readonly HexagonTexture HexagonTexture = new HexagonTexture(0.1f, 0.5f, 0.9f, 0.02f, 0.27f, 0.5f, 0.73f, 0.98f, 1, 1);

        private readonly Vector3 _pos;
        readonly float[] _texturePoints;

        public HexagonMeshItem1X1(Vector3 pos)
        {
            _pos = pos;
            _texturePoints = HexagonTexture.GetPoints(0, 0);
        }

        #region IDrawableObject Member

        public List<VertexPositionNormalTexture> Vertices
        {
            get
            {
                var vertices = new List<VertexPositionNormalTexture>();

                vertices.Add(new VertexPositionNormalTexture(_pos + Center, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[5]))); // 0
                vertices.Add(new VertexPositionNormalTexture(_pos + TopCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[3]))); // 1
                vertices.Add(new VertexPositionNormalTexture(_pos + TopRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[4]))); // 2
                vertices.Add(new VertexPositionNormalTexture(_pos + BottomRight, Vector3.Up, new Vector2(_texturePoints[2], _texturePoints[6]))); // 3
                vertices.Add(new VertexPositionNormalTexture(_pos + BottomCenter, Vector3.Up, new Vector2(_texturePoints[1], _texturePoints[7]))); // 4
                vertices.Add(new VertexPositionNormalTexture(_pos + BottomLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[6]))); // 5
                vertices.Add(new VertexPositionNormalTexture(_pos + TopLeft, Vector3.Up, new Vector2(_texturePoints[0], _texturePoints[4]))); // 6

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
