using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace ProjectVK
{
    class Character : GameObject
    {
        public AnimationState AnimState { get; set; }
        public AnimationStateData AnimStateData { get; set; }
        public Skeleton Skeleton { get; set; }
        private Vector2 position;
        public new Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if(Skeleton != null)
                {
                    Skeleton.X = position.X;
                    Skeleton.Y = position.Y;
                }
            }
        }

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
