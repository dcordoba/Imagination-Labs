#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion


namespace GameStateManagement
{
    /// <summary>
    /// Base class for objects that will be drawn on a slide. Every object must be able to draw itself.
    /// </summary>
    public abstract class SlideObject
    {

       // public override void Draw(GameTime gameTime)
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        { 
            //every object must specify how to draw itself, or it will not be drawn
        }
    }
}
