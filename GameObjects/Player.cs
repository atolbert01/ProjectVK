using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine;

namespace ProjectVK
{
    enum PLAYER_STATE
    {
        IDLE,
        R_RUN,
        L_RUN,
        JUMP,
        FALLING
    }
    class Player : Character
    {
        public PLAYER_STATE PlayerState { get; set; }
        public Rectangle Bounds { get { return new Rectangle((int)(Position.X) - 55, (int)(Position.Y) - yOffset, 110, 288); } }
        public bool IsGrounded { get; set; }
        public bool IsBlockedLeft { get; set; }
        public bool IsBlockedRight { get; set; }
        public bool IsBlockedTop { get; set; }
        public Room CurrentRoom { get; set; }
        private TileMap Tiles { get; set; }
        private float gravity;
        private readonly int yOffset = 288;
        private int TileSize { get; set; }
        private Vector2 PrevPos { get; set; }
        private Vector2 RunVelocity = new Vector2(12, 0);
        private KeyboardState CurrentKeyState { get; set; }
        private KeyboardState PrevKeyState { get; set; }
        private Point TopMid { get { return Tiles.GetClampedPoint(Bounds.Top, (int)(Bounds.Center.X)); } }
        private Point MidLeft { get { return Tiles.GetClampedPoint((int)(Bounds.Top + (Bounds.Height * 0.5)), (int)(Bounds.Left + 1)); } }
        private Point MidRight { get { return Tiles.GetClampedPoint((int)(Bounds.Top + (Bounds.Height * 0.5)), (int)(Bounds.Right)); } }
        //private Point LoRight { get { return new Point((int)(Bounds.Right + RunVelocity.X), (int)(Bounds.Bottom - (Bounds.Height * 0.222f))); } }
        //private Point LoLeft { get { return new Point((int)(Bounds.Left - RunVelocity.X), (int)(Bounds.Bottom - (Bounds.Height * 0.222f))); } }
        private Point LoRight { get { return new Point((Bounds.Right + 12), (int)(Bounds.Bottom - (Bounds.Height * 0.222f))); } }
        private Point LoLeft { get { return new Point((Bounds.Left - 12), (int)(Bounds.Bottom - (Bounds.Height * 0.222f))); } }
        private Point BotLeft { get { return new Point(Bounds.Left - 1, Bounds.Bottom + 1); } }
        private Point BotMid { get { return new Point(Bounds.Center.X, Bounds.Bottom + 1); } }
        private Point BotRight { get { return new Point(Bounds.Right + 1, Bounds.Bottom + 1); } }
        private Vector2 groundPos { get; set; }
        public Player(GraphicsDevice graphicsDevice, string assetsFolder, Vector2 startPos, float gravity, Room currentRoom) : base(startPos)
        {
            Load(new Atlas(assetsFolder + "clove.atlas", new XnaTextureLoader(graphicsDevice)), assetsFolder + "clove");
            this.gravity = gravity;
            CurrentRoom = currentRoom;
            Tiles = CurrentRoom.Tiles;
            PlayerState = PLAYER_STATE.FALLING;
        }
        public override void Load(Atlas atlas, string animName)
        {
            SkeletonData skeletonData;

            SkeletonJson json = new SkeletonJson(atlas);
            json.Scale = 1.0f;
            skeletonData = json.ReadSkeletonData(animName + ".json");
            Skeleton = new Skeleton(skeletonData);

            AnimStateData = new AnimationStateData(Skeleton.Data);
            AnimStateData.SetMix("idle", "run", 0.1f);
            AnimStateData.SetMix("run", "idle", 0.1f);
            AnimState = new AnimationState(AnimStateData);

            AnimState.SetAnimation(0, "idle", true);

            Skeleton.X = Position.X;
            Skeleton.Y = Position.Y;

            Skeleton.UpdateWorldTransform();
        }

