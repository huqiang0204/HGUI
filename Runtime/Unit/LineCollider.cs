using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Unit
{
    /// <summary>
    /// 线碰撞器
    /// </summary>
    public class LineCollider:UCollider2D
    {
        public LineCollider()
        {
            type = CollisionType.Line;
        }
        /// <summary>
        /// 起点
        /// </summary>
        public Vector2 start;
        /// <summary>
        /// 终点
        /// </summary>
        public Vector2 end;
    }
}
