using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            return base.LoadFromObject(com, buffer);
        }
        public override void LoadToObject(FakeStruct fake, Component com)
        {
            base.LoadToObject(fake, com);
        }
    }
}
