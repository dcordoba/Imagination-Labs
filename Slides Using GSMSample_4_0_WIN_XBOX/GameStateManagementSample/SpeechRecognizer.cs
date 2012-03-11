//------------------------------------------------------------------------------
// <copyright file="SpeechRecognizer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

// This module provides sample code used to demonstrate the use
// of the KinectAudioSource for speech recognition in a game setting.

// IMPORTANT: This sample requires the Speech Platform SDK (v11) to be installed on the developer workstation

namespace GameStateManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.Windows.Media;
   // using ShapeGame.Utils;

    public class SpeechRecognizer : IDisposable
    {
        private readonly Dictionary<string, WhatSaid> gameplayPhrases = new Dictionary<string, WhatSaid>
            {
                { "Faster", new WhatSaid { Verb = Verbs.Faster } },
                { "Slower", new WhatSaid { Verb = Verbs.Slower } },
                { "Bigger Shapes", new WhatSaid { Verb = Verbs.Bigger } },
                { "Bigger", new WhatSaid { Verb = Verbs.Bigger } },
                { "Larger", new WhatSaid { Verb = Verbs.Bigger } },
                { "Huge", new WhatSaid { Verb = Verbs.Biggest } },
                { "Giant", new WhatSaid { Verb = Verbs.Biggest } },
                { "Biggest", new WhatSaid { Verb = Verbs.Biggest } },
                { "Super Big", new WhatSaid { Verb = Verbs.Biggest } },
                { "Smaller", new WhatSaid { Verb = Verbs.Smaller } },
                { "Tiny", new WhatSaid { Verb = Verbs.Smallest } },
                { "Super Small", new WhatSaid { Verb = Verbs.Smallest } },
                { "Smallest", new WhatSaid { Verb = Verbs.Smallest } },
                { "More Shapes", new WhatSaid { Verb = Verbs.More } },
                { "More", new WhatSaid { Verb = Verbs.More } },
                { "Less", new WhatSaid { Verb = Verbs.Fewer } },
                { "Fewer", new WhatSaid { Verb = Verbs.Fewer } },
            };

        private readonly Dictionary<string, WhatSaid> shapePhrases = new Dictionary<string, WhatSaid>
            {
                /*
                { "Seven Pointed Stars", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Star7 } },
                { "Triangles", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Triangle } },
                { "Squares", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Square } },
                { "Boxes", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Square } },
                { "Hexagons", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Hex } },
                { "Pentagons", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Pentagon } },
                { "Stars", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Star } },
                { "Circles", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Circle } },
                { "Balls", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Circle } },
                { "Bubbles", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.Bubble } },
                { "All Shapes", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.All } },
                { "Everything", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.All } },
                { "Shapes", new WhatSaid { Verb = Verbs.DoShapes, Shape = PolyType.All } },
                 * */
            };

        private readonly Dictionary<string, WhatSaid> colorPhrases = new Dictionary<string, WhatSaid>
            {
                /*
                { "Every Color", new WhatSaid { Verb = Verbs.RandomColors } },
                { "All Colors", new WhatSaid { Verb = Verbs.RandomColors } },
                { "Random Colors", new WhatSaid { Verb = Verbs.RandomColors } },
                {
                    "Red",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(240, 60, 60) }
                    },
                {
                    "Green",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(60, 240, 60) }
                    },
                {
                    "Blue",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(60, 60, 240) }
                    },
                {
                    "Yellow",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(240, 240, 60) }
                    },
                {
                    "Orange",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(255, 110, 20) }
                    },
                {
                    "Purple",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(70, 30, 255) }
                    },
                {
                    "Violet",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(160, 30, 245) }
                    },
                {
                    "Pink",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(255, 128, 225) }
                    },
                {
                    "Gray",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(192, 192, 192) }
                    },
                {
                    "Brown",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(130, 80, 50) }
                    },
                {
                    "Dark",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(40, 40, 40) }
                    },
                {
                    "Black", new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(5, 5, 5) }
                    },
                {
                    "Bright",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(240, 240, 240) }
                    },
                {
                    "White",
                    new WhatSaid { Verb = Verbs.Colorize, Color = System.Windows.Media.Color.FromRgb(255, 255, 255) }
                    },
                 * */
            };

        private readonly Dictionary<string, WhatSaid> singlePhrases = new Dictionary<string, WhatSaid>
            {
                { "Speed Up", new WhatSaid { Verb = Verbs.Faster } },
                { "Slow Down", new WhatSaid { Verb = Verbs.Slower } },
                { "Reset", new WhatSaid { Verb = Verbs.Reset } },
                { "Clear", new WhatSaid { Verb = Verbs.Reset } },
                { "Stop", new WhatSaid { Verb = Verbs.Pause } },
                { "Pause Game", new WhatSaid { Verb = Verbs.Pause } },
                { "Freeze", new WhatSaid { Verb = Verbs.Pause } },
                { "Unfreeze", new WhatSaid { Verb = Verbs.Resume } },
                { "Resume", new WhatSaid { Verb = Verbs.Resume } },
                { "Continue", new WhatSaid { Verb = Verbs.Resume } },
                { "Play", new WhatSaid { Verb = Verbs.Resume } },
                { "Start", new WhatSaid { Verb = Verbs.Resume } },
                { "Go", new WhatSaid { Verb = Verbs.Resume } },
            };

        private SpeechRecognitionEngine sre;
        private KinectAudioSource kinectAudioSource;
        private bool paused;
        private bool isDisposed;

        private SpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            this.sre = new SpeechRecognitionEngine(ri);
            this.LoadGrammar(this.sre);
        }

        public event EventHandler<SaidSomethingEventArgs> SaidSomething;

        public enum Verbs
        {
            None = 0,
            Bigger,
            Biggest,
            Smaller,
            Smallest,
            More,
            Fewer,
            Faster,
            Slower,
            Colorize,
            RandomColors,
            DoShapes,
            ShapesAndColors,
            Reset,
            Pause,
            Resume
        }

        public EchoCancellationMode EchoCancellationMode
        {
            get
            {
                this.CheckDisposed();

                return this.kinectAudioSource.EchoCancellationMode;
            }

            set
            {
                this.CheckDisposed();

                this.kinectAudioSource.EchoCancellationMode = value;
            }
        }

        // This method exists so that it can be easily called and return safely if the speech prereqs aren't installed.
        // We isolate the try/catch inside this class, and don't impose the need on the caller.
        public static SpeechRecognizer Create()
        {
            SpeechRecognizer recognizer = null;

            try
            {
                recognizer = new SpeechRecognizer();
            }
            catch (Exception)
            {
                // speech prereq isn't installed. a null recognizer will be handled properly by the app.
            }

            return recognizer;
        }

        public void Start(KinectAudioSource kinectSource)
        {
            this.CheckDisposed();

            this.kinectAudioSource = kinectSource;
            this.kinectAudioSource.AutomaticGainControlEnabled = false;
            this.kinectAudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = this.kinectAudioSource.Start();
            this.sre.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            this.CheckDisposed();

            if (this.sre != null)
            {
                this.kinectAudioSource.Stop();
                this.sre.RecognizeAsyncCancel();
                this.sre.RecognizeAsyncStop();

                this.sre.SpeechRecognized -= this.SreSpeechRecognized;
                this.sre.SpeechHypothesized -= this.SreSpeechHypothesized;
                this.sre.SpeechRecognitionRejected -= this.SreSpeechRecognitionRejected;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "sre",
            Justification = "This is suppressed because FXCop does not see our threaded dispose.")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stop();

                if (this.sre != null)
                {
                    // NOTE: The SpeechRecognitionEngine can take a long time to dispose
                    // so we will dispose it on a background thread
                    ThreadPool.QueueUserWorkItem(
                        delegate(object state)
                        {
                            IDisposable toDispose = state as IDisposable;
                            if (toDispose != null)
                            {
                                toDispose.Dispose();
                            }
                        },
                            this.sre);
                    this.sre = null;
                }

                this.isDisposed = true;
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("SpeechRecognizer");
            }
        }

        private void LoadGrammar(SpeechRecognitionEngine speechRecognitionEngine)
        {
            // Build a simple grammar of shapes, colors, and some simple program control
            var single = new Choices();
            foreach (var phrase in this.singlePhrases)
            {
                single.Add(phrase.Key);
            }

            var gameplay = new Choices();
            foreach (var phrase in this.gameplayPhrases)
            {
                gameplay.Add(phrase.Key);
            }

            var shapes = new Choices();
            foreach (var phrase in this.shapePhrases)
            {
                shapes.Add(phrase.Key);
            }

            var colors = new Choices();
            foreach (var phrase in this.colorPhrases)
            {
                colors.Add(phrase.Key);
            }

            var coloredShapeGrammar = new GrammarBuilder();
            coloredShapeGrammar.Append(colors);
            coloredShapeGrammar.Append(shapes);

            var objectChoices = new Choices();
            objectChoices.Add(gameplay);
            objectChoices.Add(shapes);
            objectChoices.Add(colors);
            objectChoices.Add(coloredShapeGrammar);

            var actionGrammar = new GrammarBuilder();
            actionGrammar.AppendWildcard();
            actionGrammar.Append(objectChoices);

            var allChoices = new Choices();
            allChoices.Add(actionGrammar);
            allChoices.Add(single);

            // This is needed to ensure that it will work on machines with any culture, not just en-us.
            var gb = new GrammarBuilder { Culture = speechRecognitionEngine.RecognizerInfo.Culture };
            gb.Append(allChoices);

            var g = new Grammar(gb);
            speechRecognitionEngine.LoadGrammar(g);
            speechRecognitionEngine.SpeechRecognized += this.SreSpeechRecognized;
            speechRecognitionEngine.SpeechHypothesized += this.SreSpeechHypothesized;
            speechRecognitionEngine.SpeechRecognitionRejected += this.SreSpeechRecognitionRejected;
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            var said = new SaidSomethingEventArgs { Verb = Verbs.None, Matched = "?" };

            if (this.SaidSomething != null)
            {
                this.SaidSomething(new object(), said);
            }

            Console.WriteLine("\nSpeech Rejected");
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.Write("\rSpeech Recognized: \t{0}", e.Result.Text);

            if ((this.SaidSomething == null) || (e.Result.Confidence < 0.3))
            {
                return;
            }

            var said = new SaidSomethingEventArgs{ Verb = 0, Phrase = e.Result.Text }; //new SaidSomethingEventArgs { RgbColor = System.Windows.Media.Color.FromRgb(0, 0, 0), Shape = 0, Verb = 0, Phrase = e.Result.Text };

            // First check for color, in case both color _and_ shape were both spoken
            bool foundColor = false;
            foreach (var phrase in this.colorPhrases)
            {
                if (e.Result.Text.Contains(phrase.Key) && (phrase.Value.Verb == Verbs.Colorize))
                {/*
                    said.RgbColor = phrase.Value.Color;
                    said.Matched = phrase.Key;
                    foundColor = true;
                  * */
                    break;
                }
            }

            // Look for a match in the order of the lists below, first match wins.
            List<Dictionary<string, WhatSaid>> allDicts = new List<Dictionary<string, WhatSaid>> { this.gameplayPhrases, this.shapePhrases, this.colorPhrases, this.singlePhrases };

            bool found = false;
            for (int i = 0; i < allDicts.Count && !found; ++i)
            {
                foreach (var phrase in allDicts[i])
                {
                    if (e.Result.Text.Contains(phrase.Key))
                    {
                        said.Verb = phrase.Value.Verb;
                        //said.Shape = phrase.Value.Shape;
                        if ((said.Verb == Verbs.DoShapes) && foundColor)
                        {
                            said.Verb = Verbs.ShapesAndColors;
                            said.Matched += " " + phrase.Key;
                        }
                        else
                        {
                            said.Matched = phrase.Key;
                            //said.RgbColor = phrase.Value.Color;
                        }

                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                return;
            }

            if (this.paused)
            {
                // Only accept restart or reset
                if ((said.Verb != Verbs.Resume) && (said.Verb != Verbs.Reset))
                {
                    return;
                }

                this.paused = false;
            }
            else
            {
                if (said.Verb == Verbs.Resume)
                {
                    return;
                }
            }

            if (said.Verb == Verbs.Pause)
            {
                this.paused = true;
            }

            if (this.SaidSomething != null)
            {
                this.SaidSomething(new object(), said);
            }
        }

        private struct WhatSaid
        {
            public Verbs Verb;
           // public PolyType Shape;
          //  public System.Windows.Media.Color Color;
        }

        public class SaidSomethingEventArgs : EventArgs
        {
            public Verbs Verb { get; set; }

           // public PolyType Shape { get; set; }

            //public System.Windows.Media.Color RgbColor { get; set; }

            public string Phrase { get; set; }

            public string Matched { get; set; }
        }
    }
}