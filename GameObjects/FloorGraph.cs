using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace ProjectVK
{
    class FloorGraph
    {
        public ArrayList<FloorNode> Nodes { get; set; }
        private KeyboardState currentKeyState { get; set; }
        /// <summary>
        /// Represents the graph of floor nodes. Nodes will always be ordered
        /// from left to right, starting at index zero for the leftmost node.
        /// </summary>
        /// <param name="nodes"></param>
        public FloorGraph(ArrayList<FloorNode> nodes)
        {
            for (int i = 0; i < nodes.Items.Length; i++)
            {
                if (i - 1 != -1)
                {
                    nodes.Items[i].LeftNeighbor = nodes.Items[i - 1];
                }

                if (i + 1 != nodes.Items.Length)
                {
                    nodes.Items[i].RightNeighbor = nodes.Items[i + 1];
                }
            }
            nodes.Items[0].LeftNeighbor = nodes.Items[0];
            nodes.Items[nodes.Items.Length - 1].RightNeighbor = nodes.Items[nodes.Items.Length - 1];
            Nodes = nodes;
        }

        public void Update(GameTime gameTime)
        {
            currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Keys.A))
            {
                foreach (FloorNode node in Nodes.Items)
                {
                    node.Position += new Vector2(-4, 0);
                }
            }

            if (currentKeyState.IsKeyDown(Keys.D))
            {
                foreach (FloorNode node in Nodes.Items)
                {
                    node.Position += new Vector2(4, 0);
                }
            }
            foreach (FloorNode node in Nodes.Items)
            {
                node.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (FloorNode node in Nodes.Items)
            {
                node.Draw(spriteBatch);
            }
        }
    }
}
