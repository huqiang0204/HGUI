using System;
using System.Net;
using System.Threading;

namespace huqiang
{
    public class KcpServer<T>:KcpListener where T:KcpLink,new()
    {
        static Random random = new Random();
        public static int SingleCount = 2048;
        public LinkThread<T>[] linkBuff;
        int tCount = 0;
        public KcpServer(int port = 0) :base(port)
        {
            Instance = this;
            Day = DateTime.Now.Day;
        }
        public void Run(int threadCount = 8,int threadbuff = 2048)
        {
            tCount = threadCount;
            Start();
            if (tCount > 0)
            {
                linkBuff = new LinkThread<T>[threadCount];
                for (int i = 0; i < threadCount; i++)
                {
                    linkBuff[i] = new LinkThread<T>(threadbuff);
                    linkBuff[i].soc = soc;
                }
            }
            else
            {
                tCount = 1;
                linkBuff = new LinkThread<T>[1];
                linkBuff[0] = new LinkThread<T>();
                linkBuff[0].soc = soc;
            }
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
            var link = FindLink(ip,ep.Port);
            if (link == null)
            {
                link = new T();
                byte[] key = new byte[32];
                random.NextBytes(key);
                link.Key = key;
                byte[] iv = new byte[16];
                random.NextBytes(iv);
                link.Iv = iv;
                link.envelope = new KcpEnvelope();
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
        public override void Dispatch(byte[] dat, IPEndPoint ep)
        {
            var link = FindOrCreateLink(ep);
            if(link!=null)
            {
                link._connect = true;
                link.metaData.Enqueue(dat);
            }
        }
        public override void RemoveLink(KcpLink link)
        {
            linkBuff[link.buffIndex].Delete(link);
        }
        public override void Dispose()
        {
            base.Dispose();
            soc.Close();
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
        public T FindLink(int ip,int port)
        {
            for (int i = 0; i < tCount; i++)
            {
                var l = linkBuff[i].Find(ip, port);
                if (l != null)
                    return l;
            }
            return null;
        }
        ThreadTimer timer;
        static byte[] Heart = new byte[1];
        static int Day;
        static byte[][] HeartData;
        /// <summary>
        /// 开启心跳,防止超时断线
        /// </summary>
        public void OpenHeart()
        {
            HeartData = Envelope.PackAll(Heart, EnvelopeType.Heart, 0, 1472);
            if (timer==null)
            {
                timer = new ThreadTimer(1000);
                timer.Tick = (o,e) => {
                    try
                    {
                        for (int i = 0; i < tCount; i++)
                        {
                            linkBuff[i].SendAll(soc, HeartData);
                        }
                    }
                    catch
                    {
                    }
                };
            }
        }
        /// <summary>
        /// 关闭心跳
        /// </summary>
        public void CloseHeart()
        {
            if(timer!=null)
            {
                timer.Dispose();
                timer = null;
            }
        }
        UInt16 bid = 60000;
        public override void Broadcast(byte[] dat, byte type)
        {
            var tmp = Envelope.PackAll(dat, type, bid, 1472);
            long now = DateTime.Now.Ticks;
            for (int i = 0; i < tCount; i++)
            {
                linkBuff[i].AddMsg(tmp, now,bid);
            }
            bid++;
            if (bid > 64000)
                bid = 60000;
        }
        public void SendAll()
        {
            long now = DateTime.Now.Ticks;
            for (int i = 0; i < tCount; i++)
            {
                linkBuff[i].SendAll(soc, now);
            }
        }
    }
}
