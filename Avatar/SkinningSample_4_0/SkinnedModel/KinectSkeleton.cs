using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;

namespace SkinnedModel
{
    class KinectSkeleton
    {
        Skeleton skeleton;

        public Matrix[] BoneTransforms;

        enum Joints {
            Head,
            ShoulderL, ShoulderCenter, ShoulderR,
            ElbowL, ElbowR,
            WristL, WristR,
            HandL, HandR,
            Spine,
            HipL, HipCenter, HipR,
            KneeL, KneeR,
            AnkleL, AnkleR,
            FootL, FootR
        };

        public enum Bone {
            Head,
            Clavicle,
            UpperArmLeft,
            UpperArmRight,
            ForearmLeft,
            ForearmRight,
            HandLeft,
            HandRight,
            UpperBack,
            LowerBack,
            Pelvis,
            ThighLeft,
            ThighRight,
            ShinLeft,
            ShinRight,
            FootLeft,
            FootRight
        };

        Vector3[] bones;

        public KinectSkeleton()
        {
            //skeletonData = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
            BoneTransforms = new Matrix[18];
            bones = new Vector3[18];
        }

        public void setMatrix(Bone bone, Matrix matrix)
        {
            BoneTransforms[(int)bone] = Matrix.Identity * matrix;
        }

        public Matrix getMatrix(Bone bone)
        {
            Matrix m = BoneTransforms[(int)bone];
            if (m.M44 == 0) m = Matrix.Identity;
            return m;
        }

        public void UpdateSkeleton(Skeleton s)
        {
            skeleton = s;

            UpdateBones();

            UpdatePelvis();
            UpdateUpperBack();
            UpdateUpperArmLeft();
            UpdateForearmLeft();
            //  UpdateHandLeft();  //  hand data really seems to be poor quality
            UpdateUpperArmRight();
            UpdateForearmRight();
            UpdateThighLeft();
            UpdateShinLeft();
            UpdateThighRight();
            UpdateShinRight();
            
        }

        private void UpdateBones()
        {
            bones[(int)Bone.Head] =          getBone(JointType.Head,           JointType.ShoulderCenter);
            bones[(int)Bone.Clavicle] =      getBone(JointType.ShoulderRight,  JointType.ShoulderLeft);
            bones[(int)Bone.UpperArmLeft] =  getBone(JointType.ShoulderLeft,   JointType.ElbowLeft);
            bones[(int)Bone.UpperArmRight] = getBone(JointType.ShoulderRight,  JointType.ElbowRight);
            bones[(int)Bone.ForearmLeft] =   getBone(JointType.ElbowLeft,      JointType.WristLeft);
            bones[(int)Bone.ForearmRight] =  getBone(JointType.ElbowRight,     JointType.WristRight);
            bones[(int)Bone.HandLeft] =      getBone(JointType.WristLeft,      JointType.HandLeft);
            bones[(int)Bone.HandRight] =     getBone(JointType.WristRight,     JointType.HandRight);
            bones[(int)Bone.UpperBack] =     getBone(JointType.Spine,          JointType.ShoulderCenter);
            bones[(int)Bone.LowerBack] =     getBone(JointType.HipCenter,      JointType.Spine);
            bones[(int)Bone.Pelvis] =        getBone(JointType.HipRight,       JointType.HipLeft);
            bones[(int)Bone.ThighLeft] =     getBone(JointType.HipLeft,        JointType.KneeLeft);
            bones[(int)Bone.ThighRight] =    getBone(JointType.HipRight,       JointType.KneeRight);
            bones[(int)Bone.ShinLeft] =      getBone(JointType.KneeLeft,       JointType.AnkleLeft);
            bones[(int)Bone.ShinRight] =     getBone(JointType.KneeRight,      JointType.AnkleRight);
            bones[(int)Bone.FootLeft] =      getBone(JointType.AnkleLeft,      JointType.FootLeft);
            bones[(int)Bone.FootRight] =     getBone(JointType.AnkleRight,     JointType.FootRight);
        }

        //  taking a vector x and another vector in the Z plane, compute a world basis matrix
        private Matrix getWorldRotation(Vector3 x, Vector3 inPlaneZ)
        {
            Vector3 X = Vector3.Normalize(x);
            Vector3 Y = Vector3.Normalize(Vector3.Cross(X, inPlaneZ));
            Vector3 Z = Vector3.Normalize(Vector3.Cross(X, Y));

            return Matrix.CreateWorld(new Vector3(0, 0, 0), Y, Z);
        }

        private Matrix RotateBasis(Matrix rotationBasis, Matrix basisToRotate)
        {
            return Matrix.CreateWorld(new Vector3(0, 0, 0),
                                      Vector3.Normalize(Vector3.Transform(basisToRotate.Forward, Matrix.Invert(rotationBasis))),
                                      Vector3.Normalize(Vector3.Transform(basisToRotate.Up,      Matrix.Invert(rotationBasis))));
        }

