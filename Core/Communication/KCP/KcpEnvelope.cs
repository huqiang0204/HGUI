using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace huqiang
{
    public struct KcpEnvelopeData
    {
        public KcpHead Head;
        public byte[] Data;
    }
    public class KcpEnvelope 
    {
        public UInt16 MinID = 60000;
        public UInt16 MaxID = 64000;
        static int FragmentSize = (1472 - 8 - 15) - 12 * 4;//包头4字节,标志1字节,包尾4字节,数据头14字节,12个节点,每个节点4字节=1399
        byte[] buf = new byte[1490];
        public List<KcpEnvelopeData> datas = new List<KcpEnvelopeData>();
        public UInt16 MsgId = 60000;
        static byte[] PackInt(byte[] src, int slen)
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
        public void PackAll(byte type,byte[] dat)
        {
            MsgId++;
            if (MsgId > MaxID)
                MsgId = MinID;
            datas.Clear();
            int len = dat.Length;
            int p = len / FragmentSize;
            int r = len % FragmentSize;
            int all = p;//总计分卷数量
            if (r > 0)
                all++;
            int s = 0;
            unsafe
            {
                fixed (byte* bp = &buf[0])
                {
                    for (int i = 0; i < p; i++)
                    {
                        KcpHead* head = (KcpHead*)bp;
                        head->Type = type;
                        head->MsgID = MsgId;
                        head->CurPart = (UInt16)i;
                        head->AllPart = (UInt16)all;
                        head->PartLen = (UInt16)FragmentSize;
                        head->Lenth = (UInt32)len;
                        byte* dp = bp + KcpHead.Size;
                        for (int j = 0; j < FragmentSize; j++)
                        {
                            dp[j] = dat[s];
                            s++;
                        }
                        KcpEnvelopeData ked = new KcpEnvelopeData();
                        ked.Head = *head;
                        ked.Data =  PackInt(buf, FragmentSize + KcpHead.Size);
                        datas.Add(ked);
                    }
                    if (r > 0)
                    {
                        KcpHead* head = (KcpHead*)bp;
                        head->Type = type;
                        head->MsgID = MsgId;
                        head->CurPart = (UInt16)p;
                        head->AllPart = (UInt16)all;
                        head->PartLen = (UInt16)r;
                        head->Lenth = (UInt32)len;
                        byte* dp = bp + KcpHead.Size;
                        for (int j = 0; j < r; j++)
                        {
                            dp[j] = dat[s];
                            s++;
                        }
                        KcpEnvelopeData ked = new KcpEnvelopeData();
                        ked.Head = *head;
                        ked.Data = PackInt(buf, r + KcpHead.Size);
                        datas.Add(ked);
                    }
                }
            }
        }
    }
}