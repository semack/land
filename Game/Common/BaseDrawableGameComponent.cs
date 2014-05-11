using Microsoft.Xna.Framework;

namespace Land.Common
{
    public class BaseDrawableGameComponent : DrawableGameComponent
    {
        public BaseDrawableGameComponent(TheGame game)
            : base(game)
        {
        }

        public new TheGame Game
        {
            get { return base.Game as TheGame; }
        }

        public virtual void Show(bool value)
        {
            Enabled = value;
            Visible = value;
        }
    }
}