using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Research.Kinect.Audio;
// note browse for C:\Program Files\Microsoft Speech Platform SDK\Assembly\Microsoft.Speech.dll
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.ComponentModel;

namespace KinectLibrary
{
    public class Recognizer : IActionFired
    {

        private KinectAudioSource kinectSource;
        private SpeechRecognitionEngine m_Sre;
        private const string RecognizerId = "SR_MS_en-US_Kinect_10.0";
        private IDictionary<String, Action> m_Words = new Dictionary<String, Action>();
        private Grammar m_CurrentGrammer;
        public event ActionFiredEventEventHandler ActionFired;


        public Recognizer()
        {
            RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();
            if (ri == null)
                return;

            SRE = new SpeechRecognitionEngine(ri.Id);
            Thread t = new Thread(StartDMO);
            t.Start();

  
        }

        public void AddAction(Action act)
        {
            AddAction(act.Name, act);
        }

        public void AddAction(String word,Action act)
        {
            m_Words[word] = act;
            act.PropertyChanged += onPropertyChanged;
        }

        public void onPropertyChanged(object caller, PropertyChangedEventArgs args)
        {
            Action act = (Action)caller;
            if ("Name" == args.PropertyName)
            {

            }

            if ("Enabled" == args.PropertyName)
            {
                BuildGrammer();
            }

        }


        public SpeechRecognitionEngine SRE
        {
            get { return m_Sre; }
            set
            {
                if (m_Sre == value)
                    return;
                if (m_Sre != null)
                {
                    m_Sre.SpeechRecognized += sre_SpeechRecognized;
                    m_Sre.SpeechHypothesized += sre_SpeechHypothesized;
                    m_Sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);
                }
                m_Sre = value;
                m_Sre.SpeechRecognized += sre_SpeechRecognized;
                m_Sre.SpeechHypothesized += sre_SpeechHypothesized;
                m_Sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);
 
            }
        }

        public virtual void BuildGrammer()
        {
            if (m_CurrentGrammer != null)
            {
                m_Sre.RecognizeAsyncCancel();
                m_Sre.RecognizeAsyncStop();
                m_Sre.UnloadGrammar(m_CurrentGrammer);
            }
            Choices choices = new Choices();
            foreach (String phrase in m_Words.Keys)
                choices.Add(phrase);


            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(choices);
            m_CurrentGrammer = new Grammar(gb);
            m_Sre.LoadGrammar(m_CurrentGrammer);
            m_Sre.RecognizeAsync(RecognizeMode.Multiple);
        }

    
 
        private void StartDMO()
        {
            kinectSource = new KinectAudioSource();
            kinectSource.SystemMode = SystemMode.OptibeamArrayOnly;
            kinectSource.FeatureMode = true;
            kinectSource.AutomaticGainControl = false;
            kinectSource.MicArrayMode = MicArrayMode.MicArrayAdaptiveBeam;
            var kinectStream = kinectSource.Start();
            m_Sre.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(
                                                  EncodingFormat.Pcm, 16000, 16, 1,
                                                  32000, 2, null));
        }
 

        public void Stop()
        {
            if (m_Sre != null)
            {
                m_Sre.RecognizeAsyncCancel();
                m_Sre.RecognizeAsyncStop();
                kinectSource.Dispose();
            }
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
             Console.WriteLine("\nSpeech Rejected");
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;
            String test = result.Text;
            Console.Write("\rSpeech Recognized: \t{0}", e.Result.Text);
            FireAction(test);

          }

        public Action this[String test]
        {
            get
            {
                Action act;
                m_Words.TryGetValue(test, out  act);
                return act;
            }
        }


        public void FireAction(String test)
        {

            FireAction(this[test]);
        }

        public void FireAction(Action test)
        {
            test.Fire();
            if (ActionFired != null)
            {
                ActionFired.Invoke(this, new ActionFiredEventArgs(test));
            }
        }
 
    }
}
