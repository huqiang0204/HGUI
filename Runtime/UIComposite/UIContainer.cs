using huqiang.Core.HGUI;
using huqiang.Core.UIData;
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
        public UIElement main;
        public FakeStruct mod;
    }
    public class Linker
    {
        /// <summary>
        /// 实体模型,用于计算实体尺寸
        /// </summary>
        public UIElement enityModel;
        /// <summary>
        /// 元素个数
        /// </summary>
        public int ElementCount { get; private set; }
        /// <summary>
        /// 连接模型缓存
        /// </summary>
        protected List<LinkerMod> buffer = new List<LinkerMod>();
        /// <summary>
        /// 创建一个连接模型
        /// </summary>
        /// <returns></returns>
        public virtual LinkerMod CreateUI() { return null; }
        /// <summary>
        /// 获取项目尺寸,如果没有则返回默认高度40
        /// </summary>
        /// <param name="u">模型实例</param>
        /// <returns></returns>
        public virtual float LayoutItem(object u) { return 40; }
        /// <summary>
        /// 刷新项目内容
        /// </summary>
        /// <param name="t">项目UI实例</param>
        /// <param name="u">数据实例</param>
        /// <param name="index">数据索引</param>
        public virtual void RefreshItem(object t, object u, int index) { }
        /// <summary>
        /// 回收项目连接器
        /// </summary>
        /// <param name="mod"></param>
        public void RecycleItem(LinkerMod mod)
        {
            buffer.Add(mod);
            mod.main.activeSelf = false;
        }
        /// <summary>
        /// 弹出一个项目连接器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 清除索引
        /// </summary>
        public void ClearIndex()
        {
            for (int i = 0; i < buffer.Count; i++)
                buffer[i].index = -1;
        }
        /// <summary>
        /// 获取子元素计数
        /// </summary>
        /// <param name="trans"></param>
        protected void GetElementCount(UIElement trans)
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
        public Func<UIElement, U, float> LayoutCallback;
        UIContainer con;
        UIInitializer initializer;
        T uiModel;
        private UILinker()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container">模型名称</param>
        /// <param name="mod">模式数据</param>
        public UILinker(UIContainer container, UIElement mod)
        {
            enityModel = mod;
            enityModel.activeSelf = false;
            con = container;
            model = enityModel.mod;
            container.linkers.Add(this);
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
            uiModel = new T();
            initializer.ReflectionEnity(uiModel,mod);
            GetElementCount(mod);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dat"></param>
        public void InsertData(U dat)
        {
            con.InsertData(this,dat);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="dat"></param>
        public void AddData(U dat)
        {
            con.AddData(this, dat);
        }
        /// <summary>
        /// 添加数据并移动指定长度
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="h">数据UI高度</param>
        public void AddAndMove(U dat,float h)
        {
            if (h <= 0)
            {
                h = UIElementLoader.GetSize(model).y;
            }
            con.AddAndMove(this, dat, h);
        }
        /// <summary>
        /// 添加数据并移动到底部
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="h">数据UI高度</param>
        public void AddAndMoveEnd(U dat,float h)
        {
            if (h<=0)
            {
                h = UIElementLoader.GetSize(model).y;
            }
            con.AddAndMoveEnd(this, dat, h);
        }
        /// <summary>
        /// 创建项目连接器
        /// </summary>
        /// <returns></returns>
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
            mod.main = HGUIManager.Clone(model,initializer);
            mod.UI = t;
            return mod;
        }
        /// <summary>
        /// 获取项目尺寸
        /// </summary>
        /// <param name="u">项目实例</param>
        /// <returns></returns>
        public override float LayoutItem(object u)
        {
            if (LayoutCallback != null)
                return LayoutCallback(enityModel, u as U);
            return UIElementLoader.GetSize(model).y;
        }
        /// <summary>
        /// 刷新项目显示内容
        /// </summary>
        /// <param name="t">项目UI实例</param>
        /// <param name="u">数据实例</param>
        /// <param name="index">数据索引</param>
        public override void RefreshItem(object t, object u, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(t as T, u as U, index);
        }
    }
    /// <summary>
    /// 对象型连接器，用用于热更新块
    /// </summary>
    public class ObjectLinker : Linker
    {
        /// <summary>
        /// 项目更新委托
        /// </summary>
        public Action<object, object, int> ItemUpdate;
        /// <summary>
        /// 项目连接器创建委托
        /// </summary>
        public Action<ObjectLinker, LinkerMod> ItemCreate;
        /// <summary>
        /// 项目高度计算委托
        /// </summary>
        public Func<UIElement, object, float> LayoutCallback;
        UIContainer con;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container">容器实例</param>
        public ObjectLinker(UIContainer container,UIElement mod)
        {
            enityModel = mod;
            enityModel.activeSelf = false;
            container.linkers.Add(this);
            GetElementCount(mod);
        }
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="dat"></param>
        public void AddData(object dat)
        {
            con.AddData(this, dat);
        }
        /// <summary>
        /// 创建UI项目连接器
        /// </summary>
        /// <returns></returns>
        public override LinkerMod CreateUI()
        {
            for (int i = 0; i < buffer.Count; i++)
                if (buffer[i].index < 0)
                {
                    var item = buffer[i];
                    buffer.RemoveAt(i);
                    return item;
                }
            LinkerMod mod = new LinkerMod();
            if (ItemCreate != null)
                ItemCreate(this, mod);
            return mod;
        }
        /// <summary>
        /// 刷新项目内容
        /// </summary>
        /// <param name="t">项目实例</param>
        /// <param name="u">数据实例</param>
        /// <param name="index">数据索引</param>
        public override void RefreshItem(object t, object u, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(t, u, index);
        }
        /// <summary>
        /// 获取项目尺寸
        /// </summary>
        /// <param name="u">项目实例</param>
        /// <returns></returns>
        public override float LayoutItem(object u)
        {
            if (LayoutCallback != null)
                return LayoutCallback(enityModel, u);
            if (enityModel != null)
                if (enityModel.mod != null)
                    return UIElementLoader.GetSize(enityModel.mod).y;
            return 40;
        }
    }
    public struct BaseLayout
    {
        public Vector3 position;
        public Vector2 sizeDelta;
        public Quaternion rotate;
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    public class BindingData
    {
        /// <summary>
        /// 容器中的偏移位置
        /// </summary>
        public float offset;
        /// <summary>
        /// 内容宽度
        /// </summary>
        public float width;
        /// <summary>
        /// 内容高度
        /// </summary>
        public float high;
        /// <summary>
        /// 数据
        /// </summary>
        public object Data;
        /// <summary>
        /// 连接器
        /// </summary>
        public Linker linker;
        /// <summary>
        /// 布局数据缓存
        /// </summary>
        public BaseLayout[] layouts;
        static int id = 0;
        /// <summary>
        /// 载入布局数据
        /// </summary>
        /// <param name="trans"></param>
        public void LoadLayout(UIElement trans)
        {
            id = 0;
            if (layouts != null)
                LoadLayout(trans, layouts);
        }
        static void LoadLayout(UIElement trans, BaseLayout[] layouts)
        {
            if (id >= layouts.Length)
                return;
            trans.localPosition = layouts[id].position;
            trans.SizeDelta = layouts[id].sizeDelta;
            var c = trans.childCount;
            id++;
            for (int i = 0; i < c; i++)
                LoadLayout(trans.GetChild(i), layouts);
        }
        /// <summary>
        /// 保存实例的布局数据
        /// </summary>
        /// <param name="trans"></param>
        public void SaveLayout(UIElement trans)
        {
            id = 0;
            if (layouts != null)
                SaveLayout(trans, layouts);
        }
        static void SaveLayout(UIElement trans, BaseLayout[] layouts)
        {
            if (id >= layouts.Length)
                return;
            layouts[id].position = trans.localPosition;
            layouts[id].sizeDelta = trans.SizeDelta;
            var c = trans.childCount;
            id++;
            for (int i = 0; i < c; i++)
                SaveLayout(trans.GetChild(i), layouts);
        }
     
    }
    /// <summary>
    /// UI容器
    /// </summary>
    public class UIContainer:Composite
    {
        class Item
        {
            /// <summary>
            /// 连接器
            /// </summary>
            public Linker linker;
            /// <summary>
            /// 连接器模型
            /// </summary>
            public LinkerMod mod;
            /// <summary>
            /// UI实例
            /// </summary>
            public object UI;
            /// <summary>
            /// 数据实例
            /// </summary>
            public object Data;
            /// <summary>
            /// 绑定数据
            /// </summary>
            public BindingData binding;
            /// <summary>
            /// 在容器中的索引
            /// </summary>
            public int Index = -1;
            /// <summary>
            /// 当前位置
            /// </summary>
            public Vector3 pos;
            /// <summary>
            /// 当前尺寸
            /// </summary>
            public Vector2 size;
            /// <summary>
            /// 主体实例对象
            /// </summary>
            public UIElement main;
            /// <summary>
            /// 容器中的偏移位置
            /// </summary>
            public float offset;
            /// <summary>
            /// 内容高度
            /// </summary>
            public float high;
        }
        /// <summary>
        /// 主体事件
        /// </summary>
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
        /// <summary>
        /// 模型集合
        /// </summary>
        public FakeStruct model;
        /// <summary>
        /// 连接器列表
        /// </summary>
        public List<Linker> linkers = new List<Linker>();
        public UIContainer()
        {
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fake">数据模型</param>
        /// <param name="script">元素主体</param>
        public override void Initial(FakeStruct fake, UIElement script,UIInitializer initializer)
        {
            base.Initial(fake, script, initializer);
            items = new List<Item>();
            eventCall = Enity.RegEvent<UserEvent>();
            eventCall.AutoColor = false;
            eventCall.ForceEvent = true;
            eventCall.PointerDown = (o, e) => { UpdateVelocity = false; };
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = OnDragEnd;
            //eventCall.Scrolling = Scrolling;
            //eventCall.ScrollEndY = OnScrollEnd;
            eventCall.MouseWheel= (o,e) => { Move(e.MouseWheelDelta*100); UpdateVelocity = true; };
            model = fake;
            HGUIManager.RecycleChild(Enity);
        }
        /// <summary>
        /// 数据计数
        /// </summary>
        public int DataCount { get { return datas.Count; } }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="linker">连接器</param>
        /// <param name="data">数据实例</param>
        /// <returns></returns>
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
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="linker">连接器</param>
        /// <param name="data">数据实例</param>
        /// <returns></returns>
        public BindingData AddData(Linker linker, object data)
        {
            BindingData binding = new BindingData();
            binding.linker = linker;
            binding.Data = data;
            binding.layouts = new BaseLayout[linker.ElementCount];
            datas.Add(binding);
            return binding;
        }
        /// <summary>
        /// 添加数据并移动指定长度
        /// </summary>
        /// <param name="linker">连接器</param>
        /// <param name="data">数据实例</param>
        /// <param name="h">高度</param>
        public void AddAndMove(Linker linker, object data, float h)
        {
            float end = End;
            BindingData binding = AddData(linker, data);
            //binding.LoadLayout(linker.enityModel);
            //binding.width = Enity.SizeDelta.x;
            binding.offset = end;
            binding.high = h;
            float y = end + h - Point - Enity.SizeDelta.y;
            if (y < 0)
                y = 0;
            else if (y > h)
                y = h;
            Move(y);
        }
        /// <summary>
        /// 添加数据并移动到底部
        /// </summary>
        /// <param name="linker">连接器</param>
        /// <param name="data">数据实例</param>
        /// <param name="h">数据高度</param>
        public void AddAndMoveEnd(Linker linker, object data,float h)
        {
            float end = End;
            BindingData binding = AddData(linker, data);
            //binding.LoadLayout(linker.enityModel);
            //binding.width = Enity.SizeDelta.x;
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
                    BounceBack(ref offset);
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
        void OnDragEnd(UserEvent back, UserAction action, Vector2 v)
        {
            Scrolling(back, v);
            startVelocity = mVelocity = back.VelocityY;
            UpdateVelocity = true;
        }
        public void Move(float y)
        {
            if (y < 0)
            {
                MoveBack(y);
            }
            else if (y > 0)
            {
                float ch = End - Start;
                if (ch > Enity.m_sizeDelta.y)
                {
                    float a = End - Point - Enity.m_sizeDelta.y;
                    if (a < 0)
                        a = 0;
                    else if (a > y)
                        a = y;
                    MoveForward(a);
                }
                else
                {
                    MoveForward(y);
                }
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
                    data.high = data.linker.LayoutItem(data.Data);
                    data.SaveLayout(data.linker.enityModel);
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
       UIElement UpdateItem(BindingData data, int index, float offset)
        {
            var mod = data.linker.PopItem(index);
            if (mod == null)
            {
                mod = data.linker.CreateUI();
                mod.main.SetParent(Enity);
            }
            mod.main.activeSelf = true;
            var son = mod.main;
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
                son.SetParent(Enity);
                son.localScale = Vector3.one;
            }
            else
            {
                mod.index = index;
                item.mod = mod;
            }
            items.Add(item);
            data.LoadLayout(son);
            son.localPosition = new Vector3(0, -offset, 0);
            return son;
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
            if (index == 0)
                if (offsetRatio < 0)
                    return false;
            float w = Enity.m_sizeDelta.x;
            float l = Enity.m_sizeDelta.y;
            int end = datas.Count;

            var os = datas[index].offset + datas[index].high;
            float start = -datas[index].high * (offsetRatio - 1);
            for (int i = index + 1; i < end; i++)
            {
                var dat = datas[i];
                if (dat.width != w)//如果当前数据未计算实际高度
                {
                    dat.high = dat.linker.LayoutItem(dat.Data);
                    dat.SaveLayout(dat.linker.enityModel);
                    dat.offset = os;
                    dat.width = w;
                }
                os += dat.high;
                start += dat.high;
                if (start > l )
                    return false;
            }
            return true;
        }

        int index;
        float offsetRatio;
        int outState;
       public float Point {
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
       public float Start
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
       public float End
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
                    dat.high = dat.linker.LayoutItem(dat.Data);
                    dat.SaveLayout(dat.linker.enityModel);
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
                    dat.high = dat.linker.LayoutItem(dat.Data);
                    dat.SaveLayout(dat.linker.enityModel);
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
        /// <summary>
        /// 滚动回弹限定
        /// </summary>
        /// <param name="eventCall">用户事件</param>
        /// <param name="v">参考移动量</param>
        protected void BounceBack(ref Vector2 v)
        {
            OutRatio = 0;
            outState = 0;
            float y = Enity.SizeDelta.y;
            if (v.y < 0)//往起点移动
            {
                if (OutBack())
                {
                    float l = y * 0.5f;
                    float f = datas[0].high * -offsetRatio;
                    float d = 1 - f / l;
                    if (d > 1)
                        d = 1;
                    else if (d < 0)
                        d = 0;
                    OutRatio = d;
                    v.y *= d;
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
                    outState = 1;
                }
            }
        }
        /// <summary>
        /// 初始速率
        /// </summary>
        protected float startVelocity;
        float mVelocity;
        public float DecayRate = 0.997f;
        protected bool UpdateVelocity;
        public override void Update(float time)
        {
            if (!UpdateVelocity)
                return;
            if (datas.Count == 0)
                return;
            float y = 0;
            int count = UserAction.TimeSlice;
            if (mVelocity == 0)
            {
                if (OutBack())
                {
                    float d = -datas[index].high * offsetRatio;
                    mVelocity = MathH.DistanceToVelocity(DecayRate,d+2);
                }
                else if (OutForward())
                {
                    float cl = End - Start;
                    float l = Enity.SizeDelta.y;
                    if (l > cl)
                        l = cl;
                    int c = datas.Count - 1;
                    float e = datas[c].offset + datas[c].high;
                    float d = l - e + Point;
                    mVelocity = MathH.DistanceToVelocity(DecayRate, -d - 1);
                    if (mVelocity < 0 & mVelocity > -0.01f)
                    { 
                        mVelocity = 0;
                        UpdateVelocity = false;
                    }
                }
                else UpdateVelocity = false;
            }
            else
            {
                float dr = DecayRate;
                if (mVelocity < 0)
                {
                    if (OutBack())
                    {
                        dr *= 0.9f;
                    }
                }
                else
                {
                    if (OutForward())
                    {
                        dr *= 0.9f;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    y += mVelocity;
                    mVelocity *= dr;
                }
                if (mVelocity < 0.01f & mVelocity > -0.01f)
                {
                    mVelocity = 0;
                }
            }
            if (y != 0)
                Move(y);
        }
    }
}
