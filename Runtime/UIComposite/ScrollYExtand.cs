using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 带有标题的,可以展开收缩的
    /// </summary>
    public class ScrollYExtand : Composite
    {
        /// <summary>
        /// 数据模板
        /// </summary>
        public class DataTemplate
        {
            /// <summary>
            /// 标题对象实例
            /// </summary>
            public object Title;
            /// <summary>
            /// 尾部对象实例
            /// </summary>
            public object Tail;
            /// <summary>
            /// 绑定数据
            /// </summary>
            public IList Data;
            /// <summary>
            /// 隐藏数据
            /// </summary>
            public bool Hide;
            /// <summary>
            /// 隐藏尾部
            /// </summary>
            public bool HideTail;
            /// <summary>
            /// 数据高度
            /// </summary>
            public float Height { internal set; get; }
            /// <summary>
            /// 最后计算高度,用于比对是否需要重新计算
            /// </summary>
            public float ShowHeight = 0;
            /// <summary>
            /// 展开动画时间
            /// </summary>
            public float aniTime;
        }
        /// <summary>
        /// 事件主体
        /// </summary>
        UserEvent eventCall;
        protected float height;
        int wm = 1;
        public float Point;
        public Vector2 ActualSize;
        /// <summary>
        /// 滚动事件
        /// </summary>
        public Action<ScrollYExtand, Vector2> Scroll;
        UIElement BodyParent;
        UIElement TitleParent;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">数据模型</param>
        /// <param name="element">主体元素</param>
        public override void Initial(FakeStruct fake,UIElement element,UIInitializer initializer)
        {
            base.Initial(fake,element,initializer);
            element.SizeChanged = (o) => { Refresh(); };
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.PointerDown = (o, e) => { UpdateVelocity = false; };
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = OnDragEnd;
            eventCall.MouseWheel = (o, e) => {
                Point+=BounceBack((e.MouseWheelDelta * 100));
                Order(); 
                UpdateVelocity = true; 
            };
            eventCall.ForceEvent = true;
            Size = Enity.SizeDelta;
            eventCall.CutRect = true;
            BodyParent = element.Find("Bodys");
            TitleParent = element.Find("Titles");
            HGUIManager.RecycleChild(Enity,new string[]{ "Bodys", "Titles" });
           
            TitleMod =  HGUIManager.FindChild(fake,"Title");
            ItemMod = HGUIManager.FindChild(fake, "Item");
            TailMod = HGUIManager.FindChild(fake, "Tail");
            Body = HGUIManager.FindChild(fake, "Body");
            ItemSize = UIElementLoader.GetSize(ItemMod);
            TitleSize = UIElementLoader.GetSize(TitleMod);
            if (TailMod != null)
                TailSize = UIElementLoader.GetSize(TailMod);
        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Enity== null)
                return;
            v.y /= Enity.localScale.y;
            float y = BounceBack(v.y);
            Point += y;
            Order();
            if (y != 0)
            {
                if (Scroll != null)
                    Scroll(this, v);
            }
        }
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            startVelocity = mVelocity = back.VelocityY;
            UpdateVelocity = true;
        }
        public float Space = 0;
        /// <summary>
        /// 滚动框尺寸
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// 标头模型尺寸
        /// </summary>
        public Vector2 TitleSize;
        /// <summary>
        /// 项目尺寸
        /// </summary>
        public Vector2 ItemSize;
        /// <summary>
        /// 尾部尺寸
        /// </summary>
        public Vector2 TailSize;
        /// <summary>
        /// 标头模型
        /// </summary>
        public FakeStruct TitleMod;
        /// <summary>
        /// 尾部模型
        /// </summary>
        public FakeStruct TailMod;
        /// <summary>
        /// 项目模型
        /// </summary>
        public FakeStruct ItemMod;
        /// <summary>
        /// 项目载体模型
        /// </summary>
        public FakeStruct Body;
        /// <summary>
        /// 绑定模板数据
        /// </summary>
        public List<DataTemplate> BindingData;
        /// <summary>
        /// 标头偏移位置
        /// </summary>
        public Vector2 TitleOffset = Vector2.zero;
        /// <summary>
        /// 尾部偏移位置
        /// </summary>
        public Vector2 TailOffset = Vector2.zero;
        /// <summary>
        /// 项目偏移位置
        /// </summary>
        public Vector2 ItemOffset = Vector2.zero;
        List<ScrollItem> Titles=new List<ScrollItem>();
        List<ScrollItem> Tails=new List<ScrollItem>();
        List<ScrollItem> Items=new List<ScrollItem>();
        List<ScrollItem> Bodys=new List<ScrollItem>();
        List<ScrollItem> TitleBuffer = new List<ScrollItem>();
        List<ScrollItem> ItemBuffer = new List<ScrollItem>();
        List<ScrollItem> TailBuffer = new List<ScrollItem>();
        List<ScrollItem> TitleRecycler = new List<ScrollItem>();
        List<ScrollItem> ItemRecycler = new List<ScrollItem>();
        List<ScrollItem> TailRecycler = new List<ScrollItem>();
        List<ScrollItem> BodyBuffer = new List<ScrollItem>();
        List<ScrollItem> BodyRecycler = new List<ScrollItem>();
        /// <summary>
        /// 所有设置完毕或更新数据时刷新
        /// </summary>
        public void Refresh(float y = 0)
        {
            if (BindingData == null)
                return;
            if (ItemMod == null)
                return;
            if (ItemSize.y == 0)
                return;
            Size = Enity.SizeDelta;
            CalculSize();
            Order(true);
        }
        /// <summary>
        /// 计算尺寸
        /// </summary>
        public void CalculSize()
        {
            height = 0;
            if (BindingData == null)
                return;
            wm = (int)(ItemSize.x / Size.x);
            if (wm < 1)
                wm = 1;
            int c = BindingData.Count;
            height += TitleSize.y * c;
            if (TailMod != null)
                height += TailSize.y * c;
            for (int i = 0; i < BindingData.Count; i++)
            {
                var dat = BindingData[i].Data;
                if(dat!=null)
                {
                    int n = dat.Count;
                    int a = n / wm;
                    if (n % wm > 0)
                        a++;
                    BindingData[i].Height = a * ItemSize.y;
                    if (!BindingData[i].Hide)
                    {
                        height += BindingData[i].ShowHeight;
                    }
                }
                else
                {
                    BindingData[i].Height = 0;
                }
            }
            if (height < Size.y)
                height = Size.y;
            ActualSize.y = height;
        }
        void Order(bool force=false)
        {
            PushItems();
            float y = Point;
            float oy = 0;
            if (BindingData == null)
                return;
            for (int i = 0; i < BindingData.Count; i++)
            {
                var dat = BindingData[i];
                OrderTitle(oy - y, dat, i, force);
                oy += TitleSize.y;
                if (!dat.Hide)
                {
                    float so = dat.ShowHeight;
                    OrderBody(oy - y, dat, i, force);
                    oy += dat.ShowHeight;
                }
                if (oy - y > Size.y)
                    break;
                if(TailMod!=null)
                {
                    OrderTail(oy-y,dat,i,force);
                    oy += TailSize.y;
                    if (oy - y > Size.y)
                        break;
                }
            }
            RecycleRemain();
        }
        void OrderTitle(float os,DataTemplate dat,int index, bool force)
        {
            if (os < -TitleSize.y)
                return;
            var t = PopItem(TitleBuffer, index);
            bool u = false;
            if (t == null)
            {
                t = CreateTitle();
                t.index = index;
                u = true;
            }
            Titles.Add(t);
            t.target.localPosition = new Vector3(TitleOffset.x,  -os, 0);
            t.target.activeSelf = true;
            if(force|u)
            ItemUpdate(t.obj,dat,index,TitleCreator);
        }
        void OrderBody(float os,DataTemplate dat,int index, bool force)
        {
            if (os > Size.y)
                return;
            float h =dat.ShowHeight;
            float oe = os + h;
            if (oe < 0)
                return;
            var t = PopItem(BodyBuffer, index);
            if (t == null)
            {
                t =CreateBody();
                t.index = index;
            }
            Bodys.Add(t);
            t.target.localPosition = new Vector3(0, -os, 0);
            var size = t.target.SizeDelta;
            size.y = dat.ShowHeight;
            t.target.SizeDelta = size;
            t.target.activeSelf = true;
            if (dat.Data != null)
                for (int i = 0; i < dat.Data.Count; i++)
                    OrderItem(os, dat.Data[i], i, force, t.target);
        }
        void OrderItem(float os, object dat, int index, bool force,UIElement parent)
        {
            int r = index / wm;
            float oy = r * ItemSize.y;
            os +=oy;
            if (os < -ItemSize.y)
                return;
            if (os > Size.y + ItemSize.y)
                return;
            var t = PopItem(ItemBuffer, index);
            if (t == null)
            {
                t = CreateItem();
                t.index = index;
            }
            Items.Add(t);
            t.target.localPosition = new Vector3(ItemOffset.x,  - oy , 0);
            t.target.activeSelf = true;
            t.target.SetParent(parent);
            ItemUpdate(t.obj, dat, index, ItemCreator);
        }
        void OrderTail(float os, DataTemplate dat, int index, bool force)
        {
            if (os < -TailSize.y)
                return;
            var t = PopItem(TailBuffer, index);
            bool u = false;
            if (t == null)
            {
                t = CreateTail();
                t.index = index;
                u = true;
            }
            Tails.Add(t);
            t.target.localPosition = new Vector3(TailOffset.x, - os , 0);
            t.target.activeSelf = true;
            if (force | u)
                ItemUpdate(t.obj, dat, index, TailCreator);
        }
        /// <summary>
        /// 设置滚动框尺寸
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector2 size)
        {
            Enity.SizeDelta = size;
            Size = size;
        }
        /// <summary>
        /// 释放缓存资源
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < Titles.Count; i++)
                HGUIManager.RecycleUI(Titles[i].target);
            for (int i = 0; i < Items.Count; i++)
                HGUIManager.RecycleUI(Items[i].target);
            for (int i = 0; i < Tails.Count; i++)
                HGUIManager.RecycleUI(Tails[i].target);
            Titles.Clear();
            Items.Clear();
            Tails.Clear();
        }
        Constructor ItemCreator;
        Constructor TitleCreator;
        Constructor TailCreator;
        /// <summary>
        /// 设置标头更新函数
        /// </summary>
        /// <typeparam name="T">UI模板</typeparam>
        /// <typeparam name="U">数据模板</typeparam>
        /// <param name="action">更新函数回调</param>
        public void SetTitleUpdate<T, U>(Action<T, U, int> action) where T : class, new()
        {
            for (int i = 0; i < Titles.Count; i++)
                HGUIManager.RecycleUI(Titles[i].target);
            Titles.Clear();
            var m = new Middleware<T, U>();
            m.Invoke = action;
            TitleCreator = m;
        }
        /// <summary>
        /// 设置标头更新函数
        /// </summary>
        /// <param name="constructor">热更新的中间件</param>
        public void SetTitleUpdate(HotMiddleware constructor)
        {
            for (int i = 0; i < Titles.Count; i++)
                HGUIManager.RecycleUI(Titles[i].target);
            Titles.Clear();
            TitleCreator = constructor;
        }
        /// <summary>
        /// 设置项目更新函数
        /// </summary>
        /// <typeparam name="T">UI模板</typeparam>
        /// <typeparam name="U">数据模板</typeparam>
        /// <param name="action">更新函数回调</param>
        public void SetItemUpdate<T, U>(Action<T, U, int> action) where T : class, new()
        {
            for (int i = 0; i < Items.Count; i++)
                HGUIManager.RecycleUI(Items[i].target);
            Items.Clear();
            var m = new Middleware<T, U>();
            m.Invoke = action;
            ItemCreator = m;
        }
        /// <summary>
        /// 设置项目更新函数
        /// </summary>
        /// <param name="constructor">热更新的中间件</param>
        public void SetItemUpdate(HotMiddleware constructor)
        {
            for (int i = 0; i < Items.Count; i++)
                HGUIManager.RecycleUI(Items[i].target);
            Items.Clear();
            ItemCreator = constructor;
        }
        /// <summary>
        /// 设置标尾更新函数
        /// </summary>
        /// <typeparam name="T">UI模板</typeparam>
        /// <typeparam name="U">数据模板</typeparam>
        /// <param name="action">更新函数回调</param>
        public void SetTailUpdate<T, U>(Action<T, U, int> action) where T : class, new()
        {
            for (int i = 0; i < Tails.Count; i++)
                HGUIManager.RecycleUI(Tails[i].target);
            Tails.Clear();
            var m = new Middleware<T, U>();
            m.Invoke = action;
            TailCreator = m;
        }
        /// <summary>
        /// 设置标尾更新函数
        /// </summary>
        /// <param name="constructor">热更新的中间件</param>
        public void SetTailUpdate(HotMiddleware constructor)
        {
            for (int i = 0; i < Tails.Count; i++)
                HGUIManager.RecycleUI(Tails[i].target);
            Tails.Clear();
            TailCreator = constructor;
        }
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="obj">UI实例对象</param>
        /// <param name="dat">数据实例对象</param>
        /// <param name="index">数据索引</param>
        /// <param name="con">构造器</param>
        protected void ItemUpdate(object obj, object dat, int index,Constructor con)
        {
            if (con != null)
            {
                con.Call(obj, dat, index);
            }
        }
        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="buffer">项目缓存</param>
        /// <param name="con">构造器</param>
        /// <param name="mod">UI模型数据</param>
        /// <param name="parent">父坐标变换</param>
        /// <returns></returns>
        protected ScrollItem CreateItem(List<ScrollItem> buffer, Constructor con, FakeStruct mod, UIElement parent)
        {
            if (buffer.Count > 0)
            {
                var it = buffer[0];
                it.target.activeSelf = true;
                it.index = -1;
                buffer.RemoveAt(0);
                return it;
            }
            ScrollItem a = new ScrollItem();
            if (con == null)
            {
                a.obj = a.target = HGUIManager.Clone(mod);
            }
            else
            {
                a.obj = con.Create();
                a.target = HGUIManager.Clone(mod, con.initializer);
            }
            a.target.SetParent(parent);
            a.target.localScale = Vector3.one;
            a.target.localRotation = Quaternion.identity;
            return a;
        }
        ScrollItem CreateTitle()
        {
            return CreateItem(TitleRecycler,TitleCreator,TitleMod,TitleParent);
        }
        ScrollItem CreateItem()
        {
            return CreateItem(ItemRecycler,ItemCreator,ItemMod,BodyParent);
        }
        ScrollItem CreateTail()
        {
            return CreateItem(TailRecycler,TailCreator,TailMod,TitleParent);
        }
        ScrollItem CreateBody()
        {
            return CreateItem(BodyRecycler, null, Body, BodyParent);
        }
        /// <summary>
        /// 将源项目压入目标项目
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="src"></param>
        protected void PushItems(List<ScrollItem> tar, List<ScrollItem> src)
        {
            for (int i = 0; i < src.Count; i++)
                src[i].target.activeSelf = false;
            tar.AddRange(src);
            src.Clear();
        }
        /// <summary>
        /// 将项目压入缓存
        /// </summary>
        protected void PushItems()
        {
            PushItems(TitleBuffer,Titles);
            PushItems(ItemBuffer,Items);
            PushItems(TailBuffer,Tails);
            PushItems(BodyBuffer,Bodys);
        }
        /// <summary>
        /// 回收未被重复利用的项目
        /// </summary>
        protected void RecycleRemain()
        {
            PushItems(TitleRecycler, TitleBuffer);
            PushItems(ItemRecycler, ItemBuffer);
            PushItems(TailRecycler, TailBuffer);
            PushItems(BodyRecycler,BodyBuffer);
        }
        /// <summary>
        /// 弹出一个目标项目与数据索引相同的项目
        /// </summary>
        /// <param name="tar">缓存</param>
        /// <param name="index">数据索引</param>
        /// <returns></returns>
        protected ScrollItem PopItem(List<ScrollItem> tar, int index)
        {
            for (int i = 0; i < tar.Count; i++)
            {
                var t = tar[i];
                if (t.index == index)
                {
                    tar.RemoveAt(i);
                    t.target.activeSelf = true;
                    return t;
                }
            }
            return null;
        }
        DataTemplate hideSect;
        DataTemplate showSect;
        /// <summary>
        /// 隐藏节点,带动画
        /// </summary>
        /// <param name="template"></param>
        public void HideSection(DataTemplate template)
        {
            if (template == null)
                return;
            template.ShowHeight = template.Height;
            template.aniTime = 0;
            hideSect = template;
        }
        /// <summary>
        /// 展开节点,带动画
        /// </summary>
        /// <param name="template"></param>
        public void OpenSection(DataTemplate template)
        {
            if (template == null)
                return;
            template.ShowHeight = 0;
            template.aniTime = 0;
            showSect = template;
            template.Hide = false;
            CalculSize();
        }
        public ScrollYExtand()
        {
        }
        /// <summary>
        /// 计算动画中的尺寸
        /// </summary>
        void CalculSizeA()
        {
            height = 0;
            wm = (int)(ItemSize.x / Size.x);
            if (wm < 1)
                wm = 1;
            int c = BindingData.Count;
            height += TitleSize.y * c;
            if (TailMod != null)
                height += TailSize.y * c;
            for (int i = 0; i < BindingData.Count; i++)
            {
                if (!BindingData[i].Hide)
                {
                    height += BindingData[i].ShowHeight;
                }
            }
            if (height < Size.y)
                height = Size.y;
            ActualSize.y = height;
        }
        /// <summary>
        /// 初始速率
        /// </summary>
        protected float startVelocity;
        float mVelocity;
        public float DecayRate = 0.997f;
        protected bool UpdateVelocity = true;
        /// <summary>
        /// 帧更新,包含展开收缩动画
        /// </summary>
        /// <param name="time">时间片</param>
        public override void Update(float time)
        {
            bool up = false;
            if (hideSect != null)
            {
                up = true;
                //float a = hideSect.aniTime;
                hideSect.aniTime += time;
                if (hideSect.aniTime > 400)
                    hideSect.aniTime = 400;
                float r = hideSect.aniTime / 400;
                hideSect.ShowHeight = hideSect.Height * (1 - r);
                if (r == 1)
                {
                    hideSect.Hide = true;
                    hideSect = null;
                }
            }
            if (showSect != null)
            {
                up = true;
                //float a = showSect.aniTime;
                showSect.aniTime += time;
                if (showSect.aniTime > 400)
                    showSect.aniTime = 400;
                float r = showSect.aniTime / 400;
                showSect.ShowHeight = showSect.Height * r;
                if (r == 1)
                    showSect = null;
            }
            if(up)
            {
                if (Point + Size.y > height)
                    Point = height - Size.y;
                CalculSizeA();
                Order();
            }
            if(UpdateVelocity)
            {
                int count = UserAction.TimeSlice;
                float dr = DecayRate;
                if (mVelocity < 0)
                {
                    if (Point<0)
                    {
                        dr *= 0.9f;
                    }
                }
                else
                {
                    float max = height;
                    if (max < Size.y)
                        max = Size.y;
                    if (Point + Size.y > max)
                    {
                        dr *= 0.9f;
                    }
                }
                float y = 0;
                for (int i = 0; i < count; i++)
                {
                    y += mVelocity;
                    mVelocity *= dr;
                }
                Point += y;
                Order();
                if (mVelocity < 0.01f & mVelocity > -0.01f)
                {
                    mVelocity = 0;
                }
                if (mVelocity == 0)
                {
                    if (Point < -ScrollContent.Tolerance)
                    {
                        mVelocity = MathH.DistanceToVelocity(DecayRate, -Point);
                    }
                    else
                    {
                        float max = height + ScrollContent.Tolerance;
                        if (max < Size.y)
                            max = Size.y + ScrollContent.Tolerance;
                        if (Point + Size.y > max)
                        {
                            float d = ActualSize.y - Point - Size.y;
                            mVelocity = MathH.DistanceToVelocity(DecayRate, d * Enity.localScale.y);
                        }
                    }
                    if (mVelocity < 0.01f & mVelocity > -0.01f)
                    {
                        mVelocity = 0;
                        UpdateVelocity = false;
                    }
                }
            }
        }
        protected float BounceBack(float vy)
        {
            float r = 1;
            if (Point < 0)
            {
                if (vy < 0)
                {
                    r += Point / (Size.y * 0.5f);
                    if (r < 0)
                        r = 0;
                    mVelocity = 0;
                }
            }
            else if (Point + Size.y > height)
            {
                if (vy > 0)
                {
                    r = 1 - (Point - height + Size.y) / (Size.y * 0.5f);
                    if (r < 0)
                        r = 0;
                    else if (r > 1)
                        r = 1;
                    mVelocity = 0;
                }
            }
            vy *= r;
            return vy;
        }
    }
}
