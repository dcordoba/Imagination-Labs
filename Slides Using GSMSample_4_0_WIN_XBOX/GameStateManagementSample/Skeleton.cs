#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement.DTWGestureRecognition;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using System.IO;

#endregion

namespace GameStateManagement
{
    public class SkeletonTracker 
    {
        
        KinectSensor kinectSensor;
        //SpeechRecognitionEngine speechRecognizer;
        SpeechRecognizer speechRecognizer; 

        private Skeleton[] skeletonData;
        private ScreenManager screenManager;
        private Vector2 midViewPort;
        private Sprite2D head;
        private Dictionary<JointType,Sprite2D> skeletonShapes;
        private DTWImplementation dtw;

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
                
                kinectSensor.Start();
                kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(KinectSkeletonFramesReady);
                dtw = new DTWImplementation(kinectSensor, this);
                this.skeletonData = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];

                this.speechRecognizer = SpeechRecognizer.Create();//SpeechRecognizer.Create(kinectSensor);
               // InitSpeechRecognition();
               

                /*Orig sprites
                //initialize the "head" sprite
                Color transColor = new Color(255, 255, 255);
                head = new Sprite2D(screenManager.Avatar1, new Rectangle(0, 0, 100, 150), transColor);//Color.Black);
                Sprite2D rightHand = new Sprite2D(screenManager.BlankTexture,new Rectangle(0, 0, 50, 50), Color.LightGreen);
                 */
                ///*Circle sprites
                //InitCircles();
                InitSprites();
                /*Sprite2D rightHand = circleSmall;
                Sprite2D rightElbow = circleMed;
                Sprite2D head = circleSmall;
                Sprite2D leftElbow = circleMed;
                Sprite2D leftHand = circleSmall;
                Sprite2D leftKnee = circleMed;
                Sprite2D rightKnee = circleMed;
                Sprite2D leftAnkle = circleSmall;
                Sprite2D rightAnkle = circleSmall;
                Sprite2D Spine = new Sprite2D();

                skeletonShapes.Add(JointType.Head, head);
                skeletonShapes.Add(JointType.HandRight, rightHand);
                skeletonShapes.Add(JointType.HandLeft, rightHand);
                skeletonShapes.Add(JointType.ElbowRight, rightElbow);
                skeletonShapes.Add(JointType.ElbowLeft, leftElbow);
                skeletonShapes.Add(JointType.KneeLeft, leftKnee);
                skeletonShapes.Add(JointType.KneeRight, rightKnee);
                skeletonShapes.Add(JointType.AnkleLeft, leftAnkle);
                skeletonShapes.Add(JointType.AnkleRight, rightAnkle);
                skeletonShapes.Add(JointType.Spine, Spine);
                //TODO: create new sprites for each joint type
                */
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
        /*
        private void InitSpeechRecognition()
        {
            // Obtain the KinectAudioSource to do audio capture
            KinectAudioSource source = sensor.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None; // No AEC for this sample
            source.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition

            RecognizerInfo ri = GetKinectRecognizer();

            if (ri == null)
            {
                Console.WriteLine("Could not find Kinect speech recognizer. Please refer to the sample requirements.");
                return;
            }

            Console.WriteLine("Using: {0}", ri.Name);

            // NOTE: Need to wait 4 seconds for device to be ready right after initialization
            int wait = 4;
            while (wait > 0)
            {
                Console.Write("Device will be ready for speech recognition in {0} second(s).\r", wait--);
                Thread.Sleep(1000);
            }

            using (var sre = new SpeechRecognitionEngine(ri.Id))
            {
                var colors = new Choices();
                colors.Add("red");
                colors.Add("green");
                colors.Add("blue");

                var gb = new GrammarBuilder { Culture = ri.Culture };

                // Specify the culture to match the recognizer in case we are running in a different culture.                                 
                gb.Append(colors);

                // Create the actual Grammar instance, and then load it into the speech recognizer.
                var g = new Grammar(gb);

                sre.LoadGrammar(g);
                sre.SpeechRecognized += SreSpeechRecognized;
                sre.SpeechHypothesized += SreSpeechHypothesized;
                sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

                using (Stream s = source.Start())
                {
                    sre.SetInputToAudioStream(
                        s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

                    Console.WriteLine("Recognizing speech. Say: 'red', 'green' or 'blue'. Press ENTER to stop");

                    sre.RecognizeAsync(RecognizeMode.Multiple);
                    Console.ReadLine();
                    Console.WriteLine("Stopping recognizer ...");
                    sre.RecognizeAsyncStop();
                }
            }
        }
        */
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
        private void InitSprites()
        {   
            Texture2D cLarge = screenManager.Game.Content.Load<Texture2D>("circleLarge");
            Texture2D cMed = screenManager.Game.Content.Load<Texture2D>("circleMedium");
            Texture2D cSmall = screenManager.Game.Content.Load<Texture2D>("circleSmall");

            skeletonShapes.Add(JointType.Head, new Sprite2D(cLarge, new Rectangle(0, 0, 75, 75), Color.White));
            skeletonShapes.Add(JointType.HandRight, new Sprite2D(cSmall, new Rectangle(0, 0, 20, 20), Color.White));
            skeletonShapes.Add(JointType.HandLeft, new Sprite2D(cSmall, new Rectangle(0, 0, 20, 20), Color.White));
            skeletonShapes.Add(JointType.ElbowRight, new Sprite2D(cMed, new Rectangle(0, 0, 40, 40), Color.White));
            skeletonShapes.Add(JointType.ElbowLeft, new Sprite2D(cMed, new Rectangle(0, 0, 40, 40), Color.White));
            skeletonShapes.Add(JointType.KneeLeft, new Sprite2D(cMed, new Rectangle(0, 0, 40, 40), Color.White));
            skeletonShapes.Add(JointType.KneeRight, new Sprite2D(cMed, new Rectangle(0, 0, 40, 40), Color.White));
            skeletonShapes.Add(JointType.AnkleLeft, new Sprite2D(cSmall, new Rectangle(0, 0, 20, 20), Color.White));
            skeletonShapes.Add(JointType.AnkleRight, new Sprite2D(cSmall, new Rectangle(0, 0, 20, 20), Color.White));
            skeletonShapes.Add(JointType.Spine, new Sprite2D(cLarge, new Rectangle(0, 0, 75, 75), Color.White));

            //temporary var for checking capture
            head = skeletonShapes[JointType.Head];
        }
        public void KinectSkeletonFramesReady(object sender,SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();
            if (sf != null && screenManager != null)
            {
                Skeleton[] skeletons = new Skeleton[sf.SkeletonArrayLength];
                sf.CopySkeletonDataTo(skeletons);

                foreach (Skeleton skeleton in skeletons)
                {
                    //  take skeleton data and update avatar state

                    midViewPort.X = screenManager.GraphicsDevice.Viewport.Width / 2;
                    midViewPort.Y = screenManager.GraphicsDevice.Viewport.Height / 2;
                    UpdateJointPos(JointType.Head, skeleton);
                    UpdateJointPos(JointType.Spine, skeleton);
                    UpdateJointPos(JointType.HandRight, skeleton);
                    UpdateJointPos(JointType.HandLeft, skeleton);

                    UpdateJointPos(JointType.ElbowRight, skeleton);
                    UpdateJointPos(JointType.ElbowLeft, skeleton);

                    UpdateJointPos(JointType.KneeRight, skeleton);
                    UpdateJointPos(JointType.KneeLeft, skeleton);

                    UpdateJointPos(JointType.AnkleRight, skeleton);
                    UpdateJointPos(JointType.AnkleLeft, skeleton);


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
                sf.Dispose();
            }
            else
            {
                // skeletonFrame is null because the request did not arrive in time
            }

        }

        private void UpdateJointPos(JointType jointName, Skeleton s)
        {
            if (s == null)
                return;
            ColorImagePoint cp = kinectSensor.MapSkeletonPointToColor(s.Joints[jointName].Position, ColorImageFormat.RgbResolution640x480Fps30);
            float jointX = s.Joints[jointName].Position.X; //floats between -1 and 1
            float jointY = s.Joints[jointName].Position.Y;
            //int screenXScale = 200;
            //int screenYScale = 200;
            Point screen_position = GetDisplayPosition(s.Joints[jointName]);
            if (jointX != 0 || jointY != 0)
            {
                //skeletonShapes[jointName].SetRectPos((int)((jointX * screenXScale) + midViewPort.X), (int)(midViewPort.Y - (jointY * screenYScale)));
                skeletonShapes[jointName].SetRectPos(screen_position.X, screen_position.Y);
            }
        }

        public Point GetDisplayPosition(Joint j)
        {
            if (j.Position.X == 0 && j.Position.Y == 0)
                return new Point(-1, -1) ; // null
            ColorImagePoint cp = kinectSensor.MapSkeletonPointToColor(j.Position, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(cp.X * screenManager.GraphicsDevice.Viewport.Width / 640, cp.Y * screenManager.GraphicsDevice.Viewport.Height / 480);
        }

        public void SetScreenManager(ScreenManager sm)
        {
            screenManager = sm;
        }
        public Sprite2D Head
        {
            get { return head; }
        }

        public Dictionary<JointType,Sprite2D> SkeletonShapes
        {
            get { return skeletonShapes; }
        }

        public KinectSensor Kinect
        {
            get { return kinectSensor; }

        }
        public void cycleAvatar()
        {
            Console.Out.WriteLine("Cycling Avatar");
        }
       

       
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
