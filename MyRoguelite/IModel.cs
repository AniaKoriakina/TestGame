using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyRoguelite
{
    public interface IModel
    {
        int PlayerId { get; set; }
        Dictionary<int, IObject> Objects { get; set; }
        //событие можно вызывать в программе как метод
        event EventHandler<GameplayEventArgs> Updated; //событие, которое генерируется, когда происходит обновление игры.
                                                       //генерируется с помощью GameplayEventArgs, которое определяет позицию игрока. 

        void Update(); 
        void MovePlayer(Direction dir);
        void Initialize();

        public enum Direction : byte // это перечисление, которое определяет восемь возможных направлений, в которых игрок может двигаться.
        {
            up,
            down,
            right,
            left,
            upLeft,
            upRight,
            downLeft,
            downRight,
            None
        }
    }

    public class GameplayEventArgs : EventArgs //определяет, какие данные будут отправлены в событии Updated
    {
       // public Vector2 PlayerPos { get; set; } 
        public Dictionary<int, IObject> Objects { get; set; }
    }
}
