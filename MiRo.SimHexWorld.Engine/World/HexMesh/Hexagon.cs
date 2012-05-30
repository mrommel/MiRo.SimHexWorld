using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.World.Meshed;
using Microsoft.Xna.Framework.Content;
using NUnit.Framework;

namespace MiRo.SimHexWorld.Engine.World
{
    public class Hexagon
    {
        public class HexagonPoint
        {
            public Vector3 Point { get; set; }
            public Vector2 TexturePoint { get; set; }
            public string Weight { get; set; }
        }

        public class Triangle 
        { 
            public short A { get; set; }
            public short B { get; set; }
            public short C { get; set; } 
        }

        public List<HexagonPoint> Points { get; set; }
        public List<Triangle> Triangles { get; set; }

        [ContentSerializerIgnore]
        public Vector3 Offset { get; set; }

        TileType _top, _topRight, _bottomRight, _bottom, _bottomLeft, _topLeft, _center;
        float _topH, _topRightH, _bottomRightH, _bottomH, _bottomLeftH, _topLeftH, _centerH;

        public Hexagon()
        { 
        }

        private float GetHeights(string weight)
        {
            string[] parts = weight.Split(' ');
            float topWeight = float.Parse(parts[0]);
            float topRightWeight = float.Parse(parts[1]);
            float bottomRightWeight = float.Parse(parts[2]);
            float bottomWeight = float.Parse(parts[3]);
            float bottomLeftWeight = float.Parse(parts[4]);
            float topLeftWeight = float.Parse(parts[5]);
            float centerWeight = float.Parse(parts[6]);

            return _topH * topWeight +
                _topRightH * topRightWeight +
                _bottomRightH * bottomRightWeight +
                _bottomH * bottomWeight +
                _bottomLeftH * bottomLeftWeight +
                _topLeftH * topLeftWeight +
                _centerH * centerWeight;
        }

        private float[] GetWeights(string weight)
        {
            float[] result = new float[4];

            string[] parts = weight.Split(' ');
            float topWeight = float.Parse(parts[0]);
            float topRightWeight = float.Parse(parts[1]);
            float bottomRightWeight = float.Parse(parts[2]);
            float bottomWeight = float.Parse(parts[3]);
            float bottomLeftWeight = float.Parse(parts[4]);
            float topLeftWeight = float.Parse(parts[5]);
            float centerWeight = float.Parse(parts[6]);

            result[0] = ((_top & TileType.Sand) == TileType.Sand ? 1 : 0) * topWeight +
                ((_topRight & TileType.Sand) == TileType.Sand ? 1 : 0) * topRightWeight +
                ((_bottomRight & TileType.Sand) == TileType.Sand ? 1 : 0) * bottomRightWeight +
                ((_bottom & TileType.Sand) == TileType.Sand ? 1 : 0) * bottomWeight +
                ((_bottomLeft & TileType.Sand) == TileType.Sand ? 1 : 0) * bottomLeftWeight +
                ((_topLeft & TileType.Sand) == TileType.Sand ? 1 : 0) * topLeftWeight +
                ((_center & TileType.Sand) == TileType.Sand ? 1 : 0) * centerWeight;

            result[1] = ((_top & TileType.Grass) == TileType.Grass ? 1 : 0) * topWeight +
                ((_topRight & TileType.Grass) == TileType.Grass ? 1 : 0) * topRightWeight +
                ((_bottomRight & TileType.Grass) == TileType.Grass ? 1 : 0) * bottomRightWeight +
                ((_bottom & TileType.Grass) == TileType.Grass ? 1 : 0) * bottomWeight +
                ((_bottomLeft & TileType.Grass) == TileType.Grass ? 1 : 0) * bottomLeftWeight +
                ((_topLeft & TileType.Grass) == TileType.Grass ? 1 : 0) * topLeftWeight +
                ((_center & TileType.Grass) == TileType.Grass ? 1 : 0) * centerWeight;

            result[2] = ((_top & TileType.Rock) == TileType.Rock ? 1 : 0) * topWeight +
                ((_topRight & TileType.Rock) == TileType.Rock ? 1 : 0) * topRightWeight +
                ((_bottomRight & TileType.Rock) == TileType.Rock ? 1 : 0) * bottomRightWeight +
                ((_bottom & TileType.Rock) == TileType.Rock ? 1 : 0) * bottomWeight +
                ((_bottomLeft & TileType.Rock) == TileType.Rock ? 1 : 0) * bottomLeftWeight +
                ((_topLeft & TileType.Rock) == TileType.Rock ? 1 : 0) * topLeftWeight +
                ((_center & TileType.Rock) == TileType.Rock ? 1 : 0) * centerWeight;

            result[3] = ((_top & TileType.Snow) == TileType.Snow ? 1 : 0) * topWeight +
                ((_topRight & TileType.Snow) == TileType.Snow ? 1 : 0) * topRightWeight +
                ((_bottomRight & TileType.Snow) == TileType.Snow ? 1 : 0) * bottomRightWeight +
                ((_bottom & TileType.Snow) == TileType.Snow ? 1 : 0) * bottomWeight +
                ((_bottomLeft & TileType.Snow) == TileType.Snow ? 1 : 0) * bottomLeftWeight +
                ((_topLeft & TileType.Snow) == TileType.Snow ? 1 : 0) * topLeftWeight +
                ((_center & TileType.Snow) == TileType.Snow ? 1 : 0) * centerWeight;

            return result;
        }

