using huqiang.Core.HGUI;
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
        None, Loop, BounceBack
    }
    public unsafe struct ScrollInfo
    {
        public ScrollType scrollType;
        public Vector2 minBox;
        public static int Size = sizeof(ScrollInfo);
        public static int ElementSize = Size / 4;
    }
    public class Constructor
    {
        public virtual object Create() { return null; }
        public virtual void Call(object obj, object dat, int index) {
            if (Update != null)
                Update(obj,dat,index);
        }
        public bool create;
        public Action<object, object, int> Update;
        public Func<Transform, object> reflect;
        public Initializer initializer;
    }
    public class Middleware<T, U> : Constructor where T : class, new()
    {
        public Middleware()
        {
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        public override object Create()
        {
            var t = new T();
            initializer.Reset(t);
            return t;
        }
        public Action<T, U, int> Invoke;
        U u;
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
    public class HotMiddleware : Constructor
    {
        public object Context;
        public Func<object> creator;
        public Action<object, object, int> caller;
        public override object Create()
        {
            if (creator == null)
                return null;
            return creator();
        }
        public override void Call(object obj, object dat, int index)
        {
            if (caller != null)
                caller(obj, dat, index);
        }
    }
    public class ScrollContent: Composite
    {
        /// <summary>
        /// 滚动公差值
        /// </summary>
        public static float Tolerance = 0.25f;
        public ScrollType scrollType = ScrollType.BounceBack;
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        public Vector2 Size;//scrollView的尺寸
        public Vector2 ActualSize { get; protected set; }//相当于Content的尺寸
        public Vector2 ItemSize = new Vector2(1,1);
        FakeStruct modData;
        public FakeStruct ItemMod
        {
            set
            {
                modData = value;
                var c = Items.Count;
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
        public Vector2 ItemOffset = Vector2.zero;
        public Vector2 MinBox;
        protected int max_count;
        public List<ScrollItem> Items=new List<ScrollItem>();
        List<ScrollItem> Buffer=new List<ScrollItem>();
        List<ScrollItem> Recycler = new List<ScrollItem>();
        /// <summary>
        /// 当某个ui超出Mask边界，被回收时调用
        /// </summary>
        public Action<ScrollItem> ItemRecycle;
        public Transform Main;
        protected UISlider m_slider;
        public virtual UISlider Slider { get; set; }
        public override void Initial(FakeStruct mod, UIElement script)
        {
            base.Initial(mod,script);
            Main = script.transform;
            int c = Main.childCount;
            if (c > 0)
            {
                var it = Main.Find("Item").gameObject;
                HGUIManager.GameBuffer.RecycleGameObject(it);
                var sli = Main.Find("Slider");
                if (sli != null)
                {
                    var ui = sli.GetComponent<UIElement>();
                    Slider = ui.composite as UISlider;
                }
                ItemMod = HGUIManager.FindChild(mod, "Item");
                unsafe
                {
                    ItemSize = ((TransfromData*)ItemMod.ip)->size;
                    var ex = mod.buffer.GetData(((TransfromData*)mod.ip)->ex) as FakeStruct;
                    if (ex != null)
                    {
                        ScrollInfo* tp = (ScrollInfo*)ex.ip;
                        scrollType = tp->scrollType;
                        MinBox = tp->minBox;
                    }
                }
            }
        }
        public virtual void Refresh(float x = 0, float y = 0)
        {
        }
        protected void Initialtems()
        {
            int x = (int)(Size.x / ItemSize.x) + 2;
            int y = (int)(Size.y / ItemSize.y) + 2;
            max_count = x * y;
        }
        protected ScrollItem CreateItem()
        {
            if (Recycler.Count> 0)
            {
                var it = Recycler[0];
                it.target.gameObject.SetActive(true);
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
                    a.target = HGUIManager.GameBuffer.Clone(ItemMod, creator.initializer).transform;
                }
                else
                {
                    var go = HGUIManager.GameBuffer.Clone(ItemMod);
                    a.obj = go;
                    a.target = go.transform;
                }
            }
            else
            {
                var go = HGUIManager.GameBuffer.Clone(ItemMod);
                a.obj = go;
                a.target = go.transform;
            }
            a.target.SetParent(Main);
            a.target.SetAsFirstSibling();
            a.target.localRotation = Quaternion.identity;
            return a;
        }
        Constructor creator;
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
        public virtual void Order(float os, bool force = false)
        {
        }
        public void Clear()
        {
            for (int i = 0; i < Items.Count; i++)
                HGUIManager.GameBuffer.RecycleGameObject(Items[i].target.gameObject);
            for (int i = 0; i < Recycler.Count; i++)
                HGUIManager.GameBuffer.RecycleGameObject(Recycler[i].target.gameObject);
            Items.Clear();
            Recycler.Clear();
        }
        protected void PushItems()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].target.gameObject.SetActive(false);
            Buffer.AddRange(Items);
            Items.Clear();
        }
        protected ScrollItem PopItem(int index)
        {
            for(int i=0;i<Buffer.Count;i++)
            {
                var t = Buffer[i];
                if(t.index==index)
                {
                    Buffer.RemoveAt(i);
                    t.target.gameObject.SetActive(true);
                    return t;
                }
            }
            var it = CreateItem();
            return it;
        }
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
        /// 回收范围外的条目
        /// </summary>
        /// <param name="down"></param>
        /// <param name="top"></param>
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
        protected void RecycleRemain()
        {
            for (int i = 0; i < Buffer.Count; i++)
                Buffer[i].target.gameObject.SetActive(false);
            Recycler.AddRange(Buffer);
            Buffer.Clear();
        }
        protected void RecycleItem(ScrollItem it)
        {
            it.target.gameObject.SetActive(false);
            Recycler.Add(it);
            if (ItemRecycle != null)
                ItemRecycle(it);
        }
        protected Vector2 ScrollNone(UserEvent eventCall,ref Vector2 v,ref float x,ref float y)
        {
            Vector2 v2 = Vector2.zero;
            float vx = x - v.x;
            if (vx < 0)
            {
                x = 0;
                eventCall.VelocityX = 0;
                v.x = 0;
            }
            else if (vx + Size.x > ActualSize.x)
            {
                x = ActualSize.x - Size.x;
                eventCall.VelocityX = 0;
                v.x = 0;
            }
            else
            {
                x -= v.x;
                v2.x = v.x;
            }
            float vy = y + v.y;
            if (vy < 0)
            {
                y = 0;
                eventCall.VelocityY = 0;
                v.y = 0;
            }
            else if (vy + Size.y > ActualSize.y)
            {
                y = ActualSize.y - Size.y;
                eventCall.VelocityY = 0;
                v.y = 0;
            }
            else
            {
                y += v.y;
                v2.y = v.y;
            }
            return v2;
        }
        protected Vector2 ScrollLoop(UserEvent eventCall, ref Vector2 v, ref float x, ref float y)
        {
            x -= v.x;
            y += v.y;
            if(x<0)
                x+= ActualSize.x;
            else x %= ActualSize.x;
            if (y < 0)
                y += ActualSize.y;
            else y %= ActualSize.y;
            return v;
        }
        protected Vector2 BounceBack(UserEvent eventCall, ref Vector2 v, ref float x, ref float y)
        {
            if (eventCall.Pressed)
            {
                float rx = 1;
                if (x < 0)
                {
                    if (v.x > 0)
                    {
                        rx += x / (Size.x * 0.5f);
                        if (rx < 0)
                            rx = 0;
                        eventCall.VelocityX = 0;
                    }
                }
                else if (x + Size.x > ActualSize.x)
                {
                    if (v.x < 0)
                    {
                        rx = 1 - (x - ActualSize.x + Size.x) / (Size.x * 0.5f);
                        if (rx < 0)
                            rx = 0;
                        else if (rx > 1)
                            rx = 1;
                        eventCall.VelocityX = 0;
                    }
                }
                x -= v.x * rx;
                float ry = 1;
                if (y < 0)
                {
                    if (v.y < 0)
                    {
                        ry += y / (Size.y * 0.5f);
                        if (ry < 0)
                            ry = 0;
                        eventCall.VelocityY = 0;
                    }
                }
                else if (y + Size.y > ActualSize.y)
                {
                    if (v.y > 0)
                    {
                        ry = 1 - (y - ActualSize.y + Size.y) / (Size.y * 0.5f);
                        if (ry < 0)
                            ry= 0;
                        else if (ry > 1)
                            ry = 1;
                        eventCall.VelocityY = 0;
                    }
                }
                y += v.y*ry;
            }
            else
            {
                x -= v.x;
                y += v.y;
                if (x < 0)
                {
                    if (v.x > 0)
                        if (eventCall.DecayRateX >= 0.95f)
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
        protected void ItemUpdate(object obj,object dat, int index)
        {
            if (creator != null)
            {
                creator.Call(obj, dat, index);
            }
        }
    }
}
