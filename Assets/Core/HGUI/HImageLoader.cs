using System;
using huqiang.Data;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public unsafe struct HImageData
    {
        public HGraphicsData graphicsData;
        public Int32 Sprite;
        public float fillAmount;
        public bool fillCenter;
        public bool fillClockwise;
        public FillMethod fillMethod;
        public Int32 fillOrigin;
        public bool preserveAspect;
        public int filltype;
        public Int32 shader;
        public static int Size = sizeof(HGraphicsData);
        public static int ElementSize = Size / 4;
    }
    public class HImageLoader:HGraphicsLoader
    {
        protected unsafe void LoadHImage(FakeStruct fake, HImage tar)
        {
            HImageData* src = (HImageData*)fake.ip;
            string str = fake.GetData<string>(src->Sprite);
            if (str != null)
                tar.Sprite = ElementAsset.FindSprite();
        }
        protected unsafe void SaveHImage(FakeStruct fake, HImage src)
        {
            HImageData* tar = (HImageData*)fake.ip;
         
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
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            HImage image = com as HImage;
            if (image == null)
                return;
            LoadScript(fake.ip,image);
            LoadHGraphics(fake,image);
            LoadHImage(fake,image);
        }
    }
}
