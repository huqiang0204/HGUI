using System;
using System.Collections.Generic;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;
using UnityEngine;

public class SliderHelper : UIHelper
{
    public Vector2 StartOffset;
    public Vector2 EndOffset;
    public float MinScale=1;
    public float MaxScale=1;
    public UISlider.Direction direction;
    UISlider slider;
    public unsafe override void ToBufferData(DataBuffer db, UITransfromData* data)
    {
        FakeStruct fake = new FakeStruct(db, SliderInfo.ElementSize);
        SliderInfo* si = (SliderInfo*)fake.ip;
        si->StartOffset = StartOffset;
        si->EndOffset = EndOffset;
        si->MinScale = MinScale;
        si->MaxScale = MaxScale;
        si->direction = direction;
        data->composite = db.AddData(fake);
    }
}