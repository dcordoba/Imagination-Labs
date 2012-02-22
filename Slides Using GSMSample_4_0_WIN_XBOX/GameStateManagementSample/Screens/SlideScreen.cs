using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GameStateManagement
{
    public struct SpriteObject 
    {
        public Texture2D texture;
        public Color color;
        public Rectangle rec;


        public SpriteObject(Texture2D tex,  Rectangle r, Color c)
        {
            this.texture = tex;            
            this.rec = r;
            this.color = c;
        }
    }


    class SlideScreen : GameScreen
    {
        #region variables
        SlideMenuScreen parentSlideMenu;
        

        List<SpriteObject> slideObjects;
        SpriteObject tovSprite;
        SpriteObject questionSprite;
        SpriteBatch spriteBatch;
               
        Color backColor = Color.CornflowerBlue;
        ContentManager content;
        Boolean captured;
       
        
        #endregion

        #region Initialization
        public SlideScreen(SlideMenuScreen slideMenu)
        {   
            this.slideObjects =   new List<SpriteObject>();            
            this.parentSlideMenu = slideMenu;
            this.captured = false;
        }
        #endregion
        
        //Color backColor = Color.CornflowerBlue;
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            
            spriteBatch = new SpriteBatch(ScreenManager.Game.GraphicsDevice);
           // tovTexture = this.content.Load<Texture2D>("tov");
           // back
           // backgroundTexture =  //content.Load<Texture2D>("background");
        }
        #region Handle Input
         /// <summary>
        /// Responds to user input, capturing...
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex requesteeIndex;
            //press c to "capture"
            if(input.IsNewKeyPress(Keys.C, null,out requesteeIndex)){
                Captured();
            }
            //press "b" or the left arrow key to go back one slide
            if(input.IsNewKeyPress(Keys.B,null, out requesteeIndex)||input.IsNewKeyPress(Keys.Left, null, out requesteeIndex)){
                parentSlideMenu.PreviousSlide(requesteeIndex);
                this.ExitScreen();
            }
           
            //press right arrow key to move forward one slide
            if (input.IsNewKeyPress(Keys.Right, null, out requesteeIndex))
            {
                parentSlideMenu.NextSlide(requesteeIndex);
                
            }
            //press "n" to create a new slide
            if (input.IsNewKeyPress(Keys.N, null, out requesteeIndex))
            {
                parentSlideMenu.NewSlide(requesteeIndex);
            }
            //press "m" to go to slideMenu
            if (input.IsNewKeyPress(Keys.M, null, out requesteeIndex))
            {
                
            }
        }

        /*Captured
         * current Capture method will print two sprites (a jpg and a png) the first time is captured.
         * subsequent capture events, will put extra question mark pngs on the screen
         */
        private void Captured()
        {
            if (captured)//already captured once
            {
                int offset = 50 * slideObjects.Count;
                
                Rectangle rect  = questionSprite.rec;
                rect.X = questionSprite.rec.X - offset;
                rect.Y = questionSprite.rec.Y + offset;

                SpriteObject newQuestionSprite = new SpriteObject(questionSprite.texture, rect, questionSprite.color);
                slideObjects.Add(newQuestionSprite);              

            }
            else
            {
                captured = true;
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
                Rectangle smallRect = new Rectangle(viewport.Width - (viewport.Width / 8), 0, viewport.Width / 8, viewport.Height / 8);
                Color transColor = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);

                tovSprite = new SpriteObject(this.content.Load<Texture2D>("tov"), fullscreen, transColor);
                questionSprite = new SpriteObject(this.content.Load<Texture2D>("questionIcon"), smallRect, transColor);

                slideObjects.Add(tovSprite);
                slideObjects.Add(questionSprite);
            }
        }
        #endregion
        #region Display
        public override void Draw(GameTime gameTime)
        {
            
           /* GraphicsDevice.Clear(backColor);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            spriteBatch.Begin();
            

            spriteBatch.End();
            base.Draw(gameTime);
            */

           //spriteBatch = ScreenManager.SpriteBatch;
           
            ScreenManager.Game.GraphicsDevice.Clear(Color.DeepSkyBlue);
          //  if(captured){
                //draws all the sprite objects on the slide
                spriteBatch.Begin();
                for(int i = 0; i < slideObjects.Count; i++){
                    SpriteObject curSprite = slideObjects[i];
                    spriteBatch.Draw(curSprite.texture, curSprite.rec, curSprite.color);
                }
               
                spriteBatch.End();
          //  }
        }
         #endregion

    }
       
     

}
