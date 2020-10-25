using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    /// <summary>
    /// 循环使用缓存器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoopBuffer<T> where T : class
    {
        T[] buffer;
        int mlen;
        int point;
        /// <summary>
        /// 缓存长度
        /// </summary>
        public int Lenth { get { return mlen; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="len"></param>
        public LoopBuffer(int len)
        {
            buffer = new T[len];
            mlen = len;
            point = 0;
        }
        /// <summary>
        /// 推入一个数据
        /// </summary>
        /// <param name="t"></param>
        public void Push(T t)
        {
            buffer[point]=t;
            if (point >= mlen - 1)
                point = 0;
            else point++;
        }
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    return null;
                if (index >= mlen)
                    return null;
                index += point;
                if (index >= mlen)
                    index -= mlen;
                return buffer[index];
            }
            set
            {
                if (index < 0)
                    return;
                if (index >= mlen)
                    return;
                index += point;
                if (index >= mlen)
                    index -= mlen;
                buffer[index] = value;
            }
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < mlen; i++)
                buffer[i] = null;
        }
    }
}
