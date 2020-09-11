using huqiang.Core.HGUI;
using huqiang.Core.Line;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang.UIModel;
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
            public GridScroll grid;
            public UserEvent last;
            public UserEvent next;
            public HLine Line;
        }
        View view;
        class ItemView
        {
            public UserEvent img;
            public HText t1;
        }
        public override void Initial(Transform parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "start");
            List<string> data = new List<string>();
            for (int i = 1000; i < 1200; i++)
                data.Add(i.ToString()+"😄");
            var scrolly = view.scrolly;
            scrolly.BindingData = data;
            scrolly.SetItemUpdate<ItemView, string>(ItemUpdate);
            scrolly.Refresh();
            var scrollx = view.scrollx;
            scrollx.BindingData = data;
            scrollx.SetItemUpdate<ItemView, string>(ItemUpdate);
            scrollx.Refresh();
            var grid = view.grid;
            grid.Column = 10;
            grid.BindingData = data;
            grid.SetItemUpdate<ItemView, string>(ItemUpdate);
            grid.Refresh();
            view.last.Click = (o, e) => { LoadPage<TreeViewPage>(); };
            view.next.Click = (o, e) => { LoadPage<TestUPage>(); };
            Beeline beeline = new Beeline();
            beeline.lineBase.Color = Color.red;
            beeline.lineBase.Width = 2;
            beeline.Start.x = -100;
            beeline.End.x = 100;
            beeline.End.y = 100;

            ArcLine arc = new ArcLine();
            arc.Closed = true;
            arc.lineBase.Color = Color.green;
            arc.lineBase.Width = 2;
            arc.Pos = new Vector2(0,220);
            arc.Scale = new Vector2(1,0.6f);
            arc.Dic = 44;
            arc.Angle = 160;
            arc.Precision = 0.05f;
            arc.Radius = 300;

            BzierLine bzier = new BzierLine();
            bzier.lineBase.Color = Color.blue;
            bzier.lineBase.Width = 2;
            bzier.A = new Vector2(-11,-22);
            bzier.B = new Vector2(0, -220);
            bzier.C = new Vector2(100, 46);
            bzier.Precision = 0.04f;

            view.Line.AddLine(ref beeline);
            view.Line.AddLine(ref arc);
            view.Line.AddLine(ref bzier);
        }
        void ItemUpdate(ItemView item,string dat,int index)
        {
            item.t1.Text = dat;
            item.img.DataContext = index;
            item.img.Click = ItemClick;

        }
        void ItemClick(UserEvent user, UserAction action)
        {
            Debug.Log("Item: " + (int)user.DataContext + " Click !");
        }
    }
}
