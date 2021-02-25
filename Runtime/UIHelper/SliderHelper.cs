using System;
using huqiang.Data;
using huqiang.Helper.HGUI;
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
    public unsafe override void ToBufferData(DataBuffer db, huqiang.Core.UIData.UIElementData* data)
    {
        FakeStruct fake = new FakeStruct(db, SliderInfo.ElementSize);
        SliderInfo* si = (SliderInfo*)fake.ip;
        si->StartOffset = StartOffset;
        si->EndOffset = EndOffset;
        si->MinScale = MinScale;
        si->MaxScale = MaxScale;
        si->direction = direction;
        int type = db.AddData("SliderHelper");
        int index = db.AddData(fake);
        data->composite = type << 16 | index;
    }
    public unsafe override void LoadFromBuffer(FakeStruct fake, Initializer initializer)
    {
        SliderInfo* si = (SliderInfo*)fake.ip;
        StartOffset = si->StartOffset;
        EndOffset = si->EndOffset;
        MinScale = si->MinScale;
        MaxScale = si->MaxScale;
        direction = si->direction;
    }
}