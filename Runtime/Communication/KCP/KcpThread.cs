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
        QueueBuffer<Mission> queue = new QueueBuffer<Mission>();
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
                    int c = queue.Count;
                    for (int i = 0; i < c; i++)
                    {
                        var mis = queue.Dequeue();
                        if (mis != null)
                            if (mis.action != null)
                                mis.action(mis.data);
                    }
                }
                catch //(Exception ex)
                {
                    //ServerLog.Error(ex.StackTrace);
                }
                long t = DateTime.Now.Ticks;
                t -= now;
                t /= 10000;
                if (t < 10)
                    Thread.Sleep(1);
            }
        }
        public void AddMission(Action<object> action, object obj)
        {
            Mission mis = new Mission();
            mis.action = action;
            mis.data = obj;
            queue.Enqueue(mis);
        }
        public void SendAll(Kcp kcp, Int16 time)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
                if (l != null)
                {
                    kcp.SendMsg(l, time);
                }
            }
        }
        public void SendAll(Socket soc, Kcp kcp, Int16 time, byte[] heart)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
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
            for (int i = 0; i < top; i++)
            {
                var link = buffer[i];
                if (now - link.RecvTime > KcpListener.TimeOut)
                    if (link.Disconnect())
                    {
                        link.RecyclingTime = now;
                        kcp.PreRecycling(link);
                        buffer[i] = null;
                        top--;
                        buffer[i] = buffer[top];
                        buffer[top] = null;
                        if (top > 0)
                            buffer[i].Index = i;
                        i--;
                    }
            }
        }
    }
}
