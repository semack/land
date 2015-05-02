using Land.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace Land.Classes
{
    public class GameSprite
    {
        public GameSprite(SpriteTypeEnum itemType, Texture2D texture, BackColorEnum backColor)
        {
            ItemType = itemType;
            Texture = texture;
            BackColor = backColor;
        }

        public SpriteTypeEnum ItemType { get; private set; }
        public Texture2D Texture { get; private set; }
        public BackColorEnum BackColor { get; private set; }
    }
}