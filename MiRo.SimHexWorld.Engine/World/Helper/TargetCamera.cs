using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public class TargetCamera : AbstractCamera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }

        public TargetCamera(Vector3 position, Vector3 target, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Position = position;
            this.Target = target;
        }

        public override void Update()
        {
            Vector3 forward = Target - Position;
            Vector3 side = Vector3.Cross(forward, Vector3.Up);
            Vector3 up = Vector3.Cross(forward, side);
            this.View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}