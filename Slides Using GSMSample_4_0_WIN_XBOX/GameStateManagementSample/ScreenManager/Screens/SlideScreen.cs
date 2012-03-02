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
    
    class SlideScreen : GameScreen
    {
        #region variables
        SlideMenuScreen parentSlideMenu;

        List<Sprite2D> slideObjects;
        Sprite2D tovSprite;
        Sprite2D questionSprite;
        SpriteBatch spriteBatch;
        Texture2D background_dirty;
        Texture2D background_active;
        Color backColor = Color.CornflowerBlue;
        ContentManager content;
        Boolean captured;
        Viewport viewport;
        
        #endregion

        #region Initialization
        public SlideScreen(SlideMenuScreen slideMenu)
        {   
            this.slideObjects =   new List<Sprite2D>();            
            this.parentSlideMenu = slideMenu;
            this.captured = false;
        }
        #endregion
        
        //Color backColor = Color.CornflowerBlue;
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            background_dirty = content.Load<Texture2D>(@"bg1_dirty");
            background_active = content.Load<Texture2D>(@"bg1_active");
            spriteBatch = ScreenManager.SpriteBatch;
            viewport = ScreenManager.GraphicsDevice.Viewport;
            
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
            //press "z" to go to change to avatar1
            if (input.IsNewKeyPress(Keys.Z, null, out requesteeIndex))
            {
                parentSlideMenu.ChangeToAvatar(0);
                Console.Out.WriteLine("Changed to 0");
            }
            //press "x" to go to change to avatar2
            if (input.IsNewKeyPress(Keys.X, null, out requesteeIndex))
            {
                parentSlideMenu.ChangeToAvatar(1);
                Console.Out.WriteLine("Changed to 1");
            }
            //press "a" to go to change to avatar2
            if (input.IsNewKeyPress(Keys.A, null, out requesteeIndex))
            {
                parentSlideMenu.ChangeToAvatar(2);
                Console.Out.WriteLine("changed to 2");
            }
            //press "m" to go to slideMenu
            if (input.IsNewKeyPress(Keys.M, null, out requesteeIndex))
            {
                //TO DO implement menu!
            }
        }

        /*Captured
         * current Capture method will print two sprites (a jpg and a png) the first time is captured.
         * subsequent capture events, will put extra question mark pngs on the screen
         */
        private void Captured()
        {
           ///*
            if (captured)//already captured once
            {
                int offset = 50 * slideObjects.Count;
                
                Rectangle rect  = questionSprite.Rectangle;
                rect.X = questionSprite.Rectangle.X - offset;
                rect.Y = questionSprite.Rectangle.Y + offset;

                //Sprite2D newQuestionSprite = new Sprite2D(questionSprite.texture, rect, questionSprite.color);
                Sprite2D newQuestionSprite = new Sprite2D(questionSprite.Texture, rect, questionSprite.Color);
               slideObjects.Add(newQuestionSprite);

                
            }/*
            else
            {
                captured = true;
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
                Rectangle smallRect = new Rectangle(viewport.Width - (viewport.Width / 8), 0, viewport.Width / 8, viewport.Height / 8);
                Color transColor = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);

                tovSprite = new Sprite2D(this.content.Load<Texture2D>("tov"), fullscreen, transColor);
                questionSprite = new Sprite2D(this.content.Load<Texture2D>("questionIcon"), smallRect, transColor);

                //slideObjects.Add(tovSprite);
               // slideObjects.Add(questionSprite);
            }
            */
            //Create a Sprite with the current Avatar
            Sprite2D curAvatar = new Sprite2D(ScreenManager.CurSkeletonTexture(), ScreenManager.CurSkeletonRectangle(), ScreenManager.CurSekeltonColor());
            slideObjects.Add(curAvatar);
        }
        #endregion
        #region Display
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.Game.GraphicsDevice.Clear(backColor);
           //draws all the sprite objects on the slide
            spriteBatch.Begin();

            spriteBatch.Draw(background_dirty, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            for(int i = 0; i < slideObjects.Count; i++){
               Sprite2D curSprite = slideObjects[i];
               spriteBatch.Draw(curSprite.Texture, curSprite.Rectangle, curSprite.Color);
            }
            spriteBatch.End();
        }
         #endregion

    }
       
     

}
