using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;

namespace GameStateManagement
{
    public class SkeletonDerived : Skeleton
    {
       // Skeleton skeleton;
        // public DerivedSkeleton(Skeleton oldSkel)
        public  JointCollectionDerived Joints { get; private set; }
        public SkeletonPoint Position { get; set; }
        //public SkeletonQuality Quality { get; set; }
        public int TrackingID { get; set; }
        public SkeletonTrackingState TrackingState { get; set; }
        public  FrameEdges ClippedEdges{ get; set; }

        public SkeletonDerived(Skeleton oldSkel)  
        {
           
           // oldSkel.Joints
          // this.Joints[JointType.HandLeft] = new Joint () ;
           foreach (Joint CurrentJoint in oldSkel.Joints)
           {
               this.UpdateJoint(CurrentJoint);
           }

           this.Position = oldSkel.Position;
           this.TrackingID = oldSkel.TrackingId;
           this.TrackingState = oldSkel.TrackingState;
           this.ClippedEdges = oldSkel.ClippedEdges;    
            
        }
        public void UpdateJoint(Joint NewJoint)
        {
           // this.Joints[NewJoint.JointType] = new JointDerived(NewJoint);//new Joint()//
           /* {
               // NewJoint.
                //JointType = NewJoint.JointType,
                Position = new SkeletonPoint(){
                    X = NewJoint.Position.X,
                    Y = NewJoint.Position.Y,
                    Z = NewJoint.Position.Z,
                },             
                TrackingState = NewJoint.TrackingState
            };
            * */
        }

       
    }
}
