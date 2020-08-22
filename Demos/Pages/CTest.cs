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
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "CTest");//"baseUI"创建的bytes文件名,"page"为创建的页面名
    }
    //语言切换功能用
    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
    }
    //接收消息
    public override void Cmd(DataBuffer dat)
    {
        base.Cmd(dat);
    }
    //页面弹窗
    public override T PopUpWindow<T>(object obj = null)
    {
        return base.PopUpWindow<T>(obj);
    }
}
