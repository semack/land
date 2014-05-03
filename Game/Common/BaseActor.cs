using System;
using Land.Classes;
using Land.Components;
using Land.Enums;
using Microsoft.Xna.Framework;

namespace Land.Common
{
    public abstract class BaseActor : BaseDrawableGameComponent
    {
        private readonly int _speedCoef;

        private SpriteTypeEnum _heroSprite = SpriteTypeEnum.Space;
        private bool _isFalling;
        private TimeSpan _moveInterval;

        protected BaseActor(TheGame game, Room room, int speedCoef = 1)
            : base(game)
        {
            Room = room;
            Visible = false;
            Enabled = false;
            _speedCoef = speedCoef;
        }

        protected DirectionEnum Direction { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        protected Room Room { get; private set; }

        protected virtual void Reset(int x, int y)
        {
            X = x;
            Y = y;
            _isFalling = false;
            Direction = DirectionEnum.None;
            _moveInterval = new TimeSpan();
        }

        private bool HasNoStrongHold(SpriteTypeEnum sprite, bool isFalling = false)
        {
            bool result = Maps.IsBiomass(sprite)
                          || Maps.IsLiveWall(sprite)
                          || Maps.IsSpace(sprite)
                          || Maps.IsChest(sprite);
            if (isFalling && !result)
                result = Maps.IsFloor(sprite);
            return result;
        }

        protected virtual bool ProcessFalling()
        {
            SpriteTypeEnum cur1 = Room[X, Y];
            SpriteTypeEnum cur2 = Room[X + 1, Y];
            SpriteTypeEnum pos1 = Room[X, Y + 1];
            SpriteTypeEnum pos2 = Room[X + 1, Y + 1];

            if (_isFalling) // TODO допилить проверку! одна из смолы, падаем если смола под ногами.
            {
                _isFalling = HasNoStrongHold(pos1, true) && HasNoStrongHold(pos2, true)
                             && !(Maps.IsBiomass(cur1) && Maps.IsBiomass(cur2));
            }
            else
                _isFalling = HasNoStrongHold(pos1) && HasNoStrongHold(pos2)
                             && !(Maps.IsStairs(cur1) || Maps.IsStairs(cur2));

            if (_isFalling)
                Y++;
            return _isFalling;
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

        protected virtual bool CanMove(DirectionEnum direction)
        {
            bool result = false;
            switch (direction)
            {
                case DirectionEnum.Up:
                {
                    if (Y == 0)
                        break;
                    SpriteTypeEnum cur1 = Room[X, Y];
                    SpriteTypeEnum cur2 = Room[X + 1, Y];
                    SpriteTypeEnum pos1 = Room[X, Y - 1];
                    SpriteTypeEnum pos2 = Room[X + 1, Y - 1];
                    result = ((Maps.IsStairs(cur1) || Maps.IsStairs(cur2)) && !(Maps.IsWall(pos1) || Maps.IsWall(pos2)));
                    break;
                }
                case DirectionEnum.Down:
                {
                    if (Y == 16)
                        break;
                    SpriteTypeEnum pos1 = Room[X, Y + 1];
                    SpriteTypeEnum pos2 = Room[X + 1, Y + 1];
                    result = !(Maps.IsWall(pos1) || Maps.IsWall(pos2)) && !(Maps.IsFloor(pos1) || Maps.IsFloor(pos2));
                    break;
                }
                case DirectionEnum.Left:
                {
                    if (X == 0)
                        break;
                    SpriteTypeEnum pos = Room[X - 1, Y];
                    result = !Maps.IsWall(pos);
                    break;
                }
                case DirectionEnum.Right:
                {
                    if (X == 49)
                        break;
                    SpriteTypeEnum pos = Room[X + 2, Y];
                    result = !Maps.IsWall(pos);
                    break;
                }
            }
            if (result)
                CorrectHeroStairPosition();
            return result;
        }

        protected virtual void Move(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                {
                    if (CanMove(DirectionEnum.Left))
                        X--;
                    //else
                        Direction = DirectionEnum.None;
                    break;
                }
                case DirectionEnum.Right:
                {
                    if (CanMove(DirectionEnum.Right))
                        X++;
                    else
                        Direction = DirectionEnum.None;
                    break;
                }
                case DirectionEnum.Down:
                {
                    if (CanMove(DirectionEnum.Down))
                        Y++;
                    else
                        Direction = DirectionEnum.None;
                    break;
                }
                case DirectionEnum.Up:
                {
                    if (CanMove(DirectionEnum.Up))
                        Y--;
                    else
                        Direction = DirectionEnum.None;
                    break;
                }
            }
        }

        protected abstract SpriteTypeEnum GetSprite(bool isFalling, SpriteTypeEnum oldSprite);


        public override void Update(GameTime gameTime)
        {
            _moveInterval = _moveInterval - gameTime.ElapsedGameTime;
            if (_moveInterval.Ticks <= 0)
            {
                ActorUpdate(gameTime);
                _moveInterval = new TimeSpan(Game.GameSpeedScaleFactor*_speedCoef);
            }
            base.Update(gameTime);
        }

        protected virtual void ActorUpdate(GameTime gameTime)
        {
            ProcessFalling();
            if (!_isFalling)
                Move(Direction);
            else
                Direction = DirectionEnum.None;
            _heroSprite = GetSprite(_isFalling, _heroSprite);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw(Game.Sprites[_heroSprite, Game.BackColor].Texture, new Vector2(X*16, (Y + 2)*32),
                Color.White);
            Game.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}