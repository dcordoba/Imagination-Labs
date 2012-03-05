#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Kinect;
using GameStateManagement.GestureSelector;


#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        InputState input = new InputState();

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;
        

        Texture2D avatar1;
        Texture2D avatar2;
        List<Texture2D> avatars;
        SkeletonTracker skeleton;

        GestureMenuScreen mainGestureMenu;
        
        bool isInitialized;

        bool traceEnabled;

        #endregion

        #region Properties
        /// <summary>
        /// A default blankTexture shared by all the screens. This saves
        /// each screen(and especially the skeleton) having to bother creating their own local instance.
        /// </summary>
        public Texture2D BlankTexture
        {
            get { return blankTexture; }
        }

        
        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SkeletonTracker CurSkeletonTracker
        {
            get { return skeleton; }
        }
     
        //Method to get the texture of the current skeleton becuase of class privacy compatabilities
        public Texture2D CurSkeletonTexture()
        {
            return skeleton.Head.Texture;
        }
        public void SetCurSkeletonTexture(int textureIndex)
        {
            if (textureIndex >= 0 && textureIndex < avatars.Count)
            {
                skeleton.Head.Texture = avatars[textureIndex];
            }
        }

        //Method to get the rectangle of the current skeleton becuase of class privacy compatabilities
        public Rectangle CurSkeletonRectangle()
        {
            return skeleton.Head.Rectangle;
        }
        //Method to get the color of the current skeleton becuase of class privacy compatabilities
        public Color CurSekeltonColor()
        {
            return skeleton.Head.Color;
        }

        
        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
        }

        /// <summary>
        /// A default avatar shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public Texture2D Avatar1
        {
            get { return avatar1; }
        }
        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }
        
       
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            avatars = new List<Texture2D>();
        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");
            InitAvatars(content);
           
            //Texture2D av1 = new Texture2D(GraphicsDevice, 50, 100);
            skeleton = new SkeletonTracker(this);
            Texture2D t_up = content.Load<Texture2D>("up");
            Texture2D t_over = content.Load<Texture2D>("over");
            Texture2D t_down = content.Load<Texture2D>("down");
            GestureMenuEntry gme1 = new GestureMenuEntry(t_up, t_over, t_down, new Rectangle(0, 0, 100, 100), "");
            GestureMenuEntry gme2 = new GestureMenuEntry(t_up, t_over, t_down, new Rectangle(0, 100, 100, 100), "");
            GestureMenuEntry gme3 = new GestureMenuEntry(t_up, t_over, t_down, new Rectangle(0, 200, 100, 100), "");
            mainGestureMenu = new GestureMenuScreen(new Rectangle(0, 0, 100, GraphicsDevice.Viewport.Height), 2000, "Main Menu", skeleton, content.Load<Texture2D>("gesture_menu"), this);
            mainGestureMenu.AddMenuItem(gme1, new Rectangle(0, 0, 100, 100));
            mainGestureMenu.AddMenuItem(gme2, new Rectangle(0, 100, 100, 100));
            mainGestureMenu.AddMenuItem(gme3, new Rectangle(0, 200, 100, 100));
            

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }

        private void InitAvatars(ContentManager content)
        {
            avatar1 = content.Load<Texture2D>("knight");
             avatar2 = content.Load<Texture2D>("batman");
           
          //  avatar1 = content.Load<Texture2D>("blank");
            Texture2D avatar3 = content.Load<Texture2D>("princess");
            Texture2D avatar4 = content.Load<Texture2D>("questionIcon");
            avatars.Add(avatar1);
            avatars.Add(avatar2);
            avatars.Add(avatar3);
            avatars.Add(avatar4);
        }
        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (traceEnabled)
                TraceScreens();
        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        ///  
      
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
            mainGestureMenu.Draw(gameTime);
            skeleton.Draw(gameTime);
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (screens.Count > 0)
            {
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            }
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            spriteBatch.End();
        }

        


        #endregion
    }
}
