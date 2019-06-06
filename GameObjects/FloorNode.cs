using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    enum FLOOR_NODE_TYPE
    {
        FLOOR,
        R_LEDGE,
        L_LEDGE,
        WALL,
        CEILING
    }
    class FloorNode
    {
        private Rectangle box;
        public Rectangle Bounds { get { return new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16); } }
        private Line l_line;
        private Line r_line;
        //public Vector2 Position { get; set; }
        public float ElapsedTime { get; set; }
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
                position += Vector2.Zero * ElapsedTime;
            }
        }
        private FloorNode _rightNeighbor;
        public FloorNode RightNeighbor
        {
            get
            {
                return _rightNeighbor;
            }
            set
            {
                _rightNeighbor = value;
                r_line = new Line(Position, RightNeighbor.Position, 2, Color.Chartreuse);
            }
        }
        private FloorNode _leftNeighbor;
        public FloorNode LeftNeighbor
        {
            get
            {
                return _leftNeighbor;
            }
            set
            {
                _leftNeighbor = value;
                l_line = new Line(Position, LeftNeighbor.Position, 2, Color.Chartreuse);
            }
        }
        public FloorNode(Vector2 position)
        {
            Position = position;
            box = new Rectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16);
        }

        public void Update(GameTime gameTime)
        {
            ElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            l_line = new Line(Position, LeftNeighbor.Position, 2, Color.Chartreuse);
            r_line = new Line(Position, RightNeighbor.Position, 2, Color.Chartreuse);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(CONSTANTS.PIXEL_TEX, Bounds, Color.Chartreuse);

            if (LeftNeighbor != null)
            {
                l_line.Draw(spriteBatch);
            }
            if (RightNeighbor != null)
            {
                r_line.Draw(spriteBatch);
            }
        }
    }
}
