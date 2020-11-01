using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// UI顶点
    /// </summary>
    public struct HVertex
    {
        public Vector3 position;
        public Vector3 normal;
        //public Vector4 tangent;
        public Color32 color;
        public Vector2 uv;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector2 uv4;
        /// <summary>
        /// 纹理索引0-3
        /// </summary>
        public int picture;
    }
    [DisallowMultipleComponent]
    public class HGraphics:UIElement
    {
        /// <summary>
        /// 三角形块级缓存
        /// </summary>
        protected static BlockBuffer<int> trisBuffer = new BlockBuffer<int>(48, 1024);
        /// <summary>
        /// 默认的UIShader,当ui上的材质球为空时默认使用吃着色器
        /// </summary>
        internal static Shader DefShader { get {
                if(shader==null)
                    shader = Shader.Find("HGUI/UIDef");//HGUI/UIDef
                return shader;
            } }
        /// <summary>
        /// 获取或设置默认着色器
        /// </summary>
        public static Shader shader;
        [SerializeField]
        internal Color32 m_color = Color.white;
        public Vector4 uvrect = new Vector4(0,0,1,1);
        public override Color32 MainColor { get => m_color; set { m_color = value; m_colorChanged = true;} }
        public override Vector2 SizeDelta { get => m_sizeDelta; set { m_sizeDelta = value; m_vertexChange = true; } }
        internal int[] tris;
        /// <summary>
        /// 顶点块级内存
        /// </summary>
        internal BlockInfo<HVertex> vertInfo;
        /// <summary>
        /// 三角形块级内存
        /// </summary>
        internal BlockInfo<int> trisInfo;
        /// <summary>
        /// 三角形块级内存2
        /// </summary>
        internal BlockInfo<int> trisInfo2;
        [SerializeField]
        internal Material m_material;
        public Material Material { get => m_material; set {
                m_material = value;
                if (value == null)
                    MatID = 0;
                else MatID = value.GetInstanceID();
            } }
        internal int MatID;
        /// <summary>
        /// 材质污染,需要重新计算uv
        /// </summary>
        [HideInInspector]
        public bool m_dirty = true;
        /// <summary>
        /// 网格污染,网格需要重新计算
        /// </summary>
        [HideInInspector]
        public bool m_vertexChange = true;
        /// <summary>
        /// 颜色改变后,需要重新填色
        /// </summary>
        [HideInInspector]
        public bool m_colorChanged = true;
        /// <summary>
        /// 支持4张纹理
        /// </summary>
        [HideInInspector]
        [SerializeField]
        internal Texture[] textures = new Texture[4];
        /// <summary>
        /// 纹理id
        /// </summary>
        [HideInInspector]
        [SerializeField]
        internal int[] texIds = new int[4];
        /// <summary>
        /// 是否使用填充色,即不使用纹理颜色
        /// </summary>
        [HideInInspector]
        [SerializeField]
        internal bool[] fillColors = new bool[4];
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
        /// <summary>
        /// 开启阴影
        /// </summary>
        public bool Shadow;
        /// <summary>
        /// 阴影偏移
        /// </summary>
        public Vector2 shadowOffsset=new Vector2(1,-1);
        /// <summary>
        /// 阴影颜色
        /// </summary>
        public Color32 shadowColor=new Color32(0,0,0,255);
        public override void ReSized()
        {
            base.ReSized();
            m_vertexChange = true;
        }
        /// <summary>
        /// 更新网格
        /// </summary>
        public virtual void UpdateMesh()
        {
            if(m_colorChanged)
            {
                if (vertInfo.DataCount>0)
                {
                    unsafe
                    {
                        HVertex* hv = vertInfo.Addr;
                        for (int i = 0; i < vertInfo.DataCount; i++)
                            hv[i].color = m_color;
                    }
                }
                m_colorChanged = false;
            }
        }
        protected override void Start()
        {
            base.Start();
            m_dirty = true;
        }
        /// <summary>
        /// 销毁时释放非托管的块级内存
        /// </summary>
        protected virtual void OnDestroy()
        {
            vertInfo.Release();
            trisInfo.Release();
            trisInfo2.Release();
        }
        /// <summary>
        /// 载入网格,使用外部计算好的网格
        /// </summary>
        /// <param name="vert">顶点</param>
        /// <param name="tris">三角形</param>
        public void LoadFromMesh(List<HVertex> vert, List<int> tris)
        {
            LoadVert(vert);
            LoadTris(tris);
        }
        /// <summary>
        /// 载入网格,使用外部计算好的网格
        /// </summary>
        /// <param name="vert">顶点信息</param>
        public void LoadVert(HVertex[] vert)
        {
            m_dirty = false;
            int c = vert.Length;
            if (c > vertInfo.Size | c + 32 < vertInfo.Size)
            {
                vertInfo.Release();
                vertInfo = HGUIMesh.blockBuffer.RegNew(c);
            }
            unsafe
            {
                HVertex* hv = vertInfo.Addr;
                for (int i = 0; i < c; i++)
                {
                    hv[i] = vert[i];
                }
                vertInfo.DataCount = c;
            }
        }
        /// <summary>
        /// 载入网格,使用外部计算好的网格
        /// </summary>
        /// <param name="vert">顶点信息</param>
        public void LoadVert(List<HVertex> vert)
        {
            m_dirty = false;
            int c = vert.Count;
            if (c > vertInfo.Size | c + 32 < vertInfo.Size)
            {
                vertInfo.Release();
                vertInfo = HGUIMesh.blockBuffer.RegNew(c);
            }
            unsafe
            {
                HVertex* hv = vertInfo.Addr;
                for (int i = 0; i < c; i++)
                {
                    hv[i] = vert[i];
                }
                vertInfo.DataCount = c;
            }
        }    
        void LoadTris(int[] tri, ref BlockInfo<int> info)
        {
            tris = null;
            int tc = tri.Length;
            if (tc > info.Size | tc + 48 < info.Size)
            {
                info.Release();
                info = trisBuffer.RegNew(tc);
            }
            unsafe
            {
                int* ht = (int*)info.Addr;
                for (int i = 0; i < tc; i++)
                {
                    ht[i] = tri[i];
                }
                info.DataCount = tc;
            }
        }
        /// <summary>
        /// 载入网格,使用外部计算好的三角形1
        /// </summary>
        /// <param name="tri"></param>
        public void LoadTris(int[] tri)
        {
            LoadTris(tri,ref trisInfo);
        }
        /// <summary>
        /// 载入网格,使用外部计算好的三角形2
        /// </summary>
        /// <param name="tri"></param>
        public void LoadTris2(int[] tri)
        {
            LoadTris(tri, ref trisInfo2);
        }
        void LoadTris(List<int> tri, ref BlockInfo<int> info)
        {
            tris = null;
            int tc = tri.Count;
            if (tc > info.Size | tc + 48 < info.Size)
            {
                info.Release();
                info = trisBuffer.RegNew(tc);
            }
            unsafe
            {
                int* ht = (int*)info.Addr;
                for (int i = 0; i < tc; i++)
                {
                    ht[i] = tri[i];
                }
                info.DataCount = tc;
            }
        }
        /// <summary>
        /// 载入网格,使用外部计算好的三角形1
        /// </summary>
        /// <param name="tri"></param>
        public void LoadTris(List<int> tri)
        {
            LoadTris(tri, ref trisInfo);
        }
        /// <summary>
        /// 载入网格,使用外部计算好的三角形2
        /// </summary>
        /// <param name="tri"></param>
        public void LoadTris2(List<int> tri)
        {
            LoadTris(tri, ref trisInfo2);
        }
    }
}
