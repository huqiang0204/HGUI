using huqiang.Core.HGUI;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class StackPanelHelper:UICompositeHelp
{
    public Direction direction = Direction.Horizontal;
    public float spacing = 0;
    public bool FixedSize;
    public float FixedSizeRatio = 1;
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
            var y = scr.SizeDelta.y;
            var trans = scr.transform;
            var c = trans.childCount;
            for(int i=0;i<c;i++)
            {
                var son = trans.GetChild(i);
                var ss = son.GetComponent<UIElement>();
                float w = 0;
                float p = 0.5f;
                if (ss!=null)
                {
                   if(FixedSize)
                    {
                        var s = ss.SizeDelta;
                        s.x = ps;
                        ss.SizeDelta = s;
                    }
                    w = ss.SizeDelta.x;
                    p = ss.Pivot.x;
                }
                float os = sx - w * (p-1);
                son.localPosition = new Vector3(os,0,0);
                son.localScale = Vector3.one;
                sx += w+spacing;
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
            var x = scr.SizeDelta.x;
            var trans = scr.transform;
            var c = trans.childCount;
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
                        var s = ss.SizeDelta;
                        s.y = ps;
                        ss.SizeDelta = s;
                    }
                    h = ss.SizeDelta.y;
                    p = ss.Pivot.y;
                }
                float os = sy + h * (p - 1);
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
}
