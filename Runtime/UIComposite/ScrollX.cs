using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 横向滚动框
    /// </summary>
    public class ScrollX:ScrollContent
    {
        /// <summary>
        /// 主体事件
        /// </summary>
        public UserEvent eventCall;
        /// <summary>
        /// 内容总计宽度
        /// </summary>
        protected float width;
        int Row = 1;
        /// <summarx>
        /// 滚动的当前位置，从0开始
        /// </summarx>
        public float Point { get { return _pos.x; } set { Refresh(0, value - _pos.x); } }
        /// <summarx>
        /// 0-1之间
        /// </summarx>
        public float Pos
        {
            get {
                if (base._contentSize.x <= Size.x)
                    return 0;
                var p = _pos.x / (base._contentSize.x - Size.x);
                if (p < 0) p = 0; 
                else if (p > 1) 
                    p = 1; 
                return p; }
            set
            {
                if (value < 0 | value > 1)
                    return;
                _pos.x = value * (base._contentSize.x - Size.x);
                Order();
            }
        }
        /// <summary>
        /// 项目每次滚动居中
        /// </summary>
        public bool ItemDockCenter;
        /// <summary>
        /// 动态尺寸,自动对齐
        /// </summary>
        public bool DynamicSize = true;
        float ctScale;
        /// <summary>
        /// 滑块条,可以为空
        /// </summary>
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
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">数据模型</param>
        /// <param name="script">主体元素</param>
        public override void Initial(FakeStruct mod, UIElement script,UIInitializer initializer)
        {
            base.Initial(mod,script,initializer);
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.PointerDown = (o, e) => { UpdateVelocity = false; };
            eventCall.Drag = Draging;
            eventCall.DragEnd = OnDragEnd;
            //eventCall.Scrolling = Scrolling;
            //eventCall.ScrollEndX = OnScrollEnd;
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            eventCall.CutRect = true;
            Size = Enity.SizeDelta;
            eventCall.CutRect = true;
            Enity.SizeChanged = (o) => {
                if (modData != null)
                    ItemSize = UIElement.GetSize(Enity, modData);
                Refresh(_pos.x,0);
            };
        }
        /// <summary>
        /// 滚动事件
        /// </summary>
        public Action<ScrollX, Vector2> Scroll;
        /// <summary>
        /// 开始滚动事件
        /// </summary>
        public Action<ScrollX> ScrollStart;
        /// <summary>
        /// 滚动结束事件
        /// </summary>
        public Action<ScrollX> ScrollEnd;
        void Draging(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
        }
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            if (scrollType == ScrollType.Loop)
            {
                if (ItemDockCenter)
                {
                    var x = -back.VelocityX;
                    var d = MathH.PowDistance(DecayRateY, x, 100000);
                    float t = _pos.y + d;
                    float o = (Size.x - ItemSize.x) * 0.5f;
                    float r = o % ItemSize.x;
                    float i = (int)(t / ItemSize.x) + 1;
                    t = i * ItemSize.x - r;
                    d = t - _pos.x;
                    startVelocity.x = mVelocity.x = MathH.DistanceToVelocity(DecayRateX, d);
                }
            }
            else
                startVelocity.x = mVelocity.x = - back.VelocityX;
            UpdateVelocity = true;
            if (ScrollStart != null)
                ScrollStart(this);
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
            v.x /= Enity.localScale.x;
            v.x = -v.x;
            v.y = 0;
            switch (scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(v);
                    _pos.x += v.x;
                    break;
                case ScrollType.Loop:
                    _pos.x += v.x;
                    if (_pos.x < 0)
                        _pos.x += _contentSize.x;
                    else _pos.x %= _contentSize.x;
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(v);
                    _pos.x += v.x;
                    break;
            }
            Order();
            if (m_slider != null)
            {
                m_slider.Percentage = Pos;
            }
            if (Scroll != null)
                Scroll(this, u);
        }
        /// <summary>
        /// 内容尺寸计算
        /// </summary>
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
                ItemActualSize.y = dy;
                ItemActualSize.x = ItemSize.x * ctScale;
            }
            else
            {
                ItemActualSize = ItemSize;
                ctScale = 1;
            }
            int c = DataLength;
            int a = c % Row;
            c /= Row;
            if (a > 0)
                c++;
            width = c * ItemActualSize.x;
            if (width < Size.x)
                width = Size.x;
            _contentSize = new Vector2(width, Size.y);
            GetItemOffset();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="x">横向位置</param>
        /// <param name="y">无效</param>
        public override void Refresh(float x = 0, float y = 0)
        {
            _pos.x = x;
            Size = Enity.SizeDelta;
            base._contentSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].target.activeSelf = false;
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
                if (_contentSize.x <= Enity.SizeDelta.x)
                    m_slider.Enity.activeSelf = false;
                else m_slider.Enity.activeSelf = true;
            }
        }
        /// <summarx>
        /// 指定下标处的位置重排
        /// </summarx>
        /// <param name="_index"></param>
        public void ShowBxIndex(int _index)
        {
            _contentSize = Vector2.zero;
            if (DataLength == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                    Items[i].target.activeSelf = false;
                return;
            }
            if (ItemMod == null)
            {
                return;
            }
            Calcul();
            float x = _index * ItemSize.x;
            _pos.x = x;
            Order(true);
        }
        void Order(bool force = false)
        {
            int len = DataLength;
            if (len <= 0)
                return;
            float lx = ItemActualSize.x;
            int sr = (int)(_pos.x / lx);//起始索引
            int er = (int)((_pos.x + Size.x) / lx) + 1;
            sr *= Row;
            er *= Row;//结束索引
            int e = er - sr;//总计显示数据
            if (e > len)
                e = len;
            if (scrollType == ScrollType.Loop)
            {
                //if (er >= len)
                //{
                //    er -= len;
                //}
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
                    ox = _contentSize.x;
                }
            }
            RecycleRemain();
        }
        void UpdateItem(int index, float ox, bool force)
        {
            int col = index / Row;//行
            int row = index % Row;
            float sx = col * ItemActualSize.x;
            float sy = row * ItemActualSize.y;
            sx -= _pos.x - ox;
            var a = PopItem(index);
            var p = ItemOffset;
            p.x += sx;
            p.y -= sy;
            a.target.localPosition = p;
            Items.Add(a);
            if (a.index < 0 | force)
            {
                var dat = GetData(index);
                a.datacontext = dat;
                a.index = index;
                ItemUpdate(a.obj,dat, index);
            }
        }
        public override void Update(float time)
        {
            mVelocity.y = 0;
            base.Update(time);
        }
        public override void DurScroll(Vector2 v)
        {
            _pos.x += v.x;
            if(scrollType==ScrollType.Loop)
            {
                if (_pos.x < 0)
                    _pos.x += _contentSize.x;
                else _pos.x %= _contentSize.x;
            }
            Order();
            if (m_slider != null)
            {
                m_slider.Percentage = Pos;
                if (_contentSize.x <= Enity.SizeDelta.x)
                    m_slider.Enity.activeSelf = false;
                else m_slider.Enity.activeSelf = true;
            }
            if (Scroll != null)
                Scroll(this, v);
        }
        /// <summary>
        /// 获取最接近中心的项目
        /// </summary>
        /// <param name="items">项目列表</param>
        /// <returns></returns>
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