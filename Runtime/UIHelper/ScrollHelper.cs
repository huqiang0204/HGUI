using System;
using huqiang.Data;
using huqiang.Helper.HGUI;
using huqiang.UIComposite;
using UnityEngine;


public class ScrollHelper: UIHelper
{
    public ScrollType scrollType = ScrollType.BounceBack;
    public Vector2 minBox=new Vector2(80,80);
    public Transform Slider;

    public unsafe override void ToBufferData(DataBuffer db, huqiang.Core.UIData.UIElementData* data)
    {
        FakeStruct fake = new FakeStruct(db, ScrollInfo.ElementSize);
        ScrollInfo* info = (ScrollInfo*)fake.ip;
        info->minBox = minBox;
        info->scrollType = scrollType;
        if (Slider != null)
            info->Slider = Slider.GetInstanceID();
        else info->Slider = 0;
        int type = db.AddData("ScrollHelper");
        int index = db.AddData(fake);
        data->composite = type << 16 | index;
    }
    public unsafe override void LoadFromBuffer(FakeStruct fake,UIInitializer initializer)
    {
        ScrollInfo* info = (ScrollInfo*)fake.ip;
        minBox = info->minBox;
        scrollType = info->scrollType;
        int id = info->Slider;
        if (id != 0)
        {
            initializer.AddContextAction(ContextAction, id);
        }
    }
    void ContextAction(UIElement element)
    {
        Slider = element.transform;
    }
}
