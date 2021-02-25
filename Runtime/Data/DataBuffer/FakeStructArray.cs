using System;
using System.Runtime.InteropServices;


namespace huqiang.Data
{
    public class FakeStructArray : ToBytes
    {
        IntPtr ptr;
        /// <summary>
        /// 本结构体的非托管内存地址
        /// </summary>
        public unsafe byte* ip;
        internal Int32 m_size;
        int m_len;
        int all_len;
        /// <summary>
        /// 数据缓存
        /// </summary>
        public DataBuffer buffer;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="size">元素个数</param>
        /// <param name="len">数组长度</param>
        public FakeStructArray(DataBuffer db, int size, int len)
        {
            int max = size * len;
            all_len = max * 4;
            ptr = Marshal.AllocHGlobal(all_len);
            unsafe
            {
                ip = (byte*)ptr;
                Int32* p = (Int32*)ip;
                for (int i = 0; i < max; i++)
                {
                    *p = 0;
                    p++;
                }
            }
            m_size = size;
            m_len = len;
            buffer = db;
        }
        /// <summary>
        /// 构造函数,复制目标地址的数据
        /// </summary>
        /// <param name="db">数据缓存</param>
        /// <param name="size">元素尺寸</param>
        /// <param name="len">数据长度</param>
        /// <param name="point">内存地址</param>
        public unsafe FakeStructArray(DataBuffer db, int size, int len, Int32* point)
        {
            int max = size * len;
            all_len = max * 4;
            ptr = Marshal.AllocHGlobal(all_len);
            unsafe
            {
                ip = (byte*)ptr;
                Int32* p = (Int32*)ip;
                for (int i = 0; i < max; i++)
                {
                    *p = *point;
                    point++;
                    p++;
                }
            }
            m_size = size;
            m_len = len;
            buffer = db;
        }
        /// <summary>
        /// 结构体尺寸
        /// </summary>
        public int StructSize { get { return m_size; } }
        /// <summary>
        /// 数组长度
        /// </summary>
        public Int32 Length { get { return m_len; } }
        /// <summary>
        /// 索引器,获取元素地址
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <returns></returns>
        public unsafe byte* this[int index]
        {
            get { return ip + index * m_size * 4; }
        }
        public Int32 this[int index, int os]
        {
            get { return GetInt32(index, os); }
            set { SetInt32(index, os, value); }
        }

        /// <summary>
        /// 获取整数
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe Int32 GetInt32(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(Int32*)(ip + o);
        }
        /// <summary>
        /// 设置一个整数值
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <param name="value">整数值</param>
        public unsafe void SetInt32(int index, int offset, Int32 value)
        {
            int o = (index * m_size + offset) * 4;
            *(Int32*)(ip + o) = value;
        }
        /// <summary>
        /// 获取长整数
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe Int64 GetInt64(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(Int64*)(ip + o);
        }
        /// <summary>
        /// 设置一个长整数值
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <param name="value">长整数值</param>
        public unsafe void SetInt64(int index, int offset, Int64 value)
        {
            int o = (index * m_size + offset) * 4;
            *(Int64*)(ip + o) = value;
        }
        /// <summary>
        /// 获取浮点数
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe float GetFloat(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(float*)(ip + o);
        }
        /// <summary>
        /// 设置一个浮点数值
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <param name="value">浮点数值</param>
        public unsafe void SetFloat(int index, int offset, float value)
        {
            int o = (index * m_size + offset) * 4;
            *(float*)(ip + o) = value;
        }
        /// <summary>
        /// 获取双浮点数
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe double GetDouble(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(double*)(ip + o);
        }
        /// <summary>
        /// 设置一个双浮点数值
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <param name="value">双浮点数值</param>
        public unsafe void SetDouble(int index, int offset, double value)
        {
            int o = (index * m_size + offset) * 4;
            *(double*)(ip + o) = value;
        }
        /// <summary>
        /// 设置一个数据对象,数据类型必须为DataType中任意一种
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <param name="dat">数据对象</param>
        public unsafe void SetData(int index, int offset, object dat)
        {
            int o = (index * m_size + offset) * 4;
            Int32* a = (Int32*)(ip + o);
            buffer.RemoveData(*a);
            *a = buffer.AddData(dat);
        }
        /// <summary>
        /// 获取一个数据对象,数据类型必须为DataType中任意一种
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe object GetData(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            Int32* a = (Int32*)(ip + o);
            return buffer.GetData(*a);
        }
        ~FakeStructArray()
        {
            Marshal.FreeHGlobal(ptr);
        }
        /// <summary>
        /// 将假结构体数组中的数据导出位byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] buf = new byte[all_len];
            Marshal.Copy(ptr, buf, 0, all_len);

