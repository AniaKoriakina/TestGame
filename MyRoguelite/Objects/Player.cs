using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    internal class Player : IObject
    {
        public int ImageId { get; set; }
        public Vector2 Pos { get; set; }
        public int Speed { get; set; }

        public void Update()
        {

        }
    }
}
