using System;
using Land.Classes;
using Land.Components;
using Land.Enums;
using Land.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Land
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class TheGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly Room _room;
        private readonly Splash _splash;
        private KeyboardState _oldKeyState;

        public TheGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = (Maps.CapacityY + 2)*32,
                PreferredBackBufferWidth = Maps.CapacityX*16,
            };
            Content.RootDirectory = "Content";
            MapBank = 1;
            Range = 3;
            BackColor = BackColorEnum.Black;
            _splash = new Splash(this);
            _splash.OnPlayingStarted += OnPlayingStarted;

            _room = new Room(this);
            _room.OnPlayingFinished += OnRoomPlayingFinished;
            OnRoomPlayingFinished(this, new EventArgs());
        }

        public int MapBank { get; private set; }
        public int Range { get; private set; }
        public BackColorEnum BackColor { get; set; }
        public Color BackgroundColor
        {
            get { return BackColor == BackColorEnum.White ? Color.White : Color.Black; }
        }

        public Color ForegroudColor
        {
            get { return BackColor == BackColorEnum.White ? Color.Black : Color.White; }
        }

        public int GameSpeedScaleFactor
        {
            get { return 300000 + Range*250000; }
        }

        public GameSpritesCollection Sprites { get; private set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public SpriteFont InfoFont { get; private set; }
        public SpriteFont GameFont { get; private set; }

        private void OnRoomPlayingFinished(object sender, EventArgs e)
        {
            _room.Show(false);
            _splash.Show(true);
        }

        private void OnPlayingStarted(object sender, PlayingStartedEventArgs e)
        {
            _splash.Show(false);
            _splash.Visible = false;
            Range = e.Range;
            _room.Reset();
            _room.Show(true);
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Components.Add(_splash);
            Components.Add(_room);
            base.Initialize();
        }


        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Sprites = new GameSpritesCollection(Content);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            InfoFont = Content.Load<SpriteFont>("Graphics/Fonts/InfoFont");
            GameFont = Content.Load<SpriteFont>("Graphics/Fonts/GameFont");
        }

        public void DrawScores(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprites[SpriteTypeEnum.ScoreLabel, BackColor].Texture, new Vector2(1 * 16, 0), BackgroundColor == Color.White ? Color.Black : Color.White);
            spriteBatch.DrawString(GameFont, string.Format("{0:D5}", _room.Score), new Vector2(7 * 16, 0), BackgroundColor == Color.White ? Color.Black : Color.White);
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyPressed(_oldKeyState, Keys.F12))
            {
                _graphics.ToggleFullScreen();
                _graphics.ApplyChanges();
            }
            else if (state.IsKeyPressed(_oldKeyState, Keys.F10))
            {
                
                MapBank++;
                if (MapBank > Maps.Banks.Count-1)
                    MapBank = 0;
                OnRoomPlayingFinished(this, null);
                _room.Score = 0;
            }
            base.Update(gameTime);
            _oldKeyState = state;
        }
    }
}