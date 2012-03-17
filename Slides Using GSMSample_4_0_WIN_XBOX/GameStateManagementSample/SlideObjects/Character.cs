/*
 * Usage
 * 
 *     Character(Game)
 *         - initialize character with reference to the game
 *     Character.setKinect(KinectSensor)
 *         - character requires a reference to kinect sensor for drawing/scaling purposes
 *     Character.load(String[])
 *         - a list of character image directories
 *         - each directory should contain the 15 files necessary for a character:
 *               Head
 *               Torso
 *               Hips
 *               UpperArmLeft
 *               UpperArmRight
 *               ForearmLeft
 *               ForearmRight
 *               HandLeft
 *               HandRight
 *               ThighLeft
 *               ThighRight
 *               ShinLeft
 *               ShinRight
 *               FootLeft
 *               FootRight
 *    Character.update(Skeleton)
 *        - pass in a skeleton that you want the character to pose in
 *        - you can call this function wherever you have a reference to a skeleton
 *            you may also update, then draw, then update and draw again within the
 *            same frame and you will get 2 avatars drawn to the screen
 *    Character.draw(characterIndex)
 *        - pass in the index corresponding to the character directory that was
 *          loaded in Character.load and that character's image will be drawn in
 *          the pose of the last skeleton that you passed in
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
    public class Character
    {
        Game game;
        //Skeleton skeleton;
        SkeletonJoints skeleton;
        KinectSensor sensor;
        Texture2D[][] sprites;
        SpriteBatch spriteBatch;
        Point HandLeft;
        Point HandRight;
        String[] spriteNames = {
            "Head",          "Torso",        "Hips",
            "UpperArmLeft",  "ForearmLeft",  "HandLeft",
            "UpperArmRight", "ForearmRight", "HandRight",
            "ThighLeft",     "ShinLeft",     "FootLeft",
            "ThighRight",    "ShinRight",    "FootRight"
        };

        float[] widthHeightRatio = {
            0.15f, 0.2f,  0.2f,
            0.1f,  0.1f,  0.1f,
            0.1f,  0.1f,  0.1f,
            0.1f,  0.1f,  0.1f,
            0.1f,  0.1f,  0.1f
        };

        public Character(Game game, SpriteBatch batch)
        {
            // TODO: Complete member initialization
            this.game = game;
            spriteBatch = batch;
        }

        public void setKinect(KinectSensor sensor)
        {
            this.sensor = sensor;
        }

        public void load(String[] contentDirectories)
        {
            sprites = new Texture2D[contentDirectories.Length][];
            //  load all the character's
            for (int i = 0; i < contentDirectories.Length; i++)
            {
                sprites[i] = new Texture2D[spriteNames.Length];
                for (int j = 0; j < spriteNames.Length; j++)
                {
                    String filepath = contentDirectories[i] + "/" + spriteNames[j];
                    sprites[i][j] = game.Content.Load<Texture2D>(filepath);
                }
            }
        }

        public void update(SkeletonJoints skeleton)//Skeleton skeleton)
        {
            this.skeleton = skeleton;

            HandLeft = jointToPoint(skeleton.Joints[JointType.HandLeft]);
            HandRight = jointToPoint(skeleton.Joints[JointType.HandRight]);
        }

        public Point getLeftHandPoint()
        {
            return HandLeft;
        }

        public Point getRightHandPoint()
        {
            return HandRight;
        }

        public void draw(int characterIndex)
        {
            if (this.skeleton == null || this.sensor == null) return;

            Point headTop = jointToPoint(skeleton.Joints[JointType.Head]);
            Point lowerBack = jointToPoint(skeleton.Joints[JointType.Spine]);

            Point shoulderLeft = jointToPoint(skeleton.Joints[JointType.ShoulderLeft]);
            Point shoulderRight = jointToPoint(skeleton.Joints[JointType.ShoulderRight]);
            Point elbowLeft = jointToPoint(skeleton.Joints[JointType.ElbowLeft]);
            Point elbowRight = jointToPoint(skeleton.Joints[JointType.ElbowRight]);
            Point handLeft = jointToPoint(skeleton.Joints[JointType.HandLeft]);
            Point handRight = jointToPoint(skeleton.Joints[JointType.HandRight]);

            Point hipLeft = jointToPoint(skeleton.Joints[JointType.HipLeft]);
            Point hipRight = jointToPoint(skeleton.Joints[JointType.HipRight]);
            Point kneeLeft = jointToPoint(skeleton.Joints[JointType.KneeLeft]);
            Point kneeRight = jointToPoint(skeleton.Joints[JointType.KneeRight]);
            Point footLeft = jointToPoint(skeleton.Joints[JointType.AnkleLeft]);
            Point footRight = jointToPoint(skeleton.Joints[JointType.AnkleRight]);

            //Console.WriteLine(hipLeft.X);

            // aesthetic adjustments
            lowerBack.Y += 30;
            hipLeft.Y += 40;
            hipRight.Y += 40;

            Point shoulderCenter = new Point((shoulderLeft.X + shoulderRight.X) / 2, (shoulderLeft.Y + shoulderRight.Y) / 2);
            Point hipCenter = new Point((hipLeft.X + hipRight.X) / 2, (hipLeft.Y + hipRight.Y) / 2);

            //  TODO: switch to a better height metric later
            float height = 400;

            int shoulderWidth = (int)distanceBetweenPoints(shoulderLeft, shoulderRight);

            drawBetween(shoulderCenter, lowerBack, shoulderWidth, sprites[characterIndex][1], 0.3f);
            drawBetween(lowerBack, hipCenter, shoulderWidth, sprites[characterIndex][2], 0.25f);
            drawBetween(headTop, shoulderCenter, shoulderWidth / 2, sprites[characterIndex][0], 0.35f);

            drawBetween(shoulderLeft, elbowLeft, (int)(widthHeightRatio[3] * height), sprites[characterIndex][3], 0.2f);
            drawBetween(elbowLeft, handLeft, (int)(widthHeightRatio[4] * height), sprites[characterIndex][4], 0.3f);
            drawAt(handLeft, sprites[characterIndex][5], 0.4f, getAngle(elbowLeft, handLeft), height / 10);

            drawBetween(shoulderRight, elbowRight, (int)(widthHeightRatio[6] * height), sprites[characterIndex][6], 0.2f);
            drawBetween(elbowRight, handRight, (int)(widthHeightRatio[7] * height), sprites[characterIndex][7], 0.3f);
            drawAt(handRight, sprites[characterIndex][8], 0.4f, getAngle(elbowRight, handRight), height / 10);

            drawBetween(hipLeft, kneeLeft, (int)(widthHeightRatio[9] * height), sprites[characterIndex][9], 0.2f);
            drawBetween(kneeLeft, footLeft, (int)(widthHeightRatio[10] * height), sprites[characterIndex][10], 0.3f);
            drawAt(footLeft, sprites[characterIndex][11], 0.4f, getAngle(kneeLeft, footLeft), height / 10);

            drawBetween(hipRight, kneeRight, (int)(widthHeightRatio[6] * height), sprites[characterIndex][12], 0.2f);
            drawBetween(kneeRight, footRight, (int)(widthHeightRatio[7] * height), sprites[characterIndex][13], 0.3f);
            drawAt(footRight, sprites[characterIndex][11], 0.4f, getAngle(kneeRight, footRight), height / 10);
        }

        private float getAngle(Point p1, Point p2)
        {
            float rotation = (float)Math.Acos(Vector2.Dot(Vector2.Normalize(new Vector2(p2.X - p1.X, p2.Y - p1.Y)), new Vector2(0, 1)));

            if (p1.X < p2.X) rotation *= -1;

            return rotation;
        }

        private void drawBetween(Point p1, Point p2, int width, Texture2D sprite, float layer)
        {
            //  height of image is distance between pts
            int height = (int)distanceBetweenPoints(p1, p2);

            float rotation = getAngle(p1, p2);

            //  used to offset drawn segments so they go overlap 
            int offsetX = sprite.Width / 2;
            int offsetY = sprite.Width / 4;

            spriteBatch.Draw(
                sprite,
                new Rectangle(p1.X, p1.Y, width, height + sprite.Height / 4),
                new Rectangle(0, 0, sprite.Width, sprite.Height),
                Color.White,
                rotation,
                new Vector2(sprite.Width / 2, sprite.Height / 8),
                SpriteEffects.None,
                layer);
        }

        private void drawAt(Point point, Texture2D sprite, float layer, float rotation, float width)
        {
            int w = (int)width;// sprite.Width;
            int h = sprite.Height * (int)width / sprite.Width;
            spriteBatch.Draw(
                sprite,
                new Rectangle(point.X, point.Y, w, h),
                new Rectangle(0, 0, sprite.Width, sprite.Height),
                Color.White,
                rotation,
                new Vector2(sprite.Width / 2, sprite.Height / 2),
                SpriteEffects.None,
                layer);
        }

        private double distanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private Point jointToPoint(SkeletonPoint jointPosition)//Joint joint)
        {
            //ColorImagePoint cp = sensor.MapSkeletonPointToColor(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
            ColorImagePoint cp = sensor.MapSkeletonPointToColor(jointPosition, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(cp.X * game.GraphicsDevice.Viewport.Width / 640, cp.Y * game.GraphicsDevice.Viewport.Height / 480);
        }
    }
}
