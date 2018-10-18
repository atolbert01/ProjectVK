using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;

namespace ProjectVK
{
    class Room
    {
        public ArrayList<Tile> Tiles { get; set; }
        public ArrayList<Texture2D> Textures { get; set; }
        public Tile[,] TileMap { get; set; }
        public ArrayList<Character> Characters { get; set; }
        public Player Player { get; set; }
        public Room(GraphicsDevice graphicsDevice, string assetsFolder, ArrayList<Texture2D> textures, Tile[ , ] tileMap)
        {
            Textures = textures;
            TileMap = tileMap;
            Characters = new ArrayList<Character>();


            TileMap[0, 0] = new Tile(TILE_TYPE.BLOCK, 0, 0, Textures.Items[0]);
            TileMap[1, 0] = new Tile(TILE_TYPE.BLOCK, 1, 0, Textures.Items[0]);
            TileMap[2, 0] = new Tile(TILE_TYPE.BLOCK, 2, 0, Textures.Items[0]);
            TileMap[3, 0] = new Tile(TILE_TYPE.BLOCK, 3, 0, Textures.Items[0]);
            TileMap[4, 0] = new Tile(TILE_TYPE.BLOCK, 4, 0, Textures.Items[0]);
            TileMap[5, 0] = new Tile(TILE_TYPE.BLOCK, 5, 0, Textures.Items[0]);
            TileMap[6, 0] = new Tile(TILE_TYPE.BLOCK, 6, 0, Textures.Items[0]);
            TileMap[7, 0] = new Tile(TILE_TYPE.BLOCK, 7, 0, Textures.Items[0]);
            TileMap[8, 0] = new Tile(TILE_TYPE.BLOCK, 8, 0, Textures.Items[0]);
            TileMap[9, 0] = new Tile(TILE_TYPE.BLOCK, 9, 0, Textures.Items[0]);
            TileMap[10, 0] = new Tile(TILE_TYPE.BLOCK, 10, 0, Textures.Items[0]);
            TileMap[11, 0] = new Tile(TILE_TYPE.BLOCK, 11, 0, Textures.Items[0]);
            TileMap[12, 0] = new Tile(TILE_TYPE.BLOCK, 12, 0, Textures.Items[0]);

            TileMap[0, 4] = new Tile(TILE_TYPE.BLOCK, 0, 4, Textures.Items[0]);
            TileMap[1, 4] = new Tile(TILE_TYPE.BLOCK, 1, 4, Textures.Items[0]);
            TileMap[2, 4] = new Tile(TILE_TYPE.BLOCK, 2, 4, Textures.Items[0]);
            TileMap[3, 4] = new Tile(TILE_TYPE.BLOCK, 3, 4, Textures.Items[0]);
            TileMap[4, 4] = new Tile(TILE_TYPE.BLOCK, 4, 4, Textures.Items[0]);
            TileMap[5, 4] = new Tile(TILE_TYPE.BLOCK, 5, 4, Textures.Items[0]);
            TileMap[6, 4] = new Tile(TILE_TYPE.BLOCK, 6, 4, Textures.Items[0]);

            TileMap[12, 1] = new Tile(TILE_TYPE.BLOCK, 12, 1, Textures.Items[0]);
            TileMap[12, 2] = new Tile(TILE_TYPE.BLOCK, 12, 2, Textures.Items[0]);
            TileMap[12, 3] = new Tile(TILE_TYPE.BLOCK, 12, 3, Textures.Items[0]);
            TileMap[12, 4] = new Tile(TILE_TYPE.BLOCK, 12, 4, Textures.Items[0]);
            TileMap[12, 5] = new Tile(TILE_TYPE.BLOCK, 12, 5, Textures.Items[0]);
            TileMap[12, 6] = new Tile(TILE_TYPE.BLOCK, 12, 6, Textures.Items[0]);
            TileMap[12, 7] = new Tile(TILE_TYPE.BLOCK, 12, 7, Textures.Items[0]);
            TileMap[12, 8] = new Tile(TILE_TYPE.BLOCK, 12, 8, Textures.Items[0]);
            TileMap[12, 9] = new Tile(TILE_TYPE.BLOCK, 12, 9, Textures.Items[0]);
            TileMap[12, 10] = new Tile(TILE_TYPE.BLOCK, 12, 10, Textures.Items[0]);
            TileMap[12, 11] = new Tile(TILE_TYPE.BLOCK, 12, 11, Textures.Items[0]);
            TileMap[12, 12] = new Tile(TILE_TYPE.BLOCK, 12, 12, Textures.Items[0]);
            TileMap[12, 13] = new Tile(TILE_TYPE.BLOCK, 12, 13, Textures.Items[0]);
            TileMap[12, 14] = new Tile(TILE_TYPE.BLOCK, 12, 14, Textures.Items[0]);
            TileMap[12, 15] = new Tile(TILE_TYPE.BLOCK, 12, 15, Textures.Items[0]);
            TileMap[12, 16] = new Tile(TILE_TYPE.BLOCK, 12, 16, Textures.Items[0]);
            TileMap[12, 17] = new Tile(TILE_TYPE.BLOCK, 12, 17, Textures.Items[0]);
            TileMap[12, 18] = new Tile(TILE_TYPE.BLOCK, 12, 18, Textures.Items[0]);
            TileMap[12, 19] = new Tile(TILE_TYPE.BLOCK, 12, 19, Textures.Items[0]);
            TileMap[12, 20] = new Tile(TILE_TYPE.BLOCK, 12, 20, Textures.Items[0]);
            TileMap[12, 21] = new Tile(TILE_TYPE.BLOCK, 12, 21, Textures.Items[0]);
            TileMap[12, 22] = new Tile(TILE_TYPE.BLOCK, 12, 22, Textures.Items[0]);


            TileMap[11, 3] = new Tile(TILE_TYPE.R_RAMP, 11, 3, 0.5f, 1.0f, Textures.Items[4]);
            TileMap[11, 4] = new Tile(TILE_TYPE.R_RAMP, 11, 4, 0.0f, 0.5f, Textures.Items[3]);

            //TileMap[11, 3] = new Tile(TILE_TYPE.R_RAMP, 11, 3, 0.0f, 0.5f, Textures.Items[3]);
            //TileMap[11, 4] = new Tile(TILE_TYPE.R_RAMP, 11, 4, 0.5f, 1.0f, Textures.Items[4]);

            TileMap[11, 6] = new Tile(TILE_TYPE.BLOCK, 11, 6, Textures.Items[0]);

            // Air ramp test
            TileMap[7, 7] = new Tile(TILE_TYPE.BLOCK, 7, 7, Textures.Items[0]);
            TileMap[6, 8] = new Tile(TILE_TYPE.BLOCK, 6, 8, Textures.Items[0]);
            TileMap[6, 9] = new Tile(TILE_TYPE.BLOCK, 6, 9, Textures.Items[0]);
            TileMap[6, 10] = new Tile(TILE_TYPE.BLOCK, 6, 10, Textures.Items[0]);
            TileMap[5, 11] = new Tile(TILE_TYPE.R_RAMP, 5, 11, Textures.Items[1]);
            TileMap[4, 12] = new Tile(TILE_TYPE.R_RAMP, 4, 12, 0.0f, 0.5f, Textures.Items[3]);
            TileMap[4, 13] = new Tile(TILE_TYPE.R_RAMP, 4, 13, 0.5f, 1.0f, Textures.Items[4]);
            TileMap[3, 14] = new Tile(TILE_TYPE.R_RAMP, 3, 14, 0.0f, 0.5f, Textures.Items[3]);
            TileMap[3, 15] = new Tile(TILE_TYPE.R_RAMP, 3, 15, 0.5f, 1.0f, Textures.Items[4]);


            TileMap[11, 8] = new Tile(TILE_TYPE.L_RAMP, 11, 8, Textures.Items[2]);
            TileMap[11, 9] = new Tile(TILE_TYPE.R_RAMP, 11, 9, Textures.Items[1]);
            TileMap[11, 10] = new Tile(TILE_TYPE.BLOCK, 11, 10, Textures.Items[0]);
            TileMap[11, 11] = new Tile(TILE_TYPE.BLOCK, 11, 11, Textures.Items[0]);
            TileMap[11, 12] = new Tile(TILE_TYPE.BLOCK, 11, 12, Textures.Items[0]);
            TileMap[11, 13] = new Tile(TILE_TYPE.BLOCK, 11, 13, Textures.Items[0]);
            TileMap[11, 14] = new Tile(TILE_TYPE.L_RAMP, 11, 14, Textures.Items[2]);
            TileMap[11, 15] = new Tile(TILE_TYPE.R_RAMP, 11, 15, Textures.Items[1]);

            //TileMap[10, 16] = new Tile(TILE_TYPE.BLOCK, 10, 16, Textures.Items[0]);
            TileMap[10, 17] = new Tile(TILE_TYPE.L_RAMP, 10, 17, 1.0f, 0.5f, Textures.Items[6]);
            TileMap[10, 18] = new Tile(TILE_TYPE.L_RAMP, 10, 18, 0.5f, 0.0f, Textures.Items[5]);
            TileMap[11, 16] = new Tile(TILE_TYPE.BLOCK, 11, 16, Textures.Items[0]);
            TileMap[11, 17] = new Tile(TILE_TYPE.BLOCK, 11, 17, Textures.Items[0]);

            TileMap[11, 18] = new Tile(TILE_TYPE.BLOCK, 11, 18, Textures.Items[0]);
            TileMap[11, 19] = new Tile(TILE_TYPE.L_RAMP, 11, 19, Textures.Items[2]);

            TileMap[11, 21] = new Tile(TILE_TYPE.L_RAMP, 11, 21, 1.0f, 0.5f, Textures.Items[6]);
            TileMap[11, 20] = new Tile(TILE_TYPE.R_RAMP, 11, 20, 0.5f, 1.0f, Textures.Items[4]);
            //TileMap[11, 20] = new Tile(TILE_TYPE.L_RAMP, 11, 20, 0.5f, 0.0f, Textures.Items[5]);

            TileMap[10, 10] = new Tile(TILE_TYPE.R_RAMP, 10, 10, Textures.Items[1]);
            TileMap[10, 11] = new Tile(TILE_TYPE.BLOCK, 10, 11, Textures.Items[0]);
            TileMap[10, 12] = new Tile(TILE_TYPE.L_RAMP, 10, 12, Textures.Items[2]);

            Player = new Player(graphicsDevice, assetsFolder, new Vector2(420, 400), 12.0f, this);
            Characters.Add(Player);
        }

        public Point GetRoomClampedPoint(int y, int x)
        {
            int row = MathHelper.Clamp((int)Math.Ceiling(((float)x / Constants.TILE_SIZE) - 1), 0, TileMap.GetLength(1) - 1);
            int column = MathHelper.Clamp((int)Math.Ceiling(((float)y / Constants.TILE_SIZE) - 1), 0, TileMap.GetLength(0) - 1);

            return new Point(row, column);
        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
        }

        public void DrawCharacters(SkeletonRenderer skeletonRenderer, GameTime gameTime)
        {
            foreach (Character character in Characters.Items)
            {
                character.Draw(gameTime);
                skeletonRenderer.Draw(character.Skeleton);
            }

        }

        public void DrawObjects(SpriteBatch spriteBatch)
        {
            //Player.DrawBounds(spriteBatch, Textures.Items[0]);

            for (int row = 0; row < TileMap.GetLength(0); row++)
            {
                for(int column = 0; column < TileMap.GetLength(1); column++)
                {
                    if (TileMap[row, column] != null)
                    {
                        TileMap[row, column].Draw(spriteBatch);
                    }
                }
            }
        }
    }
}
