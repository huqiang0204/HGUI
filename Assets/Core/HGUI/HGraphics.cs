using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGraphics:AsyncScript
    {
        Color _color;
        public Color color { get; set; }
        internal Vector3[] vertex;
        internal Vector2[] uv;
        internal int[] tris;
        public virtual void UpdateMesh()
        {
        }
    }
}
