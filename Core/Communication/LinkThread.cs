using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    public class LinkBuffer<T> where T : NetworkLink, new()
    {
        protected T[] buffer;
        protected int max;
        protected int top;
        public bool running;
        public LinkBuffer(int size = 2048)
        {
            buffer = new T[size];
            max = size;
        }
        public T Find(int ip,int port)
        {
            for (int i = 0; i < top; i++)
            {
                var link = buffer[i];
                if (link != null)
                    if (link.ip == ip)
                        if (link.port == port)
                            return link;
            }
            return null;
        }
        public T Find(Int64 id)
        {
            for (int i = 0; i < top; i++)
            {
                var link = buffer[i];
                if (link != null)
                    if (link.id == id)
                        return link;
            }
            return null;
        }
        public void Add(T link)
        {
            link.Index = top;
            buffer[top] = link;
            top++;
        }
        public void Delete(KcpLink link)
        {
            int index = link.Index;
            buffer[index] = null;
            top--;
            buffer[index] = buffer[top];
            buffer[top] = null;
            if (top > 0)
                buffer[index].Index = index;
        }
        public int Count { get { return top; } }
        public T this[int index]
        {
            get { return buffer[index]; }
        }
        public void SendAll(Socket soc, byte[][] data)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
                if (l != null)
                    for (int j = 0; j < data.Length; j++)
                        soc.SendTo(data[j], l.endpPoint);
            }
        }
        public void SendAll(Socket soc, long now)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
                if (l != null)
                {
                    l.Send(soc, now);
                }
            }
        }
        public void AddMsg(byte[][] dat, long now,UInt16 msgID)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
                if (l != null)
                {
                    l.AddMsg(dat, now,msgID);
                }
            }
        }
        public void Recive()
        {
            var now = DateTime.Now.Ticks;
            for (int i = 0; i < top; i++)
            {
                var c = buffer[i];
                if (c != null)
                {
                    try
                    {
                        c.Recive(now);
                    }catch
                    {
                    }
                }
            }
        }
    }
    public class LinkThread<T> : LinkBuffer<T> where T :NetworkLink, new()
    {
        class Mission
        {
            public Action<object> action;
            public object data;
        }
        public int Id;
        Thread thread;
        public Socket soc;
        QueueBuffer<Mission> queue = new QueueBuffer<Mission>();
        public LinkThread(int size =2048):base (size)
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
                    for(int i=0;i<c;i++)
                    {
                       var mis =  queue.Dequeue();
                        if (mis != null)
                            if (mis.action != null)
                                mis.action(mis.data);
                    }
                }
                catch (Exception ex)
                {
                    
                }
                try
                {
                    if (soc != null)
                        SendAll(soc, now);
                }
                catch
                {

                }
                long t = DateTime.Now.Ticks;
                t -= now;
                t /= 10000;
                if (t < 10)
                    Thread.Sleep(1);
            }
        }
        public void AddMission(Action<object> action,object obj)
        {
            Mission mis = new Mission();
            mis.action = action;
            mis.data = obj;
            queue.Enqueue(mis);
        }
    }
}
