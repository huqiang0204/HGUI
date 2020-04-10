using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
using System.Collections.Generic;
using Assets.Scripts;

public class LayoutPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public DesignedDockPanel Layout;
        public UserEvent last;
        public UserEvent next;
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "layout");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialLayout();
        view.last.Click = (o, e) => { LoadPage<DockPage>(); };
        view.next.Click = (o, e) => { LoadPage<TreeViewPage>(); };
    }
    void InitialLayout()
    {
        var area = view.Layout.MainContent;
        area.AddContent("page0");
        var d = area.AddArea(DockpanelArea.Dock.Down, 0.3f);
        var context = d.AddContent("page1");

        d.model.MainColor = Color.red;
        var one = d.AddArea(DockpanelArea.Dock.Right, 0.4f);
        context = one.AddContent("page2");
        context.LoadPopWindow<GridTestWindow>();

        one.model.MainColor = Color.green;
        var top = area.AddArea(DockpanelArea.Dock.Top, 0.2f);
        top.AddContent("page3");

        top.model.MainColor = Color.yellow;
        var l = top.AddArea(DockpanelArea.Dock.Left, 0.4f);
        l.model.MainColor = Color.blue;
        l.control.headDock = TabControl.HeadDock.Down;

        context = l.AddContent("page5");
        context.LoadPopWindow<GridTestWindow2>();
        view.Layout.Refresh();
    }
}
public class GridTestWindow : PopWindow
{
    class View
    {
        public ScrollY Scroll;
    }
    class Item
    {
        public HText Text;
    }
    View view;
    public override void Initial(Transform parent, UIPage ui, object obj = null)
    {
        base.Initial(parent, ui, obj);
        view = LoadUI<View>("baseUI", "gridScroll");

        List<int> testData = new List<int>();
        for (int i = 0; i < 33; i++)
            testData.Add(i);
        view.Scroll.BindingData = testData;
        view.Scroll.SetItemUpdate<Item, int>((o, e, i) => { o.Text.Text = i.ToString(); });
        view.Scroll.Refresh();
    }
}
public class GridTestWindow2 : PopWindow
{
    class View
    {
        public ScrollX Scroll;
    }
    class Item
    {
        public HText Text;
    }
    View view;
    public override void Initial(Transform parent, UIPage ui, object obj = null)
    {
        base.Initial(parent, ui, obj);
        view = LoadUI<View>("baseUI", "gridScroll");

        List<int> testData = new List<int>();
        for (int i = 0; i < 44; i++)
            testData.Add(i);
        view.Scroll.BindingData = testData;
        view.Scroll.SetItemUpdate<Item, int>((o, e, i) => { o.Text.Text = i.ToString(); });
        view.Scroll.Refresh();
    }
}
