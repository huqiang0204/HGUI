using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Unit
{
    /// <summary>
    /// 多边形碰撞器
    /// </summary>
    public class UPolygonCollider:UCollider2D
    {
        public Vector2[] points;
        public Vector2[] buf;
        public UPolygonCollider()
        {
            type = CollisionType.Pollygon;
        }
        public override void Update()
        {
            if (Target == null)
                return;
            if (buf == null)
                buf = new Vector2[points.Length];
            var ls = Target.localScale;
            var pos = Target.localPosition;
            var q = Target.localRotate;
            for(int i=0;i<buf.Length;i++)
            {
                var v = points[i];
                v.x *= ls.x;
                v.y *= ls.y;
                buf[i] = q * v + pos;
            }
        }
    }
}
