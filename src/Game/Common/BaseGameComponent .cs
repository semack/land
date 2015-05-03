using Land.Enums;
using Microsoft.Xna.Framework;

namespace Land.Common
{
    public class BaseGameComponent : GameComponent
    {
        public BaseGameComponent(TheGame game)
            : base(game)
        {
        }

        public new TheGame Game
        {
            get { return base.Game as TheGame; }
        }
    }
}