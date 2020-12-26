using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public interface ToBytes
    {
        byte[] ToBytes();
    }
    public interface FakeArray
    {
         Int32 Length { get; }
    }
    public class DataType
    {
        public const short String = 0;
        public const short FakeStruct = 1;
        public const short FakeStructArray = 2;
        public const short ByteArray = 3;
        public const short Int32Array = 4;
        public const short FloatArray = 5;
        public const short Int16Array = 6;
        public const short Int64Array = 7;
        public const short DoubleArray = 8;
        public const short FakeStringArray = 9;
        public const short Int = 10;
        public const short Float = 11;
        public const short Long = 12;
        public const short Double = 13;
        public const short Decimal = 14;

        public const short Struct = 32;

        public const short Class = 64;
        public const short StructArray = 33;
        /// <summary>
        /// 类对象数组
        /// </summary>
        public const short ClassArray = 65;
        /// <summary>
        /// Two-dimensional array
        /// </summary>
        public const short TwoDArray = 34;
        /// <summary>
        /// Two-dimensional array
        /// </summary>
        public const short TwoDList = 66;
    }
    /// <summary>
    /// C#的这些类型不能被继承：System.ValueType, System.Enum, System.Delegate, System.Array, etc.
    /// </summary>
    public class DataBuffer
    {
        static Int32 GetType(object obj)
        {
            if (obj is string)
                return DataType.String;
            else if (obj is FakeStruct)
                return DataType.FakeStruct;
            else if (obj is FakeStructArray)
                return DataType.FakeStructArray;
            else if (obj is byte[])
                return DataType.ByteArray;
            else if (obj is Int32[])
                return DataType.Int32Array;
            else if (obj is Single[])
                return DataType.FloatArray;
            else if (obj is Int16[])
                return DataType.Int16Array;
            else if (obj is Int64[])
                return DataType.Int64Array;
            else if (obj is Double[])
                return DataType.DoubleArray;
            else if (obj is FakeStringArray)
                return DataType.FakeStringArray;
            return -1;
        }
        static int GetArraySize(Array array)
        {
            int size = 1;
            if (array is Int32[])
                size = 4;
            else if (array is float[])
                size = 4;
            else if (array is Int16[])
                size = 2;
            else if (array is Double[])
                size = 8;
            else if (array is Int64[])
                size = 8;
            return size;
        }
        public FakeStruct fakeStruct;
        static byte[] Zreo = new byte[4];
        struct ReferenceCount
        {
            /// <summary>
            /// Reference count
            /// </summary>
            public Int16 rc;
            /// <summary>
            ///byte[], String,FakeStruct,FakeStructArray,int[],float[],double[]
            /// </summary>
            public Int16 type;
            public Int32 size;
            public object obj;
        }
        /// <summary>
        /// 添加一个引用类型的数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int AddData(object obj)
        {
            return AddData(obj, GetType(obj));
        }
        /// <summary>
        ///  添加一个引用类型的数据
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public int AddData(object obj, Int32 type)
        {
            if (obj is string[])
                return 0;
            int min = max;
            if(type==DataType.String)
            {
                string str = obj as string;
                for (int a = max - 1; a >= 0; a--)
                {
                    var c = buff[a].obj as string;
                    if (str == c)
                    {
                        buff[a].rc++;
                        return a;
                    }
                    if (buff[a].rc == 0)
                    {
                        min = a;
                    }
                }
            }
            else
            {
                for (int a = max - 1; a >= 0; a--)
                {
                    if (obj == buff[a].obj)
                    {
                        buff[a].rc++;
                        return a;
                    }
                    if (buff[a].rc == 0)
                    {
                        min = a;
                    }
                }
            }
            buff[min].rc = 1;
            buff[min].type = (Int16)type;
            buff[min].obj = obj;
            if (type == DataType.FakeStructArray)
                buff[min].size = (obj as FakeStructArray).m_size;
            if (min == max)
            {
                max++;
                if (max >= Count)
                {
                    Count *= 2;
                    ReferenceCount[] tmp = new ReferenceCount[Count];
                    Array.Copy(buff, tmp, min + 1);
                    buff = tmp;
                }
            }
            return min;
        }
        /// <summary>
        /// 提取一个对象
        /// </summary>
        /// <param name="index">对象索引</param>
        /// <returns></returns>
        public object GetData(int index)
        {
            return buff[index].obj;
        }
        /// <summary>
        /// 提取对象索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get {
                if (index < 0|index>buff.Length)
                    return null;
                    return buff[index].obj;
            }
        }
        /// <summary>
        /// 移除某个对象,引用计数减一,当引用计数归0时,真正移除
        /// </summary>
        /// <param name="index"></param>
        public void RemoveData(int index)
        {
            if (index < 1)
                return;
            if (index < buff.Length)
                buff[index].rc--;
        }
        ReferenceCount[] buff;
        int max = 1;
        int Count = 256;
        /// <summary>
        /// 创建一个空白的缓存
        /// </summary>
        /// <param name="buffCount"></param>
        public DataBuffer(int buffCount = 256)
        {
            buff = new ReferenceCount[buffCount];
            buff[0].rc = 256;
            Count = buffCount;
        }
        byte[] temp;
        unsafe byte* tempStart;
        /// <summary>
        /// 从已有的数据进行恢复
        /// </summary>
        /// <param name="dat"></param>
        public unsafe DataBuffer(byte[] dat)
        {
            temp = dat;
            var src = Marshal.UnsafeAddrOfPinnedArrayElement(dat, 0);
            tempStart = (byte*)src;
            Int32* ip = (Int32*)src;
            ip = ReadHead(ip);
            if (fakeStruct == null)
                return;
            Int32 len = *ip;
            ip++;
            Int32* rs = ip + len * 3;
            int a = len;
            for (int i = 0; i < 32; i++)
            {
                a >>= 1;
                if (a == 0)
                {
                    a = 2 << i;
                    break;
                }
            }
            max = len;
            Count = a;
            buff = new ReferenceCount[a];
            byte* bp = (byte*)rs;
            for (int i = 0; i < len; i++)
            {
                GetTable(ip, i, bp);
                ip += 3;
            }
            temp = null;
        }
        unsafe Int32* ReadHead(Int32* p)
        {
            int len = *p;
            if (len > 0)
            {
                p++;
                if (AddressDetection((byte*)p, len))
                {
                    len /= 4;
                    fakeStruct = new FakeStruct(this, len, p);
                    p += len;
                }
            }
            else
            {
                p++;
            }
            return p;
        }
        unsafe void GetTable(Int32* p, int index, byte* rs)
        {
            if (!AddressDetection((byte*)p, 8))
                return;
            Int16* sp = (Int16*)p;
            buff[index].rc = *sp;
            sp++;
            buff[index].type = *sp;
            p++;
            buff[index].size = *p;
            p++;
            Int32 offset = *p;
            rs += offset;
            if (AddressDetection(rs, 4))//如果资源地址合法
                buff[index].obj = GetObject(rs, buff[index].type, buff[index].size);
        }
        unsafe object GetObject(byte* bp, short type, int size)
        {
            Int32* p = (Int32*)bp;
            int len = *p;
            if (len == 0)
                return null;
            p++;
            if (!AddressDetection((byte*)p, len))
                return null;
            if (type == DataType.String)
            {
                int offset = (int)((byte*)p - tempStart);
                return Encoding.UTF8.GetString(temp, offset, len);
            }
            else if (type == DataType.FakeStruct)
            {
                len /= 4;
                return new FakeStruct(this, len, p);
            }
            else if (type == DataType.FakeStructArray)
            {
                len /= size;
                len /= 4;
                return new FakeStructArray(this, size, len, p);
            }
            else if (type == DataType.ByteArray)
            {
                byte[] buf = new byte[len];
                bp += 4;
                for (int i = 0; i < len; i++)
                { buf[i] = *bp; bp++; }
                return buf;
            }
            else if (type == DataType.Int32Array)
            {
                len /= 4;
                Int32[] buf = new Int32[len];
                for (int i = 0; i < len; i++)
                { buf[i] = *p; p++; }
                return buf;
            }
            else if (type == DataType.FloatArray)
            {
                len /= 4;
                Single[] buf = new Single[len];
                float* sp = (float*)p;
                for (int i = 0; i < len; i++)
                { buf[i] = *sp; sp++; }
                return buf;
            }
            else if (type == DataType.Int16Array)
            {
                len /= 2;
                Int16[] buf = new Int16[len];
                Int16* sp = (Int16*)p;
                for (int i = 0; i < len; i++)
                {
                    buf[i] = *sp; sp++;
                }
                return buf;
            }
            else if (type == DataType.Int64Array)
            {
                len /= 8;
                Int64[] buf = new long[len];
                Int64* sp = (Int64*)p;
                for (int i = 0; i < len; i++)
                { 
                    buf[i] = *sp; 
                    sp++; 
                }
                return buf;
            }
            else if (type == DataType.DoubleArray)
            {
                len /= 8;
                Double[] buf = new Double[len];
                double* sp = (double*)p;
                for (int i = 0; i < len; i++)
                { buf[i] = *sp; sp++; }
                return buf;
            }
            else if (type == DataType.FakeStringArray)
            {
                len = *p;
                p++;
                return new FakeStringArray(this, p, len);
            }
            return null;
        }
        /// <summary>
        /// 检测地址是否合法
        /// </summary>
        /// <param name="bp"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public unsafe bool AddressDetection(byte* bp, int len)
        {
            var offset = bp - tempStart;
            if (offset + len > temp.Length)
                return false;
            return true;
        }
        static byte[] GetBytes(object obj)
        {
            if (obj is byte[])
                return obj as byte[];
            var array = obj as Array;
            if (array != null)
            {
                int size = GetArraySize(array);
                int len = array.Length;
                byte[] buf = new byte[len * size];
                var src = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
                unsafe
                {
                    byte* sp = (byte*)src;
                    for (int i = 0; i < buf.Length; i++)
                    {
                        buf[i] = *sp;
                        sp++;
                    }
                }
                //Marshal.Copy(src, buf, 0, buf.Length);
                return buf;
            }
            else
            {
                var str = obj as string;
                if (str == null)
                {
                    var to = obj as ToBytes;
                    if (to != null)
                        return to.ToBytes();
                    return null;
                }
                return Encoding.UTF8.GetBytes(str);
            }
        }
        /// <summary>
        /// 将对象数据转换成byte[]并取出
        /// </summary>
        /// <param name="index">对象索引</param>
        /// <returns></returns>
        public byte[] GetBytes(int index)
        {
            return GetBytes(buff[index].obj);
        }
        /// <summary>
        /// 将整个DataBuffer中的数据转换成byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            MemoryStream table = new MemoryStream();
            if (fakeStruct == null)
            {
                table.Write(Zreo, 0, 4);
            }
            else
            {
                byte[] buf = fakeStruct.ToBytes();
                Int32 len = buf.Length;
                table.Write(len.ToBytes(), 0, 4);//写入头的长度
                table.Write(buf, 0, len);//写入头数据
            }
            table.Write(max.ToBytes(), 0, 4);//写入表的长度
            MemoryStream ms = new MemoryStream();
            Int32 offset = 0;
            for (int i = 0; i < max; i++)
            {
                var buf = GetBytes(buff[i].obj);
                table.Write(buff[i].rc.ToBytes(), 0, 2);//引用计数
                table.Write(buff[i].type.ToBytes(), 0, 2);//数据类型
                table.Write(buff[i].size.ToBytes(), 0, 4);//数据结构体长度
                table.Write(offset.ToBytes(), 0, 4);//数据偏移地址
                if (buf == null)
                {
                    ms.Write(Zreo, 0, 4);
                    offset += 4;
                }
                else
                {
                    Int32 len = buf.Length;
                    ms.Write(len.ToBytes(), 0, 4);
                    ms.Write(buf, 0, len);
                    offset += len + 4;
                }
            }
            byte[] tmp = ms.ToArray();
            table.Write(tmp, 0, tmp.Length);//合并表和托管类型数据
            tmp = table.ToArray();
            ms.Dispose();
            table.Dispose();
            return tmp;
        }
        /// <summary>
        /// 添加一个数组对象,此对象元素类型必须为非托管结构体
        /// </summary>
        /// <typeparam name="T">非托管结构体</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public int AddArray<T>(T[] obj) where T : unmanaged
        {
            if(obj is byte[])
               return AddData(obj,DataType.ByteArray);
            var dat = obj.ToBytes<T>();
            return AddData(dat,DataType.ByteArray);
        }
        /// <summary>
        /// 获取一个数组对象,此对象元素类型必须为非托管结构体
        /// </summary>
        /// <typeparam name="T">非托管结构体</typeparam>
        /// <param name="index">对象索引</param>
        /// <returns></returns>
        public T[] GetArray<T>(int index) where T : unmanaged
        {
            var buf = buff[index].obj as byte[];
            if(buf!=null)
               return buf.ToArray<T>();
            return null;
        }
    }
}