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
            //Rooms.Add(new Room(graphicsDevice, assetsFolder, textures, new Tile[13, 23]));

            ArrayList<FloorNode> floorNodes = new ArrayList<FloorNode>();

            floorNodes.Add(new FloorNode(new Vector2(200, 900)));
            floorNodes.Add(new FloorNode(new Vector2(400, 950)));
            floorNodes.Add(new FloorNode(new Vector2(800, 800)));
            floorNodes.Add(new FloorNode(new Vector2(1200, 800)));
            floorNodes.Add(new FloorNode(new Vector2(1600, 200)));
            floorNodes.Add(new FloorNode(new Vector2(1800, 1200)));
            floorNodes.Add(new FloorNode(new Vector2(2100, 1200)));
            floorNodes.Add(new FloorNode(new Vector2(2500, 1000)));

            FloorGraph floor = new FloorGraph(floorNodes);
            Rooms.Add(new Room(graphicsDevice, assetsFolder, textures, floor));

            CurrentRoom = Rooms.Items[0];
        }
    }
}
