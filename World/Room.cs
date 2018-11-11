using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace ProjectVK
{
    class Room
    {
        //public ArrayList<Tile> Tiles { get; set; }
        public ArrayList<Texture2D> Textures { get; set; }
        //public Tile[,] TileMap { get; set; }
        public TileMap Tiles { get; set; }
        public ArrayList<Character> Characters { get; set; }
        public Player Player { get; set; }
        public Room(GraphicsDevice graphicsDevice, string assetsFolder, ArrayList<Texture2D> textures, Tile[ , ] tileMap)
        {
            Textures = textures;
            //TileMap = tileMap;
            Characters = new ArrayList<Character>();

            Tiles = new TileMap(tileMap);
            // Left Wall
            Tiles.InsertTile(0, 0, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 2, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 1, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 3, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 4, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 5, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 6, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 7, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 8, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 9, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 10, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 11, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(0, 12, 1.0f, 1.0f, Textures.Items[0]);

            // Right Wall
            Tiles.InsertTile(22, 2, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 1, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 0, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 3, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 4, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 5, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 6, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 7, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 8, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 9, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 10, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 11, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 12, 1.0f, 1.0f, Textures.Items[0]);

            // Floor
            Tiles.InsertTile(1, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(2, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(3, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(4, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(5, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(6, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(7, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(8, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(9, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(10, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(11, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(12, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(13, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(14, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(15, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(16, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(17, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(18, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(19, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(20, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(21, 12, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(22, 12, 1.0f, 1.0f, Textures.Items[0]);

            // Ramp tests 1
            //Tiles.InsertTile(2, 11, 0.0f, 0.5f, Textures.Items[3]);
            //Tiles.InsertTile(3, 11, 0.0f, 1.0f, Textures.Items[1]);
            Tiles.InsertTile(3, 11, 1.0f, 0.0f, Textures.Items[2]);

            Tiles.InsertTile(4, 11, 0.0f, 0.5f, Textures.Items[3]);
            Tiles.InsertTile(5, 11, 0.5f, 1.0f, Textures.Items[4]);
            Tiles.InsertTile(6, 10, 0.0f, 1.0f, Textures.Items[1]);
            Tiles.InsertTile(7, 10, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(8, 10, 1.0f, 0.0f, Textures.Items[2]);
            //Tiles.InsertTile(9, 11, 1.0f, 0.5f, Textures.Items[6]);
            //Tiles.InsertTile(10, 11, 0.5f, 0.0f, Textures.Items[5]);

            Tiles.InsertTile(9, 11, 1.0f, 0.0f, Textures.Items[2]);
            Tiles.InsertTile(10, 11, 0.0f, 1.0f, Textures.Items[1]);
            // Ramp tests 2
            Tiles.InsertTile(14, 11, 0.0f, 1.0f, Textures.Items[1]);
            Tiles.InsertTile(15, 10, 0.0f, 0.5f, Textures.Items[3]);
            Tiles.InsertTile(16, 10, 0.5f, 1.0f, Textures.Items[4]);
            Tiles.InsertTile(17, 9, 1.0f, 1.0f, Textures.Items[0]);
            Tiles.InsertTile(18, 10, 1.0f, 0.5f, Textures.Items[6]);
            Tiles.InsertTile(19, 10, 0.5f, 0.0f, Textures.Items[5]);
            Tiles.InsertTile(20, 11, 1.0f, 0.0f, Textures.Items[2]);

            Player = new Player(graphicsDevice, assetsFolder, new Vector2(800, 400), 12.0f, this);
            Characters.Add(Player);
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

            Tiles.Draw(spriteBatch);
        }
    }
}
