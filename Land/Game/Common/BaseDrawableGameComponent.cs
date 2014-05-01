using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Land.Classes;
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
        public virtual void Show(bool value = true)
        {
            Enabled = value;
            Visible = value;
        }

    }
}
