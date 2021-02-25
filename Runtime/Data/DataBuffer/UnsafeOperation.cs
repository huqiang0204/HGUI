using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace huqiang.Data
{
    /// <summary>
    /// 仅适用于Window平台
    /// </summary>
    public static class UnsafeOperation
    {
#if ENABLE_IL2CPP
        static object[] Arr;
        static IntPtr ArrPtr;
        static UnsafeOperation()
        {
            Arr = new object[1];
            GCHandle.Alloc(Arr, GCHandleType.Pinned);
            ArrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(Arr, 0);
        }
#endif
        /// <summary>
        ///  获取引用类型的地址
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public unsafe static IntPtr GetStructAddr(object obj)
        {
#if ENABLE_MONO
            TypedReference tf = __makeref(obj);
            long* p = (long*)&tf;
            p++;
            long b = *p;
            return (IntPtr)(*(long*)b) + 16;
#else
			Arr[0] = obj;
            var ptr = *(IntPtr*)ArrPtr;
            Arr[0] = null;//清除GC引用计数
            return ptr + 16;
#endif
        }
        /// <summary>
        ///  获取引用类型的地址
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public unsafe static IntPtr GetObjectAddr(object obj)
        {
#if ENABLE_MONO
            TypedReference tf = __makeref(obj);
            long* p = (long*)&tf;
            p++;
            long b = *p;
            return (IntPtr)(*(long*)b);
#else
            Arr[0] = obj;
            var ptr = *(IntPtr*)ArrPtr;
            Arr[0] = null;//清除GC引用计数
            return ptr;
#endif
        }
        /// <summary>
        ///  程序的字段声明偏移位置
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public unsafe static Int16 GetFeildOffset(IntPtr ptr, bool isStruct)
        {
            if(isStruct)
                return (Int16)(*(Int16*)(ptr + 24) - 16);
            return *(Int16*)(ptr + 24);
        }
        /// <summary>
        /// 将对象的地址设置到目标地址，有类型判定和引用计数，推荐在堆上操作
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="obj"></param>
        public unsafe static void SetObject(IntPtr tar, object obj)
        {
#if ENABLE_MONO
            object tmp = "";
            TypedReference tr = __makeref(tmp);
            long* p = (long*)&tr;
            p++;
            *p = (long)tar;
            __refvalue(tr, object) = obj;
#else
            Arr[0] = obj;
            var tr = *(long*)ArrPtr;
            *(long*)tar = tr;
            *(long*)ArrPtr = 0;
#endif
        }
        /// <summary>
        /// 获取目标地址的对象
        /// </summary>
        /// <param name="tar"></param>
        /// <returns></returns>
        public unsafe static object GetObject(IntPtr tar)
        {
#if ENABLE_MONO
            object tmp = "";
            TypedReference tr = __makeref(tmp);//new TypedReference(); 
            long* p = (long*)&tr;
            p++;
            *p = (long)tar;
            return __refvalue(tr, object);
#else
			*(IntPtr*)ArrPtr = *(IntPtr*)tar;
            return Arr[0];
#endif
        }
        /// <summary>
        ///  获取引用类型的地址
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public unsafe static IntPtr GetListElement(IntPtr ptr, int index)
        {
             IntPtr p = *(IntPtr*)(ptr + 16);
             return p + index * 4 + 32;
        }
    }
}
