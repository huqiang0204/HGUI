using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Core.HGUI;

namespace huqiang.UIComposite
{
    public class UIDate : Composite
    {
        class ItemView
        {
            public HText Item;
        }
        UserEvent callBack;
        ScrollY Year;
        ScrollY Month;
        ScrollY Day;
        public int StartYear = 1800;
        public int EndYear = 2400;
        int[] ys;
        string[] ms;
        List<string> Days;
        string unitY = " Year";
        string unitM = " Month";
        string unitD = " Day";
        public string YearUnit
        {
            get { return unitY; }
            set
            {
                unitY = value;
                var its = Year.Items;
                for (int i = 0; i < its.Count; i++)
                {
                    var it = its[i];
                    var ele = Year.Items[i].obj as HText;
                    if (ele != null)
                    {
                        ele.Text = it.datacontext.ToString() + value;
                    }
                }
            }
        }
        public string MonUnit
        {
            get { return unitM; }
            set
            {
                unitM = value;
                var its = Month.Items;
                for (int i = 0; i < its.Count; i++)
                {
                    var it = its[i];
                    var ele = Year.Items[i].obj as HText;
                    if (ele != null)
                    {
                        ele.Text = it.datacontext.ToString() + value;
                    }
                }
            }
        }
        public string DayUnit
        {
            get { return unitD; }
            set
            {
                unitD = value;
                var its = Day.Items;
                for (int i = 0; i < its.Count; i++)
                {
                    var it = its[i];
                    var ele = Year.Items[i].obj as HText;
                    if (ele != null)
                    {
                        ele.Text = it.datacontext as string + value;
                    }
                }
            }
        }
        public override void Initial(FakeStruct mod,UIElement element)
        {
            base.Initial(mod,element);
            var mui = Enity.GetComponent<UIElement>();
            mui.userEvent.CutRect = true;
            var mask = Enity.transform;
            Year = mask.Find("Year").GetComponent<UIElement>().composite as ScrollY;
            Year.SetItemUpdate<ItemView, int>((o, e, i) => { o.Item.Text = e.ToString(); });
            Year.Scroll = Scrolling;
            Year.ScrollEnd = YearScrollToEnd;
            Year.ItemDockCenter = true;
            Year.scrollType = ScrollType.Loop;
            Year.eventCall.BoxAdjuvant = new Vector2(0, -200);

            Month = mask.Find("Month").GetComponent<UIElement>().composite as ScrollY;
            Month.SetItemUpdate<ItemView, string>((o, e, i) => { o.Item.Text = e; });
            Month.Scroll = Scrolling;
            Month.ScrollEnd = MonthScrollToEnd;
            Month.ItemDockCenter = true;
            Month.scrollType = ScrollType.Loop;
            Month.eventCall.BoxAdjuvant = new Vector2(0, -200);

            Day = mask.Find("Day").GetComponent<UIElement>().composite as ScrollY;
            Day.SetItemUpdate<ItemView, string>((o, e, i) => { o.Item.Text = e ; });
            Day.Scroll = Scrolling;
            Day.ScrollEnd = DayScrollToEnd;
            Day.ItemDockCenter = true;
            Day.ScrollEnd = DayScrollToEnd;
            Day.scrollType = ScrollType.Loop;
            Day.eventCall.BoxAdjuvant = new Vector2(0, -200);

            unsafe
            {
                var ex = mod.buffer.GetData(((TransfromData*)mod.ip)->ex) as FakeStruct;
                if (ex != null)
                {
                    StartYear = ex[0];
                    EndYear = ex[1];
                    if (EndYear < StartYear)
                        EndYear = StartYear;
                }
            }
        
            year = StartYear;
            month = 1;
            day = 1;
            int len = EndYear - StartYear;
            ys = new int[len];
            int s = StartYear;
            for (int i = 0; i < len; i++)
            { ys[i] = s; s++; }
            Year.BindingData = ys;
            Year.Refresh();
            ms = new string[12];
            for (int i = 0; i < 12; i++)
                ms[i] = (i + 1).ToString();
            Month.BindingData = ms;
            Month.Refresh();
            Days = new List<string>();
            for (int i = 0; i < 31; i++)
                Days.Add((i + 1).ToString());
            Day.BindingData = Days;
            Day.Refresh();
            UpdateItems(Year);
            UpdateItems(Month);
            UpdateItems(Day);
        }
        void Scrolling(ScrollY scroll, Vector2 vector)
        {
            UpdateItems(scroll);
        }
        void UpdateItems(ScrollY scroll)
        {
            var items = scroll.Items;
            float hy = scroll.Enity.SizeDelta.y * 0.5f;
            for (int i = 0; i < items.Count; i++)
            {
                var mod = items[i].target;
                var txt = (items[i].obj as ItemView).Item;
                float h = txt.SizeDelta.y;
                float y = mod.localPosition.y;
                float angle = y / h * 15f;
                float r = y / hy;
                if (r < 0)
                    r = -r;
                r = 1 - r;
                if (r < 0)
                    r = 0;
                mod.localRotation = Quaternion.Euler(angle, 0, 0);
                var v = MathH.Tan2(90 - angle);
                mod.localPosition =new Vector3(0, v.y * 100,0);
                var col = txt.MainColor;
                col.a = (byte)(r*255);
                txt.MainColor = col;
            }
        }
        void YearScrollToEnd(ScrollY scroll)
        {
            var item = ScrollY.GetCenterItem(scroll.Items);
            if (item == null)
                return;
            year = ys[item.index];
            RefreshDay();
        }
        void MonthScrollToEnd(ScrollY scroll)
        {
            var item = ScrollY.GetCenterItem(scroll.Items);
            month = item.index + 1;
            RefreshDay();
        }
        void DayScrollToEnd(ScrollY scroll)
        {
            var item = ScrollY.GetCenterItem(scroll.Items);
            day = item.index + 1;
        }
        public int year;
        public int month = 1;
        public int day = 1;
        static int[] daysTable = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        void RefreshDay()
        {
            int a = daysTable[month - 1];
            if (a == 1)
                if (year % 4 == 0)//闰二月
                    a++;
            Days.Clear();
            for (int i = 0; i < a; i++)
                Days.Add((i + 1).ToString());
            Day.Refresh(0, Day.Point);
            UpdateItems(Day);
        }
    }
}

