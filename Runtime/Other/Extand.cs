using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace huqiang
{
    public static class Extand
    {
        static byte[] Zreo = new byte[4];
        static string hex = "0123456789abcdef";
        /// <summary>
        /// 16进制文本转颜色值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color32 HexToColor(this string value)
        {
            if (value == null)
                return new Color32();
            byte[] tmp = new byte[8];

            for (int i = 0; i < value.Length; i++)
            {
                tmp[i] = (byte)hex.IndexOf(value[i]);
                if (tmp[i] < 0)
                    tmp[i] = 0;
            }
            for (int i = 0; i < 8; i += 2)
            {
                tmp[i] *= 16;
                tmp[i] += tmp[i + 1];
            }
            return new Color32(tmp[0], tmp[2], tmp[4], tmp[6]);
        }
        /// <summary>
        /// 整数转颜色值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static Color32 ToColor(this Int32 value)
        {
            byte* b = (byte*)&value;
            return new Color32(*(b + 3), *(b + 2), *(b + 1), *b);
        }
        /// <summary>
        /// 短整数转bytes[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(this Int16 value)
        {
            byte[] buff = new byte[2];
            fixed (byte* bp = &buff[0])
                *(Int16*)bp = value;
            return buff;
        }
        /// <summary>
        /// 整数转byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(this Int32 value)
        {
            byte[] buff = new byte[4];
            fixed (byte* bp = &buff[0])
                *(Int32*)bp = value;
            return buff;
        }
        /// <summary>
        /// 浮点数转byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(this Single value)
        {
            byte[] buff = new byte[4];
            fixed (byte* bp = &buff[0])
                *(Single*)bp = value;
            return buff;
        }
        /// <summary>
        /// 长整数转byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(this Int64 value)
        {
            byte[] buff = new byte[8];
            fixed (byte* bp = &buff[0])
                *(Int64*)bp = value;
            return buff;
        }
        /// <summary>
        /// 双浮点转byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(this Double value)
        {
            byte[] buff = new byte[8];
            fixed (byte* bp = &buff[0])
                *(Double*)bp = value;
            return buff;
        }
        /// <summary>
        /// 无符号整数转颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color32 ToColor(this UInt32 value)
        {
            unsafe
            {
                byte* b = (byte*)&value;
                return new Color32(*(b + 3), *(b + 2), *(b + 1), *b);
            }
        }
        /// <summary>
        /// 将字符串写入流
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="str">字符串</param>
        public static void WriteString(this Stream stream, string str)
        {
            if (str == null)
            {
                stream.Write(Zreo, 0, 4);
            }
            else if (str.Length == 0)
            {
                stream.Write(Zreo, 0, 4);
            }
            else
            {
                var buf = Encoding.UTF8.GetBytes(str);
                stream.Write(buf.Length.ToBytes(), 0, 4);
                stream.Write(buf, 0, buf.Length);
            }
        }
        /// <summary>
        /// 将内存指针中的数据写入流
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="p">内存地址</param>
        /// <param name="size">数据长度</param>
        public unsafe static void Write(this Stream stream, byte* p, int size)
        {
            for (int i = 0; i < size; i++)
            { stream.WriteByte(*p); p++; }
        }
        /// <summary>
        /// 从byte[]中读取短整数
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset">偏移位置</param>
        /// <returns></returns>
        public unsafe static Int32 ReadInt16(this byte[] buff, Int32 offset)
        {
            fixed (byte* bp = &buff[0])
                return *(Int16*)(bp + offset);
        }
        /// <summary>
        /// 从byte[]中读取整数
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset">偏移位置</param>
        /// <returns></returns>
        public unsafe static Int32 ReadInt32(this byte[] buff, Int32 offset)
        {
            fixed (byte* bp = &buff[0])
                return *(Int32*)(bp + offset);
        }

        public unsafe static void Read(this byte[] buff, void* p, int offset, int size)
        {
            byte* bp = (byte*)p;
            for (int i = 0; i < size; i++)
            {
                *bp = buff[offset];
                bp++;
                offset++;
            }
        }
        /// <summary>
        /// 二维向量向前移动
        /// </summary>
        /// <param name="v">向量</param>
        /// <param name="len">移动距离</param>
        /// <returns></returns>
        public static Vector2 Move(this Vector2 v, float len)
        {
            if (v.x == 0 & v.y == 0)
                return v;
            float sx = v.x * v.x + v.y * v.y;
            float r = Mathf.Sqrt(len * len / sx);
            return new Vector2(v.x * r, v.y * r);
        }
        /// <summary>
        /// 二维向量旋转
        /// </summary>
        /// <param name="v">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            var a = MathH.atan(v.x,v.y);
            a += angle;
            a %= 360;
            if (a < 0)
                a += 360;
            return MathH.Tan2(a);
        }
        /// <summary>
        /// 三维向量移动
        /// </summary>
        /// <param name="v">向量</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static Vector3 Move(this Vector3 v, float len)
        {
            if (v.x == 0 & v.y == 0 & v.z == 0)
                return v;
            float sx = v.x * v.x + v.y * v.y + v.z * v.z;
            float r = Mathf.Sqrt(len * len / sx);
            return new Vector3(v.x * r, v.y * r, v.z * r);
        }
        /// <summary>
        /// 向量向某个向量旋转后的位置
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 RotateDirection(this Vector3 v, Vector3 v2)
        {
            var q = Quaternion.LookRotation(v2);
            return q * v;
        }
        /// <summary>
        /// 向量旋转多少欧拉角后的位置
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 RotateAngle(this Vector3 v, Vector3 v2)
        {
            var q = Quaternion.Euler(v2);
            return q * v;
        }
        /// <summary>
        /// 某个向量旋转多少欧拉角后的四元数
        /// </summary>
        /// <param name="v">向量</param>
        /// <param name="v2">角度</param>
        /// <returns></returns>
        public static Quaternion Rotate(this Vector3 v, Vector3 v2)
        {
            var q = Quaternion.LookRotation(v);
            return q * Quaternion.Euler(v2);
        }
        /// <summary>
        /// 将结构体数组转换成byte[],结构体中字段不能有引用类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes<T>(this T[] obj) where T : unmanaged
        {
            int size = sizeof(T);
            int len = obj.Length;
            int all = size * len;
            byte[] buf = new byte[all];
            unsafe
            {
                fixed (T* ptr = &obj[0])
                fixed (byte* bp = &buf[0])
                {
                    var sp = (byte*)ptr;
                    var tp = bp;
                    for (int i = 0; i < all; i++)
                    {
                        *tp = *sp;
                        sp++;
                        tp++;
                    }
                }
            }
            return buf;
        }
        /// <summary>
        /// 将byte[]转换成结构体数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public unsafe static T[] ToArray<T>(this byte[] obj) where T : unmanaged
        {
            int size = sizeof(T);
            int all = obj.Length;
            int len = all / size;
            all = len * size;//取整,防止溢出
            T[] buf = new T[len];
            unsafe
            {
                fixed (T* ptr = &buf[0])
                fixed (byte* bp = &obj[0])
                {
                    var tp = (byte*)ptr;
                    var sp = bp;
                    for (int i = 0; i < all; i++)
                    {
                        *tp = *sp;
                        sp++;
                        tp++;
                    }
                }
            }
            return buf;
        }
        /// <summary>
        /// 从内存地址中读取一个非托管结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ip">目标内存地址</param>
        /// <param name="src">源内存地址</param>
        public unsafe static void ReadFrom<T>(this IntPtr ip,byte* src) where T : unmanaged
        {
            byte* tp = (byte*)ip;
            int size = sizeof(T);
            for (int i = 0; i < size; i++)
            {
                *tp = *src;
                tp++;
                src++;
            }
        }
        /// <summary>
        /// 将一个非托管结构体中的数据写入到内存
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="ip">源内存地址</param>
        /// <param name="tar">目标内存地址</param>
        public unsafe static void WriteTo<T>(this IntPtr ip, byte* tar) where T : unmanaged
        {
            int size = sizeof(T);
            byte* bp = (byte*)ip;
            for (int i = 0; i < size; i++)
            {
                *tar = *bp;
                tar++;
                bp++;
            }
        }
    }
}