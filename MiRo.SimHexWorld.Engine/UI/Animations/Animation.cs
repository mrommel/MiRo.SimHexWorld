using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MiRo.SimHexWorld.Engine.UI.Controls;
using log4net;

namespace MiRo.SimHexWorld.Engine.UI.Animations
{
    public class Animation
    {
        ILog log = LogManager.GetLogger(typeof(Animation));

        private bool reseted = false;
        private bool started = false;
        private GameTime startTime;
        private Interpolator _interpolator;
        private Grid9Control _control;

        public Animation()
        {
            interpolator = "linear";
            _interpolator = new LinearInterpolator();
        }

        [ContentSerializer(Optional = true)]
        public List<Animation> set { get; set; }

        #region general

        /// <summary>
        /// in milliseconds
        /// </summary>
        public int duration { get; set; }

        /// <summary>
        /// in milliseconds
        /// </summary>
        public int startOffset { get; set; }

        [ContentSerializer(Optional = true)]
        public String interpolator { get; set; }

        #endregion

        #region alpha

        private bool alphaFinished = false;

        /// <summary>
        /// start alpha value from 0 to 1
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float fromAlpha { get; set; }

        /// <summary>
        /// target alpha value from 0 to 1
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float toAlpha { get; set; }

        #endregion alpha

        public virtual void Update(GameTime time)
        {
            if (reseted)
            {
                startTime = time;
                reseted = false;
            }

            if (set != null)
            {
                foreach (Animation a in set)
                    a.Update(time);
            }

            if (started && !IsFinished)
            {
                // fading
                if (fromAlpha != toAlpha)
                {
                    double elapsed = time.TotalGameTime.TotalMilliseconds - startTime.TotalGameTime.TotalMilliseconds;

                    // wait
                    double x = elapsed - startOffset;
                    if (x > 0)
                    {
                        double lerp = x / duration;

                        if (lerp <= 1)
                        {
                            _control.Alpha = (byte)(MathHelper.Lerp(fromAlpha * 255, toAlpha * 255, (float)lerp));
                            log.InfoFormat("alpha: {0}, from: {1}, to: {2}, lerp: {3}", _control.Alpha, fromAlpha * 255, toAlpha * 255, lerp);
                            alphaFinished = false;
                        }
                        else
                            alphaFinished = true;
                    }
                    else
                        alphaFinished = false;
                }
                else
                    alphaFinished = true;
            }
        }

        public bool IsFinished
        {
            get
            {
                return alphaFinished;
            }
        }

        public void Start(Grid9Control control)
        {
            _control = control;

            reseted = true;
            started = true;
        }
    }
}
