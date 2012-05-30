using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public class MeshedRectangle : IMeshObject
    {
        public Vector3 Pos;
        public Vector3 Dim;

        public MeshedRectangle(Vector3 pos, Vector3 dim)
        {
            Pos = pos;
            Dim = dim;
        }

        public List<VertexPositionNormalTexture> Vertices
        {
            get
            {
                List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();

                vertices.Add(new VertexPositionNormalTexture(Pos + new Vector3(Dim.X, 0, Dim.Z), Vector3.Up, new Vector2(0, 0))); // 0
                vertices.Add(new VertexPositionNormalTexture(Pos + new Vector3(Dim.X, 0, -Dim.Z), Vector3.Up, new Vector2(0, 1.0f))); // 0
                vertices.Add(new VertexPositionNormalTexture(Pos + new Vector3(-Dim.X, 0, -Dim.Z), Vector3.Up, new Vector2(1.0f, 1.0f))); // 0
                vertices.Add(new VertexPositionNormalTexture(Pos + new Vector3(-Dim.X, 0, Dim.Z), Vector3.Up, new Vector2(1.0f, 0))); // 0

                return vertices;
            }
        }

        public List<short> Indices
        {
            get
            {
                return new List<short>(new short[] {
		                         0,  1,  2,  
		                         0,  3,  2 });

            }
        }
    }
}
