using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Communication
{
    public class KcpData:NetworkLink
    {
        /// <summary>
        /// 最后一次接受到数据的时钟周期
        /// </summary>
        public long RecvTime;
        /// <summary>
        /// 包头4字节,包尾4字节,数据头21字节,12个节点,每个节点4字节=1399
        /// </summary>
        public static int FragmentSize = (1472 - 8 - 15) - 12 * 4;
        /// <summary>
        /// 设置分包分卷状态
        /// </summary>
        /// <param name="checks">状态缓存</param>
        /// <param name="part">当前分卷</param>
        /// <returns>如果已经缓存过了返回假</returns>
        unsafe static bool SetChecked(Int32* checks, int part)
        {
            int c = part / 32;
            int r = part % 32;
            int o = 1 << r;
            int v = checks[c];
            if ((v & o) > 0)
                return false;
            v |= o;
            checks[c] = v;
            return true;
        }
        static void CopyToBuff(ref BlockInfo buff, ref MsgHead head, byte[] src)
        {
            int start = MsgHead.Size;
            if (buff.DataCount > 0)
            {
                int index = head.CurPart * FragmentSize;
                int len = head.PartLen;
                int all = buff.DataCount;
                unsafe
                {
                    byte* tar = buff.Addr;
                    for (int i = 0; i < len; i++)
                    {
                        if (index >= all)
                            break;
                        tar[index] = src[start];
                        index++;
                        start++;
                    }
                }
            }
        }
        /// <summary>
        /// 接受到的消息缓存信息
        /// </summary>
        struct MsgCache:IDisposable
        {
            public MsgHead head;
            public UInt32 part;
            public UInt32 rcvLen;
            public BlockInfo buff;
            public long time;
            public BlockInfo<int> states;

            public void Dispose()
            {
                buff.Release();
                states.Release();
            }
            //public bool done;//保留一定时效
        }
        /// <summary>
        /// 接受到的消息缓存
        /// </summary>
        MsgCache[] caches = new MsgCache[128];
        int delayStart;
        int delayEnd;
        Int16[] delays = new Int16[128];//时延统计
        int max = 0;
        /// <summary>
        /// 接收到的解包数据
        /// </summary>
        protected QueueBufferS<BlockData> recvQueue = new QueueBufferS<BlockData>(128);
        /// <summary>
        /// 需要发送的封包数据
        /// </summary>
        internal List<MsgInfo> Msgs = new List<MsgInfo>();
        public int Remain = 0;
        public UInt16 MsgId = 34000;
        /// <summary>
        /// 添加一个接收到的消息分卷
        /// </summary>
        /// <param name="dat">数据缓存</param>
        /// <param name="len">数据长度</param>
        /// <param name="kcp">封包器</param>
        /// <param name="time">数据接收时间,单位毫秒</param>
        public void AddMsgPart(byte[] dat, int len, Kcp kcp, Int16 time)
        {
            byte type = dat[0];
            if (type == EnvelopeType.Success)
            {
                MsgReturn head;
                unsafe
                {
                    fixed (byte* bp = &dat[0])
                        head = *(MsgReturn*)bp;
                }
                Success(ref head, time);
            }
            else
            {
                MsgHead head;
                unsafe
                {
                    fixed (byte* bp = &dat[0])
                        head = *(MsgHead*)bp;
                }
                int nul = 0;
                int fill = 0;
                int id = head.MsgID;
                for (int i = 0; i < 128; i++)
                {
                    int cid = caches[i].head.MsgID;
                    if (cid > 0)
                    {
                        if (cid == id)
                        {
                            FillMsg(ref head, dat, i);
                            nul = i;
                            goto label;
                        }
                        fill++;
                    }
                    else
                    if (nul == 0)
                        nul = i;
                    if (fill >= max)
                        break;
                }
                caches[nul].buff = kcp.MemoryRequest((int)head.Lenth);
                caches[nul].buff.DataCount = (int)head.Lenth;
                if (head.AllPart > 1)
                {
                    int c = head.AllPart / 32;
                    if (head.AllPart % 32 > 0)
                        c++;
                    caches[nul].states = kcp.statesBuffer.RegNew(c);
                    caches[nul].states.Zero();
                }
                caches[nul].time = time;
                caches[nul].head = head;
                caches[nul].part = head.AllPart;
                AddNew(ref head, dat, nul);
                max++;
            label:;
                if (caches[nul].rcvLen >= caches[nul].part)
                {
                    caches[nul].head.MsgID = 0;
                    caches[nul].states.Release();
                    BlockData data = new BlockData();
                    data.type = head.Type;
                    data.dat = caches[nul].buff;
                    recvQueue.Enqueue(data);
                    max--;
                }
                kcp.Success(ref head, this);
            }
        }
        void Success(ref MsgReturn head,Int16 time)
        {
            for (int i = Msgs.Count - 1; i >= 0; i--)
            {
                if (head.MsgID == Msgs[i].MsgID)
                {
                    if (head.CurPart == Msgs[i].CurPart)
                    {
                        Int16 ot = (Int16)(time - Msgs[i].CreateTime);
                        if (ot < 0)
                            ot += 30000;
                        delays[delayEnd] = ot;
                        delayEnd++;
                        if (delayEnd >= 128)
                            delayEnd = 0;
                        if (head.MsgID < 60000)//这是一个私有消息，可以释放内存
                            Msgs[i].data.Release();
                        Msgs.RemoveAt(i);
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 数据平均时延
        /// </summary>
        public int Delay
        {
            get
            {
                int a = 0;
                int i = 0;
                for (; i < 128; i++)
                {
                    if (delayStart == delayEnd)
                        break;
                    a += delays[delayStart];
                    delays[delayStart] = 0;
                    if (delayStart < 127)
                        delayStart++;
                    else delayStart = 0;
                }
                if (i == 0)
                    return 0;
                return a / i;
            }
        }
        void AddNew(ref MsgHead head, byte[] dat, int index)
        {
            if (head.AllPart > 1)
            {
                unsafe
                {
                    SetChecked(caches[index].states.Addr, head.CurPart);
                }
            }
            CopyToBuff(ref caches[index].buff, ref head, dat);
            caches[index].rcvLen = 1;
        }
        void FillMsg(ref MsgHead head,byte[] dat, int index)
        {
            bool a;
            unsafe
            {
                a = SetChecked(caches[index].states.Addr, head.CurPart);
            }
            if (a)
            {
                CopyToBuff(ref caches[index].buff, ref head, dat);
                caches[index].rcvLen++;
            }
        }
        /// <summary>
        /// 处理接收缓存中的消息,如果未被重写,则未释放消息内存
        /// </summary>
        /// <param name="now"></param>
        internal override void Recive(long now)
        {
            int c = recvQueue.Count;
            for (int i = 0; i < c; i++)
            {
                recvQueue.Dequeue().dat.Release();
            }
        }
        /// <summary>
        /// 释放所占用的非托管内存
        /// </summary>
        internal override void FreeMemory()
        {
            int c = recvQueue.Count;
            for (int i = 0; i < c; i++)
            {
                recvQueue.Dequeue().dat.Release();
            }
        }
        /// <summary>
        /// 返回true,则在线程管理中移除,不再参与重连
        /// </summary>
        /// <returns></returns>
        public virtual bool Disconnect()
        {
            _connect = false;
            return true;
        }
    }
}
