using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 滚动框项目
    /// </summary>
    public class ScrollItem
    {
        /// <summary>
        /// 当前绑定数据索引
        /// </summary>
        public int index = -1;
        /// <summary>
        /// 主体坐标变换
        /// </summary>
        public Transform target;
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object datacontext;
        /// <summary>
        /// 附加对象
        /// </summary>
        public object obj;
    }
}
