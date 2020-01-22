using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
public class DrawingPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "drawing");//"baseUI"创建的bytes文件名,"page"为创建的页面名
    }
}
