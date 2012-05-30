using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public abstract class AbstractCamera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        protected GraphicsDevice GraphicsDevice { get; set; }

        public AbstractCamera(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            GeneratePerspectiveProjectionMatrix(MathHelper.PiOver4);
        }
        private void GeneratePerspectiveProjectionMatrix(float fieldOfView)
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            float aspectRatio = (float)pp.BackBufferWidth /
                                (float)pp.BackBufferHeight;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000000.0f);
        }
        public virtual void Update()
        {
        }
    }
}