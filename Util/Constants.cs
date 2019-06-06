using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    static class CONSTANTS
    {
        public const int TILE_SIZE = 128;
        public static Texture2D PIXEL_TEX;
        public static GraphicsDevice GRAPHICS_DEVICE;

        /// <summary>
        /// Returns the Vector2 on a line between two Vector2's given an X value betwen them.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="xVal"></param>
        /// <returns></returns>
        public static Vector2 Y_INTERSECTION(Vector2 v1, Vector2 v2, float xVal)
        {
            Vector2 intersection;
            intersection = new Vector2((int)xVal, (int)((v2.Y - v1.Y) * ((xVal - v1.X) / (v2.X - v1.X)) + v1.Y));

            return intersection;
        }

        /// <summary>
        /// Returns a Vector2 on a line between two given Vector2's a certain distance from the start Vector2 (v1)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Vector2 VECTOR_DISTANCE(Vector2 v1, Vector2 v2, float dist)
        {
            float totalDist = Vector2.Distance(v1, v2);
            float newX = v1.X - ((dist * (v1.X - v2.X)) / totalDist);
            float newY = v1.Y - ((dist * (v1.Y - v2.Y)) / totalDist);
            Vector2 intersection = new Vector2(newX, newY);

            return intersection;
        }

        public static float GET_ROTATION(Vector2 v1, Vector2 v2)
        {
            float rotation = 0.0f;
            float adj = v1.X - v2.X;
            float opp = v1.Y - v2.Y;
            float tan = opp / adj;
            rotation = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            rotation = (rotation - 180) % 360;
            if (rotation < 0) { rotation += 360; }
            rotation = MathHelper.ToRadians(rotation);
            return rotation;
        }
    }
}
