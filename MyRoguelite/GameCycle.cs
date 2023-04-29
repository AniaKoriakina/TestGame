using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite
{
    public class GameCycle : IModel
    {
        public event EventHandler<GameplayEventArgs> Updated = delegate { };

        private Vector2 _pos = new Vector2(900, 500);


        public void Update()
        {
            Updated.Invoke(this, new GameplayEventArgs { PlayerPos = _pos });
        }

        public void MovePlayer(IModel.Direction dir, int playerSpeed)
        {
            switch (dir)
            {
                case IModel.Direction.up:
                    {
                        _pos += new Vector2(0, -playerSpeed);
                        break;
                    }
                case IModel.Direction.down:
                    {
                        _pos += new Vector2(0, playerSpeed);
                        break;
                    }
                case IModel.Direction.right:
                    {
                        _pos += new Vector2(playerSpeed, 0);
                        break;
                    }
                case IModel.Direction.left:
                    {
                        _pos += new Vector2(-playerSpeed, 0);
                        break;
                    }
                case IModel.Direction.upRight:
                    {
                        _pos += new Vector2(playerSpeed - 2, -playerSpeed + 2);
                        break;
                    }
                case IModel.Direction.upLeft:
                    {
                        _pos += new Vector2(-playerSpeed + 2, -playerSpeed + 2);
                        break;
                    }
                case IModel.Direction.downRight:
                    {
                        _pos += new Vector2(playerSpeed - 2, playerSpeed - 2);
                        break;
                    }
                case IModel.Direction.downLeft:
                    {
                        _pos += new Vector2(-playerSpeed + 2, playerSpeed - 2);
                        break;
                    }

            }
        }
    }
}
