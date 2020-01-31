using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class LinkerMod
    {
        public int index = -1;
        public object UI;
        public GameObject main;
        public FakeStruct mod;
    }
    public class Linker
    {
        /// <summary>
        /// 实体模型,用于计算实体尺寸
        /// </summary>
        public Transform enityModel;
        public int ElementCount { get; private set; }
        protected List<LinkerMod> buffer = new List<LinkerMod>();
        public virtual LinkerMod CreateUI() { return null; }
        public virtual float GetItemSize(object u) { return 40; }
        public virtual void RefreshItem(object t, object u, int index) { }
        public virtual void SetEnityModel(Transform transform) 
        {
            enityModel = transform;
            ElementCount = 0;
            if (enityModel != null)
                GetElementCount(transform);
        }
        public void RecycleItem(LinkerMod mod)
        {
            buffer.Add(mod);
            mod.main.SetActive(false);
        }
        public LinkerMod PopItem(int index)
        {
            for(int i=0;i<buffer.Count;i++)
                if(buffer[i].index==index)
                {
                    var t = buffer[i];
                    buffer.RemoveAt(i);
                    return t;
                }
            return null;
        }
        public void ClearIndex()
        {
            for (int i = 0; i < buffer.Count; i++)
                buffer[i].index = -1;
        }
        protected void GetElementCount(Transform trans)
        {
            ElementCount++;
            for(int i=0;i<trans.childCount;i++)
            {
                GetElementCount(trans.GetChild(i));
            }
        }
    }
    /// <summary>
    /// 泛型连接器
    /// </summary>
    /// <typeparam name="T">模型</typeparam>
    /// <typeparam name="U">数据</typeparam>
    public class UILinker<T, U> : Linker where T : class, new() where U : class, new()
    {
        FakeStruct model;
        public Action<T, U, int> ItemUpdate;
        public Func<T, U, float> CalculItemHigh;
        UIContainer con;
        UIInitializer initializer;
        T uiModel;
        private UILinker()
        {
        }
        public UILinker(UIContainer container, FakeStruct mod)
        {
            con = container;
            model = mod;
            container.linkers.Add(this);
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        public void InsertData(U dat)
        {
            con.InsertData(this,dat);
        }
        public void AddData(U dat)
        {
            con.AddData(this, dat);
        }
        public void AddAndMove(U dat)
        {
            float h;
            if (CalculItemHigh != null)
                h = CalculItemHigh(uiModel, dat);
            else
                unsafe { h = ((TransfromData*)model.ip)->size.y; }
            con.AddAndMove(this, dat, h);
        }
        public void AddAndMoveEnd(U dat)
        {
            float h;
            if (CalculItemHigh != null)
                h = CalculItemHigh(uiModel, dat);
            else
                unsafe { h = ((TransfromData*)model.ip)->size.y; }
            con.AddAndMoveEnd(this, dat, h);
        }
        public override LinkerMod CreateUI()
        {
            for(int i=0;i<buffer.Count;i++)
                if(buffer[i].index<0)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            LinkerMod mod = new LinkerMod();
            T t = new T();
            if (initializer == null)
                initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
            initializer.Reset(t);
            mod.main = HGUIManager.GameBuffer.Clone(model,initializer);
            mod.UI = t;
            return mod;
        }
        public override float GetItemSize(object u)
        {
            if (CalculItemHigh != null)
                return CalculItemHigh(uiModel, u as U);
            unsafe { return ((TransfromData*)model.ip)->size.y; }
        }
        public override void RefreshItem(object t, object u, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(t as T, u as U, index);
        }
        public override void SetEnityModel(Transform transform)
        {
            enityModel = transform;
            if (enityModel != null)
            {
                uiModel = new T();
                initializer.ReflectionEnity(uiModel, transform);
                GetElementCount(transform);
            }
        }
    }
    /// <summary>
    /// 对象型连接器，用用于热更新块
    /// </summary>
    public class ObjectLinker : Linker
    {
        FakeStruct model;
        public Action<object, object, int> ItemUpdate;
        public Action<ObjectLinker, LinkerMod> ItemCreate;
        public Func<object, object, float> CalculItemHigh;
        UIContainer con;
        public ObjectLinker(UIContainer container, FakeStruct mod)
        {
            con = container;
            model = mod;
            container.linkers.Add(this);
        }
        public void AddData(object dat)
        {
            con.AddData(this, dat);
        }
        public override LinkerMod CreateUI()
        {
            LinkerMod mod = new LinkerMod();
            mod.main = HGUIManager.GameBuffer.Clone(model);
            if (ItemCreate != null)
                ItemCreate(this, mod);
            return mod;
        }
        public override void RefreshItem(object t, object u, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(t, u, index);
        }
    }
    public struct BaseLayout
    {
        public Vector3 position;
        public Vector2 sizeDelta;
        public Quaternion rotate;
    }
    public class BindingData
    {
        public float offset;
        public float width;
        public float high;
        public object Data;
        public Linker linker;
        public BaseLayout[] layouts;
        static int id = 0;
        public void ApplayLayout(Transform trans)
        {
            id = 0;
            if (layouts != null)
                ApplayLayout(trans, layouts);
        }
        static void ApplayLayout(Transform trans, BaseLayout[] layouts)
        {
            if (id >= layouts.Length)
                return;
            trans.localPosition = layouts[id].position;
            var ui = trans.GetComponent<UIElement>();
            if (ui != null)
                ui.SizeDelta = layouts[id].sizeDelta;
            var c = trans.childCount;
            id++;
            for (int i = 0; i < c; i++)
                ApplayLayout(trans.GetChild(i), layouts);
        }
        public void LoadLayout(Transform trans)
        {
            id = 0;
            if (layouts != null)
                LoadLayout(trans, layouts);
        }
        static void LoadLayout(Transform trans, BaseLayout[] layouts)
        {
            if (id >= layouts.Length)
                return;
            layouts[id].position = trans.localPosition;
            var ui = trans.GetComponent<UIElement>();
            if (ui != null)
                layouts[id].sizeDelta = ui.SizeDelta;
            var c = trans.childCount;
            id++;
            for (int i = 0; i < c; i++)
                LoadLayout(trans.GetChild(i), layouts);
        }
     
    }
    public class UIContainer:Composite
    {
        class Item
        {
            public Linker linker;
            public LinkerMod mod;
            public object UI;
            public object Data;
            public BindingData binding;
            public int Index = -1;
            public Vector3 pos;
            public Vector2 size;
            public GameObject main;
            public float offset;
            public float high;
        }
        public UserEvent eventCall;
        /// <summary>
        /// 最大值滚动框的0.5倍
        /// </summary>
        public float OutRatio { get; private set; }
        /// <summary>
        /// 用于上拉刷新
        /// </summary>
        public Action<UIContainer> ScrollOutDown;
        /// <summary>
        /// 用于下拉刷新
        /// </summary>
        public Action<UIContainer> ScrollOutTop;
        List<Item> items;
        List<BindingData> datas = new List<BindingData>();
        public FakeStruct model;
        public List<Linker> linkers = new List<Linker>();
        public UIContainer()
        {
        }
        public override void Initial(FakeStruct fake, UIElement script)
        {
            base.Initial(fake, script);
            items = new List<Item>();
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.AutoColor = false;
            eventCall.ForceEvent = true;
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => {
                if (o.VelocityY == 0)
                    OnScrollEnd(o);
                else Scrolling(o,s);
            };
            eventCall.Scrolling = Scrolling;
            eventCall.ScrollEndY = OnScrollEnd;
            eventCall.MouseWheel= (o,e) => { Move(e.MouseWheelDelta*100); };
            model = fake;
            var trans = Enity.transform;
            for (int i = 0; i < trans.childCount; i++)
                trans.GetChild(i).gameObject.SetActive(false);
        }
        public UILinker<T, U> RegLinker<T,U>(string ItemName)  where T : class, new() where U : class, new()
        {
            if (model == null)
                return null;
            var mod = HGUIManager.FindChild(model, ItemName);
            if (mod == null)
                return null;
            var trans = Enity.transform.Find(ItemName);
            UILinker<T, U> link = new UILinker<T, U>(this, mod);
            link.SetEnityModel(trans);
            return link;
        }
        public int DataCount { get { return datas.Count; } }
        public BindingData InsertData(Linker linker,object data)
        {
            BindingData binding = new BindingData();
            binding.linker = linker;
            binding.Data = data;
            binding.layouts = new BaseLayout[linker.ElementCount];
            datas.Insert(0,binding);
            index++;
            return binding;
        }
        public BindingData AddData(Linker linker, object data)
        {
            BindingData binding = new BindingData();
            binding.linker = linker;
            binding.Data = data;
            binding.layouts = new BaseLayout[linker.ElementCount];
            datas.Add(binding);
            return binding;
        }
        public void AddAndMove(Linker linker, object data, float h)
        {
            float end = End;
            BindingData binding = AddData(linker, data);
            binding.LoadLayout(linker.enityModel);
            binding.width = Enity.SizeDelta.x;
            binding.offset = end;
            binding.high = h;
            float y = end + h - Point - Enity.SizeDelta.y;
            if (y < 0)
                y = 0;
            else if (y > h)
                y = h;
            Move(y);
        }
        public void AddAndMoveEnd(Linker linker, object data,float h)
        {
            float end = End;
            BindingData binding = AddData(linker, data);
            binding.LoadLayout(linker.enityModel);
            binding.width = Enity.SizeDelta.x;
            binding.offset = end;
            binding.high = h;
            float y = end + h - Point - Enity.SizeDelta.y;
            if (y < 0)
                y = 0;
            Move(y);
        }
        void Scrolling(UserEvent scroll, Vector2 offset)
        {
            if (offset != Vector2.zero)
                if (datas.Count > 0)
                {
                    BounceBack(scroll, ref offset);
                    if (offset.y < 0)
                        MoveBack(offset.y);
                    else MoveForward(offset.y);
                    Order();
                    if (outState < 0)
                    {
                        if (ScrollOutTop != null)
                            ScrollOutTop(this);
                    }
                    else if (outState > 0)
                    {
                        if (ScrollOutDown != null)
                            ScrollOutDown(this);
                    }
                }
        }
        void OnScrollEnd(UserEvent back)
        {
            if (datas.Count == 0)
                return;
            if (OutBack())
            {
                back.DecayRateY = 0.988f;
                float d = -datas[index].high * offsetRatio;
                if (d > 0.01f)
                    back.ScrollDistanceY = d * Enity.transform.localScale.y;
                else back.VelocityY = 0;
            }
            else if (OutForward())
            {
                back.DecayRateY = 0.988f;
                float cl = End - Start;
                float l = Enity.SizeDelta.y;
                if (l > cl)
                    l = cl;
                int c = datas.Count - 1;
                float e = datas[c].offset + datas[c].high;
                float d = l - e + Point;
                if (d > 0.01f)
                    back.ScrollDistanceY = -d * Enity.transform.localScale.y;
                else back.VelocityY = 0;
            }
        }
        public void Move(float y)
        {
            if (y < 0)
            {
                MoveBack(y);
                if (index == 0)
                    if (offsetRatio < 0)
                        offsetRatio = 0;
            }
            else if (y > 0)
            {
                float a = End - Point - Enity.SizeDelta.y;
                if (a < 0)
                    a = 0;
                else if (a > y)
                    a = y;
                MoveForward(a);
            }
            Order();
        }
        List<Item> buffer = new List<Item>();
        void Order()
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.linker.RecycleItem(item.mod);
            }
            buffer.AddRange(items);
            items.Clear();
            float w = Enity.SizeDelta.x;
            float os = datas[index].offset;
            float start = -datas[index].high * offsetRatio;
            for (int i = index; i < datas.Count; i++)
            {
                var data = datas[i];
                if(data.width!=w)
                {
                    data.high = data.linker.GetItemSize(data.Data);
                    data.LoadLayout(data.linker.enityModel);
                    data.offset = os;
                    data.width = w;
                }
                UpdateItem(data, i, start);
                start += data.high;
                os += data.high;
                if (start > Enity.SizeDelta.y)
                {
                    break;
                }
            }
            for (int i = 0; i < buffer.Count; i++)
                buffer[i].Index = -1;
            for (int i = 0; i < linkers.Count; i++)
                linkers[i].ClearIndex();
        }
        Item FindOrCreateItem(int index)
        {
            for(int i=0;i<buffer.Count;i++)
                if(buffer[i].Index==index)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            for (int i = 0; i < buffer.Count; i++)
                if (buffer[i].Index <0)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            return new Item();
        }
        void UpdateItem(BindingData data, int index, float offset)
        {
            var mod = data.linker.PopItem(index);
            if (mod == null)
            {
                mod = data.linker.CreateUI();
                mod.main.transform.SetParent(Enity.transform);
            }
            mod.main.SetActive(true);
            var son = mod.main.transform;
            var item = FindOrCreateItem(index);
            if (item.Index < 0)
            {
                mod.index = index;
                data.linker.RefreshItem(mod.UI, data.Data, index);
                item.mod = mod;
                item.linker = data.linker;
                item.Index = index;
                item.binding = data;
                item.Data = data.Data;
                item.UI = mod.UI;
                item.offset = data.offset;
                item.high = data.high;
                item.main = mod.main;
                son.SetParent(Enity.transform);
                son.localScale = Vector3.one;
            }
            else
            {
                mod.index = index;
                item.mod = mod;
            }
            data.ApplayLayout(son);
            items.Add(item);
            son.localPosition = new Vector3(0, -offset, 0);
        }
        bool OutBack()
        {
            if (index== 0)
                if (offsetRatio < 0)
                    return true;
            return false;
        }
        bool OutForward()
        {
            if (offsetRatio > 1)
                return true;
            float w = Enity.SizeDelta.x;
            float l = Enity.SizeDelta.y;
            int end = datas.Count;

            var os = datas[index].offset + datas[index].high;
            float start = -datas[index].high * (offsetRatio - 1);
            for (int i = index + 1; i < end; i++)
            {
                var dat = datas[i];
                if (dat.width != w)//如果当前数据未计算实际高度
                {
                    dat.high = dat.linker.GetItemSize(dat.Data);
                    dat.LoadLayout(dat.linker.enityModel);
                    dat.offset = os;
                    dat.width = w;
                }
                os += dat.high;
                start += dat.high;
                if (start > l)
                    return false;
            }
            return true;
        }

        int index;
        float offsetRatio;
        int outState;
        float Point {
            get
            {
                if (datas == null)
                    return 0;
                if (datas.Count < 1)
                    return 0;
               return datas[index].offset + datas[index].high * offsetRatio;
            }
            set
            {
                if (datas == null)
                    return;
                if (datas.Count < 1)
                    return;
                if(value<=datas[0].offset)
                {
                    index = 0;
                    offsetRatio = (value - datas[0].offset) / datas[0].high;
                    return;
                }else
                {
                    var c = datas.Count;
                    if(value>=datas[c].offset+datas[c].high)
                    {
                        index = c;
                        offsetRatio = (value - datas[c].offset) / datas[c].high;
                        return;
                    }
                }
                for (int i = 0; i < datas.Count; i++)
                {
                    var dt = datas[i];
                    if (value >= dt.offset & value <= dt.offset + dt.high)
                    {
                        index = i;
                        offsetRatio = (value - dt.offset) / dt.high;
                        break;
                    }
                }
            }
        }
        float Start
        {
            get
            {
                if (datas == null)
                    return 0;
                if (datas.Count < 1)
                    return 0;
                return datas[0].offset;
            }
        }
        float End
        {
            get
            {
                if (datas == null)
                    return 0;
                if (datas.Count < 1)
                    return 0;
                var c = datas.Count - 1;
                return datas[c].offset + datas[c].high;
            }
        }
        /// <summary>
        /// 向起点滚动
        /// </summary>
        /// <param name="y"></param>
        void MoveBack(float y)
        {
            float w = Enity.SizeDelta.x;
            float p = Point + y;
            float os = datas[index].offset;
            for (int i = index; i >= 0; i--)
            {
                var dat = datas[i];
                if (dat.width != w)//如果当前数据未计算实际高度
                {
                    dat.high = dat.linker.GetItemSize(dat.Data);
                    dat.LoadLayout(dat.linker.enityModel);
                    dat.offset = os - dat.high;
                    dat.width = w;
                }
                if (p > dat.offset)//如果当前指针大于数据的起始位置
                {
                    index = i;
                    offsetRatio = (p - datas[i].offset) / datas[i].high;
                    return;
                }
                os = dat.offset;
            }
            if (p < datas[0].offset)//偏移百分比为负数
            {
                index = 0;
                offsetRatio = (p - datas[0].offset) / datas[0].high;
            }
        }
        /// <summary>
        /// 向终点滚动
        /// </summary>
        /// <param name="y"></param>
        void MoveForward(float y)
        {
            float w = Enity.SizeDelta.x;
            float op = Point;
            float p = op + y;
            float os = datas[index].offset;
            int end = datas.Count;
            for (int i = index; i <end; i++)
            {
                var dat = datas[i];
                if (dat.width!= w)//如果当前数据未计算实际高度
                {
                    dat.high = dat.linker.GetItemSize(dat.Data);
                    dat.LoadLayout(dat.linker.enityModel);
                    dat.offset = os;
                    dat.width = w;
                }
                if (p < dat.offset+dat.high)//如果当前指针小于数据的结束位置
                {
                    index = i;
                    offsetRatio = (p - datas[i].offset) / datas[i].high;
                    return;
                }
                os += dat.high;
            }
            end--;
            if (end >= 0)
                if (p > datas[end].offset + datas[end].high)//偏移百分比为大于1
                {
                    index = end;
                    offsetRatio = (p - datas[end].offset) / datas[end].high;
                }
        }
        protected void BounceBack(UserEvent eventCall, ref Vector2 v)
        {
            OutRatio = 0;
            outState = 0;
            float y = Enity.SizeDelta.y;
            if (eventCall.Pressed)
            {
                if (v.y < 0)//往起点移动
                {
                    if (OutBack())
                    {
                        float l = y*0.5f;
                        float f = datas[0].high * -offsetRatio;
                        float d = 1 - f / l;
                        if (d > 1)
                            d = 1;
                        else if (d < 0)
                            d = 0;
                        OutRatio = d;
                        v.y *= d;
                        eventCall.VelocityY = 0;
                        outState = -1;
                    }
                }
                else if (v.y > 0)//往终点移动
                {
                    if (OutForward())
                    {
                        float end = End;
                        float cl = end - Start;
                        if (cl < y)
                            end += y - cl;
                        float l = y * 0.5f;
                        float os = end - Point - l;
                        if (os < 0)
                            os = 0;
                        float r = os / l;
                        OutRatio = r;
                        v.y *= r;
                        eventCall.VelocityY = 0;
                        outState = 1;
                    }
                }
            }
            else
            {
                if (v.y < 0)
                {
                    if (eventCall.DecayRateY >= 0.95f)
                    {
                        if (OutBack())
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                    }
                }
                else if (v.y > 0)
                {
                    if (eventCall.DecayRateY >= 0.95f)
                    {
                        if (OutForward())
                        {
                            float l = y;
                            int c = datas.Count - 1;
                            float e = datas[c].offset + datas[c].high;
                            float d = l - e + Point;
                            if (d < 0.01f)
                            {
                                eventCall.VelocityY = 0;
                            }
                            else
                            {
                                eventCall.DecayRateY = 0.9f;
                                eventCall.VelocityY = eventCall.VelocityY;
                            }
                        }
                    }
                }
            }
        }
        //void ReSized()
        //{
        //    for(int i=0;i<items.Count;i++)//重新计算内容的高度
        //    {
        //        var it = items[i];
        //        it.binding.high = it.high = it.binding.linker.GetItemSize(it.Data);
        //    }
        //}
    }
}
