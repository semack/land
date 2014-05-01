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
    public class Biomass : BaseGameComponent
    {
        private TimeSpan _moveInterval;
        private const int SpeedCoef = 2;
        private readonly Room _room;

        public Biomass(TheGame game, Room room)
            : base(game)
        {
            _moveInterval = new TimeSpan();
            _room = room;
        }

        protected SpriteTypeEnum GetSprite(SpriteTypeEnum oldSprite)
        {
            if (oldSprite == SpriteTypeEnum.Biomass1)
                return SpriteTypeEnum.Biomass2;
            if (oldSprite == SpriteTypeEnum.Biomass2)
                return SpriteTypeEnum.Biomass3;
            if (oldSprite == SpriteTypeEnum.Biomass3)
                return SpriteTypeEnum.Biomass4;
            if (oldSprite == SpriteTypeEnum.Biomass4)
                return SpriteTypeEnum.Biomass1;
            return oldSprite;
        }

        private bool CanMoveLeft(SpriteTypeEnum sprite)
        {
            return CanMoveBottom(sprite) && !Maps.IsBiomass(sprite);
        }

        private bool CanMoveBottom(SpriteTypeEnum sprite)
        {
            return !Maps.IsStairs(sprite) && !Maps.IsWall(sprite) && !Maps.IsChest(sprite) && !Maps.IsFloor(sprite);
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
                        if (Maps.IsBiomass(_room[x, y]))
                        {
                            _room[x, y] = GetSprite(_room[x, y]); // next sprite
                            if (CanMoveBottom(_room[x, y + 1])) // bottom
                            {
                                _room[x, y + 1] = _room[x, y];
                                _room[x, y] = SpriteTypeEnum.Space;
                            }
                            else if (CanMoveLeft(_room[x - 1, y])) // left
                            {
                                _room[x - 1, y] = _room[x, y];
                                _room[x, y] = SpriteTypeEnum.Space;
                            }

                        }
                    }

                }
                _moveInterval = new TimeSpan(Game.GameSpeedScaleFactor * SpeedCoef);
            }
        }
    }
}
