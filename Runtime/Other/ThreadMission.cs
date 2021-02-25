using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace huqiang
{
    /// <summary>
    /// 线程任务管理器
    /// </summary>
    public class ThreadMission
    {
        class Mission
        {
            public Action<object> action;
            public object data;
            public Action<object> waitAction;
            public int Id;
        }
        QueueBuffer<Mission> SubMission;
        QueueBuffer<Mission> MainMission;
#if UNITY_WSA
        System.Threading.Tasks.Task thread;
#else
         Thread thread;
#endif
        public string Tag;
        public int Id { get; private set; }
        AutoResetEvent are;
        bool run;
        bool subFree,mainFree;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">标志</param>
        public ThreadMission(string tag)
        {
            Tag = tag;
            SubMission = new QueueBuffer<Mission>();
            MainMission = new QueueBuffer<Mission>();
#if UNITY_WSA
            thread = System.Threading.Tasks.Task.Run(Run);
            are = new AutoResetEvent(false);
            run = true;
            thread.Start();
            Id = thread.Id;
#else
            thread = new Thread(Run);
            are = new AutoResetEvent(false);
            run = true;
            thread.Start();
            Id = thread.ManagedThreadId;
#endif

            threads.Add(this);
        }
        void Run()
        {
            while (run)
            {
                try
                {
                    var m = SubMission.Dequeue();
                    if (m == null)
                    {
                        subFree = true;
                        are.WaitOne(1);
                    }
                    else
                    {
                        subFree = false;
                        try
                        {
                            m.action(m.data);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.LogError(ex.StackTrace);
#endif
                        }
                        if (m.waitAction != null)//如果有等待的任务
                        {
                            if (m.Id == MainID)//交给主线程
                            {
                                m.action = m.waitAction;
                                m.waitAction = null;
                                MainMission.Enqueue(m);
                                goto label;
                            }
                            for (int i = 0; i < threads.Count; i++)
                                if (m.Id == threads[i].Id)
                                {
                                    threads[i].AddMainMission(m.waitAction, m.data);//任务交给源线程
                                    goto label;
                                }
                            m.action = m.waitAction;
                            m.waitAction = null;
                            MainMission.Enqueue(m);//否则交给主线程
                        }
                    label:;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.LogError(ex.StackTrace);
#endif
                }
            }
            are.Dispose();
        }
        /// <summary>
        /// 添加一个其它线程的任务
        /// </summary>
        /// <param name="action">委托任务</param>
        /// <param name="dat">联系上下文</param>
        /// <param name="wait">执行完毕后的回调</param>
        public void AddSubMission(Action<object> action, object dat,Action<object> wait = null)
        {
            Mission mission = new Mission();
            mission.action = action;
            mission.data = dat;
            mission.waitAction = wait;
            mission.Id = Thread.CurrentThread.ManagedThreadId;
            SubMission.Enqueue(mission);
        }
        /// <summary>
        /// 其它线程向本线程添加任务
        /// </summary>
        /// <param name="action">委托任务</param>
        /// <param name="dat">联系上下文</param>
        /// <param name="wait">执行完毕后的回调</param>
        public void AddMainMission(Action<object> action, object dat, Action<object> wait = null)
        {
            Mission mission = new Mission();
            mission.action = action;
            mission.data = dat;
            mission.waitAction = wait;
            MainMission.Enqueue(mission);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            run = false;
            threads.Remove(this);
        }
        static List<ThreadMission>threads = new List<ThreadMission>();
        /// <summary>
        /// 添加一个委托任务
        /// </summary>
        /// <param name="action">委托任务</param>
        /// <param name="dat">联系上下文</param>
        /// <param name="tag">目标线程标志</param>
        /// <param name="wait">执行完毕后的回调</param>
        public static void AddMission(Action<object> action, object dat, string tag = null, Action<object> wait = null)
        {
            if (threads == null)
            {
                return;
            }
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].Tag == tag)
                {
                    threads[i].AddSubMission(action, dat, wait);
                    return;
                }
            }
            var mis = new ThreadMission(tag);
            mis.AddSubMission(action, dat, wait);
            threads.Add(mis);
        }
        /// <summary>
        /// 给主线程添加一个任务
        /// </summary>
        /// <param name="action">委托任务</param>
        /// <param name="dat">联系上下文</param>
        /// <param name="wait">执行完毕后的回调</param>
        public static void InvokeToMain(Action<object> action, object dat, Action<object> wait=null)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            if(id==MainID)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].Tag == null)
                    {
                        threads[i].AddMainMission(action, dat, wait);
                        return;
                    }
                }
                var mis = new ThreadMission(null);
                mis.AddMainMission(action, dat, wait);
                threads.Add(mis);
            }
            else
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].Id == id)
                    {
                        threads[i].AddMainMission(action, dat, wait);
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 执行主线程任务
        /// </summary>
        public static void ExtcuteMain()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                ExtcuteMain(threads[i]);
            }
        }
        static void ExtcuteMain(ThreadMission mission)
        {
            for (int j = 0; j < 2048; j++)
            {
                var m = mission.MainMission.Dequeue();
                if (m == null)
                {
                    mission.mainFree = true;
                    break;
                }
                else
                {
                    mission.mainFree = false;
                    m.action(m.data);
                    if (m.waitAction != null)
                        mission.AddSubMission(m.waitAction, m.data);
                }
            }
        }
        /// <summary>
        /// 释放所有线程
        /// </summary>
        public static void DisposeAll()
        {
            for (int i = 0; i < threads.Count; i++)
                threads[i].Dispose();
            threads.Clear();
        }
        /// <summary>
        /// 释放没有任务的线程
        /// </summary>
        public static void DisposeFree()
        {
            int c = threads.Count-1;
            lock (threads)
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].subFree & threads[i].mainFree)
                    {
                        threads[i].Dispose();
                        threads.RemoveAt(i);
                    }
                }
        }
        static int MainID;
        /// <summary>
        /// 设置主线程id
        /// </summary>
        public static void SetMianId()
        {
            MainID = Thread.CurrentThread.ManagedThreadId;
        }
        /// <summary>
        /// 通过标志查询线程
        /// </summary>
        /// <param name="name">标志名</param>
        /// <returns></returns>
        public static ThreadMission FindMission(string name)
        {
            for (int i = 0; i < threads.Count; i++)
                if (threads[i].Tag == name)
                    return threads[i];
            return null;
        }
        /// <summary>
        /// 创建一个线程
        /// </summary>
        /// <param name="tag">标志名</param>
        /// <returns></returns>
        public static ThreadMission CreateMission(string tag)
        {
            for(int i=0;i<threads.Count;i++)
            {
                if (threads[i].Tag == tag)
                    return threads[i];
            }
            return new ThreadMission(tag);
        }
    }
    struct MissionContent
    {
        public int State;
        public int ID;
        public Action<object> Invoke;
        public Action<object> CallBack;
        public object Obj;
    }
    class MissionCache
    {
        MissionContent[] mcs;
        int length;
        public MissionCache(int len = 32)
        {
            mcs = new MissionContent[len];
            length = len;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invoke"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="spin">自旋验证次数</param>
        /// <returns></returns>
        public bool Post(int id, Action<object> invoke, object obj, Action<object> callback = null, int spin = 16)
        {
            for (int i = 0; i < length; i++)
            {
                if (mcs[i].State == 0)
                {
                    mcs[i].State = 1;
                    mcs[i].ID = id;
                    for (int j = 0; j < spin; j++)
                        if (mcs[i].ID != id)
                            goto label;
                    mcs[i].Invoke = invoke;
                    mcs[i].CallBack = callback;
                    mcs[i].Obj = obj;
                    mcs[i].State = 2;
                    return true;
                }
            label:;
            }
            return false;
        }
        /// <summary>
        /// 强制行投递委托，如果缓存被沾满
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invoke"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        public void ForcePost(int id, Action<object> invoke, object obj, Action<object> callback = null)
        {
            var tick = DateTime.Now.Ticks;
            while (true)
            {
                if (Post(id, invoke, obj, callback))
                    return;
                if (DateTime.Now.Ticks - tick > 100000)//超过10毫秒都无法添加任务
                {
                    Debug.LogError("缓存已满，目标线程超过10毫秒都没有处理缓存中的任务");
                    return; 
                }
            }
        }
        public bool Get(ref MissionContent content)
        {
            for (int i = 0; i < length; i++)
            {
                if (mcs[i].State == 2)
                {
                    content = mcs[i];
                    mcs[i].Invoke = null;
                    mcs[i].CallBack = null;
                    mcs[i].Obj = null;
                    mcs[i].State = 0;
                    return true;
                }
            }
            return false;
        }
        public void Clear()
        {
            for (int i = 0; i < length; i++)
            {
                if (mcs[i].State == 2)
                {
                    mcs[i].Invoke = null;
                    mcs[i].CallBack = null;
                    mcs[i].Obj = null;
                    mcs[i].State = 0;
                }
            }
        }
    }
}