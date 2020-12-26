using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIComposite
{
    public enum ScrollType
    {
        /// <summary>
        /// 撞到边界立即停止
        /// </summary>
        None,
        /// <summary>
        /// 循环滚动
        /// </summary>
        Loop,
        /// <summary>
        /// 撞到边界回弹
        /// </summary>
        BounceBack
    }
    /// <summary>
    /// 滚动内容附件信息,使用ScrollHelper收集
    /// </summary>
    public unsafe struct ScrollInfo
    {
        /// <summary>
        /// 滚动类型
        /// </summary>
        public ScrollType scrollType;
        /// <summary>
        /// 最小尺寸
        /// </summary>
        public Vector2 minBox;
        /// <summary>
        /// 滑块条的实例ID
        /// </summary>
        public Int32 Slider;
        public static int Size = sizeof(ScrollInfo);
        public static int ElementSize = Size / 4;
    }
    /// <summary>
    /// 构造器
    /// </summary>
    public class Constructor
    {
        /// <summary>
        /// UI实体创建
        /// </summary>
        /// <returns></returns>
        public virtual object Create() { return null; }
        /// <summary>
        /// 项目更新函数
        /// </summary>
        /// <param name="obj">UI实体对象</param>
        /// <param name="dat">数据实体对象</param>
        /// <param name="index">数据索引</param>
        public virtual void Call(object obj, object dat, int index) {
            if (Update != null)
                Update(obj,dat,index);
        }
        /// <summary>
        /// 自动创建
        /// </summary>
        public bool create;
        /// <summary>
        /// 项目更新委托
        /// </summary>
        public Action<object, object, int> Update;
        /// <summary>
        /// 反射组件,用于热更新
        /// </summary>
        public Func<UIElement, object> reflect;
        /// <summary>
        /// UI初始化器
        /// </summary>
        public UIInitializer initializer;
    }
    /// <summary>
    /// 构造中间件
    /// </summary>
    /// <typeparam name="T">UI模型</typeparam>
    /// <typeparam name="U">数据模型</typeparam>
    public class Middleware<T, U> : Constructor where T : class, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Middleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        /// <summary>
        /// 创建UI实体
        /// </summary>
        /// <returns></returns>
        public override object Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        /// <summary>
        /// 项目更新委托
        /// </summary>
        public Action<T, U, int> Invoke;
        U u;
        /// <summary>
        /// 项目更新函数
        /// </summary>
        /// <param name="obj">UI实体</param>
        /// <param name="dat">数据实体</param>
        /// <param name="index">数据索引</param>
        public override void Call(object obj, object dat, int index)
        {
            if (Invoke != null)
            {
                try
                {
                    u = (U)dat;
                }
                catch
                {
                }
                Invoke(obj as T, u, index);
            }
        }
    }
    /// <summary>
    /// 热更新构造中间件
    /// </summary>
    public class HotMiddleware : Constructor
    {
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object Context;
        /// <summary>
        /// 实体创建委托
        /// </summary>
        public Func<object> creator;
        /// <summary>
        /// 项目更新委托
        /// </summary>
        public Action<object, object, int> caller;
        /// <summary>
        /// UI实体创建
        /// </summary>
        /// <returns></returns>
        public override object Create()
        {
            if (creator == null)
                return null;
            return creator();
        }
        /// <summary>
        /// 项目更新函数
        /// </summary>
        /// <param name="obj">UI实体</param>
        /// <param name="dat">数据实体</param>
        /// <param name="index">数据索引</param>
        public override void Call(object obj, object dat, int index)
        {
            if (caller != null)
                caller(obj, dat, index);
        }
    }
    /// <summary>
    /// 滚动内容管理器
    /// </summary>
    public class ScrollContent: Composite
    {
        /// <summary>
        /// 滚动公差值
        /// </summary>
        public static float Tolerance = 0.25f;
        /// <summary>
        /// 滚动类型
        /// </summary>
        public ScrollType scrollType = ScrollType.BounceBack;
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        /// <summary>
        /// 滚动框的尺寸
        /// </summary>
        public Vector2 Size;
        protected Vector2 _contentSize;
        /// <summary>
        /// 内容的实际尺寸
        /// </summary>
        public Vector2 ContentSize { get => _contentSize; }
        protected Vector2 _pos;
        public virtual Vector2 Position { get => _pos; set => _pos = value; }
        /// <summary>
        /// 项目其实偏移位置
        /// </summary>
        protected Vector2 ItemOffset = Vector2.zero;
        /// <summary>
        /// 项目尺寸
        /// </summary>
        public Vector2 ItemSize = new Vector2(1,1);
        /// <summary>
        /// 尺寸自适应后的尺寸
        /// </summary>
        public Vector2 ItemActualSize = new Vector2(1, 1);
        public int SelectIndex = -1;
        protected FakeStruct modData;
        /// <summary>
        /// 项目模板
        /// </summary>
        public FakeStruct ItemMod
        {
            set
            {
                modData = value;
                if (modData != null)
                {
                    ItemSize = UIElement.GetSize(Enity, modData);
                }
                Clear();
            }
            get { return modData; }
        }
        IList dataList;
        Array array;
        FakeArray fakeStruct;
        /// <summary>
        /// 传入类型为IList
        /// </summary>
        public object BindingData
        {
            get
            {
                if (dataList != null)
                    return dataList;
                if (array != null)
                    return array;
                return fakeStruct;
            }
            set
            {
                if (value is IList)
                {
                    dataList = value as IList;
                    array = null;
                    fakeStruct = null;
                }
                else if (value is Array)
                {
                    dataList = null;
                    array = value as Array;
                    fakeStruct = null;
                }
                else if (value is FakeArray)
                {
                    dataList = null;
                    array = null;
                    fakeStruct = value as FakeArray;
                }
                else
                {
                    dataList = null;
                    array = null;
                    fakeStruct = null;
                }
            }
        }
        int m_len;
        /// <summary>
        /// 数据长度
        /// </summary>
        public int DataLength
        {
            set { m_len = value; }
            get
            {
                if (dataList != null)
                    return dataList.Count;
                if (array != null)
                    return array.Length;
                if (fakeStruct != null)
                    return fakeStruct.Length;
                return m_len;
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <returns></returns>
        public object GetData(int index)
        {
            if (index < 0)
                return null;
            if (dataList != null)
            {
                if (index >= dataList.Count)
                    return null;
                return dataList[index]; 
            }
            if (array != null)
            {
                if (index >= array.Length)
                    return null;
                return array.GetValue(index); 
            }
            return null;
        }

        /// <summary>
        /// 最小尺寸
        /// </summary>
        public Vector2 MinBox;
        /// <summary>
        /// 最大缓存
        /// </summary>
        protected int max_count;
        /// <summary>
        /// UI项目列表
        /// </summary>
        public List<ScrollItem> Items=new List<ScrollItem>();
        /// <summary>
        /// UI项目缓存列表
        /// </summary>
        protected List<ScrollItem> Buffer=new List<ScrollItem>();
        /// <summary>
        ///  UI项目回收列表
        /// </summary>
        protected List<ScrollItem> Recycler = new List<ScrollItem>();
        /// <summary>
        /// 当某个ui超出Mask边界，被回收时调用
        /// </summary>
        public Action<ScrollItem> ItemRecycle;
        /// <summary>
        /// 主体对象
        /// </summary>
        public UIElement Main;
        /// <summary>
        /// 滑块条
        /// </summary>
        protected UISlider m_slider;
        /// <summary>
        /// 滑块条
        /// </summary>
        public virtual UISlider Slider { get; set; }
        /// <summary>
        /// 设置项目模型
        /// </summary>
        /// <param name="name">模型名称</param>
        public void SetItemMod(string name)
        {
            if (BufferData == null)
                return;
            var mod = HGUIManager.FindChild(BufferData,name);
            if(mod!=null)
            {
                unsafe
                {
                    ItemSize = UIElementLoader.GetSize(mod);
                    ItemActualSize = ItemSize;
                    var ex = UIElementLoader.GetCompositeData(mod);
                    if (ex != null)
                    {
                        ScrollInfo* tp = (ScrollInfo*)ex.ip;
                        scrollType = tp->scrollType;
                        MinBox = tp->minBox;
                    }
                }
            }
            ItemMod = mod;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">模型数据</param>
        /// <param name="script">主体元素</param>
        public override void Initial(FakeStruct mod, UIElement script, UIInitializer initializer)
        {
            base.Initial(mod,script,initializer);
            Main = script;
            SetItemMod("Item");
            HGUIManager.RecycleChild(script);
            var ex = UIElementLoader.GetCompositeData(mod);
            if (ex != null)
            {
                unsafe
                {
                    ScrollInfo* si = (ScrollInfo*)ex.ip;
                    scrollType = si->scrollType;
                    MinBox = si->minBox;
                }
                if (initializer != null)
                {
                    initializer.AddContextAction((trans) => {
                        if (trans != null)
                            Slider = trans.composite as UISlider;
                    }, ex[3]);
                }
            }
        }
        /// <summary>
        /// 刷新显示UI
        /// </summary>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public virtual void Refresh(float x = 0, float y = 0)
        {
        }
        protected void Initialtems()
        {
            int x = (int)(Size.x / ItemSize.x) + 2;
            int y = (int)(Size.y / ItemSize.y) + 2;
            max_count = x * y;
        }
        /// <summary>
        /// 创建项目实例
        /// </summary>
        /// <returns></returns>
        protected ScrollItem CreateItem()
        {
            if (Recycler.Count> 0)
            {
                var it = Recycler[0];
                it.target.activeSelf = true;
                it.index = -1;
                Recycler.RemoveAt(0);
                return it;
            }
            ScrollItem a = new ScrollItem();
            if (creator != null)
            {
                if (creator.create)
                {
                    a.obj = creator.Create();
                    a.target = HGUIManager.Clone(ItemMod, creator.initializer);
                }
                else
                {
                    var go = HGUIManager.Clone(ItemMod);
                    a.obj = go;
                    a.target = go;
                }
            }
            else
            {
                var go = HGUIManager.Clone(ItemMod);
                a.obj = go;
                a.target = go;
            }
            a.target.SetParent(Main);
            a.target.SetAsFirstSibling();
            a.target.localRotation = Quaternion.identity;
            a.target.localScale = Vector3.one;
            UIElement.Resize(a.target);
            ItemSize = a.target.m_sizeDelta;
            return a;
        }
        Constructor creator;
        /// <summary>
        /// 设置项目跟新函数
        /// </summary>
        /// <typeparam name="T">UI模板</typeparam>
        /// <typeparam name="U">数据模板</typeparam>
        /// <param name="action">项目更新回调</param>
        /// <param name="reflect">使用反射</param>
        public void SetItemUpdate<T,U>(Action<T,U, int> action,bool reflect = true)where T:class,new()
        {
            Clear();
            var m = new Middleware<T,U>();
            m.Invoke = action;
            creator = m;
            m.create = reflect;
        }
        /// <summary>
        /// 热更新无法跨域,使用此函数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="reflect"></param>
        public void SetItemUpdate(HotMiddleware constructor)
        {
            Clear();
            creator = constructor;
            creator.create = true;
        }
        /// <summary>
        /// UI排序
        /// </summary>
        /// <param name="os"></param>
        /// <param name="force"></param>
        public virtual void Order(float os, bool force = false)
        {
        }
        /// <summary>
        /// 清除UI资源
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Items.Count; i++)
                HGUIManager.RecycleUI(Items[i].target);
            for (int i = 0; i < Recycler.Count; i++)
                HGUIManager.RecycleUI(Recycler[i].target);
            Items.Clear();
            Recycler.Clear();
        }
        /// <summary>
        /// 压入缓存
        /// </summary>
        protected void PushItems()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].target.activeSelf = false;
            Buffer.AddRange(Items);
            Items.Clear();
        }
        /// <summary>
        /// 从缓存中提取
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        protected ScrollItem PopItem(int index)
        {
            for(int i=0;i<Buffer.Count;i++)
            {
                var t = Buffer[i];
                if(t.index==index)
                {
                    Buffer.RemoveAt(i);
                    t.target.activeSelf = true;
                    return t;
                }
            }
            var it = CreateItem();
            return it;
        }
        /// <summary>
        /// 回收边界内的项目
        /// </summary>
        /// <param name="down">上边界</param>
        /// <param name="top">下边界</param>
        protected void RecycleInside(int down, int top)
        {
            int c = Items.Count - 1;
            for (; c >= 0; c--)
            {
                var it = Items[c];
                int index = Items[c].index;
                if (index >= down & index <= top)
                {
                    RecycleItem(it);
                    Items.RemoveAt(c);
                }
            }
        }
        /// <summary>
        /// 回收超出边界的项目
        /// </summary>
        /// <param name="down">上边界</param>
        /// <param name="top">下边界</param>
        protected void RecycleOutside(int down, int top)
        {
            int c = Items.Count - 1;
            for (; c >= 0; c--)
            {
                var it = Items[c];
                int index = Items[c].index;
                if (index < down | index > top)
                {
                    RecycleItem(it);
                    Items.RemoveAt(c);
                }
            }
        }
        /// <summary>
        /// 回收缓存中剩余的项目
        /// </summary>
        protected void RecycleRemain()
        {
            for (int i = 0; i < Buffer.Count; i++)
                Buffer[i].target.activeSelf = false;
            Recycler.AddRange(Buffer);
            Buffer.Clear();
        }
        /// <summary>
        /// 回收一个项目
        /// </summary>
        /// <param name="it"></param>
        protected void RecycleItem(ScrollItem it)
        {
            it.target.activeSelf = false;
            Recycler.Add(it);
            if (ItemRecycle != null)
                ItemRecycle(it);
        }
        protected void GetItemOffset()
        {
            float ax = Enity.m_sizeDelta.x;
            float ay = Enity.m_sizeDelta.y;
            float apx = Enity.Pivot.x;
            float apy = Enity.Pivot.y;
            float alx = ax * -apx;
            float ady = ay * -apy;

            float x = ItemActualSize.x;
            float y = ItemActualSize.y;
            var pi= UIElementLoader.GetPivot(ItemMod);
            float px = pi.x;
            float py = pi.y;
            float lx = x * -px;
            float dy = y * -py;
            x = alx - lx;
            y = (ay + ady) - (y + dy);
            ItemOffset.x = x;
            ItemOffset.y = y;
        }
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="obj">UI实例</param>
        /// <param name="dat">数据实例</param>
        /// <param name="index">数据索引</param>
        protected void ItemUpdate(object obj,object dat, int index)
        {
            if (creator != null)
            {
                creator.Call(obj, dat, index);
            }
        }

        #region
        /// <summary>
        /// 初始速率
        /// </summary>
        protected Vector2 startVelocity;
        /// <summary>
        /// 当前速率
        /// </summary>
        protected Vector2 mVelocity;
        public float DecayRateX = 0.997f;
        public float DecayRateY = 0.997f;
        protected bool UpdateVelocity = true;
        public override void Update(float time)
        {
            if (!UpdateVelocity)
                return;
            float x = 0;
            float y = 0;
            int count = UserAction.TimeSlice;
            if (mVelocity.x != 0)
            {
                float dr = DecayRateX;
                if (scrollType == ScrollType.BounceBack)
                {
                    if (mVelocity.x < 0)
                    {
                        if (_pos.x < 0)
                        {
                            dr *= 0.9f;
                        }
                    }
                    else
                    {
                        if (_pos.x + Size.x > _contentSize.x)
                        {
                            dr *= 0.9f;
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    mVelocity.x *= dr;
                    x += mVelocity.x;
                }
                if (mVelocity.x < 0.01f & mVelocity.x > -0.01f)
                {
                    mVelocity.x = 0;
                }
            }
            if (mVelocity.y != 0)
            {
                float dr = DecayRateY;
                if (scrollType == ScrollType.BounceBack)
                {
                    if (mVelocity.y < 0)
                    {
                        if (_pos.y < 0)
                        {
                            dr *= 0.9f;
                        }
                    }
                    else
                    {
                        if (_pos.y + Size.y > _contentSize.y)
                        {
                            dr *= 0.9f;
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    mVelocity.y *= dr;
                    y += mVelocity.y;
                }
                if (mVelocity.y < 0.01f & mVelocity.y > -0.01f)
                {
                    mVelocity.y = 0;
                }
            }
            if (x != 0 | y != 0)
                Scrolling(new Vector2(x, y));
            if (scrollType == ScrollType.BounceBack)
            {
                if (mVelocity.x == 0)
                {
                    if (_pos.x < -Tolerance)
                    {
                        mVelocity.x = MathH.DistanceToVelocity(DecayRateX, -_pos.x);
                    }
                    else if (_pos.x + Size.x > _contentSize.x + Tolerance)
                    {
                        mVelocity.x = MathH.DistanceToVelocity(DecayRateX, _contentSize.x - _pos.x - Size.x);
                    }
                }
                if (mVelocity.y == 0)
                {
                    if (_pos.y < -Tolerance)
                    {
                        mVelocity.y = MathH.DistanceToVelocity(DecayRateY, -_pos.y);
                    }
                    else if (_pos.y + Size.y > _contentSize.y + Tolerance)
                    {
                        mVelocity.y = MathH.DistanceToVelocity(DecayRateY, _contentSize.y - _pos.y - Size.y);
                    }
                }
            }
            if (mVelocity.x == 0 & mVelocity.y == 0)
                UpdateVelocity = false;

        }
        void Scrolling(Vector2 v)
        {
            switch(scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(v);
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(v);
                    break;
            }
            DurScroll(v);
        }
        protected Vector2 ScrollNone(Vector2 v)
        {
            if (_contentSize.x <= Enity.m_sizeDelta.x)
            {
                _pos.x = 0;
                v.x = 0;
                mVelocity.x = 0;
                if (_contentSize.y <= Enity.m_sizeDelta.y)
                {
                    _pos.y = 0;
                    v.y = 0;
                    mVelocity.y = 0;
                    return v;
                }
            }
            else
             if (_contentSize.y <= Enity.m_sizeDelta.y)
            {
                _pos.y = 0;
                v.y = 0;
                mVelocity.y = 0;
            }

            if (v.x <= 0)
            {
                if (_pos.x + v.x < 0)
                {
                    v.x = 0 - _pos.x;
                    mVelocity.x = 0;
                }
            }
            else
            {
                if (_pos.x + v.x + Enity.m_sizeDelta.x > _contentSize.x)
                {
                    v.x = _contentSize.x - _pos.x - Enity.m_sizeDelta.x;
                    mVelocity.y = 0;
                }
            }
            if (v.y <= 0)
            {
                if (_pos.y + v.y < 0)
                {
                    v.y = 0 - _pos.y;
                    mVelocity.y = 0;
                }
            }
            else
            {
                if (_pos.y + v.y + Enity.m_sizeDelta.y > _contentSize.y)
                {
                    v.y = _contentSize.y - _pos.y - Enity.m_sizeDelta.y;
                    mVelocity.y = 0;
                }
            }
            return v;
        }
        protected Vector2 BounceBack(Vector2 v)
        {
            if (v.x < 0)
            {
                if (_pos.x + v.x < 0)
                {
                    if (_pos.x < 0)
                    {
                        float hx = Enity.m_sizeDelta.x * 0.5f;
                        float r = _pos.x / hx;
                        r = -r;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.x *= r;
                    }
                }
            }
            else
            {
                if (_pos.x + v.x + Enity.m_sizeDelta.x > _contentSize.x)
                {
                    float rx = _pos.x + Enity.m_sizeDelta.x - _contentSize.x;
                    if (rx > 0)
                    {
                        float hx = Enity.m_sizeDelta.x * 0.5f;
                        float r = rx / hx;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.x *= r;
                    }
                }
            }
            if (v.y < 0)
            {
                if (_pos.y + v.y < 0)
                {
                    if (_pos.y < 0)
                    {
                        float hy = Enity.m_sizeDelta.y * 0.5f;
                        float r = _pos.y / hy;
                        r = -r;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.y *= r;
                    }
                }
            }
            else
            {
                if (_pos.y + v.y + Enity.m_sizeDelta.y > _contentSize.y)
                {
                    float ty = _pos.y + Enity.m_sizeDelta.y - _contentSize.y;
                    if (ty > 0)
                    {
                        float hy = Enity.m_sizeDelta.x * 0.5f;
                        float r = ty / hy;
                        r = 1 - r;
                        if (r < 0)
                            r = 0;
                        v.y *= r;
                    }
                }
            }
            return v;
        }
        public virtual void DurScroll(Vector2 v)
        {
        }
        #endregion
    }
}
