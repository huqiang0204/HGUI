using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGraphics:AsyncScript
    {
        internal static Shader defShader { get {
                if(shader==null)
                    shader = Shader.Find("Custom/UIDef");
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
            material = new Material(defShader);
        }
        public virtual void UpdateMesh()
        {
        }
    }
}
