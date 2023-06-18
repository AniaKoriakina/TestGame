using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
using MyRoguelite.Model;
using MyRoguelite.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Screen = System.Windows.Forms.Screen;

namespace MyRoguelite.View
{
    public class Game1 : Game, IView
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private float currentTime = 0f;

        public event EventHandler CycleFinished = delegate { };
        public event EventHandler<ControlsEventArgs> PlayerMoved = delegate { };

        private AnimatedSprite _moveSprite;
        private AnimatedSprite _mapFrame;
        private AnimatedSprite _enemySprite;
        private AnimatedSprite _bulletSprite;

        private Texture2D background;


        private Vector2 _visualShift = new Vector2(0, 0);

        private Dictionary<int, IObject> _objects = new Dictionary<int, IObject>();
        public Dictionary<int, AnimatedSprite> _textures = new Dictionary<int, AnimatedSprite>();

        public static SpriteFont Font { get; set; }
        public static SpriteFont FontEnemy { get; set; }

        GameCycle gameCycle;
        public static GameTime GameTime { get; private set; }

        public void SetupGame()
        {
            Initialize();
            LoadContent();
        }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.IsFullScreen = false;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameCycle = new GameCycle();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var spriteSheet = Content.Load<SpriteSheet>("AllframesPlayer.sf", new JsonContentLoader());
            var sprite = new AnimatedSprite(spriteSheet);
            sprite.Play("idle");
            _moveSprite = sprite;
            _textures.Add(1, _moveSprite);

            var mapFrame = Content.Load<SpriteSheet>("MapFrame.sf", new JsonContentLoader());
            var mapSprite = new AnimatedSprite(mapFrame);
            mapSprite.Play("floor");
            _mapFrame = mapSprite;
            _textures.Add(2, _mapFrame);

            var enemy = Content.Load<SpriteSheet>("enemyObj.sf", new JsonContentLoader());
            var enemySprite = new AnimatedSprite(enemy);
            enemySprite.Play("enemySprite");
            _enemySprite = enemySprite;
            _textures.Add(3, _enemySprite);

            Font = Content.Load<SpriteFont>("Health");
            FontEnemy = Content.Load<SpriteFont>("Health");

            var bullet = Content.Load<SpriteSheet>("bulletFrame.sf", new JsonContentLoader());
            var bulletSprite = new AnimatedSprite(bullet);
            bulletSprite.Play("bullet");
            _bulletSprite = bulletSprite;
            _textures.Add(4, _bulletSprite);

            background = Content.Load<Texture2D>("background");

            //SoundEffect backgroundSlow = Content.Load<SoundEffect>("backgroundSlow");
            //SoundEffectInstance backgroundSlowInstance = backgroundSlow.CreateInstance();
            //backgroundSlowInstance.IsLooped = true;
            //backgroundSlowInstance.Play();


            Song backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

            base.LoadContent();
        }

        public void LoadGCParameters(Dictionary<int, IObject> Objects, Vector2 POVShift)
        {
            _objects = Objects;
            _visualShift += POVShift;
        }


        protected override void Update(GameTime gameTime)
        {

            Game1.GameTime = gameTime;
            //gameCycle.Update();
            MovePlayer(gameTime);
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
            CycleFinished.Invoke(this, new EventArgs());
        }
        private string DisplayTime()
        {
            int minutes = (int)Math.Floor(currentTime / 60f);
            int seconds = (int)Math.Floor(currentTime % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }


        public void MovePlayer(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var animation = "idle";
            bool isWPressed = keys.Contains(Keys.W);
            bool isSPressed = keys.Contains(Keys.S);
            bool isDPressed = keys.Contains(Keys.D);
            bool isAPressed = keys.Contains(Keys.A);
            bool isEscPressed = keys.Contains(Keys.Escape);
            var directionKey = (isWPressed, isAPressed, isSPressed, isDPressed);
            var moveDirections = new Dictionary<(bool, bool, bool, bool), (string, IModel.Direction)> {
                { (false, false, true, true), ("moveRight", IModel.Direction.downRight) },
                { (true, true, false, false), ("moveLeft", IModel.Direction.upLeft) },
                { (false, true, true, false), ("moveLeft", IModel.Direction.downLeft) },
                { (true, false, false, true), ("moveRight", IModel.Direction.upRight) },
                { (true, false, false, false), ("moveUp", IModel.Direction.up) },
                { (false, false, true, false), ("moveDown", IModel.Direction.down) },
                { (false, true, false, false), ("moveLeft", IModel.Direction.left) },
                { (false, false, false, true),("moveRight", IModel.Direction.right) }};

            if (moveDirections.TryGetValue(directionKey, out var moveDirection))
            {
                animation = moveDirection.Item1;
                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = moveDirection.Item2 });
            }
            else if (isEscPressed) Exit();
            

            _moveSprite.Play(animation);
            _moveSprite.Update(deltaSeconds);
        }

        private string currentHealthText = "";
        private string enemyCountText = "";
        private string currentTimeText = "";

        protected override void Draw(GameTime gameTime)
        {
            currentHealthText = GameCycle.HealthText;
            enemyCountText = GameCycle.EnemyCountText;
            currentTimeText = DisplayTime();

            _graphics.GraphicsDevice.Clear(Color.DarkSlateGray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            

            _spriteBatch.Draw(background, new Vector2(0,0),Color.White);

            foreach (var o in _objects.Values)
            {
                _spriteBatch.Draw(_textures[o.ImageId], o.Pos - _visualShift);
            }
            _spriteBatch.DrawString(Font, currentHealthText, new Vector2(50, 20), Color.White);
            if (Font != null && enemyCountText != null)
                _spriteBatch.DrawString(Font, enemyCountText, new Vector2(400, 20), Color.White);
            _spriteBatch.DrawString(Font, currentTimeText, new Vector2(750, 20), Color.White);

            gameCycle.SetObjects(_objects);
            gameCycle.DrawHealthBars(_spriteBatch);


            _spriteBatch.End();

            base.Draw(gameTime);


        }
    }
}
