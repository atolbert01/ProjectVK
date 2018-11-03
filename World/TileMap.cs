using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    class TileMap
    {
        public Tile[,] Map { get; set; }
        public TileMap(Tile[,] tiles)
        {
            Map = tiles;
        }

        public void InsertTile(int x, int y, float startMultiplier, float endMultiplier, Texture2D sprite)
        {
            Tile botTile = new Tile();
            Tile topTile = new Tile();
            Tile tile = new Tile(y, x, startMultiplier, endMultiplier, sprite);
            tile.TopDepth = 0;
            tile.BottomDepth = 0;
 
            Map[y, x] = tile;

            if (y + 1 < Map.GetLength(0) && Map[y + 1, x] != null)
                botTile = FindBottom(x, y);
            else
                botTile = tile;

            if (0 <= y - 1 && Map[y - 1, x] != null)
                topTile = FindTop(x, y);
            else
                topTile = tile;

            int depthRange = botTile.Row - topTile.Row;

            for(int i = 0; i <= depthRange; i++)
            {
                Map[topTile.Row + i, topTile.Column].TopDepth = -i;
                Map[topTile.Row + i, topTile.Column].BottomDepth = depthRange - i;
            }

            Map[y, x] = tile;
        }

        public Tile FindBottom(int x, int y)
        {
            if (y < Map.GetLength(0) && Map[y, x] != null)
                return FindBottom(x, y + 1);
            else
                return Map[y - 1, x];
        }

        public Tile FindTop(int x, int y)
        {
            if (-1 < y && Map[y, x] != null)
                return FindTop(x, y - 1);
            else
                return Map[y + 1, x];
        }

        public Point GetClampedPoint(int y, int x)
        {
            int row = MathHelper.Clamp((int)Math.Ceiling(((float)x / Constants.TILE_SIZE) - 1), 0, Map.GetLength(1) - 1);
            int column = MathHelper.Clamp((int)Math.Ceiling(((float)y / Constants.TILE_SIZE) - 1), 0, Map.GetLength(0) - 1);

            return new Point(row, column);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Map.GetLength(0); row++)
            {
                for (int column = 0; column < Map.GetLength(1); column++)
                {
                    if (Map[row, column] != null)
                    {
                        Map[row, column].Draw(spriteBatch);
                    }
                }
            }
        }
    }
}
