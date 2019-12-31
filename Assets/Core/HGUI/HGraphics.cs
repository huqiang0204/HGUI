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
        internal Vector2[] uv2;
        internal Color[] Colors;
        internal int[] tris;
        internal int[][] subTris;
        public Material Material { get; set; }
        internal int MatID;
        internal bool m_dirty = true;
        internal bool m_vertexChange = true;
        internal Texture[] textures = new Texture[4];
        internal int[] texIds = new int[4];
        public Texture MainTexture
        {
            get => textures[0];
            set {
                textures[0] = value;
                if (value != null)
                    texIds[0] = value.GetInstanceID();
                else
                    texIds[0] = 0;
            }
        }
        public Texture STexture
        {
            get => textures[1];
            set
            {
                textures[1] = value;
                if (value != null)
                    texIds[1] = value.GetInstanceID();
                else
                    texIds[1] = 0;
            }
        }
        public Texture TTexture
        {
            get => textures[2];
            set
            {
                textures[2] = value;
                if (value != null)
                    texIds[2] = value.GetInstanceID();
                else
                    texIds[2] = 0;
            }
        }
        public Texture FTexture
        {
            get => textures[3];
            set
            {
                textures[3] = value;
                if (value != null)
                    texIds[3] = value.GetInstanceID();
                else
                    texIds[3] = 0;
            }
        }
        internal int MainTexID;
        internal int STexID;
        internal int TTexID;
        internal int FTexID;
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
