using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    public class Bullets : IObject
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }
        public int ImageId { get; set; }
        public Vector2 Pos { get; set; }
        public int Speed { get; set; }
        public Vector2 Size { get; set; }
        public Collider Collider { get; set; }

        public Bullets(Vector2 position, Vector2 velocity, int speed, int damage)
        {
            Pos = position;
            Velocity = velocity;
            Speed = speed;
            Damage = damage;
            Size = new Vector2(10, 10);
            Collider = new Collider((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y);
        }

        public void Update()
        {
            Pos += Velocity * Speed;
            MoveCollider(Pos);
        }

        public void MoveCollider(Vector2 newPos)
        {
            Collider.Boundary = new Rectangle((int)newPos.X, (int)newPos.Y, (int)Size.X, (int)Size.Y);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D bulletTexture)
        {
            spriteBatch.Draw(bulletTexture, Pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
