using huqiang.Core.UIData;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public class HImage: HGraphics
    {
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
                }
                else
                {
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
        internal SpriteType m_spriteType;
        public SpriteType SprType { get => m_spriteType; set {
                m_spriteType = value;
                m_vertexChange = true;
            } }
        [SerializeField]
        [HideInInspector]
        internal FillMethod m_fillMethod;
        public FillMethod FillMethod { get => m_fillMethod;
            set {
                m_fillMethod = value;
                m_vertexChange = true;
            } }
        /// <summary>
        /// 逆时针填充
        /// </summary>
        [SerializeField]
        internal bool m_fillClockwise;
        [SerializeField]
        [HideInInspector]
        internal int m_fillOrigin;
        public int FillOrigin { get => m_fillOrigin; set {
                m_fillOrigin = value;
                m_vertexChange = true;
            } }
  
        [SerializeField]
        [HideInInspector]
        internal float m_fillAmount = 1;
        /// <summary>
        /// 填充比例
        /// </summary>
        public float FillAmount {
            get => m_fillAmount;
            set {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                m_fillAmount = value;
                m_vertexChange = true;
            } }
        [SerializeField]
        [HideInInspector]
        internal bool m_preserveAspect;
        /// <summary>
        /// 开启此项,按弧度填充,否则按矩形四个角填充
        /// </summary>
        public bool PreserveAspect { get => m_preserveAspect; set => m_preserveAspect = value; }
        internal float m_pixelsPerUnit = 1;
        internal bool m_fillCenter = true;
        public bool FillCenter {
            get { return m_fillCenter; }
            set { if (m_fillCenter != value)
                    m_vertexChange = true;
                m_fillCenter = value; }
        }
        public float PixelsPerUnitMultiplier { get => m_pixelsPerUnit;
            set {
                if (m_pixelsPerUnit != value)
                    m_vertexChange = true;
                m_pixelsPerUnit = value;
            } }
        /// <summary>
        /// 使用纹理尺寸
        /// </summary>
        public void SetNativeSize()
        {
            if(m_sprite!=null)
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
        protected void SaveToHImage(Core.HGUI.HImage ui, bool activeSelf,bool haveChild)
        {
            ui.Sprite = m_sprite;
            ui.m_rect = m_rect;
            ui.m_textureSize = m_textureSize;
            ui.m_border = m_border;
            ui.m_pivot = m_pivot;
            ui.s_id = s_id;
            ui.m_spriteType = m_spriteType;
            ui.m_fillMethod = m_fillMethod;
            ui.m_fillClockwise = m_fillClockwise;
            ui.m_fillOrigin = m_fillOrigin;
            ui.m_fillAmount = m_fillAmount;
            ui.m_preserveAspect = m_preserveAspect;
            ui.m_pixelsPerUnit = m_pixelsPerUnit;
            ui.m_fillCenter = m_fillCenter;
            SaveToHGraphics(ui, activeSelf,haveChild);
        }
        public override Core.HGUI.UIElement ToHGUI2(bool activeSelf,bool haveChild=true)
        {
            Core.HGUI.HImage himg = new Core.HGUI.HImage();
            SaveToHImage(himg, activeSelf,haveChild);
            return himg;
        }
        public override void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        {
            SaveToHImage(ui as Core.HGUI.HImage, activeSelf,false);
        }
    }
}
