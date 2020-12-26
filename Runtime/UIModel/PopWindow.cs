using System;
using UnityEngine;

namespace huqiang.UIModel
{
    /// <summary>
    /// 弹出式窗口,位于Page和Menu中间
    /// </summary>
    public class PopWindow : UIBase
    {
        public Func<bool> Back { get; set; }
        /// <summary>
        /// 联系上下文的基础UI
        /// </summary>
        public UIBase UIContext;
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="obj">附加内容</param>
        public virtual void Show(object obj = null)
        {
            if (Main != null)
                Main.activeSelf = true;
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="context">联系上下文的基础UI</param>
        /// <param name="obj">附加内容</param>
        public virtual void Show(UIBase context, object obj = null)
        {
            UIContext = context;
            if (Main != null)
                Main.activeSelf = true;
        }
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public virtual void Hide()
        {
            if (Main != null)
                Main.activeSelf = false;
        }
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="dat">数据</param>
        /// <returns>成功返回真</returns>
        public virtual bool Handling(string cmd, object dat)
        {
            return false;
        }
    }
}

