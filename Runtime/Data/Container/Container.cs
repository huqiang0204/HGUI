using System;


namespace huqiang.Data
{
    /// <summary>
    /// 泛型容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Container<T> where T : class
    {
        int top = 0;
        T[] buffer;
        int mlen;
        /// <summary>
        /// 当前容器中的内容数量
        /// </summary>
        public int Count { get { return top; } }
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
                if (index >= top)
                    return null;
                return buffer[index];
            }
            set
            {
                if (index < 0)
                    return;
                if (index >= top)
                    return;
                buffer[index] = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="len"></param>
        public Container(int len = 2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="data"></param>
        public void Add(T data)
        {
            buffer[top] = data;
            top++;
        }
        /// <summary>
        /// 移除一个对象
        /// </summary>
        /// <param name="index">对象索引</param>
        public void RemoveAt(int index)
        {
            if (index < 0)
                return;
            if (index >= top)
                return;
            top--;
            buffer[index] = buffer[top];
            buffer[top] = null;
        }
        /// <summary>
        /// 移除一个对象
        /// </summary>
        /// <param name="data"></param>
        public void Remove(T data)
        {
            for (int i = 0; i < top; i++)
            {
                if (buffer[i] == data)
                {
                    top--;
                    buffer[i] = buffer[top];
                    buffer[top] = null;
                    return;
                }
            }
        }
        /// <summary>
        /// 根据条件移除一个对象
        /// </summary>
        /// <param name="action">委托</param>
        public void Remove(Func<T, bool> action)
        {
            if (action == null)
                return;
            for (int i = 0; i < top; i++)
            {
                if (action(buffer[i]))
                {
                    top--;
                    buffer[i] = buffer[top];
                    buffer[top] = null;
                    return;
                }
            }
        }
        /// <summary>
        /// 根据条件查询对象的索引
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public int FindIndex(Func<T, bool> action)
        {
            if (action == null)
                return -1;
            for (int i = 0; i < top; i++)
                if (action(buffer[i]))
                    return i;
            return -1;
        }
        /// <summary>
        /// 根据条件查询某个对象
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public T Find(Func<T, bool> action)
        {
            if (action == null)
                return null;
            for (int i = 0; i < top; i++)
                if (action(buffer[i]))
                    return buffer[i];
            return null;
        }
        /// <summary>
        /// 根据条件交换某个对象
        /// </summary>
        /// <param name="action"></param>
        /// <param name="index">交换的目标索引</param>
        /// <returns></returns>
        public T FindAndSwap(Func<T, bool> action, int index)
        {
            if (action == null)
                return null;
            for (int i = 0; i < top; i++)
                if (action(buffer[i]))
                {
                    var t = buffer[index];
                    var r = buffer[i];
                    buffer[i] = t;
                    buffer[index] = r;
                    return r;
                }
            return null;
        }
        /// <summary>
        /// 交换两个对象
        /// </summary>
        /// <param name="source">源对象索引</param>
        /// <param name="target">目标对象索引</param>
        public void Swap(int source, int target)
        {
            var s = buffer[source];
            var t = buffer[target];
            buffer[source] = t;
            buffer[target] = s;
        }
        /// <summary>
        /// 清除所有对象
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < top; i++)
            {
                buffer[i] = null;
            }
            top = 0;
        }
        /// <summary>
        /// 根据条件进行排序
        /// </summary>
        /// <param name="com">比较a,b返回真则b排在前面</param>
        public void Sort(Func<T, T, bool> com)
        {
            for (int i = 0; i < top; i++)
            {
                var t = buffer[i];
                int s = i;
                for (int j = i + 1; j < top; j++)
                {
                    if (com(t, buffer[j]))
                    {
                        t = buffer[j];
                        s = j;
                    }
                }
                var u = buffer[i];
                buffer[i] = t;
                buffer[s] = u;
            }
        }
    }
}
