using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework.Audio;


namespace GameStateManagement
{
    
    class SlideScreen : GameScreen
    {
        #region variables
        SlideMenuScreen parentSlideMenu;
        int slideno;
        //Sprite2D tovSprite;
        //Sprite2D questionSprite;
        public int backgroundIndex = 0; //temporary tracker for toggling backgrounds
        private Texture2D backgroundScene;
        private List<SlideObject> slideObjects;
        //private List<Skeleton> capturedSkeletons;
        private List<SkeletonJoints> capturedSkeletons;
        private List<int> capturedAvatarIndices;



       //   Sprite2D tovSprite;
       // Sprite2D questionSprite;
        SpriteBatch spriteBatch;
        Texture2D background_dirty;
        Texture2D background_active;
        Color backColor = Color.CornflowerBlue;
        ContentManager content;
        bool captured;
        Viewport viewport;
        //Recording private variables
        BackgroundWorker bw;
        MemoryStream audioStream = null;
        SoundEffectInstance soundInstance;
        private const int RiffHeaderSize = 20;
        private const string RiffHeaderTag = "RIFF";
        private const int WaveformatExSize = 18; // native sizeof(WAVEFORMATEX)
        private const int DataHeaderSize = 8;
        private const string DataHeaderTag = "data";
        private const int FullHeaderSize = RiffHeaderSize + WaveformatExSize + DataHeaderSize;
        
        #endregion

        #region Initialization
        public SlideScreen(SlideMenuScreen slideMenu)
        {
            this.slideObjects = new List<SlideObject>();// new List<Sprite2D>(); 
          
            this.parentSlideMenu = slideMenu;
            this.captured = false;
            this.slideno = this.parentSlideMenu.MaxSlideIndex;
            this.capturedSkeletons = new List<SkeletonJoints>();//List<Skeleton>();
            this.capturedAvatarIndices = new List<int>();
           

            //Recording initialization
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_recordAudio);
        }

        private KinectSensor kinectSensor()
        {
            if (parentSlideMenu.ScreenManager == null) return null;
            return parentSlideMenu.ScreenManager.Kinect;
        }

        #endregion
        
        //Color backColor = Color.CornflowerBlue;
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            background_dirty = content.Load<Texture2D>(@"bg1_dirty");
            background_active = content.Load<Texture2D>(@"bg1_active");
            spriteBatch = ScreenManager.SpriteBatch;
            viewport = ScreenManager.GraphicsDevice.Viewport;
            if(backgroundScene == null)
                backgroundScene = ScreenManager.GetBackgroundScene(0); //initialize default background 

