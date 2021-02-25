using huqiang.Core.HGUI;
using huqiang.Core.Line;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
using huqiang.UIModel;
using System;
using System.Collections.Generic;
using System.IO;
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
            public UIElement TipA;
            public UIElement TipB;
            public InputBox InputBox;
            public HText www;
        }
        View view;
        class ItemView
        {
            public UserEvent img;
            public HText t1;
        }
        int Index = 0;
        public override void Initial(UIElement parent, object dat = null)
        {
            base.Initial(parent, dat);
            view = LoadUI<View>("baseUI", "start");
            InitialEvent();
            InitialData();
            //view.InputBox.ReadOnly = true;
            view.InputBox.InputString = "0123456789大随风😝AA";
            CustomFont font = new CustomFont(ElementAsset.FindTexture(null, "icons"));
            var sp = ElementAsset.FindSprites(null, "icons", new string[] { "Badge1", "Badge2", "Badge3l", });
            string str = "123";
            for (int i = 0; i < 3; i++)
                font.AddCharMap(str[i], sp[i], 66);
            view.www.customFont = font;
            view.www.Text = "0123456789大随风😝AA";
            view.scrolly.ItemDockCenter = true;
            //var tb = new SerializedData.TestB();
            //tb.a = 56;
            //tb.v.x = 100;
            //tb.v.y = 66;
            //tb.h = new SerializedData.TestA();
            //tb.h.v2 = new Vector2[] { new Vector2(11, 22) };
            //tb.h.b = new string[] { "a", "b" };
            //tb.h.e = new List<string>(new string[] { "c", "d" });
            //str = tb.Save().ToString();
            //var jo = JObject.Parse(str);
            //var tc = new SerializedData.TestB();
            //tc.Load(jo);
        }
        void InitialEvent()
        {
            view.TipA.activeSelf = false;
            view.TipB.activeSelf = false;
            List<string> data = new List<string>();
            for (int i = 1000; i < 1200; i++)
                data.Add(i.ToString() + "😄");
            var scrolly = view.scrolly;
            scrolly.SetItemUpdate<ItemView, string>(ItemUpdate);
            scrolly.eventCall.PointerMove = (o, e) => {
                if (o.Pressed)
                {
                    if(view.scrolly.Point<-100)
                    {
                        view.TipA.activeSelf = true;
                        view.TipB.activeSelf = false;
                    }
                    else if (view.scrolly.Point+view.scrolly.Enity.SizeDelta.y>view.scrolly.ContentSize.y+100)
                    {
                        view.TipA.activeSelf = false;
                        view.TipB.activeSelf = true;
                    }
                    else
                    {
                        view.TipA.activeSelf = false;
                        view.TipB.activeSelf = true;
                    }
                }
            };
            scrolly.DragEnd = (o, e, v) => {
                if (view.scrolly.Point < -100)
                {
                    Index -= 20;
                    RefeshScrollY();
                }
                else if (view.scrolly.Point + view.scrolly.Enity.SizeDelta.y > view.scrolly.ContentSize.y + 100)
                {
                    Index += 20;
                    RefeshScrollY();
                }
                view.TipA.activeSelf = false;
                view.TipB.activeSelf = false;
            };
            var scrollx = view.scrollx;
            scrollx.BindingData = data;
            scrollx.SetItemUpdate<ItemView, string>(ItemUpdate);
            scrollx.Refresh();
            var grid = view.grid;
            grid.Column = 10;
            grid.BindingData = data;
            grid.SetItemUpdate<ItemView, string>(ItemUpdate);
            grid.Refresh();
            view.last.Click = (o, e) => { LoadPage<DataGridPage>(); };
            view.next.Click = (o, e) => { LoadPage<TestUPage>(); };
            RefeshScrollY();
        }
        void InitialData()
        {
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
            arc.Pos = new Vector2(0, 220);
            arc.Scale = new Vector2(1, 0.6f);
            arc.Dic = 44;
            arc.Angle = 160;
            arc.Precision = 0.05f;
            arc.Radius = 300;

            BzierLine bzier = new BzierLine();
            bzier.lineBase.Color = Color.blue;
            bzier.lineBase.Width = 2;
            bzier.A = new Vector2(-11, -22);
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
        void RefeshScrollY()
        {
            List<string> data = new List<string>();
            int c = Index;
            for (int i = 0; i < 200; i++)
            {
                data.Add(c.ToString() + "😄");
                c++;
            }
            view.scrolly.BindingData = data;
            view.scrolly.Refresh(0,view.scrolly.Point);
        }
    }
}
