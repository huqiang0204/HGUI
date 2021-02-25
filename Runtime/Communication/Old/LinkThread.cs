using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    /// <summary>
    /// 线程缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadBuffer<T> where T : NetworkLink, new()
    {
        public bool running;
        /// <summary>
        /// 内容缓存
        /// </summary>
        protected DisorderlyQueue<T> queue;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size">缓存大小,容纳用户最大的连接数</param>
        public ThreadBuffer(int size = 2048)
        {
            queue = new DisorderlyQueue<T>(size);
        }
        /// <summary>
        /// 使用ip地址查询某个用户
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public T Find(int ip,int port)
        {
            return queue.Find((o) => {
                if (o == null)
                    return false;
                return o.ip == ip & o.port == port;
            });
        }
        /// <summary>
        /// 使用id查询某个用户
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public T Find(Int64 id)
        {
            return queue.Find((o)=> {
                if (o == null)
                    return false;
                return o.id==id;
            });
        }
        /// <summary>
        /// 添加一个用户连接
        /// </summary>
        /// <param name="link"></param>
        public void Add(T link)
        {
            queue.Add(link);
        }
        /// <summary>
        /// 移除某个用户连接
        /// </summary>
        /// <param name="link"></param>
        public void Delete(NetworkLink link)
        {
            queue.Remove(link as T);
        }
        /// <summary>
        /// 当前连接数
        /// </summary>
        public int Count { get { return queue.Count; } }
        public T this[int index]
        {
            get { return queue[index]; }
        }
        /// <summary>
        /// 个欸所有连接发送消息
        /// </summary>
        /// <param name="soc">Socket</param>
        /// <param name="data">数据</param>
        public void SendAll(Socket soc, byte[][] data)
        {
            int c = queue.Count;
            for (int i = 0; i < c; i++)
            {
                var l = queue[i];
                if (l != null)
                    for (int j = 0; j < data.Length; j++)
                        soc.SendTo(data[j], l.endpPoint);
            }
        }
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        public void Recive()
        {
            var now = DateTime.Now.Ticks;
            int c = queue.Count;
            for (int i = 0; i < c; i++)
            {
                var r = queue[i];
                if (r != null)
                {
                    try
                    {
                        r.Recive(now);
                    }catch
                    {
                    }
                }
            }
        }
    }
    /// <summary>
    /// 用户连接线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkThread<T> : ThreadBuffer<T> where T :NetworkLink, new()
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
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size">缓存大小,容纳用户最大的连接数</param>
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
        public void AddMission(Action<object> action,object obj)
        {
            Mission mis = new Mission();
            mis.action = action;
            mis.data = obj;
            queue.Enqueue(mis);
        }
    }
}
