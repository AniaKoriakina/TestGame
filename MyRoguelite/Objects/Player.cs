using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    public class Player : IObject, ISolid
    {
        public int ImageId { get; set; }
        public Vector2 Pos { get; set; }
        public int Speed { get; set; }
        public Collider Collider { get; set; }
        public Vector2 Size { get; set; }
        

        public Player(Vector2 position, Vector2 size) 
        { 
            Pos = position;
            Collider = new Collider((int)Pos.X, (int)Pos.Y, 135, 137);
            Size = size;
        }

        public void MoveCollider(Vector2 newPos)
        {
            Collider.Boundary = new Rectangle((int)newPos.X, (int)newPos.Y, (int)Size.X, (int)Size.Y);
        }

        public void Update()
        {
           MoveCollider(Pos);
        }
    }
}
