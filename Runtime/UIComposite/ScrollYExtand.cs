using huqiang.Core.HGUI;
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
        Transform BodyParent;
        Transform TitleParent;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">数据模型</param>
        /// <param name="element">主体元素</param>
        public override void Initial(FakeStruct fake,UIElement element,Initializer initializer)
        {
            base.Initial(fake,element,initializer);
            element.SizeChanged = (o) => { Refresh(); };
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.ScrollEndY = OnScrollEnd;
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            Size = Enity.SizeDelta;
            eventCall.CutRect = true;
            var trans = element.transform;
            BodyParent = trans.Find("Bodys");
            TitleParent = trans.Find("Titles");
            HGUIManager.GameBuffer.RecycleChild(Enity.gameObject,new string[]{ "Bodys", "Titles" });
           
            TitleMod =  HGUIManager.FindChild(fake,"Title");
            ItemMod = HGUIManager.FindChild(fake, "Item");
            TailMod = HGUIManager.FindChild(fake, "Tail");
            Body = HGUIManager.FindChild(fake, "Body");
            unsafe
            {
                ItemSize = ((UITransfromData*)ItemMod.ip)->size;
                TitleSize= ((UITransfromData*)TitleMod.ip)->size;
                if(TailMod!=null)
                    TailSize= ((UITransfromData*)TailMod.ip)->size;
            }
        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            if (Enity== null)
                return;
            v.y /= Enity.transform.localScale.y;
            back.VelocityX = 0;
            v.x = 0;
            float x = 0;
            float y = 0;
            y = BounceBack(back, ref v, ref x, ref Point).y;
            Order();
            if (y != 0)
            {
                if (Scroll != null)
                    Scroll(this, v);
            }
        }
        void OnScrollEnd(UserEvent back)
        {
            if (Point < -ScrollContent.Tolerance)
            {
                back.DecayRateY = 0.988f;
                float d = -Point;
                back.ScrollDistanceY = d * Enity.transform.localScale.y;
            }
            else
            {
                float max = height + ScrollContent.Tolerance;
                if (max < Size.y)
                    max = Size.y + ScrollContent.Tolerance;
                if (Point + Size.y > max)
                {
                    back.DecayRateY = 0.988f;
                    float d = ActualSize.y - Point - Size.y;
                    back.ScrollDistanceY = d * Enity.transform.localScale.y;
                }
            }
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
        int max_count;
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
            t.target.gameObject.SetActive(true);
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
            var ui = t.target.GetComponent<UIElement>();
            var size = ui.SizeDelta;
            size.y = dat.ShowHeight;
            ui.SizeDelta = size;
            t.target.gameObject.SetActive(true);
            if (dat.Data != null)
                for (int i = 0; i < dat.Data.Count; i++)
                    OrderItem(os, dat.Data[i], i, force, t.target);
        }
        void OrderItem(float os, object dat, int index, bool force,Transform parent)
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
            t.target.gameObject.SetActive(true);
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
            t.target.gameObject.SetActive(true);
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
                HGUIManager.GameBuffer.RecycleGameObject(Titles[i].target.gameObject);
            for (int i = 0; i < Items.Count; i++)
                HGUIManager.GameBuffer.RecycleGameObject(Items[i].target.gameObject);
            for (int i = 0; i < Tails.Count; i++)
                HGUIManager.GameBuffer.RecycleGameObject(Tails[i].target.gameObject);
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
                HGUIManager.GameBuffer.RecycleGameObject(Titles[i].target.gameObject);
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
                HGUIManager.GameBuffer.RecycleGameObject(Titles[i].target.gameObject);
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
                HGUIManager.GameBuffer.RecycleGameObject(Items[i].target.gameObject);
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
                HGUIManager.GameBuffer.RecycleGameObject(Items[i].target.gameObject);
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
                HGUIManager.GameBuffer.RecycleGameObject(Tails[i].target.gameObject);
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
                HGUIManager.GameBuffer.RecycleGameObject(Tails[i].target.gameObject);
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
        protected ScrollItem CreateItem(List<ScrollItem> buffer, Constructor con, FakeStruct mod, Transform parent)
        {
            if (buffer.Count > 0)
            {
                var it = buffer[0];
                it.target.gameObject.SetActive(true);
                it.index = -1;
                buffer.RemoveAt(0);
                return it;
            }
            ScrollItem a = new ScrollItem();
            if (con == null)
            {
                a.obj = a.target = HGUIManager.GameBuffer.Clone(mod).transform;
            }
            else
            {
                a.obj = con.Create();
                a.target = HGUIManager.GameBuffer.Clone(mod, con.initializer).transform;
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
                src[i].target.gameObject.SetActive(false);
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
                    t.target.gameObject.SetActive(true);
                    return t;
                }
            }
            return null;
        }
        /// <summary>
        /// 回弹滚动
        /// </summary>
        /// <param name="eventCall">用户事件</param>
        /// <param name="v">参考移动量</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected Vector2 BounceBack(UserEvent eventCall, ref Vector2 v, ref float x, ref float y)
        {
            if (eventCall.Pressed)
            {
                float r = 1;
                if (y < 0)
                {
                    if (v.y < 0)
                    {
                        r += y / (Size.y * 0.5f);
                        if (r < 0)
                            r = 0;
                        eventCall.VelocityY = 0;
                    }
                }
                else if (y + Size.y > height)
                {
                    if (v.y > 0)
                    {
                        r = 1 - (y - height + Size.y) / (Size.y * 0.5f);
                        if (r < 0)
                            r = 0;
                        else if (r > 1)
                            r = 1;
                        eventCall.VelocityY = 0;
                    }
                }
                y += v.y * r;
            }
            else
            {
                x -= v.x;
                y += v.y;
                if (x < 0)
                {
                    if (v.x > 0)
                        if (eventCall.DecayRateX >= 0.99f)
                        {
                            eventCall.DecayRateX = 0.9f;
                            eventCall.VelocityX = eventCall.VelocityX;
                        }
                }
                else if (x + Size.x > ActualSize.x)
                {
                    if (v.x < 0)
                        if (eventCall.DecayRateX >= 0.95f)
                        {
                            eventCall.DecayRateX = 0.9f;
                            eventCall.VelocityX = eventCall.VelocityX;
                        }
                }
                if (y < 0)
                {
                    if (v.y < 0)
                        if (eventCall.DecayRateY >= 0.95f)
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                }
                else if (y + Size.y > ActualSize.y)
                {
                    if (v.y > 0)
                        if (eventCall.DecayRateY >= 0.95f)
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                }
            }
            return v;
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
        void CalculSizeD()
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
        /// 帧更新,包含展开收缩动画
        /// </summary>
        /// <param name="time">时间片</param>
        public override void Update(float time)
        {
            bool up = false;
            if (hideSect != null)
            {
                up = true;
                float a = hideSect.aniTime;
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
                float a = showSect.aniTime;
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
                CalculSizeD();
                Order();
            }
            if(eventCall!=null)
            {
               if(eventCall.VelocityY==0)
                {
                    OnScrollEnd(eventCall);
                }
            }
        }
    }
}
