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
    /* GestureMenuScreen is an implementation of MenuScreen that takes the Kinect Sensor and uses it as input.
* On initialization, one must specify a Rectangle that defines the hitbox of the Menu.
* After it is initialized, one must add menu items through the function
* AddMenuEntry and give it the menu entry and the Rectangle specifying the hitbox.
*/
    class GestureMenuScreen : MenuScreen
    {
        #region Private Vars
        // hitArea specifies the hitarea of the whole menu screen
        Rectangle _hitArea;
        // timer specifies how long one must hover over the item in milliseconds for a menu item to be selected
        Texture2D _img;
        Texture2D _imgoff;
        Texture2D _imgdisabled;
        KinectSensor _sensor;
        Character _skeleton;
        Character _skeleton2;
        int _threshold;
        Stopwatch _timer;
        Stopwatch _unselectedTimer;
        List<KeyValuePair<GestureMenuEntry, Rectangle>> _hitboxes;
        ScreenManager _manager;

        #endregion

        #region Public Vars
        int selection = -1;
        bool is_selected = false; // A specific menu item is selected
        bool is_disabled = false; // The whole menu screen is disabled
        bool was_disabled = false; // housekeeping boolean for is_disabled
        bool is_over = false;     // A hand is over the entire menu
        int ignoredTime;
        public int IgnoredLimit
        {
            get { return ignoredTime; }
            set { ignoredTime = value; }
        }
        public bool Disabled
        {
            get { return is_disabled; }
            set { is_disabled = value; }
        }
        KeyValuePair<Texture2D, Rectangle> _other;
        public KeyValuePair<Texture2D,Rectangle> Other
        {
            get { return _other; }
            set { _other = value; }
        }
        public event EventHandler<PlayerIndexEventArgs> Ignored;
        #endregion


        #region Initialization
        public GestureMenuScreen(Rectangle initArea, int init_time, string Title, Character skeleton, Character skeleton2, Texture2D MenuImg, Texture2D MenuImgOff, Texture2D MenuImgDisabled, ScreenManager manager)
            : base(Title)
        {
            this._hitArea = initArea;
            this.ignoredTime = 4000;
            this._threshold = init_time;
            this._sensor = manager.Kinect;
            this._skeleton = skeleton;
            this._skeleton2 = skeleton2;
            this._img = MenuImg;
            this._imgoff = MenuImgOff;
            this._imgdisabled = MenuImgDisabled;
            this._hitboxes = new List<KeyValuePair<GestureMenuEntry, Rectangle>>();
            if (this._sensor != null)
                this._sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(CheckGesture);
            this._manager = manager;
            this._timer = new Stopwatch();
            this._timer.Start();
            this._unselectedTimer = new Stopwatch();
            this._unselectedTimer.Start();
            this.ScreenManager = manager;
        }
        #endregion

        public void AddMenuItem(GestureMenuEntry entry, Rectangle hitbox)
        {
            this._hitboxes.Add(new KeyValuePair<GestureMenuEntry, Rectangle>(entry, hitbox));
        }

        public void OnIgnored(PlayerIndex pi)
        {
            if (Ignored != null)
                Ignored(this, new PlayerIndexEventArgs(PlayerIndex.One));
        }

        private void CheckGesture(object sender, SkeletonFrameReadyEventArgs s)
        {
            if (this.is_disabled)
            {
                was_disabled = true;
                return;
            }
            else if (was_disabled)
            {
                was_disabled = false;
                _unselectedTimer.Restart();
            }

            SkeletonFrame sf = s.OpenSkeletonFrame();
            if (sf == null)
                return;
            Skeleton[] sdata = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(sdata);
            sf.Dispose();
            int selector = -1;
            int temp_selected = -1;
            bool over = false;
            foreach (Skeleton data in sdata)
            {
                if (data != null && !(data.Joints[JointType.HandLeft].Position.X == 0 && data.Joints[JointType.HandRight].Position.X == 0 && data.Joints[JointType.HandLeft].Position.Y == 0 && data.Joints[JointType.HandRight].Position.Y == 0))
                {
                    Point hand_left = this._skeleton.getLeftHandPoint();
                    Point hand_right = this._skeleton.getRightHandPoint();
                    Point hand_left2 = this._skeleton2.getLeftHandPoint();
                    Point hand_right2 = this._skeleton2.getRightHandPoint();
                    foreach (KeyValuePair<GestureMenuEntry, Rectangle> entry_rect in this._hitboxes)
                    {
                        if (RectTouched(hand_left, entry_rect.Value) || RectTouched(hand_right, entry_rect.Value))
                        {
                            temp_selected = this._hitboxes.IndexOf(entry_rect);
                            selector = 1;
                        } else if  ( RectTouched(hand_left2, entry_rect.Value) || RectTouched(hand_right2, entry_rect.Value))
                        {
                            temp_selected = this._hitboxes.IndexOf(entry_rect);
                            selector = 2;
                        }
                        if (RectTouched(hand_left, this._hitArea) || RectTouched(hand_right, this._hitArea) || RectTouched(hand_left2, this._hitArea) || RectTouched(hand_right2, this._hitArea))
                        {
                            over = true;
                        }
                    }
                }
            }
            this.is_over = over;
            if (temp_selected > -1 && this.selection == -1)
                _unselectedTimer.Stop();
            else if (temp_selected == -1 && this.selection > -1)
                _unselectedTimer.Restart();
            if (temp_selected > -1 && temp_selected != this.selection)
                this._hitboxes[temp_selected].Key.OnOnOverEntry(PlayerIndex.One);
            if (this.is_selected && temp_selected != this.selection)
                this._hitboxes[this.selection].Key.OnUnselectEntry(PlayerIndex.One); // default to one
            if (temp_selected > -1 && temp_selected == this.selection)
            {

                if (this._timer.ElapsedMilliseconds > this._threshold && !this.is_selected)
                {
                    PlayerIndex pi = (selector == 1) ? PlayerIndex.One : PlayerIndex.Two;
                    this._hitboxes[this.selection].Key.OnSelectEntry(pi); // default to player one
                    this._timer.Stop();
                    this.is_selected = true;
                }
            }
            else if (temp_selected != this.selection)
            {
                this._timer.Restart();
                this.selection = temp_selected;
                this.is_selected = false;
            }
            if (temp_selected == -1 && this.selection == -1 && _unselectedTimer.ElapsedMilliseconds > ignoredTime)
            {
                OnIgnored(PlayerIndex.One);
                this._unselectedTimer.Reset();
            }
        }

        private bool RectTouched(Point p, Rectangle r)
        {
            return (p.X >= 0 && p.Y >= 0 && r.X <= p.X && p.X <= r.X + r.Width && r.Y <= p.Y && p.Y <= r.Y + r.Height);
        }

        #region Draw and Update

        public void Draw(GameTime gameTime, float sortmode)
        {
            //base.Draw(gameTime);
            SpriteBatch spriteBatch = _manager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            if (this.is_disabled)
                spriteBatch.Draw(this._imgdisabled, this._hitArea, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.49F + sortmode);
            else if (!this.is_over)
                spriteBatch.Draw(this._imgoff, this._hitArea, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.49F + sortmode);
            else if (this.is_over)
            {
                spriteBatch.Draw(this._img, this._hitArea, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.49F + sortmode);
                if (!this._other.Equals(default(KeyValuePair<Texture2D,Rectangle>)))
                    spriteBatch.Draw(this._other.Key, this._other.Value, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.50F + sortmode);
            }
            foreach (KeyValuePair<GestureMenuEntry, Rectangle> kvp in _hitboxes)
            {
                GestureMenuEntry.MenuEntryState mes = GestureMenuEntry.MenuEntryState.UP;
                if (_hitboxes.IndexOf(kvp) == this.selection)
                {
                    if (this._timer.ElapsedMilliseconds > this._threshold)
                        mes = GestureMenuEntry.MenuEntryState.DOWN;
                    else
                        mes = GestureMenuEntry.MenuEntryState.OVER;
                }
                if (this.is_disabled)
                    mes = GestureMenuEntry.MenuEntryState.DISABLED;
                kvp.Key.Draw(this, mes, gameTime, sortmode);
            }
            spriteBatch.End();
        }
        #endregion
    }
}
