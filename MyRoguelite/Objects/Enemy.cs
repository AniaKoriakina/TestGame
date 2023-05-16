using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    public class Enemy : IObject
    {
        public int ImageId { get; set; }
        public Vector2 Pos { get; set; }
        public int Speed { get; set; }
        public Collider Collider { get; set; }
        public Vector2 Size { get; set; }
        public float Health { get; set; }
        private bool Alive = true;
        public Enemy(Vector2 position, float health)
        {
            Pos = position;
            Size = new Vector2(135, 137);
            Collider = new Collider((int)Pos.X, (int)Pos.Y, 135, 137);
            Health = health;
        }
        public bool IsDead()
        {
            if (Health <= 0)
            {
                this.Alive = false;
                return true;
            }
            return false;
        }

        public void MoveCollider(Vector2 newPos)
        {
            Collider.Boundary = new Rectangle((int)newPos.X, (int)newPos.Y, 135, 137);
        }

        public void Update()
        {
            MoveCollider(Pos);
           
        }
    } //у меня есть класс enemy, и player, игрок должен сталкиваться с объектом, а не входить в него,
      //а у меня игрок входит в другой объект
}
