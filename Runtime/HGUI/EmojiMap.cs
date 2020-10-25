using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using huqiang.Data;
using System.Reflection.Emit;

namespace huqiang.Core.HGUI
{
    public struct CharUV
    {
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
    }
    public class EmojiInfo
    {
        public int pos;
        public string chr;
        public Vector2[] uv;
    }
    /// <summary>
    /// 表情符图
    /// </summary>
    public partial class EmojiMap
    {
        class CharInfo
        {
            public int len;
            public char[] dat;
            public CharUV[] uVs;
            /// <summary>
            /// 查询表情符是否存在
            /// </summary>
            /// <param name="vs"></param>
            /// <param name="index"></param>
            /// <param name="uv"></param>
            /// <returns></returns>
            public bool Find(char[] vs,int index, Vector2[] uv)
            {
                if (dat == null)
                    return false;
                if (index + len > vs.Length)
                    return false;
                int c = dat.Length / len;
                for (int i = 0; i < c; i++)
                {
                    int s = i*len;
                    int t = index;
                    for (int j = 0; j < len; j++)
                    {
                        if (vs[t] != dat[s])
                            goto label;
                        t++;
                        s++;
                    }
                    if(uv!=null)
                    {
                        uv[0] = uVs[i].uv0;
                        uv[1] = uVs[i].uv1;
                        uv[2] = uVs[i].uv2;
                        uv[3] = uVs[i].uv3;
                    }
                    return true;
                label:;
                }
                return false;
            }
        }
        static CharInfo[] charInfos;
        static EmojiMap()
        {
            Initial(huqiang.Resources.Assets.EmojiInfo);
        }
        /// <summary>
        /// 初始化表情符信息
        /// </summary>
        /// <param name="data"></param>
        public static void Initial(byte[] data)
        {
            var db = new DataBuffer(data);
            var fake = db.fakeStruct;
            charInfos = new CharInfo[16];
            for(int i=0;i<16;i++)
            {
                FakeStruct fs = fake.GetData<FakeStruct>(i);
                if(fs!=null)
                {
                    CharInfo info = new CharInfo();
                    info.len = fs[0];
                    info.dat = db.GetArray<char>(fs[1]);
                    info.uVs = db.GetArray<CharUV>(fs[2]);
                    charInfos[i] = info;
                }
            }
        }
        /// <summary>
        /// 查询表情符,失败返回0,成功返回字符串长度
        /// </summary>
        /// <param name="buff">字符串缓存</param>
        /// <param name="index">起始位置</param>
        /// <param name="uv">查询到的uv数据</param>
        /// <returns></returns>
        public static int FindEmoji(char[] buff, int index,  Vector2[] uv)
        {
            if (index >= buff.Length)
                return 0;
            for(int i=15;i>=0;i--)
            {
                var ci = charInfos[i];
                if(ci!=null)
                {
                    if(ci.Find(buff,index,uv))
                    {
                        return i + 1;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 查询表情符,失败返回0,成功返回字符串长度
        /// </summary>
        /// <param name="buff">字符串缓存</param>
        /// <param name="index">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="uv">查询到的uv数据</param>
        /// <returns></returns>
        public static int FindEmoji(char[] buff, int index, int end, Vector2[] uv)
        {
            if (index >= end)
                return 0;
            int max = end - index;
            if (max > 15)
                max = 15;
            for (int i = max; i >= 0; i--)
            {
                var ci = charInfos[i];
                if (ci != null)
                {
                    if (ci.Find(buff, index, uv))
                    {
                        return i + 1;
                    }
                }
            }
            return 0;
        }
        public const char emSpace = '@';//'\u2001';☀@
        /// <summary>
        /// 检查字符串中的表情符,并替换位@符号
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="list">所包含表情符列表</param>
        /// <returns>替换后的额字符串</returns>
        public static string CheckEmoji(string str, List<EmojiInfo> list)
        {
            if (str == ""|str==null)
                return str;
            StringBuilder sb = new StringBuilder();
            char[] cc = str.ToCharArray();
            int i = 0;
            int len = cc.Length;
            int pos = 0;
            while (i < len)
            {
                char c = cc[i];
                UInt16 v = (UInt16)c;
                if (v < 0x8cff)
                {
                    if (v >= 0x2600 & v <= 0x27bf)
                    {
                        var pst = new EmojiInfo();
                        pst.chr = new string(c, 1);
                        pst.uv = new Vector2[4];
                        FindEmoji(cc, i, pst.uv);
                        pst.pos = pos;
                        list.Add(pst);
                        sb.Append(emSpace);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    i++;
                }
                else
                {
                    var pst = new EmojiInfo();
                    pst.pos = pos;
                    pst.uv = new Vector2[4];
                    int a = FindEmoji(cc, i, pst.uv);
                    if (a > 0)
                    {
                        pst.chr = new string(cc, i, a);
                        i += a;
                        list.Add(pst);
                        sb.Append(emSpace);
                    }
                    else {
                        i++;
                        sb.Append(c);
                    }
                }
                pos++;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将表情符和字符串合并
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="list">所包含表情符列表</param>
        /// <returns></returns>
        public static string EmojiToFullString(string str, List<EmojiInfo> list)
        {
            StringBuilder sb = new StringBuilder();
            char[] cc = str.ToCharArray();
            int s = 0;
            for(int i=0;i<cc.Length;i++)
            {
                if(cc[i]==emSpace)
                {
                    sb.Append(list[s].chr);
                    s++;
                }
                else
                {
                    sb.Append(cc[i]);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 移除表情符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="builder"></param>
        public static int RemoveEmoji(char[] str, int charLen, char[] buf, int max)
        {
            int i = 0;
            int pos = 0;
            while (i < charLen)
            {
                char c = str[i];
                UInt16 v = (UInt16)c;
                if (v < 0x8cff)
                {
                    if (v >= 0x2600 & v <= 0x27bf)
                    {
                    }
                    else
                    {
                        for (int j = 0; j < max; j++)
                        {
                            if (buf[j] == c)
                                goto label1;
                        }
                        buf[max] = c;
                        max++;
                    label1:;
                    }
                    i++;
                }
                else
                {
                    int a = FindEmoji(str, i, charLen, null);
                    if (a > 0)
                    {
                        i += a;
                    }
                    else
                    {
                        i++;
                        for (int j = 0; j < max; j++)
                        {
                            if (buf[j] == c)
                                goto label1;
                        }
                        buf[max] = c;
                        max++;
                    label1:;
                    }
                }
                pos++;
            }
            return max;
        }
    }
}