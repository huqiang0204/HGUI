using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.Line
{
    public struct ArcLine
    {
        public LineBase lineBase;
        public Vector2 Pos;
        public Vector2 Scale;
        /// <summary>
        /// 角度
        /// </summary>
        public float Angle;
        /// <summary>
        /// 方向
        /// </summary>
        public float Dic;
        /// <summary>
        /// 半径
        /// </summary>
        public float Radius;
        /// <summary>
        /// 精度
        /// </summary>
        public float Precision;
        /// <summary>
        /// 闭合的
        /// </summary>
        public bool Closed;
    }
}
