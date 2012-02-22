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

    class Sprite2D
    {
        #region vars
        Texture2D texture;
        Rectangle rec;
        Color color;
        #endregion
        #region Initialization
        public Sprite2D(Texture2D tex,  Rectangle r, Color c)
        {
            this.texture = tex;            
            this.rec = r;
            this.color = c;
        }
        #endregion
        #region Public Methods
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Rectangle Rectangle
        {
            get { return rec; }
            set { rec = value; }
        }
        public void SetRectPos(int x, int y){
            rec.X = x;
            rec.Y = y;
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }


        #endregion
    }
}
