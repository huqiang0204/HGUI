using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Unit
{
    /// <summary>
    /// 胶囊体碰撞器
    /// </summary>
    public class UCapsulesCollider:UCollider2D
    {
        /// <summary>
        /// 顶部
        /// </summary>
        public Vector2 top;
        /// <summary>
        /// 底部
        /// </summary>
        public Vector2 down;
        /// <summary>
        /// 中间矩形
        /// </summary>
        public Vector2[] buf=new Vector2[4];
        /// <summary>
        /// 总体尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 与实体的偏移位置
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// 弧形半径
        /// </summary>
        public float radius;
        /// <summary>
        /// 当x=y时,胶囊体为圆形
        /// </summary>
        public bool circle = false;
        public UCapsulesCollider()
        {
            type = CollisionType.Capsules;
        }
        /// <summary>
        /// 更新顶点数据
        /// </summary>
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
                var q = Target.localRotate;
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
