using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite
{
    public interface IView
    {
        event EventHandler CycleFinished; //событие, которое генерируется, когда цикл игры закончен. вызывается, когда цикл закончен
        event EventHandler<ControlsEventArgs> PlayerMoved; //событие, которое генерируется, когда игрок двигается в каком-либо направлении. 

        void LoadParameters(Vector2 pos); //загружает параметры игры.
                                          //Он принимает Vector2 в качестве параметра, который представляет позицию в игре.
        void Run();
    }

    public class ControlsEventArgs : EventArgs //свойство, которое представляет направление, в котором двигается игрок.
    {
        public IModel.Direction Direction { get; set; }
    }
}

