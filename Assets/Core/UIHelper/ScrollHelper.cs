using System;
using System.Collections.Generic;
using huqiang.Data;
using huqiang.UIComposite;
using UnityEngine;


public class ScrollHelper: UICompositeHelp
{
    public ScrollType scrollType = ScrollType.Loop;
    public Vector2 minBox=new Vector2(80,80);

    public unsafe override object ToBufferData(DataBuffer data)
    {
        FakeStruct fake = new FakeStruct(data, ScrollInfo.ElementSize);
        ScrollInfo* info = (ScrollInfo*)fake.ip;
        info->minBox = minBox;
        info->scrollType = scrollType;
        return fake;
    }
}
