using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Data
{
    /// <summary>
    /// DataBuffer存储的预制体初始化器
    /// </summary>
    public abstract class Initializer
    {
        /// <summary>
        /// 当预制体创建好时调用此函数
        /// </summary>
        /// <param name="fake"></param>
        /// <param name="com"></param>
        public virtual void Initialiezd(FakeStruct fake, Component com)
        {
        }
        /// <summary>
        /// 当再次初始化时,先调用此函数重置初始化器
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Reset(object obj) { }
        /// <summary>
        /// 初始化完毕
        /// </summary>
        public virtual void Done() { }
    }
    /// <summary>
    /// 数据对象载入器
    /// </summary>
    public abstract class DataLoader
    {
        /// <summary>
        /// 游戏对象管理缓存器
        /// </summary>
        public GameobjectBuffer gameobjectBuffer;
        /// <summary>
        /// 载入组件数据
        /// </summary>
        /// <param name="fake">假结构体</param>
        /// <param name="com">unity组件</param>
        /// <param name="main"></param>
        public virtual void LoadToComponent(FakeStruct fake, Component com, FakeStruct main) { }
        /// <summary>
        /// 载入游戏对象数据
        /// </summary>
        /// <param name="fake">假结构体</param>
        /// <param name="com">unity组件</param>
        /// <param name="initializer"></param>
        public virtual void LoadToObject(FakeStruct fake, Component com, Initializer initializer) { }
        /// <summary>
        /// 将游戏对象数据写入假结构体
        /// </summary>
        /// <param name="com">unity组件</param>
        /// <param name="buffer">DataBuffer</param>
        /// <returns></returns>
        public virtual FakeStruct LoadFromObject(Component com, DataBuffer buffer) { return null; }
    }
}
