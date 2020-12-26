using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class HImage : HGraphics
    {
        public override string TypeName { get => "HImage"; }
        [SerializeField]
        internal Sprite m_sprite = null;
        internal Rect m_rect;
        internal Vector2 m_textureSize;
        internal Vector4 m_border;
        internal Vector2 m_pivot;
        internal int s_id;
        public Sprite Sprite
        {
            get
            {
                return m_sprite;
            }
            set
            {
                m_sprite = value;
                m_dirty = true;
                if (value == null)
                {
                    MainTexture = null;
                    s_id = 0;
                }
                else
                {
                    s_id = value.GetInstanceID();
                    MainTexture = value.texture;
                }
            }
        }
        internal void ApplySpriteInfo()
        {
            if (m_sprite != null)
            {
#if UNITY_EDITOR
                s_id = m_sprite.GetInstanceID();
#endif
                m_rect = m_sprite.rect;
                m_textureSize.x = m_sprite.texture.width;
                m_textureSize.y = m_sprite.texture.height;
                m_border = m_sprite.border;
                m_pivot = m_sprite.pivot;
                if (Material != null)
                    Material.SetTexture("_MainTex", m_sprite.texture);
            }
            m_vertexChange = true;
        }
        [SerializeField]
        //[HideInInspector]
        internal SpriteType m_spriteType;
        public SpriteType SprType
        {
            get => m_spriteType; set
            {
                m_spriteType = value;
                m_vertexChange = true;
            }
        }
        [SerializeField]
        [HideInInspector]
        internal FillMethod m_fillMethod;
        public FillMethod FillMethod
        {
            get => m_fillMethod;
            set
            {
                m_fillMethod = value;
                m_vertexChange = true;
            }
        }
        /// <summary>
        /// 逆时针填充
        /// </summary>
        [SerializeField]
        internal bool m_fillClockwise;
        [SerializeField]
        [HideInInspector]
        internal int m_fillOrigin;
        public int FillOrigin
        {
            get => m_fillOrigin; set
            {
                m_fillOrigin = value;
                m_vertexChange = true;
            }
        }

        [SerializeField]
        [HideInInspector]
        internal float m_fillAmount = 1;
        /// <summary>
        /// 填充比例
        /// </summary>
        public float FillAmount
        {
            get => m_fillAmount;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                m_fillAmount = value;
                m_vertexChange = true;
            }
        }
        [SerializeField]
        [HideInInspector]
        internal bool m_preserveAspect;
        /// <summary>
        /// 开启此项,按弧度填充,否则按矩形四个角填充
        /// </summary>
        public bool PreserveAspect { get => m_preserveAspect; set => m_preserveAspect = value; }
        internal float m_pixelsPerUnit = 1;
        internal bool m_fillCenter = true;
        public bool FillCenter
        {
            get { return m_fillCenter; }
            set
            {
                if (m_fillCenter != value)
                    m_vertexChange = true;
                m_fillCenter = value;
            }
        }
        public float PixelsPerUnitMultiplier
        {
            get => m_pixelsPerUnit;
            set
            {
                if (m_pixelsPerUnit != value)
                    m_vertexChange = true;
                m_pixelsPerUnit = value;
            }
        }
        /// <summary>
        /// 使用纹理尺寸
        /// </summary>
        public void SetNativeSize()
        {
            if (m_sprite != null)
            {
                m_sizeDelta.x = m_sprite.rect.width;
                m_sizeDelta.y = m_sprite.rect.height;
            }
        }
        public void SetCircleMask()
        {
            if (m_material == null)
                return;
            if (m_sprite == null)
            {
                m_material.SetVector("_SRect", new Vector4(0.5f, 0.5f, 1, 1));
                return;
            }
            var sp = m_sprite;
            float w = sp.texture.width;
            float h = sp.texture.height;
            float lx = sp.rect.x / w;
            float pw = sp.rect.width / w;
            float cx = lx + pw * 0.5f;
            float dy = sp.rect.y / h;
            float ph = sp.rect.height / h;
            float cy = dy + ph * 0.5f;
            m_material.SetVector("_SRect", new Vector4(cx, cy, pw, ph));
        }
        public override void MainUpdate()
        {
            base.MainUpdate();
            if (m_dirty)
            {
                ApplySpriteInfo();
                m_dirty = false;
            }
        }
        /// <summary>
        /// 更新网格
        /// </summary>
        public override void UpdateMesh()
        {
            if (m_vertexChange)
            {
                HGUIMesh.CreateMesh(this);
                m_vertexChange = false;
                m_colorChanged = false;
            }
            else if (m_colorChanged)
            {
                m_colorChanged = false;
                var c = vertInfo.DataCount;
                if (c > 0)
                {
                    unsafe
                    {
                        HVertex* hv = (HVertex*)vertInfo.Addr;
                        for (int i = 0; i < c; i++)
                            hv[i].color = m_color;
                    }
                }
            }
        }
    }
}
