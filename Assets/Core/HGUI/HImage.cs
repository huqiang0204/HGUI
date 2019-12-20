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
        Sprite _sprite;
        Rect _rect;
        Vector4 _border;
        Vector2 _pivot;
        public Sprite sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
                _rect = value.rect;
                _border = value.border;
                _pivot = value.pivot;
            }
        }
        public SpriteType type { get; set; }
        public FillMethod fillMethod { get; set; }
        public int fillOrigin { get; set; }
        public bool preserveAspect { get; set; }
       
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
            switch(type)
            {
                case SpriteType.Simple://单一类型
                    CreateSimpleMesh();
                    break;
                case SpriteType.Sliced://9宫格,中间部分为拉伸
                    CreateSlicedMesh();
                    break;
                case SpriteType.Filled://填充类型
                    CreateFilledMesh();
                    break;
                case SpriteType.Tiled://9宫格,中间部分为平铺
                    CreateTiledMesh();
                    break;
            }
        }
        void CreateSimpleMesh()
        {
            vertex = new Vector3[4];
            float x = SizeDelta.x * 0.5f;
            float y = SizeDelta.y * 0.5f;
            vertex[0].x = -x;
            vertex[0].y = -y;
            vertex[1].x = -x;
            vertex[1].y = y;
            vertex[2].x = x;
            vertex[2].y = y;
            vertex[3].x = x;
            vertex[3].y = -y;
            tris = Triangle.Rectangle;
        }
        void CreateSlicedMesh()
        {

        }
        void CreateFilledMesh()
        {

        }
        void CreateTiledMesh()
        {

        }
        public override void SubUpdate()
        {

        }
    }
}
