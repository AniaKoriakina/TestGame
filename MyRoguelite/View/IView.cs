using Microsoft.Xna.Framework;
using MyRoguelite.Model;
using MyRoguelite.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.View
{
    public interface IView
    {
        event EventHandler CycleFinished; //событие, когда цикл закончен
        event EventHandler<ControlsEventArgs> PlayerMoved; //событие, когда игрок двигается в каком-либо направлении. 

        void LoadGCParameters(Dictionary<int, IObject> _objects,Vector2 POVShift);

        //  void LoadParameters(Vector2 pos); //загружает параметры игры.
        //Он принимает Vector2 в качестве параметра, который представляет позицию в игре.
        void Run();
    }

    public class ControlsEventArgs : EventArgs //свойство, которое представляет направление, в котором двигается игрок.
    {
        public IModel.Direction Direction { get; set; }
    }
}

