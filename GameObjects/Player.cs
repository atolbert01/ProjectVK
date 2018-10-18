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
        JUMP
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
        private Tile[,] TileMap { get; set; }
        private float gravity;
        private readonly int yOffset = 288;
        private int TileSize { get; set; }
        private Vector2 PrevPos { get; set; }
        private Vector2 RunVelocity = new Vector2(12, 0);
        private Vector2 JumpStart { get; set; }
        private readonly int jumpMaxHeight = 256;
        private Vector2 jumpVelocity = new Vector2(0, 18);
        private int hDir, vDir = 0;
        private KeyboardState CurrentKeyState { get; set; }
        private KeyboardState PrevKeyState { get; set; }
        private Point TopMid { get { return CurrentRoom.GetRoomClampedPoint(Bounds.Top - (int)jumpVelocity.Y, (int)(Bounds.Center.X)); } }
        private Point MidLeft { get { return CurrentRoom.GetRoomClampedPoint((int)(Bounds.Top + (Bounds.Height * 0.5)), (int)(Bounds.Left - (RunVelocity.X) + 1)); } }
        private Point LoLeft { get { return CurrentRoom.GetRoomClampedPoint((int)(Bounds.Bottom - (Bounds.Height * 0.1)), (int)(Bounds.Left - (RunVelocity.X) + 1)); } }
        private Point MidRight { get { return CurrentRoom.GetRoomClampedPoint((int)(Bounds.Top + (Bounds.Height * 0.5)), (int)(Bounds.Right + RunVelocity.X)); } }
        private Point LoRight { get { return CurrentRoom.GetRoomClampedPoint((int)(Bounds.Bottom - (Bounds.Height * 0.1)), (int)(Bounds.Right + RunVelocity.X)); } }
        private Point BotLeft { get { return CurrentRoom.GetRoomClampedPoint(Bounds.Bottom + (int)gravity, Bounds.Left + 1); } }
        private Point BotMid { get { return CurrentRoom.GetRoomClampedPoint(Bounds.Bottom + (int)gravity, Bounds.Center.X); } }
        private Point BotRight { get { return CurrentRoom.GetRoomClampedPoint(Bounds.Bottom + (int)gravity, Bounds.Right); } }
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
            TileMap = CurrentRoom.TileMap;
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
                if (PlayerState != PLAYER_STATE.IDLE && PlayerState != PLAYER_STATE.JUMP)
                {
                    AnimState.SetAnimation(0, "idle", true);
                    PlayerState = PLAYER_STATE.IDLE;
                }
            }

            if (PlayerState != PLAYER_STATE.JUMP && CurrentKeyState.IsKeyDown(Keys.Z) && PrevKeyState.IsKeyUp(Keys.Z))
            {
                JumpStart = new Vector2(Bounds.Center.X, Bounds.Bottom);
                PlayerState = PLAYER_STATE.JUMP;
            }

            PrevKeyState = CurrentKeyState;
        }
        public void ResolveCollisions()
        {
            AssignCollisionZones();
            ResolveHorizontalCollisions();
            ResolveVerticalCollisions();
        }
        void AssignCollisionZones()
        {
            ColTopMid = TileMap[TopMid.Y, TopMid.X];
            ColMidRight = TileMap[MidRight.Y, MidRight.X];
            ColLoRight = TileMap[LoRight.Y, LoRight.X];
            ColBotRight = TileMap[BotRight.Y, BotRight.X];
            ColBotMid = TileMap[BotMid.Y, BotMid.X];
            ColBotLeft = TileMap[BotLeft.Y, BotLeft.X];
            ColLoLeft = TileMap[LoLeft.Y, LoLeft.X];
            ColMidLeft = TileMap[MidLeft.Y, MidLeft.X];
        }

        void ResolveHorizontalCollisions()
        {
            if (/*hDir > 0*/PlayerState == PLAYER_STATE.R_RUN)
            { // Begin Right Collision Check
                IsBlockedLeft = false;
                if (ColLoRight != null && ColLoRight.Bounds.Left - (Bounds.Right + RunVelocity.X) <= 0)
                {
                    distToWall = ColLoRight.Bounds.Left - (Bounds.Right);
                    //if (IsGrounded)
                    {
                        if (ColBotMid != null && ColBotMid != ColLoRight)
                        {
                            if (ColBotMid.EndPoint.Y > ColLoRight.StartPoint.Y && Bounds.Bottom > ColLoRight.StartPoint.Y)
                            {
                                IsBlockedRight = true;
                            }
                            else
                            {
                                IsBlockedRight = false;
                                distToWall = (int)RunVelocity.X + 1;
                            }
                        }
                    }
                    //else
                    //{
                    //    IsBlockedRight = true;
                    //}
                }
                else
                {
                    IsBlockedRight = false;
                    distToWall = (int)RunVelocity.X + 1;
                }

                if (ColMidRight != null && ColMidRight.Bounds.Left - (Bounds.Right + RunVelocity.X) <= 0)
                {
                    distToWall = ColMidRight.Bounds.Left - (Bounds.Right);
                    //if (IsGrounded)
                    {
                        if (ColLoRight != null)
                        {
                            if (ColLoRight.EndPoint.Y >= ColMidRight.StartPoint.Y)
                            {
                                IsBlockedRight = true;
                            }
                            else
                            {
                                IsBlockedRight = false;
                                distToWall = (int)RunVelocity.X + 1;
                            }
                        }

                        if (ColBotMid != null)
                        {
                            if (ColBotMid.EndPoint.Y > ColMidRight.StartPoint.Y)
                            {
                                IsBlockedRight = true;
                            }
                            else
                            {
                                IsBlockedRight = false;
                                distToWall = (int)RunVelocity.X + 1;
                            }
                        }
                    }
                    //else
                    //{
                    //    IsBlockedRight = true;
                    //}
                }
            } // End Right Collision Check

            if (/*hDir < 0*/PlayerState == PLAYER_STATE.L_RUN)
            { // Begin Left Collision Check
                IsBlockedRight = false;
                if (ColLoLeft != null && (Bounds.Left - RunVelocity.X) - ColLoLeft.Bounds.Right <= 0)
                {
                    distToWall = (Bounds.Left) - ColLoLeft.Bounds.Right;
                    //if (IsGrounded)
                    {
                        if (ColBotMid != null && ColBotMid != ColLoLeft)
                        {
                            if (ColBotMid.StartPoint.Y > ColLoLeft.EndPoint.Y)
                            {
                                IsBlockedLeft = true;
                            }
                            else
                            {
                                IsBlockedLeft = false;
                                distToWall = (int)RunVelocity.X + 1;
                            }
                        }
                    }
                    //else
                    //{
                    //    IsBlockedLeft = true;
                    //}
                }
                else
                {
                    IsBlockedLeft = false;
                    distToWall = (int)RunVelocity.X + 1;
                }

                if (ColMidLeft != null && (Bounds.Left - RunVelocity.X) - ColMidLeft.Bounds.Right <= 0)
                {
                    distToWall = Bounds.Left - ColMidLeft.Bounds.Right;
                    //if (IsGrounded)
                    {
                        if (ColLoLeft != null)
                        {
                            if (ColLoLeft.StartPoint.Y >= ColMidLeft.EndPoint.Y)
                            {
                                IsBlockedLeft = true;
                            }
                            else
                            {
                                IsBlockedLeft = false;
                                distToWall = (int)RunVelocity.X + 1;
                            }
                        }

                        if (ColBotMid != null)
                        {
                            if (ColBotMid.StartPoint.Y > ColMidLeft.EndPoint.Y)
                            {
                                IsBlockedLeft = true;
                            }
                            else
                            {
                                IsBlockedLeft = false;
                                distToWall = (int)RunVelocity.X + 1;
                            }
                        }
                    }
                    //else
                    //{
                    //    IsBlockedLeft = true;
                    //}
                }
            } // End Left Collision Check
        }
        void ResolveVerticalCollisions()
        {
            // The sidePoint and sideTile are used to determine which tile the player will walk onto next
            Point sidePoint = new Point();
            Tile sideTile = null;
            // Determine if on ground
            if (ColBotRight == null && ColBotLeft == null)
                IsGrounded = false;

            if (!IsGrounded)
            {
                distToGround = gravity;
            }

            if (IsGrounded)
            {
                if (hDir > 0 && !IsBlockedRight)
                {
                    sidePoint = CurrentRoom.GetRoomClampedPoint((int)(Bounds.Bottom - (Bounds.Height * 0.25)), (int)(Bounds.Center.X + RunVelocity.X));
                    sideTile = TileMap[sidePoint.Y, sidePoint.X];
                }
                else if (hDir < 0 && !IsBlockedLeft)
                {
                    sidePoint = CurrentRoom.GetRoomClampedPoint((int)(Bounds.Bottom - (Bounds.Height * 0.25)), (int)(Bounds.Center.X - RunVelocity.X));
                    sideTile = TileMap[sidePoint.Y, sidePoint.X];
                }
            }

            if (sideTile != null)
            {
                distToGround = sideTile.GetYIntersection(Bounds.Center.X) - Bounds.Bottom;
            }
            else
            {
                // Ramp voodoo magic below
                if (ColBotLeft != null)
                    if (ColBotMid == null)
                    {
                        distToGround = ColBotLeft.GetYIntersection(Bounds.Left + (ColBotLeft.Bounds.Right - Bounds.Left)) - Bounds.Bottom;
                    }
                    else
                    {
                        if (hDir < 0)
                        {
                            distToGround = ColBotMid.GetYIntersection(Bounds.Center.X + (Bounds.Right - ColBotMid.Bounds.Left)) - Bounds.Bottom;
                        }
                        else
                        {
                            distToGround = ColBotLeft.GetYIntersection(Bounds.Center.X) - Bounds.Bottom;
                        }

                    }
                if (ColBotRight != null)
                    if (ColBotMid == null)
                    {
                        distToGround = ColBotRight.GetYIntersection(Bounds.Right - (Bounds.Right - ColBotRight.Bounds.Left)) - Bounds.Bottom;
                    }
                    else
                    {
                        if (hDir < 0)
                        {
                            distToGround = ColBotRight.GetYIntersection(Bounds.Center.X + (ColBotRight.Bounds.Left - Bounds.Right)) - Bounds.Bottom;
                        }
                        else
                        {
                            distToGround = ColBotRight.GetYIntersection(Bounds.Center.X) - Bounds.Bottom;
                        }
                    }

                if (ColBotMid != null)
                {
                    float yIntersection = ColBotMid.GetYIntersection(Bounds.Center.X);

                    distToGround = yIntersection - Bounds.Bottom;

                    if (hDir < 0) // We moving to the left?
                    {
                        if (ColBotRight != null)
                        {
                            if (ColBotRight.StartPoint.Y < yIntersection && Bounds.Center.X <= ColBotRight.StartPoint.X && ColBotRight.StartPoint.Y != ColBotMid.EndPoint.Y)
                            {
                                distToGround = (ColBotRight.StartPoint.Y - Bounds.Bottom);
                            }
                        }
                        if (ColBotLeft != null)
                        {
                            if (ColBotLeft.EndPoint.Y < yIntersection && Bounds.Center.X >= ColBotLeft.EndPoint.X && ColBotLeft.EndPoint.Y < ColBotMid.StartPoint.Y)
                            {
                                distToGround = ColBotLeft.GetYIntersection(Bounds.Center.X + (ColBotLeft.EndPoint.X - Bounds.Center.X)) - Bounds.Bottom;
                            }
                        }
                    }
                    else // We moving to the right.
                    {
                        if (ColBotLeft != null)
                        {
                            if (ColBotLeft.EndPoint.Y < yIntersection && Bounds.Center.X >= ColBotLeft.EndPoint.X && ColBotLeft.EndPoint.Y != ColBotMid.StartPoint.Y)
                            {
                                distToGround = (ColBotLeft.EndPoint.Y - Bounds.Bottom);
                            }
                        }

                        if (ColBotRight != null)
                        {
                            if(ColBotRight.StartPoint.Y < yIntersection && Bounds.Center.X <= ColBotRight.StartPoint.X && ColBotRight.StartPoint.Y != ColBotMid.EndPoint.Y)
                            {
                                distToGround = (ColBotRight.StartPoint.Y - Bounds.Bottom);
                            }
                        }
                    }
                }
            }


            // After all ground tile checks apply downward motion
            if (PlayerState != PLAYER_STATE.JUMP)
            {
                if (distToGround > gravity)
                    distToGround = gravity;

                if (distToGround < 0 && distToGround > -RunVelocity.X)
                    Position += new Vector2(0, distToGround);
                else if (distToGround != 0)
                    Position += new Vector2(0, distToGround);
                else
                {
                    distToGround = 0.0f;
                }

                if (distToGround == 0.0f)
                    IsGrounded = true;
            }
        }
        public void ApplyStateBehavior()
        {
            switch (PlayerState)
            {
                case PLAYER_STATE.IDLE:
                    break;
                case PLAYER_STATE.R_RUN:
                    if (0 <= distToWall && distToWall < RunVelocity.X)
                        Position += new Vector2(distToWall, 0);
                    else
                        Position += RunVelocity;
                    break;
                case PLAYER_STATE.L_RUN:
                    if (0 <= distToWall && distToWall < RunVelocity.X)
                        Position -= new Vector2(distToWall, 0);
                    else
                        Position -= RunVelocity;
                    break;
                case PLAYER_STATE.JUMP:
                    if (Bounds.Bottom > JumpStart.Y - jumpMaxHeight)
                    {
                        Position -= jumpVelocity;
                        if (IsBlockedTop == false)
                        {
                            Position -= jumpVelocity;
                        }
                        else
                        {
                            int distToCeiling = ((TopMid.Y * Constants.TILE_SIZE) + Constants.TILE_SIZE + 1) - Bounds.Top;
                            if (distToCeiling != 0)
                            {
                                Position += new Vector2(0, distToCeiling);
                            }
                            PlayerState = PLAYER_STATE.IDLE;
                        }

                        if (CurrentKeyState.IsKeyDown(Keys.Right))
                        {
                            if (0 <= distToWall && distToWall < RunVelocity.X)
                                Position += new Vector2(distToWall, 0);
                            else
                                Position += RunVelocity;
                        }
                        else if (CurrentKeyState.IsKeyDown(Keys.Left))
                        {
                            if (0 <= distToWall && distToWall < RunVelocity.X)
                                Position -= new Vector2(distToWall, 0);
                            else
                                Position -= RunVelocity;
                        }

                    }
                    else
                    {
                        PlayerState = PLAYER_STATE.IDLE;
                    }
                    break;
            }
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
