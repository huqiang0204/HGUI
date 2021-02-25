using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang.Communication
{
    internal struct NetworkContent
    {
        /// <summary>
        /// 内容状态
        /// </summary>
        public int State;
        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadID;
        /// <summary>
        /// 消息ID
        /// </summary>
        public int MsgID;
        /// <summary>
        /// 需要编码的数据
        /// </summary>
        public byte[] dat;
    }
    public class NetworkLink
    {
        NetworkContent[] ncs;
        int ncLength;
        public int ip;
        public int port;
        public int buffIndex;
        public Int64 id;
        public IPEndPoint endpPoint;
        internal bool _connect;
        public long RecyclingTime;
        /// <summary>
        /// 接收的数据长度
        /// </summary>
        protected int RecvLen;
        /// <summary>
        /// 接收到缓存数据
        /// </summary>
        internal byte[] RecvBuffer = new byte[32768];
        public bool Connected { get { return _connect; } }
        public NetworkLink()
        {
            ncLength = 32;
            ncs = new NetworkContent[ncLength];
        }
        public NetworkLink(int size =32)
        {
            ncs = new NetworkContent[size];
            ncLength = size;
        }
        public virtual void Recive(byte[] buff, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (RecvLen >= 32768)
                {
                    Console.WriteLine("buffer overflow");
                    return;
                }
                RecvBuffer[RecvLen] = buff[i];
                RecvLen++;
            }
        }
        internal virtual void Recive(long now)
        {
        }
        internal virtual void FreeMemory()
        {
        }
        public virtual void Dispose()
        {
        }
        /// <summary>
        /// 缓存默认大小为32，注意每帧添加的数据不要超过32
        /// </summary>
        /// <param name="id">线程ID</param>
        /// <param name="dat">如果使用广播消息，此处为空，并在第三个参数上填上广播消息的ID</param>
        /// <param name="spin">广播消息ID</param>
        /// <returns></returns>
        public virtual bool Post(int id, byte[] dat, int msgID, int spin = 16)
        {
            for (int i = 0; i < ncLength; i++)
            {
                if (ncs[i].State == 0)
                {
                    ncs[i].State = 1;
                    ncs[i].ThreadID = id;
                    for (int j = 0; j < spin; j++)
                        if (ncs[i].ThreadID != id)
                            goto label;
                    ncs[i].MsgID = msgID;
                    ncs[i].dat = dat;
                    ncs[i].State = 2;
                    return true;
                }
            label:;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">线程ID</param>
        /// <param name="dat">如果使用广播消息，此处为空，并在第三个参数上填上广播消息的ID</param>
        /// <param name="msgID">广播消息ID</param>
        public virtual void FrocePost(int id,byte[] dat, int msgID = 0)
        {
            var tick = DateTime.Now.Ticks;
            while (true)
            {
                if (Post(id, dat, msgID))
                    return;
                if (DateTime.Now.Ticks - tick > 100000)//超过10毫秒都无法添加数据
                {
                    Console.WriteLine("缓存已满，目标线程超过10毫秒都没有处理缓存中的任务");
                    return;
                }
            }
        }
        internal bool Get(ref NetworkContent content)
        {
            for (int i = 0; i < ncLength; i++)
            {
                if (ncs[i].State == 2)
                {
                    content = ncs[i];
                    ncs[i].dat = null;
                    ncs[i].MsgID = 0;
                    ncs[i].State = 0;
                    return true;
                }
            }
            return false;
        }
    }
}
