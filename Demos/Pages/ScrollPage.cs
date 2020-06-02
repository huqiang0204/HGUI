using huqiang.Core.HGUI;
using System;
using UnityEngine;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang;
using huqiang.Data;
using System.Collections.Generic;
using huqiang.UIModel;

public class ScrollPage:UIPage
{
    //反射UI界面上的物体
    class View
    {
        public ScrollYExtand scrollY;
        public ScrollY scroll;
        public DropdownEx dropDown;
        public ScrollX scrollX;
        public UserEvent last;
        public UserEvent next;
    }
    class TitleItem
    {
        public UserEvent Image;
        public HText Text;
    }
    class SubItem
    {
        public HText Text;
    }
    View view;
    public override void Initial(Transform parent, object dat = null)
    {
        base.Initial(parent, dat);
        view = LoadUI<View>("baseUI", "scrollex");//"baseUI"创建的bytes文件名,"page"为创建的页面名
        InitialScrollEx();
        InitialScrollY();
        view.last.Click = (o, e) => { LoadPage<DrawingPage>(); };
        view.next.Click = (o, e) => { LoadPage<TabPage>(); };
    }
    void InitialScrollEx()
    {
        List<ScrollYExtand.DataTemplate> datas = new List<ScrollYExtand.DataTemplate>();
        ScrollYExtand.DataTemplate tmp = new ScrollYExtand.DataTemplate();
        tmp.Title = "test1";
        tmp.Tail = "over1";
        List<string> list = new List<string>();
        for (int i = 0; i < 22; i++)
            list.Add("tttt" + i.ToString());
        tmp.Hide = true;
        tmp.Data = list;
        datas.Add(tmp);

        tmp = new ScrollYExtand.DataTemplate();
        tmp.Title = "test2";
        tmp.Tail = "over2";
        list = new List<string>();
        for (int i = 0; i < 11; i++)
            list.Add("tttt" + i.ToString());
        tmp.Hide = true;
        tmp.Data = list;
        datas.Add(tmp);

        tmp = new ScrollYExtand.DataTemplate();
        tmp.Title = "test3";
        tmp.Tail = "over3";
        list = new List<string>();
        for (int i = 0; i < 7; i++)
            list.Add("tttt" + i.ToString());
        tmp.Hide = true;
        tmp.Data = list;
        datas.Add(tmp);

        ScrollYExtand scrollY = view.scrollY;
        scrollY.BindingData = datas;
        scrollY.SetTitleUpdate<TitleItem, ScrollYExtand.DataTemplate>(TitleUpdate);
        scrollY.SetItemUpdate<SubItem, string>(ItemUpdate);
        scrollY.SetTailUpdate<TitleItem, ScrollYExtand.DataTemplate>(TailUpdate);
        scrollY.Refresh();
    }
    ScrollYExtand.DataTemplate current;
    void TitleUpdate(TitleItem title, ScrollYExtand.DataTemplate data, int index)
    {
        title.Text.Text = data.Title as string;
        title.Image.DataContext = data;
        title.Image.Click = (o, e) => {
            var dt = o.DataContext as ScrollYExtand.DataTemplate;
            if (dt.Hide)
            {
                view.scrollY.OpenSection(dt);
                if (current != dt)
                {
                    view.scrollY.HideSection(current);
                }
                current = dt;
            }
            else
            {
                view.scrollY.HideSection(dt);
                if (dt == current)
                    current = null;
            }
        };
    }
    void TailUpdate(TitleItem title, ScrollYExtand.DataTemplate data, int index)
    {
        title.Text.Text = data.Tail as string;
    }
    void ItemUpdate(SubItem item, string data, int index)
    {
        item.Text.Text = data;
    }
    void InitialScrollY()
    {
        List<string> data = new List<string>();
        for (int i = 1000; i < 1400; i++)
            data.Add("Num:"+i.ToString());
        view.scroll.BindingData = data;
        view.scroll.SetItemUpdate<SubItem, string>(ItemUpdate);
        view.scroll.Refresh();
        view.scrollX.BindingData = data;
        view.scrollX.SetItemUpdate<SubItem, string>(ItemUpdate);
        view.scrollX.Refresh();
        view.dropDown.scrollY.BindingData = data;
        view.dropDown.scrollY.Refresh();
    }
}
