using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
using DTWGestureRecognition;

namespace GameStateManagement.DTWGestureRecognition
{
    class DTWImplementation
    {
        #region vars
        DtwGestureRecognizer _dtw;
        ArrayList _video;
       // SkeletonTracker _skeleton;
        ScreenManager _screenManager;
        
        private int _flipFlop = 0;
        #endregion
        #region const vars
        private const int MinimumFrames = 6;
        private const int BufferSize = 32;
        private const int CaptureCountdownSeconds = 3;
        private const string GestureSaveFileLocation = @"cycle.txt";
        private const int Ignore = 2;
        #endregion
        KinectSensor kinect;

        // DTWImplementation(KinectSensor s, SkeletonTracker st)
        public DTWImplementation(KinectSensor s, ScreenManager sm)
        {
            _dtw = new DtwGestureRecognizer(12, 0.6, 2, 2, 10);
            _video = new ArrayList();
            _screenManager = sm;
            kinect = s;
            System.IO.FileInfo fi = new System.IO.FileInfo(GestureSaveFileLocation);
            LoadGesturesFromFile(fi.FullName);

            s.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(DTWSkeletonReady);
            Skeleton2DDataExtract.Skeleton2DdataCoordReady += NuiSkeleton2DdataCoordReady;
        }
        private void DTWSkeletonReady(object sender, SkeletonFrameReadyEventArgs s)
        {
            SkeletonFrame sf = s.OpenSkeletonFrame();
            if (sf == null)
                return;
            Skeleton[] sdata = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(sdata);
            sf.Dispose();
            foreach (Skeleton data in sdata)
            {
                if (data != null)
                    Skeleton2DDataExtract.ProcessData(data);
            }
        }

        /// <summary>
        /// Opens the sent text file and creates a _dtw recorded gesture sequence
        /// Currently not very flexible and totally intolerant of errors.
        /// </summary>
        /// <param name="fileLocation">Full path to the gesture file</param>
        /// Work Cited: http://kinectdtw.codeplex.com/
        private void LoadGesturesFromFile(string fileLocation)
        {
            int itemCount = 0;
            string line;
            string gestureName = String.Empty;

            // TODO I'm defaulting this to 12 here for now as it meets my current need but I need to cater for variable lengths in the future
            ArrayList frames = new ArrayList();
            double[] items = new double[12];

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(fileLocation);
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("@"))
                {
                    gestureName = line;
                    continue;
                }

                if (line.StartsWith("~"))
                {
                    frames.Add(items);
                    itemCount = 0;
                    items = new double[12];
                    continue;
                }

                if (!line.StartsWith("----"))
                {
                    items[itemCount] = Double.Parse(line);
                }

                itemCount++;

                if (line.StartsWith("----"))
                {
                    _dtw.AddOrUpdate(frames, gestureName);
                    frames = new ArrayList();
                    gestureName = String.Empty;
                    itemCount = 0;
                }
            }

            file.Close();
        }

        /// <summary>
        /// Runs every time our 2D coordinates are ready.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="a">Skeleton 2Ddata Coord Event Args</param>
        /// Work Cited: http://kinectdtw.codeplex.com/
        private void NuiSkeleton2DdataCoordReady(object sender, Skeleton2DdataCoordEventArgs a)
        {
            // We need a sensible number of frames before we start attempting to match gestures against remembered sequences
            if (_video.Count > MinimumFrames)
            {
                ////Debug.WriteLine("Reading and video.Count=" + video.Count);
                string s = _dtw.Recognize(_video);
                if (!s.Contains("__UNKNOWN"))
                {
                    // There was no match so reset the buffer
                    _video = new ArrayList();
                }
                if (s.Contains("Avatar"))
                {
                    //To Do make sure that change avatar has a parameter for playerINdex
                    _screenManager.CycleAvatar();
                }
                if (s.Contains("Next"))
                {
                    SlideScreen curScreen = (SlideScreen)_screenManager.GetScreens()[_screenManager.NumScreens - 1];
                    curScreen.NextSlide();
                }
                if (s.Contains("Previous"))
                {
                    SlideScreen curScreen = (SlideScreen)_screenManager.GetScreens()[_screenManager.NumScreens - 1];
                    curScreen.PreviousSlide();
                }
                if (s.Contains("Background"))
                {
                    SlideScreen curScreen = (SlideScreen)_screenManager.GetScreens()[_screenManager.NumScreens - 1];
                    curScreen.CycleBackground();
                }
            }

            // Ensures that we remember only the last x frames
            if (_video.Count > BufferSize)
            {
                // Remove the first frame in the buffer
                _video.RemoveAt(0);
            }

            // Decide which skeleton frames to capture. Only do so if the frames actually returned a number. 
            // For some reason my Kinect/PC setup didn't always return a double in range (i.e. infinity) even when standing completely within the frame.
            // TODO Weird. Need to investigate this
            if (!double.IsNaN(a.GetPoint(0).X))
            {
                // Optionally register only 1 frame out of every n
                _flipFlop = (_flipFlop + 1) % Ignore;
                if (_flipFlop == 0)
                {
                    _video.Add(a.GetCoords());
                }
            }
        }

    }
}