        private void UpdatePelvis()
        {
            //  in relation to a basis formed by spine and pelvis
            Vector3 clavicle = bones[(int)Bone.Clavicle];
            Vector3 upperBack = bones[(int)Bone.UpperBack];
            Vector3 lowerBack = bones[(int)Bone.LowerBack];
            Vector3 pelvis = bones[(int)Bone.Pelvis];

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(new Vector3(0, 1, 0), new Vector3(0, 0, 1));
            Matrix lowerBasis = getWorldRotation(new Vector3(0, 1, 0), Vector3.Cross(pelvis, new Vector3(0, 1, 0)));

            BoneTransforms[(int)Bone.Pelvis] = Matrix.CreateWorld(new Vector3(0,0,0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));// RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateUpperBack()
        {
            //  in relation to a basis formed by spine and pelvis
            Vector3 clavicle = bones[(int)Bone.Clavicle];
            Vector3 upperBack = bones[(int)Bone.UpperBack];
            Vector3 lowerBack = bones[(int)Bone.LowerBack];
            Vector3 pelvis = bones[(int)Bone.Pelvis];

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(lowerBack, Vector3.Cross(pelvis, upperBack));
            Matrix lowerBasis = getWorldRotation(upperBack, Vector3.Cross(clavicle, upperBack));

            BoneTransforms[(int)Bone.UpperBack] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateUpperArmLeft()
        {
            Vector3 upperBack = bones[(int)Bone.UpperBack];
            Vector3 clavicle = bones[(int)Bone.Clavicle];
            Vector3 leftUpperArm = bones[(int)Bone.UpperArmLeft];
            Vector3 leftForearm  = bones[(int)Bone.ForearmLeft];

            //  get basis components in world space
            Vector3 X = leftUpperArm;
            Vector3 Y = Vector3.Cross(leftForearm, X);
            Vector3 Z = Vector3.Cross(X, Y);

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(clavicle, Vector3.Cross(clavicle, upperBack));
            Matrix lowerBasis = getWorldRotation(X, Z);

            BoneTransforms[(int)Bone.UpperArmLeft] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateUpperArmRight()
        {
            Vector3 upperBack = bones[(int)Bone.UpperBack];
            Vector3 clavicle = -bones[(int)Bone.Clavicle];
            Vector3 rightUpperArm = bones[(int)Bone.UpperArmRight];
            Vector3 rightForearm = bones[(int)Bone.ForearmRight];

            //  get basis components in world space
            Vector3 X = rightUpperArm;
            Vector3 Y = Vector3.Cross(rightForearm, X);
            Vector3 Z = Vector3.Cross(X, Y);

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(clavicle, -Vector3.Cross(clavicle, upperBack));
            Matrix lowerBasis = getWorldRotation(X, Z);

            BoneTransforms[(int)Bone.UpperArmRight] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateForearmLeft()
        {
            Vector3 leftUpperArm = bones[(int)Bone.UpperArmLeft];
            Vector3 leftForearm = bones[(int)Bone.ForearmLeft];
            Vector3 leftHand = bones[(int)Bone.HandLeft];

            //  get basis components in world space
            Vector3 X = leftForearm;
            Vector3 Y = Vector3.Cross(X, leftUpperArm);
            Vector3 Z = Vector3.Cross(X, Y);

            Vector3 upperArmZ = Z;
            //  if angle between upper arm and forearm is less than 90, reverse the Z vector that will be used to calculate lower arm
            //  done this way because the rotation matrix for the upper bone is calculated assuming arm is straight
            if (Math.PI / 2 < Math.Acos(Vector3.Dot(Vector3.Normalize(leftUpperArm), Vector3.Normalize(leftForearm))))
            {
                upperArmZ = -Z;
            }

            /*/  the proper Z for the forarm is coplanar with the forearm and the hand
            Vector3 forearmZ = Vector3.Cross(leftForearm, Vector3.Cross(leftHand, leftForearm));
            if (Math.PI / 2 > Math.Acos(Vector3.Dot(Vector3.Normalize(forearmZ), Vector3.Normalize(upperArmZ))))
            {
                forearmZ = -forearmZ;
            }*/


            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(leftUpperArm, upperArmZ);
            Matrix lowerBasis = getWorldRotation(leftForearm, Z);

            BoneTransforms[(int)Bone.ForearmLeft] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateForearmRight()
        {
            Vector3 rightUpperArm = bones[(int)Bone.UpperArmRight];
            Vector3 rightForearm = bones[(int)Bone.ForearmRight];

            //  get basis components in world space
            Vector3 X = rightForearm;
            Vector3 Y = Vector3.Cross(X, rightUpperArm);
            Vector3 Z = Vector3.Cross(X, Y);

            Vector3 upperArmZ = Z;

            //  if angle between upper arm and forearm is less than 90, reverse the Z vector that will be used to calculate lower arm
            //  done this way because the rotation matrix for the upper bone is calculated assuming arm is straight
            if (Math.PI / 2 < Math.Acos(Vector3.Dot(Vector3.Normalize(rightUpperArm), Vector3.Normalize(rightForearm))))
            {
                upperArmZ = -Z;
            }

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(rightUpperArm, upperArmZ);
            Matrix lowerBasis = getWorldRotation(rightForearm, Z);

            BoneTransforms[(int)Bone.ForearmRight] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateThighLeft()
        {
            Vector3 upperBack = bones[(int)Bone.UpperBack];
            Vector3 pelvis = bones[(int)Bone.Pelvis];
            Vector3 leftThigh = bones[(int)Bone.ThighLeft];
            Vector3 leftShin = bones[(int)Bone.ShinLeft];

            //  get basis components in world space
            Vector3 X = leftThigh;
            Vector3 Y = Vector3.Cross(leftShin, X);
            Vector3 Z = Vector3.Cross(X, Y);

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(upperBack, Vector3.Cross(upperBack, pelvis));
            Matrix lowerBasis = getWorldRotation(X, Z);

            BoneTransforms[(int)Bone.ThighLeft] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateShinLeft()
        {
            Vector3 leftThigh = bones[(int)Bone.ThighLeft];
            Vector3 leftShin = bones[(int)Bone.ShinLeft];

            //  get basis components in world space
            Vector3 X = leftShin;
            Vector3 Y = Vector3.Cross(X, leftThigh);
            Vector3 Z = Vector3.Cross(X, Y);

            Vector3 thighZ = Z;
            //  if angle between upper arm and forearm is less than 90, reverse the Z vector that will be used to calculate lower arm
            //  done this way because the rotation matrix for the upper bone is calculated assuming arm is straight
            if (Math.PI / 2 < Math.Acos(Vector3.Dot(Vector3.Normalize(leftThigh), Vector3.Normalize(leftShin))))
            {
                thighZ = -Z;
            }

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(leftThigh, thighZ);
            Matrix lowerBasis = getWorldRotation(leftShin, Z);

            BoneTransforms[(int)Bone.ShinLeft] = RotateBasis(upperBasis, lowerBasis);
        }

        private void UpdateThighRight()
        {
            Vector3 upperBack = bones[(int)Bone.UpperBack];
            Vector3 pelvis = -bones[(int)Bone.Pelvis];
            Vector3 rightThigh = bones[(int)Bone.ThighRight];
            Vector3 rightShin = bones[(int)Bone.ShinRight];

            //  get basis components in world space
            Vector3 X = rightThigh;
            Vector3 Y = Vector3.Cross(rightShin, X);
            Vector3 Z = Vector3.Cross(X, Y);

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(upperBack, -Vector3.Cross(upperBack, pelvis));
            Matrix lowerBasis = getWorldRotation(X, Z);

            BoneTransforms[(int)Bone.ThighRight] = RotateBasis(upperBasis, lowerBasis);
        }


        private void UpdateShinRight()
        {
            Vector3 rightThigh = bones[(int)Bone.ThighRight];
            Vector3 rightShin = bones[(int)Bone.ShinRight];

            //  get basis components in world space
            Vector3 X = rightShin;
            Vector3 Y = Vector3.Cross(X, rightThigh);
            Vector3 Z = Vector3.Cross(X, Y);

            Vector3 thighZ = Z;
            //  if angle between upper arm and forearm is less than 90, reverse the Z vector that will be used to calculate lower arm
            //  done this way because the rotation matrix for the upper bone is calculated assuming arm is straight
            if (Math.PI / 2 < Math.Acos(Vector3.Dot(Vector3.Normalize(rightThigh), Vector3.Normalize(rightShin))))
            {
                thighZ = -Z;
            }

            //  get parent basis in world space
            Matrix upperBasis = getWorldRotation(rightThigh, thighZ);
            Matrix lowerBasis = getWorldRotation(rightShin, Z);

            BoneTransforms[(int)Bone.ShinRight] = RotateBasis(upperBasis, lowerBasis);
        }

        private Vector3 getBone(JointType j1, JointType j2)
        {
            Vector3 jA = jointTypeToVector(j1);
            Vector3 jB = jointTypeToVector(j2);
            Vector3 bone = jB - jA;
            bone.Normalize();
            return  bone;
        }

        private Vector3 jointTypeToVector(JointType jt)
        {
            //  NOTE, y z swapped and z is negative in accordance with more standard mapping
            return new Vector3(skeleton.Joints[jt].Position.X,
                               skeleton.Joints[jt].Position.Y,
                               skeleton.Joints[jt].Position.Z);
        }

    }
}
