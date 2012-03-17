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
        Texture2D _disabled;
        Rectangle _pos;
        #endregion

        #region Public Vars
        public enum MenuEntryState { UP, OVER, DOWN, DISABLED };
        String _text;
        public event EventHandler<PlayerIndexEventArgs> Unselected;
        public String Text
        {
            get { return _text; }
        }
        #endregion


        #region Initialization
        public GestureMenuEntry(Texture2D up, Texture2D over, Texture2D down, Texture2D disabled, Rectangle pos, String text)
            : base(text)
        {
            this._up = up;
            this._over = over;
            this._down = down;
            this._disabled = disabled;
            this._pos = pos;
            this._text = text;
        }
        #endregion

        protected internal override void OnSelectEntry(PlayerIndex playerIndex)
        {
            base.OnSelectEntry(playerIndex);
        }

        public void OnUnselectEntry(PlayerIndex pi)
        {
            if (Unselected != null)
                Unselected(this, new PlayerIndexEventArgs(pi));
        }

        #region Draw and Update

        public void Draw(MenuScreen screen, MenuEntryState state, GameTime gameTime, float sortmode)
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
                case MenuEntryState.UP:
                    drawTexture = this._up;
                    break;
                default:
                    drawTexture = this._disabled;
                    break;
            }
            spriteBatch.Draw(drawTexture, this._pos, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.48F + sortmode);
        }
        #endregion
    }
}