using MiRo.SimHexWorld.Engine.World.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiRoSimHexWorld.Engine.World.Helper
{
    public class ArcBallCamera : AbstractCamera
    {
        // Rotation around the two axes
        public float RotationX { get; set; }
        public float RotationY { get; set; }

        // Y axis rotation limits (radians)
        public float MinRotationY { get; set; }
        public float MaxRotationY { get; set; }

        // Distance between the target and camera
        public float Distance { get; set; }

        // Distance limits
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }

        // Calculated position and specified target
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; set; }

        public ArcBallCamera(Vector3 target,
                             float rotationX,
                             float rotationY, float minRotationY, float maxRotationY,
                             float distance, float minDistance, float maxDistance,
                             GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.Target = target;
            this.MinRotationY = minRotationY;
            this.MaxRotationY = maxRotationY;

            // Lock the y axis rotation between the min and max values
            this.RotationY = MathHelper.Clamp(rotationY, minRotationY, maxRotationY);

            this.RotationX = rotationX;
            this.MinDistance = minDistance;
            this.MaxDistance = maxDistance;

            // Lock the distance between the min and max values
            this.Distance = MathHelper.Clamp(distance, minDistance, maxDistance);
        }

        public void Move(float distanceChange)
        {
            this.Distance += distanceChange;
            this.Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Rotate(float rotationXChange, float rotationYChange)
        {
            this.RotationX += rotationXChange;
            this.RotationY += -rotationYChange;
            this.RotationY = MathHelper.Clamp(RotationY, MinRotationY, MaxRotationY);
        }

        public void Translate(Vector3 positionChange)
        {
            this.Position += positionChange;
        }

        public override void Update()
        {
            // Calculate rotation matrix from rotation values
            Matrix rotation = Matrix.CreateFromYawPitchRoll(RotationX, -RotationY, 0);

            // Translate down the Z axis by the desired distance
            // between the camera and object, then rotate that
            // vector to find the camera offset from the target
            Vector3 translation = new Vector3(0, 0, Distance);
            translation = Vector3.Transform(translation, rotation);
            Position = Target + translation;

            // Calculate the up vector from the rotation matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            View = Matrix.CreateLookAt(Position, Target, up);
        }

        public Vector3 Up
        {
            get
            {
                // Calculate the rotation matrix
                Matrix rotation = Matrix.CreateFromYawPitchRoll(RotationX, -RotationY, 0);

                return Vector3.Transform(Vector3.Up, rotation);
            }
        }

        public Vector3 Right
        {
            get
            {
                // Calculate the rotation matrix
                Matrix rotation = Matrix.CreateFromYawPitchRoll(RotationX, -RotationY, 0);

                return Vector3.Transform(Vector3.Right, rotation);
            }
        }
    }
}