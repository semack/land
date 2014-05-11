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

        private int _x { get; set; }
        private int _y { get; set; }

        public bool IsActive
        {
            get { return _direction != DirectionEnum.None; }
        }

        public void Reset(int x = 1, int y = 1, DirectionEnum direction = DirectionEnum.None)
        {
            _x = x;
            _y = y;
            _direction = direction;
        }

        public void Move()
        {
            if (_direction != DirectionEnum.None)
            {
                int delta = 1;
                if (_direction == DirectionEnum.Left)
                    delta = -1;
                if (!Maps.IsWall(_room[_x + delta, _y]))
                {
                    _x = _x + delta;
                }
                else
                {
                    if (Maps.IsBrickWall(_room[_x + delta, _y]))
                    {
                        _room[_x + delta, _y] = SpriteTypeEnum.WallLive1;
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
                    new Vector2(_x*16, (_y + 2)*32), Color.White);
                Game.SpriteBatch.End();
                base.Draw(gameTime);
            }
        }
    }
}