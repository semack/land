using System;
using Land.Classes;
using Land.Common;
using Land.Enums;
using Microsoft.Xna.Framework;

namespace Land.Components.Actors
{
    public enum DevilNumberEnum
    {
        First,
        Second
    }

    public class Devil : BaseActor
    {
        private readonly Hero _hero;
        private DirectionEnum _horizontalDirection = DirectionEnum.None;
        public bool HasCaught { get; set; }

        private int _isHeroCaughtAnimation;

        private DirectionEnum _verticalDirection = DirectionEnum.None;

        public Devil(TheGame game, Room room, Hero hero, DevilNumberEnum number)
            : base(game, room, 3)
        {
            Number = number;
            _hero = hero;
        }

        public DevilNumberEnum Number { get; private set; }
        public event EventHandler OnLifeFired;

        public void Reset()
        {
            _isHeroCaughtAnimation = 10;
            HasCaught = false;
            int x = 15;
            if (Number == DevilNumberEnum.Second)
                x = 30;
            base.Reset(x, 0);
            _verticalDirection = DirectionEnum.None;
            _horizontalDirection = DirectionEnum.None;
        }

        protected override SpriteTypeEnum GetSprite(bool isFalling, SpriteTypeEnum oldSprite)
        {
            if (HasCaught && !_hero.IsInBiomass)
            {
                _isHeroCaughtAnimation--;
                if (_isHeroCaughtAnimation == 0 && OnLifeFired != null)
                        OnLifeFired(this, new EventArgs());

                if (oldSprite == SpriteTypeEnum.DevilStairsLeft)
                    return SpriteTypeEnum.DevilStairsRight;
                return SpriteTypeEnum.DevilStairsLeft;
            }
            if (isFalling || Direction == DirectionEnum.Up || Direction == DirectionEnum.Down)
            {
                if (oldSprite == SpriteTypeEnum.DevilStairsLeft)
                    return SpriteTypeEnum.DevilStairsRight;
                return SpriteTypeEnum.DevilStairsLeft;
            }
            if (Direction == DirectionEnum.Left)
            {
                if (oldSprite == SpriteTypeEnum.DevilMoveLeft1)
                    return SpriteTypeEnum.DevilMoveLeft2;
                if (oldSprite == SpriteTypeEnum.DevilMoveLeft2)
                    return SpriteTypeEnum.DevilMoveLeft3;
                if (oldSprite == SpriteTypeEnum.DevilMoveLeft3)
                    return SpriteTypeEnum.DevilMoveLeft4;
                if (oldSprite == SpriteTypeEnum.DevilMoveLeft4)
                    return SpriteTypeEnum.DevilMoveLeft5;
                return SpriteTypeEnum.DevilMoveLeft1;
            }
            if (Direction == DirectionEnum.Right)
            {
                if (oldSprite == SpriteTypeEnum.DevilMoveRight1)
                    return SpriteTypeEnum.DevilMoveRight2;
                if (oldSprite == SpriteTypeEnum.DevilMoveRight2)
                    return SpriteTypeEnum.DevilMoveRight3;
                if (oldSprite == SpriteTypeEnum.DevilMoveRight3)
                    return SpriteTypeEnum.DevilMoveRight4;
                if (oldSprite == SpriteTypeEnum.DevilMoveRight4)
                    return SpriteTypeEnum.DevilMoveRight5;
                return SpriteTypeEnum.DevilMoveRight1;
            }
            if (oldSprite == SpriteTypeEnum.DevilStairsLeft || oldSprite == SpriteTypeEnum.DevilStairsRight)
                return SpriteTypeEnum.DevilMoveLeft1;

            return oldSprite;
        }


        public override void Update(GameTime gameTime)
        {
            _horizontalDirection = DirectionEnum.None;
            _verticalDirection = DirectionEnum.None;
            if (!HasCaught)
            {
                if (X > _hero.X)
                    _horizontalDirection = DirectionEnum.Left;
                else if (X < _hero.X)
                    _horizontalDirection = DirectionEnum.Right;
                if (Y > _hero.Y)
                    _verticalDirection = DirectionEnum.Up;
                else if (Y < _hero.Y)
                    _verticalDirection = DirectionEnum.Down;

                if (Maps.IsBiomass(Room[X, Y]) && Maps.IsBiomass(Room[X + 1, Y]))
                    Reset();
            }


            base.Update(gameTime);
        }

        protected override bool ProcessFalling()
        {
            if (!HasCaught)
                return base.ProcessFalling();
            return false;
        }

        protected override bool Move(DirectionEnum direction)
        {
            // devil movements related to original game
            if (!base.Move(_horizontalDirection))
                _horizontalDirection = DirectionEnum.None;
            if (!base.Move(_verticalDirection))
                _verticalDirection = DirectionEnum.None;
            Direction = _verticalDirection != DirectionEnum.None ? _verticalDirection : _horizontalDirection;
            return true;
        }
    }
}