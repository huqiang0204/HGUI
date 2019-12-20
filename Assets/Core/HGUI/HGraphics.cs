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
        protected Vector3[] vertex;
        protected int[] tris;
        public virtual void UpdateMesh()
        {
        }
    }
}
