using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
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

        private AnimatedSprite _moveSprite;


        //  int frameWidth = 95;
        //  int frameHeight = 120;
        //  Point currentFrame = new Point(0, 0);
        //  Point spriteSizeMoveLeft = new Point(6, 1);
        //  Texture2D texture;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
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
            //  texture = Content.Load<Texture2D>("frameLeft");

            var spriteSheet = Content.Load<SpriteSheet>("PlayerFrames.sf", new JsonContentLoader());
            var sprite = new AnimatedSprite(spriteSheet);

            sprite.Play("stayRight");
            _moveSprite = sprite;

            Song backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
            base.LoadContent();
        }

        public void LoadParameters(Vector2 playerPos)
        {
            _playerPos = playerPos;
        }

        protected override void Update(GameTime gameTime)
        {
            MovePlayer(gameTime);
            base.Update(gameTime);
            CycleFinished.Invoke(this, new EventArgs());
        }

        public void MovePlayer(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var animation = "stayRight";
            if (keys.Length > 0)
            {
                bool isWPressed = keys.Contains(Keys.W);
                bool isSPressed = keys.Contains(Keys.S);
                bool isDPressed = keys.Contains(Keys.D);
                bool isAPressed = keys.Contains(Keys.A);

                if (isWPressed && isDPressed)
                {
                    animation = "walkRight";
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.upRight });
                }
                else if (isWPressed && isAPressed)
                {
                    animation = "walkLeft";
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.upLeft });
                }
                else if (isSPressed && isAPressed)
                {
                    animation = "walkLeft";
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.downLeft });
                }
                else if (isSPressed && isDPressed)
                {
                    animation = "walkRight";
                    PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.downRight });
                }
                else
                {
                    var k = keys[0];
                    switch (k)
                    {
                        case Keys.W:
                            {
                                animation = "walkRight";
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.up });
                                break;
                            }
                        case Keys.S:
                            {
                                animation = "walkRight";
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.down });
                                break;
                            }
                        case Keys.D:
                            {
                                animation = "walkRight";
                                PlayerMoved.Invoke(this, new ControlsEventArgs { Direction = IModel.Direction.right });
                                break;
                            }
                        case Keys.A:
                            {
                                animation = "walkLeft";
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
                _moveSprite.Play(animation);
                _moveSprite.Update(deltaSeconds);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Bisque);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(_moveSprite, _playerPos);
            //   _spriteBatch.Draw(_playerImage, _playerPos, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);


        }
    }
}