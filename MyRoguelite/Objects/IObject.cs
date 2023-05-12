using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    public interface IObject
    {
        int ImageId { get; set; }
        Vector2 Pos { get; set; }
        int Speed { get; set; }
        void Update();
    }
}
