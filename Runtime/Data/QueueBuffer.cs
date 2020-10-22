using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    public class QueueBuffer<T> where T : class
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        public QueueBuffer(int len = 2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        public int BufferLenth { get { return mlen; } }
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
    public class QueueBufferS<T> where T : struct, IDisposable
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        public QueueBufferS(int len = 2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        public int BufferLenth { get { return mlen; } }
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
    public class DisorderlyQueueS<T> where T : struct, IDisposable
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        public T this[int index]
        {
            get
            {
                int count = 0;
                if (end >= start)
                    count = end - start;
                else count = end + mlen - start;
                if (index >= count)
                    throw new IndexOutOfRangeException();
                int i = index + start;
                if (i >= mlen)
                    i -= mlen;
                return buffer[i];
            }
        }
        public DisorderlyQueueS(int len = 256)
        {
            buffer = new T[len];
            mlen = len;
        }
        public int BufferLenth { get { return mlen; } }
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
        public bool Add(T t)
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
        public bool AddRange(T[] t)
        {
            int r = mlen - Count;
            int l = t.Length;
            if (r < l)
                return false;
            for (int i = 0; i < l; i++)
            {
                buffer[end] = t[i];
                end++;
                if (end >= mlen)
                    end = 0;
            }
            return true;
        }
        public void RemoveAt(int index)
        {
            if (start != end)
            {
                int count = 0;
                if (end >= start)
                    count = end - start;
                else count = end + mlen - start;
                if (index >= count)
                    throw new IndexOutOfRangeException();
                int i = index + start;
                if (i >= mlen)
                    i -= mlen;
                buffer[i].Dispose();
                buffer[i] = buffer[start];
                if (start >= mlen - 1)
                    start = 0;
                else start++;
            }
        }
        public void Clear()
        {
            start = end;
        }
    }
}
