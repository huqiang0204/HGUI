using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGraphics:AsyncScript
    {
        internal static Shader DefShader { get {
                if(shader==null)
                    shader = Shader.Find("Custom/UIDef");//Custom/UIDef
                return shader;
            } }
        public static Shader shader;
        [SerializeField]
        internal Color m_color = Color.white;
        public virtual Color Color { get => m_color; set { m_color = value; } }
        internal Vector3[] vertex;
        internal Vector2[] uv;
        internal Vector2[] uv1;
        internal Color[] Colors;
        internal int[] tris;
        public Material material;
        internal bool m_dirty = true;
        internal bool m_vertexChange = true;
        public Texture MainTexture { get; set; }
        internal int TextureID;
        public bool ShareMaterial;
        public bool Mask;
        public virtual void UpdateMesh()
        {
        }
        public virtual bool CompareMaterial(HGraphics graphics)
        {
            return false;
        }
        internal virtual Material GetMaterial(int index, HCanvas canvas)
        {
            return null;
        }
        internal int PipelineIndex;
    }
}
