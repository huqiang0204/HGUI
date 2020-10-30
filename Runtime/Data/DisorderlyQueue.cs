using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    /// <summary>
    /// 用于两个线程无锁交互,a线程写入内容,b线程可以乱序移除内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisorderlyQueue<T> where T : class
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="len"></param>
        public DisorderlyQueue(int len = 256)
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
        /// <summary>
        /// 添加一个内容
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 添加一组内容
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 移除某个内容
        /// </summary>
        /// <param name="obj">实列对象</param>
        public void Remove(T obj)
        {
            if (start == end)
                return;
            int c = end - start;
            if (c < 0)
                c += mlen;
            int s = start;
            for (int i = 0; i < c; i++)
            {
                if (buffer[s] == obj)
                {
                    buffer[s] = buffer[start];
                    buffer[s] = null;
                    if (start + 1 >= mlen)//这样判定防止另一个线程同时访问了start
                        start = 0;
                    else
                        start++;
                    return;
                }
                s++;
            }
        }
        /// <summary>
        /// 移除某个内容
        /// </summary>
        /// <param name="index">索引</param>
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
                buffer[i] = null;
                buffer[i] = buffer[start];
                if (start >= mlen - 1)
                    start = 0;
                else start++;
            }
        }
        /// <summary>
        /// 查询某个实例
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public T Find(Func<T, bool> func)
        {
            if (start == end)
                return null;
            int c = end - start;
            if (c < 0)
                c += mlen;
            int s = start;
            for (int i = 0; i < c; i++)
            {
                if (func(buffer[s]))
                    return buffer[s];
                s++;
            }
            return null;
        }
        /// <summary>
        /// 清除所有内容
        /// </summary>
        public void Clear()
        {
            int c = end - start;
            if (c < 0)
                c += mlen;
            for (int i = 0; i < c; i++)
            {
                buffer[start] = null;
                start++;
                if (start >= mlen)
                    start = 0;
            }
            start = end;
        }
    }
    /// <summary>
    /// 用于两个线程无锁交互,a线程写入内容,b线程可以乱序移除内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisorderlyQueueS<T> where T : struct, IDisposable
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="len"></param>
        public DisorderlyQueueS(int len = 256)
        {
            buffer = new T[len];
            mlen = len;
        }
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
        /// <summary>
        /// 添加一个内容
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 添加一组内容
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 移除某个内容
        /// </summary>
        /// <param name="index">索引</param>
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
        /// <summary>
        /// 清除所有内容
        /// </summary>
        public void Clear()
        {
            int c = end - start;
            if (c < 0)
                c += mlen;
            for (int i = 0; i < c; i++)
            {
                buffer[start].Dispose();
                start++;
                if (start >= mlen)
                    start = 0;
            }
            start = end;
        }
    }
}
