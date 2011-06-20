using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace KinectLibrary
{
    /// <summary>
    /// Good representation of a single Kinect
    /// </summary>
    public class Kinect
    {
        private static  Kinect gIstance;
        public static Kinect Instance {
            get {
                if (gIstance == null)
                    gIstance = new Kinect();
                return gIstance;
                
            }
        }

        private readonly Recognizer m_VoiceCommand = new Recognizer();
        private readonly Runtime m_Nui = new Runtime();
        protected bool m_Initialized;
        // this is a singleton
        private Kinect()
        {
        }

        public Runtime Nui { get { return m_Nui; } }
        public Recognizer VoiceCommand { get { return m_VoiceCommand; } }
        public Camera Motor { get { return Nui.NuiCamera; } }

        public virtual void GuaranteeInitialized()
        {
            if (!Initialized)
                Initialized = true;
        }
        protected bool Initialized { 
            get { return m_Initialized; }
            set
            {
                if (m_Initialized == value)
                    return;
                if (value)
                    InitializeNui();
                else
                    UninitializeNui();
            }
        }

        public virtual void AddVoiceCommand(Action act)
        {
            m_VoiceCommand.AddAction(act.Name,act);
        }

        public virtual void AddVoiceCommand(String name, Action act)
        {
            m_VoiceCommand.AddAction(name,act);
        }
  
        protected void InitializeNui()
        {
            UninitializeNui();
            Runtime nui = Nui;
             nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
    
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.SkeletonEngine.TransformSmooth = true;
            m_Initialized = true;
            // A little low - good for me
            int mid = (Camera.ElevationMaximum + 2 * Camera.ElevationMinimum) / 3;
            nui.NuiCamera.ElevationAngle = mid;

            AddVoiceCommand(new ThreadStartAction("Up", CameraUp));
            AddVoiceCommand(new ThreadStartAction("Down", CameraDown));

            m_VoiceCommand.BuildGrammer();
          }

        protected void UninitializeNui()
        {
            Runtime nui = Nui;
            if (m_Initialized)
                nui.Uninitialize();
            m_Initialized = false;
        }

        /// <summary>
        /// move up 5%  in to give to an action
        /// </summary>
        public void CameraUp()
        {
            ChangeCameraPosition(+0.05);
        }

        /// <summary>
        /// move down 5%  in to give to an action
        /// </summary>
        public void CameraDown()
        {
            ChangeCameraPosition(-0.05);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="del">fracion of the range</param>
        public void ChangeCameraPosition(double del)
        {
            double range = Camera.ElevationMaximum - Camera.ElevationMinimum;
            int move = (int)(range * del);
            int oldAngle = CameraAngle;
            int newpos = oldAngle + move;
            newpos = Math.Max(Camera.ElevationMinimum, newpos);
            newpos = Math.Min(Camera.ElevationMaximum, newpos);
            CameraAngle = newpos;
        }


        /// <summary>
        ///  represent the angle of the camera
        /// </summary>
        public int CameraAngle
        {
            get
            {
                if (Nui == null) return 0;
                if (Nui.NuiCamera == null) return 0;
                return Nui.NuiCamera.ElevationAngle;
            }
            set
            {
                if (Nui == null) return ;
                if (Nui.NuiCamera == null) return;
                int angle = Math.Min(Camera.ElevationMaximum, value);
                angle = Math.Max(Camera.ElevationMinimum, angle);
                if (Nui.NuiCamera.ElevationAngle == angle)
                    return;
                Nui.NuiCamera.ElevationAngle = angle;
            }
        }

    }
}
