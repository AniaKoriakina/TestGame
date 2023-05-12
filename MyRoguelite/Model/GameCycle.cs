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

        public void Update()
        {
            Vector2 playerInitPos = Objects[PlayerId].Pos;
            for (int i = 1; i <= Objects.Keys.Count; i++)
            {
                Vector2 objInitPos = Objects[i].Pos;
                Objects[i].Update();
                if (Objects[i] is Player p1 && objInitPos != Objects[i].Pos)
                {
                    for (int j = 1; j <= Objects.Keys.Count; j++)
                    {
                        if (i == j)
                            continue;
                        if (Objects[j] is ISolid p2)
                        {
                            while (Collider.IsCollided(p1.Collider, p2.Collider))
                            {
                                Objects[i].Pos = objInitPos;
                                p1.MoveCollider(Objects[i].Pos);
                            }
                        }
                    }
                }
            }

            //  Vector2 playerShift = Objects[PlayerId].Pos - playerInitPos;
            Updated.Invoke(this, new GameplayEventArgs
            {
                Objects = Objects,

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
                        Player player = new Player(new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2), new Vector2(135,137));
                        player.ImageId = 1;
                        player.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2);
                        player.Speed = 7;
                        PlayerId = _currentId;

                        Objects.Add(_currentId, player);
                        isPlacedPlayer = true;
                        _currentId++;
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
            Player p = (Player)Objects[PlayerId];
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
            p.Pos = newPos;

            int x = (int)Math.Floor(newPos.X / _tileSizeWidth);
            int y = (int)Math.Floor(newPos.Y / _tileSizeHeight);

            if (x >= 0 && x < _map.GetLength(0) && y >= 0 && y < _map.GetLength(1))
            {
                if (_map[x, y] != 'W')
                {
                    p.Pos = newPos;
                }
            }
        }
    }
}
