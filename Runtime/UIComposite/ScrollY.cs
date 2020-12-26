using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 纵向滚动框
    /// </summary>
    public class ScrollY : ScrollContent
    {
        /// <summary>
        /// 主体事件
        /// </summary>
        public UserEvent eventCall;
        /// <summary>
        /// 内容总宽度
        /// </summary>
        protected float height;
        int Column = 1;
        /// <summary>
        /// 滚动的当前位置，从0开始
        /// </summary>
        public float Point { get { return _pos.y; } set { Refresh(0,value - _pos.y); } }
        /// <summary>
        /// 0-1之间
        /// </summary>
        public float Pos
        {
            get {
                if (base._contentSize.y <= Size.y)
                    return 0;
                var p = _pos.y / (base._contentSize.y - Size.y);
                if (p < 0)
                    p = 0;
                else if (p > 1)
                    p = 1;
                return p; 
            }
            set
            {
                if (value < 0 | value > 1)
                    return;
                _pos.y = value * (_contentSize.y - Size.y);
                Order();
            }
        }
        /// <summary>
        /// 项目每次滚动居中
        /// </summary>
        public bool ItemDockCenter;
        ///// <summary>
        ///// 内容总尺寸
        ///// </summary>
        //public Vector2 ContentSize { get; private set; }
        /// <summary>
        /// 动态尺寸,用以适应宽度
        /// </summary>
        public bool DynamicSize = true;
        float ctScale;
        /// <summary>
        /// 滑块条,可以为空
        /// </summary>
        public override UISlider Slider { 
            get => m_slider; 
            set {
                if (m_slider != null)
                    m_slider.OnValueChanged = null;
                m_slider = value;
                if (m_slider != null)
                    m_slider.OnValueChanged = (o) => { Pos = 1 - o.Percentage; };
            } }
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
            eventCall.MouseWheel = (o, e) => { Scrolling(o, new Vector2(0, e.MouseWheelDelta * 100)); UpdateVelocity = true; };
            eventCall.ForceEvent = true;
            eventCall.AutoColor = false;
            eventCall.CutRect = true;
            Size = Enity.SizeDelta;
            Enity.SizeChanged = (o) => {
                if (modData != null)
                    ItemSize = UIElement.GetSize(Enity, modData);
                Refresh(0,_pos.y);
            };
        }
        /// <summary>
        /// 滚动事件
        /// </summary>
        public Action<ScrollY, Vector2> Scroll;
        /// <summary>
        /// 开始滚动事件
        /// </summary>
        public Action<ScrollY> ScrollStart;
        /// <summary>
        /// 结束滚动事件
        /// </summary>
        public Action<ScrollY> ScrollEnd;
        /// <summary>
        /// 光标拖拽完毕
        /// </summary>
        public Action<UserEvent, UserAction, Vector2> DragEnd;
        void Draging(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
        }
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            if(scrollType==ScrollType.Loop)
            {
                if(ItemDockCenter)
                {
                    var y = back.VelocityY;
                    var d = MathH.PowDistance(DecayRateY, y, 100000);
                    float t = _pos.y + d;
                    float o = (Size.y - ItemSize.y) * 0.5f;
                    float r = o % ItemSize.y;
                    float i = (int)(t / ItemSize.y) + 1;
                    t = i * ItemSize.y - r;
                    d = t - _pos.y;
                    startVelocity.y = mVelocity.y = MathH.DistanceToVelocity(DecayRateY, d);
                }
            }else
            {
                startVelocity.y = mVelocity.y = back.VelocityY;
            }
            UpdateVelocity = true;
            if (ScrollStart != null)
                ScrollStart(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="back"></param>
        /// <param name="v">移动的实际像素位移</param>
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Main == null)
                return;
            Vector2 u = v;
            v.y /= Enity.localScale.y;
            v.x = 0;
            switch (scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(v);
                    _pos.y += v.y;
                    break;
                case ScrollType.Loop:
                    _pos.y += v.y;
                    if (_pos.y < 0)
                        _pos.y += _contentSize.y;
                    else _pos.y %= _contentSize.y;
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(v);
                    _pos.y += v.y;
                    break;
            }
            Order();
            if (m_slider != null)
            {
                m_slider.Percentage = 1 - Pos;
            }
            if (Scroll != null)
                Scroll(this, u);
        }
        /// <summary>
        /// 内容尺寸计算
        /// </summary>
        public void Calcul()
        {
            float w = Enity.m_sizeDelta.x;
            float dw = w / ItemSize.x;
            Column = (int)dw;
            if (Column < 1)
                Column = 1;
            if (DynamicSize)
            {
                float dx = w / Column;
                ctScale = dx / ItemSize.x;
                ItemActualSize.x = dx;
                ItemActualSize.y = ItemSize.y * ctScale;
            }
            else
            {
                ItemActualSize = ItemSize;
                ctScale = 1;
            }
            int c = DataLength;
            int a = c % Column;
            c /= Column;
            if (a > 0)
                c++;
            height = c * ItemActualSize.y;
            if (height < Size.y)
                height = Size.y;
            base._contentSize = new Vector2(Size.x, height);
            GetItemOffset();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="x">无效</param>
        /// <param name="y">纵向位置</param>
        public override void Refresh(float x = 0, float y = 0)
        {
            _pos.y = y;
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
                m_slider.Percentage = 1 - Pos;
                if (base._contentSize.y <= Enity.SizeDelta.y)
                    m_slider.Enity.activeSelf = false;
                else m_slider.Enity.activeSelf = true;
            }
        }
        /// <summary>
        /// 指定下标处的位置重排
        /// </summary>
        /// <param name="_index"></param>
        public void ShowByIndex(int _index)
        {
            base._contentSize = Vector2.zero;
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
            float y = _index * ItemActualSize.y;
            _pos.y = y;
            Order(true);
        }
        void Order(bool force=false)
        {
            int len = DataLength;
            if (len <= 0)
                return;
            float ly = ItemActualSize.y;
            int sr = (int)(_pos.y /ly);//起始索引
            int er = (int)((_pos.y + Size.y) / ly)+1;
            sr *= Column;
            er *= Column;//结束索引
            int e = er - sr;//总计显示数据
            if (e > len)
                e = len;
            if(scrollType==ScrollType.Loop)
            {
                if (er >= len)
                {
                    er -= len;
                    RecycleInside(er, sr);
                }
                else
                {
                    RecycleOutside(sr, er);
                }
            }
            else
            {
                if (sr < 0)
                    sr = 0;
                if (er >= len)
                    er = len;
                e = er - sr;
                RecycleOutside(sr, er);
            }
            PushItems();//将未被回收的数据压入缓冲区
            int index = sr;
            float oy = 0;
            for (int i = 0; i < e; i++)
            {
                UpdateItem(index, oy, force);
                index++;
                if (index >= len)
                {
                    index = 0;
                    oy = _contentSize.y;
                }
            }
            RecycleRemain();
        }
        void UpdateItem(int index,float oy,bool force)
        {
            int row = index / Column;
            int col = index % Column;
            float sx = col * ItemActualSize.x;
            float sy = row* ItemActualSize.y;
            sy -=_pos.y - oy;
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
                ItemUpdate(a.obj, dat, index);
            }
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
                float y = items[i].target.localPosition.y;
                if (y < 0)
                    y = -y;
                if (y < min)
                {
                    min = y;
                    item = items[i];
                }
            }
            return item;
        }
        public override void Update(float time)
        {
            mVelocity.x = 0;
            base.Update(time);
        }
        public override void DurScroll(Vector2 v)
        {
            _pos.y += v.y;
            if (scrollType == ScrollType.Loop)
            {
                if (_pos.y < 0)
                    _pos.y += _contentSize.y;
                else _pos.y %= _contentSize.y;
            }
            Order();
            if (m_slider != null)
            {
                m_slider.Percentage = 1 - Pos;
            }
            if (Scroll != null)
                Scroll(this, v);
        }
    }
}
