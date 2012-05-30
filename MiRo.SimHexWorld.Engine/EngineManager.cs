using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiRoSimHexWorld.Engine
{
    public class EngineManager : GameEngine
    {
        private static Game _game;

        /// <summary>
        /// The XNA game.
        /// </summary>
        public static Game Game
        {
            get { return _game; }
            set { _game = value; }
        }

        public EngineManager(string unitTestName)
            : base(unitTestName)
        {
        }

        public EngineManager()
            : base("Engine")
        {
        }

        protected override void Draw(GameTime gameTime)
        {           
            Device.Clear(BackgroundColor);
            base.Draw(gameTime);
        }
    }
}
