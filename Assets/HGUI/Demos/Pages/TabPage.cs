using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;

public class TabPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public UIElement tab;
        public UserEvent last;
        public UserEvent next;
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "tabcontroll");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialTab();
        view.last.Click = (o, e) => { LoadPage<ScrollPage>(); };
        view.next.Click = (o, e) => { LoadPage<DockPage>(); };
    }
    void InitialTab()
    {
        TabControl tab = view.tab.composite as TabControl;
        var img = UICreator.CreateHImage(Vector3.zero,Vector2.zero,"con1",null);
        img.marginType = MarginType.Margin;
        img.MainColor = Color.red;
        tab.AddContent(img,"标签1");
        img = UICreator.CreateHImage(Vector3.zero, Vector2.zero, "con2", null);
        img.marginType = MarginType.Margin;
        img.MainColor = Color.green;
        tab.AddContent(img,"标签2");
        img = UICreator.CreateHImage(Vector3.zero, Vector2.zero, "con3", null);
        img.marginType = MarginType.Margin;
        img.MainColor = Color.blue;
        tab.AddContent(img,"标签3");
    }
}
