using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    public struct SocMsg:IDisposable
    {
        public BlockInfo<byte> dat;
        public KcpData link;

        public void Dispose()
        {
            dat.Release();
            link = null;
        }
    }

    public class Kcp
    {
        public static int FragmentSize = (1472 - 8 - 15) - 12 * 4;//包头4字节,包尾4字节,数据头21字节,12个节点,每个节点4字节=1399
        public static int MsgTimeOut = 500;
        public static UInt16 MinID = 34000;
        public static UInt16 MaxID = 44000;
        public List<BlockBuffer<byte>> Buffer = new List<BlockBuffer<byte>>();
        byte[] tmpBuffer = new byte[1500];
        byte[] tmp2 = new byte[1500];
        static QueueBufferS<SocMsg> queue = new QueueBufferS<SocMsg>(4096);
        BlockBuffer<byte> recvBuffer  = new BlockBuffer<byte>(1500, 1024);
        public BlockBuffer<int> statesBuffer;
        internal BlockInfo<byte> MemoryRequest(int len)
        {
            int c = len / 1500;
            if (len % 1500 > 0)
                c++;
            BlockInfo<byte> block = new BlockInfo<byte>();
            for (int i = 0; i < Buffer.Count; i++)
            {
                var t = Buffer[i];
                if (t.RemainBlock >= c)
                {
                    if (t.RegNew(ref block, len))
                    {
                        return block;
                    }
                }
            }
            BlockBuffer<byte> tmp = new BlockBuffer<byte>(1500, 8192);
            tmp.RegNew(ref block, len);
            Buffer.Add(tmp);
            return block;
        }
        KcpListener kcpListener;
        public void Run(KcpListener listener,int threadCount)
        {
            kcpListener = listener;
            statesBuffer = new BlockBuffer<int>(8, 2048 * threadCount);
        }
        public void ReciveMsg(byte[] buf, int len,KcpData link)
        {
            var tmp = recvBuffer.RegNew(len);
            tmp.DataCount = len;
            unsafe
            {
                byte* ptr = tmp.Addr;
                for (int i = 0; i < len; i++)
                    ptr[i] = buf[i];
            }
            SocMsg msg = new SocMsg();
            msg.link = link;
            msg.dat = tmp;
            queue.Enqueue(msg);
        }
        internal void UnPack(Int16 time)
        {
            int c = queue.Count;
            for (int i = 0; i < c; i++)
            {
                SocMsg msg = queue.Dequeue();
                MergeData(ref msg);
                ParseData(ref msg,time);
                msg.dat.Release();
            }
        }
        void MergeData(ref SocMsg msg)
        {
            byte[] buf = msg.link.RecvBuffer;
            int s = msg.link.Remain;
            int c = msg.dat.DataCount;
            unsafe
            {
                byte* src = msg.dat.Addr;
                for (int i = 0; i < c; i++)
                {
                    buf[s] = src[i];
                    s++;
                }
            }
            msg.link.Remain = s;
        }
        void ParseData(ref SocMsg msg,Int16 time)
        {
            int s = 0;
            var buf = msg.link.RecvBuffer;
            int r = msg.link.Remain;
            int e = r;
            while (GetPart(buf, ref s, ref e))
            {
                int len = e - s;
                if (len > 4 & len < 1490)
                {
                    len = UnpackInt(buf, s, e);
                    msg.link.AddMsgPart(tmpBuffer, len, this, time);
                }
                s = e + 4;
                e = r;
            }
            if (s > 0)
            {
                int len = r - s;
                for (int i = 0; i < len; i++)
                {
                    buf[i] = buf[s];
                    s++;
                }
                msg.link.Remain = len;
            }
        }
        bool GetPart(byte[] buf, ref int start, ref int end)
        {
            int s = start;
            int len = end - start;
            for (int i = 0; i < len; i++)
            {
                var b = buf[s];
                if (b == 255)
                {
                    if (s + 3 < end)
                    {
                        if (buf[s + 1] == 255)
                            if (buf[s + 2] == 255)
                            {
                                b = buf[s + 3];
                                if (b == 255)
                                {
                                    start = i + 4;
                                    s += 3;
                                }
                                else if (b == 254)
                                {
                                    end = s;
                                    return true;
                                }
                            }
                    }
                }
                s++;
            }
            return false;
        }
        void UnpackInt(byte[] src, int si, int ti,int len)
        {
            int a = src.ReadInt32(si);
            int st = si + 4;
            int tt = ti;
            for (int i = 0; i < 124; i++)
            {
                if (tt >= len)
                    break;
                tmpBuffer[tt] = src[st];
                st++;
                tt++;
            }
            si += 4;
            for (int i = 0; i < 31; i++)
            {
                if (ti >= len)
                    break;
                byte b = src[si];
                if ((a & 1) > 0)
                    b += 128;
                a >>= 1;
                tmpBuffer[ti] = b;
                ti += 4;
                si += 4;
            }
        }
        int UnpackInt(byte[] dat, int start, int end)
        {
            int len = end - start;
            int part = len / 128;
            int r = len % 128;
            int tl = part * 124;
            if (r > 0)
            {
                part++;
                tl += r;
                tl -= 4;
            }
            int s0 = start;
            int s1 = 0;
            for (int i = 0; i < part; i++)
            {
                UnpackInt(dat, s0, s1, tl);
                s0 += 128;
                s1 += 124;
            }
            return tl;
        }
        public int SendMsg(KcpData link,Int16 time)
        {
            var q = link.sendQueue;
            int c = q.Count;
            for (int i = 0; i < c; i++)
            {
                var dat = q.Dequeue();
                link.MsgId++;
                if (link.MsgId >= MaxID)
                    link.MsgId = MinID;
                PackAll(link, dat.dat,dat.type,link.MsgId,time);
            }
            ReSendTimeOut(link, time);
            return c;
        }
        void ReSendTimeOut(KcpData link, Int16 time)
        {
            for (int i = 0; i < link.Msgs.Count; i++)
            {
                int os = time - link.Msgs[i].SendTime;
                if (os < 0)
                    os += 10000;
                if (os >= MsgTimeOut)
                {
                    var msg = link.Msgs[i];
                    SendMsg(link, ref msg, time);
                    link.Msgs[i] = msg;
                }
            }
        }
        BlockInfo<byte> PackInt(int slen)
        {
            int p = slen / 124;
            int r = slen % 124;
            if (r > 0)
                p++;
            int len = p * 4 + slen;
            BlockInfo<byte> block = MemoryRequest(len + 8);
            block.DataCount = len + 8;
            unsafe
            {
                byte* tar = block.Addr;
                tar[0] = 255;
                tar[1] = 255;
                tar[2] = 255;
                tar[3] = 255;
                int o = len + 4;
                tar[o] = 255;
                o++;
                tar[o] = 255;
                o++;
                tar[o] = 255;
                o++;
                tar[o] = 254;
                int s = 0;
                int t = 4;
                for (int j = 0; j < p; j++)
                {
                    int a = 0;
                    int st = t;
                    t += 4;
                    for (int i = 0; i < 31; i++)
                    {
                        byte b = tmpBuffer[s];
                        if (b > 127)
                        {
                            a |= 1 << i;
                            b -= 128;
                        }
                        tar[t] = b;
                        t++; s++;
                        if (s >= slen)
                            break;
                        tar[t] = tmpBuffer[s];
                        t++; s++;
                        if (s >= slen)
                            break;
                        tar[t] = tmpBuffer[s];
                        t++; s++;
                        if (s >= slen)
                            break;
                        tar[t] = tmpBuffer[s];
                        t++; s++;
                        if (s >= slen)
                            break;
                    }
                    tar[st] = (byte)a;
                    a >>= 8;
                    tar[st + 1] = (byte)a;
                    a >>= 8;
                    tar[st + 2] = (byte)a;
                    a >>= 8;
                    tar[st + 3] = (byte)a;
                }
            }
            return block;
        }
        void PackAll(KcpData link, byte[] dat, byte type, UInt16 msgId,Int16 time)
        {
            int len = dat.Length;
            int p = len / FragmentSize;
            int r = len % FragmentSize;
            int all = p;//总计分卷数量
            if (r > 0)
                all++;
            int s = 0;
            unsafe
            {
                fixed(byte* bp=&tmpBuffer[0])
                {
                    for (int i = 0; i < p; i++)
                    {
                        KcpHead* head = (KcpHead*)bp;
                        head->Type = type;
                        head->MsgID = msgId;
                        head->CurPart = (UInt16)i;
                        head->AllPart = (UInt16)all;
                        head->PartLen = (UInt16)FragmentSize;
                        head->Lenth = (UInt32)len;
                        head->Time = time;
                        byte* dp = bp + KcpHead.Size;
                        for (int j = 0; j < FragmentSize; j++)
                        {
                            dp[j] = dat[s];
                            s++;
                        }
                        BlockInfo<byte> block = PackInt(FragmentSize + KcpHead.Size);
                        MsgInfo msg = new MsgInfo();
                        msg.data = block;
                        msg.MsgID = msgId;
                        msg.CurPart = (UInt16)i;
                        msg.CreateTime = time;
                        SendMsg(link, ref msg, time);
                        link.Msgs.Add(msg);
                    }
                    if (r > 0)
                    {
                        KcpHead* head = (KcpHead*)bp;
                        head->Type = type;
                        head->MsgID = msgId;
                        head->CurPart = (UInt16)p;
                        head->AllPart = (UInt16)all;
                        head->PartLen = (UInt16)r;
                        head->Lenth = (UInt32)len;
                        head->Time = time;
                        byte* dp = bp + KcpHead.Size;
                        for (int j = 0; j < r; j++)
                        {
                            dp[j] = dat[s];
                            s++;
                        }
                        BlockInfo<byte> block = PackInt(r + KcpHead.Size);
                        MsgInfo msg = new MsgInfo();
                        msg.data = block;
                        msg.MsgID = msgId;
                        msg.CurPart = (UInt16)p;
                        msg.CreateTime = time;
                        SendMsg(link,ref msg, time);
                        link.Msgs.Add(msg);
                    }
                }
            }
        }
        void SendMsg(KcpData link, ref MsgInfo msg, Int16 time)
        {
            int c = msg.data.DataCount;
            unsafe
            {
                byte* bp = msg.data.Addr;
                for (int i = 0; i < c; i++)
                {
                    tmpBuffer[i] = bp[i];
                }
            }
            msg.SendTime = time;
            msg.SendCount++;
            kcpListener.soc.SendTo(tmpBuffer,c,SocketFlags.None,link.endpPoint);
        }
        static int PackInt(byte[] src, int slen, byte[] tar)
        {
            int p = slen / 124;
            int r = slen % 124;
            if (r > 0)
                p++;
            int len = p * 4 + slen;
            tar[0] = 255;
            tar[1] = 255;
            tar[2] = 255;
            tar[3] = 255;
            int o = len + 4;
            tar[o] = 255;
            o++;
            tar[o] = 255;
            o++;
            tar[o] = 255;
            o++;
            tar[o] = 254;
            int s = 0;
            int t = 4;
            for (int j = 0; j < p; j++)
            {
                int a = 0;
                int st = t;
                t += 4;
                for (int i = 0; i < 31; i++)
                {
                    byte b = src[s];
                    if (b > 127)
                    {
                        a |= 1 << i;
                        b -= 128;
                    }
                    tar[t] = b;
                    t++; s++;
                    if (s >= slen)
                        break;
                    tar[t] = src[s];
                    t++; s++;
                    if (s >= slen)
                        break;
                    tar[t] = src[s];
                    t++; s++;
                    if (s >= slen)
                        break;
                    tar[t] = src[s];
                    t++; s++;
                    if (s >= slen)
                        break;
                }
                tar[st] = (byte)a;
                a >>= 8;
                tar[st + 1] = (byte)a;
                a >>= 8;
                tar[st + 2] = (byte)a;
                a >>= 8;
                tar[st + 3] = (byte)a;
            }
            return len + 8;
        }
        public void Success(ref KcpHead head,KcpData link)
        {
            unsafe
            {
                fixed(byte* bp=&tmp2[0])
                {
                    KcpReturn* hp = (KcpReturn*)bp;
                    hp->Type = EnvelopeType.Success;
                    hp->MsgID = head.MsgID;
                    hp->CurPart = head.CurPart;
                    hp->Time = head.Time;
                }
            }
            int c = PackInt(tmp2, KcpReturn.Size, tmpBuffer);
            kcpListener.soc.SendTo(tmpBuffer, c, SocketFlags.None, link.endpPoint);
        }
        public int UsageMemory { get {
                int len = recvBuffer.UsageMemory;
                if (statesBuffer != null)
                    len += statesBuffer.UsageMemory;
                for (int i = 0; i < Buffer.Count; i++)
                    len += Buffer[i].UsageMemory;
                return len;
            } }
        public int AllMemory { get {
                int len = recvBuffer.AllMemory;
                if (statesBuffer != null)
                    len += statesBuffer.AllMemory;
                for (int i = 0; i < Buffer.Count; i++)
                    len += Buffer[i].AllMemory;
                return len;
            } }
        public int PEMemory { get {
                int len = recvBuffer.PEMemory;
                if (statesBuffer != null)
                    len += statesBuffer.PEMemory;
                for (int i = 0; i < Buffer.Count; i++)
                    len += Buffer[i].PEMemory;
                return len;
            } }
        unsafe static byte[] PackInt(byte* src, int slen)
        {
            int p = slen / 124;
            int r = slen % 124;
            if (r > 0)
                p++;
            int len = p * 4 + slen;
            byte[] tar = new byte[len + 8];
            tar[0] = 255;
            tar[1] = 255;
            tar[2] = 255;
            tar[3] = 255;
            int o = len + 4;
            tar[o] = 255;
            o++;
            tar[o] = 255;
            o++;
            tar[o] = 255;
            o++;
            tar[o] = 254;
            int s = 0;
            int t = 4;
            for (int j = 0; j < p; j++)
            {
                int a = 0;
                int st = t;
                t += 4;
                for (int i = 0; i < 31; i++)
                {
                    byte b = src[s];
                    if (b > 127)
                    {
                        a |= 1 << i;
                        b -= 128;
                    }
                    tar[t] = b;
                    t++; s++;
                    if (s >= slen)
                        break;
                    tar[t] = src[s];
                    t++; s++;
                    if (s >= slen)
                        break;
                    tar[t] = src[s];
                    t++; s++;
                    if (s >= slen)
                        break;
                    tar[t] = src[s];
                    t++; s++;
                    if (s >= slen)
                        break;
                }
                tar[st] = (byte)a;
                a >>= 8;
                tar[st + 1] = (byte)a;
                a >>= 8;
                tar[st + 2] = (byte)a;
                a >>= 8;
                tar[st + 3] = (byte)a;
            }
            return tar;
        }
        public static byte[][] Pack(byte[] dat, byte type, UInt16 msgId, Int16 time)
        {
            int len = dat.Length;
            int p = len / FragmentSize;
            int r = len % FragmentSize;
            int all = p;//总计分卷数量
            if (r > 0)
                all++;
            byte[][] tmp = new byte[all][];
            int s = 0;
            unsafe
            {
                byte* buf = stackalloc byte[1490];
                for (int i = 0; i < p; i++)
                {
                    KcpHead* head = (KcpHead*)buf;
                    head->Type = type;
                    head->MsgID = msgId;
                    head->CurPart = (UInt16)i;
                    head->AllPart = (UInt16)all;
                    head->PartLen = (UInt16)FragmentSize;
                    head->Lenth = (UInt32)len;
                    byte* dp = buf + KcpHead.Size;
                    for (int j = 0; j < FragmentSize; j++)
                    {
                        dp[j] = dat[s];
                        s++;
                    }
                    tmp[i] = PackInt(buf, FragmentSize + KcpHead.Size);
                }
                if (r > 0)
                {
                    KcpHead* head = (KcpHead*)buf;
                    head->Type = type;
                    head->MsgID = msgId;
                    head->CurPart = (UInt16)p;
                    head->AllPart = (UInt16)all;
                    head->PartLen = (UInt16)r;
                    head->Lenth = (UInt32)len;
                    byte* dp = buf + KcpHead.Size;
                    for (int j = 0; j < r; j++)
                    {
                        dp[j] = dat[s];
                        s++;
                    }
                    tmp[p] = PackInt(buf, r + KcpHead.Size);
                }
            }
            return tmp;
        }
    }
}
