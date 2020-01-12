using huqiang.Core.HGUI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class StartPage:UIPage
    {
        class View
        {
            public ScrollY scrolly;
            public ScrollX scrollx;
            public UISlider slider;
        }
        View view;
        class ItemView
        {
            public UserEvent img;
            public HLabel t1;
        }
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "start");
            List<string> data = new List<string>();
            for (int i = 1000; i < 1200; i++)
                data.Add(i.ToString()+"😄");
            view.scrolly.BindingData = data;
            view.scrolly.SetItemUpdate<ItemView, string>(ItemUpdate);
            view.scrolly.Refresh();
            view.scrollx.BindingData = data;
            view.scrollx.SetItemUpdate<ItemView, string>(ItemUpdate);
            view.scrollx.Refresh();
        }
        void ItemUpdate(ItemView item,string dat,int index)
        {
            item.t1.Text = dat;
        }
    }
}
