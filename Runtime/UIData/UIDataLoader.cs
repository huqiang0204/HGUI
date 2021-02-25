using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.UIData
{

    public class UIDataLoader
    {
        /// <summary>
        /// 游戏对象管理缓存器
        /// </summary>
        public UIelementBuffer uiBuffer;
        /// <summary>
        /// 初始化器
        /// </summary>
        public Initializer initializer;
        /// <summary>
        /// 将游戏对象数据写入假结构体
        /// </summary>
        /// <param name="com">unity组件</param>
        /// <param name="buffer">DataBuffer</param>
        /// <returns></returns>
        public virtual FakeStruct SaveUI(Component com, DataBuffer buffer) { return null; }
        /// <summary>
        /// 将游戏对象数据写入假结构体
        /// </summary>
        /// <param name="com">unity组件</param>
        /// <param name="buffer">DataBuffer</param>
        /// <returns></returns>
        public virtual void LoadUI(UIElement com, FakeStruct fake, UIInitializer initializer) { }
    }
}
