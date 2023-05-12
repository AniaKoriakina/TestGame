﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
using MyRoguelite.Model;
using MyRoguelite.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Input;

namespace MyRoguelite.View
{
    public class Game1 : Game, IView
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public event EventHandler CycleFinished = delegate { };
        public event EventHandler<ControlsEventArgs> PlayerMoved = delegate { };

        private AnimatedSprite _moveSprite;
        private AnimatedSprite _mapFrame;
        private AnimatedSprite _enemySprite;

        private Vector2 _visualShift = new Vector2(0, 0);

        private Dictionary<int, IObject> _objects = new Dictionary<int, IObject>();
        private Dictionary<int, AnimatedSprite> _textures = new Dictionary<int, AnimatedSprite>();


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

            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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


            //Song backgroundMusic = Content.Load<Song>("backgroundMusic");
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(backgroundMusic);

            base.LoadContent();
        }

        public void LoadGCParameters(Dictionary<int, IObject> Objects, Vector2 POVShift)
        {
            _objects = Objects;
            _visualShift += POVShift;
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

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.DarkSlateGray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);


            foreach (var o in _objects.Values)
            {
                _spriteBatch.Draw(_textures[o.ImageId], o.Pos - _visualShift);
            }

            _spriteBatch.End();

            base.Draw(gameTime);


        }
    }
}
