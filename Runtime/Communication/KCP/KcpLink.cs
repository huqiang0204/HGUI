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
        /// <summary>
        /// 唯一id,可以由客户机传递过来
        /// </summary>
        public string uniId;
        /// <summary>
        ///  随机生成的,用于aes加密的钥匙
        /// </summary>
        public byte[] Key;
        /// <summary>
        /// 随机生成的,用于aes加密的钥匙
        /// </summary>
        public byte[] Iv;

        /// <summary>
        /// 连接超时时间 5秒
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
        /// <summary>
        /// 处理接收缓存中的消息
        /// </summary>
        /// <param name="now"></param>
        public override void Recive(long now)
        {
            int c = recvQueue.Count;
            for (int i = 0; i < c; i++)
            {
                var dat = recvQueue.Dequeue();
                Dispatch(dat.dat,dat.type);
                dat.dat.Release();//释放指针内存
            }
        }
        /// <summary>
        /// 初次建立连接调用
        /// </summary>
        public virtual void Awake()
        {
        }
        public virtual void Connect()
        {
        }
        /// <summary>
        /// 派发接收到的消息
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="tag"></param>
        public virtual void Dispatch(BlockInfo<byte> dat, byte tag)
        {
        }
        /// <summary>
        /// ip重定向
        /// </summary>
        /// <param name="ciP">ip地址</param>
        /// <param name="cport">端口</param>
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
