using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
public class DockPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public DockPanel dock;
        public UserEvent last;
        public UserEvent next;
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "dockpanel");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialDockPanel();
        view.last.Click = (o, e) => { LoadPage<TabPage>(); };
        view.next.Click = (o, e) => { LoadPage<LayoutPage>(); };
    }
    void InitialDockPanel()
    {
        var area = view.dock.MainArea.AddArea(DockpanelArea.Dock.Down);
        area.model.MainColor = Color.red;
        var rightdown = area.AddArea(DockpanelArea.Dock.Right);
        rightdown.model.MainColor = Color.green;
        var lefttop = view.dock.MainArea.AddArea(DockpanelArea.Dock.Left);
        lefttop.model.MainColor = Color.blue;
        view.dock.Refresh();
    }
}
