using huqiang.Data;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace huqiang
{
    public class KcpLink:NetworkLink
    {
        internal KcpListener kcp;
        public Int64 id;
        public string uniId;
        public byte[] Key;
        public byte[] Iv;
        public KcpEnvelope envelope = new KcpEnvelope();
     
        public QueueBuffer<byte[]> metaData = new QueueBuffer<byte[]>();
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
        internal bool _connect;
        public bool Connected { get { return _connect; } }
        public override void Recive(long now)
        {
            int c = metaData.Count;
            if (c == 0)
            {
                if (now - lastTime > TimeOut)
                {
                    envelope.Clear();
                    if (_connect)
                    {
                        Disconnect();
                        _connect = false;
                    }
                }
            }
            else
            {
                lastTime = now;
                if (!_connect)
                    ConnectionOK();
                _connect = true;
            }
            for (int i = 0; i < c; i++)
            {
                var dat = metaData.Dequeue();
                if (dat != null)
                    envelope.Unpack(dat, dat.Length, now);
            }
            var queue = envelope.QueueBuf;
            c = queue.Count;
            try
            {
                for (int i = 0; i < c; i++)
                {
                    var dat = queue.Dequeue();
                    if (dat != null)
                    {
                        Dispatch(dat.data, dat.type);
                    }
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 支持数据长度30Mb 30000 * 1024
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool Send(byte[] data, byte type)
        {
            if (data.Length > 30000 * 1024)
                return false;
            envelope.Pack(data, type);
            //for (int i = 0; i < ss.Length; i++)
            //    kcp.soc.SendTo(ss[i], endpPoint);
            return true;
        }
        public override void AddMsg(byte[][] dat, long now,UInt16 msgID)
        {
            envelope.AddMsg(dat,now,msgID);
        }
        public override void Send(Socket soc, long now)
        {
            envelope.Send(soc,now,endpPoint);
        }
        public virtual void Awake()
        {
        }
        public virtual void Connect()
        {
        }
        public virtual void Disconnect()
        {
            _connect = false;
        }
        public virtual void ConnectionOK()
        {
        }
        public virtual void Dispatch(byte[] dat, byte tag)
        {
        }
        public virtual void Dispose()
        {
            envelope.Dispose();
            envelope = null;
            if (metaData != null)
                metaData.Clear();
            metaData = null;
            kcp = null;
        }
        public void Redirect(int ciP,int cport)
        {
            endpPoint.Address = new IPAddress(ciP.ToBytes());
            endpPoint.Port = cport;
            ip = ciP;
            port = cport;
            envelope.Clear();
            metaData.Clear();
        }
    }
}