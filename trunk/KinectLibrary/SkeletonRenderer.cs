using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Research.Kinect.Nui;

using System.Windows;
using System.Windows.Shapes;

namespace KinectLibrary
{
    public class SkeletonRenderer
    {

        private readonly Dictionary<JointID, Brush> m_JointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };

        public SkeletonRenderer()
        {
            IsInWheelchair = true;
        }

        public bool IsInWheelchair { get; set; }

        public bool IsJointHidden(JointID id)
        {
            if (!IsInWheelchair)
                return true;
            switch (id)
            {
                case JointID.AnkleLeft:
                case JointID.AnkleRight:
                case JointID.KneeLeft:
                case JointID.KneeRight:
                case JointID.FootLeft:
                case JointID.FootRight:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsJointHidden(Joint  joint)
        {
            return IsJointHidden(joint.ID);
        }

       
        public Brush this[JointID joint] {
            get { return m_JointColors[joint]; }
            set { m_JointColors[joint] = value; }
        }

        public void Render(Panel skeleton, Runtime tr, SkeletonData data, Brush brush)
        {
            int width = (int)skeleton.Width;
            int height = (int)skeleton.Height;

            skeleton.Children.Add(KinectUtilities.getBodySegment(tr, data.Joints, brush, width, height, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
            skeleton.Children.Add(KinectUtilities.getBodySegment(tr, data.Joints, brush, width, height, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
            skeleton.Children.Add(KinectUtilities.getBodySegment(tr, data.Joints, brush, width, height, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
            if (!IsInWheelchair)
            {
                skeleton.Children.Add(KinectUtilities.getBodySegment(tr, data.Joints, brush, width, height, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                skeleton.Children.Add(KinectUtilities.getBodySegment(tr, data.Joints, brush, width, height, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));
            }

            // Draw joints
            foreach (Joint joint in data.Joints)
            {
                if (IsJointHidden(joint))
                    continue;
                Point jointPos = KinectUtilities.getDisplayPosition(tr, joint, width, height);
                Line jointLine = new Line();
                jointLine.X1 = jointPos.X - 3;
                jointLine.X2 = jointLine.X1 + 6;
                jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                jointLine.Stroke = m_JointColors[joint.ID];
                jointLine.StrokeThickness = 6;
                skeleton.Children.Add(jointLine);
            }
        }

        public void Render(Panel skeleton, Runtime tr, SkeletonFrame skeletonFrame, Brush[] brushes)
        {
            int iSkeleton = 0;
            skeleton.Children.Clear();
             foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                     Render(skeleton, tr, data, brush);
                }
                iSkeleton++;
            } // for each skeleton
        }
    }
    
}
