using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SerializedData
{
    [Serializable]
    public partial class TestB
    {
        public byte a;
        public Int16 b;
        public int c;
        public long d;
        public float e;
        public double f;
        public string g;
        public TestA h;
        public Vector2 v;
        public Vector3 v3;
        public Vector4 v4;
        public Color col;
        public Color32 col32;
        public Quaternion q;
    }
}
