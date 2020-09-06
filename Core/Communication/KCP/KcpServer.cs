using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang
{
    public class KcpServer<T> : KcpListener where T : KcpLink, new()
    {

        static Random random = new Random();
        public static int SingleCount = 2048;
        public KcpThread<T>[] linkBuff;
        int tCount = 0;
        Kcp kcp;
        public KcpServer(int port = 0) : base(port)
        {
            Instance = this;
            kcp = new Kcp();
        }
        public void Run(int threadCount = 8, int threadbuff = 2048)
        {
            tCount = threadCount;
            if (tCount > 0)
            {
                linkBuff = new KcpThread<T>[threadCount];
                for (int i = 0; i < threadCount; i++)
                {
                    linkBuff[i] = new KcpThread<T>(threadbuff);
                }
            }
            else
            {
                tCount = 1;
                linkBuff = new KcpThread<T>[1];
                linkBuff[0] = new KcpThread<T>();
            }
            kcp.Run(this, tCount);
            Start();
        }
        public void Close()
        {
            soc.Close();
        }
        //设置用户的udp对象用于发送消息
        public T FindOrCreateLink(IPEndPoint ep)
        {
            var b = ep.Address.GetAddressBytes();
            int ip = 0;
            unsafe
            {
                fixed (byte* bp = &b[0])
                    ip = *(Int32*)bp;
            }
            var link = FindLink(ip, ep.Port);
            if (link == null)
            {
                link = new T();
                byte[] key = new byte[32];
                random.NextBytes(key);
                link.Key = key;
                byte[] iv = new byte[16];
                random.NextBytes(iv);
                link.Iv = iv;
                link.kcp = this;
                link.ConnectTime = DateTime.Now.Ticks;
                link.endpPoint = ep;
                link.port = ep.Port;
                link.ip = ip;
                int s = 0;
                int c = linkBuff[0].Count;
                for (int i = 1; i < tCount; i++)
                {
                    if (c > linkBuff[i].Count)
                    {
                        s = i;
                        c = linkBuff[i].Count;
                    }
                }
                link.buffIndex = s;
                linkBuff[s].Add(link);
                link.Awake();
            }
            return link;
        }
        public override void Dispatch(byte[] dat, int len, IPEndPoint ep)
        {
            var link = FindOrCreateLink(ep);
            if (link != null)
            {
                link._connect = true;
                link.RecvTime = DateTime.Now.Ticks;
                kcp.ReciveMsg(dat, len, link);
            }
        }
        public override void RemoveLink(NetworkLink link)
        {
            linkBuff[link.buffIndex].Delete(link);
        }
        public override void Dispose()
        {
            base.Dispose();
            Instance = null;
            for (int i = 0; i < tCount; i++)
                linkBuff[i].running = false;
        }
        public T FindLink(Int64 id)
        {
            for (int i = 0; i < tCount; i++)
            {
                var l = linkBuff[i].Find(id);
                if (l != null)
                    return l;
            }
            return null;
        }
        public T FindLink(int ip, int port)
        {
            for (int i = 0; i < tCount; i++)
            {
                var l = linkBuff[i].Find(ip, port);
                if (l != null)
                    return l;
            }
            return null;
        }
        /// <summary>
        /// 开启心跳,防止超时断线
        /// </summary>
        public void OpenHeart()
        {
            heart = true;
        }
        /// <summary>
        /// 关闭心跳
        /// </summary>
        public void CloseHeart()
        {
            heart = false;
        }
        UInt16 bid = 60000;
        public override void Broadcast(byte[] dat, byte type)
        {
            var tmp = Envelope.PackAll(dat, type, bid, 1472);
            long now = DateTime.Now.Ticks;
            for (int i = 0; i < tCount; i++)
            {
                linkBuff[i].AddMsg(tmp, now, bid);
            }
            bid++;
            if (bid > 64000)
                bid = 60000;
        }
        bool heart;
        byte[] Heart = new byte[] { 255, 255, 255, 255, 0, 255, 255, 255, 254 };
        long last;
        public override void SendAll()
        {
            long now = DateTime.Now.Ticks / 10000;
            long r = now % 10000;
            Int16 time = (Int16)r;
            kcp.UnPack(time);
            if(heart)
            {
                long c = now / 10000;
                bool n = c > last ? true : false;
                if (n)
                {
                    last = c;
                    for (int i = 0; i < tCount; i++)
                    {
                        linkBuff[i].SendAll(soc, kcp, time, Heart);
                    }
                }
                else
                {
                    for (int i = 0; i < tCount; i++)
                    {
                        linkBuff[i].SendAll(kcp, time);
                    }
                }
            }
            else
            {
                for (int i = 0; i < tCount; i++)
                {
                    linkBuff[i].SendAll(kcp, time);
                }
            }
        }
        protected override void ManageLinks()
        {
            long now = DateTime.Now.Ticks;
            for (int i = 0; i < tCount; i++)
            {
                linkBuff[i].DeleteTimeOutLink(this, now);
            }
        }
    }
}
