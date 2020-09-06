using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace huqiang
{
    public class NetworkLink
    {
        public int ip;
        public int port;
        public int Index;
        public int buffIndex;
        public Int64 id;
        public IPEndPoint endpPoint;
        internal bool _connect;
        public long RecyclingTime;
        public bool Connected { get { return _connect; } }
        public virtual void Recive(long now)
        {
        }
        public virtual void AddMsg(byte[][] dat, long now,UInt16 msgID)
        {
        }
        internal virtual void FreeMemory()
        {
        }
        public virtual void Dispose()
        {
        }
    }
}
