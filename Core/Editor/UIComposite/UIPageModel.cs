using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIPageModel
{
    public static string GetPageModel(string pageName,string uiName)
    {
        return PageModelFront + pageName + PageModelRear+uiName+PageModelEnd;
    }
    private const string PageModelFront =
        @"using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
public class ";
    private const string PageModelRear = @":UIPage
{
    //反射UI界面上的物体
    class View
    {
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>(""baseUI"", """;
    private const string PageModelEnd = @""");//""baseUI""创建的bytes文件名,""page""为创建的页面名
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
}";
}
