using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    class Level
    {
        public ArrayList<Room> Rooms { get; set; }
        public Room CurrentRoom { get; set; }
        public Level(GraphicsDevice graphicsDevice, string assetsFolder, ArrayList<Texture2D> textures)
        {
            Rooms = new ArrayList<Room>();
            Rooms.Add(new Room(graphicsDevice, assetsFolder, textures, new Tile[13, 23]));
            CurrentRoom = Rooms.Items[0];
        }
    }
}
