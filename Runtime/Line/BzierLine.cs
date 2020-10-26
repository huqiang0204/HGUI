using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.Line
{
    /// <summary>
    /// 贝塞尔曲线
    /// </summary>
    public struct BzierLine
    {
        public LineBase lineBase;
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;
        /// <summary>
        /// 精度
        /// </summary>
        public float Precision;
    }
}
