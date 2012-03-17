using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System;
using System.Diagnostics;


namespace GameStateManagement
{
    [Serializable]
    [DebuggerDisplay("Position:{Position} JointType:{JointType} TrackingState:{TrackingState}")]
    public class JointDerived
    {
        private Joint joint;
        public JointType jointType;

        public JointDerived(Joint NewJoint){
            joint = new Joint()
            {
               // JointType = NewJoint.JointType,
                Position = new SkeletonPoint(){
                    X = NewJoint.Position.X,
                    Y = NewJoint.Position.Y,
                    Z = NewJoint.Position.Z,
                },             
                TrackingState = NewJoint.TrackingState
            };
            jointType = NewJoint.JointType;
        }

        public JointType JointType {
            get { return jointType; }
            set { jointType = value; }
        } 

        /*
        public static bool operator !=(JointDerived joint1, JointDerived joint2);
        public static bool operator ==(JointDerived joint1, JointDerived joint2);

        public JointType JointType { get; internal set; }
        public SkeletonPoint Position { get; set; }
        public JointTrackingState TrackingState { get; set; }

        public bool Equals(Joint joint);
        public override bool Equals(object obj);
        public override int GetHashCode();
         * */
    }
}
