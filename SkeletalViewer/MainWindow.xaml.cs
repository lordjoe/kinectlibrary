/////////////////////////////////////////////////////////////////////////
//
// This module contains code to do Kinect NUI initialization and
// processing and also to display NUI streams on screen.
//
// Copyright © Microsoft Corporation.  All rights reserved.  
// This code is licensed under the terms of the 
// Microsoft Kinect for Windows SDK (Beta) from Microsoft Research 
// License Agreement: http://research.microsoft.com/KinectSDK-ToU
//
/////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using KinectLibrary;

namespace SkeletalViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Kinect m_KinectDevice;
        private readonly SkeletonRenderer m_Renderer = new SkeletonRenderer();
        private int totalFrames = 0;
        private int lastFrames = 0;
        private DateTime lastTime = DateTime.MaxValue;

        byte[] depthFrame32 = new byte[320 * 240 * 4];

        private readonly SkeletonRenderer jointColors = new SkeletonRenderer();
     

        private void Window_Loaded(object sender, EventArgs e)
        {
            m_KinectDevice = Kinect.Instance;
            m_KinectDevice.GuaranteeInitialized();


            m_KinectDevice.VoiceCommand.ActionFired += OnVoiceCommand;
            lastTime = DateTime.Now;

            m_KinectDevice.Nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            m_KinectDevice.Nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            m_KinectDevice.Nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_ColorFrameReady);
        }

        /// <summary>
        /// display the name of the last voice command exexuted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnVoiceCommand(object sender, ActionFiredEventArgs e)
        {
            lastCommand.Text = "Said " + e.WasFired.Name;
        }

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage Image = e.ImageFrame.Image;
            KinectUtilities.convertDepthFrame(Image.Bits, depthFrame32);

            depth.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, depthFrame32, Image.Width * 4);

            ++totalFrames;

            DateTime cur = DateTime.Now;
            if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
                frameRate.Text = frameDiff.ToString() + " fps";
            }
        }


        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

              Runtime tr = Kinect.Instance.Nui;

               m_Renderer.Render(skeleton,tr, skeletonFrame,  brushes);
         }

        void nui_ColorFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // 32-bit per pixel, RGBA image
            PlanarImage Image = e.ImageFrame.Image;
            video.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            m_KinectDevice.Nui.Uninitialize();
            Environment.Exit(0);
        }

    }
}
