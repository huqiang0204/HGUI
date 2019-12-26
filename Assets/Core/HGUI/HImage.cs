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
        [SerializeField]
        internal Sprite _sprite = null;
        internal Rect _rect;
        internal Vector2 _textureSize;
        internal Vector4 _border;
        internal Vector2 _pivot;
        public Sprite Sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
                _dirty = true;
            }
        }
        internal void ApplySpriteInfo()
        {
            if (_sprite != null)
            {
                _rect = _sprite.rect;
                _textureSize.x = _sprite.texture.width;
                _textureSize.y = _sprite.texture.height;
                _border = _sprite.border;
                _pivot = _sprite.pivot;
                material.SetTexture("_MainTex", _sprite.texture);
                _vertexChange = true;
            }
        }
        public SpriteType SprType { get; set; }
        internal FillMethod _fillMethod;
        public FillMethod FillMethod { get => _fillMethod;
            set {
                _fillMethod = value;
                _vertexChange = true;
            } }
        internal bool _fillClockwise;
        public bool FillClockwise { get; set; }
        internal int _fillOrigin;
        public int FillOrigin { get => _fillOrigin; set {
                _fillOrigin = value;
                _vertexChange = true;
            } }
        internal float _fillAmount = 1;
        public float FillAmount {
            get => _fillAmount;
            set {
                _fillAmount = value;
                _vertexChange = true;
            } }
        /// <summary>
        /// 开启此项,按弧度填充,否则按矩形四个角填充
        /// </summary>
        public bool PreserveAspect { get; set; }
        internal float _pixelsPerUnit = 1;
        internal bool _fillCenter = true;
        public bool FillCenter {
            get { return _fillCenter; }
            set { if (_fillCenter != value)
                    _vertexChange = true;
                _fillCenter = value; }
        }
        public float PixelsPerUnitMultiplier { get => _pixelsPerUnit;
            set {
                if (_pixelsPerUnit != value)
                    _vertexChange = true;
                _pixelsPerUnit = value;
            } }
        public void SetNativeSize()
        {
            if(_sprite!=null)
            {
                SizeDelta.x = _sprite.rect.width;
                SizeDelta.y = _sprite.rect.height;
            }
        }
        public override void MainUpdate()
        {
            if(_dirty)
            {
                ApplySpriteInfo();
                _dirty = false;
            }
        }
        public override void UpdateMesh()
        {
            if(_vertexChange)
            {
                HGUIMesh.CreateMesh(this);
                _vertexChange = false;
            }
        }
        internal override Material GetMaterial(int index, HCanvas canvas)
        {
            return material;
        }
    }
}
