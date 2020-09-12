using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    public class StructBufferU<T> where T : unmanaged
    {
        int max;
        int tsize;
        IntPtr ptr;
        public unsafe StructBufferU(int len)
        {
            max = len;
            tsize = sizeof(T) + 1;
            int msize = len * tsize;
            ptr = Marshal.AllocHGlobal(msize);
            byte* bp = (byte*)ptr;
            for (int i = 0; i < msize; i++)
                *bp = 0;
        }
        public unsafe T* RegNew()
        {
            byte* bp = (byte*)ptr;
            for (int i = 0; i < max; i++)
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
            for (int i = 0; i < max; i++)
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
