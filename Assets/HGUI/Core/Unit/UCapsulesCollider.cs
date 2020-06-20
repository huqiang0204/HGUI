using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Unit
{
    public class UCapsulesCollider:UCollider2D
    {
        public Vector2 top;
        public Vector2 down;
        public Vector2[] buf=new Vector2[4];
        public Vector2 size;
        public Vector2 offset;
        public float radius;
        public bool circle = false;
        public override void Update()
        {
            radius = Target.localScale.x * size.x;
            if (size.y<size.x)
            {
                circle = true;
            }
            else
            {
                Vector2 s = size;
                s.x *= Target.localScale.x;
                s.y *= Target.localScale.y;
                var q = Target.localRoate;
                var p = Target.localPosition;
                p.x += offset.x;
                p.y += offset.y;
                float hy = (s.y - s.x)*0.5f;
                circle = false;
                top = p;
                top.y +=hy;
                top = q * top;
                down = p;
                down.y -= hy;
                down = q * down;

                float rx = s.x *0.5f;
                float lx = -rx;
                float ty = hy;
                float dy = -ty;
                Vector2 v = Vector2.zero;
                v.x = lx;
                v.y = dy;
                v = q * v + p;
                buf[0] = v;

                v.x = lx;
                v.y = ty;
                v = q * v +p;
                buf[1] = v;

                v.x = rx;
                v.y = ty;
                v = q * v + p;
                buf[2] = v;

                v.x = rx;
                v.y = dy;
                v = q * v + p;
                buf[3] = v;
            }
        }
    }
}
