using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine;
using System;

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
        public Rectangle Bounds { get { return new Rectangle((int)(Position.X) - 55, (int)(Position.Y) - yOffset, 110, 286); } }
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

        private Vector2 JumpStart { get; set; }
        private readonly int jumpMaxHeight = 256;
        private Vector2 jumpVelocity = new Vector2(0, 8);
        private float jumpMultiplier = 0.0f;

        private int hDir, vDir = 0;
        private KeyboardState CurrentKeyState { get; set; }
        private KeyboardState PrevKeyState { get; set; }
        private Point TopMid { get { return Tiles.GetClampedPoint(Bounds.Top - (int)jumpVelocity.Y, (int)(Bounds.Center.X)); } }
        private Point MidLeft { get { return Tiles.GetClampedPoint((int)(Bounds.Top + (Bounds.Height * 0.5)), (int)(Bounds.Left - (RunVelocity.X) + 1)); } }
        private Point LoLeft { get { return Tiles.GetClampedPoint((int)(Bounds.Bottom - (Bounds.Height * 0.1)), (int)(Bounds.Left - (RunVelocity.X) + 1)); } }
        private Point MidRight { get { return Tiles.GetClampedPoint((int)(Bounds.Top + (Bounds.Height * 0.5)), (int)(Bounds.Right + RunVelocity.X)); } }
        private Point LoRight { get { return Tiles.GetClampedPoint((int)(Bounds.Bottom - (Bounds.Height * 0.1)), (int)(Bounds.Right + RunVelocity.X)); } }
        private Point BotLeft { get { return Tiles.GetClampedPoint(Bounds.Bottom + (int)gravity, Bounds.Left + 1); } }
        private Point BotMid { get { return Tiles.GetClampedPoint(Bounds.Bottom + (int)gravity, Bounds.Center.X); } }
        private Point BotRight { get { return Tiles.GetClampedPoint(Bounds.Bottom + (int)gravity, Bounds.Right); } }
        private Tile ColLoLeft { get; set; }
        private Tile ColMidLeft { get; set; }
        private Tile ColLoRight { get; set; }
        private Tile ColMidRight { get; set; }
        private Tile ColBotLeft { get; set; }
        private Tile ColBotRight { get; set; }
        private Tile ColBotMid { get; set; }
        private Tile ColTopMid { get; set; }
        private float distToGround = 0.0f;
        private int distToWall = 0;

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

            HandleInput(); // Here, we will get the input and make a determination about what player state we should be in.
            ResolveCollisions(); // Based on our anticipated player state, different types of motion will be authorized. Before applying motion, we need to ensure that the player can move.
            ApplyStateBehavior(); // Now we know what state the player is in. Apply the correct behavior.

            AnimState.Apply(Skeleton);
            if (PrevPos != null)
            {
                if (Position.X - PrevPos.X < 0)
                    hDir = -1;
                else if (Position.X - PrevPos.X > 0)
                    hDir = 1;

                if (Position.Y - PrevPos.Y < 0)
                    vDir = -1;
                else
                    vDir = 1;
            }
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
                JumpStart = new Vector2(Bounds.Center.X, Bounds.Bottom);
                PlayerState = PLAYER_STATE.JUMP;
            }

            PrevKeyState = CurrentKeyState;
        }
        public void ResolveCollisions()
        {
            AssignCollisionZones();
            //ResolveHorizontalCollisions();
            ResolveVerticalCollisions();
        }
        void AssignCollisionZones()
        {
            ColTopMid = Tiles.Map[TopMid.Y, TopMid.X];
            ColMidRight = Tiles.Map[MidRight.Y, MidRight.X];
            ColLoRight = Tiles.Map[LoRight.Y, LoRight.X];
            ColBotRight = Tiles.Map[BotRight.Y, BotRight.X];
            ColBotMid = Tiles.Map[BotMid.Y, BotMid.X];
            ColBotLeft = Tiles.Map[BotLeft.Y, BotLeft.X];
            ColLoLeft = Tiles.Map[LoLeft.Y, LoLeft.X];
            ColMidLeft = Tiles.Map[MidLeft.Y, MidLeft.X];
        }
        
        void ResolveHorizontalCollisions()
        {
        }
        void ResolveVerticalCollisions()
        {
            if (ColBotLeft == null && ColBotRight == null)
            {
                IsGrounded = false;
            }
            else if (ColBotMid != null && ColBotMid.TopDepth == 0)
            {
                IsGrounded = true;
            }
            else if (ColBotLeft != null && ColBotLeft.TopDepth == 0)
            {
                
                IsGrounded = true;
            }
            else if (ColBotRight != null && ColBotRight.TopDepth == 0)
            {
                IsGrounded = true;
            }

            if (!IsGrounded)
            {
                Position += new Vector2(0, gravity);
            }
        }
        public void ApplyStateBehavior()
        {
            switch (PlayerState)
            {
                case PLAYER_STATE.IDLE:
                    break;
                case PLAYER_STATE.L_RUN:
                    Position = GetGroundPosition(-1);
                    break;
                case PLAYER_STATE.R_RUN:
                    //Position += RunVelocity;
                    Position = GetGroundPosition(1);
                    break;
            }

        }
        Vector2 GetGroundPosition(int direction)
        {
            Vector2 groundPos = new Vector2();
            Tile groundTile = GroundTile(direction);

            if (direction < 0)
            {
                if (groundTile != null)
                {
                    groundPos = new Vector2(Position.X - RunVelocity.X, Position.Y - (Bounds.Bottom - groundTile.GetYIntersection((Bounds.Center.X))));
                }
                else
                {
                    groundPos = Position - RunVelocity;
                }
            }
            else
            {
                if (groundTile != null)
                {
                    groundPos = new Vector2(Position.X + RunVelocity.X, Position.Y - (Bounds.Bottom - groundTile.GetYIntersection(Bounds.Center.X)));
                }
                else
                {
                    groundPos = Position + RunVelocity;
                }
            }

            return groundPos;
        }
        Tile GroundTile(int direction)
        {
            if (direction < 0) // We're moving to the left
            {
                if (ColBotMid != null)
                {
                    if (ColBotMid.TopDepth != 0)
                    {
                        Tile topTile = Tiles.Map[ColBotMid.Row + ColBotMid.TopDepth, ColBotMid.Column];
                        if (Bounds.Bottom == topTile.EndPoint.Y)
                        {
                            return topTile;
                        }
                    }
                    else
                    {
                        return ColBotMid;
                    }
                }

                if (ColBotRight != null)
                {
                    if (ColBotRight.TopDepth == 0)
                    {
                        if (ColBotRight.StartPoint.Y < Bounds.Bottom)
                        {
                            return ColBotRight;
                        }
                    }
                }

                if (ColLoLeft != null)
                {
                    if (ColLoLeft.TopDepth == 0)
                    {
                        if (ColLoLeft.EndPoint.Y >= Bounds.Bottom)
                        {
                            return ColLoLeft;
                        }
                    }
                }
            }
            else if (0 < direction) // We're moving to the right
            {
                if (ColBotMid != null)
                {
                    if (ColBotMid.TopDepth != 0)
                    {
                        Tile topTile = Tiles.Map[ColBotMid.Row + ColBotMid.TopDepth, ColBotMid.Column];
                        if (Bounds.Bottom == topTile.StartPoint.Y)
                        {
                            return topTile;
                        }
                    }
                    else
                    {
                        return ColBotMid;
                    }
                }

                if (ColBotLeft != null)
                {
                    if (ColBotLeft.TopDepth == 0)
                    {
                        if (ColBotLeft.EndPoint.Y < Bounds.Bottom)
                        {
                            return ColBotLeft;
                        }
                    }
                }


                if (ColLoLeft != null)
                {
                    if (ColLoLeft.TopDepth == 0)
                    {
                        if (ColLoLeft.EndPoint.Y >= Bounds.Bottom)
                        {
                            return ColLoLeft;
                        }
                    }
                }

                if (ColBotRight != null)
                {
                    if (ColBotRight.TopDepth == 0)
                    {
                        if (ColBotRight.StartPoint.Y == Bounds.Bottom)
                        {
                            return ColBotRight;
                        }
                    }
                }

                if (ColLoRight != null)
                {
                    if (ColLoRight.TopDepth == 0)
                    {
                        if (ColLoRight.StartPoint.Y >= Bounds.Bottom)
                        {
                            return ColLoRight;
                        }
                    }
                }
            }

            return null;
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
