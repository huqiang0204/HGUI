using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang.Communication
{
    public class KcpListener
    {
        public static byte[] Heart = new byte[] { 255, 255, 255, 255, 0, 255, 255, 255, 254 };
        public static long TimeOut = 50000000;//5*1000*10000
        public static KcpListener Instance;
        public Socket soc;
        public Thread recvThread;
        public Thread sendThread;
        protected bool running;
        int _port;
        public int Port { get { return _port; } }
        public KcpListener(int port = 0)
        {
            _port = port;
        }
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
                ProBroadcast();
                SendAll();
            }
        }
        /// <summary>
        /// sendThread
        /// </summary>
        public virtual void SendAll()
        {
        }
        public virtual void Dispose()
        {
            running = false;
            recvThread = null;
            soc.Close();
        }
        /// <summary>
        /// recvThread
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="len"></param>
        /// <param name="endPoint"></param>
        public virtual void Dispatch(byte[] dat,int len, IPEndPoint endPoint)
        {
        }
        public void Send(byte[] data, IPEndPoint ip)
        {
            soc.SendTo(data,  ip);
        }
        public virtual void RemoveLink(NetworkLink link)
        {
        }
        /// <summary>
        /// 投递广播消息
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="type">数据类型</param>
        /// <param name="spin">验证自旋周期，建议值为16</param>
        /// <returns></returns>
        public virtual bool PostBroadcast(byte[] dat,byte type, int spin)
        {
            return false;
        }
        /// <summary>
        /// 处理广播消息
        /// </summary>
        protected virtual void ProBroadcast()
        {

        }
        public virtual BroadcastMsg FindBroadcastMsg(int msgID)
        {
            return null;
        }
        public virtual void UpdateBroadcastMsg(int msgID)
        {
        }
        /// <summary>
        /// recvThread 
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
