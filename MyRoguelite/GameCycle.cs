using Microsoft.Xna.Framework;
using System;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite
{
    public class GameCycle : IModel
    {
        public event EventHandler<GameplayEventArgs> Updated = delegate { };

    //    private static Vector2 _pos = new Vector2(900, 500);
        public int PlayerId { get; set; }

        public Dictionary<int, IObject> Objects { get; set; }

        public void Update()
        {
            Updated.Invoke(this, new GameplayEventArgs { Objects = this.Objects });
        }

        public void Initialize()
        {
            Objects = new Dictionary<int, IObject>();
            Player player = new Player();
            player.Pos = new Vector2(900, 500);
            player.ImageId = 1;
            player.Speed = 7;
            Objects.Add(1, player);
            PlayerId = 1;
        }

        public void MovePlayer(IModel.Direction dir)
        {
            Player p = (Player)Objects[PlayerId];
            switch (dir)
            {
                case IModel.Direction.up:
                    {
                        p.Pos += new Vector2(0, -p.Speed);
                        break;
                    }
                case IModel.Direction.down:
                    {
                        p.Pos += new Vector2(0, p.Speed);
                        break;
                    }
                case IModel.Direction.right:
                    {
                        p.Pos += new Vector2(p.Speed, 0);
                        break;
                    }
                case IModel.Direction.left:
                    {
                        p.Pos += new Vector2(-p.Speed, 0);
                        break;
                    }
                case IModel.Direction.upRight:
                    {
                        p.Pos += new Vector2(p.Speed - 2, -p.Speed + 2);
                        break;
                    }
                case IModel.Direction.upLeft:
                    {
                        p.Pos += new Vector2(-p.Speed + 2, -p.Speed + 2);
                        break;
                    }
                case IModel.Direction.downRight:
                    {
                        p.Pos += new Vector2(p.Speed - 2, p.Speed - 2);
                        break;
                    }
                case IModel.Direction.downLeft:
                    {
                        p.Pos += new Vector2(-p.Speed + 2, p.Speed - 2);
                        break;
                    }
                case IModel.Direction.None:
                    {
                        break;
                    }

            }
        }
    }
}
