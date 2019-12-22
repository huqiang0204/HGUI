using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        internal Sprite _sprite=null;
        internal Rect _rect;
        internal Vector2 _textureSize;
        internal Vector4 _border;
        internal Vector2 _pivot;
        //网格待处理
        bool _vertexPending;
        //三角形待处理
        bool _trisPending;
        public Sprite sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
                if (value != null)
                {
                    _rect = value.rect;
                    _textureSize.x = value.texture.width;
                    _textureSize.y = value.texture.height;
                    _border = value.border;
                    _pivot = value.pivot;
                    material.SetTexture("_MainTex", value.texture);
                }
            }
        }
        public SpriteType type { get; set; }
        internal FillMethod _fillMethod;
        public FillMethod fillMethod { get => _fillMethod;
            set {
                _fillMethod = value;
            } }
        internal int _fillOrigin;
        public int fillOrigin { get => _fillOrigin; set {
                _fillOrigin = value;
            } }
        internal float _fillAmount=1;
        public float fillAmount { get => _fillAmount;
            set {
                _fillAmount = value;
            } }
        public bool preserveAspect { get; set; }
        internal float _pixelsPerUnit = 1;
        internal bool _fillCenter = true;
        public bool fillCenter {
            get { return _fillCenter; }
            set { if (_fillCenter != value)
                    _vertexPending = true;
                _fillCenter = value; }
        }
        public float pixelsPerUnitMultiplier { get => _pixelsPerUnit;
            set {
                if (_pixelsPerUnit != value)
                    _vertexPending = true;
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
        public override void UpdateMesh()
        {
            if(_vertexPending)
            {
                HGUIMesh.CreateMesh(this);
                _vertexPending = false;
            }
        }
        public override void SubUpdate()
        {

        }
#if UNITY_EDITOR
        public void Test()
        {
            HGUIMesh.CreateMesh(this);
            var mesh = GetComponent<MeshFilter>();
            if(mesh!=null)
            {
                mesh.sharedMesh.uv = null;
                mesh.sharedMesh.vertices = vertex;
                if (_sprite != null)
                    mesh.sharedMesh.uv = uv;
                mesh.sharedMesh.triangles = tris;
            }
            //var mr = GetComponent<MeshRenderer>();
            //if(mr!=null)
            //{
            //    mr.material = material;
            //}
        }
#endif
    }
}
