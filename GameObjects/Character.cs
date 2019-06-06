using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;

namespace ProjectVK
{
    class Character : GameObject
    {
        public AnimationState AnimState { get; set; }
        public AnimationStateData AnimStateData { get; set; }
        public Skeleton Skeleton { get; set; }
        public float ElapsedTime { get; set; }
        private Vector2 position;
        private float xRemainder = 0.0f;
        private float yRemainder = 0.0f;
        public new Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                PrevPosition = Position;
                position = value;
                //int newX = (int)Math.Floor(value.X + xRemainder);
                //int newY = (int)Math.Floor(value.Y + yRemainder);

                //xRemainder = value.X - newX;// Math.Abs(newX - value.X);
                //yRemainder = value.Y - newY;// Math.Abs(newY - value.Y);

                //position = new Vector2(newX, newY);
                //position = new Vector2((int)value.X, (int)value.Y);
                position += Vector2.Zero * ElapsedTime;
                if (Skeleton != null)
                {
                    Skeleton.X = position.X;
                    Skeleton.Y = position.Y;
                }
            }
        }
        public Vector2 PrevPosition { get; set; }

        /// <summary>
        /// The Character class is a subclass of GameObject. Character objects have a Skeleton, AnimationState, and SkeletonBounds. Handles logic for updating and drawing the skeletal animation.
        /// </summary>
        /// <param name="startPos"></param>
        public Character(Vector2 startPos) : base(startPos)
        {
            Position = startPos;
        }

        public virtual void Load(Atlas atlas, string animName)
        {
            SkeletonData skeletonData;

            SkeletonJson json = new SkeletonJson(atlas);
            json.Scale = 1.0f;
            skeletonData = json.ReadSkeletonData(animName + ".json");
            Skeleton = new Skeleton(skeletonData);

            AnimStateData = new AnimationStateData(Skeleton.Data);
            AnimState = new AnimationState(AnimStateData);

            Skeleton.X = Position.X;
            Skeleton.Y = Position.Y;

            Skeleton.UpdateWorldTransform();
        }

        public virtual void Draw(GameTime gameTime)
        {
            AnimState.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            AnimState.Apply(Skeleton);
            Skeleton.UpdateWorldTransform();
        }
    }
}
