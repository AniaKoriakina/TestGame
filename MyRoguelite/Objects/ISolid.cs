using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite.Objects
{
    public interface ISolid
    {
        Collider Collider { get; set; }
        void MoveCollider(Vector2 newPos);
    }
}
