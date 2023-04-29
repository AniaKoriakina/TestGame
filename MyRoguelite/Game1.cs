using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Windows.Input;

namespace MyRoguelite
{
    public class Game1 : Game, IView
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public event EventHandler CycleFinished = delegate { };
        public event EventHandler<ControlsEventArgs> PlayerMoved = delegate { };

        private Vector2 _playerPos = Vector2.Zero;
        private Texture2D _playerImage;

        int frameWidth = 95;
        int frameHeight = 120;
        Point currentFrame = new Point(0, 0);
        Point spriteSizeMoveLeft = new Point(6, 1);
        Texture2D texture;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerImage = Content.Load<Texture2D>("WitchPlayer");
            texture = Content.Load<Texture2D>("frameLeft");

            Song backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
            base.LoadContent();
            // TODO: use this.Content to load your game content here
        }

        public void LoadParameters(Vector2 playerPos)
        {
            _playerPos = playerPos;
        }

        protected override void Update(GameTime gameTime)
        {
            MovePlayer();
            base.Update(gameTime);
            CycleFinished.Invoke(this, new EventArgs());
        }

        public void MovePlayer()
        {
            var keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Length > 0)
            {
                bool isWPressed = keys.Contains(Keys.W);
                bool isSPressed = keys.Contains(Keys.S);
                bool isDPressed = keys.Contains(Keys.D);
                bool isAPressed = keys.Contains(Keys.A);

                if (isWPressed && isDPressed)
                {
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.upRight });
                }
                else if (isWPressed && isAPressed)
                {
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.upLeft });
                }
                else if (isSPressed && isAPressed)
                {
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.downLeft });
                }
                else if (isSPressed && isDPressed)
                {
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.downRight });
                }
                else
                {
                    var k = keys[0];
                    switch (k)
                    {
                        case Keys.W:
                            {
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.up });
                                break;
                            }
                        case Keys.S:
                            {
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.down });
                                break;
                            }
                        case Keys.D:
                            {
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.right });
                                break;
                            }
                        case Keys.A:
                            {
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.left });
                                break;
                            }

                        case Keys.Escape:
                            {
                                Exit();
                                break;
                            }
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Bisque);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_playerImage, _playerPos, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);


        }
    }
}