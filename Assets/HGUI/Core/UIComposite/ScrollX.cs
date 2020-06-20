using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class ScrollX:ScrollContent
    {
        public static void CenterScroll(ScrollX scroll)
        {
            var eve = scroll.eventCall;
            var d = scroll.eventCall.ScrollDistanceX;//滚动距离
            float tx = scroll.Size.x * 0.5f;//滚动框中心
            float tar = d - scroll.Point + d;//目标地址
            float sx = scroll.ItemSize.x;
            float index = (int)(tar / sx);
            if (tar%sx < -sx * 0.5f)
                index--;
            float offset = (tx - sx * 0.5f) % sx;
            float qt = -index * sx + offset;
            d = qt - scroll.Point;
            scroll.eventCall.ScrollDistanceX = -d;
        }
        public static void CenterScrollByIndex(ScrollX scroll,int index)
        {
            var eve = scroll.eventCall;
            var d = scroll.eventCall.ScrollDistanceX;//滚动距离
            float tx = scroll.Size.x * 0.5f;//滚动框中心
            float tar = d - scroll.Point + d;//目标地址
            float sx = scroll.ItemSize.x;
            if (tar % sx < -sx * 0.5f)
                index--;
            float offset = (tx - sx * 0.5f) % sx;
            float qt = -index * sx + offset;
            d = qt - scroll.Point;
            scroll.eventCall.ScrollDistanceX = -d;
        }
        public UserEvent eventCall;//scrollx自己的按钮
        protected float width;
        int Row = 1;
        float m_point;
        /// <summarx>
        /// 滚动的当前位置，从0开始
        /// </summarx>
        public float Point { get { return m_point; } set { Refresh(0, value - m_point); } }
        /// <summarx>
        /// 0-1之间
        /// </summarx>
        public float Pos
        {
            get {
                if (ActualSize.x <= Size.x)
                    return 0;
                var p = m_point / (ActualSize.x - Size.x);
                if (p < 0) p = 0; 
                else if (p > 1) 
                    p = 1; 
                return p; }
            set
            {
                if (value < 0 | value > 1)
                    return;
                m_point = value * (ActualSize.x - Size.x);
                Order();
            }
        }
        public bool ItemDockCenter;
        public int PreDockindex { get; private set; }
        public Vector2 ContentSize { get; private set; }
        /// <summary>
        /// 动态尺寸,自动对齐
        /// </summary>
        public bool DynamicSize = true;
        Vector2 ctSize;
        float ctScale;
        public override UISlider Slider
        {
            get => m_slider;
            set
            {
                if (m_slider != null)
                    m_slider.OnValueChanged = null;
                m_slider = value;
                if (m_slider != null)
                    m_slider.OnValueChanged = (o) => { Pos = o.Percentage; };
            }
        }
        public override void Initial(FakeStruct mod, UIElement script)
        {
            base.Initial(mod,script);
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.Drag = Draging;
            eventCall.DragEnd = (o, e, s) =>
            {
                Scrolling(o, s);
                if (ItemDockCenter)
                    CenterScroll(this);
                if (ScrollStart != null)
                    ScrollStart(this);
                if (eventCall.VelocityX == 0)
                    OnScrollEnd(o);
            };
            eventCall.Scrolling = Scrolling;
            eventCall.ScrollEndX = OnScrollEnd;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            eventCall.CutRect = true;
            Size = Enity.SizeDelta;
            eventCall.CutRect = true;
            Enity.SizeChanged = (o) => {
                Refresh(m_point,0);
            };
        }
        public Action<ScrollX, Vector2> Scroll;
        public Action<ScrollX> ScrollStart;
        public Action<ScrollX> ScrollEnd;
        public Action<ScrollX> ScrollToTop;
        public Action<ScrollX> ScrollToDown;
        public float DecayRate = 0.998f;
        void Draging(UserEvent back, UserAction action, Vector2 v)
        {
            back.DecayRateX = DecayRate;
            Scrolling(back, v);
        }
        /// <summarx>
        /// 
        /// </summarx>
        /// <param name="back"></param>
        /// <param name="v">移动的实际像素位移</param>
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Main == null)
                return;
            Vector2 u = v;
            v.x /= Enity.transform.localScale.x;
            back.VelocityY = 0;
            v.y = 0;
            float x = 0;
            float y = 0;
            switch (scrollType)
            {
                case ScrollType.None:
                    x = ScrollNone(back, ref v, ref m_point, ref y).x;
                    break;
                case ScrollType.Loop:
                    x = ScrollLoop(back, ref v, ref m_point, ref y).x;
                    break;
                case ScrollType.BounceBack:
                    x = BounceBack(back, ref v, ref m_point, ref y).x;
                    break;
            }
            Order();
            if (x != 0)
            {
                if (Scroll != null)
                    Scroll(this, u);
            }
            else
            {
                if (ScrollEnd != null)
                    ScrollEnd(this);
            }
            if (m_slider != null)
            {
                m_slider.Percentage = Pos;
            }
        }
        void OnScrollEnd(UserEvent back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (m_point < -Tolerance)
                {
                    back.DecayRateX = DecayRate;
                    float d = -m_point;
                    back.ScrollDistanceX = -d * eventCall.Context.transform.localScale.x;
                }
                else
                {
                    float max = ActualSize.x + Tolerance;
                    if (max < Size.x)
                        max = Size.x + Tolerance;
                    if (m_point + Size.x > max)
                    {
                        back.DecayRateX = DecayRate;
                        float d = ActualSize.x - m_point - Size.x ;
                        back.ScrollDistanceX = -d * eventCall.Context.transform.localScale.x;
                    }
                    else
                    {
                        if (ScrollEnd != null)
                            ScrollEnd(this);
                    }
                }
            }
            else if (ScrollEnd != null)
                ScrollEnd(this);
            if (m_slider != null)
                m_slider.Percentage =  Pos;
        }
        public void Calcul()
        {
            float w = Size.y - ItemOffset.y;
            float dw = w / ItemSize.y;
            Row = (int)dw;
            if (Row < 1)
                Row = 1;
            if (DynamicSize)
            {
                float dy = w / Row;
                ctScale = dy / ItemSize.y;
                ctSize.y = dy;
                ctSize.x = ItemSize.x * ctScale;
            }
            else
            {
                ctSize = ItemSize;
                ctScale = 1;
            }
            int c = DataLength;
            int a = c % Row;
            c /= Row;
            if (a > 0)
                c++;
            width = c * ctSize.x;
            if (width < Size.x)
                width = Size.x;
            ActualSize = new Vector2(width, Size.y);
        }
        public override void Refresh(float x = 0, float y = 0)
        {
            m_point = x;
            Size = Enity.SizeDelta;
            ActualSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].target.gameObject.SetActive(false);
                }
                return;
            }
            if (ItemMod == null)
            {
                return;
            }
            Calcul();
            Order(true);
            if (m_slider != null)
            {
                m_slider.Percentage = Pos;
                if (ActualSize.x <= Enity.SizeDelta.x)
                    m_slider.Enity.gameObject.SetActive(false);
                else m_slider.Enity.gameObject.SetActive(true);
            }
        }
        /// <summarx>
        /// 指定下标处的位置重排
        /// </summarx>
        /// <param name="_index"></param>
        public void ShowBxIndex(int _index)
        {
            ActualSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                    Items[i].target.gameObject.SetActive(false);
                return;
            }
            if (ItemMod == null)
            {
                return;
            }
            Calcul();
            float x = _index * ItemSize.x;
            m_point = x;
            Order(true);
        }
        void Order(bool force = false)
        {
            int len = DataLength;
            if (len <= 0)
                return;
            float lx = ctSize.x;
            int sr = (int)(m_point / lx);//起始索引
            int er = (int)((m_point + Size.x) / lx) + 1;
            sr *= Row;
            er *= Row;//结束索引
            int e = er - sr;//总计显示数据
            if (e > len)
                e = len;
            if (scrollType == ScrollType.Loop)
            {
                if (er >= len)
                {
                    er -= len;
                }
            }
            else
            {
                if (sr < 0)
                    sr = 0;
                if (er >= len)
                    er = len;
                e = er - sr;
            }

            PushItems();//将未被回收的数据压入缓冲区
            int index = sr;
            float ox = 0;
            for (int i = 0; i < e; i++)
            {
                UpdateItem(index, ox, force);
                index++;
                if (index >= len)
                {
                    index = 0;
                    ox = ActualSize.x;
                }
            }
            RecycleRemain();
        }
        void UpdateItem(int index, float ox, bool force)
        {
            float lx = ctSize.x;
            int col = index / Row;//列
            float dx = lx * col + ox;//列起点
            dx -= m_point;//滚动框当前起点
            float ss = -0.5f * Size.x + 0.5f * lx;//x起点
            dx += ss;
            float os = Size.y * 0.5f- ItemOffset.y - (index % Row) * ctSize.y - ctSize.y * 0.5f  ;//行起点
            var a = PopItem(index);
            a.target.localPosition = new Vector3(dx, os, 0);
            a.target.localScale = new Vector3(ctScale,ctScale,ctScale);
            Items.Add(a);
            if (a.index < 0 | force)
            {
                var dat = GetData(index);
                a.datacontext = dat;
                a.index = index;
                ItemUpdate(a.obj,dat, index);
            }
        }
        public static ScrollItem GetCenterItem(List<ScrollItem> items)
        {
            if (items.Count < 1)
                return null;
            float min = 100;
            ScrollItem item = items[0];
            for (int i = 1; i < items.Count; i++)
            {
                float x = items[i].target.localPosition.x;
                if (x < 0)
                    x = -x;
                if (x < min)
                {
                    min = x;
                    item = items[i];
                }
            }
            return item;
        }
    }
}