using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.Line
{
    public struct BzierLine
    {
        public LineBase lineBase;
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;
        public float Precision;
    }
}
