using System;
using huqiang.Data;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public unsafe struct HImageData
    {
        public HGraphicsData graphicsData;
        public Int32 Sprite;
        public float fillAmount;
        public bool fillCenter;
        public bool fillClockwise;
        public FillMethod fillMethod;
        public Int32 fillOrigin;//方向
        public float pixelsPerUnit;
        public static int Size = sizeof(HImageData);
        public static int ElementSize = Size / 4;
    }
    public class HImageLoader:HGraphicsLoader
    {
        protected string Sprite;
        protected unsafe void LoadHImage(FakeStruct fake, HImage tar)
        {
            HImageData* src = (HImageData*)fake.ip;
            Sprite = fake.buffer.GetData(src->Sprite)as string;
            if (Sprite != null)
                tar.Sprite = ElementAsset.FindSprite(asset, MainTexture, Sprite);
            else tar.Sprite = null;
            tar.m_fillAmount = src->fillAmount;
            tar.m_fillCenter = src->fillCenter;
            tar.m_fillClockwise = src->fillClockwise;
            tar.m_fillMethod = src->fillMethod;
            tar.m_fillOrigin = src->fillOrigin;
            tar.m_pixelsPerUnit = src->pixelsPerUnit;
        }
        protected unsafe void SaveHImage(FakeStruct fake, HImage src)
        {
            HImageData* tar = (HImageData*)fake.ip;
            tar->fillAmount = src.m_fillAmount;
            tar->fillCenter = src.m_fillCenter;
            tar->fillClockwise = src.m_fillClockwise;
            tar->fillMethod = src.m_fillMethod;
            tar->fillOrigin = src.m_fillOrigin;
            tar->pixelsPerUnit = src.m_pixelsPerUnit;
            var sprite = src.m_sprite;
            if (sprite != null)
                tar->Sprite = fake.buffer.AddData(sprite.name);
        }
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            HImage image = com.GetComponent<HImage>();
            if (image == null)
                return;
            LoadScript(fake.ip, image);
            LoadHGraphics(fake, image);
            LoadHImage(fake, image);
        }
        public unsafe override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as HImage;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HImageData.ElementSize);
            SaveScript(fake.ip, src);
            SaveHGraphics(fake, src);
            SaveHImage(fake,src);
            return fake;
        }

    }
}
