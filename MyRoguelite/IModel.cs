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

        void Update();  //обновляет состояние игры
        void MovePlayer(Direction dir); //перемещает игрока в заданном направлении.
        void Initialize();

        //Он принимает Direction в качестве параметра, чтобы определить, в каком направлении двигаться.

        public enum Direction : byte // это перечисление, которое определяет восемь возможных направлений, в которых игрок может двигаться.
        {
            up,
            down,
            right,
            left,
            upLeft,
            upRight,
            downLeft,
            downRight
        }
    }

    public class GameplayEventArgs : EventArgs //определяет, какие данные будут отправлены в событии Updated
    {
        public Vector2 PlayerPos { get; set; } //свойство, которое представляет позицию игрока в игровой среде.
        public Dictionary<int, IObject> Objects { get; set; }
    }
}
