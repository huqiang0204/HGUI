using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace huqiang.Communication
{
    public struct KcpServerStats
    {
        public int UsageMemory;
        public int AllMemory;
        public int PEMemory;
        public int LinkCount;
    }
    public class KcpServer<T> : KcpListener where T : KcpLink, new()
    {
        NetworkContent[] ncs=new NetworkContent[32];
        List<BroadcastMsg> datas = new List<BroadcastMsg>();
        public static UInt16 MinID = 60000;
        public static UInt16 MaxID = 64000;
        static Random random = new Random();
        public static int SingleCount = 2048;
        public KcpThread<T>[] linkBuff;
        int tCount = 0;
        /// <summary>
        /// 拒绝外部发起的新连接
        /// </summary>
        public bool RejectAutoConnections = false;
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
            link.RecvTime = DateTime.Now.Ticks;
            return link;
        }
        public override void Dispatch(byte[] dat, int len, IPEndPoint ep)
        {
            T link;
            if (RejectAutoConnections)
            {
                var b = ep.Address.GetAddressBytes();
                int ip = 0;
                unsafe
                {
                    fixed (byte* bp = &b[0])
                        ip = *(Int32*)bp;
                }
                link = FindLink(ip, ep.Port);
            }
            else
            {
                link = FindOrCreateLink(ep);
            }
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
        public override bool PostBroadcast(byte[] dat, byte type, int spin)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            for (int i = 0; i < 32; i++)
            {
                if (ncs[i].State == 0)
                {
                    ncs[i].State = 1;
                    ncs[i].ThreadID = id;
                    for (int j = 0; j < spin; j++)
                        if (ncs[i].ThreadID != id)
                            goto label;
                    ncs[i].MsgID = type;
                    ncs[i].dat = dat;
                    ncs[i].State = 2;
                    return true;
                }
            label:;
            }
            return false;
        }
        bool GetBroadcast(ref NetworkContent content)
        {
            for (int i = 0; i < 32; i++)
            {
                if (ncs[i].State == 2)
                {
                    content = ncs[i];
                    ncs[i].dat = null;
                    ncs[i].MsgID = 0;
                    ncs[i].State = 0;
                    return true;
                }
            }
            return false;
        }
        protected override void ProBroadcast()
        {
            NetworkContent nc = new NetworkContent();
            int id = sendThread.ManagedThreadId;
            long now = DateTime.Now.Ticks / 10000;
            long r = now % 30000;
            Int16 time = (Int16)r;
            while (GetBroadcast(ref nc))
            {
                var dat = kcp.Pack(nc.dat, (byte)nc.MsgID, bid, time);
                BroadcastMsg msg = new BroadcastMsg();
                msg.MsgID = bid;
                msg.data = dat;
                datas.Add(msg);
                for (int i = 0; i < tCount; i++)
                {
                    linkBuff[i].Broadcast(id,bid);
                }
                bid++;
                if (bid >= MaxID)
                    bid = MinID;
            }
        }
        public override BroadcastMsg FindBroadcastMsg(int msgID)
        {
            for(int i=0;i<datas.Count;i++)
            {
                if(datas[i].MsgID==msgID)
                {
                    return datas[i];
                }
            }
            return null;
        }
        public override void UpdateBroadcastMsg(int msgID)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].MsgID == msgID)
                {
                    datas[i].SendTime = now;
                    datas[i].SendCount++;
                    return;
                }
            }
        }
        bool heart;
  
        long now,last;
        public override void SendAll()
        {
            now = DateTime.Now.Ticks;
            for (int i=0;i<datas.Count;i++)
            {
                datas[i].SendCount = 0;
            }
            long t = now / 10000;//将单位时间设为毫秒
            long r = t % 30000;//设置循环时间为30000/1000=30秒
            Int16 time = (Int16)r;
            kcp.UnPack(time);
            if(heart)
            {
                long c = t / 1000;//当前时间，单位秒
                if (c > last)//需要给当前没有消息的用户发送心跳包
                {
                    last = c;
                    for (int i = 0; i < tCount; i++)
                    {
                        linkBuff[i].SendAll(soc, kcp, time, Heart);
                    }
                }
                else//发送用户的消息
                {
                    for (int i = 0; i < tCount; i++)
                    {
                        linkBuff[i].SendAll(kcp, time);
                    }
                }
            }
            else//发送用户的消息
            {
                for (int i = 0; i < tCount; i++)
                {
                    linkBuff[i].SendAll(kcp, time);
                }
            }
            for (int c = datas.Count - 1; c >= 0; c--)
            {
                if (datas[c].SendCount == 0)
                {
                    if (now - datas[c].SendTime > 10*1000*10000)//清除超过10秒未被使用的广播消息
                    {
                        var bm = datas[c];
                        bm.Dispose();
                        datas.RemoveAt(c);
                    }
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
        /// <summary>
        /// 获取统计数据
        /// </summary>
        public KcpServerStats GetStats()
        {
            KcpServerStats kss = new KcpServerStats();
            kss.AllMemory = kcp.AllMemory;
            kss.PEMemory = kcp.PEMemory;
            kss.UsageMemory = kcp.UsageMemory;
            for (int i = 0; i < tCount; i++)
            {
                kss.LinkCount += linkBuff[i].Count;
            }
            return kss;
        }
    }
}
