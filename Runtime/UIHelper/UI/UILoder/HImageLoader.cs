﻿using System;
using huqiang.Core.UIData;
using huqiang.Data;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
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
    public class HImageLoader:HGraphicsLoader
    {
        protected string Sprite;
        public FakeStructHelper ImageHelper;
        protected  void LoadHImage(FakeStruct fake, HImage hi)
        {
            HImageData tmp;
            unsafe
            {
                tmp = *(HImageData*)fake.ip;
            }
            if (hi.Content == null)
                hi.Content = new Core.HGUI.HImage();
            var tar = hi.Content;
            Sprite = fake.buffer.GetData(tmp.Sprite) as string;
            if (Sprite != null)
                tar.Sprite = ElementAsset.FindSprite(asset, MainTexture, Sprite);
            else {
                tar.Sprite = null;
                if (MainTexture != null)
                    tar.MainTexture = ElementAsset.FindTexture(asset,MainTexture);
            }
            tar.m_spriteType = tmp.spriteType;
            tar.m_fillAmount =tmp.fillAmount;
            tar.m_fillCenter = tmp.fillCenter;
            tar.m_fillClockwise = tmp.fillClockwise;
            tar.m_fillMethod = tmp.fillMethod;
            tar.m_fillOrigin = tmp.fillOrigin;
            tar.m_pixelsPerUnit = tmp.pixelsPerUnit;
            tar.m_preserveAspect = tmp.preserveAspect;
        }
        protected unsafe void SaveHImage(FakeStruct fake, HImage hi)
        {
            var src = hi.Content;
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
        public unsafe override void LoadToComponent(FakeStruct fake, Component com,FakeStruct main)
        {
            HImage image = com.GetComponent<HImage>();
            if (image == null)
                return;
            LoadElement(fake, image);
            LoadHGraphics(fake, image);
            LoadHImage(fake, image);
            //image.Initial(main,initializer as UIInitializer);
        }
        public unsafe override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as HImage;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HImageData.ElementSize);
            //SaveScript(fake.ip, src);
            //SaveHGraphics(fake, src);
            //SaveHImage(fake,src);
            return fake;
        }
    }
}
