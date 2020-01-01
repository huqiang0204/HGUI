using UnityEngine;

namespace Assets.Core.HGUI
{
    public enum Origin180
    {
        Bottom = 0,
        Left = 1,
        Top = 2,
        Right = 3
    }
    public enum SpriteType
    {
        Simple = 0,
        Sliced = 1,
        Tiled = 2,
        Filled = 3
    }
    public enum FillMethod
    {
        Horizontal = 0,
        Vertical = 1,
        Radial90 = 2,
        Radial180 = 3,
        Radial360 = 4
    }
    public enum OriginHorizontal
    {
        Left = 0,
        Right = 1
    }
    public enum OriginVertical
    {
        Bottom = 0,
        Top = 1
    }
    public enum Origin90
    {
        BottomLeft = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 3
    }
    public enum Origin360
    {
        Bottom = 0,
        Right = 1,
        Top = 2,
        Left = 3
    }
    public class HImage:HGraphics
    {
        static Material m_DefaultMat;
        static Material DefaultMat {
            get { if (m_DefaultMat == null)
                    m_DefaultMat = new Material(DefShader);
                return m_DefaultMat;
            } }
        [SerializeField]
        internal Sprite m_sprite = null;
        internal Rect m_rect;
        internal Vector2 m_textureSize;
        internal Vector4 m_border;
        internal Vector2 m_pivot;
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
                else {
                    MainTexture = value.texture;
                }
            }
        }
        Material mainMaterial;
        internal void ApplySpriteInfo()
        {
            if (m_sprite != null)
            {
                m_rect = m_sprite.rect;
                m_textureSize.x = m_sprite.texture.width;
                m_textureSize.y = m_sprite.texture.height;
                m_border = m_sprite.border;
                m_pivot = m_sprite.pivot;
                if (Material == null)
                    Material = new Material(DefShader);
                Material.SetTexture("_MainTex", m_sprite.texture);
                mainMaterial = Material;
            }
            else
            {
                mainMaterial = DefaultMat;
            }
            m_vertexChange = true;
        }
        public SpriteType SprType { get; set; }
        internal FillMethod m_fillMethod;
        public FillMethod FillMethod { get => m_fillMethod;
            set {
                m_fillMethod = value;
                m_vertexChange = true;
            } }
        internal bool m_fillClockwise;
        public bool FillClockwise { get; set; }
        internal int m_fillOrigin;
        public int FillOrigin { get => m_fillOrigin; set {
                m_fillOrigin = value;
                m_vertexChange = true;
            } }
        internal float m_fillAmount = 1;
        public float FillAmount {
            get => m_fillAmount;
            set {
                m_fillAmount = value;
                m_vertexChange = true;
            } }
        /// <summary>
        /// 开启此项,按弧度填充,否则按矩形四个角填充
        /// </summary>
        public bool PreserveAspect { get; set; }
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
        public void SetNativeSize()
        {
            if(m_sprite!=null)
            {
                SizeDelta.x = m_sprite.rect.width;
                SizeDelta.y = m_sprite.rect.height;
            }
        }
        public override void MainUpdate()
        {
            if(m_dirty)
            {
                ApplySpriteInfo();
                m_dirty = false;
            }
        }
        public override void UpdateMesh()
        {
            if(m_vertexChange)
            {
                HGUIMesh.CreateMesh(this);
                m_vertexChange = false;
            }
        }
    }
}
