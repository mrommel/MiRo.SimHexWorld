using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MiRoSimHexWorld.Engine
{
    public class Input : GameComponent
    {
        public KeyboardState CurrentKeyboardState;
        public GamePadState CurrentGamePadState;
        public MouseState CurrentMouseState;

        public KeyboardState LastKeyboardState;
        public GamePadState LastGamePadState;
        public MouseState LastMouseState;

        private Point _lastMouseLocation;

        private Vector2 _mouseMoved;
        public Vector2 MouseMoved
        {
            get { return _mouseMoved; }
        }

        public Input(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePadState = CurrentGamePadState;
            LastMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentMouseState = Mouse.GetState();

            _mouseMoved = new Vector2(LastMouseState.X - CurrentMouseState.X, LastMouseState.Y - CurrentMouseState.Y);
            _lastMouseLocation = new Point(CurrentMouseState.X, CurrentMouseState.Y);
        }

        /// <summary>
        /// Checks for a "menu up" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
                       (CurrentGamePadState.DPad.Up == ButtonState.Pressed &&
                        LastGamePadState.DPad.Up == ButtonState.Released) ||
                       (CurrentGamePadState.ThumbSticks.Left.Y > 0 &&
                        LastGamePadState.ThumbSticks.Left.Y <= 0);
            }
        }

        /// <summary>
        /// Checks for a "menu down" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
                       (CurrentGamePadState.DPad.Down == ButtonState.Pressed &&
                        LastGamePadState.DPad.Down == ButtonState.Released) ||
                       (CurrentGamePadState.ThumbSticks.Left.Y < 0 &&
                        LastGamePadState.ThumbSticks.Left.Y >= 0);
            }
        }

        /// <summary>
        /// Checks for a "menu select" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       (CurrentGamePadState.Buttons.A == ButtonState.Pressed &&
                        LastGamePadState.Buttons.A == ButtonState.Released) ||
                       (CurrentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Start == ButtonState.Released);
            }
        }

        /// <summary>
        /// Checks for a "menu cancel" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePadState.Buttons.B == ButtonState.Pressed &&
                        LastGamePadState.Buttons.B == ButtonState.Released) ||
                       (CurrentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Back == ButtonState.Released);
            }
        }

        /// <summary>
        /// Checks for a "pause the game" input action (on either keyboard or gamepad).
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Back == ButtonState.Released) ||
                       (CurrentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Start == ButtonState.Released);
            }
        }

        public bool IsMouseRightDown
        {
            get
            {
                return CurrentMouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released;
            }
        }

        public bool IsMouseLeftDown
        {
            get
            {
                return CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released;
            }
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) &&
                    LastKeyboardState.IsKeyUp(key));
        }
    }
}
