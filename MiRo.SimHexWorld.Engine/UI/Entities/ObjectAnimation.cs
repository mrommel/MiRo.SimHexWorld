using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.UI.Entities
{
    public class ObjectAnimation
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 StartRotation { get; set; }
        public Vector3 EndRotation { get; set; }

        public string Duration
        {
            get { return _duration.ToString(); }
            set
            {
                try
                {
                    _duration = TimeSpan.Parse(value);
                }
                catch (Exception)
                {
                    _duration = TimeSpan.FromSeconds(10);
                }
            }
        } 

        private TimeSpan _duration { get; set; }
        public bool Loop { get; set; }

        private TimeSpan elapsedTime = TimeSpan.FromSeconds(0);

        public ObjectAnimation() { }

        public ObjectAnimation(Vector3 startPosition, Vector3 endPosition,
            Vector3 startRotation, Vector3 endRotation, TimeSpan duration,
            bool loop = false)
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.StartRotation = startRotation;
            this.EndRotation = endRotation;
            this._duration = duration;
            this.Loop = loop;

            Position = startPosition;
            Rotation = startRotation;
        }

        [ContentSerializerIgnore]
        public Vector3 Position { get; private set; }

        [ContentSerializerIgnore]
        public Vector3 Rotation { get; private set; }

        public bool Ready
        {
            get
            {
                // Determine how far along the duration value we are (0 to 1)
                float amt = (float)elapsedTime.TotalSeconds / (float)_duration.TotalSeconds;

                return amt >= 1.0f;
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update the time
            this.elapsedTime += gameTime.ElapsedGameTime;

            // Determine how far along the duration value we are (0 to 1)
            float amt = (float)elapsedTime.TotalSeconds / (float)_duration.TotalSeconds;

            if (Loop)
                while (amt > 1) // Wrap the time if we are looping
                    amt -= 1;
            else // Clamp to the end value if we are not
                amt = MathHelper.Clamp(amt, 0, 1);

            // Update the current position and rotation
            Position = Vector3.Lerp(StartPosition, EndPosition, amt);

            // todo: let the angle go the shortest way
            Rotation = Vector3.Lerp(StartRotation, EndRotation, amt);
        }
    }
}
