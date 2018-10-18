using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectVK
{
    class GameObject
    {
        public Vector2 Position { get; set; }
        public GameObject() { }
        public GameObject(Vector2 startPos)
        {
            Position = startPos;
        }
    }
}
