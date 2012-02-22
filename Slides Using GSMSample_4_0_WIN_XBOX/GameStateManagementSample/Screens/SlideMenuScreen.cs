using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
    class SlideMenuScreen : MenuScreen
    {
         #region Initialization
        List<SlideScreen> slides; 
        int currentSlideIndex;
        //string menuTitle = "Slides Menu";
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
     public SlideMenuScreen()
          :base("Slides Menu")
        {
            slides = new List<SlideScreen>();
            currentSlideIndex = 0;
            SlideScreen firstSlide = new SlideScreen(this);
            slides.Add(firstSlide);

            // Create our menu entries.
            MenuEntry playSlideMenuEntry = new MenuEntry("Play Slides");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            MenuEntry slidesMenuEntry = new MenuEntry("Slides");

            // Hook up menu event handlers
            playSlideMenuEntry.Selected += PlayMenuEntrySelected;
          
            exitMenuEntry.Selected += OnCancel;
            slidesMenuEntry.Selected += SlidesMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(slidesMenuEntry);
            MenuEntries.Add(playSlideMenuEntry);            
            MenuEntries.Add(exitMenuEntry);
           
        }


        #endregion

        #region Handle Input
        /// <summary>
        /// Event handler for when the Slides menu entry is selected.
        /// </summary>
        void SlidesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.TraceEnabled = true;
            ScreenManager.AddScreen(slides[currentSlideIndex],e.PlayerIndex);
           // ScreenManager.AddScreen(new SlideScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "PlayBack Functionality is not implemented yet.";

            MessageBoxScreen playMessageBox = new MessageBoxScreen(message);
            playMessageBox.Accepted += PlayMessageBoxAccepted;

            ScreenManager.AddScreen(playMessageBox, e.PlayerIndex);
           // LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                              // new GameplayScreen());
        }
        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void PlayMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion

        #region Slide Menu Functions
        /// <summary> NewSlide(PlayerIndex playerIndex) or
        /// Creates a newSlide with passing the playerIndex parameter to the screenManager.
        /// note: this method takes O(n) .....
        /// </summary>
        public void NewSlide(PlayerIndex playerIndex)
        {
            //insert newSlide after the current slide
            SlideScreen slide = new SlideScreen(this);            
            slides.Insert(currentSlideIndex +1, slide);
            currentSlideIndex++;
            ScreenManager.AddScreen(slide, playerIndex);
        }/*
        public void NewSlide(SlideMenuScreen slideMenu, PlayerIndex playerIndex)
        {
            SlideScreen slide = new SlideScreen(slideMenu, slides.Count);
            slides.Add(slide);
            ScreenManager.AddScreen(slide, playerIndex);
        } */
        /// <summary> NextSlide(int curSlideIndex)
        /// Changes the screen to show the next slide in the
        /// list of slides.
        /// </summary>
        public void NextSlide(PlayerIndex playerIndex){
            //if the current slide is not the last slide
            if (currentSlideIndex < slides.Count - 1)
            {
                ScreenManager.AddScreen(slides[currentSlideIndex +1],playerIndex);
                currentSlideIndex++;
            }
        }

         /// <summary> PreviousSlide(int curSlideIndex, PlayerIndex pi)
        /// Changes the screen to show the previous slide in the
        /// list of slides.
        /// </summary>
        public void PreviousSlide(PlayerIndex playerIndex)
        {
            if (currentSlideIndex > 0)
            {
                //ScreenManager.AddScreen(slides[currentSlideIndex - 1], playerIndex);
                currentSlideIndex--;
            }
        }


        #endregion

        #region Draw
        
        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();
            IList<MenuEntry> menuEntries = MenuEntries;
            int selectedEntry = SelectedEntryIndex;

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            //Draw the slides on the bottom
            int offset = 50;
            Vector2 pos = new Vector2(0, graphics.Viewport.Height - offset);
            string slideName;
            for(int i = 0; i < slides.Count; i++){
                slideName = "Slide "+ i.ToString();
                spriteBatch.DrawString(font, slideName, pos, Color.Black);
                float x = font.MeasureString(slideName).X + offset;
                pos.X += x;
            }
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            string menuTitle = MenuTitle;

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        
        #endregion
    }
    
}
