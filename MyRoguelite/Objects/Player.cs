using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyRoguelite.Model;
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
        public float Speed { get; set; }
        public Collider Collider { get; set; }
        public Vector2 Size { get; set; }
        public float Health { get; set; }
        public Dictionary<int, IObject> Objects { get; set; }

        public Player(Vector2 position, Vector2 size, int health)
        {
            Pos = position;
            float halfWidth = Size.X / 2;
            float halfHeight = Size.Y / 2;
            Collider = new Collider((int)(Pos.X - halfWidth), (int)(Pos.Y - halfHeight), 135, 137);
            Size = size;
            Health = health;
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
            float halfHeight = Size.Y / 2;
            Collider.Boundary = new Rectangle((int)(Pos.X - halfWidth), (int)(Pos.Y - halfHeight), 135, 137);
        }

        public void Update()
        {
            MoveCollider(Pos);
        }
    }
}