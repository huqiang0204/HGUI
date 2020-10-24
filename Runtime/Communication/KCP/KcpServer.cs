using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang
{
    /// <summary>
    /// kcp服务,kcp一个单独的线程用于接收消息
    /// 一个单独得定线程发送消息
    /// 一个单独的线程解析kcp数据包,然后推到每个连接的缓存中
    /// 连接更新线程有多个,检查和处理每个连接中的缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KcpServer<T> : KcpListener where T : KcpLink, new()
    {
        /// <summary>
        /// 用于广播服务的最小消息id
        /// </summary>
        public static UInt16 MinID = 60000;
        /// <summary>
        /// 用于广播服务的最大消息id
        /// </summary>
        public static UInt16 MaxID = 64000;
        static Random random = new Random();
        public KcpThread<T>[] linkBuff;
        int tCount = 0;
        /// <summary>
        /// 拒绝外部发起的新连接
        /// </summary>
        public bool RejectAutoConnections = false;
        Kcp kcp;
        /// <summary>
        /// kcp服务器
        /// </summary>
        /// <param name="port">服务端口,默认为自动端口</param>
        public KcpServer(int port = 0) : base(port)
        {
            Instance = this;
            kcp = new Kcp();
        }
        /// <summary>
        /// 运行服务
        /// </summary>
        /// <param name="threadCount">线程数量</param>
        /// <param name="threadbuff">每个线程的缓存数</param>
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
        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void Close()
        {
            soc.Close();
        }
        /// <summary>
        /// 查找或创建一个该地址的用户连接
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 处理监听到的消息
        /// </summary>
        /// <param name="dat">缓存</param>
        /// <param name="len">数据长度</param>
        /// <param name="ep">远程地址</param>
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
        /// <summary>
        /// 移除某个连接
        /// </summary>
        /// <param name="link"></param>
        public override void RemoveLink(NetworkLink link)
        {
            linkBuff[link.buffIndex].Delete(link);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            Instance = null;
            for (int i = 0; i < tCount; i++)
                linkBuff[i].running = false;
        }
        /// <summary>
        /// 使用id查询某个用户
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
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
        /// <summary>
        /// 使用ip地址查询某个用户
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
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
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="type">数据类型</param>
        public override void Broadcast(byte[] dat, byte type)
        {
            long now = DateTime.Now.Ticks / 10000;
            long r = now % 10000;
            Int16 time = (Int16)r;
            var tmp = Kcp.Pack(dat, type, bid, time);
            MsgInfo2[] msg = new MsgInfo2[tmp.Length];
            for(int i=0;i<msg.Length;i++)
            {
                msg[i].CurPart =(ushort) i;
                msg[i].data = tmp[i];
                msg[i].CreateTime = time;
                msg[i].MsgID = bid;
            }
            for (int i = 0; i < tCount; i++)
            {
                linkBuff[i].AddMsg(msg);
            }
            bid++;
            if (bid >= MaxID)
                bid = MinID;
        }
        bool heart;
  
        long last;
        /// <summary>
        /// 发送缓存中的消息
        /// </summary>
        public override void SendAll()
        {
            long now = DateTime.Now.Ticks / 10000;
            long r = now % 10000;
            Int16 time = (Int16)r;
            kcp.UnPack(time);
            if(heart)
            {
                long c = now / 1000;
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
        /// <summary>
        /// 管理当前的用户连接
        /// </summary>
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
