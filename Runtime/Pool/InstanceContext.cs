using System;
using UnityEngine;

namespace huqiang.Pool
{
    /// <summary>
    /// 实例联系上下文
    /// </summary>
    public class InstanceContext
    {
        /// <summary>
        /// 游戏对象实例
        /// </summary>
        public GameObject Instance;
        /// <summary>
        /// 实例id
        /// </summary>
        public int Id;
        /// <summary>
        /// 对象类型, 所有组件的位或值
        /// </summary>
        public Int64 Type;
        /// <summary>
        /// 模型类型对应的缓存
        /// </summary>
        public ModelBuffer buffer;
        /// <summary>
        /// 缓存中的位置索引
        /// </summary>
        public int Index;
    }
}
