﻿using Microsoft.Xna.Framework;
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
        float Speed { get; set; }
        float Health { get; set; }

        void Update();
    }
}