            //Voice Recognition initialization
            //ScreenManager.SpeechRecognizer.SaidSomething += new EventHandler<SpeechRecognizer.SaidSomethingEventArgs>(SlideRecognizerSaidSomething);
        }

        #region Recording
        public bool isRecording()
        {
            return bw.IsBusy;
        }

        public bool isPlaying()
        {
            if (soundInstance == null) return false;
            return soundInstance.State == SoundState.Playing;
        }

        void beginRecording()
        {
            if (!isRecording() && !isPlaying())
            {
                bw.RunWorkerAsync();
            }
            else bw.CancelAsync();
        }

        private void bw_recordAudio(object sender, DoWorkEventArgs e)
        {
            var buffer = new byte[4096];
            int recordingLength = 0;
            BackgroundWorker worker = sender as BackgroundWorker;

            // FXCop note: This try/finally block may look strange, but it is
            // the recommended way to correctly dispose a stream that is used
            // by a writer to avoid the stream from being double disposed.
            // For more information see FXCop rule: CA2202
            //Begin something
            /*
                try
                {
                    fileStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096,true);

                    WriteWavHeader(fileStream);
                    audioStream = source.Start();
                    recording = true;


                }
                catch
                {
                    recording = false;
                    audioStream = null;
                }*/
            //Begin other
            using (var fileStream = new MemoryStream())
            {
                // FXCop note: This try/finally block may look strange, but it is
                // the recommended way to correctly dispose a stream that is used
                // by a writer to avoid the stream from being double disposed.
                // For more information see FXCop rule: CA2202
                FileStream logStream = null;
                try
                {
                    logStream = new FileStream("samples.log", FileMode.Create);
                    using (var sampleStream = new StreamWriter(logStream))
                    {
                        logStream = null;
                        //WriteWavHeader(fileStream);



                        // Start capturing audio                               
                        using (var audioStream = kinectSensor().AudioSource.Start())
                        {
                            // Simply copy the data from the stream down to the file
                            int count;
                            while (!worker.CancellationPending && ((count = audioStream.Read(buffer, 0, buffer.Length)) > 0))
                            {

                                fileStream.Write(buffer, 0, count);
                                recordingLength += count;
                            }
                            e.Cancel = true;
                        }
                        this.audioStream = new MemoryStream(fileStream.ToArray());
                        UpdateDataLength(fileStream, recordingLength);
                    }
                }
                finally
                {
                    if (logStream != null)
                    {
                        logStream.Dispose();
                    }
                }
            }




        }

        /// <summary>
        /// A bare bones WAV file header writer
        /// </summary>        
        private static void WriteWavHeader(Stream stream)
        {
            // Data length to be fixed up later
            int dataLength = 0;

            // We need to use a memory stream because the BinaryWriter will close the underlying stream when it is closed
            MemoryStream memStream = null;
            BinaryWriter bw = null;

            // FXCop note: This try/finally block may look strange, but it is
            // the recommended way to correctly dispose a stream that is used
            // by a writer to avoid the stream from being double disposed.
            // For more information see FXCop rule: CA2202
            try
            {
                memStream = new MemoryStream(64);

                WAVEFORMATEX format = new WAVEFORMATEX
                {
                    FormatTag = 1,
                    Channels = 1,
                    SamplesPerSec = 16000,
                    AvgBytesPerSec = 32000,
                    BlockAlign = 2,
                    BitsPerSample = 16,
                    Size = 0
                };

                bw = new BinaryWriter(memStream);

                // RIFF header
                WriteHeaderString(memStream, RiffHeaderTag);
                bw.Write(dataLength + FullHeaderSize - 8); // File size - 8
                WriteHeaderString(memStream, "WAVE");
                WriteHeaderString(memStream, "fmt ");
                bw.Write(WaveformatExSize);

                // WAVEFORMATEX
                bw.Write(format.FormatTag);
                bw.Write(format.Channels);
                bw.Write(format.SamplesPerSec);
                bw.Write(format.AvgBytesPerSec);
                bw.Write(format.BlockAlign);
                bw.Write(format.BitsPerSample);
                bw.Write(format.Size);

                // data header
                WriteHeaderString(memStream, DataHeaderTag);
                bw.Write(dataLength);
                memStream.WriteTo(stream);
            }
            finally
            {
                if (bw != null)
                {
                    memStream = null;
                    bw.Dispose();
                }

                if (memStream != null)
                {
                    memStream.Dispose();
                }
            }
        }

        private static void UpdateDataLength(Stream stream, int dataLength)
        {
            using (var bw = new BinaryWriter(stream))
            {
                // Write file size - 8 to riff header
                bw.Seek(RiffHeaderTag.Length, SeekOrigin.Begin);
                bw.Write(dataLength + FullHeaderSize - 8);

                // Write data size to data header
                bw.Seek(FullHeaderSize - 4, SeekOrigin.Begin);
                bw.Write(dataLength);
            }
        }

        private static void WriteHeaderString(Stream stream, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            // Debug.Assert(bytes.Length == s.Length, "The bytes and the string should be the same length.");
            stream.Write(bytes, 0, bytes.Length);
        }

        private struct WAVEFORMATEX
        {
            public ushort FormatTag;
            public ushort Channels;
            public uint SamplesPerSec;
            public uint AvgBytesPerSec;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public ushort Size;
        }

        #endregion

        #region Audio Playback
        void playAudio()
        {
            if (isRecording() || audioStream == null) return;
            if (isPlaying())
            {
                soundInstance.Stop();
                soundInstance.Dispose();
                soundInstance = null;
                return;
            }
            /*
            MemoryStream playbackStream = new MemoryStream(audioStream.ToArray());
            BinaryReader reader = new BinaryReader(playbackStream);

            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            int fmtCode = reader.ReadInt16();
            int channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            int fmtBlockAlign = reader.ReadInt16();
            int bitDepth = reader.ReadInt16();

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();

            p_byteArray = reader.ReadBytes(dataSize);
            playbackStream.Close();
            dynamicSound = new DynamicSoundEffectInstance(sampleRate, (AudioChannels)channels);
            dynamicSound.IsLooped = false;
            p_count = dynamicSound.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(100));
            p_position = 0;
            dynamicSound.BufferNeeded += new EventHandler<EventArgs>(DynamicSound_BufferNeeded);
            dynamicSound.Play();
             * */
            SoundEffect sf = new SoundEffect(audioStream.ToArray(), 16000, (AudioChannels)1);
            soundInstance = sf.CreateInstance();
            soundInstance.Play();

        }
        #endregion
        #region Handle Input
        /// <summary>
        /// Responds to user input, capturing...
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (parentSlideMenu.IsPlaying) // If Playing, don't accept input
                return;
            PlayerIndex requesteeIndex;
            //SkeletonTracker skel = ScreenManager.Skeleton;
            //int leftHandX = skel.GetDisplayPosition(skel.Joints[JointType.HandLeft]);
          //  if(ScreenManager.Skeleton.
            #region keyboard commands
            //press c to "capture"
            if(input.IsNewKeyPress(Keys.C, null,out requesteeIndex)){
                Captured();
            }
            //press the left arrow key to go back one slide
            if(input.IsNewKeyPress(Keys.Left, null, out requesteeIndex)){
               // parentSlideMenu.PreviousSlide(requesteeIndex);
                parentSlideMenu.PreviousSlide();
                this.ExitScreen();
            }
           // press "b" to change the background
            if(input.IsNewKeyPress(Keys.B,null, out requesteeIndex)){

                backgroundIndex = (backgroundIndex + 1) % 4;
             
                //TODO: use gesture recognition to generate background indices. 
                ChangeBackground(backgroundIndex);
            }
            //press u to undo last capture for this slide
            if (input.IsNewKeyPress(Keys.U, null, out requesteeIndex))
            {
                if (slideObjects.Count > 0)
                {
                    slideObjects.RemoveAt(slideObjects.Count - 1);
                }
            }
            //press right arrow key to move forward one slide
            if (input.IsNewKeyPress(Keys.Right, null, out requesteeIndex))
            {
                parentSlideMenu.NextSlide();
            }
            //press "n" to create a new slide
            if (input.IsNewKeyPress(Keys.N, null, out requesteeIndex))
            {
                parentSlideMenu.NewSlide();
            }
            //press "z" to go to change to avatar1
            if (input.IsNewKeyPress(Keys.Z, null, out requesteeIndex))
            {
                parentSlideMenu.ChangeToAvatar(0);
                Console.Out.WriteLine("Changed to 0");
            }
            //press "x" to go to change to avatar2
            if (input.IsNewKeyPress(Keys.X, null, out requesteeIndex))
            {
                parentSlideMenu.ChangeToAvatar(1);
                Console.Out.WriteLine("Changed to 1");
            }
            //press "a" to go to change to avatar2
            if (input.IsNewKeyPress(Keys.A, null, out requesteeIndex))
            {
                parentSlideMenu.ChangeToAvatar(2);
                Console.Out.WriteLine("changed to 2");
            }
            //press "m" to go to slideMenu
            if (input.IsNewKeyPress(Keys.M, null, out requesteeIndex))
            {
                //TO DO implement menu!
            }
            if (input.IsNewKeyPress(Keys.Q, null, out requesteeIndex))
            {
                // DEBUGGING FUNCTION
                Console.Out.WriteLine("Num Slides in ScreenManager: " + ScreenManager.NumScreens);
                Console.Out.WriteLine("Num Hidden Slides in ScreenManager: " + ScreenManager.NumScreensHidden);
            }
            //Press 'r' to start recording. If you're recording, it will stop recording
            if (input.IsNewKeyPress(Keys.R, null, out requesteeIndex))
            {
                beginRecording();
            }
            //Press 'p' to play recording. If already playing, will stop it.
            if (input.IsNewKeyPress(Keys.P, null, out requesteeIndex))
            {
                playAudio();
            }
            if (input.IsNewKeyPress(Keys.W, null, out requesteeIndex))
            {
                // DEBUGGING FOR PLAY ALL FUNCTIONALITY
                parentSlideMenu.PlayAll(2000);
            }
            #endregion

        }
        #endregion
        #region ChangeBackground(index)
       /// <summary>
       /// Changes the background of the slide to be the new background texture
       /// indicated by the index for the background array.
       /// </summary>
       /// <param name="newBackgroundIndex"></param> 
        public void ChangeBackground(int newBackgroundIndex)
        {
            this.backgroundScene = ScreenManager.GetBackgroundScene(newBackgroundIndex);
        }
        #endregion
        #region Captured()
        /*Captured
         * current Capture method will print two sprites (a jpg and a png) the first time is captured.
         * subsequent capture events, will put extra question mark pngs on the screen
         */
        private void Captured()
        //public void Captured()
        {
            SkeletonJoints curJoints = new SkeletonJoints(ScreenManager.CurSkeleton);
            capturedSkeletons.Add(curJoints);
            capturedAvatarIndices.Add(ScreenManager.CurAvatarIndex);
            //capturedAvatarIndices.Add(ScreenManager.Ch
            //Create a Sprite with the current Avatar
            //Sprite2D curAvatar = new Sprite2D(ScreenManager.CurSkeletonTexture(), ScreenManager.CurSkeletonRectangle(), ScreenManager.CurSekeltonColor());
            //slideObjects.Add(curAvatar);

           // CharacterObject charObj = new CharacterObject(ScreenManager.CurSkeletonTracker.SkeletonShapes);
           // this.slideObjects.Add(charObj);
        }
        #endregion
         
        #region Display
        public override void Draw(GameTime gameTime)
        {

            ScreenManager.Game.GraphicsDevice.Clear(backColor);

           //draws all the objects on the slide
            spriteBatch.Begin();
           // spriteBatch.Draw(background_dirty, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            //starts by drawing the backgrounds
            /*
            int sideMenuWidth = 75;
            int dockWidth = 40;
            int sideMenuIconOffset_X = 0;  
            int sideMenuIconOffset_Y = 10;  
            Rectangle sideMenuDockRect = new Rectangle(0, 0, dockWidth, viewport.Height);
            Rectangle sideMenuIconsRect = new Rectangle(sideMenuIconOffset_X, sideMenuIconOffset_Y, sideMenuWidth, viewport.Height);
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);//new Rectangle(0, 0, viewport.Width, viewport.Height);
           
             */
            spriteBatch.Draw(ScreenManager.BackgroundExtraPages, ScreenManager.fullscreenRectangle, Color.White);
            spriteBatch.Draw(backgroundScene,ScreenManager.fullscreenRectangle, Color.White);
          
            for(int i = 0; i < slideObjects.Count; i++){
               SlideObject curSprite = slideObjects[i];
                 curSprite.Draw(gameTime,spriteBatch);
              
            }
            // Draws page number of the slide
            int fontHeightAdj = 50; //adjust height by the height of the font
            int menuWidth = ScreenManager.MainGestureMenu.Width;
            int x = (viewport.Width / 2) - menuWidth; //- ScreenManager.;
            int y = viewport.Height - fontHeightAdj;

            spriteBatch.DrawString(ScreenManager.Font, "~ Page " + this.parentSlideMenu.CurrentPageNumber() +" ~", new Vector2(x, y), Color.DarkSlateGray);
            //
          // SkeletonDerived skel = new SkeletonDerived(ScreenManager.CurSkeleton);
            
            //skel.Joints = ScreenManager.CurSkeleton.Joints;
            for(int i = 0; i < capturedSkeletons.Count; i++){
                ScreenManager.CurCharacter.update(capturedSkeletons[i]);
                ScreenManager.CurCharacter.draw(capturedAvatarIndices[i]);
            }
            spriteBatch.End();
            Console.WriteLine("capturedSkeletons.count " + capturedSkeletons.Count);
        }
        #endregion
        #region Speech Recognition
        public void SlideRecognizerSaidSomething(object sender, SpeechRecognizer.SaidSomethingEventArgs e)
        {
            //if (!System.Object.ReferenceEquals(ScreenManager.GetScreens()[ScreenManager.NumScreens - 1], this))
            //    return;
            switch (e.Verb)
            {

                case SpeechRecognizer.Verbs.Capture:
                    Console.WriteLine("*****SLIDE Recognized 'Capture'!!!!!!!!!!!!!!!!!!");
                    Captured();
                    break;
                case SpeechRecognizer.Verbs.New:
                    Console.WriteLine("*****SLIDE Recognized 'New Slide'!!!!!!!!!!!!!!!!!!");
                    parentSlideMenu.NewSlide();
                    break;
                case SpeechRecognizer.Verbs.Back:
                    Console.WriteLine("*****SLIDE Recognized 'Back'!!!!!!!!!!!!!!!!!!");
                    parentSlideMenu.PreviousSlide();
                    this.ExitScreen();
                    break;
                case SpeechRecognizer.Verbs.Next:
                    Console.WriteLine("*****SLIDE Recognized 'Next'!!!!!!!!!!!!!!!!!! ");
                    parentSlideMenu.NextSlide();
                    break;
            }
        }
        #endregion


    }
       
     

}
