using System;
using Land.Classes;
using Land.Common;
using Land.Components.Actors;
using Land.Enums;
using Land.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Land.Components
{
    public class Room : BaseDrawableGameComponent
    {
        private readonly Biomass _biomass;
        private readonly Bullet _bullet;
        private readonly Devil _devil1;
        private readonly Devil _devil2;
        private readonly Hero _hero;
        private readonly Wall _wall;
        private int _attempts;
        private SpriteTypeEnum[,] _map;
        private KeyboardState _oldKeyState;
        private int _range;
        private int _score;
        private int _stage;

        public Room(TheGame game)
            : base(game)
        {
            _bullet = new Bullet(Game, this);
            _hero = new Hero(Game, this, _bullet);
            _hero.OnChestHappened += OnHeroChestHappened;
            _hero.OnLifeFired += OnHeroLifeFired;
            _hero.OnRoomFinished += OnHeroRoomFinished;
            _biomass = new Biomass(Game, this);
            _wall = new Wall(Game, this);
            _devil1 = new Devil(Game, this, _hero, DevilNumberEnum.First);
            _devil2 = new Devil(Game, this, _hero, DevilNumberEnum.Second);
            _devil1.OnLifeFired += OnHeroLifeFired;
            _devil2.OnLifeFired += OnHeroLifeFired;
            Reset(null);
        }

        public SpriteTypeEnum this[int x, int y]
        {
            get { return _map[x, y]; }
            set { _map[x, y] = value; }
        }

        private Color BackColor
        {
            get { return Game.BackColor == BackColorEnum.White ? Color.White : Color.Black; }
        }

        public event EventHandler OnPlayingFinished;

        private void OnHeroRoomFinished(object sender, EventArgs e)
        {
            SetNextStage();
        }

        private void OnHeroLifeFired(object sender, EventArgs e)
        {
            _attempts--;
            if (_attempts <= 0)
            {
                if (OnPlayingFinished != null)
                    OnPlayingFinished(this, new EventArgs());
            }
            else
                SetStage(_stage);
        }

        private void OnHeroChestHappened(object sender, EventArgs e)
        {
            _score += 13;
            if (_score > 99999)
                _score = 0;
        }

        public void Reset(int? range)
        {
            _score = 0;
            _stage = 1;
            _attempts = 20;
            if (range != null)
                _range = (int)range;
            SetStage(_stage);
        }


        public override void Initialize()
        {
            Game.Components.Add(_hero);
            Game.Components.Add(_biomass);
            Game.Components.Add(_bullet);
            Game.Components.Add(_wall);
            Game.Components.Add(_devil1);
            Game.Components.Add(_devil2);
            SetStage(_stage);
            base.Initialize();
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            _hero.Show(Enabled);
            _biomass.Enabled = Enabled;
            _bullet.Show(Enabled);
            _wall.Enabled = Enabled;
            _devil1.Show(Enabled);
            _devil2.Show(Enabled);
            base.OnEnabledChanged(sender, args);
        }

        private void SetStage(int stage)
        {
            _stage = stage;
            _map = Maps.Get(_stage);
            _hero.Reset();
            _bullet.Reset(1, 1, DirectionEnum.None);
            _devil1.Reset();
            _devil2.Reset();
            int i = Maps.MapsCount;
        }

        private void SetNextStage()
        {
            if (_stage >= Maps.MapsCount)
                _stage = 1;
            else
                _stage++;
            SetStage(_stage);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyPressed(_oldKeyState, Keys.Q))
            {
                if (OnPlayingFinished != null)
                    OnPlayingFinished(this, new EventArgs());
            }
            if (state.IsKeyPressed(_oldKeyState, Keys.OemSemicolon))
                SetNextStage();
            _oldKeyState = state;
        }


        private void DrawInfoPanel(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.ScoreLabel, Game.BackColor].Texture, new Vector2(1 * 16, 0),
                Color.White);
            spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.RangeLabel, Game.BackColor].Texture, new Vector2(16 * 16, 0),
                Color.White);
            spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.AttemptsLabel, Game.BackColor].Texture, new Vector2(27 * 16, 0),
                Color.White);
            spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.StageLabel, Game.BackColor].Texture, new Vector2(42 * 16, 0),
                Color.White);
            for (int i = 0; i < Maps.CapacityX; i++)
            {
                spriteBatch.Draw(Game.Sprites[SpriteTypeEnum.Delimiter, Game.BackColor].Texture, new Vector2(i * 16, 1 * 32),
                    Color.White);
            }

            spriteBatch.DrawString(Game.GameFont, string.Format("{0:D5}", _score), new Vector2(7 * 16, 0),
                BackColor == Color.White ? Color.Black : Color.White);
            spriteBatch.DrawString(Game.GameFont, string.Format("{0:D2}", _range), new Vector2(22 * 16, 0),
                BackColor == Color.White ? Color.Black : Color.White);
            spriteBatch.DrawString(Game.GameFont, string.Format("{0:D2}", _attempts), new Vector2(36 * 16, 0),
                BackColor == Color.White ? Color.Black : Color.White);
            spriteBatch.DrawString(Game.GameFont, string.Format("{0:D2}", _stage), new Vector2(46 * 16, 0),
                BackColor == Color.White ? Color.Black : Color.White);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackColor);

            Game.SpriteBatch.Begin();
            DrawInfoPanel(Game.SpriteBatch);
            for (int x = 0; x < Maps.CapacityX; x++)
            {
                for (int y = 0; y < Maps.CapacityY; y++)
                {
                    SpriteTypeEnum item = this[x, y];
                    Game.SpriteBatch.Draw(Game.Sprites[item, Game.BackColor].Texture, new Vector2(x * 16, (y + 2) * 32),
                        Color.White);
                }
            }
            Game.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}