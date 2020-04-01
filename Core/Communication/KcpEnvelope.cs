using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace huqiang
{
    public class KcpEnvelope:IDisposable
    {
        public static long timeout = 5000000;
        public class DataItem
        {
            public UInt16 msgID;
            public UInt16 partID;
            public long time;
            public byte[] dat;
            public bool send;
        }
        int delayStart;
        int delayEnd;
        UInt16[] delays = new UInt16[256];//时延统计
        public static UInt16 MinID = 34000;
        public static UInt16 MaxID = 44000;
        public QueueBuffer<EnvelopeData> QueueBuf = new QueueBuffer<EnvelopeData>(64);
        List<DataItem> sendBuffer = new List<DataItem>();
        public List<byte[]> ValidateData = new List<byte[]>();
        protected UInt16 id = 34000;
        protected UInt16 Fragment = 1472;
        protected EnvelopeItem[] pool = new EnvelopeItem[128];
        protected int remain = 0;
        protected byte[] buffer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffLen">256kb</param>
        public KcpEnvelope(int buffLen = 262144)
        {
            buffer = new byte[buffLen];
        }
        public byte[][] Pack(byte[] dat, byte type)
        {
            var tmp = Envelope.PackAll(dat, type, id, Fragment);
            long now = DateTime.Now.Ticks;
            for(int i=0;i<tmp.Length;i++)
            {
                DataItem item = new DataItem();
                item.msgID = id;
                item.partID =(UInt16)i;
                item.dat = tmp[i];
                item.time = now;
                sendBuffer.Add(item);
            }
            id++;
            if (id >= MaxID)
                id = MinID;
            return tmp;
        }
        protected void ClearTimeout(long now)
        {
            for (int i = 0; i < 128; i++)
            {
                if (pool[i].head.MsgID > 0)
                    if (now - pool[i].time > 20 * 10000000)//清除超时20秒的消息
                        pool[i].head.MsgID = 0;
            }
        }
        public void Unpack(byte[] dat, int len, long now)
        {
            try
            {
                ClearTimeout(now);
                var list = Envelope.UnpackInt(dat, len, buffer, ref remain);
                var dats = Envelope.EnvlopeDataToPart(list);
                int c = dats.Count - 1;
                for (; c >= 0; c--)
                {
                    var item = dats[c];
                    UInt16 tag = item.head.Type;
                    byte type = item.type;
                    if (type == EnvelopeType.Heart)//这是一个心跳包
                    {
                        dats.RemoveAt(c);
                    }
                    else
                    if (type == EnvelopeType.Success)//这是一个数据接收成功的回执
                    {
                        Success(item.head.MsgID, item.head.CurPart, now);
                        dats.RemoveAt(c);
                    }
                    else
                    {
                        var tmp = Envelope.PackAll(EnvelopeType.Success, item.head.MsgID, item.head.CurPart);
                        ValidateData.Add(tmp);
                    }
                }
                OrganizeSubVolume(dats, 1403);
            }
            catch
            {
                remain = 0;
            }
        }
        int point;
        protected void OrganizeSubVolume(List<EnvelopePart> list, int fs)
        {
            if (list != null)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    int ap = item.head.AllPart;
                    if (ap > 1)
                    {
                        for (int i = 0; i < 128; i++)
                        {
                            if (item.head.MsgID == pool[i].head.MsgID)
                            {
                                if (pool[i].buff == null)
                                    goto label;
                                if (Envelope.SetChecked(pool[i].checks, item.head.CurPart))
                                {
                                    Envelope.CopyToBuff(pool[i].buff, item.data, 0, item.head, fs);
                                    pool[i].part++;
                                    pool[i].rcvLen += item.head.PartLen;
                                    if (pool[i].rcvLen >= item.head.Lenth)
                                    {
                                        EnvelopeData data = new EnvelopeData();
                                        data.data = pool[i].buff;
                                        data.type = (byte)(pool[i].head.Type);
                                        pool[i].buff = null;
                                        //pool[i].checks = null;
                                        QueueBuf.Enqueue(data);
                                        pool[i].done = true;
                                    }
                                }
                                goto label;
                            }
                        }
                        int s = point;
                        for (int i = 0; i < 128; i++)
                        {
                            if (pool[s].head.MsgID == 0 | pool[s].done)
                            { point = s; break; }
                            s++;
                            if (s >= 128)
                                s = 0;
                        }
                        pool[s].head = item.head;
                        pool[s].part = 1;
                        pool[s].rcvLen = item.head.PartLen;
                        pool[s].buff = new byte[item.head.Lenth];
                        pool[s].time = DateTime.Now.Ticks;
                        Envelope.CopyToBuff(pool[s].buff, item.data, 0, item.head, fs);
                        int c = ap / 32 + 1;
                        pool[s].checks = new Int32[c];
                        Envelope.SetChecked(pool[s].checks, item.head.CurPart);
                    }
                    else
                    {
                        EnvelopeData data = new EnvelopeData();
                        data.data = item.data;
                        data.type = (byte)(item.head.Type);
                        QueueBuf.Enqueue(data);
                    }
                label:;
                }
            }
        }
        public void Clear()
        {
            remain = 0;
            for (int i = 0; i < 128; i++)
            {
                pool[i].head.MsgID = 0;
                pool[i].buff = null;
            }
            sendBuffer.Clear();
        }
        
        void Success(UInt16 _id,UInt16 part, long now)
        {
            for (int i = 0; i < sendBuffer.Count; i++)
                if (sendBuffer[i].msgID == _id)
                    if (sendBuffer[i].partID == part)
                    {
                        now -= sendBuffer[i].time;
                        now /= 10000;
                        delays[delayEnd] = (UInt16)now;
                        if (delayEnd < 255)
                            delayEnd++;
                        else delayEnd = 0;
                        sendBuffer.RemoveAt(i);
                        break;
                    }
        }
        public int Delay { get {
                int a = 0;
                int i = 0;
                for (; i < 256; i++)
                {
                    if (delayStart == delayEnd)
                        break;
                    a += delays[delayStart];
                    delays[delayStart] = 0;
                    if (delayStart < 255)
                        delayStart++;
                    else delayStart = 0;
                }
                if (i == 0)
                    return 0;
                return a / i;
            } }
        public void Dispose()
        {
            Clear();
            ValidateData.Clear();
        }
        public bool Send(Socket soc, long now, IPEndPoint ip)
        {
            int c = 0;
            lock (ValidateData)
            {
                int len = ValidateData.Count;
                for (int i = 0; i < len; i++)
                    soc.SendTo(ValidateData[i], ip);//通知对方接收数据成功
                ValidateData.Clear();//清除接收成功的数据
            }
            for (int i = 0; i < sendBuffer.Count; i++)
            {
                var dat = sendBuffer[i];
                if (dat != null)
                {
                    if(dat.send)//已经发送过了
                    {
                        if (now - dat.time > timeout)
                        {
                            dat.time += timeout;
                           c = soc.SendTo(dat.dat, ip);//重新发送超时的数据
                        }
                    }
                    else
                    {
                        c = soc.SendTo(dat.dat, ip);//首次发送数据
                        dat.send = true;
                        dat.time = now;
                    }
                }
            }
            return c > 0;
        }
       
        /// <summary>
        /// 添加需要发送的消息
        /// </summary>
        public void AddMsg(byte[][] dat,long now,UInt16 msgID)
        {
            lock (sendBuffer)
            {
                for(UInt16 i=0;i<dat.Length;i++)
                {
                    DataItem item = new DataItem();
                    item.msgID = msgID;
                    item.partID = i;
                    item.time = now;
                    item.dat = dat[i];
                    sendBuffer.Add(item);
                }
            }
        }
    }
}
