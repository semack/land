using System;
using Land.Classes;
using Land.Common;
using Land.Enums;
using Land.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Land.Components.Actors
{
    public class Hero : BaseActor
    {
        private readonly Bullet _bullet;
        private int _bioMassAttempts;
        private bool _heroIdleDivider;
        private DirectionEnum _heroIdleHeadDirection = DirectionEnum.Left;
        private GamePadState _oldGamePadState;
        private KeyboardState _oldKeyboardStateState;
        private DirectionEnum _shootDirection;
        private ShootStageEnum _shootStage;

        public Hero(TheGame game, Room room, Bullet bullet)
            : base(game, room, 1)
        {
            _bullet = bullet;
        }

        public bool IsInBiomass { get; private set; }

        private int ChestCount
        {
            get
            {
                int chestCount = 0;
                for (int i = 0; i < Maps.CapacityX; i++)
                {
                    for (int j = 0; j < Maps.CapacityY; j++)
                    {
                        if (Maps.IsChest(Room[i, j]))
                            chestCount++;
                    }
                }
                return chestCount;
            }
        }

        public event EventHandler OnRoomFinished;
        public event EventHandler OnLifeFired;
        public event EventHandler OnChestHappened;

        public void Reset()
        {
            IsInBiomass = false;
            _bioMassAttempts = 15;
            _shootDirection = DirectionEnum.None;
            Visible = true;
            base.Reset(1, 14);
        }

        protected override SpriteTypeEnum GetSprite(bool isFalling, SpriteTypeEnum oldSprite)
        {
            if (_shootStage == ShootStageEnum.Fire)
            {
                _shootStage = ShootStageEnum.None;
                DirectionEnum direction = _shootDirection;
                if (direction == DirectionEnum.Left)
                    return SpriteTypeEnum.HeroShotLeft;
                return SpriteTypeEnum.HeroShotRight;
            }

            if (_shootStage == ShootStageEnum.Preparation)
            {
                _shootStage = ShootStageEnum.Fire;
                if (_shootDirection == DirectionEnum.Left)
                    return SpriteTypeEnum.HeroPrepareShotLeft;
                return SpriteTypeEnum.HeroPrepareShotRight;
            }

            if (IsInBiomass)
            {
                _bioMassAttempts--;
                if (_bioMassAttempts == 0 && OnLifeFired != null)
                    OnLifeFired(this, new EventArgs());

                if (oldSprite == SpriteTypeEnum.HeroInBiomass1)
                    return SpriteTypeEnum.HeroInBiomass2;
                if (oldSprite == SpriteTypeEnum.HeroInBiomass2)
                    return SpriteTypeEnum.HeroInBiomass3;
                return SpriteTypeEnum.HeroInBiomass1;
            }

            if (isFalling || (Direction == DirectionEnum.Up || Direction == DirectionEnum.Down)) // Fall and Stairs
            {
                if (oldSprite == SpriteTypeEnum.HeroStairsLeft)
                    return SpriteTypeEnum.HeroStairsRight;
                return SpriteTypeEnum.HeroStairsLeft;
            }

            if (Direction == DirectionEnum.None) //  Idle
            {
                if (_heroIdleDivider)
                {
                    _heroIdleDivider = false;

                    if (oldSprite == SpriteTypeEnum.HeroIdleRight)
                    {
                        _heroIdleHeadDirection = DirectionEnum.Left;
                        return SpriteTypeEnum.HeroIdle;
                    }
                    if (oldSprite == SpriteTypeEnum.HeroIdleLeft)
                    {
                        _heroIdleHeadDirection = DirectionEnum.Right;
                        return SpriteTypeEnum.HeroIdle;
                    }
                    if (oldSprite == SpriteTypeEnum.HeroIdle)
                    {
                        if (_heroIdleHeadDirection == DirectionEnum.Right)
                            return SpriteTypeEnum.HeroIdleRight;
                        if (_heroIdleHeadDirection == DirectionEnum.Left)
                            return SpriteTypeEnum.HeroIdleLeft;
                    }
                }
                else
                {
                    _heroIdleDivider = true;
                    return oldSprite;
                }
            }
            if (Direction == DirectionEnum.Left)
            {
                if (oldSprite == SpriteTypeEnum.HeroMoveLeft1)
                    return SpriteTypeEnum.HeroMoveLeft2;
                if (oldSprite == SpriteTypeEnum.HeroMoveLeft2)
                    return SpriteTypeEnum.HeroMoveLeft3;
                if (oldSprite == SpriteTypeEnum.HeroMoveLeft3)
                    return SpriteTypeEnum.HeroMoveLeft4;
                return SpriteTypeEnum.HeroMoveLeft1;
            }
            if (Direction == DirectionEnum.Right)
            {
                if (oldSprite == SpriteTypeEnum.HeroMoveRight1)
                    return SpriteTypeEnum.HeroMoveRight2;
                if (oldSprite == SpriteTypeEnum.HeroMoveRight2)
                    return SpriteTypeEnum.HeroMoveRight3;
                if (oldSprite == SpriteTypeEnum.HeroMoveRight3)
                    return SpriteTypeEnum.HeroMoveRight4;
                return SpriteTypeEnum.HeroMoveRight1;
            }
            return SpriteTypeEnum.HeroIdle;
        }

        protected override bool CanMove(DirectionEnum direction)
        {
            if (Direction == DirectionEnum.Up) // Exiting via Door
            {
                if (Maps.IsExitDoor(Room[X, Y - 1]) && Maps.IsExitDoor(Room[X + 1, Y - 1]))
                {
                    if (ChestCount == 0 && OnRoomFinished != null)
                    {
                        OnRoomFinished(this, new EventArgs());
                    }
                    else
                        return false;
                }
            }
            return base.CanMove(direction);
        }

        private void CorrectHeroStairsPosition(DirectionEnum direction)
        {
            if (direction == DirectionEnum.Down || direction == DirectionEnum.Up)
            {
                SpriteTypeEnum pos1 = Room[X, Y];
                SpriteTypeEnum pos2 = Room[X + 1, Y];
                if (direction == DirectionEnum.Down)
                {
                    pos1 = Room[X, Y + 1];
                    pos2 = Room[X + 1, Y + 1];
                }
                if (pos1 == SpriteTypeEnum.StairsRight)
                    X--;
                if (pos2 == SpriteTypeEnum.StairsLeft)
                    X++;
            }
        }

        protected override bool Move(DirectionEnum direction)
        {
            CorrectHeroStairsPosition(direction);
            return base.Move(direction);
        }

        public override void Update(GameTime gameTime)
        {
            if (Direction == DirectionEnum.Up || Direction == DirectionEnum.Down) // reset direction if going vertically
                Direction = DirectionEnum.None;

            if (Visible)
            {
                GamePadState gState = GamePad.GetState(PlayerIndex.One);
                KeyboardState kState = Keyboard.GetState();

                if (kState.IsKeyPressed(_oldKeyboardStateState, Keys.Left, Keys.NumPad4) ||
                    gState.IsButtonPressed(_oldGamePadState, Buttons.DPadLeft))
                    Direction = DirectionEnum.Left;
                else if (kState.IsKeyDown(Keys.Down, Keys.NumPad5) || gState.IsButtonDown(Buttons.DPadDown))
                    Direction = DirectionEnum.Down;
                else if (kState.IsKeyPressed(_oldKeyboardStateState, Keys.Right, Keys.NumPad6) ||
                         gState.IsButtonPressed(_oldGamePadState, Buttons.DPadRight))
                    Direction = DirectionEnum.Right;
                else if (kState.IsKeyDown(Keys.Up, Keys.NumPad8) || gState.IsButtonDown(Buttons.DPadUp))
                    Direction = DirectionEnum.Up;

                if (!_bullet.IsActive)
                {
                    if (kState.IsKeyPressed(_oldKeyboardStateState, Keys.Z, Keys.NumPad7) ||
                        gState.IsButtonPressed(_oldGamePadState, Buttons.LeftTrigger))
                    {
                        _shootStage = ShootStageEnum.Preparation;
                        _shootDirection = DirectionEnum.Left;
                    }
                    else if (kState.IsKeyPressed(_oldKeyboardStateState, Keys.X, Keys.NumPad9) ||
                             gState.IsButtonPressed(_oldGamePadState, Buttons.RightTrigger))
                    {
                        _shootStage = ShootStageEnum.Preparation;
                        _shootDirection = DirectionEnum.Right;
                    }
                }
                _oldKeyboardStateState = kState;
                _oldGamePadState = gState;
            }
            base.Update(gameTime);
        }

        protected override bool ProcessFalling()
        {
            bool isFalling = base.ProcessFalling();
            if (isFalling && _shootStage != ShootStageEnum.None)
            {
                Y--;
            }
            return isFalling;
        }

        private void CheckChest(int x, int y)
        {
            if (Maps.IsChest(Room[x, y]))
            {
                Room[x, y] = SpriteTypeEnum.Space;
                if (OnChestHappened != null)
                    OnChestHappened(this, new EventArgs());
            }
        }

        protected override void ActorUpdate(GameTime gameTime)
        {
            //UpdateControls(gameTime);
            CheckChest(X, Y);
            CheckChest(X + 1, Y);
            if (Maps.IsBiomass(Room[X, Y]) && Maps.IsBiomass(Room[X + 1, Y]))
            {
                IsInBiomass = true;
                Direction = DirectionEnum.None;
            }
            if (_shootStage != ShootStageEnum.None)
            {
                if (_shootStage == ShootStageEnum.Fire)
                {
                    if (_shootDirection == DirectionEnum.Right)
                        _bullet.Reset(X + 1, Y, _shootDirection);
                    else if (_shootDirection == DirectionEnum.Left)
                        _bullet.Reset(X, Y, _shootDirection);
                }
                if (Direction == DirectionEnum.Right)
                    X--;
                else if (Direction == DirectionEnum.Left)
                    X++;
            }
            base.ActorUpdate(gameTime);
        }

        private enum ShootStageEnum
        {
            None,
            Preparation,
            Fire
        }
    }
}