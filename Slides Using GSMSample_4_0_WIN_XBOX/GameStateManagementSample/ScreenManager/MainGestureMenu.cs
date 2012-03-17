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
    public class MainGestureMenu
    {
        GestureMenuScreen _mainGestureMenu;
        GestureMenuScreen _backgroundGestureMenu;
        GestureMenuScreen _avatarGestureMenu;
        GestureMenuScreen _exportGestureMenu;
        ScreenManager screenManager;
        int menuWidth;
        public int Width
        {
            get { return menuWidth; }
        }
        bool _backgroundActivate = false;

        public MainGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            _backgroundGestureMenu = InitBackgroundGestureMenu(GD, content, skeleton, sm);
            _avatarGestureMenu = InitAvatarGestureMenu(GD, content, skeleton, sm);
            _exportGestureMenu = InitExportGestureMenu(GD, content, skeleton, sm);
            _mainGestureMenu = InitMainGestureMenu(GD, content, skeleton, sm);
            screenManager = sm;
        }
        private GestureMenuScreen InitBackgroundGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            Texture2D background_bar = content.Load<Texture2D>("places menu/places menu bar");
            Texture2D empty = new Texture2D(GD, 1, 1);

            Texture2D desert_up          = content.Load<Texture2D>("places menu/desert");
            Texture2D desert_over        = content.Load<Texture2D>("places menu/desert (hilite)1");
            Texture2D fantasy_hills_up   = content.Load<Texture2D>("places menu/fantasy hills");
            Texture2D fantasy_hills_over = content.Load<Texture2D>("places menu/fantasy hills (hilite)");
            Texture2D outer_space_up     = content.Load<Texture2D>("places menu/outer space");
            Texture2D outer_space_over   = content.Load<Texture2D>("places menu/outer space (hilite)");
            Texture2D regular_hills_up   = content.Load<Texture2D>("places menu/regular hills");
            Texture2D regular_hills_over = content.Load<Texture2D>("places menu/regular hills (hilite)");
            Texture2D undersea_up        = content.Load<Texture2D>("places menu/undersea");
            Texture2D undersea_over      = content.Load<Texture2D>("places menu/undersea (hilite)");
            int X = 0;
            int X_1 = 80;
            int X_2 = 180;
            int X_3 = 280;
            int X_4 = 380;
            int X_5 = 480;
            int Y = 138;
            int Y_elem = 100;
            int Width  = GD.Viewport.Width * 21 / 30;
            int Height = Width * 68 / 1063;
            int menu_Width  = 181 * 45 / 100;
            int menu_Height = menu_Width * 129 / 181;
            GestureMenuScreen backgroundGestureMenu = new GestureMenuScreen(new Rectangle(X, Y, Width, Height), 2000, "Background", skeleton, background_bar, background_bar, empty, sm);
            backgroundGestureMenu.Disabled = true;
            GestureMenuEntry desert         = new GestureMenuEntry(desert_up, desert_over, desert_over, empty, new Rectangle(X_1, Y_elem, menu_Width, menu_Height), "desert");
            GestureMenuEntry fantasy_hills = new GestureMenuEntry(fantasy_hills_up, fantasy_hills_over, fantasy_hills_over, empty, new Rectangle(X_2, Y_elem, menu_Width, menu_Height), "fantasy_hills");
            GestureMenuEntry outer_space = new GestureMenuEntry(outer_space_up, outer_space_over, outer_space_over, empty, new Rectangle(X_3, Y_elem, menu_Width, menu_Height), "outer_space");
            GestureMenuEntry regular_hills = new GestureMenuEntry(regular_hills_up, regular_hills_over, regular_hills_over, empty, new Rectangle(X_4, Y_elem, menu_Width, menu_Height), "regular_hills");
            GestureMenuEntry undersea = new GestureMenuEntry(undersea_up, undersea_over, undersea_over, empty, new Rectangle(X_5, Y_elem, menu_Width, menu_Height), "undersea");
            desert.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            fantasy_hills.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            outer_space.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            regular_hills.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            undersea.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            backgroundGestureMenu.AddMenuItem(desert, new Rectangle(X_1, Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(fantasy_hills,    new Rectangle(X_2,  Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(outer_space,      new Rectangle(X_3,  Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(regular_hills,    new Rectangle(X_4,  Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(undersea,         new Rectangle(X_5,  Y_elem, menu_Width, menu_Height));
            return backgroundGestureMenu;
        }
        private GestureMenuScreen InitAvatarGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            return null;
        }
        private GestureMenuScreen InitExportGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            return null;
        }

        private GestureMenuScreen InitMainGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            Texture2D empty = new Texture2D(GD, 1, 1);
            Texture2D t_up = empty;
            int Height = GD.Viewport.Height;
            int Width = GD.Viewport.Height * 155 / 1000;
            this.menuWidth = Width;
            KeyValuePair<Texture2D, Rectangle> sideDock = new KeyValuePair<Texture2D, Rectangle>(content.Load<Texture2D>("menu/menu_sideDock"), new Rectangle(0, 0, 50, Height));
            Texture2D t_over = content.Load<Texture2D>("menu/menu_circleHighlight");
            Texture2D t_down = content.Load<Texture2D>("menu/menu_circleHighlight");
            GestureMenuEntry gme1 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 0, Width, Width), "");
            GestureMenuEntry gme2 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 100, Width, Width), "background");
            GestureMenuEntry gme3 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 200, Width, Width), "");
            GestureMenuEntry gme4 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 300, Width, Width), "");
            GestureMenuEntry gme5 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 400, Width, Width), "");
            gme2.Selected += new EventHandler<PlayerIndexEventArgs>(ActivateBackgroundScreen);
            GestureMenuScreen mainGestureMenu = new GestureMenuScreen(new Rectangle(0, 0, Width, Height), 2000, "Main Menu", skeleton, content.Load<Texture2D>("menu/menu_sideIcons_active"), content.Load<Texture2D>("menu/menu_sideIcons_idle"), empty, sm);
            mainGestureMenu.Disabled = true;
            mainGestureMenu.Other = sideDock;
            mainGestureMenu.AddMenuItem(gme1, new Rectangle(0, 0, Width, Width));
            mainGestureMenu.AddMenuItem(gme2, new Rectangle(0, 100, Width, Width));
            mainGestureMenu.AddMenuItem(gme3, new Rectangle(0, 200, Width, Width));
            mainGestureMenu.AddMenuItem(gme4, new Rectangle(0, 300, Width, Width));
            mainGestureMenu.AddMenuItem(gme5, new Rectangle(0, 400, Width, Width));
            return mainGestureMenu;
        }

        private void ActivateBackgroundScreen(object sender, PlayerIndexEventArgs p)
        {
            _backgroundActivate = true;
            _backgroundGestureMenu.Disabled = false;
        }

        private void SelectBackground(object sender, PlayerIndexEventArgs p)
        {
            int backgroundIndex = -1;
            switch (((GestureMenuEntry)sender).Text)
            {
                case "desert":
                    break;
                case "fantasy_hills":
                    backgroundIndex = 0;
                    break;
                case "outer_space":
                    backgroundIndex = 1;
                    break;
                case "regular_hills":
                    break;
                case "undersea":
                    break;
                default:
                    break;
            }
            if (backgroundIndex != -1)
                ((SlideScreen)screenManager.GetScreens()[screenManager.NumScreens - 1]).ChangeBackground(backgroundIndex);
            _backgroundGestureMenu.Disabled = true;
        }
        public void DisableMainScreen()
        {
            _mainGestureMenu.Disabled = true;
            _backgroundGestureMenu.Disabled = true;
        }

        public void EnableMainScreen()
        {
            _mainGestureMenu.Disabled = false;
        }

        public void Draw(GameTime gametime)
        {
            _backgroundGestureMenu.Draw(gametime, 0.1F);
            _mainGestureMenu.Draw(gametime, 0.0F);
        }

    }
}
