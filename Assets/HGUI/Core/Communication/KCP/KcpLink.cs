using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang
{
    public  class KcpLink:KcpData
    {
        internal KcpListener kcp;
        public string uniId;
        public byte[] Key;
        public byte[] Iv;

        /// <summary>
        /// 5秒
        /// </summary>
        public static long TimeOut = 50000000;//5*1000*10000
        /// <summary>
        /// 第一次连接时间
        /// </summary>
        public long ConnectTime;
        /// <summary>
        /// 最后一次接收到数据的时间
        /// </summary>
        protected long lastTime;

        public override void Recive(long now)
        {
            int c = recvQueue.Count;
            for (int i = 0; i < c; i++)
            {
                var dat = recvQueue.Dequeue();
                Dispatch(dat.dat,dat.type);
                dat.dat.Release();
            }
        }
        public virtual void Awake()
        {
        }
        public virtual void Connect()
        {
        }
        public virtual void Dispatch(BlockInfo<byte> dat, byte tag)
        {
        }
        public void Redirect(int ciP, int cport)
        {
            endpPoint.Address = new IPAddress(ciP.ToBytes());
            endpPoint.Port = cport;
            ip = ciP;
            port = cport;
            int c = recvQueue.Count;
            for (int i = 0; i < c; i++)
            {
                recvQueue.Dequeue().dat.Release();
            }
        }
    }
}
