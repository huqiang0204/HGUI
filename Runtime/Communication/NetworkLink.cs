using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace huqiang
{
    /// <summary>
    /// 网络连接
    /// </summary>
    public class NetworkLink
    {
        /// <summary>
        /// 远程ip地址
        /// </summary>
        public int ip;
        /// <summary>
        /// 远程端口
        /// </summary>
        public int port;
        /// <summary>
        /// 所在线程中的索引
        /// </summary>
        public int Index;
        /// <summary>
        /// 线程的索引
        /// </summary>
        public int buffIndex;
        /// <summary>
        /// 用户ID
        /// </summary>
        public Int64 id;
        /// <summary>
        /// 远程地址
        /// </summary>
        public IPEndPoint endpPoint;
        /// <summary>
        /// 连接状态
        /// </summary>
        internal bool _connect;
        /// <summary>
        /// 用户触发销毁的时间,由KcpListener统一销毁,防止线程资源冲突
        /// </summary>
        public long RecyclingTime;
        public bool Connected { get { return _connect; } }
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="now"></param>
        public virtual void Recive(long now)
        {
        }
        /// <summary>
        /// 添加一个共有消息
        /// </summary>
        /// <param name="msgs"></param>
        public virtual void AddMsg(MsgInfo2[] msgs)
        {
        }
        /// <summary>
        /// 释放非托管内存资源
        /// </summary>
        internal virtual void FreeMemory()
        {
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
