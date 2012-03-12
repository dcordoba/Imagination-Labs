#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
#endregion


namespace GameStateManagement
{
    /// <summary>
    /// CharacterObject stores  an instance of a certain charater that should be displayed 
    /// on the screen after it has been "captured."
    /// At the moment, it stores all the joints of the skeleton, the shapes of the skeleton
    /// and a string name for what 3d avatar is being represented.. It uses
    /// the array of skeletonShapes to draw the character on the screen.
    /// (TODO: presumably once
    /// 3D avatars are integrated, then the skeletonShapes array won't be needed and the
    /// "Draw" method can be updated to directly draw the appropriate 3D avatar)
    /// </summary>
    public class CharacterObject: SlideObject
    {
        #region variables
        private string characterSkin;
        Skeleton skeleton;
        private Dictionary<JointType,Sprite2D> skeletonShapes;
        #endregion

        #region Initialization
        public CharacterObject(Skeleton skel, Dictionary<JointType, Sprite2D> skelShapes, string avatarSkin)
        {
            skeleton = skel;
            skeletonShapes = DeepCopyDictionary(skelShapes);
            characterSkin = avatarSkin;
        }
        //Inits characterSkin to "unknown" if nothing is passed in
        public CharacterObject(Skeleton skel, Dictionary<JointType, Sprite2D> skelShapes)
        {
            skeleton = skel;
            skeletonShapes = DeepCopyDictionary(skelShapes);
            characterSkin = "unknown";
        }
        //used for testing since in order to draw the skeleton all you need are skeletonShapes
        public CharacterObject( Dictionary<JointType, Sprite2D> skelShapes)
        {
            skeletonShapes = DeepCopyDictionary(skelShapes);
            skeleton = null;
            characterSkin = "unknown";
        }
        #endregion
        #region Draw
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
           
            foreach (KeyValuePair<JointType, Sprite2D> shape in skeletonShapes)
            {
               spriteBatch.Draw(shape.Value.Texture, shape.Value.Rectangle, shape.Value.Color);
            }
           
        }
        #endregion
        private Dictionary<JointType, Sprite2D> DeepCopyDictionary(Dictionary<JointType, Sprite2D> skelShapes)
        {
            Dictionary<JointType, Sprite2D> newDict = new Dictionary<JointType,Sprite2D>();
            foreach (KeyValuePair<JointType, Sprite2D> shape in skelShapes){
                Sprite2D newSprite = new Sprite2D(shape.Value.Texture, shape.Value.Rectangle, shape.Value.Color);
                newDict.Add(shape.Key, newSprite);
            }
            return newDict; 
        }

    }
}
