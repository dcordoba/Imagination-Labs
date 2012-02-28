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
        private Dictionary<JointType,Sprite2D> skeletonShapes;


        Sprite2D circleLarge;
        Sprite2D circleMed;
        Sprite2D circleSmall;
      

        

        // private Rectangle head = new Rectangle(0,0, 50,50);
        
            // JASON's CODE
        public SkeletonTracker(ScreenManager sManager)
       //public SkeletonTracker()
        {
            Console.WriteLine ("INIT SKeleton**************************************");
            screenManager = sManager;
            skeletonShapes = new Dictionary<JointType, Sprite2D>();
            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectSensor = KinectSensor.KinectSensors[0];
                TransformSmoothParameters p = new TransformSmoothParameters
                {
                    Smoothing = 0.75f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };
                kinectSensor.SkeletonStream.Enable(p);
               // kinectSensor.SkeletonStream.
               
                kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(KinectAllFramesReady);
                kinectSensor.Start();

                this.skeletonData = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];


                /*Orig sprites
                //initialize the "head" sprite
                Color transColor = new Color(255, 255, 255);
                head = new Sprite2D(screenManager.Avatar1, new Rectangle(0, 0, 100, 150), transColor);//Color.Black);
                Sprite2D rightHand = new Sprite2D(screenManager.BlankTexture,new Rectangle(0, 0, 50, 50), Color.LightGreen);
                 */
                ///*Circle sprites
                InitCircles();
                Sprite2D rightHand = circleMed;
                Sprite2D rightElbow = circleSmall;
                Sprite2D head = circleLarge;
                skeletonShapes.Add(JointType.Head, head);
                skeletonShapes.Add(JointType.HandRight, rightHand);
                skeletonShapes.Add(JointType.HandLeft, rightHand);
                skeletonShapes.Add(JointType.ElbowRight, rightElbow);
                skeletonShapes.Add(JointType.ElbowLeft, rightElbow);
                skeletonShapes.Add(JointType.KneeLeft, rightElbow);
                skeletonShapes.Add(JointType.KneeRight, rightElbow);
                skeletonShapes.Add(JointType.AnkleLeft, rightElbow);
                skeletonShapes.Add(JointType.AnkleRight, rightElbow);
                skeletonShapes.Add(JointType.Spine, rightHand);
                //TODO: create new sprites for each joint type

                Console.WriteLine("****************************************created new Skeleton data.\n");
                //*/
                /*Circle sprites
                InitCircles();
                Sprite2D rightHand = Circle();
                Sprite2D rightElbow = Circle();
                Sprite2D head = Circle();
                skeletonShapes.Add("Head", head);
                skeletonShapes.Add("HandRight", rightHand);
                skeletonShapes.Add("ElbowRight", rightElbow);
                Console.WriteLine("****************************************created new Skeleton data.\n");
                //  END JASON's CODE */
            }
        }
        private Sprite2D Circle()
        {
            Texture2D cLarge = screenManager.Game.Content.Load<Texture2D>("circleLarge");
            Sprite2D circleLarge = new Sprite2D(cLarge, new Rectangle(0, 0, 100, 150), Color.White);
            return circleLarge;
        }
        private void InitCircles()
        {   Texture2D cLarge = screenManager.Game.Content.Load<Texture2D>("circleLarge");
            Texture2D cMed = screenManager.Game.Content.Load<Texture2D>("circleMedium");
            Texture2D cSmall = screenManager.Game.Content.Load<Texture2D>("circleSmall");

            circleLarge = new Sprite2D(cLarge, new Rectangle(0, 0, 75, 75), Color.White);
            circleMed = new Sprite2D(cMed, new Rectangle(0, 0, 40, 40), Color.White);
            circleSmall = new Sprite2D(cSmall, new Rectangle(0, 0, 20, 20), Color.White);
        }
        public void KinectAllFramesReady(object sender,AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null && screenManager != null)
                {
                    //  take skeleton data and update avatar state
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    midViewPort.X = screenManager.GraphicsDevice.Viewport.Width / 2;
                    midViewPort.Y = screenManager.GraphicsDevice.Viewport.Height / 2;
                    UpdateJointPos(JointType.Head);
                    UpdateJointPos(JointType.Spine);
                    UpdateJointPos(JointType.HandRight);
                    UpdateJointPos(JointType.HandLeft);

                    UpdateJointPos(JointType.ElbowRight);                   
                    UpdateJointPos(JointType.ElbowLeft);

                    UpdateJointPos(JointType.KneeRight);
                    UpdateJointPos(JointType.KneeLeft);

                    UpdateJointPos(JointType.AnkleRight);
                    UpdateJointPos(JointType.AnkleLeft);
                   


                    /*Original joint updating code by Isabelle
                    float headX = skeletonData[0].Joints[JointType.Head].Position.X; //floats between -1 and 1
                    float headY = skeletonData[0].Joints[JointType.Head].Position.Y;
                    //map to hand for testing
                    float rHandX = skeletonData[0].Joints[JointType.HandRight].Position.X; //floats between -1 and 1
                    float rHandY = skeletonData[0].Joints[JointType.HandRight].Position.Y;
                    //map to elbow for testing
                    float rElbowX = skeletonData[0].Joints[JointType.ElbowRight].Position.X; //floats between -1 and 1
                    float rElbowY = skeletonData[0].Joints[JointType.ElbowRight].Position.Y;
                  

                    midViewPort.X = screenManager.GraphicsDevice.Viewport.Width / 2;
                    midViewPort.Y = screenManager.GraphicsDevice.Viewport.Height / 2;
                    //set the posistion of the head's rectangle to be in the center of the screen and move by the joint amount
                    //TODO: figure out if skeleton data stream has lower left origin, because XNA has upper left origin and we adjust for that
                    int screenXScale = 500;
                    int screenYScale = 500;
                    skeletonShapes[JointType.Head].SetRectPos((int)((headX * screenXScale) + midViewPort.X), (int)(midViewPort.Y - (headY * screenYScale)));
                    skeletonShapes[JointType.HandRight].SetRectPos((int)((rHandX * screenXScale) + midViewPort.X), (int)(midViewPort.Y - (rHandY * screenYScale)));
                    skeletonShapes[JointType.ElbowRight].SetRectPos((int)((rElbowX * screenXScale) + midViewPort.X), (int)(midViewPort.Y - (rElbowY * screenYScale)));
                    //head.SetRectPos((int)((headX * screenXScale) + midViewPort.X), (int)(midViewPort.Y-(headY *screenYScale)));
                     * 
                    
//
head = skeletonShapes[JointType.Head];
Console.WriteLine( "head: " + head.Rectangle.X + ", " + head.Rectangle.Y );
Console.WriteLine("joint: " + headX + ", " + headY);
                     * */

                }
                else
                {
                    // skeletonFrame is null because the request did not arrive in time
                }
            }
        }

        private void UpdateJointPos(JointType jointName)
        {
            
            float jointX = skeletonData[0].Joints[jointName].Position.X; //floats between -1 and 1
            float jointY = skeletonData[0].Joints[jointName].Position.Y;
            int screenXScale = 200;
            int screenYScale = 200;
            skeletonShapes[jointName].SetRectPos((int)((jointX * screenXScale) + midViewPort.X), (int)(midViewPort.Y - (jointY * screenYScale)));

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
            foreach (KeyValuePair<JointType, Sprite2D> shape in skeletonShapes){
                 screenManager.SpriteBatch.Draw(shape.Value.Texture,shape.Value.Rectangle, shape.Value.Color);
            //  screenManager.SpriteBatch.Draw(head.Texture,head.Rectangle, head.Color);
            }
            screenManager.SpriteBatch.End();
        }
    }
     
}
