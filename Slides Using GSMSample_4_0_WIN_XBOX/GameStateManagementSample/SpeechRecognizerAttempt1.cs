//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

// This module provides sample code used to demonstrate the use
// of the KinectAudioSource for speech recognition

// IMPORTANT: This sample requires the Speech Platform SDK (v11) to be installed on the developer workstation

namespace GameStateManagement
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;

    public class SpeechRecognizerAttempt1 //: IDisposable
    {
        SpeechRecognitionEngine speechRecognizer;
        private KinectSensor _kinect;

        
        // This method exists so that it can be easily called and return safely if the speech prereqs aren't installed.
        // We isolate the try/catch inside this class, and don't impose the need on the caller.
        public static SpeechRecognizer Create(KinectSensor kinect)
        {
            SpeechRecognizer recognizer = null;
           
            try
            {
                recognizer = new SpeechRecognizer(kinect);
            }
            catch (Exception)
            {
                // speech prereq isn't installed. a null recognizer will be handled properly by the app.
            }

            return recognizer;
        }

         private SpeechRecognizer(KinectSensor kinect)
        {
            RecognizerInfo ri = GetKinectRecognizer();
            this.speechRecognizer = new SpeechRecognitionEngine(ri);
            // Obtain the KinectAudioSource to do audio capture
            KinectAudioSource source = kinect.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None; // No AEC for this sample
            source.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition

            //this.LoadGrammar(kinect);
       // }

        /*public static void Main(string[] args)
        {
            // Obtain a KinectSensor if any are available
            KinectSensor sensor = (from sensorToCheck in KinectSensor.KinectSensors where sensorToCheck.Status == KinectStatus.Connected select sensorToCheck).FirstOrDefault();
            if (sensor == null)
            {
                Console.WriteLine(
                        "No Kinect sensors are attached to this computer or none of the ones that are\n" +
                        "attached are \"Connected\".\n" +
                        "Attach the KinectSensor and restart this application.\n" +
                        "If that doesn't work run SkeletonViewer-WPF to better understand the Status of\n" +
                        "the Kinect sensors.\n\n" +
                        "Press any key to continue.\n");

                // Give a chance for user to see console output before it is dismissed
                Console.ReadKey(true);
                return;
            }

            sensor.Start();
       
            // Obtain the KinectAudioSource to do audio capture
            KinectAudioSource source = sensor.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None; // No AEC for this sample
            source.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition
       // */
        // private void LoadGrammar(KinectSensor kinect)
         //{
           // RecognizerInfo ri = GetKinectRecognizer();

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

            //using (var sre = new SpeechRecognitionEngine(ri.Id))
            //{
           // speechRecognizer 
                var colors = new Choices();
                colors.Add("red");
                colors.Add("green");
                colors.Add("blue");

                var gb = new GrammarBuilder { Culture = ri.Culture };

                // Specify the culture to match the recognizer in case we are running in a different culture.                                 
                gb.Append(colors);

                // Create the actual Grammar instance, and then load it into the speech recognizer.
                var g = new Grammar(gb);

                speechRecognizer.LoadGrammar(g);
                speechRecognizer.SpeechRecognized += SreSpeechRecognized;
                speechRecognizer.SpeechHypothesized += SreSpeechHypothesized;
                speechRecognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
                speechRecognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(SpeechDetectedHandler);
                Console.WriteLine("IN Speech Reconizer load function");
               // speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
             ///*
                using (Stream s = kinect.AudioSource.Start())
                {
                    speechRecognizer.SetInputToAudioStream(
                        s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

                    Console.WriteLine("Recognizing speech. Say: 'red', 'green' or 'blue'. Press ENTER to stop");

                    speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                    Console.ReadLine();
                    Console.WriteLine("Stopping recognizer ...");
                    speechRecognizer.RecognizeAsyncStop();
                }
              //* */
           // }
        }

         // Handle the SpeechDetected event.
        private static void SpeechDetectedHandler(object sender, SpeechDetectedEventArgs e)
         {
             Console.WriteLine(" In SpeechDetectedHandler:");
             Console.WriteLine(" - AudioPosition = {0}", e.AudioPosition);
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

        private static void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("\nSpeech Rejected");
            if (e.Result != null)
            {
                DumpRecordedAudio(e.Result.Audio);
            }
        }

        private static void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        private static void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.7)
            {
                Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}", e.Result.Text, e.Result.Confidence);
            }
            else
            {
                Console.WriteLine("\nSpeech Recognized but confidence was too low: \t{0}", e.Result.Confidence);
                DumpRecordedAudio(e.Result.Audio);
            }
        }

        private static void DumpRecordedAudio(RecognizedAudio audio)
        {
            if (audio == null)
            {
                return;
            }

            int fileId = 0;
            string filename;
            while (File.Exists((filename = "RetainedAudio_" + fileId + ".wav")))
            {
                fileId++;
            }

            Console.WriteLine("\nWriting file: {0}", filename);
            using (var file = new FileStream(filename, System.IO.FileMode.CreateNew))
            {
                audio.WriteToWaveStream(file);
            }
        }
    }
}
