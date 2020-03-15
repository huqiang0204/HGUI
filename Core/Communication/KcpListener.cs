using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    public class KcpListener
    {
        public static KcpListener Instance;
        public Socket soc;
        public Thread thread;
        protected bool running;
        int _port;
        public int Port { get { return _port; } }
        public KcpEnvelope envelope = new KcpEnvelope();
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
            if (thread == null)
            {
                //创建消息接收线程
                thread = new Thread(Run);
                thread.Start();
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
                        byte[] dat = new byte[len];
                        for (int i = 0; i < len; i++)
                            dat[i] = buffer[i];
                        Dispatch(dat, end as IPEndPoint);
                    }
                }
                catch (Exception ex)
                {
                   
                }
            }
        }
        public virtual void Dispose()
        {
            running = false;
            thread = null;
            soc.Close();
        }
        public virtual void Dispatch(byte[] dat, IPEndPoint endPoint)
        {
        }
        public void Send(byte[] data, byte type, IPEndPoint ip)
        {
            var ss = envelope.Pack(data, type);
            for (int i = 0; i < ss.Length; i++)
                soc.SendTo(ss[i],  ip);
        }
        public void Send(byte[] data, IPEndPoint ip)
        {
            soc.SendTo(data,  ip);
        }
        public virtual void RemoveLink(KcpLink link)
        {
        }
        public virtual void Broadcast(byte[] dat,byte type)
        {
        }
    }
}
