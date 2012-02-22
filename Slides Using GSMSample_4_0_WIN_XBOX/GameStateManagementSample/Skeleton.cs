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
        private Rectangle head = new Rectangle(0,0, 50,50);
        private ScreenManager screenManager;

            // JASON's CODE
        public SkeletonTracker(ScreenManager sManager)
        {
            
            screenManager = sManager;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectSensor = KinectSensor.KinectSensors[0];

                kinectSensor.SkeletonStream.Enable();
                kinectSensor.Start();
                kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(KinectAllFramesReady);

                this.skeletonData = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];
                Console.WriteLine("created new Skeleton data.\n");
                //  END JASON's CODE */
            }
        }
        public void KinectAllFramesReady(object sender,AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    //  take skeleton data and update avatar state
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    float headX = skeletonData[0].Joints[JointType.Head].Position.X;
                    float headY = skeletonData[0].Joints[JointType.Head].Position.Y;
                    head.X =(int) headX;
                    head.Y = (int)headY;
//
Console.WriteLine( headX + ", " + headY );
                    
                 
                }
                else
                {
                    // skeletonFrame is null because the request did not arrive in time
                }
            }
        }

       
        
        //  END JASON's CODE

       

    
     public  void  Draw(GameTime gameTime) {
           
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Texture2D blankTexture = new Texture2D(graphics, 50, 50);
            spriteBatch.Begin();
            Viewport viewport = graphics.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,head, Color.Black);

            spriteBatch.End();
        }
    }
}
