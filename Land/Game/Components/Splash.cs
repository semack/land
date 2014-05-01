using System;
using System.Text;
using Land.Classes;
using Land.Common;
using Land.Enums;
using Land.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Land.Components
{
    public class PlayingStartedEventArgs : EventArgs
    {
        public int? Range { get; set; }
    }

    public class Splash : BaseDrawableGameComponent
    {
        private readonly StringBuilder _infoText = new StringBuilder();
        private readonly Vector2 _infoVector = new Vector2(2 * 16, 17 * 32);
        private readonly Vector2 _logoVector = new Vector2(4 * 16, 1 * 32);
        private BackColorEnum _backColor = BackColorEnum.Black;
        private DisplayModeEnum _displayMode = DisplayModeEnum.Splash;
        private KeyboardState _oldKeyState;
        private TimeSpan _splashInterval;

        public Splash(TheGame game)
            : base(game)
        {
            _infoText.Append(
                "This is the port of the formely popular game LAND. It was originally developed by ASP corp. on 1986 and ran under PDP-11 compatible computers.\r\n");
            _infoText.Append(
                "Ported to Windows using XNA framework 4.0 in April 2014 by Andriy S'omak (semack@gmail.com). Sourcecode at https://github.com/semack/land");
        }

        private Color BackColor
        {
            get { return _backColor == BackColorEnum.White ? Color.White : Color.Black; }
        }

        public event EventHandler<PlayingStartedEventArgs> OnPlayingStarted;


        private void UpdateSplash(KeyboardState state, GameTime gameTime)
        {
            if (state.IsKeyPressed(_oldKeyState, Keys.Enter, Keys.Space, Keys.Escape, Keys.D0, Keys.D1, Keys.D2, Keys.D3,
                Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9))
            {
                _displayMode = DisplayModeEnum.GameStart;
                _splashInterval = new TimeSpan(Game.GameSpeedScaleFactor * 20);
            }

            _splashInterval = _splashInterval - gameTime.ElapsedGameTime;
            if (_splashInterval.Ticks < 0)
            {
                _backColor = (_backColor == BackColorEnum.White ? BackColorEnum.Black : BackColorEnum.White);
                _splashInterval = new TimeSpan(Game.GameSpeedScaleFactor * 10);
            }
        }

        private void UpdateGameStart(KeyboardState state, GameTime gameTime)
        {
            if (state.IsKeyPressed(_oldKeyState, Keys.Enter, Keys.Space, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
                Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9))
            {
                _displayMode = DisplayModeEnum.Splash;
                int? range = null;
                string value = Encoding.ASCII.GetString(new[] { (byte)state.GetPressedKeys()[0] });
                int newRange;
                if (int.TryParse(value, out newRange))
                    range = newRange;
                if (OnPlayingStarted != null)
                {
                    Game.BackColor = _backColor;
                    OnPlayingStarted(this, new PlayingStartedEventArgs
                    {
                        Range = range
                    });
                }
                return;
            }
            _splashInterval = _splashInterval - gameTime.ElapsedGameTime;
            if (_splashInterval.Ticks < 0)
            {
                _displayMode = DisplayModeEnum.Splash;
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (_displayMode == DisplayModeEnum.Splash)
            {
                UpdateSplash(state, gameTime);
            }
            else
            {
                UpdateGameStart(state, gameTime);
            }
            base.Update(gameTime);
            _oldKeyState = state;
        }

        public void DrawSplash(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.Splash, _backColor].Texture, _logoVector, Color.White);
            spriteBatch.DrawString(Game.InfoFont, _infoText, _infoVector,
                BackColor == Color.White ? Color.Black : Color.White);
        }

        public void DrawGameStart(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Maps.CapacityX; i++)
            {
                for (int j = 0; j < Maps.CapacityY + 2; j++)
                {
                    if (i == 0 || i == Maps.CapacityX - 1)
                        spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.StoneWall, _backColor].Texture,
                            new Vector2(i * 16, j * 32), Color.White);
                    if (j == 0 || j == Maps.CapacityY + 2 - 1)
                        spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.StoneWall, _backColor].Texture,
                            new Vector2(i * 16, j * 32), Color.White);
                }
            }
            spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.YourRangeLabel, _backColor].Texture, new Vector2(20 * 16, 8 * 32),
                Color.White);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackColor);
            Game.SpriteBatch.Begin();
            if (_displayMode == DisplayModeEnum.Splash)
                DrawSplash(Game.SpriteBatch);
            else
                DrawGameStart(Game.SpriteBatch);
            Game.SpriteBatch.End();
            base.Draw(gameTime);
        }

        private enum DisplayModeEnum
        {
            Splash,
            GameStart
        }
    }
}