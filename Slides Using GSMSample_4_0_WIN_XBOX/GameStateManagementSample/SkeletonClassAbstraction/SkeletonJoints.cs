using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GameStateManagement
{

    public class SkeletonJoints
    {
        public Dictionary<JointType, SkeletonPoint> Joints;
        public SkeletonJoints(Skeleton skel)
        {
            this.Joints = new Dictionary<JointType, SkeletonPoint>();
            foreach (Joint joint in skel.Joints)
            {
                SkeletonPoint pt = new SkeletonPoint()
                {
                    X = joint.Position.X,
                    Y = joint.Position.Y,
                    Z = joint.Position.Z,
                };

                this.Joints.Add(joint.JointType, pt);
            }
        }

        public void UpdateJointPositions(Skeleton skel){
            foreach ( Joint joint in skel.Joints){
                SkeletonPoint curPt = this.Joints[joint.JointType];
                curPt.X = joint.Position.X;
                curPt.Y = joint.Position.Y;
                curPt.Z = joint.Position.Z;
                this.Joints[joint.JointType] = curPt;
                //this.Joints[joint.JointType].X = joint.Position.X;
                //this.Joints[joint.JointType].y = joint.Position.X;
            }
        }
    }
}
