using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
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
        private bool _isHeroCaught;
        private int _isHeroCaughtAnimation;
        public event EventHandler OnLifeFired;
        public DevilNumberEnum Number { get; private set; }

        public Devil(TheGame game, Room room, Hero hero, DevilNumberEnum number)
            : base(game, room, 3)
        {
            Number = number;
            _hero = hero;
        }

        public void Reset()
        {
            _isHeroCaughtAnimation = 8;
            _isHeroCaught = false;
            int x = 15;
            if (Number == DevilNumberEnum.Second)
                x = 30;
            base.Reset(x, 0);
        }

        protected override SpriteTypeEnum GetSprite(bool isFalling, SpriteTypeEnum oldSprite)
        {
            if (_isHeroCaught)
            {
                _hero.Visible = false;
                _isHeroCaughtAnimation--;
                if (_isHeroCaughtAnimation == 0)
                {
                    _hero.Visible = true;
                    if (OnLifeFired != null)
                        OnLifeFired(this, new EventArgs());
                }
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
            Direction = DirectionEnum.None;
            if (!_isHeroCaught)
            {
                if (X > _hero.X && CanMove(DirectionEnum.Left))
                    Direction = DirectionEnum.Left;
                else if (X < _hero.X && CanMove(DirectionEnum.Right))
                    Direction = DirectionEnum.Right;

                if (Y > _hero.Y && CanMove(DirectionEnum.Up))
                    Direction = DirectionEnum.Up;
                else if (Y < _hero.Y && CanMove(DirectionEnum.Down))
                    Direction = DirectionEnum.Down;
            }
            base.Update(gameTime);
        }

        protected override void ActorUpdate(GameTime gameTime)
        {
            if (Maps.IsBiomass(Room[X, Y]) && Maps.IsBiomass(Room[X + 1, Y]))
                Reset();
            else if ((_hero.X == X || _hero.X + 1 == X || _hero.X + 1 == X || _hero.X + 1 == X + 1)
                && (_hero.Y == Y || _hero.Y + 1 == Y || _hero.Y + 1 == Y || _hero.Y + 1 == Y + 1))
            {
                _isHeroCaught = true;
            }
            base.ActorUpdate(gameTime);
        }
    }
}
