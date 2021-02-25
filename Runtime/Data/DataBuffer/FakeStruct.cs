using System;
using System.Runtime.InteropServices;


namespace huqiang.Data
{
    /// <summary>
    /// 假结构体
    /// </summary>
    public class FakeStruct : ToBytes
    {
        /// <summary>
        /// 从目标结构体中复制数据
        /// </summary>
        /// <param name="tar">目标结构地址</param>
        public unsafe void ReadFromStruct(void* tar)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ip;
            for (int i = 0; i < element; i++)
            {
                *p = *t;
                t++;
                p++;
            }
        }
        /// <summary>
        /// 将数据写入目标结构体
        /// </summary>
        /// <param name="tar">目标结构体地址</param>
        public unsafe void WitreToStruct(void* tar)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ip;
            for (int i = 0; i < element; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        /// <summary>
        /// 从目标结构体中复制数据
        /// </summary>
        /// <param name="tar">目标结构地址</param>
        /// <param name="start">本结构体开始位置</param>
        /// <param name="size">复制数据长度</param>
        public unsafe void ReadFromStruct(void* tar, int start, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ip;
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
        /// <param name="tar">目标结构地址</param>
        /// <param name="start">本结构体开始位置</param>
        /// <param name="size">复制数据长度</param>
        public unsafe void WitreToStruct(void* tar, int start, int size)
        {
            Int32* t = (Int32*)tar;
            Int32* p = (Int32*)ip;
            p += start;
            for (int i = 0; i < size; i++)
            {
                *t = *p;
                t++;
                p++;
            }
        }
        IntPtr ptr;
        /// <summary>
        /// 本结构体的非托管内存地址
        /// </summary>
        public unsafe byte* ip;
        int msize;
        int element;
        /// <summary>
        /// 元素长度
        /// </summary>
        public int Length { get => element; }
        /// <summary>
        /// 数据缓存器
        /// </summary>
        public DataBuffer buffer;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db">数据缓存器</param>
        /// <param name="size">元素个数</param>
        public FakeStruct(DataBuffer db, int size)
        {
            element = size;
            msize = size * 4;
            if (size > 0)
            {
                ptr = Marshal.AllocHGlobal(msize);
                unsafe
                {
                    ip = (byte*)ptr;
                    Int32* p = (Int32*)ptr;
                    for (int i = 0; i < size; i++)
                    {
                        *p = 0;
                        p++;
                    }
                }
            }
            buffer = db;
        }
        /// <summary>
        /// 构造函数,复制目标地址的数据
        /// </summary>
        /// <param name="db">数据缓存器</param>
        /// <param name="size">元素个数</param>
        /// <param name="point">源数据地址</param>
        public unsafe FakeStruct(DataBuffer db, int size, void* point)
        {
            element = size;
            msize = size * 4;
            ptr = Marshal.AllocHGlobal(msize);
            unsafe
            {
                Int32* sp = (Int32*)point;
                Int32* p = (Int32*)ptr;
                for (int i = 0; i < size; i++)
                {
                    *p = *sp;
                    sp++;
                    p++;
                }
            }
            ip = (byte*)ptr;
            buffer = db;
        }
        public unsafe void SetPoint(byte* point)
        {
            ip = point;
        }
        ~FakeStruct()
        {
            Marshal.FreeHGlobal(ptr);
        }
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns>32位整数</returns>
        public unsafe Int32 this[int index]
        {
            set
            {
                if (index < 0)
                    return;
                if (index >= element)
                    return;
                int o = index * 4;
                *(Int32*)(ip + o) = value;
            }
            get
            {
                if (index < 0)
                    return 0;
                if (index >= element)
                    return 0;
                int o = index * 4;
                return *(Int32*)(ip + o);
            }
        }
        /// <summary>
        /// 添加一个对象型数据，数据类型必须为DataType中任意一种
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <param name="dat">数据对象</param>
        public unsafe void SetData(int index, object dat)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            Int32* a = (Int32*)(ip + o);
            buffer.RemoveData(*a);
            *a = buffer.AddData(dat);
        }
        /// <summary>
        /// 获取一个对象型数据，数据类型必须为DataType中任意一种
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="index">元素位置</param>
        /// <returns></returns>
        public unsafe T GetData<T>(int index) where T : class
        {
            if (index < 0)
                return null;
            if (index >= element)
                return null;
            int o = index * 4;
            Int32* a = (Int32*)(ip + o);
            if (*a >= 0)
                return buffer.GetData(*a) as T;
            return null;
        }
        /// <summary>
        /// 获取一个对象型数据，数据类型必须为DataType中任意一种
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <returns></returns>
        public unsafe object GetData(int index)
        {
            if (index < 0)
                return null;
            if (index >= element)
                return null;
            int o = index * 4;
            Int32* a = (Int32*)(ip + o);
            return buffer.GetData(*a);
        }
        /// <summary>
        /// 添加一个非托管类型的结构体数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="index">元素位置</param>
        /// <param name="obj">对象数据</param>
        public void AddArray<T>(int index, T[] obj) where T : unmanaged
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            unsafe
            {
                int o = index * 4;
                Int32* a = (Int32*)(ip + o);
                buffer.RemoveData(*a);
                *a = buffer.AddArray<T>(obj);
            }
        }
        /// <summary>
        /// 获取一个非托管类型的结构体数组
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="index">元素位置</param>
        /// <returns></returns>
        public T[] GetArray<T>(int index) where T : unmanaged
        {
            if (index < 0)
                return null;
            if (index >= element)
                return null;
            unsafe
            {
                int o = index * 4;
                Int32* a = (Int32*)(ip + o);
                index = *a;
                if (index >= 0)
                    return buffer.GetArray<T>(*a);
            }
            return null;
        }
        /// <summary>
        /// 添加一个对象数据
        /// </summary>
        /// <param name="addr">内存地址</param>
        /// <param name="dat">对象数据</param>
        public unsafe void SetData(int* addr, object dat)
        {
            int a = (int)addr - (int)ip;
            if (a < 0 | a >= msize)//超过界限
                return;
            buffer.RemoveData(*addr);
            *addr = buffer.AddData(dat);
        }
        /// <summary>
        /// 获取一个对象数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="addr">内存地址</param>
        /// <returns></returns>
        public unsafe T GetData<T>(int* addr) where T : class
        {
            int a = (int)addr - (int)ip;
            if (a < 0 | a >= msize)//超过界限
                return null;
            return buffer.GetData(*addr) as T;
        }
        /// <summary>
        /// 设置一个64位的整数值
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <param name="value">值</param>
        public unsafe void SetInt64(int index, Int64 value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(Int64*)(ip + o) = value;
        }
        /// <summary>
        /// 获取一个64位的整数值
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <returns></returns>
        public unsafe Int64 GetInt64(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(Int64*)(ip + o);
        }
        /// <summary>
        /// 设置一个浮点数
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <param name="value">值</param>
        public unsafe void SetFloat(int index, float value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(float*)(ip + o) = value;
        }
        /// <summary>
        /// 获取一个浮点数
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <returns></returns>
        public unsafe float GetFloat(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(float*)(ip + o);
        }
        /// <summary>
        /// 设置一个双浮点数
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <param name="value">值</param>
        public unsafe void SetDouble(int index, double value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(double*)(ip + o) = value;
        }
        /// <summary>
        /// 获取一个双浮点数
        /// </summary>
        /// <param name="index">元素位置</param>
        /// <returns></returns>
        public unsafe double GetDouble(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(double*)(ip + o);
        }
        public unsafe void SetDecimal(int index, Decimal value)
        {
            if (index < 0)
                return;
            if (index >= element)
                return;
            int o = index * 4;
            *(Decimal*)(ip + o) = value;
        }
        public unsafe Decimal GetDecimal(int index)
        {
            if (index < 0)
                return 0;
            if (index >= element)
                return 0;
            int o = index * 4;
            return *(Decimal*)(ip + o);
        }
        /// <summary>
        /// 将假结构体中的数据导出位byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] tmp = new byte[msize];
            Marshal.Copy(ptr, tmp, 0, msize);
            return tmp;
        }
        /// <summary>
        /// 进行元素扩展
        /// </summary>
        /// <param name="size"></param>
        public unsafe void Extend(int size)
        {
            int s = msize;
            element += size;
            msize = s + size * 4;
            var tp = Marshal.AllocHGlobal(msize);
            unsafe
            {
                ip = (byte*)ptr;
                Int32* p = (Int32*)ptr;
                Int32* t = (Int32*)tp;
                for (int i = 0; i < s; i++)
                {
                    *t = *p;
                    p++;
                    t++;
                }
            }
            Marshal.FreeHGlobal(ptr);
            ptr = tp;
        }

        #region //提高容错率,做了范围鉴定,需要速度可以使用上面的函数
        int offset;
        /// <summary>
        /// 设置位置索引
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public bool Seek(int index = 0)
        {
            if (index < 0)
                return false;
            if (index >= element)
                return false;
            offset = index;
            return true;
        }
        /// <summary>
        /// 写入一个整数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteInt(int value)
        {
            if (offset < element)
            {
                this[offset] = value;
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
            if (offset < element)
            {
                SetFloat(offset, value);
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
            if (offset + 1 < element)
            {
                SetInt64(offset, value);
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
            if (offset + 1 < element)
            {
                SetDouble(offset, value);
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
            if (offset < element)
            {
                SetData(offset, value);
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
            if (offset < element)
            {
                AddArray<T>(offset, value);
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
            if (offset < element)
            {
                var value = this[offset];
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
            if (offset < element)
            {
                var value = GetFloat(offset);
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
            if (offset + 1 < element)
            {
                var value = GetInt64(offset);
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
            if (offset + 1 < element)
            {
                var value = GetDouble(offset);
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
            if (offset < element)
            {
                var value = GetData<T>(offset);
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
            if (offset < element)
            {
                var value = GetArray<T>(offset);
                offset++;
                return value;
            }
            return null;
        }
        #endregion
    }
}
