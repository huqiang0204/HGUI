using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Data
{
    /// <summary>
    /// DataBuffer存储的预制体初始化器
    /// </summary>
    public abstract class Initializer
    {
        protected struct ContextAction
        {
            public Action<Transform> CallBack;
            public int InsID;
        }
        protected struct ContextObject
        {
            public Transform Ins;
            public int InsID;
        }
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
        public virtual void Done()
        {
            int c = contexts.Count;
            int m = objects.Count;
            for (int i = 0; i < c; i++)
            {
                var act = contexts[i].CallBack;
                int id = contexts[i].InsID;
                if(act!=null)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if(objects[j].InsID==id)
                        {
                            act(objects[j].Ins);
                            break;
                        }
                    }
                }
            }
            objects.Clear();
            contexts.Clear();
        }
        /// <summary>
        /// 添加联系上下文
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="insID"></param>
        public void AddContext(Transform trans, int insID)
        {
            ContextObject co = new ContextObject();
            co.Ins = trans;
            co.InsID = insID;
            objects.Add(co);
        }
        protected List<ContextObject> objects = new List<ContextObject>();
        protected List<ContextAction> contexts = new List<ContextAction>();
        public void AddContextAction(Action<Transform> action, int insID)
        {
            ContextAction ca = new ContextAction();
            ca.CallBack = action;
            ca.InsID = insID;
            contexts.Add(ca);
        }
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
