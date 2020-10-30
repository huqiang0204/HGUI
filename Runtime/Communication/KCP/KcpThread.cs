using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    public class KcpThread<T> : ThreadBuffer<T> where T : KcpData, new()
    {
        class Mission
        {
            public Action<object> action;
            public object data;
        }
        public int Id;
        /// <summary>
        /// 本线程任务只负责派发解包后的消息与逻辑，用户的添加和释放由kcpserver的recvThread线程统一管理
        /// 消息的解压包由kcpserver的sendThread线程统一管理
        /// </summary>
        Thread thread;
        QueueBuffer<Mission> missions = new QueueBuffer<Mission>();
        /// <summary>
        /// kcp线程用户管理类
        /// </summary>
        /// <param name="size">缓存大小,可容纳的最大用户数</param>
        public KcpThread(int size = 2048) : base(size)
        {
            running = true;
            thread = new Thread(Run);
            Id = thread.ManagedThreadId;
            thread.Start();
        }
        void Run()
        {
            while (running)
            {
                var now = DateTime.Now.Ticks;
                try
                {
                    Recive();
                    int c = missions.Count;
                    for (int i = 0; i < c; i++)
                    {
                        var mis = missions.Dequeue();
                        if (mis != null)
                            if (mis.action != null)
                                mis.action(mis.data);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
                long t = DateTime.Now.Ticks;
                t -= now;
                t /= 10000;
                if (t < 10)
                    Thread.Sleep(1);
            }
        }
        /// <summary>
        /// 添加其它线程委托的任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public void AddMission(Action<object> action, object obj)
        {
            Mission mis = new Mission();
            mis.action = action;
            mis.data = obj;
            missions.Enqueue(mis);
        }
        /// <summary>
        /// 发送缓存中的消息
        /// </summary>
        /// <param name="kcp">kcp封包器</param>
        /// <param name="time">时间</param>
        public void SendAll(Kcp kcp, Int16 time)
        {
            int c = queue.Count;
            for (int i = 0; i < c; i++)
            {
                var l = queue[i];
                if (l != null)
                {
                    kcp.SendMsg(l, time);
                }
            }
        }
        /// <summary>
        /// 发送缓存中的消息,如果没有则发送一条外部消息,防止超时
        /// </summary>
        /// <param name="soc">socket服务</param>
        /// <param name="kcp">kcp封包器</param>
        /// <param name="time">时间</param>
        /// <param name="heart">心跳包</param>
        public void SendAll(Socket soc, Kcp kcp, Int16 time, byte[] heart)
        {
            int c = queue.Count;
            for (int i = 0; i < c; i++)
            {
                var l = queue[i];
                if (l != null)
                {
                    if (kcp.SendMsg(l, time) == 0)
                    {
                        soc.SendTo(heart, l.endpPoint);
                    }
                }
            }
        }
        /// <summary>
        /// 由kcpserver的recvThread线程管理
        /// </summary>
        public void DeleteTimeOutLink(KcpListener kcp, long now)
        {
            int c = queue.Count;
            for (int i = c; i >= 1; i--)
            {
                var link = queue[i];
                if (now - link.RecvTime > KcpListener.TimeOut)
                    if (link.Disconnect())
                    {
                        link.RecyclingTime = now;
                        kcp.PreRecycling(link);
                        queue.RemoveAt(i);
                    }
            }
        }
    }
}
