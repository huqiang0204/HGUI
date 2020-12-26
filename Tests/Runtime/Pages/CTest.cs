using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
#if Hot
using huqiang.HotUIModel;
#else
using huqiang.UIModel;
#endif
public class CTest:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public InputBox InputBox;
    }
    View view;
    public override void Initial(UIElement parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "CTest");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialEvent();
    }
    void InitialEvent()
    {
        
    }

}
