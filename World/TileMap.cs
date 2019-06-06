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
            Tile leftTile = new Tile();
            Tile rightTile = new Tile();

            Tile tile = new Tile(y, x, startMultiplier, endMultiplier, sprite);
            tile.TopDepth = 0;
            tile.BottomDepth = 0;
            tile.LeftDepth = 0;
            tile.RightDepth = 0;
 
            Map[y, x] = tile;

            if (y + 1 < Map.GetLength(0) && Map[y + 1, x] != null)
                botTile = FindBottom(x, y);
            else
                botTile = tile;

            if (0 <= y - 1 && Map[y - 1, x] != null)
                topTile = FindTop(x, y);
            else
                topTile = tile;

            if (x + 1 < Map.GetLength(1) && Map[y, x + 1] != null)
                rightTile = FindRight(x, y);
            else
                rightTile = tile;

            if (0 <= x - 1 && Map[y, x - 1] != null)
                leftTile = FindLeft(x, y);
            else
                leftTile = tile;

            int depthRange = botTile.Row - topTile.Row;
            int sideRange = rightTile.Column - leftTile.Column;

            for(int i = 0; i <= depthRange; i++)
            {
                Map[topTile.Row + i, topTile.Column].TopDepth = -i;
                Map[topTile.Row + i, topTile.Column].BottomDepth = depthRange - i;
            }

            for (int i = 0; i <= sideRange; i++)
            {
                Map[leftTile.Row, leftTile.Column + i].LeftDepth = -i;
                Map[leftTile.Row, leftTile.Column + i].RightDepth = sideRange - i;
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

        public Tile FindLeft(int x, int y)
        {
            if (-1 < x && Map[y, x] != null)
                return FindLeft(x - 1, y);
            else
                return Map[y, x + 1];
        }

        public Tile FindRight(int x, int y)
        {
            if (x < Map.GetLength(1) && Map[y, x] != null)
                return FindRight(x + 1, y);
            else
                return Map[y, x - 1];
        }

        public Tile GetTile(Point address)
        {
            Point clampedAddress = GetClampedPoint(address.Y, address.X);
            return Map[clampedAddress.Y, clampedAddress.X];
        }

        public Point GetClampedPoint(int y, int x)
        {
            int column = MathHelper.Clamp((int)Math.Ceiling(((float)x / CONSTANTS.TILE_SIZE) - 1), 0, Map.GetLength(1) - 1);
            int row = MathHelper.Clamp((int)Math.Ceiling(((float)y / CONSTANTS.TILE_SIZE) - 1), 0, Map.GetLength(0) - 1);

            return new Point(column, row);
        }

        public Tile GetPreClampedTile(int y, int x)
        {
            return Map[MathHelper.Clamp(y, 0, Map.GetLength(0) - 1), MathHelper.Clamp(x, 0, Map.GetLength(1) - 1)];
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

        public override string ToString()
        {
            string output = "";

            for (int row = 0; row < Map.GetLength(0); row++)
            {
                for (int column = 0; column < Map.GetLength(1); column++)
                {
                    if (Map[row, column] != null)
                    {
                        string nextTile = "[" + (Map[row, column].BlockRightApproach ? "T " : "F ") + Map[row, column].LeftDepth.ToString("00") + ", " + Map[row, column].TopDepth.ToString("00") + ", " + Map[row, column].BottomDepth.ToString("00") + ", " + Map[row, column].RightDepth.ToString("00") + " " +(Map[row, column].BlockLeftApproach ? "T" : "F");
                        while (nextTile.Length < 17) nextTile = nextTile.Insert(nextTile.Length, " ");
                        nextTile += "]";
                        output += nextTile;
                    }
                    else
                    {
                        output += "[                    ]";
                    }
                }
                output += "\n";
            }
            return output;
        }
    }
}
