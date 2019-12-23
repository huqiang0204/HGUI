using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGraphics:AsyncScript
    {
        static Shader Shader { get {
                if(shader==null)
                    shader = Shader.Find("Shader Graphs/UI");
                return shader;
            } }
        public static Shader shader;
        Color _color;
        public Color color { get; set; }
        internal Vector3[] vertex;
        internal Vector2[] uv;
        internal int[] tris;
        public Material material;
        internal bool _vertexChange;
        public override void Initial()
        {
            material = new Material(Shader);
        }
        public virtual void UpdateMesh()
        {
        }
    }
}
