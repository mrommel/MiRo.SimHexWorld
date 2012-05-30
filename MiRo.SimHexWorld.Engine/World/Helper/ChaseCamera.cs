using MiRoSimHexWorld.Engine.World.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public class ChaseCamera : AbstractCamera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 FollowTargetPosition { get; private set; }
        public Vector3 FollowTargetRotation { get; private set; }
        public Vector3 PositionOffset { get; set; }
        public Vector3 TargetOffset { get; set; }
        public Vector3 RelativeCameraRotation { get; set; }

        float _springiness = .15f;
        public float Springiness
        {
            get { return _springiness; }
            set { _springiness = MathHelper.Clamp(value, 0, 1); }
        }
        public ChaseCamera(Vector3 positionOffset, Vector3 targetOffset,
                           Vector3 relativeCameraRotation, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.PositionOffset = positionOffset;
            this.TargetOffset = targetOffset;
            this.RelativeCameraRotation = relativeCameraRotation;
        }
        public void Move(Vector3 newFollowTargetPosition,
                         Vector3 newFollowTargetRotation)
        {
            this.FollowTargetPosition = newFollowTargetPosition;
            this.FollowTargetRotation = newFollowTargetRotation;
        }
        public void Rotate(Vector3 rotationChange)
        {
            this.RelativeCameraRotation += rotationChange;
        }
        public override void Update()
        {
            // Sum the rotations of the model and the camera to ensure it
            // is rotated to the correct position relative to the model's
            // rotation
            Vector3 combinedRotation = FollowTargetRotation +
                                       RelativeCameraRotation;

            // Calculate the rotation matrix for the camera
            Matrix rotation = Matrix.CreateFromYawPitchRoll(
                combinedRotation.Y, combinedRotation.X, combinedRotation.Z);

            // Calculate the position the camera would be without the spring
            // value, using the rotation matrix and target position
            Vector3 desiredPosition = FollowTargetPosition +
                                      Vector3.Transform(PositionOffset, rotation);

            // Interpolate between the current position and desired position
            Position = Vector3.Lerp(Position, desiredPosition, Springiness);

            // Calculate the new target using the rotation matrix
            Target = FollowTargetPosition + Vector3.Transform(TargetOffset,
                                                              rotation);

            // Obtain the up vector from the matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            // Recalculate the view matrix
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}