using huqiang.Core.HGUI;
using huqiang.Data;
using System;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 复合型UI组件基类
    /// </summary>
    public abstract class Composite
    {
        /// <summary>
        /// 数据缓存
        /// </summary>
        public FakeStruct BufferData;
        /// <summary>
        /// UI实体
        /// </summary>
        public UIElement Enity;
        /// <summary>
        /// 当前更新帧,防止每帧重复更新
        /// </summary>
        public int Frame;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mod">模型数据</param>
        /// <param name="element">UI元素实例</param>
        public virtual void Initial(FakeStruct mod, UIElement element)
        {
            BufferData = mod;
            Enity = element;
            Enity.composite = this;
        }
        /// <summary>
        /// 更新函数
        /// </summary>
        /// <param name="time"></param>
        public virtual void Update(float time)
        {
        }
    }
}
