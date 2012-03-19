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
        GestureMenuScreen _narrationGestureMenu;

        ScreenManager screenManager;
        int menuWidth;
        int menuActivationWaitTime = 1000;
        public int Width
        {
            get { return menuWidth; }
        }
        bool _backgroundActivate = false;
        #region icon_grid
        int X = 0;
        int X_1 = 80;
        int X_2 = 180;
        int X_3 = 280;
        int X_4 = 380;
        int X_5 = 480;
        int Y = 138;
        int Y_elem = 100;
        int width;
        int height;
        int menu_Width;
        int menu_Height;
        #endregion

        public MainGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            width = GD.Viewport.Width * 21 / 30;
            height = width * 68 / 1063;
            menu_Width = 181 * 45 / 100;
            menu_Height = menu_Width * 129 / 181;
            _backgroundGestureMenu = InitBackgroundGestureMenu(GD, content, skeleton, sm);
            _avatarGestureMenu = InitAvatarGestureMenu(GD, content, skeleton, sm);
            _exportGestureMenu = InitExportGestureMenu(GD, content, skeleton, sm);
            _narrationGestureMenu = InitRecordGestureMenu(GD, content, skeleton,sm);
            _mainGestureMenu = InitMainGestureMenu(GD, content, skeleton, sm);
            screenManager = sm;
        }
        private GestureMenuScreen InitRecordGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            Texture2D narration_bar = content.Load<Texture2D>("narration/narrate_menu_bar");
            Texture2D empty = new Texture2D(GD, 1, 1);
            Texture2D play_up = content.Load<Texture2D>("narration/playback_button_idle");
            Texture2D play_hover = content.Load<Texture2D>("narration/playback_button_hover");
            Texture2D play_pushed = content.Load<Texture2D>("narration/playback_button_pushed");
            Texture2D record_up = content.Load<Texture2D>("narration/record_button_idle");
            Texture2D record_hover = content.Load<Texture2D>("narration/record_button_hover");
            Texture2D record_pushed = content.Load<Texture2D>("narration/record_button_pushed");
            int x_bar =0;
            int y_bar = 200 + narration_bar.Height/6;
            int spaceBetweenEntries = 20;
            GestureMenuScreen narrationGestureMenu = new GestureMenuScreen(new Rectangle(x_bar, y_bar, width, height), menuActivationWaitTime, "Record", skeleton, narration_bar, narration_bar, empty, sm);
            narrationGestureMenu.Disabled =  true;
            //creating gesture menu entries
            int ratio = 60;
            
            int Y_elem = y_bar - narration_bar.Height/5;
            int X_elem1 = x_bar + menuWidth;
            int X_elem2 = X_elem1 + spaceBetweenEntries + play_up.Width;
            GestureMenuEntry playNarration = new GestureMenuEntry(play_up, play_hover, play_pushed, empty, new Rectangle(X_1, Y_elem, menu_Width, menu_Height), "narration_play");
            GestureMenuEntry recordNarration = new GestureMenuEntry(play_up, play_hover, play_pushed, empty, new Rectangle(X_1, Y_elem, menu_Width, menu_Height), "narration_record");
            //attaching event handlers to menu entries
            playNarration.Selected += new EventHandler<PlayerIndexEventArgs>(PlayNarration);
            

            //adding menu entries to narration menu
            narrationGestureMenu.AddMenuItem(playNarration, new Rectangle(X_1, Y_elem, play_up.Width * ratio / 100, play_up.Height * ratio / 100));
            narrationGestureMenu.AddMenuItem(playNarration, new Rectangle(X_1, Y_elem, play_up.Width * ratio / 100, play_up.Height * ratio / 100));
            return narrationGestureMenu;
        }
        private GestureMenuScreen InitBackgroundGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            int Y_elem_2 = Y_elem + menu_Height + 30;
            Texture2D background_bar = content.Load<Texture2D>("places menu/places menu bar");
            Texture2D empty = new Texture2D(GD, 1, 1);

            Texture2D beach_up = content.Load<Texture2D>("places menu/places/beach/o.beach_icon_idle");
            Texture2D beach_over = content.Load<Texture2D>("places menu/places/beach/o.beach_icon_hover");
            Texture2D coral_up = content.Load<Texture2D>("places menu/places/coralreef/o.coralreef_icon_idle");
            Texture2D coral_over = content.Load<Texture2D>("places menu/places/coralreef/o.coralreef_icon_hover");
            Texture2D fantasy_up = content.Load<Texture2D>("places menu/places/fantasy house/o.fantasyhouse_icon_idle");
            Texture2D fantasy_over = content.Load<Texture2D>("places menu/places/fantasy house/o.fantasyhouse_icon_hover");
            Texture2D monument_up = content.Load<Texture2D>("places menu/places/monument park/o.monumpark_icon_idle");
            Texture2D monument_over = content.Load<Texture2D>("places menu/places/monument park/o.monumpark_icon_hover");
            Texture2D rainforest_up = content.Load<Texture2D>("places menu/places/rainforest/o.rainforest_icon_idle");
            Texture2D rainforest_over = content.Load<Texture2D>("places menu/places/rainforest/o.rainforest_icon_hover");
            Texture2D snowy_up = content.Load<Texture2D>("places menu/places/snowy forest/o.snowyforest_icon_idle");
            Texture2D snowy_over = content.Load<Texture2D>("places menu/places/snowy forest/o.snowyforest_icon_hover");
            Texture2D desert_up   = content.Load<Texture2D>("places menu/places/desert pyramids/o.desert_icon_idle");
            Texture2D desert_over = content.Load<Texture2D>("places menu/places/desert pyramids/o.desert_icon_hover");
            GestureMenuScreen backgroundGestureMenu = new GestureMenuScreen(new Rectangle(X, Y, width, height), 2000, "Background", skeleton, background_bar, background_bar, empty, sm);
            backgroundGestureMenu.Disabled = true;
            GestureMenuEntry beach = new GestureMenuEntry(beach_up, beach_over, beach_over, empty, new Rectangle(X_1, Y_elem, menu_Width, menu_Height), "beach");
            GestureMenuEntry coral = new GestureMenuEntry(coral_up, coral_over, coral_over, empty, new Rectangle(X_2, Y_elem, menu_Width, menu_Height), "coral");
            GestureMenuEntry fantasy = new GestureMenuEntry(fantasy_up, fantasy_over, fantasy_over, empty, new Rectangle(X_3, Y_elem, menu_Width, menu_Height), "fantasy");
            GestureMenuEntry monument = new GestureMenuEntry(monument_up, monument_over, monument_over, empty, new Rectangle(X_4, Y_elem, menu_Width, menu_Height), "monument");
            GestureMenuEntry rainforest = new GestureMenuEntry(rainforest_up, rainforest_over, rainforest_over, empty, new Rectangle(X_5, Y_elem, menu_Width, menu_Height), "rainforest");
            GestureMenuEntry snowy = new GestureMenuEntry(snowy_up, snowy_over, snowy_over, empty, new Rectangle(X_1, Y_elem_2, menu_Width, menu_Height), "snowy");
            GestureMenuEntry desert = new GestureMenuEntry(desert_up, desert_over, desert_over, empty, new Rectangle(X_2, Y_elem_2, menu_Width, menu_Height), "desert");

            beach.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            coral.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            fantasy.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            monument.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            rainforest.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            snowy.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            desert.Selected += new EventHandler<PlayerIndexEventArgs>(SelectBackground);
            backgroundGestureMenu.AddMenuItem(beach, new Rectangle(X_1, Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(coral, new Rectangle(X_2, Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(fantasy, new Rectangle(X_3, Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(monument, new Rectangle(X_4, Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(rainforest, new Rectangle(X_5, Y_elem, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(snowy, new Rectangle(X_1, Y_elem_2, menu_Width, menu_Height));
            backgroundGestureMenu.AddMenuItem(desert, new Rectangle(X_2, Y_elem_2, menu_Width, menu_Height));
            return backgroundGestureMenu;
        }
        private GestureMenuScreen InitAvatarGestureMenu(GraphicsDevice GD, ContentManager content, Character skeleton, ScreenManager sm)
        {
            int Y = 35;
            int Y_elem = 0;
            int X_1 = 80;
            int X_2 = 200;
            int X_3 = 340;
            int X_4 = 390;
            int X_4_2 = 440;

            Texture2D avatar_bar = content.Load<Texture2D>("places menu/places menu bar");
            Texture2D empty = new Texture2D(GD, 1, 1);
            Texture2D dragon_up = content.Load<Texture2D>("avatars/dragon_icon_idle");
            Texture2D dragon_over = content.Load<Texture2D>("avatars/dragon_icon_hover");
            Texture2D girl_up = content.Load<Texture2D>("avatars/girl_icon_idle");
            Texture2D girl_over = content.Load<Texture2D>("avatars/girl_icon_hover");
            Texture2D pocahon_up = content.Load<Texture2D>("avatars/pocahon_icon_idle");
            Texture2D pocahon_over = content.Load<Texture2D>("avatars/pocahon_icon_hover");
            Texture2D wknight_up = content.Load<Texture2D>("avatars/w.knight_icon_idle");
            Texture2D wknight_over = content.Load<Texture2D>("avatars/w.knight_icon_hover");
            int ratio = 60;
            int ratio_2 = 55;
            GestureMenuScreen avatarGestureMenu = new GestureMenuScreen(new Rectangle(X, Y, width, height), 2000, "Avatars", skeleton, avatar_bar, avatar_bar, empty, sm);
            avatarGestureMenu.Disabled = true; // TODO make true
            GestureMenuEntry dragon = new GestureMenuEntry(dragon_up, dragon_over, dragon_over, empty, new Rectangle(X_1, Y_elem, dragon_up.Width * ratio / 100, dragon_up.Height * ratio / 100), "dragon");
            GestureMenuEntry girl = new GestureMenuEntry(girl_up, girl_over, girl_over, empty, new Rectangle(X_2, Y_elem, girl_up.Width * ratio / 100, girl_up.Height * ratio / 100), "girl");
            GestureMenuEntry pocahon = new GestureMenuEntry(pocahon_up, pocahon_over, pocahon_over, empty, new Rectangle(X_3, Y_elem, pocahon_up.Width * ratio / 100, pocahon_up.Height * ratio / 100), "pocahon");
            GestureMenuEntry wknight = new GestureMenuEntry(wknight_up, wknight_over, wknight_over, empty, new Rectangle(X_4, Y_elem, wknight_up.Width * ratio / 100, wknight_up.Height * ratio / 100), "wknight");
            dragon.Selected += new EventHandler<PlayerIndexEventArgs>(SelectAvatar);
            girl.Selected += new EventHandler<PlayerIndexEventArgs>(SelectAvatar);
            pocahon.Selected += new EventHandler<PlayerIndexEventArgs>(SelectAvatar);
            wknight.Selected += new EventHandler<PlayerIndexEventArgs>(SelectAvatar);
            avatarGestureMenu.AddMenuItem(dragon, new Rectangle(X_1, Y_elem, dragon_up.Width * ratio / 100, dragon_up.Height * ratio / 100));
            avatarGestureMenu.AddMenuItem(girl, new Rectangle(X_2, Y_elem, girl_up.Width * ratio / 100, girl_up.Height * ratio / 100));
            avatarGestureMenu.AddMenuItem(pocahon, new Rectangle(X_3, Y_elem, pocahon_up.Width * ratio / 100, pocahon_up.Height * ratio / 100));
            avatarGestureMenu.AddMenuItem(wknight, new Rectangle(X_4_2, Y_elem, (wknight_up.Width * ratio_2 / 100) * ratio / 100, wknight_up.Height * ratio / 100));

            return avatarGestureMenu;
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
            GestureMenuEntry gme1 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(1, 1, Width, Width), "avatar");
            GestureMenuEntry gme2 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 100, Width, Width), "background");
            GestureMenuEntry gme3 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 200, Width, Width), "narrate");
            GestureMenuEntry gme4 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 300, Width, Width), "undo");
            GestureMenuEntry gme5 = new GestureMenuEntry(t_up, empty, empty, empty, new Rectangle(0, 400, Width, Width), "exit");
            gme1.Selected += new EventHandler<PlayerIndexEventArgs>(ActivateAvatarScreen);
            gme2.Selected += new EventHandler<PlayerIndexEventArgs>(ActivateBackgroundScreen);
            gme3.Selected += new EventHandler<PlayerIndexEventArgs>(ActivateNarration);
            gme3.Unselected += new EventHandler<PlayerIndexEventArgs>(ActivateNarration);
            gme4.Selected += new EventHandler<PlayerIndexEventArgs>(ActivateUndo);
            gme5.Selected += new EventHandler<PlayerIndexEventArgs>(ActivateExit);
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
#region event handlers
        private void PlayNarration(object sender, PlayerIndexEventArgs p)
        {
            ((SlideScreen)screenManager.GetScreens()[screenManager.NumScreens - 1]).playAudio();
            _narrationGestureMenu.Disabled = true;
        }
        private void RecordNarration(object sender, PlayerIndexEventArgs p)
        {
            SlideScreen curSlide = (SlideScreen)screenManager.GetScreens()[screenManager.NumScreens - 1];
            curSlide.beginRecording();
            //(.beginRecording();
            _narrationGestureMenu.Disabled = true;
        }
        private void ActivateAvatarScreen(object sender, PlayerIndexEventArgs p)
        {
            _backgroundActivate = false;
            _backgroundGestureMenu.Disabled = true;
            _avatarGestureMenu.Disabled = false;
        }
        private void ActivateBackgroundScreen(object sender, PlayerIndexEventArgs p)
        {
            _backgroundActivate = true;
            _backgroundGestureMenu.Disabled = false;
            _avatarGestureMenu.Disabled = true;
        }
        private void ActivateNarration(object sender, PlayerIndexEventArgs p)
        {
            _backgroundActivate = false;
            _backgroundGestureMenu.Disabled = true;
            _narrationGestureMenu.Disabled = false;
            ((SlideScreen)screenManager.GetScreens()[screenManager.NumScreens - 1]).beginRecording();
            

        }
        private void ActivateUndo(object sender, PlayerIndexEventArgs p)
        {
            ((SlideScreen)screenManager.GetScreens()[screenManager.NumScreens - 1]).Undo();
        }
        private void ActivateExit(object sender, PlayerIndexEventArgs p)
        {
            int numRemoves = screenManager.NumScreens - 2;
            for (numRemoves = screenManager.NumScreens - 2; numRemoves > 0; numRemoves--)
            {
                ((SlideScreen)screenManager.GetScreens()[numRemoves + 1]).ExitScreen();
                ((SlideMenuScreen)screenManager.GetScreens()[1]).PreviousSlide();
            }
        }

        private void SelectAvatar(object sender, PlayerIndexEventArgs p)
        {
            int avatarIndex = -1;
            switch (((GestureMenuEntry)sender).Text)
            {
                case "dragon":
                    avatarIndex = 3;
                    break;
                case "girl":
                    avatarIndex = 2;
                    break;
                case "pocahon":
                    avatarIndex = 1;
                    break;
                case "wknight":
                    avatarIndex = 0;
                    break;
                default:
                    break;
            }
            if (avatarIndex > -1)
                screenManager.ChangeAvatar(avatarIndex, p.PlayerIndex);
            _avatarGestureMenu.Disabled = true;
        }

        private void SelectBackground(object sender, PlayerIndexEventArgs p)
        {
            int backgroundIndex = -1;
            switch (((GestureMenuEntry)sender).Text)
            {
                case "beach":
                    backgroundIndex = 0;
                    break;
                case "coral":
                    backgroundIndex = 1;
                    break;
                case "fantasy":
                    backgroundIndex = 2;
                    break;
                case "monument":
                    backgroundIndex = 3;
                    break;
                case "rainforest":
                    backgroundIndex = 4;
                    break;
                case "snowy":
                    backgroundIndex = 5;
                    break;
                case "desert":
                    backgroundIndex = 6;
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
            _avatarGestureMenu.Disabled = true;
            _narrationGestureMenu.Disabled = true;
        }

        public void EnableMainScreen()
        {
            _mainGestureMenu.Disabled = false;
        }
#endregion
        public void Draw(GameTime gametime)
        {
            _backgroundGestureMenu.Draw(gametime, 0.1F);
            _avatarGestureMenu.Draw(gametime, 0.0F);
            _narrationGestureMenu.Draw(gametime, 0.0F);
            _mainGestureMenu.Draw(gametime, 0.0F);
           
        }

    }
}
