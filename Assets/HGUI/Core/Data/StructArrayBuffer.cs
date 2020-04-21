using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public unsafe struct StructPoint
    {
        public byte* Point;
        static StructPoint origin;
        public static implicit operator StructPoint(void* v)
        { 
            origin.Point = (byte*)v;
            return origin;
        }
        public void Free()
        {
            if ((int)Point != 0)
                *(Point - 1) = 0;
        }
    }
    public class StructArrayBuffer<T> :IDisposable where T : unmanaged
    {
        public unsafe T* this[int index] { get { return (T*)(ptr + index * tsize + 1); } }
        IntPtr ptr;
        int len1;
        int len2;
        int tsize;
        public unsafe StructArrayBuffer(int len,int size)
        {
            len1 = len;
            len2 = size;
            tsize = size * sizeof(T) +1;
            int msize = len  * tsize;
            ptr = Marshal.AllocHGlobal(msize);
            byte* bp = (byte*)ptr;
            for (int i = 0; i < msize; i++)
                *bp = 0;
        }
        public unsafe T* RegNew()
        {
            byte* bp = (byte*)ptr;
            for (int i = 0; i < len1; i++)
            {
                if (*bp == 0)
                {
                    *bp = 1;
                    return (T*)(bp + 1);
                }
                bp += tsize;
            }
            return (T*)(ptr + 1);
        }
        public unsafe void Clear()
        {
            byte* bp = (byte*)ptr;
            for(int i=0;i<len1;i++)
            {
                *bp = 0;
                bp += tsize;
            }
        }
        public void Dispose()
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