            return buf;
        }
        /// <summary>
        /// 从目标结构体中复制数据
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="tar">目标结构体地址</param>
        public unsafe void ReadFromStruct(int index, void* tar)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ptr;
            p += index * m_size;
            for (int i = 0; i < m_size; i++)
            {
                *p = *t;
                t++;
                p++;
            }
        }
        /// <summary>
        /// 将数据写入目标结构体
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="tar">目标结构体地址</param>
        public unsafe void WitreToStruct(int index, void* tar)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ptr;
            p += index * m_size;
            for (int i = 0; i < m_size; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        /// <summary>
        /// 从目标结构体中复制数据
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="tar">目标结构地址</param>
        /// <param name="start">本结构体开始位置</param>
        /// <param name="size">复制数据长度</param>
        public unsafe void ReadFromStruct(int index, void* tar, int start, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)(ptr);
            p += index * m_size;
            p += start;
            for (int i = 0; i < size; i++)
            {
                *p = *t;
                t++;
                p++;
            }
        }
        /// <summary>
        /// 将数据写入目标结构体
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="tar">目标结构地址</param>
        /// <param name="start">本结构体开始位置</param>
        /// <param name="size">复制数据长度</param>
        public unsafe void WitreToStruct(int index, void* tar, int start, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ptr;
            p += index * m_size;
            p += start;
            for (int i = 0; i < size; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        /// <summary>
        /// 从内存地址中复制数据
        /// </summary>
        /// <param name="p">目标地址</param>
        public unsafe void ReadFromArray(IntPtr p)
        {
            Int32* sp = (Int32*)ptr;
            Int32* tp = (Int32*)p;
            int len = all_len / 4;
            for (int i = 0; i < len; i++)
            {
                *sp = *tp;
                sp++;
                tp++;
            }
        }
        /// <summary>
        /// 将数据写入到内存地址中
        /// </summary>
        /// <param name="p">目标地址</param>
        public unsafe void WriteToArray(IntPtr p)
        {
            Int32* sp = (Int32*)ptr;
            Int32* tp = (Int32*)p;
            int len = all_len / 4;
            for (int i = 0; i < len; i++)
            {
                *tp = *sp;
                sp++;
                tp++;
            }
        }
        /// <summary>
        /// 设置一个数据对象,数据类型必须为DataType中任意一种
        /// </summary>
        /// <param name="addr">内存地址</param>
        /// <param name="dat">数据对象</param>
        public unsafe void SetData(int* addr, object dat)
        {
            int a = (int)addr - (int)ip;
            if (a < 0 | a >= all_len)//超过界限
                return;
            buffer.RemoveData(*addr);
            *addr = buffer.AddData(dat);
        }
        /// <summary>
        /// 获取一个数据对象,数据类型必须为DataType中任意一种
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="addr">内存地址</param>
        /// <returns></returns>
        public unsafe T GetData<T>(int* addr) where T : class
        {
            int a = (int)addr - (int)ip;
            if (a < 0 | a >= all_len)//超过界限
                return null;
            return buffer.GetData(*addr) as T;
        }
        /// <summary>
        /// 获取一个数据对象,数据类型必须为DataType中任意一种
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe T GetData<T>(int index, int offset) where T : class
        {
            int o = (index * m_size + offset) * 4;
            int os = *(Int32*)(ip + o);
            return buffer.GetData(os) as T;
        }
        /// <summary>
        /// 设置一个非托管结构体数组对象
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <param name="dat">数组对象</param>
        public unsafe void AddArray<T>(int index, int offset, T[] dat) where T : unmanaged
        {
            int o = (index * m_size + offset) * 4;
            Int32* a = (Int32*)(ip + o);
            buffer.RemoveData(*a);
            *a = buffer.AddArray<T>(dat);
        }
        /// <summary>
        /// 设置一个非托管结构体数组对象
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="index">元素索引</param>
        /// <param name="offset">子元素偏移</param>
        /// <returns></returns>
        public unsafe T[] GetArray<T>(int index, int offset) where T : unmanaged
        {
            int o = (index * m_size + offset) * 4;
            int os = *(Int32*)(ip + o);
            if (os >= 0)
                return buffer.GetArray<T>(os);
            return null;
        }

        #region //提高容错率,做了范围鉴定,需要速度可以使用上面的函数
        int row;
        int offset;
        /// <summary>
        /// 设置位置索引
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="os">子元素偏移</param>
        /// <returns></returns>
        public bool Seek(int index, int os = 0)
        {
            if (index < 0)
                return false;
            if (index >= m_len)
                return false;
            if (os < 0)
                return false;
            if (os >= m_size)
                return false;
            row = index;
            offset = os;
            return true;
        }
        /// <summary>
        /// 写入一个整数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteInt(int value)
        {
            if (offset < m_size)
            {
                SetInt32(row, offset, value);
                offset++;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 写入一个浮点数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteFloat(float value)
        {
            if (offset < m_size)
            {
                SetFloat(row, offset, value);
                offset++;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 写入一个长整数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteInt64(Int64 value)
        {
            if (offset + 1 < m_size)
            {
                SetInt64(row, offset, value);
                offset += 2;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 写入一个双浮点数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteDouble(double value)
        {
            if (offset + 1 < m_size)
            {
                SetDouble(row, offset, value);
                offset += 2;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 写入一个对象数据,数据类型必须为DataType中任意一种
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteData(object value)
        {
            if (offset < m_size)
            {
                SetData(row, offset, value);
                offset++;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 写入一个非托管结构体数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteArray<T>(T[] value) where T : unmanaged
        {
            if (offset < m_size)
            {
                AddArray<T>(row, offset, value);
                offset++;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 读取一个整数
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            if (offset < Length)
            {
                var value = GetInt32(row, offset);
                offset++;
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 读取一个浮点数
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            if (offset < Length)
            {
                var value = GetFloat(row, offset);
                offset++;
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 读取一个长整数
        /// </summary>
        /// <returns></returns>
        public Int64 ReadInt64()
        {
            if (offset + 1 < Length)
            {
                var value = GetInt64(row, offset);
                offset += 2;
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 读取一个双浮点数
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            if (offset + 1 < Length)
            {
                var value = GetDouble(row, offset);
                offset += 2;
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 读取一个数据对象,数据类型必须为DataType中任意一种
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <returns></returns>
        public T ReadData<T>() where T : class
        {
            if (offset < Length)
            {
                var value = GetData<T>(row, offset);
                offset++;
                return value;
            }
            return null;
        }
        /// <summary>
        /// 读取一个非托管结构体数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public T[] ReadArray<T>() where T : unmanaged
        {
            if (offset < Length)
            {
                var value = GetArray<T>(row, offset);
                offset++;
                return value;
            }
            return null;
        }
        #endregion
    }
}
