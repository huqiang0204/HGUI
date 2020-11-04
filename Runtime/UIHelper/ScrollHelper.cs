using System;
using System.Collections.Generic;
using huqiang.Data;
using huqiang.UIComposite;
using UnityEngine;


public class ScrollHelper: UIHelper
{
    public ScrollType scrollType = ScrollType.BounceBack;
    public Vector2 minBox=new Vector2(80,80);
    public Transform Slider;

    public unsafe override void ToBufferData(DataBuffer db, UITransfromData* data)
    {
        FakeStruct fake = new FakeStruct(db, ScrollInfo.ElementSize);
        ScrollInfo* info = (ScrollInfo*)fake.ip;
        info->minBox = minBox;
        info->scrollType = scrollType;
        if (Slider != null)
            info->Slider = Slider.GetInstanceID();
        else info->Slider = 0;
        data->composite = db.AddData(fake);
    }
}
