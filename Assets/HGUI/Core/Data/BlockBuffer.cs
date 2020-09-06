using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public struct BlockInfo<T> where T : unmanaged
    {
        public int DataCount;
        int Index;
        int Length;
        int bufID;
        int os;
        public unsafe T* Addr
        {
            get
            {
                var add = BlockBuffer.buffers[bufID];
                if (add != null)
                    return (T*)(add.Addr + os);
                return (T*)0;
            }
        }
        public int Offset { get => Index; }
        public int Size { get => Length; }
        public BlockInfo(int addr, int index, int len, int area)
        {
            bufID = addr;
            Index = index;
            Length = len;
            DataCount = 0;
            os = index * area;
        }
        public void Release()
        {
            if (Length == 0)
                return;
            var add = BlockBuffer.buffers[bufID];
            if (add != null)
                add.Release(Index, Length);
            Length = 0;
        }
        public void Zero()
        {
            var add = BlockBuffer.buffers[bufID];
            if (add != null)
            {
                add.Zero(Index, Length);
            }
        }
        public void Clear()
        {
            Length = 0;
        }
    }
    public class BlockBuffer
    {
        internal static BlockBuffer[] buffers = new BlockBuffer[1024];
        public int eSize { get; protected set; }
        protected IntPtr ptr;
        protected int pe;
        public IntPtr Addr => ptr + pe;
        public virtual void Release(int offset, int size)
        {
        }
        public virtual void Zero(int offset, int size)
        {

        }
    }
    public class BlockBuffer<T> : BlockBuffer, IDisposable where T : unmanaged
    {
        int blockSize;
        int dataLength;
        int allLength;

        int usage;
        int ID;
        public int AllMemory { get => allLength; }
        public int UsageMemory { get => usage * blockSize * eSize; }
        public int PEMemory { get => pe; }
        /// <summary>
        /// 剩余容量
        /// </summary>
        public int RemainBlock { get => pe - usage; }
        public unsafe BlockBuffer(int block = 32, int len = 32)
        {
            eSize = sizeof(T);
            pe = len;
            blockSize = block;
            dataLength = len * blockSize * eSize;
            allLength = pe + dataLength;
            ptr = Marshal.AllocHGlobal(allLength);
            byte* bp = (byte*)ptr;
            for (int i = 0; i < pe; i++)//填0
                bp[i] = 0;
            lock (buffers)
            {
                var buf = buffers;
                for (int i = 1; i < buf.Length; i++)
                {
                    if (buf[i] == null)
                    {
                        ID = i;
                        buf[i] = this;
                        break;
                    }
                }
            }
        }
        public unsafe BlockInfo<T> RegNew(int len)
        {
            byte* bp = (byte*)ptr;
            int block = len / blockSize;
            if (len % blockSize > 0)
                block++;
            int c = 0;
            int index = 0;
            for (int i = 0; i < pe; i++)
            {
                if (bp[i] == 0)
                {
                    c++;
                    if (c >= block)
                    {
                        index = i - block + 1;
                        break;
                    }
                }
                else c = 0;
            }
            int o = index;
            for (int i = 0; i < block; i++)
            {
                bp[o] = 1;
                o++;
            }
            len = block * blockSize;
            usage += block;
            return new BlockInfo<T>(ID, index, len, blockSize * eSize);
        }
        public unsafe override void Release(int offset, int size)
        {
            int block = size / blockSize;
            byte* bp = (byte*)ptr;
            for (int i = 0; i < block; i++)
            {
                bp[offset] = 0;
                offset++;
            }
            usage -= block;
        }
        public override void Zero(int offset, int size)
        {
            int area = blockSize * eSize;
            int os = offset * area + pe;
            int len = area * size;
            int block = size / blockSize;
            unsafe
            {
                byte* bp = (byte*)ptr;
                for (int i = 0; i < len; i++)
                {
                    bp[os] = 0;
                    os++;
                }
            }
        }
        public unsafe bool RegNew(ref BlockInfo<T> blockInfo, int len)
        {
            byte* bp = (byte*)ptr;
            int block = len / blockSize;
            if (len % blockSize > 0)
                block++;
            int c = 0;
            int index = 0;
            for (int i = 0; i < pe; i++)
            {
                if (bp[i] == 0)
                {
                    c++;
                    if (c >= block)
                    {
                        index = i - block + 1;
                        goto lable;
                    }
                }
                else c = 0;
            }
            return false;
        lable:;
            int o = index;
            for (int i = 0; i < block; i++)
            {
                bp[o] = 1;
                o++;
            }
            len = block * blockSize;
            blockInfo = new BlockInfo<T>(ID, index, len, blockSize * eSize);
            usage += block;
            return true;
        }
        /// <summary>
        /// 容量不够时扩容
        /// </summary>
        public unsafe void Expansion()
        {
            int tl = pe * 2;
            int dl = tl * blockSize * eSize;
            int al = tl + dl;
            IntPtr pl = Marshal.AllocHGlobal(al);
            byte* src = (byte*)ptr;
            byte* tar = (byte*)ptr;
            for (int i = 0; i < pe; i++)
                tar[i] = src[i];
            for (int i = pe; i < tl; i++)//填0
                tar[i] = 0;
            int ts = tl;
            int ss = pe;
            for (int i = 0; i < dataLength; i++)
            {
                tar[ts] = src[ss];
                ts++;
                ss++;
            }
            Marshal.FreeHGlobal(ptr);
            pe = tl;
            ptr = pl;
            dataLength = dl;
            allLength = al;
        }
        public void Dispose()
        {
            Marshal.FreeHGlobal(ptr);
            buffers[ID] = null;
        }
    }
}
