#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameStateManagement
{
    class SkeletonTracker 
    {
        //  JASON's CODE

        KinectSensor kinectSensor;

        private Skeleton[] skeletonData;
        public SkeletonFrame skeletonFrame;
        private ScreenManager screenManager;
        private Vector2 midViewPort;
        private Sprite2D head;
        // private Rectangle head = new Rectangle(0,0, 50,50);
        
            // JASON's CODE
        public SkeletonTracker(ScreenManager sManager)
       //public SkeletonTracker()
        {
            
            screenManager = sManager;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectSensor = KinectSensor.KinectSensors[0];

                kinectSensor.SkeletonStream.Enable();
                kinectSensor.Start();
                kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(KinectAllFramesReady);

                this.skeletonData = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];

                //initialize the "head" sprite
                Color transColor = new Color(255, 255, 255);
                head = new Sprite2D(screenManager.Avatar1, new Rectangle(0, 0, 100, 150), transColor);//Color.Black);

                Console.WriteLine("created new Skeleton data.\n");
                //  END JASON's CODE */
            }
        }
        public void KinectAllFramesReady(object sender,AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null && screenManager != null)
                {
                    //  take skeleton data and update avatar state
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    float headX = skeletonData[0].Joints[JointType.Head].Position.X; //floats between -1 and 1
                    float headY = skeletonData[0].Joints[JointType.Head].Position.Y;

                    midViewPort.X = screenManager.GraphicsDevice.Viewport.Width / 2;
                    midViewPort.Y = screenManager.GraphicsDevice.Viewport.Height / 2;
                    //set the posistion of the head's rectangle to be in the center of the screen and move by the joint amount
                    head.SetRectPos((int)((headX * 100) + midViewPort.X), (int)((headY * 100) + midViewPort.Y));
//
Console.WriteLine( "head: " + head.Rectangle.X + ", " + head.Rectangle.Y );
Console.WriteLine("joint: " + headX + ", " + headY);

                }
                else
                {
                    // skeletonFrame is null because the request did not arrive in time
                }
            }
        }

        public void SetScreenManager(ScreenManager sm)
        {
            screenManager = sm;
        }
        public Sprite2D Head
        {
            get { return head; }
        }
        
        //  END JASON's CODE

       
        public  void  Draw(GameTime gameTime) {           
            screenManager.SpriteBatch.Begin();
            screenManager.SpriteBatch.Draw(head.Texture,head.Rectangle, head.Color);            
            screenManager.SpriteBatch.End();
        }
    }
     
}
