#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SkinnedModel
{
    /// <summary>
    /// The animation player is in charge of decoding bone position
    /// matrices from an animation clip.
    /// </summary>
    public class AnimationPlayer
    {
        #region Fields




        // Information about the currently playing animation clip.
        AnimationClip currentClipValue;
        TimeSpan currentTimeValue;
        int currentKeyframe;


        // Current animation transform matrices.
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransforms;


        // Backlink to the bind pose and skeleton hierarchy data.
        SkinningData skinningDataValue;


        #endregion

        //  JASON's CODE

        Game game;
        Model model;

        KinectSkeleton kinectSkeleton;

        //  END JASON's CODE


        /// <summary>
        /// Constructs a new animation player.
        /// </summary>
        public AnimationPlayer(Game g, String modelName)
        {
            game = g;
            // Load the model.
            model = game.Content.Load<Model>(modelName);

            // Look up our custom skinning information.
            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");
             
            skinningDataValue = skinningData;

            kinectSkeleton = new KinectSkeleton();

            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];

            //  creates the initial pose for the model
            //  (the important part here is the translation data for each bone)
            skinningData.BindPose.CopyTo(boneTransforms, 0);
            //  perform the first update to initialize Transform arrays
            Update(null, Matrix.Identity);
        }


        /// <summary>
        /// Starts decoding the specified animation clip.
        /// </summary>
        public void StartClip(AnimationClip clip)
        {
            /*if (clip == null)
                throw new ArgumentNullException("clip");

            currentClipValue = clip;
            currentTimeValue = TimeSpan.Zero;
            currentKeyframe = 0;
            */
            // Initialize bone transforms to the bind pose.
            //skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
        }


        /// <summary>
        /// Advances the current animation position.
        /// </summary>
        public void Update(Skeleton skeleton, Matrix rootTransform)
        {
            if (skeleton != null) UpdateBoneTransforms(skeleton);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();            
        }


        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(Skeleton skeleton)
        {

                kinectSkeleton.UpdateSkeleton(skeleton);

                //  update bone transforms (need to preserve translation data)
                //boneTransforms[8] =  kinectSkeleton.getMatrix(KinectSkeleton.Bone.Head)          * getTranslation(8);
                //boneTransforms[12] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ClavicleLeft)  * getTranslation(12);
                boneTransforms[13] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.UpperArmLeft) * getTranslation(13);
                boneTransforms[14] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ForearmLeft)   * getTranslation(14);
                //boneTransforms[15] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.HandLeft)      * getTranslation(15);
                boneTransforms[32] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.UpperArmRight) * getTranslation(32);
                boneTransforms[33] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ForearmRight)  * getTranslation(33);
                //boneTransforms[34] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.HandRight)     * getTranslation(34);
                boneTransforms[3] =  kinectSkeleton.getMatrix(KinectSkeleton.Bone.UpperBack) * Matrix.CreateRotationZ((float)Math.PI/4)     * getTranslation(3);
                //boneTransforms[3] =  kinectSkeleton.getMatrix(KinectSkeleton.Bone.LowerBack)     * getTranslation(3);
                boneTransforms[1] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.Pelvis) * getTranslation(1);
                boneTransforms[50] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ThighLeft)     * getTranslation(50);
                boneTransforms[51] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ShinLeft)      * getTranslation(51);
                //boneTransforms[52] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.FootLeft)      * getTranslation(52);
                boneTransforms[54] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ThighRight)    * getTranslation(54);
                boneTransforms[55] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.ShinRight)     * getTranslation(55);
                //boneTransforms[56] = kinectSkeleton.getMatrix(KinectSkeleton.Bone.FootRight)     * getTranslation(56);
                //*/

                //  will do pelvis later, for now just translate
                
        }

        public Matrix getTranslation(int index)
        {
            Matrix t = Matrix.CreateTranslation(
                                                   boneTransforms[index].M41,
                                                   boneTransforms[index].M42,
                                                   boneTransforms[index].M43
                                               );
            return t;
        }

        public void Draw(float cameraArc, float cameraDistance, float cameraRotation)
        {
            Matrix[] bones = GetSkinTransforms();

            // Compute camera matrices.
            Matrix view = Matrix.CreateTranslation(0, -40, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    game.GraphicsDevice.Viewport.AspectRatio,
                                                                    1,
                                                                    10000);

            // Render the skinned mesh.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningDataValue.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningDataValue.InverseBindPose[bone] * worldTransforms[bone];
            }
        }


        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }


        /// <summary>
        /// Gets the clip currently being decoded.
        /// </summary>
        public AnimationClip CurrentClip
        {
            get { return currentClipValue; }
        }


        /// <summary>
        /// Gets the current play position.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return currentTimeValue; }
        }
    }
}
