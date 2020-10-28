using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.Unit
{
    /// <summary>
    /// 圆形碰撞器
    /// </summary>
    public class UCircleCollider:UCollider2D
    {
        /// <summary>
        /// 半径
        /// </summary>
        public float radius;
        /// <summary>
        /// 
        /// </summary>
        public float rad;
        public UCircleCollider()
        {
            type = CollisionType.Cricle;
        }
        public override void Update()
        {
            rad = Target.localScale.x * radius;
        }
    }
}
