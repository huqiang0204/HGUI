using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.Core.UIData;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 日历组件
    /// </summary>
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
        /// <summary>
        /// 开始的年份
        /// </summary>
        public int StartYear = 1800;
        /// <summary>
        /// 结束的年份
        /// </summary>
        public int EndYear = 2400;
        int[] ys;
        string[] ms;
        List<string> Days;
        string unitY = " Year";
        string unitM = " Month";
        string unitD = " Day";
        /// <summary>
        /// 年份的单位
        /// </summary>
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
        /// <summary>
        /// 月份的单位
        /// </summary>
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
        /// <summary>
        /// 日的单位
        /// </summary>
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
        public override void Initial(FakeStruct mod,UIElement element,UIInitializer initializer)
        {
            base.Initial(mod,element,initializer);
            var mui = Enity;
            mui.userEvent.CutRect = true;
            Year = Enity.Find("Year").composite as ScrollY;
            Year.SetItemUpdate<ItemView, int>((o, e, i) => { o.Item.Text = e.ToString(); });
            Year.Scroll = Scrolling;
            Year.ScrollEnd = YearScrollToEnd;
            Year.ItemDockCenter = true;
            Year.scrollType = ScrollType.Loop;

            Month = Enity.Find("Month").composite as ScrollY;
            Month.SetItemUpdate<ItemView, string>((o, e, i) => { o.Item.Text = e; });
            Month.Scroll = Scrolling;
            Month.ScrollEnd = MonthScrollToEnd;
            Month.ItemDockCenter = true;
            Month.scrollType = ScrollType.Loop;

            Day = Enity.Find("Day").composite as ScrollY;
            Day.SetItemUpdate<ItemView, string>((o, e, i) => { o.Item.Text = e ; });
            Day.Scroll = Scrolling;
            Day.ScrollEnd = DayScrollToEnd;
            Day.ItemDockCenter = true;
            Day.ScrollEnd = DayScrollToEnd;
            Day.scrollType = ScrollType.Loop;

            var ex = UIElementLoader.GetCompositeData(mod);
            if (ex != null)
            {
                StartYear = ex[0];
                EndYear = ex[1];
                if (EndYear < StartYear)
                    EndYear = StartYear;
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
      
                mod.localRotation = Quaternion.Euler(angle, 0, 0);
                var v = MathH.Tan2(90 - angle);
                mod.localPosition =new Vector3(0, v.y * 100,0);
                float r = y / hy;
                if (r < 0)
                    r = -r;
                r = 1 - r;
                if (r < 0)
                    r = 0;
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
        /// <summary>
        /// 年
        /// </summary>
        public int year;
        /// <summary>
        /// 月
        /// </summary>
        public int month = 1;
        /// <summary>
        /// 日
        /// </summary>
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

