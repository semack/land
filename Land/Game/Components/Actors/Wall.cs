using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Land.Classes;
using Land.Common;
using Land.Enums;
using Microsoft.Xna.Framework;

namespace Land.Components.Actors
{
    public class Wall : BaseGameComponent
    {
        private TimeSpan _moveInterval;
        private const int SpeedCoef = 70;
        private readonly Room _room;

        public Wall(TheGame game, Room room)
            : base(game)
        {
            _moveInterval = new TimeSpan();
            _room = room;
        }

        protected SpriteTypeEnum GetSprite(SpriteTypeEnum oldSprite)
        {
            if (oldSprite == SpriteTypeEnum.WallLive1)
                return SpriteTypeEnum.WallLive2;
            if (oldSprite == SpriteTypeEnum.WallLive2)
                return SpriteTypeEnum.WallLive3;
            if (oldSprite == SpriteTypeEnum.WallLive3)
                return SpriteTypeEnum.BrickWall;
            return oldSprite;
        }



        public override void Update(GameTime gameTime)
        {
            _moveInterval = _moveInterval - gameTime.ElapsedGameTime;
            if (_moveInterval.Ticks <= 0)
            {
                for (int x = 0; x < Maps.CapacityX; x++)
                {
                    for (int y = 0; y < Maps.CapacityY; y++)
                    {
                        if (_room[x,y] >= SpriteTypeEnum.WallLive1 && _room[x,y] <= SpriteTypeEnum.WallLive3)
                        {
                            _room[x, y] = GetSprite(_room[x, y]);
                        }
                    }
                }
                _moveInterval = new TimeSpan(Game.GameSpeedScaleFactor * SpeedCoef);
            }
        }
    }
}
