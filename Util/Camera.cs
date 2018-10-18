using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    class Camera
    {
        protected Matrix inverseTransform;
        protected Viewport viewport;
        protected int scroll;
        public float Zoom { get; set; }
        public float Scale { get; set; }
        /// <summary>
        /// Camera View Matrix Property
        /// </summary>
        public Matrix Transform { get; set; }
        /// <summary>
        /// Inverse of the view matrix, can be used to get objects screen coordinates
        /// from its object coordinates
        /// </summary>
        public Matrix InverseTransform
        {
            get { return inverseTransform; }
        }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
            Scale = (float)viewport.Width / 1920;
            Zoom = 0.66f * Scale;
            scroll = 1;
            Rotation = 0.0f;
            Position = Vector2.Zero;
        }
        /// <summary>
        /// Update the camera view
        /// </summary>
        public void Update()
        {
            //Clamp zoom value
            Zoom = MathHelper.Clamp(Zoom, 0.0f, 10.0f);
            //Clamp rotation value
            Rotation = ClampAngle(Rotation);
            //Create view matrix
            Transform = Matrix.CreateRotationZ(Rotation) *
                            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                            Matrix.CreateTranslation(Position.X, Position.Y, 0);
            //Update inverse matrix
            inverseTransform = Matrix.Invert(Transform);
        }

        /// <summary>
        /// Clamps a radian value between -pi and pi
        /// </summary>
        /// <param name="radians">angle to be clamped</param>
        /// <returns>clamped angle</returns>
        protected float ClampAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
    }
}
