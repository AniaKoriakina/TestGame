using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    internal class Wall : IObject, ISolid
    {
        public int ImageId { get; set; }
        public Vector2 Pos { get; set; }
        public float Speed { get; set; }
        public float Health { get; set; }
        public Vector2 Size { get; set; }

        public Collider Collider { get; set; }

        public Wall(Vector2 size)
        {
            Size = size;
            Collider = new Collider((int)Pos.X, (int)Pos.Y, 135, 137);
        }

        public void MoveCollider(Vector2 newPos)
        {
            Collider = new Collider((int)Pos.X, (int)Pos.Y, 135, 137);
        }

        public void Update()
        {
            MoveCollider(Pos);
        }
    }
}
