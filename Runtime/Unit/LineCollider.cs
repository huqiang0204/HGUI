using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Unit
{
    public class LineCollider:UCollider2D
    {
        public LineCollider()
        {
            type = CollisionType.Line;
        }

        public Vector2 start;
        public Vector2 end;
    }
}
