using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    public struct StringPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(StringPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator StringPoint(int i)
        {
            unsafe { return *(StringPoint*)&i; }
        }
    }
    public struct FakeStrcutPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(FakeStrcutPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator FakeStrcutPoint(int i)
        {
            unsafe { return *(FakeStrcutPoint*)&i; }
        }
    }
    public struct FakeStrcutArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(FakeStrcutArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator FakeStrcutArrayPoint(int i)
        {
            unsafe { return *(FakeStrcutArrayPoint*)&i; }
        }
    }
    public struct FakeStringArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(FakeStringArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator FakeStringArrayPoint(int i)
        {
            unsafe { return *(FakeStringArrayPoint*)&i; }
        }
    }
    /// <summary>
    /// 用于ByteArray
    /// </summary>
    public struct ByteArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(ByteArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator ByteArrayPoint(int i)
        {
            unsafe { return *(ByteArrayPoint*)&i; }
        }
    }
    public struct Int16ArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(Int16ArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator Int16ArrayPoint(int i)
        {
            unsafe { return *(Int16ArrayPoint*)&i; }
        }
    }
    public struct IntArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(IntArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator IntArrayPoint(int i)
        {
            unsafe { return *(IntArrayPoint*)&i; }
        }
    }
    public struct Int64ArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(Int64ArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator Int64ArrayPoint(int i)
        {
            unsafe { return *(Int64ArrayPoint*)&i; }
        }
    }
    public struct FloatArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(FloatArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator FloatArrayPoint(int i)
        {
            unsafe { return *(FloatArrayPoint*)&i; }
        }
    }
    public struct DoubleArrayPoint
    {
        int Index;
        /// <summary>
        ///  int t = 0;
        /// FakeStrcutPoint fsp = *(FakeStrcutPoint*)&t;
        /// </summary>
        /// <param name="fsp"></param>
        public static implicit operator int(DoubleArrayPoint fsp)
        {
            return fsp.Index;
        }
        public static implicit operator DoubleArrayPoint(int i)
        {
            unsafe { return *(DoubleArrayPoint*)&i; }
        }
    }

    public class FakeStructHelper
    {
        public static FakeStruct CreateTable<T>(DataBuffer buffer) where T : unmanaged
        {
            int c = 0;
            unsafe { c = sizeof(T); }
            Type type = typeof(T);
            var fs = type.GetFields();
            FakeStruct fsa = new FakeStruct(buffer, fs.Length * 3 + 1);
            fsa.SetData(0, type.Name);
            int s = 1;
            for (int i = 0; i < fs.Length; i++)
            {
                string typ = fs[i].FieldType.Name;
                string name = fs[i].Name;
                if (fs[i].FieldType.IsEnum)
                    fsa[s] = 4;
                else fsa[s] = Marshal.SizeOf(fs[i].FieldType);
                s++;
                fsa.SetData(s, typ);
                s++;
                fsa.SetData(s, name);
                s++;
            }
            return fsa;
        }
        public string Name { get; private set; }
        Type type;
        public int Size { get; private set; }
        FakeInfo[] TarInfos;
        int[] indexs;
        struct FakeInfo
        {
            public string Type;
            public int Offset;
            public int Size;
            public string Name;
        }
        FakeInfo[] OriInfos;
        public void SetTargetModel<T>() where T : unmanaged
        {
            unsafe { Size = sizeof(T); }
            type = typeof(T);
            Name = type.Name;
            var fs = type.GetFields();
            TarInfos = new FakeInfo[fs.Length];
            int os = 0;
            for (int i = 0; i < TarInfos.Length; i++)
            {
                TarInfos[i].Type = fs[i].FieldType.Name;
                TarInfos[i].Name = fs[i].Name;
                if (fs[i].FieldType.IsEnum)
                    TarInfos[i].Size = 4;
                else TarInfos[i].Size = Marshal.SizeOf(fs[i].FieldType);
                TarInfos[i].Offset = os;
                os += TarInfos[i].Size;
            }
            Order();
        }
        public void SetOriginModel(FakeStruct fake)
        {
            if(fake==null)
            {
                OriInfos = null;
                indexs = null;
                return;
            }
            int c = fake.Length;
            int u = c / 3;
            OriInfos = new FakeInfo[u];
            int s = 1;
            int os = 0;
            for (int i = 0; i < u; i++)
            {
                OriInfos[i].Offset = os;
                OriInfos[i].Size = fake[s];
                os += fake[s];
                s++;
                OriInfos[i].Type = fake.GetData<string>(s);
                s++;
                OriInfos[i].Name = fake.GetData<string>(s);
                s++;
            }
            Order();
        }
        /// <summary>
        /// 两张表的排列相同
        /// </summary>
        bool same;
        void Order()
        {
            if (OriInfos == null)
                return;
            if (TarInfos == null)
                return;
            indexs = new int[TarInfos.Length];
            same = true;
            for (int i = 0; i < indexs.Length; i++)
            {
                indexs[i] = -1;
                string type = TarInfos[i].Type;
                string name = TarInfos[i].Name;
                int size =TarInfos[i].Size;
                for (int j = 0; j < OriInfos.Length; j++)
                {
                    if (OriInfos[j].Type == type)
                        if (OriInfos[j].Name == name)
                            if (OriInfos[j].Size == size)
                            {
                                indexs[i] = j;
                                break;
                            }
                }
                if (same)
                    if (i != indexs[i])
                        same = false;
            }
        }
        public unsafe void LoadData(byte* tar, byte* src)
        {
            if (same)//排列相同则直接复制数据
            {
                for (int i = 0; i < Size; i++)
                {
                    tar[i] = src[i];
                }
                return;
            }
            if (indexs == null)
                return;
            byte* p = tar;
            for (int i = 0; i < indexs.Length; i++)
            {
                int a = indexs[i];
                int c = TarInfos[i].Size;
                if (a > 0)
                {
                    byte* sp = src + OriInfos[a].Offset;
                    for (int j = 0; j < TarInfos[i].Size; j++)
                    {
                        *p = *sp;
                        p++;
                        sp++;
                    }
                }
                else
                {
                    p += c;
                }
            }
        }
        public T LoadData<T>(FakeStruct ori) where T : unmanaged
        {
            T t = new T();
           unsafe { LoadData((byte*)&t, ori.ip); }
            return t;
        }
    }
}
