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

        public Tile(){ }

        public Tile(TILE_TYPE tileType, int row, int column, Texture2D sprite)
        {
            TileType = tileType;
            Row = row;
            Column = column;
            Sprite = sprite;
            Position = new Vector2(Column * Constants.TILE_SIZE, Row * Constants.TILE_SIZE);
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Constants.TILE_SIZE, Constants.TILE_SIZE);

            // Really we could have arbitrary slopes if we pass the StartPoint and EndPoint values into the constructor
            switch (TileType)
            {
                case TILE_TYPE.BLOCK:
                    StartPoint = Position;
                    EndPoint = new Vector2(Bounds.Right, Bounds.Top);
                    break;
                case TILE_TYPE.R_RAMP:
                    StartPoint = new Vector2(Bounds.Left, Bounds.Bottom);
                    EndPoint = new Vector2(Bounds.Right, Bounds.Top);
                    break;
                case TILE_TYPE.L_RAMP:
                    StartPoint = new Vector2(Bounds.Left, Bounds.Top);
                    EndPoint = new Vector2(Bounds.Right, Bounds.Bottom);
                    break;
            }
        }
        /// <summary>
        /// The start and end multipliers are float values from 0 to 1. This value is multiplied by Bounds.Top to determine start and end ramp heights.
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="startMultiplier"></param>
        /// <param name="endMultiplier"></param>
        /// <param name="sprite"></param>
        public Tile(TILE_TYPE tileType, int row, int column, float startMultiplier, float endMultiplier, Texture2D sprite)
        {
            TileType = tileType;
            Row = row;
            Column = column;
            Sprite = sprite;
            Position = new Vector2(Column * Constants.TILE_SIZE, Row * Constants.TILE_SIZE);
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Constants.TILE_SIZE, Constants.TILE_SIZE);

            StartPoint = new Vector2(Bounds.Left, Bounds.Bottom - (Constants.TILE_SIZE * startMultiplier));
            EndPoint = new Vector2(Bounds.Right, Bounds.Bottom - (Constants.TILE_SIZE * endMultiplier));
        }

        /// <summary>
        /// This constructor to be utilized when creating/initializing Tiles indirectly with call to TileMap.InsertTile(). This is the preferred way to create Tiles.
        /// The start and end multipliers are float values from 0 to 1. This value is multiplied by Bounds.Top to determine start and end ramp heights.
        /// </summary>
        public Tile(int row, int column, float startMultiplier, float endMultiplier, Texture2D sprite)
        {
            Row = row;
            Column = column;
            Sprite = sprite;
            Position = new Vector2((Column * Constants.TILE_SIZE), (Row * Constants.TILE_SIZE));
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Constants.TILE_SIZE, Constants.TILE_SIZE);
            StartPoint = new Vector2(Bounds.Left, Bounds.Bottom - (Constants.TILE_SIZE * startMultiplier));
            EndPoint = new Vector2(Bounds.Right, Bounds.Bottom - (Constants.TILE_SIZE * endMultiplier));

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
