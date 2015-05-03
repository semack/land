using System;
using Land.Classes;
using Land.Common;
using Land.Enums;
using Microsoft.Xna.Framework;

namespace Land.Components.Actors
{
    public class Bullet : BaseDrawableGameComponent
    {
        private const int SpeedCoef = 1;
        private readonly Room _room;
        private DirectionEnum _direction = DirectionEnum.None;
        private TimeSpan _moveInterval;

        public Bullet(TheGame game, Room room)
            : base(game)
        {
            _room = room;
        }

        private int X { get; set; }
        private int Y { get; set; }

        public bool IsActive
        {
            get { return _direction != DirectionEnum.None; }
        }

        public void Reset(int x, int y, DirectionEnum direction)
        {
            X = x;
            Y = y;
            _direction = direction;
        }

        public void Move()
        {
            if (_direction != DirectionEnum.None)
            {
                int delta = 1;
                if (_direction == DirectionEnum.Left)
                    delta = -1;
                if (!Maps.IsWall(_room[X + delta, Y]))
                {
                    X = X + delta;
                }
                else
                {
                    if (Maps.IsBrickWall(_room[X + delta, Y]))
                    {
                        _room[X + delta, Y] = SpriteTypeEnum.WallLive1;
                    }
                    _direction = DirectionEnum.None;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            _moveInterval = _moveInterval - gameTime.ElapsedGameTime;
            if (_moveInterval.Ticks <= 0)
            {
                Move();
                _moveInterval = new TimeSpan(Game.GameSpeedScaleFactor*SpeedCoef);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_direction != DirectionEnum.None)
            {
                Game.SpriteBatch.Begin();
                Game.SpriteBatch.Draw(Game.Sprites[SpriteTypeEnum.Bullet, Game.BackColor].Texture,
                    new Vector2(X*16, (Y + 2)*32), Color.White);
                Game.SpriteBatch.End();
                base.Draw(gameTime);
            }
        }
    }
}