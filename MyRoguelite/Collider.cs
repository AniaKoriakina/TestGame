using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite
{
    public class Collider
    {
        
        public Rectangle Boundary { get; set; }
        //public Point Location { get; set; }

        public Collider(int x, int y, int width, int height)
        {
             Boundary = new Rectangle(x, y, width, height); 
        }

        public static bool IsCollided(Collider r1, Collider r2)
        {
            return r1.Boundary.Intersects(r2.Boundary);
        }

    }
}
