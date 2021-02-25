using huqiang.Data;
using System;
using System.Runtime.InteropServices;


namespace huqiang.Communication
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MsgReturn
    {
        /// <summary>
        /// 数据压缩类型
        /// </summary>
        public byte Type;
        /// <summary>
        /// 此消息的id
        /// </summary>
        public UInt16 MsgID;
        /// <summary>
        /// 此消息的某个分卷
        /// </summary>
        public UInt16 CurPart;
        /// <summary>
        /// 此消息发送的时间戳
        /// </summary>
        public Int16 Time;
        public static unsafe int Size = sizeof(MsgReturn);
    }

    public struct BlockData : IDisposable
    {
        public byte type;
        public BlockInfo dat;

        public void Dispose()
        {
        }
    }
    internal struct MsgInfo : IDisposable
    {
        /// <summary>
        /// 此消息的id 
        /// </summary>
        public UInt16 MsgID;//2
        /// <summary>
        /// 此消息的某个分卷
        /// </summary>
        public UInt16 CurPart;//2
        /// <summary>
        /// 创建的时间
        /// </summary>
        public Int16 CreateTime;
        /// <summary>
        /// 上一次发送的时间
        /// </summary>
        public Int16 SendTime;
        /// <summary>
        /// 发送的次数
        /// </summary>
        public int SendCount;
        /// <summary>
        /// 数据
        /// </summary>
        public BlockInfo data;

        public void Dispose()
        {
        }
    }

    public class BroadcastMsg : IDisposable
    {
        /// <summary>
        /// 此消息的id
        /// </summary>
        public UInt16 MsgID;//
        /// <summary>
        /// 上一次发送的时间
        /// </summary>
        public long SendTime;
        /// <summary>
        /// 发送的次数
        /// </summary>
        public int SendCount;
        /// <summary>
        /// 数据
        /// </summary>
        public BlockInfo<BlockInfo> data;

        public void Dispose()
        {
            int c = data.DataCount;
            for (int i = 0; i < c; i++)
            {
                unsafe
                {
                    ((BlockInfo*)data.Addr)[i].Release();
                }
            }
            data.Release();
        }
    }
}
