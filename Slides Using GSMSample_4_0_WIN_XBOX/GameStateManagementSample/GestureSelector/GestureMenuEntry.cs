using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;

namespace GameStateManagement.GestureSelector
{
    class GestureMenuEntry : MenuEntry
    {
        #region Private Vars
        Texture2D _up;
        Texture2D _over;
        Texture2D _down;
        Rectangle _pos;
        #endregion

        #region Public Vars
        public enum MenuEntryState { UP, OVER, DOWN };
        #endregion


        #region Initialization
        public GestureMenuEntry(Texture2D up, Texture2D over, Texture2D down, Rectangle pos, String text)
            : base(text)
        {
            this._up = up;
            this._over = over;
            this._down = down;
            this._pos = pos;
        }
        #endregion

        protected internal override void OnSelectEntry(PlayerIndex playerIndex)
        {
            Console.Out.WriteLine("Selected");
            base.OnSelectEntry(playerIndex);
        }

        #region Draw and Update

        public void Draw(MenuScreen screen, MenuEntryState state, GameTime gameTime)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            Texture2D drawTexture;
            switch (state)
            {
                case MenuEntryState.OVER:
                    drawTexture = this._over;
                    break;
                case MenuEntryState.DOWN:
                    drawTexture = this._down;
                    break;
                default:
                    drawTexture = this._up;
                    break;
            }
            spriteBatch.Draw(drawTexture, this._pos, Color.White);

        }
        #endregion
    }
}
