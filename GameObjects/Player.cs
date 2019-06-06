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
        public Rectangle Bounds { get { return new Rectangle((int)(Position.X) - 55, (int)(Position.Y) - yOffset, 110, yOffset); } }
        private bool _isGrounded;
        public bool IsGrounded
        {
            get
            {
                return _isGrounded;
            }
            set
            {
                _isGrounded = value;

                if (_isGrounded)
                {
                    airResist = 0.0f;
                }
            }
        }
        public bool IsBlockedLeft { get; set; }
        public bool IsBlockedRight { get; set; }
        public bool IsBlockedTop { get; set; }
        public Room CurrentRoom { get; set; }
        private TileMap Tiles { get; set; }
        private float gravity;
        private readonly int yOffset = 288;
        private int TileSize { get; set; }
        private float PrevBoundsBottom { get; set; }
        private Vector2 RunVelocity = new Vector2(13, 0);
        private KeyboardState CurrentKeyState { get; set; }
        private KeyboardState PrevKeyState { get; set; }
        private Vector2 floorBelow { get; set; }
        private FloorNode currentNode { get; set; }
        private float airResist { get; set; }
        private bool IsJumpPressed { get; set; }
        private bool IsRightPressed { get; set; }
        private bool IsLeftPressed { get; set; }
        private Vector2 _jumpVelocity;
        private Vector2 jumpVelocity
        {
            get
            {
                return _jumpVelocity;
            }
            set
            {
                _jumpVelocity = value;

                if (0 <= _jumpVelocity.Y)
                    IsJumpPressed = false;
                    //PlayerState = PLAYER_STATE.FALLING;
            }
        }
        private int dir = 1; // We'll start facing right by default.
        public Player(GraphicsDevice graphicsDevice, string assetsFolder, Vector2 startPos, float gravity, Room currentRoom) : base(startPos)
        {
            Load(new Atlas(assetsFolder + "clove.atlas", new XnaTextureLoader(graphicsDevice)), assetsFolder + "clove");
            this.gravity = gravity;
            CurrentRoom = currentRoom;
            //Tiles = CurrentRoom.Tiles;
            PlayerState = PLAYER_STATE.FALLING;
            airResist = 0.0f;
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
            ApplyStateBehavior();
            AnimState.Apply(Skeleton);
            PrevBoundsBottom = PrevPosition.Y;
        }
        public void HandleInput()
        {
            CurrentKeyState = Keyboard.GetState();

            if (!(CurrentKeyState.IsKeyDown(Keys.Right)) && !(CurrentKeyState.IsKeyDown(Keys.Left)))
            {
                //dir = 0;
                IsLeftPressed = false;
                IsRightPressed = false;
                if (PlayerState != PLAYER_STATE.IDLE && PlayerState != PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING)
                {
                    PlayerState = PLAYER_STATE.IDLE;
                }
            }

            if (CurrentKeyState.IsKeyDown(Keys.Right))
            {
                dir = 1;
                IsLeftPressed = false;
                IsRightPressed = true;
                if (Skeleton.FlipX == true) Skeleton.FlipX = false;
                if (PlayerState != PLAYER_STATE.R_RUN && PlayerState != PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING)
                {
                    PlayerState = PLAYER_STATE.R_RUN;
                }
            }
            else if (CurrentKeyState.IsKeyDown(Keys.Left))
            {
                dir = -1;
                IsLeftPressed = true;
                IsRightPressed = false;
                if (Skeleton.FlipX == false) Skeleton.FlipX = true;
                if (PlayerState != PLAYER_STATE.L_RUN && PlayerState != PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING)
                {
                    PlayerState = PLAYER_STATE.L_RUN;
                }
            }

            if (PlayerState != PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING 
                && CurrentKeyState.IsKeyDown(Keys.Z) && PrevKeyState.IsKeyUp(Keys.Z))
            {
                IsGrounded = false;
                PlayerState = PLAYER_STATE.JUMP;
                IsJumpPressed = true;
            }

            if (/*PlayerState == PLAYER_STATE.JUMP && PlayerState != PLAYER_STATE.FALLING 
                && */CurrentKeyState.IsKeyUp(Keys.Z) && PrevKeyState.IsKeyDown(Keys.Z))
            {
                IsJumpPressed = false;
            }

            PrevKeyState = CurrentKeyState;
        }
        public void ApplyStateBehavior()
        {
            float offset = 0.0f;
            float distToGround = -1.0f;

            switch (PlayerState)
            {
                case PLAYER_STATE.IDLE:
                    // Set the animation
                    if (AnimState.GetCurrent(0).ToString() != "idle") AnimState.SetAnimation(0, "idle", true);

                    // Check Floor to ensure we are still on the ground
                    if (IsGrounded)
                    {
                        if (currentNode != null)
                        {
                            Vector2 newFloorPos = getWalkingDistance();
                            //Vector2 newFloorPos = getFloorBelow();

                            if (newFloorPos != Vector2.Zero)
                                Position = newFloorPos;
                            else
                                PlayerState = PLAYER_STATE.FALLING;
                        }
                    }
                    else
                    {
                        PlayerState = PLAYER_STATE.FALLING;
                    }
                    break;
                case PLAYER_STATE.L_RUN:
                    // Set the animation
                    if (AnimState.GetCurrent(0).ToString() != "run") AnimState.SetAnimation(0, "run", true);

                    // Check Floor and move
                    if (currentNode != null && IsGrounded)
                    {
                        Vector2 newFloorPos = getWalkingDistance(RunVelocity);

                        if (newFloorPos != Vector2.Zero)
                            Position = newFloorPos;
                        else
                            PlayerState = PLAYER_STATE.FALLING;
                    }
                    else
                    {
                        PlayerState = PLAYER_STATE.FALLING;
                    }
                    break;
                case PLAYER_STATE.R_RUN:
                    // Set the animation
                    if (AnimState.GetCurrent(0).ToString() != "run") AnimState.SetAnimation(0, "run", true);

                    // Check Floor and move
                    if (currentNode != null && IsGrounded)
                    {
                        Vector2 newFloorPos = getWalkingDistance(RunVelocity);

                        if (newFloorPos != Vector2.Zero)
                            Position = newFloorPos;
                        else
                            PlayerState = PLAYER_STATE.FALLING;
                    }
                    else
                    {
                        PlayerState = PLAYER_STATE.FALLING;
                    }
                    break;
                case PLAYER_STATE.JUMP:
                    if (IsGrounded) PlayerState = PLAYER_STATE.IDLE;

                    // First handle horizontal movement
                    if (!IsGrounded)
                    {
                        if (IsLeftPressed)
                        {
                            Position -= RunVelocity;
                            offset = -RunVelocity.X;
                        }

                        if (IsRightPressed)
                        {
                            Position += RunVelocity;
                            offset = RunVelocity.X;
                        }
                    }

                    // Next handle vertical movement
                    floorBelow = getFloorBelow(offset);

                    //IsGrounded = false;










                    // There is no ground below us. We can jump.
                    if (floorBelow == Vector2.Zero 
                        || (Bounds.Bottom + gravity - airResist < floorBelow.Y) 
                        || float.IsNaN(floorBelow.Y)
                        /*|| IsGrounded*/){

                        Jump();
                    }
                    else // There is floor below us, decide what to do.
                    {
                        // We were on the floor. Now we're initiating a jump.
                        if (PrevBoundsBottom == Bounds.Bottom)
                        {
                            Jump();
                        }
                        if (PrevBoundsBottom < Bounds.Bottom || PrevBoundsBottom > Bounds.Bottom) // We are descending, or we are ascending.........................................
                        {
                            if (Bounds.Bottom + gravity <= floorBelow.Y)
                            {
                                distToGround = (floorBelow.Y - (Bounds.Bottom + gravity));
                            }
                            else if (floorBelow.Y > Bounds.Bottom)
                            {
                                distToGround = (floorBelow.Y - (Bounds.Bottom));
                            }
                            else if (Bounds.Bottom > floorBelow.Y && Bounds.Top < floorBelow.Y) // Bounds are below the floor
                            {
                                distToGround = (floorBelow.Y - Bounds.Bottom);
                            }

                            if (distToGround == 0)
                            {
                                IsGrounded = true;
                            }
                            else if (distToGround > 0 && distToGround < (floorBelow.Y - Bounds.Bottom) + jumpVelocity.Y + gravity + airResist/*Bounds.Bottom + distToGround > floorBelow.Y*/)
                            {
                                Position += new Vector2(0, (int)distToGround);
                                if (Bounds.Bottom == floorBelow.Y)
                                    IsGrounded = true;
                                else
                                    IsGrounded = false;
                            }
                            else if (distToGround < -1)
                            {
                                Position += new Vector2(0, distToGround);
                                //Position = floorBelow;
                                IsGrounded = true;
                            }
                            else
                            {
                                Jump();
                            }

                            //distToGround = floorBelow.Y - Bounds.Bottom;

                            //if (distToGround == 0)
                            //{
                            //    IsGrounded = true;
                            //}
                            //else if (Bounds.Bottom - (Bounds.Bottom + (gravity - airResist)) > distToGround && distToGround > 0/*Bounds.Bottom + distToGround > floorBelow.Y*/) // jumpVelocity.Y??
                            //{
                            //    Position += new Vector2(0, (int)distToGround);
                            //    if (Bounds.Bottom == floorBelow.Y)
                            //        IsGrounded = true;
                            //    else
                            //        IsGrounded = false;
                            //}

                            //if (Bounds.Bottom < floorBelow.Y) // The floor is below us
                            //{
                            //    // Continue jump
                            //}
                            //else
                            //{
                            //    // The floor is above or equal to Bounds.Bottom. We need to land.
                            //}
                        }
                    }























                    break;
                case PLAYER_STATE.FALLING:
                    if (IsGrounded) PlayerState = PLAYER_STATE.IDLE;

                    // First handle horizontal movement
                    if (!IsGrounded)
                    {
                        if (IsLeftPressed)
                        {
                            Position -= RunVelocity;
                            offset = -RunVelocity.X;
                        }

                        if (IsRightPressed)
                        {
                            Position += RunVelocity;
                            offset = RunVelocity.X;
                        }
                    }

                    floorBelow = getFloorBelow(offset);//getWalkingDistance();

                    if (airResist > gravity && floorBelow.Y > Bounds.Bottom + gravity - airResist)
                        airResist -= 0.5f;
                    else
                        airResist = 0.0f;

                    // There is no ground below us. We need to fall.
                    if (floorBelow == Vector2.Zero || (/*(Bounds.Top < floorBelow.Y) && */(Bounds.Bottom + gravity - airResist < floorBelow.Y)) || float.IsNaN(floorBelow.Y))
                    {
                        if ((gravity - airResist) < 0)
                        {
                            airResist = 0.0f;
                            Position += new Vector2(0, gravity);
                        }
                        else if (0 <= (gravity - airResist))
                        {
                            Position += new Vector2(0, gravity - airResist);
                        }
                        IsGrounded = false;
                        currentNode = null;
                    }
                    else
                    {
                        //float distToGround = -1.0f;
                        //float distToGround = (floorBelow.Y - Bounds.Bottom);
                        //int distToGround = (int)(floorBelow.Y - Bounds.Bottom);
                        //int distToGround = (int)Math.Ceiling(floorBelow.Y - (Bounds.Bottom + gravity));
                        //int distToGround = (int)Math.Ceiling(floorBelow.Y - Bounds.Bottom);
                        if (Bounds.Bottom + gravity <= floorBelow.Y)
                        {
                            distToGround = (floorBelow.Y - (Bounds.Bottom + gravity));
                        }
                        else if (floorBelow.Y > Bounds.Bottom)
                        {
                            distToGround = (floorBelow.Y - (Bounds.Bottom));
                        }
                        else if (Bounds.Bottom > floorBelow.Y && Bounds.Top < floorBelow.Y) // Bounds are below the floor
                        {
                            distToGround = (floorBelow.Y - Bounds.Bottom);
                        }

                        //distToGround = (int)(floorBelow.Y - Bounds.Bottom);
                        if (distToGround == 0)
                        {
                            IsGrounded = true;
                        }
                        else if (distToGround > 0/*Bounds.Bottom + distToGround > floorBelow.Y*/)
                        {
                            Position += new Vector2(0, (int)distToGround);
                            if (Bounds.Bottom == floorBelow.Y)
                                IsGrounded = true;
                            else
                                IsGrounded = false;
                        }
                        else if (distToGround < -1)
                        {
                            Position += new Vector2(0, distToGround);
                            //Position = floorBelow;
                            IsGrounded = true;
                        }
                    }
                    break;
            }
        }

        private void Jump()
        {
            if (IsJumpPressed)
            {
                airResist += 0.25f;
                jumpVelocity = new Vector2(0, -19.0f + airResist);
                Position += jumpVelocity;
            }
            else // Then we need to resolve this jump.
            {
                if (floorBelow.Y > Bounds.Bottom + (gravity/* - airResist*/) && jumpVelocity.Y < gravity)
                {
                    airResist += 1.0f;
                    jumpVelocity = new Vector2(0, -17.0f + airResist);
                    Position += jumpVelocity;
                }
                else
                {
                    PlayerState = PLAYER_STATE.FALLING;
                }
            }
        }
        public Vector2 getWalkingDistance(Vector2 offset = new Vector2())
        {
            Vector2 yIntersection;

            foreach (FloorNode node in CurrentRoom.Floor.Nodes.Items)
            {
                if (dir < 0)
                {
                    if ((node.Position.X <= Position.X && node.RightNeighbor.Position.X >= Position.X) ||
                        (node.Position.X <= Position.X && node.RightNeighbor.Position == node.Position)){ 
                        // Are we on the right edge trying to turn around?

                        yIntersection = CONSTANTS.Y_INTERSECTION(node.RightNeighbor.Position, node.Position, Position.X);
                        if (!IsGrounded && yIntersection.Y > Bounds.Bottom)
                        {
                            currentNode = node;
                            return yIntersection;
                        }
                        else if (IsGrounded)
                        {
                            currentNode = node;
                            return CONSTANTS.VECTOR_DISTANCE(Position, node.Position, offset.X);
                        }
                    } //Are we on the left edge?
                    else if (node.LeftNeighbor.Position == node.Position 
                        && Position.X - offset.X < node.Position.X
                        && node.Position.X <= Bounds.Right){

                        yIntersection = CONSTANTS.Y_INTERSECTION(node.RightNeighbor.Position, node.Position, Position.X);
                        if (!IsGrounded && yIntersection.Y > Bounds.Bottom)
                        {
                            currentNode = node;
                            return yIntersection;
                        }
                        else if (IsGrounded)
                        {
                            currentNode = node;
                            if (node.LeftNeighbor.Position == node.Position 
                                && Position.X - offset.X < node.LeftNeighbor.Position.X 
                                && node.Position.X <= Bounds.Right){

                                Vector2 newDist = 
                                    CONSTANTS.Y_INTERSECTION(node.Position, new Vector2(
                                        node.Position.X - offset.X + (Position.X - Bounds.Left), 
                                        node.Position.Y), Position.X - offset.X);
                                return newDist;
                            }
                            else
                            {
                                //currentNode = node;
                                //return CONSTANTS.VECTOR_DISTANCE(Position, node.Position, offset.X);
                                return Vector2.Zero;
                            }
                        }
                    }
                }
                else if (dir > 0) // Are we moving right?
                {
                    if ((node.Position.X >= Position.X && node.LeftNeighbor.Position.X <= Position.X) ||
                        (node.Position.X >= Position.X && node.LeftNeighbor.Position == node.Position)){

                        // Are we on the left edge and trying to turn around?
                        yIntersection = CONSTANTS.Y_INTERSECTION(node.LeftNeighbor.Position, node.Position, Position.X);
                        if (!IsGrounded && yIntersection.Y > Bounds.Bottom)
                        {
                            currentNode = node;
                            return yIntersection;
                        }
                        else if (IsGrounded)
                        {
                            currentNode = node;
                            return CONSTANTS.VECTOR_DISTANCE(Position, node.Position, offset.X);
                        }

                    } // Are we on the right edge?
                    else if (node.RightNeighbor.Position == node.Position 
                        && Position.X + offset.X > node.RightNeighbor.Position.X 
                        && node.Position.X >= Bounds.Left){

                        yIntersection = CONSTANTS.Y_INTERSECTION(node.LeftNeighbor.Position, node.Position, Position.X);
                        if (!IsGrounded && yIntersection.Y > Bounds.Bottom)
                        {
                            currentNode = node;
                            return yIntersection;
                        }
                        else if (IsGrounded)
                        {
                            currentNode = node;

                            if (node.RightNeighbor.Position == node.Position 
                                && Position.X + offset.X > node.Position.X
                                && node.Position.X >= Bounds.Left){
                                
                                //if (node.LeftNeighbor.Position == node.Position && Position.X - offset.X < node.LeftNeighbor.Position.X && node.Position.X <= Bounds.Right)
                                Vector2 newDist = CONSTANTS.Y_INTERSECTION(node.Position, new Vector2(
                                    node.Position.X + offset.X + (Bounds.Right - Position.X), 
                                    node.Position.Y), Position.X + offset.X);
                                //Vector2 newDist = CONSTANTS.Y_INTERSECTION(node.Position, new Vector2(node.Position.X - offset.X + (Position.X - Bounds.Left), node.Position.Y), Position.X - offset.X);
                                return newDist;
                            }
                            else
                            {
                                //currentNode = node;
                                //return CONSTANTS.VECTOR_DISTANCE(Position, node.Position, offset.X);
                                return Vector2.Zero;
                            }
                        }
                    }
                }
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Use this for finding the floor while in the air
        /// </summary>
        /// <returns></returns>
        public Vector2 getFloorBelow(float offset = 0.0f)
        {
            Vector2 yIntersection = Vector2.Zero;
            foreach (FloorNode node in CurrentRoom.Floor.Nodes.Items)
            {
                if ((Bounds.Bottom <= node.Position.Y) || (Bounds.Bottom <= node.LeftNeighbor.Position.Y) || (Bounds.Bottom <= node.RightNeighbor.Position.Y))
                {
                    if (dir < 0)
                    {
                        if ((node.Position.X <= Position.X && node.RightNeighbor.Position.X >= Position.X) ||
                            (node.Position.X <= Position.X && node.RightNeighbor.Position == node.Position))
                        {

                            // Are we on the right edge trying to turn around?
                            currentNode = node;
                            yIntersection = CONSTANTS.Y_INTERSECTION(node.RightNeighbor.Position, node.Position, Position.X);
                        } //Are we on the left edge?
                        else if (node.LeftNeighbor.Position == node.Position
                            && Position.X - offset < node.LeftNeighbor.Position.X
                            && node.Position.X <= Bounds.Right)
                        {

                            currentNode = node;
                            yIntersection = CONSTANTS.Y_INTERSECTION(node.RightNeighbor.Position, node.Position, Position.X);
                        }

                        // Are we on the right edge?
                        if (node.RightNeighbor.Position == node.Position
                            && Position.X - offset > node.RightNeighbor.Position.X
                            && node.Position.X >= Bounds.Left)
                        {

                            currentNode = node;
                            yIntersection = CONSTANTS.Y_INTERSECTION(node.LeftNeighbor.Position, node.Position, Position.X);
                        }
                    }
                    else if (dir > 0) // Are we moving right?
                    {
                        if ((node.Position.X >= Position.X && node.LeftNeighbor.Position.X <= Position.X) ||
                            (node.Position.X >= Position.X && node.LeftNeighbor.Position == node.Position))
                        {

                            // Are we on the left edge and trying to turn around?
                            currentNode = node;
                            yIntersection = CONSTANTS.Y_INTERSECTION(node.LeftNeighbor.Position, node.Position, Position.X);

                        } // Are we on the right edge?
                        else if (node.RightNeighbor.Position == node.Position
                            && Position.X + offset > node.RightNeighbor.Position.X
                            && node.Position.X >= Bounds.Left)
                        {

                            currentNode = node;
                            yIntersection = CONSTANTS.Y_INTERSECTION(node.LeftNeighbor.Position, node.Position, Position.X);
                        }

                        // Are we on the left edge?
                        if (node.LeftNeighbor.Position == node.Position
                            && Position.X + offset < node.LeftNeighbor.Position.X
                            && node.Position.X <= Bounds.Right)
                        {

                            currentNode = node;
                            yIntersection = CONSTANTS.Y_INTERSECTION(node.RightNeighbor.Position, node.Position, Position.X);
                        }
                    }
                }
            }
            return yIntersection;
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