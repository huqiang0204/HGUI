using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    /// <summary>
    /// KCP监听器
    /// </summary>
    public class KcpListener
    {
        /// <summary>
        /// 心跳包
        /// </summary>
        public static byte[] Heart = new byte[] { 255, 255, 255, 255, 0, 255, 255, 255, 254 };
        /// <summary>
        /// 连接超时时间
        /// </summary>
        public static long TimeOut = 50000000;//5*1000*10000
        /// <summary>
        /// 实例
        /// </summary>
        public static KcpListener Instance;

        public Socket soc;
        /// <summary>
        /// 处理接收到的消息的线程
        /// </summary>
        public Thread recvThread;
        /// <summary>
        /// 处理将要发送的消息的线程
        /// </summary>
        public Thread sendThread;
        /// <summary>
        /// 运行状态
        /// </summary>
        protected bool running;
        int _port;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get { return _port; } }
        public KcpListener(int port = 0)
        {
            _port = port;
        }
        /// <summary>
        /// 启动监听服务
        /// </summary>
        public void Start()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //new UdpClient(_port);//new IPEndPoint(IPAddress.Parse(ip),
            soc.Bind(ip);
            if (_port == 0)
                _port = (soc.LocalEndPoint as IPEndPoint).Port;
            soc.ReceiveTimeout = 1000;
            running = true;
            if (recvThread == null)
            {
                //创建消息接收线程
                recvThread = new Thread(Run);
                recvThread.Start();
                are = new AutoResetEvent(false);
                sendThread = new Thread(Sending);
                sendThread.Start();
            }
        }
        void Run()
        {
            byte[] buffer = new byte[65536];
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            while (running)
            {
                try
                {
                    ManageLinks();
                    DisposeRecyclingLinks();
                    EndPoint end = ip;
                    int len = 0;
                    try
                    {
                        len = soc.ReceiveFrom(buffer, ref end);//接收数据报
                    }
                    catch {
                    } 
                    if(len>0)
                    {
                        Dispatch(buffer,len, end as IPEndPoint);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }
        AutoResetEvent are;
        /// <summary>
        /// sendThread
        /// </summary>
        void Sending()
        {
            while(running)
            {
                are.WaitOne(1);
                SendAll();
            }
        }
        /// <summary>
        /// 发送所有线程缓存中的消息
        /// </summary>
        public virtual void SendAll()
        {
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            running = false;
            recvThread = null;
            soc.Close();
        }
        /// <summary>
        /// 处理监听到的消息
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="len"></param>
        /// <param name="endPoint"></param>
        public virtual void Dispatch(byte[] dat,int len, IPEndPoint endPoint)
        {
        }
        /// <summary>
        /// 向某个地址发送原始消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ip"></param>
        public void Send(byte[] data, IPEndPoint ip)
        {
            soc.SendTo(data,  ip);
        }
        /// <summary>
        /// 移除某个用户连接
        /// </summary>
        /// <param name="link"></param>
        public virtual void RemoveLink(NetworkLink link)
        {
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="type"></param>
        public virtual void Broadcast(byte[] dat,byte type)
        {
        }
        /// <summary>
        /// 管理用户连接
        /// </summary>
        protected virtual void ManageLinks()
        {
        }
        List<NetworkLink> RecyclingLinks = new List<NetworkLink>();
        ///protected List<NetworkLink> 
        /// <summary>
        /// recvThread 防止其它线程未操作完成,这里先存储起来,延迟释放
        /// </summary>
        /// <param name="link"></param>
        public virtual void PreRecycling(NetworkLink link)
        {
            RecyclingLinks.Add(link);
        }
        /// <summary>
        /// recvThread 释放链接
        /// </summary>
        /// <param name="time"></param>
        void DisposeRecyclingLinks()
        {
            var time = DateTime.Now.Ticks;
            for (int i = 0; i < RecyclingLinks.Count; i++)
            {
                var link = RecyclingLinks[i];
                if (time - link.RecyclingTime > 300000)//30毫秒
                {
                    link.FreeMemory();
                    RecyclingLinks.RemoveAt(i);
                    i--;
                    link.Dispose();
                }
                else break;
            }
        }
    }
}
