using huqiang.Core.HGUI;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StackPanelHelper:UICompositeHelp
{
    public Direction direction = Direction.Horizontal;
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
            var sx = scr.SizeDelta.x * -0.5f;
            var y = scr.SizeDelta.y;
            var trans = scr.transform;
            var c = trans.childCount;
            for(int i=0;i<c;i++)
            {
                var son = trans.GetChild(i);
                var ss = son.GetComponent<UIElement>();
                float w = 0;
                if(ss!=null)
                {
                    w = ss.SizeDelta.x;
                }
                float os = sx + w * 0.5f;
                son.localPosition = new Vector3(os,0,0);
                son.localScale = Vector3.one;
                sx += w;
            }
        }
    }
    void OrderVertical()
    {
        var scr = GetComponent<UIElement>();
        if (scr != null)
        {
            var sy = scr.SizeDelta.y * 0.5f;
            var x = scr.SizeDelta.x;
            var trans = scr.transform;
            var c = trans.childCount;
            for (int i = 0; i < c; i++)
            {
                var son = trans.GetChild(i);
                var ss = son.GetComponent<UIElement>();
                float h = 0;
                if (ss != null)
                {
                    h = ss.SizeDelta.y;
                }
                float os = sy - h * 0.5f;
                son.localPosition = new Vector3(0, os, 0);
                son.localScale = Vector3.one;
                sy -= h;
            }
        }
    }
    public override void Refresh()
    {
        Order();
    }
}
