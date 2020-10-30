using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    /// <summary>
    /// 用于两个线程无锁交互,a线程写入内容,b线程移除内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueBuffer<T> where T : class
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="len"></param>
        public QueueBuffer(int len = 2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int BufferLenth { get { return mlen; } }
        /// <summary>
        /// 有效内容个数
        /// </summary>
        public int Count
        {
            get
            {
                int a = end - start;
                if (a < 0)
                    a += mlen;
                return a;
            }
        }
        public void Enqueue(T t)
        {
            buffer[end] = t;
            if (end >= mlen - 1)
                end = 0;
            else end++;
        }
        public T Dequeue()
        {
            if (start != end)
            {
                int a = start;
                T t = buffer[a];
                if (start >= mlen - 1)
                    start = 0;
                else start++;
                buffer[a] = null;
                return t;
            }
            return null;
        }
        public void Clear()
        {
            start = end;
            for (int i = 0; i < mlen; i++)
                buffer[i] = null;
        }
    }
    /// <summary>
    /// 用于两个线程无锁交互,a线程写入内容,b线程移除内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueBufferS<T> where T : struct, IDisposable
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="len"></param>
        public QueueBufferS(int len = 2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int BufferLenth { get { return mlen; } }
        /// <summary>
        /// 有效内容个数
        /// </summary>
        public int Count
        {
            get
            {
                int a = end - start;
                if (a < 0)
                    a += mlen;
                return a;
            }
        }
        public bool Enqueue(T t)
        {
            int e = end + 1;
            if (e >= mlen)
                e = 0;
            if (e == start)//缓存已满
                return false;
            buffer[end] = t;
            end = e;
            return true;
        }
        public T Dequeue()
        {
            if (start != end)
            {
                int a = start;
                T t = buffer[a];
                if (start >= mlen - 1)
                    start = 0;
                else start++;
                buffer[a].Dispose();
                return t;
            }
            return new T();
        }
        public void Clear()
        {
            start = end;
        }
    }
}
