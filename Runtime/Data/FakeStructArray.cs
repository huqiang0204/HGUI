using System;
using System.Runtime.InteropServices;


namespace huqiang.Data
{
    public class FakeStructArray : ToBytes
    {
        IntPtr ptr;
        public unsafe byte* ip;
        internal Int32 m_size;
        int m_len;
        int all_len;
        public DataBuffer buffer;
        /// <summary>
        /// 
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
        public int StructLegth { get { return m_size; } }
        public Int32 Length { get { return m_len; } }
        public unsafe byte* this[int index]
        {
            get { return ip + index * m_size * 4; }
        }
        public Int32 this[int index, int os]
        {
            get { return GetInt32(index, os); }
            set { SetInt32(index, os, value); }
        }
        public unsafe Int32 GetInt32(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(Int32*)(ip + o);
        }
        public unsafe void SetInt32(int index, int offset, Int32 value)
        {
            int o = (index * m_size + offset) * 4;
            *(Int32*)(ip + o) = value;
        }
        public unsafe Int64 GetInt64(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(Int64*)(ip + o);
        }
        public unsafe void SetInt64(int index, int offset, Int64 value)
        {
            int o = (index * m_size + offset) * 4;
            *(Int64*)(ip + o) = value;
        }
        public unsafe float GetFloat(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(float*)(ip + o);
        }
        public unsafe void SetFloat(int index, int offset, float value)
        {
            int o = (index * m_size + offset) * 4;
            *(float*)(ip + o) = value;
        }
        public unsafe double GetDouble(int index, int offset)
        {
            int o = (index * m_size + offset) * 4;
            return *(double*)(ip + o);
        }
        public unsafe void SetDouble(int index, int offset, double value)
        {
            int o = (index * m_size + offset) * 4;
            *(double*)(ip + o) = value;
        }
        public unsafe void SetData(int index, int offset, object dat)
        {
            int o = (index * m_size + offset) * 4;
            Int32* a = (Int32*)(ip + o);
            buffer.RemoveData(*a);
            *a = buffer.AddData(dat);
        }
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
        public byte[] ToBytes()
        {
            byte[] buf = new byte[all_len];
            Marshal.Copy(ptr, buf, 0, all_len);

            return buf;
        }
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
        public unsafe void SetData(int* addr, object dat)
        {
            int a = (int)addr - (int)ip;
            if (a < 0 | a >= all_len)//超过界限
                return;
            buffer.RemoveData(*addr);
            *addr = buffer.AddData(dat);
        }
        public unsafe T GetData<T>(int* addr) where T : class
        {
            int a = (int)addr - (int)ip;
            if (a < 0 | a >= all_len)//超过界限
                return null;
            return buffer.GetData(*addr) as T;
        }
        public unsafe T GetData<T>(int index, int offset) where T : class
        {
            int o = (index * m_size + offset) * 4;
            int os = *(Int32*)(ip + o);
            return buffer.GetData(os) as T;
        }
        public unsafe void AddArray<T>(int index, int offset, T[] dat) where T : unmanaged
        {
            int o = (index * m_size + offset) * 4;
            Int32* a = (Int32*)(ip + o);
            buffer.RemoveData(*a);
            *a = buffer.AddArray<T>(dat);
        }
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
        public bool Seek(int index ,int os = 0)
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