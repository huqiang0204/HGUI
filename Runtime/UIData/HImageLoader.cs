using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.UIData
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
    public unsafe struct HImageData
    {
        public HGraphicsData graphicsData;
        public Int32 Sprite;
        public SpriteType spriteType;
        public float fillAmount;
        public bool fillCenter;
        public bool fillClockwise;
        public FillMethod fillMethod;
        public Int32 fillOrigin;//方向
        public float pixelsPerUnit;
        public bool preserveAspect;
        public static int Size = sizeof(HImageData);
        public static int ElementSize = Size / 4;
    }
    public class HImageLoader : HGraphicsLoader
    {
        protected string Sprite;
        public FakeStructHelper ImageHelper;
        protected void LoadHImage(FakeStruct fake, HImage tar)
        {
            HImageData tmp = new HImageData();
            unsafe
            {
                HImageData* src = &tmp;
                ImageHelper.LoadData((byte*)src, fake.ip);
                Sprite = fake.buffer.GetData(src->Sprite) as string;
            }
            if (Sprite != null)
                tar.Sprite = ElementAsset.FindSprite(asset, MainTexture, Sprite);
            else
            {
                tar.Sprite = null;
                if (MainTexture != null)
                    tar.MainTexture = ElementAsset.FindTexture(asset, MainTexture);
            }
            tar.m_spriteType = tmp.spriteType;
            tar.m_fillAmount = tmp.fillAmount;
            tar.m_fillCenter = tmp.fillCenter;
            tar.m_fillClockwise = tmp.fillClockwise;
            tar.m_fillMethod = tmp.fillMethod;
            tar.m_fillOrigin = tmp.fillOrigin;
            tar.m_pixelsPerUnit = tmp.pixelsPerUnit;
            tar.m_preserveAspect = tmp.preserveAspect;
        }
        protected unsafe void SaveHImage(Helper.HGUI.HImage src, FakeStruct fake)
        {
            HImageData* tar = (HImageData*)fake.ip;
            tar->spriteType = src.m_spriteType;
            tar->fillAmount = src.m_fillAmount;
            tar->fillCenter = src.m_fillCenter;
            tar->fillClockwise = src.m_fillClockwise;
            tar->fillMethod = src.m_fillMethod;
            tar->fillOrigin = src.m_fillOrigin;
            tar->pixelsPerUnit = src.m_pixelsPerUnit;
            tar->preserveAspect = src.m_preserveAspect;
            var sprite = src.m_sprite;
            if (sprite != null)
                tar->Sprite = fake.buffer.AddData(sprite.name);
        }
        protected unsafe void SaveHImage(HGUI.HImage src, FakeStruct fake)
        {
            HImageData* tar = (HImageData*)fake.ip;
            tar->spriteType = src.m_spriteType;
            tar->fillAmount = src.m_fillAmount;
            tar->fillCenter = src.m_fillCenter;
            tar->fillClockwise = src.m_fillClockwise;
            tar->fillMethod = src.m_fillMethod;
            tar->fillOrigin = src.m_fillOrigin;
            tar->pixelsPerUnit = src.m_pixelsPerUnit;
            tar->preserveAspect = src.m_preserveAspect;
            var sprite = src.m_sprite;
            if (sprite != null)
                tar->Sprite = fake.buffer.AddData(sprite.name);
        }
        public unsafe override void LoadUI(UIElement com, FakeStruct fake, UIInitializer initializer)
        {
            HImage image = com as HImage;
            if (image == null)
                return;
            LoadHGraphics(image,fake);
            LoadHImage(fake, image);
            LoadUIElement(image,fake, initializer);
        }
        public unsafe override FakeStruct SaveUI(UIElement com, DataBuffer buffer)
        {
            var src = com as HGUI.HImage;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HImageData.ElementSize);
            
            SaveHGraphics(src,fake);
            SaveHImage(src,fake);
            SaveUIElement(src, fake);
            return fake;
        }
        public unsafe override FakeStruct SaveUI(Component com, DataBuffer buffer)
        {
            var src = com.GetComponent<Helper.HGUI.HImage>();
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HImageData.ElementSize);
            SaveUIElement(src, fake);
            SaveHGraphics(src, fake);
            SaveHImage(src,fake);
            return fake;
        }
    }
}
