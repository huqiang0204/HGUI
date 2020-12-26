using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;
using System;

namespace huqiang.UIModel
{
    /// <summary>
    /// 用于热更新对话框的连接器
    /// </summary>
    /// <typeparam name="T">UI模型</typeparam>
    /// <typeparam name="U">数据模型</typeparam>
    public class HotUILinker<T, U> where T : class, new() where U : class, new()
    {
        /// <summary>
        /// 用于热更新对话框的对象连接器
        /// </summary>
        public ObjectLinker middle;
        /// <summary>
        /// 项更新委托
        /// </summary>
        public Action<T, U, int> ItemUpdate;
        /// <summary>
        /// 计算内容高度的委托
        /// </summary>
        public Func<UIElement, U, float> LayoutCallback;
        UIInitializer initializer;
        FakeStruct model;
        UIContainer con;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container">容器实例</param>
        /// <param name="mod">UI模型名称</param>
        public HotUILinker(UIContainer container, UIElement mod)
        {
            con = container;
            model = mod.mod;
            middle = new ObjectLinker(container,mod);
            middle.ItemUpdate = MItemUpdate;
            middle.LayoutCallback = MCalculItemHigh;
            middle.ItemCreate = MItemCreate;
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        void MItemUpdate(object obj, object dat, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(obj as T, dat as U, index);
        }
        float MCalculItemHigh(UIElement uI, object dat)
        {
            if (LayoutCallback != null)
                return LayoutCallback(uI, dat as U);
            return 0;
        }
        void MItemCreate(ObjectLinker obj, LinkerMod mod)
        {
            T t = new T();
            initializer.Reset(t);
            mod.main = HGUIManager.Clone(model, initializer);
            mod.UI = t;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dat">数据实例</param>
        public void InsertData(U dat)
        {
            con.InsertData(middle, dat);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="dat">数据实例</param>
        public void AddData(U dat)
        {
            con.AddData(middle, dat);
        }
        /// <summary>
        /// 添加一条数据,并向下滚动当前此数据高度
        /// </summary>
        /// <param name="dat">数据实例</param>
        /// <param name="h">数据高度</param>
        public void AddAndMove(U dat, float h)
        {
            if (h <= 0)
                h = 60;
            con.AddAndMove(middle, dat, h);
        }
        /// <summary>
        /// 插入一条数据,并滚动到底部
        /// </summary>
        /// <param name="dat">数据实例</param>
        /// <param name="h">数据高度</param>
        public void AddAndMoveEnd(U dat, float h)
        {
            if (h <= 0)
                h = 60;
            con.AddAndMoveEnd(middle, dat, h);
        }
    }
}

