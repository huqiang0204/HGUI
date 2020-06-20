using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.Unit
{
    public class UCircleCollider:UCollider2D
    {
        public float radius;
        public float rad;
        public override void Update()
        {
            rad = Target.localScale.x * radius;
        }
    }
}
