using System;
using System.Collections.Generic;
using System.Text;

namespace huqiang
{

    public struct EnvelopeItem
    {
        /// <summary>
        /// 封包头
        /// </summary>
        public EnvelopeHead head;
        /// <summary>
        /// 分卷
        /// </summary>
        public UInt32 part;
        /// <summary>
        /// 总计接收到的消息长度
        /// </summary>
        public UInt32 rcvLen;
        /// <summary>
        /// 消息缓存
        /// </summary>
        public byte[] buff;
        /// <summary>
        /// 接收到的消息时间,用来做超时处理
        /// </summary>
        public long time;
        /// <summary>
        /// 消息分卷状态
        /// </summary>
        public Int32[] checks;
        /// <summary>
        /// 保留一定时效
        /// </summary>
        public bool done;
    }
    public class TcpEnvelope
    {
        /// <summary>
        /// 消息的最小id
        /// </summary>
        public  static UInt16 MinID = 22000;
        /// <summary>
        /// 消息的最大id
        /// </summary>
        public static UInt16 MaxID = 32000;
        /// <summary>
        /// 封包类型
        /// </summary>
        public PackType type = PackType.All;
        /// <summary>
        /// 封包缓存
        /// </summary>
        protected EnvelopeItem[] pool = new EnvelopeItem[128];
        protected int remain = 0;
        protected byte[] buffer;
        protected UInt16 id = 22000;
        /// <summary>
        /// 封包分卷长度
        /// </summary>
        protected UInt16 Fragment = 1460;
        /// <summary>
        /// Solution Slices Segment
        /// </summary>
        protected Int16 sss = 1389;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffLen">256kb</param>
        public TcpEnvelope(int buffLen = 262144)
        {
            buffer = new byte[buffLen];
        }
        /// <summary>
        /// 数据封包
        /// </summary>
        /// <param name="dat">数据</param>
        /// <param name="tag">数据类型</param>
        /// <returns></returns>
        public virtual byte[][] Pack(byte[] dat, byte tag)
        {
            var all = Envelope.Pack(dat, tag, type, id,Fragment);
            id += (UInt16)all.Length;
            if (id >= MaxID)
                id = MinID;
            return all;
        }
        /// <summary>
        /// 数据解包
        /// </summary>
        /// <param name="dat">缓存</param>
        /// <param name="len">数据长度</param>
        /// <returns></returns>
        public virtual List<EnvelopeData> Unpack(byte[] dat, int len)
        {
            try
            {
                ClearTimeout();
                switch (type)
                {
                    case PackType.Part:
                        return OrganizeSubVolume(Envelope.UnpackPart(dat, len, buffer, ref remain, Fragment), Fragment - 16);
                    //case PackType.Total:
                    //    return Envelope.UnpackInt(dat, len, buffer, ref remain);
                    case PackType.All:
                        var list = Envelope.UnpackInt(dat, len, buffer, ref remain);
                        return OrganizeSubVolume(Envelope.EnvlopeDataToPart(list), sss);
                }
                return null;
            }
            catch
            {
                remain = 0;
                return null;
            }
        }
        /// <summary>
        /// 分析合并数据分卷
        /// </summary>
        /// <param name="list">数据分卷列表</param>
        /// <param name="fs">封包分卷长度</param>
        /// <returns></returns>
        protected List<EnvelopeData> OrganizeSubVolume(List<EnvelopePart> list, int fs)
        {
            if (list != null)
            {
                List<EnvelopeData> datas = new List<EnvelopeData>();
                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    int ap = item.head.AllPart;
                    if (ap> 1)
                    {
                        int s = -1;
                        for (int i = 0; i < 128; i++)
                        {
                            if (s < 0)
                            {
                                if (pool[i].head.MsgID == 0)
                                    s = i;
                            }
                            if (item.head.MsgID == pool[i].head.MsgID)
                            {
                                if(Envelope.SetChecked(pool[i].checks, item.head.CurPart))
                                {
                                    Envelope.CopyToBuff(pool[i].buff, item.data, 0, item.head, fs);
                                    pool[i].part++;
                                    pool[i].rcvLen += item.head.PartLen;
                                    if (pool[i].rcvLen >= item.head.Lenth)
                                    {
                                        EnvelopeData data = new EnvelopeData();
                                        data.data = pool[i].buff;
                                        data.type = (byte)(pool[i].head.Type);
                                        pool[i].head.MsgID = 0;
                                        datas.Add(data);
                                    }
                                }
                                goto label;
                            }
                        }
                        pool[s].head = item.head;
                        pool[s].part = 1;
                        pool[s].rcvLen = item.head.PartLen;
                        pool[s].buff = new byte[item.head.Lenth];
                        pool[s].time = DateTime.Now.Ticks;
                        Envelope.CopyToBuff(pool[s].buff, item.data, 0, item.head, fs);
                        int c = ap / 32 + 1;
                        pool[s].checks = new Int32[c];
                    }
                    else
                    {
                        EnvelopeData data = new EnvelopeData();
                        data.data = item.data;
                        data.type = (byte)(item.head.Type);
                        datas.Add(data);
                    }
                label:;
                }
                return datas;
            }
            return null;
        }
        /// <summary>
        /// 清除超时的消息
        /// </summary>
        protected void ClearTimeout()
        {
            var now = DateTime.Now.Ticks;
            for (int i = 0; i < 128; i++)
            {
                if (pool[i].head.MsgID > 0)
                    if (now - pool[i].time > 20 * 10000000)//清除超时20秒的消息
                        pool[i].head.MsgID = 0;
            }
        }
        /// <summary>
        /// 清除所有消息
        /// </summary>
        public virtual void Clear()
        {
            remain = 0;
            for (int i = 0; i < 128; i++)
            {
                pool[i].head.MsgID = 0;
            }
        }
        ~TcpEnvelope()
        {
            Clear();
        }
    }
    public class UdpEnvelope:TcpEnvelope
    {
        public UdpEnvelope()
        {
            Fragment = 1472;
            sss = 1403;
        }
    }
}
