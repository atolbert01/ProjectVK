using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    public enum TILE_TYPE
    {
        BLOCK,
        R_RAMP,
        L_RAMP,
        R_45_CEILING,
        L_45_CEILING
    }
    class Tile : GameObject
    {
        public Rectangle Bounds { get; set; }
        public Texture2D Sprite { get; set; }
        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public TILE_TYPE TileType { get; set; }
        public int TopDepth { get; set; }
        public int BottomDepth { get; set; }
        public int LeftDepth { get; set; }
        public int RightDepth { get; set; }
        public int SlopeDir { get; set; }
        public bool BlockLeftApproach { get; set; }
        public bool BlockRightApproach { get; set; }
        public Tile(){ }

        /// <summary>
        /// This constructor to be utilized when creating/initializing Tiles indirectly with call to TileMap.InsertTile(). This is the preferred way to create Tiles.
        /// The start and end multipliers are float values from 0 to 1. This value is multiplied by Bounds.Top to determine start and end ramp heights.
        /// </summary>
        public Tile(int row, int column, float startMultiplier, float endMultiplier, Texture2D sprite)
        {
            Row = row;
            Column = column;
            Sprite = sprite;
            Position = new Vector2((Column * CONSTANTS.TILE_SIZE), (Row * CONSTANTS.TILE_SIZE));
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, CONSTANTS.TILE_SIZE, CONSTANTS.TILE_SIZE);
            StartPoint = new Vector2(Bounds.Left, Bounds.Bottom - (CONSTANTS.TILE_SIZE * startMultiplier));
            EndPoint = new Vector2(Bounds.Right, Bounds.Bottom - (CONSTANTS.TILE_SIZE * endMultiplier));

            if (StartPoint.Y - EndPoint.Y < 0)
            {
                SlopeDir = -1;
                BlockLeftApproach = false;
                BlockRightApproach = true;
            }
            else if (0 < StartPoint.Y - EndPoint.Y)
            {
                SlopeDir = 1;
                BlockLeftApproach = true;
                BlockRightApproach = false;
            }
            else
            {
                SlopeDir = 0;
                BlockLeftApproach = true;
                BlockRightApproach = true;
            }
        }

        public float GetYIntersection(float xPos)
        {
            float height = 0.0f;
            height = (EndPoint.Y - StartPoint.Y) * ((xPos - StartPoint.X) / (EndPoint.X - StartPoint.X)) + StartPoint.Y;
            return height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Bounds, Color.Black);
        }
    }
}
