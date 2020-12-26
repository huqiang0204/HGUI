using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Helper.HGUI
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
    public class HGraphics: UIElement
    {
        /// <summary>
        /// 获取或设置默认着色器
        /// </summary>
        public static Shader shader;
        [SerializeField]
        internal Color32 m_color = Color.white;
        public Vector4 uvrect = new Vector4(0,0,1,1);
        public override Color32 MainColor { get => m_color; set { m_color = value;} }
        public override Vector2 SizeDelta { get => m_sizeDelta; set { m_sizeDelta = value; } }
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
        [HideInInspector]
        public Texture MainTexture;
        [HideInInspector]
        public Texture STexture;
        [HideInInspector]
        public Texture TTexture;
        [HideInInspector]
        public Texture FTexture;
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
        protected void SaveToHGraphics(Core.HGUI.HGraphics ui, bool activeSelf,bool haveChild)
        {
            ui.m_color = m_color;
            ui.uvrect = uvrect;
            ui.Material = m_material;
            ui.MainTexture = MainTexture;
            ui.STexture = STexture;
            ui.TTexture = TTexture;
            ui.FTexture = FTexture;
            ui.Shadow = this.Shadow;
            ui.shadowOffsset = this.shadowOffsset;
            ui.shadowColor = this.shadowColor;
            SaveToUIElement(ui, activeSelf,haveChild);
        }
        public override Core.HGUI.UIElement ToHGUI2(bool activeSelf,bool haveChild = true)
        {
            Core.HGUI.HGraphics graphics = new Core.HGUI.HGraphics();
            SaveToHGraphics(graphics, activeSelf,haveChild);
            return graphics;
        }
        public override void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        {
            SaveToHGraphics(ui as Core.HGUI.HGraphics, activeSelf,false);
        }
    }
}
