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
        KinectSensor _sensor;
        SkeletonTracker _skeleton;
        int _threshold;
        Stopwatch _timer;
        List<KeyValuePair<GestureMenuEntry, Rectangle>> _hitboxes;
        
        ScreenManager _manager;

        #endregion

        #region Public Vars
        int selection = -1;
        bool is_selected = false;
        bool is_disabled = false;

        #endregion


        #region Initialization
        public GestureMenuScreen(Rectangle initArea, int init_time, string Title, SkeletonTracker skeleton, Texture2D MenuImg, ScreenManager manager) : base (Title)
        {
            this._hitArea = initArea;
            
            this._threshold = init_time;
            this._sensor = skeleton.Kinect;
            this._skeleton = skeleton;
            this._img = MenuImg;
            this._hitboxes = new List<KeyValuePair<GestureMenuEntry,Rectangle>>();
            if (this._sensor != null)
                this._sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(CheckGesture);
            this._manager = manager;
            this._timer = new Stopwatch();
            this._timer.Start();
            this.ScreenManager = manager;
        }
        #endregion

        public void AddMenuItem(GestureMenuEntry entry, Rectangle hitbox)
        {
            this._hitboxes.Add(new KeyValuePair<GestureMenuEntry, Rectangle>(entry, hitbox));
        }

        private void CheckGesture(object sender, SkeletonFrameReadyEventArgs s)
        {
            if (this.is_disabled)
                return;
            SkeletonFrame sf = s.OpenSkeletonFrame();
            if (sf == null)
                return;
            Skeleton[] sdata = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(sdata);
            sf.Dispose();
            int temp_selected = -1;
            foreach (Skeleton data in sdata)
            {
                if (data != null)
                {
                    Point hand_left = this._skeleton.GetDisplayPosition(data.Joints[JointType.HandLeft]);
                    Point hand_right = this._skeleton.GetDisplayPosition(data.Joints[JointType.HandRight]);
                    foreach (KeyValuePair<GestureMenuEntry, Rectangle> entry_rect in this._hitboxes)
                    {
                        if (RectTouched(hand_left, entry_rect.Value) || RectTouched(hand_right, entry_rect.Value))
                        {
                            temp_selected = this._hitboxes.IndexOf(entry_rect);
                        }
                    }
                }
            }
            if (temp_selected > -1 && temp_selected == this.selection)
            {

                if (this._timer.ElapsedMilliseconds > this._threshold && !this.is_selected)
                {
                    this._hitboxes[this.selection].Key.OnSelectEntry(PlayerIndex.One); // default to player one
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

        }

        private bool RectTouched(Point p, Rectangle r)
        {
            return (p.X >= 0 && p.Y >= 0 && r.X <= p.X && p.X <= r.X + r.Width && r.Y <= p.Y && p.Y <= r.Y + r.Height);
        }

        #region Draw and Update

        public override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);
            SpriteBatch spriteBatch = _manager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(this._img, this._hitArea, Color.White);
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
                kvp.Key.Draw(this, mes, gameTime);
            }
            spriteBatch.End();
        }
        #endregion

    }
}

