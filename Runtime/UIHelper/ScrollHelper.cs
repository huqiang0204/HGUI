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
        {
            var ui = Slider.GetComponent<UIContext>();
            if (ui != null)
                info->Slider = ui.GetUIData().GetInstanceID();
        }
        else info->Slider = 0;
        int type = db.AddData("ScrollHelper");
        int index = db.AddData(fake);
        data->composite = type << 16 | index;
    }
    public unsafe override void LoadFromBuffer(FakeStruct fake,Initializer initializer)
    {
        ScrollInfo* info = (ScrollInfo*)fake.ip;
        minBox = info->minBox;
        scrollType = info->scrollType;
        int id = info->Slider;
        if (id != 0)
        {
            if (initializer != null)
                initializer.AddContextAction(ContextAction, id);
        }
    }
    void ContextAction(Transform element)
    {
        if(element==null)
        {
            Debug.LogError("Context is null: "+ name);
        }else
        Slider = element;
    }
}
