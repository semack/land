using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Land.Classes;
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
