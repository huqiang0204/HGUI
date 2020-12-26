using huqiang.Data;
using huqiang.Helper.HGUI;
using huqiang.UIComposite;
using System;
using UnityEngine;

public class StackPanelHelper:UIHelper
{
    public Direction direction = Direction.Horizontal;
    public float spacing = 0;
    public bool FixedSize;
    public float FixedSizeRatio = 1;
    public float ItemOffset = 0;
    void Order()
    {
        switch (direction)
        {
            case Direction.Horizontal:
                OrderHorizontal();
                break;
            case Direction.Vertical:
                OrderVertical();
                break;
        }
    }
    void OrderHorizontal()
    {
        var scr = GetComponent<UIElement>();
        if (scr != null)
        {
            float ps = scr.SizeDelta.x;
            if (FixedSizeRatio > 0)
                ps *= FixedSizeRatio;
            var sx = scr.SizeDelta.x * (scr.Pivot.x - 1);
            var trans = scr.transform;
            var c = trans.childCount;
            float ox = ItemOffset;
            for (int i = 0; i < c; i++)
            {
                var son = trans.GetChild(i);
                var ss = son.GetComponent<UIElement>();
                float w = 0;
                float p = 0.5f;
                if (ss != null)
                {
                    if (FixedSize)
                    {
                        w = ps;
                        ox = ItemOffset * w;
                    }
                    else
                    {
                        w = ss.SizeDelta.x;
                    }
                    p = ss.Pivot.x;
                }
                float os = sx - w * -p + ox;
                son.localPosition = new Vector3(os, 0, 0);
                son.localScale = Vector3.one;
                sx += w + spacing;
            }
        }
    }
    void OrderVertical()
    {
        var scr = GetComponent<UIElement>();
        if (scr != null)
        {
            float ps = scr.SizeDelta.y;
            if (FixedSizeRatio > 0)
                ps *= FixedSizeRatio;
            var sy = scr.SizeDelta.y * (1 - scr.Pivot.y);
            var trans = scr.transform;
            var c = trans.childCount;
            float oy = ItemOffset;
            for (int i = 0; i < c; i++)
            {
                var son = trans.GetChild(i);
                var ss = son.GetComponent<UIElement>();
                float h = 0;
                float p = 0.5f;
                if (ss != null)
                {
                    if (FixedSize)
                    {
                        h = ps;
                        oy = h * ItemOffset;
                    }
                    else
                    {
                        h = ss.SizeDelta.y; 
                    }
                    p = ss.Pivot.y;
                }
                float os = sy + h * (p-1)-oy;
                son.localPosition = new Vector3(0, os, 0);
                son.localScale = Vector3.one;
                sy -= h+spacing;
            }
        }
    }
    public override void Refresh()
    {
        Order();
    }
    public unsafe override void ToBufferData(DataBuffer db, huqiang.Core.UIData.UIElementData* data)
    {
        FakeStruct fake = new FakeStruct(db,5);
        fake[0] = (int)direction;
        fake.SetFloat(1,spacing);
        fake[2] = FixedSize?1:0;
        fake.SetFloat(3,FixedSizeRatio);
        fake.SetFloat(4,ItemOffset);
        int type = db.AddData("StackPanelHelper");
        int index = db.AddData(fake);
        data->composite = type << 16 | index;
    }
    public override unsafe void LoadFromBuffer(FakeStruct fake,UIInitializer initializer)
    {
        direction = (Direction)fake[0];
        spacing = fake.GetFloat(1);
        FixedSize = fake[2] == 1;
        FixedSizeRatio = fake.GetFloat(3);
        ItemOffset = fake.GetFloat(4);
    }
}
