using Microsoft.Xna.Framework;
using System;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyRoguelite.Objects;
using MonoGame.Extended;

namespace MyRoguelite.Model
{
    public class GameCycle : IModel
    {
        private int _currentId;


        private int _tileSizeWidth = 137;
        private int _tileSizeHeight = 135;

        public event EventHandler<GameplayEventArgs> Updated = delegate { };

        //    private static Vector2 _pos = new Vector2(900, 500);
        public int PlayerId { get; set; }
        private char[,] _map = new char[14, 8];

        public Dictionary<int, IObject> Objects { get; set; }

        private Player LiteralyPlayer { get; set; }


        public static string HealthText { get; set; }

        public void Update()
        {
            foreach (var obj in Objects)
            {
                Objects[obj.Key].Update();
            }

            LiteralyPlayer.Update();

            foreach (var obj in Objects)
            {
                if (obj.Value is Enemy enemy)
                {
                    if (Collider.IsCollided(LiteralyPlayer.Collider, enemy.Collider))
                    {
                        LiteralyPlayer.Health -= 0.1f;
                        HealthText = Math.Round(LiteralyPlayer.Health).ToString();
                        if (LiteralyPlayer.Health < 0)
                        {
                            LiteralyPlayer.IsDead();
                        }
                    }
                }
            }


            var obj2 = new Dictionary<int, IObject>(Objects)
            {
                { PlayerId, LiteralyPlayer }
            };
            Updated.Invoke(this, new GameplayEventArgs
            {
                Objects = obj2,
            });
        }



        public void Initialize()
        {

            Objects = new Dictionary<int, IObject>();
            _map[6, 3] = 'P';
            _map[7, 3] = 'E';
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    if (x == 0 || y == 0 || x == _map.GetLength(0) - 1 || y == _map.GetLength(1) - 1)
                    {
                        _map[x, y] = 'W';
                    }
                }
            }

            _currentId = 1;
            bool isPlacedPlayer = false;

            // Отрисовка стен
            CreateWall();

            // Отрисовка игрока
            if (!isPlacedPlayer)
            {
                isPlacedPlayer = CleatePlayer(isPlacedPlayer);
            }

            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(0); x++)
                {
                    if (_map[x, y] == 'E')
                    {
                        Enemy e = new Enemy(new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2));
                        e.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2);
                        e.ImageId = 3;
                        Objects.Add(_currentId, e);
                        _currentId++;
                    }
                }
            }
        }

        private void CreateWall()
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(0); x++)
                {
                    if (_map[x, y] == 'W')
                    {
                        Wall w = new Wall(new Vector2(135,137));
                        w.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2 + 2, y * _tileSizeHeight + _tileSizeHeight / 2 + 2);
                        w.ImageId = 2;
                        Objects.Add(_currentId, w);
                        _currentId++;
                    }
                }
            }
        }

        private bool CleatePlayer(bool isPlacedPlayer)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(0); x++)
                {
                    if (_map[x, y] == 'P')
                    {
                        Player player = new Player(new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2),
                            new Vector2(135, 137), 100);
                        player.ImageId = 1;
                        player.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2,
                            y * _tileSizeHeight + _tileSizeHeight / 2);
                        player.Speed = 7;
                        PlayerId = _currentId++;


                        LiteralyPlayer = player;
                        HealthText = player.Health.ToString();
                        LiteralyPlayer.Health = player.Health;
                        isPlacedPlayer = true;
                        break;
                    }
                }

                if (isPlacedPlayer)
                {
                    break;
                }
            }

            return isPlacedPlayer;
        }

        public void MovePlayer(IModel.Direction dir)
        {
            Player p = LiteralyPlayer;
            Vector2 playerInitPos = p.Pos;
            Vector2 newPos = p.Pos;

            switch (dir)
            {
                case IModel.Direction.up:
                    {
                        newPos += new Vector2(0, -p.Speed);
                        break;
                    }
                case IModel.Direction.down:
                    {
                        newPos += new Vector2(0, p.Speed);
                        break;
                    }
                case IModel.Direction.right:
                    {
                        newPos += new Vector2(p.Speed, 0);
                        break;
                    }
                case IModel.Direction.left:
                    {
                        newPos += new Vector2(-p.Speed, 0);
                        break;
                    }
                case IModel.Direction.upRight:
                    {
                        newPos += new Vector2(p.Speed - 2, -p.Speed + 2);
                        break;
                    }
                case IModel.Direction.upLeft:
                    {
                        newPos += new Vector2(-p.Speed + 2, -p.Speed + 2);
                        break;
                    }
                case IModel.Direction.downRight:
                    {
                        newPos += new Vector2(p.Speed - 2, p.Speed - 2);
                        break;
                    }
                case IModel.Direction.downLeft:
                    {
                        newPos += new Vector2(-p.Speed + 2, p.Speed - 2);
                        break;
                    }
                case IModel.Direction.None:
                    {
                        break;
                    }
            }
            int x = (int)Math.Round(newPos.X / _tileSizeWidth);
            int y = (int)Math.Round(newPos.Y / _tileSizeHeight);
            Collider newCollider = new Collider((int)newPos.X, (int)newPos.Y, (int)p.Size.X, (int)p.Size.Y);
            bool collided = false;
            if (playerInitPos != newPos)
            {
                Dictionary<int, ISolid> solidObjects = Objects.Where(i => i.Value is ISolid)
                    .ToDictionary(i => i.Key, i => (ISolid)i.Value);

                foreach (var solidObject in solidObjects)
                {
                    if (Collider.IsCollided(newCollider, solidObjects[solidObject.Key].Collider))
                    {
                        collided = true;
                        Vector2 returnVec = new Vector2(p.Pos.X - (playerInitPos.X + 1), p.Pos.Y - (playerInitPos.Y - 1));
                        p.Pos = newPos - returnVec;
                        p.MoveCollider(p.Pos - returnVec);
                    }
                }
            }

            if (!collided)
            {
                p.Pos = newPos;
                p.MoveCollider(p.Pos);
            }

        }
    }
}