        [ContentSerializerIgnore]
        public List<VertexMultitextured> Vertices
        {
            get
            {
                VertexMultitextured[] v = new VertexMultitextured[Points.Count];

                int i = 0;
                foreach (HexagonPoint pt in Points)
                {
                    VertexMultitextured mv = new VertexMultitextured();

                    mv.Position = pt.Point + Offset;
                    mv.Position.Y += GetHeights(pt.Weight);
                    mv.TextureCoordinate.X = pt.TexturePoint.X;
                    mv.TextureCoordinate.Y = pt.TexturePoint.Y;

                    float[] weights = GetWeights(pt.Weight);
                    mv.TexWeights.X = weights[0];
                    mv.TexWeights.Y = weights[1];
                    mv.TexWeights.Z = weights[2];
                    mv.TexWeights.W = weights[3];

                    mv.Normal = Vector3.Zero;

                    float totalWeight = mv.TexWeights.X;
                    totalWeight += mv.TexWeights.Y;
                    totalWeight += mv.TexWeights.Z;
                    totalWeight += mv.TexWeights.W;

                    mv.TexWeights.X /= totalWeight;
                    mv.TexWeights.Y /= totalWeight;
                    mv.TexWeights.Z /= totalWeight;
                    mv.TexWeights.W /= totalWeight;

                    Assert.AreEqual(1, mv.TexWeights.X + mv.TexWeights.Y + mv.TexWeights.Z + mv.TexWeights.W);

                    v[i] = mv;
                    i++;
                }

                foreach (Triangle tri in Triangles)
                {
                    short index1 = tri.A;
                    short index2 = tri.B;
                    short index3 = tri.C;

                    Vector3 side1 = v[index1].Position - v[index3].Position;
                    Vector3 side2 = v[index1].Position - v[index2].Position;
                    Vector3 normal = Vector3.Cross(side1, side2);

                    v[index1].Normal += normal;
                    v[index2].Normal += normal;
                    v[index3].Normal += normal;
                }

                for (int j = 0; j < v.Count(); j++)
                    v[j].Normal.Normalize();

                return v.ToList();
            }
        }

        [ContentSerializerIgnore]
        public List<short> Indices
        {
            get
            {
                List<short> i = new List<short>();

                foreach (Triangle tri in Triangles)
                {
                    i.Add(tri.A);
                    i.Add(tri.B);
                    i.Add(tri.C);
                }

                return i;
            }
        }

        public void Move(Vector3 vector3)
        {
            Offset = vector3;
        }

        public void Apply(TileType top, TileType topRight, TileType bottomRight, TileType bottom, TileType bottomLeft, TileType topLeft, TileType center)
        {
            _top = top;
            _topRight = topRight;
            _bottomRight = bottomRight;
            _bottom = bottom;
            _bottomLeft = bottomLeft;
            _topLeft = topLeft;
            _center = center;
        }

        public void Apply(float top, float topRight, float bottomRight, float bottom, float bottomLeft, float topLeft, float center)
        {
            _topH = top;
            _topRightH = topRight;
            _bottomRightH = bottomRight;
            _bottomH = bottom;
            _bottomLeftH = bottomLeft;
            _topLeftH = topLeft;
            _centerH = center;
        }
    }
}
