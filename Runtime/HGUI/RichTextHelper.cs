using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextProperty
    {
        public bool Bold;
        public bool Italic;
        public bool ReSize;
        public float Size;
        public bool ReColor;
        public Color32 color;
        public bool Http;
        public string Content;
    }
    /// <summary>
    /// 富文本帮助器
    /// </summary>
    public class RichTextHelper
    {
        struct LableInfo
        {
            public int Start;
            public int Length;
            public int Type;
            public int Lable;
        }
        #region unity内置标签
        public const char Start = '<';
        public const char end = '>';
        public const char equals = '=';
        public const string sb = "<b";
        public const string eb = "</b>";
        public const string si = "<i";
        public const string ei = "</i>";

        public const string sSize = "<size";
        public const string eSize = "</size>";
        public const string sColor = "<color";
        public const string eColor = "</color>";
        public const string sMaterial = "<material";
        public const string eMaterial = "</material>";
        public const string sQuad = "<quad";
        public const string eQuad = "</quad>";

        static string[] LableStart = new string[] { sb,  si,  sSize,  sColor,  sMaterial, sQuad };

        static string[] LableEnd = new string[] { eb, ei, eSize, eColor, eMaterial, eQuad};
        #endregion
        static StringBuilder tmp;
        static List<LableInfo> lables;
        /// <summary>
        /// 删除富文本中的标签
        /// </summary>
        /// <param name="str">富文本</param>
        /// <returns></returns>
        public static string DeleteLabel(string str)
        {
            if (tmp == null)
            {
                tmp = new StringBuilder();
                lables = new List<LableInfo>();
            }
            else 
            { 
                tmp.Clear();
                lables.Clear();
            }
            int index = 0;
            LableInfo lable = new LableInfo();
            for (int i = 0; i < str.Length; i++)
            {
                if (GetLable(str, index, ref lable))
                {
                    index = lable.Start + lable.Length;
                    lables.Add(lable);
                }
                else break;
            }
            int c = lables.Count;
            if (c % 2 > 0)//标签不是成对的
            {
                return str;
            }
            index = 0;
            for (int i = 0; i < lables.Count; i++)
            {
                if (index >= lables.Count)
                    break;
                if (!CheckLable(lables, ref index))
                    return str;
            }
            if (lables.Count == 0)
                return str;
            int s = 0;
            for (int i = 0; i < lables.Count; i++)
            {
                int a = lables[i].Start;
                int l = lables[i].Length;
                int len = a - s;
                if (len > 0)
                    tmp.Append(str, s, len);
                s = a + l;
            }
            int ol = str.Length - s;
            if(ol>0)
                tmp.Append(str, s, ol);
            return tmp.ToString();
        }
        static bool  CheckLable(List<LableInfo> list, ref int index)
        {
            LableInfo tmp = list[index];
            index++;
            if (list[index].Type == 0)
            {
                if (!CheckLable(list, ref index))
                    return false;
                if (index >= list.Count)
                    return false;
            }
            int c = list[index].Lable;
            index++;
            if (c == tmp.Lable)
            {
                return true;
            }
            return false;
        }
        static bool GetLable(string str, int index,ref LableInfo info)
        {
            int s = str.IndexOf(Start, index);
            if (s >= 0)
            {
                info.Start = s;
                info.Length = 1;
                int ss = s + 1;
                if (ss < str.Length)
                {
                    if (str[ss] == '/')//这是标签的结尾
                    {
                        for (int i = 0; i < LableEnd.Length; i++)
                        {
                            if (Compare(str, s, LableEnd[i]))
                            {
                                info.Start = s;
                                info.Length = LableEnd[i].Length;
                                info.Type = 1;
                                info.Lable = i;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < LableStart.Length; i++)
                        {
                            if (Compare(str, s, LableStart[i]))
                            {
                                int next = LableStart[i].Length;
                                char c = str[s + next];
                                info.Start = s;
                                info.Type = 0;
                                info.Lable = i;
                                if (c==equals)
                                {
                                    if (i < 2)
                                    {
                                        info.Length = LableStart[i].Length + 1;
                                    }
                                    else
                                    {
                                        int e = str.IndexOf(end, s + LableStart[i].Length);
                                        if (e >= 0)
                                        {
                                            info.Length = e - s + 1;
                                            return true;
                                        }
                                        else return false;
                                    }
                                }
                                else if(c==end)
                                {
                                    info.Length = LableStart[i].Length + 1;
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        static bool Compare(string src,int index,string tar)
        {
            int l = src.Length;
            int c = tar.Length;
            for (int i = 0; i < c; i++)
            {
                if (index >= l)
                    return false;
                if (src[index] != tar[i])
                {
                    return false;
                }
                index++;
            }
            return true;
        }
    }
}