        public void Update(GameTime gameTime)
        {
            ElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            HandleInput(); 
            ApplyGravity();
            ApplyStateBehavior();

            AnimState.Apply(Skeleton);
            PrevPos = Position;

        }
        public void HandleInput()
        {
            CurrentKeyState = Keyboard.GetState();

            if (CurrentKeyState.IsKeyDown(Keys.Right))
            {
                if (Skeleton.FlipX == true) Skeleton.FlipX = false;
                if (PlayerState != PLAYER_STATE.R_RUN && PlayerState != PLAYER_STATE.JUMP)
                {
                    AnimState.SetAnimation(0, "run", true);
                    PlayerState = PLAYER_STATE.R_RUN;
                }
            }
            else if (CurrentKeyState.IsKeyDown(Keys.Left))
            {
                if (Skeleton.FlipX == false) Skeleton.FlipX = true;
                if (PlayerState != PLAYER_STATE.L_RUN && PlayerState != PLAYER_STATE.JUMP)
                {
                    AnimState.SetAnimation(0, "run", true);
                    PlayerState = PLAYER_STATE.L_RUN;
                }
            }
            else if (!(CurrentKeyState.IsKeyDown(Keys.Right)) && !(CurrentKeyState.IsKeyDown(Keys.Left)))
            {
                if (PlayerState != PLAYER_STATE.IDLE && PlayerState != PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING)
                {
                    AnimState.SetAnimation(0, "idle", true);
                    PlayerState = PLAYER_STATE.IDLE;
                }
            }

            if (PlayerState != PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING && CurrentKeyState.IsKeyDown(Keys.Z) && PrevKeyState.IsKeyUp(Keys.Z))
            {
                PlayerState = PLAYER_STATE.JUMP;
            }

            PrevKeyState = CurrentKeyState;
        }
        public void ApplyGravity()
        {
            groundPos = GroundPos();
            if (groundPos == Vector2.Zero)
            {
                Position = new Vector2(Position.X, Position.Y + gravity);
            }
            else
            {
                float distToGround = groundPos.Y - Bounds.Bottom;
                // The Bounds.Center.Y check prevents finding the groundPos.Y above the player which causes gravity to be applied incorrectly.
                if (Bounds.Center.Y <= groundPos.Y && distToGround < gravity)
                {
                    Position = new Vector2(Position.X, Position.Y + distToGround);
                }
                else
                {
                    Position = new Vector2(Position.X, Position.Y + gravity);
                }
            }
        }
        public void ApplyStateBehavior()
        {
            switch (PlayerState)
            {
                case PLAYER_STATE.IDLE:
                    break;
                case PLAYER_STATE.L_RUN:
                    Tile loLeftTile = Tiles.GetTile(LoLeft);
                    if (loLeftTile == null)
                    {
                        Position = new Vector2(Position.X, (groundPos != Vector2.Zero ? groundPos.Y : Position.Y)) - RunVelocity;
                    }
                    else
                    {
                        if (loLeftTile.BlockLeftApproach)
                        {
                            int distToWall = loLeftTile.Bounds.Right - Bounds.Left;
                            if (RunVelocity.X <= distToWall)
                                Position = new Vector2(Position.X, (groundPos != Vector2.Zero ? groundPos.Y : Position.Y)) - RunVelocity;
                            else
                                Position += new Vector2(distToWall, 0);
                        }
                        else
                        {
                            if (Bounds.Center.X <= loLeftTile.EndPoint.X)
                                Position = new Vector2(Position.X, loLeftTile.GetYIntersection(Bounds.Center.X)) - RunVelocity;
                            else
                                Position = new Vector2(Position.X, groundPos.Y) - RunVelocity;
                        }
                    }
                    break;
                case PLAYER_STATE.R_RUN:
                    Tile loRightTile = Tiles.GetTile(LoRight);
                    if (loRightTile == null)
                    {
                        Position = new Vector2(Position.X, (groundPos != Vector2.Zero ? groundPos.Y : Position.Y)) + RunVelocity;
                    }
                    else
                    {
                        if (loRightTile.BlockRightApproach)
                        {
                            int distToWall = loRightTile.Bounds.Left - Bounds.Right;
                            if (RunVelocity.X <= distToWall)
                                Position = new Vector2(Position.X, (groundPos != Vector2.Zero ? groundPos.Y : Position.Y)) + RunVelocity;
                            else
                                Position += new Vector2(distToWall, 0);
                        }
                        else
                        {
                            if (loRightTile.StartPoint.X <= Bounds.Center.X)
                                Position = new Vector2(Position.X, loRightTile.GetYIntersection(Bounds.Center.X)) + RunVelocity;
                            else
                                Position = new Vector2(Position.X, groundPos.Y) + RunVelocity;
                        }
                    }
                    break;
                case PLAYER_STATE.JUMP:
                    break;
            }
        }

        Vector2 GroundPos()
        {
            Tile belowTile = null;
            Tile botMidTile = Tiles.GetTile(BotMid);
            Tile botLeftTile = Tiles.GetTile(BotLeft);
            Tile botRightTile = Tiles.GetTile(BotRight);

            if (botMidTile == null && botRightTile != null)
            {
                belowTile = Tiles.GetPreClampedTile(botRightTile.Row + botRightTile.TopDepth, botRightTile.Column);
                return new Vector2(Position.X, belowTile.GetYIntersection(Bounds.Right - (Bounds.Right - botRightTile.Bounds.Left)));
            }

            if (botMidTile == null && botLeftTile != null)
            {
                belowTile = Tiles.GetPreClampedTile(botLeftTile.Row + botLeftTile.TopDepth, botLeftTile.Column);
                return new Vector2(Position.X, belowTile.GetYIntersection(Bounds.Left + (belowTile.EndPoint.X - Bounds.Left)));
            }

            if (botMidTile != null)
            {
                belowTile = Tiles.GetPreClampedTile(botMidTile.Row + botMidTile.TopDepth, botMidTile.Column);
                return new Vector2(Position.X, belowTile.GetYIntersection(Bounds.Center.X));
            }
            return Vector2.Zero;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public void DrawBounds(SpriteBatch spriteBatch, Texture2D pixelTex)
        {
            spriteBatch.Draw(pixelTex, Bounds, Color.White * 0.5f);
        }
    }
}