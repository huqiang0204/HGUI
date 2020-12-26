using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SerializedData
{
    [Serializable]
    public partial class TestA
    {
        public int[] a;
        public string[] b;
        public TestB[] c;
        public List<int> d;
        public List<string> e;
        public List<TestB> f;
        public huqiang.UIComposite.ScrollType scroll;
        public Vector2[] v2;
        public Vector3[] v3;
        public List<Vector4> v4;
    }
}
