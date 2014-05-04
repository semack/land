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
        private KeyboardState _oldState;
        private DirectionEnum _shootDirection;
        private ShootStageEnum _shootStage;

        public Hero(TheGame game, Room room, Bullet bullet)
            : base(game, room)
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


        private void CorrectHeroStairPosition()
        {
            if (Direction == DirectionEnum.Down || Direction == DirectionEnum.Up)
            {
                SpriteTypeEnum cur1 = Room[X, Y];
                SpriteTypeEnum cur2 = Room[X + 1, Y];
                if (cur1 == SpriteTypeEnum.StairsRight)
                    X--;
                if (cur2 == SpriteTypeEnum.StairsLeft)
                    X++;
            }
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
                }
            }
            bool result = base.CanMove(direction);
            if (result)
                CorrectHeroStairPosition();
            return result;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyPressed(_oldState, Keys.Left, Keys.NumPad4))
                Direction = DirectionEnum.Left;
            else if (state.IsKeyPressed(_oldState, Keys.Down, Keys.NumPad5))
                Direction = DirectionEnum.Down;
            else if (state.IsKeyPressed(_oldState, Keys.Right, Keys.NumPad6))
                Direction = DirectionEnum.Right;
            else if (state.IsKeyPressed(_oldState, Keys.Up, Keys.NumPad8))
                Direction = DirectionEnum.Up;

            if (!_bullet.IsActive)
            {
                if (state.IsKeyPressed(_oldState, Keys.Z, Keys.NumPad7))
                {
                    _shootStage = ShootStageEnum.Preparation;
                    _shootDirection = DirectionEnum.Left;
                }
                else if (state.IsKeyPressed(_oldState, Keys.X, Keys.NumPad9))
                {
                    _shootStage = ShootStageEnum.Preparation;
                    _shootDirection = DirectionEnum.Right;
                }
            }

            if (Direction == DirectionEnum.Up || Direction == DirectionEnum.Down)
            {
                if (state.GetPressedKeys().Length == 0)
                    Direction = DirectionEnum.None;
            }

            base.Update(gameTime);
            _oldState = state;
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