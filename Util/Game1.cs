using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine;

namespace ProjectVK
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SkeletonRenderer skeletonRenderer;
        SpriteBatch spriteBatch;
        Texture2D pixelTex, r45RampTex, l45RampTex, r22ARampTex, r22BRampTex, l22ARampTex, l22BRampTex;
        ArrayList<Texture2D> textures;
        Level level;
        Camera camera;

        private string assetsFolder = "SpineAnimations/";


        public Game1()
        {
            IsFixedTimeStep = false;

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;

            //graphics.PreferredBackBufferWidth = 1024;
            //graphics.PreferredBackBufferHeight = 768;

            //graphics.PreferredBackBufferWidth = 1366;
            //graphics.PreferredBackBufferHeight = 768;

            //graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            CONSTANTS.GRAPHICS_DEVICE = graphics.GraphicsDevice;
            CONSTANTS.PIXEL_TEX = new Texture2D(CONSTANTS.GRAPHICS_DEVICE, 1, 1);
            CONSTANTS.PIXEL_TEX.SetData(new Color[] { Color.White });

            pixelTex = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixelTex.SetData(new Color[] { Color.White });

            r45RampTex = Content.Load<Texture2D>("gfx/prototype/r_45_ramp");
            l45RampTex = Content.Load<Texture2D>("gfx/prototype/l_45_ramp");

            r22ARampTex = Content.Load<Texture2D>("gfx/prototype/r_22_5_a_ramp");
            r22BRampTex = Content.Load<Texture2D>("gfx/prototype/r_22_5_B_ramp");
            l22ARampTex = Content.Load<Texture2D>("gfx/prototype/l_22_5_a_ramp");
            l22BRampTex = Content.Load<Texture2D>("gfx/prototype/l_22_5_B_ramp");


            textures = new ArrayList<Texture2D>();
            textures.Add(pixelTex);
            textures.Add(r45RampTex);
            textures.Add(l45RampTex);
            textures.Add(r22ARampTex);
            textures.Add(r22BRampTex);
            textures.Add(l22ARampTex);
            textures.Add(l22BRampTex);

            camera = new Camera(GraphicsDevice.Viewport);

            skeletonRenderer = new SkeletonRenderer(GraphicsDevice);
            skeletonRenderer.PremultipliedAlpha = false;

            level = new Level(GraphicsDevice, assetsFolder, textures);
            level.CurrentRoom = level.Rooms.Items[0];
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            camera.Update();
            level.CurrentRoom.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumVioletRed);

            ((BasicEffect)skeletonRenderer.Effect).Projection = camera.Transform * Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 1, 0);

            // Draw all objects requiring the skeletonRenderer
            skeletonRenderer.Begin();
            level.CurrentRoom.DrawCharacters(skeletonRenderer, gameTime);
            skeletonRenderer.End();

            // Draw all objects requiring the spriteBatch
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, camera.Transform);
            level.CurrentRoom.DrawObjects(spriteBatch);
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
