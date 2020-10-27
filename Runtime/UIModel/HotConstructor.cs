using System;
using System.Linq;
using System.Text;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;

namespace huqiang.UIModel
{
    /// <summary>
    /// 用于热更新滚动框的构造体
    /// </summary>
    /// <typeparam name="T">UI模型</typeparam>
    /// <typeparam name="U">数据模型</typeparam>
    public class HotConstructor<T, U> where T : class, new()
    {
        /// <summary>
        /// 用于热更新滚动框的中间件实体
        /// </summary>
        public HotMiddleware middle;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">项更新委托</param>
        public HotConstructor(Action<T, U, int> action)
        {
            middle = new HotMiddleware();
            middle.Context = this;
            middle.initializer = new UIInitializer(UIBase.ObjectFields(typeof(T)));
            middle.creator = Create;
            middle.caller = Call;
            Invoke = action;
        }
        U u;
        /// <summary>
        /// 项更新委托
        /// </summary>
        public Action<T, U, int> Invoke;
        /// <summary>
        /// 创建一个UI实体
        /// </summary>
        /// <returns></returns>
        public object Create()
        {
            var t = new T();
            middle.initializer.Reset(t);
            return t;
        }
        /// <summary>
        /// 更新项
        /// </summary>
        /// <param name="obj">UI实体</param>
        /// <param name="dat">数据实体</param>
        /// <param name="index">索引</param>
        public void Call(object obj, object dat, int index)
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
                try
                {
                    Invoke(obj as T, u, index);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.StackTrace);
                }
            }
        }
    }
    /// <summary>
    /// 用于热更新树形框的构造体
    /// </summary>
    /// <typeparam name="T">UI模型</typeparam>
    /// <typeparam name="U">数据模型</typeparam>
    public class HotTVConstructor<T, U> where T : TreeViewItem, new() where U : TreeViewNode, new()
    {
        /// <summary>
        /// 用于热更新树形框的中间件实体
        /// </summary>
        public HotTVMiddleware middle;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">项更新委托</param>
        public HotTVConstructor(Action<T, U> action)
        {
            middle = new HotTVMiddleware();
            middle.Context = this;
            middle.initializer = new UIInitializer(UIBase.ObjectFields(typeof(T)));
            middle.creator = Create;
            middle.caller = Call;
            Invoke = action;
        }
        /// <summary>
        /// 项更新委托
        /// </summary>
        public Action<T, U> Invoke;
        /// <summary>
        /// 创建一个UI实体
        /// </summary>
        /// <returns></returns>
        public TreeViewItem Create()
        {
            var t = new T();
            middle.initializer.Reset(t);
            return t;
        }
        /// <summary>
        /// 更新项
        /// </summary>
        /// <param name="obj">UI实体</param>
        /// <param name="dat">数据实体</param>
        public void Call(TreeViewItem obj, TreeViewNode dat)
        {
            if (Invoke != null)
            {
                try
                {
                    Invoke(obj as T, dat as U);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.StackTrace);
                }
            }
        }
    }
}
