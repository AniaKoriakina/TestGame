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
        public float Speed { get; set; }
        public Collider Collider { get; set; }
        public Vector2 Size { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }

        public string EnemyHealthText { get; set; }


        public Enemy(Vector2 position, float health)
        {
            Pos = position;
            Size = new Vector2(135, 137);
            float halfWidth = Size.X / 2;
            float halfHeight = Size.Y / 6;
            Collider = new Collider((int)(Pos.X - halfWidth), (int)(Pos.Y - halfHeight), (int)Size.X, (int)Size.Y);
            MaxHealth = 100f;
            Health = MaxHealth;
            
        }
        public bool IsDead()
        {
            if (Health <= 0)
            {
                return true;
            }
            return false;
        }

        public void MoveCollider(Vector2 newPos)
        {
            float halfWidth = Size.X / 2;
            float halfHeight = Size.Y / 6;
            Collider.Boundary = new Rectangle((int)(newPos.X - halfWidth), (int)(newPos.Y - halfHeight), (int)Size.X, (int)Size.Y);
        }

        public void Update()
        {
            MoveCollider(Pos);
        }
    } 
}
