
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.Line
{
    /// <summary>
    /// 二阶贝塞尔曲线
    /// </summary>
    public struct BzierLine2
    {
        public LineBase lineBase;
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;
        public Vector2 D;
        /// <summary>
        /// 精度
        /// </summary>
        public float Precision;
    }
}
