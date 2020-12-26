using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class StringEx
    {
        /// <summary>
        /// 不会产生网格的字符
        /// </summary>
        protected static char[] key_noMesh = new char[] { ' ', '\n' };//排除\r
        public struct EmojiInfo
        {
            public string Content;
            public int[] lens;
            public int[] uvs;
            public int emojiCount;
        }
        string target;
        char[] buffer;
        public EmojiInfo noEmojiInfo;
        public EmojiInfo meshInfo;
        public bool HaveLabel { get; private set; }
        /// <summary>
        /// 剔除表情符的字符串,用于TextGenerator生成网格
        /// </summary>
        public string noEmoji { 
            get => noEmojiInfo.Content; 
        }
        /// <summary>
        /// 剔除标签的字符串
        /// </summary>
        public string noLable;
        public int EmojiCount { get => noEmojiInfo.emojiCount; }
        public int Length { get; private set; }
        public bool RichText { get; private set; }
        public string FullString { get => target; }
        public string this[int index] {
            get 
            {
                if (noEmojiInfo.lens == null)
                    return "";
                int c = noEmojiInfo.lens[index];
                return noEmojiInfo.Content.Substring(index, c);
            }
        }
        public StringEx(string str, bool richText)
        {
            RichText = richText;
            target = str;
            Analysis();
        }
        void Analysis()
        {
            if (target != null & target != "")
            {
                buffer = target.ToCharArray();
                noEmojiInfo = DeleteEmoji(target);
                if (RichText)
                {
                    noLable = DeleteLabel(target);
                    meshInfo = DeleteEmoji(noLable);
                }
                else
                {
                    noLable = target;
                    meshInfo = noEmojiInfo;
                }
                Length = noEmojiInfo.lens.Length;
            }
            else
            {
                noLable = target;
                noEmojiInfo.Content = target;
                noEmojiInfo.lens = null;
                noEmojiInfo.emojiCount = 0;
                Length = 0;
            }
        }
        EmojiInfo DeleteEmoji(string buf)
        {
            int[] ids = StringInfo.ParseCombiningCharacters(buf);
            int l = buf.Length;
            int max = ids.Length - 1;
            for (int i = 0; i < max; i++)
            {
                ids[i] = ids[i + 1] - ids[i];
            }
            ids[max] = l - ids[max];

            char[] tmp = new char[ids.Length];
            int[] uvs = new int[ids.Length];
            int s = 0;
            int count = 0;
            for (int i = 0; i < ids.Length; i++)
            {
                int len = ids[i];
                if (len > 1)
                {
                    int c = EmojiMap.FindEmoji(buf, s, len);
                    uvs[i] = c;
                    if (c >= 0)
                    {
                        tmp[i] = '@';
                        count++;
                    }
                    else
                    {
                        tmp[i] = buf[s];
                    }
                }
                else
                {
                    tmp[i] = buf[s];
                    uvs[i] = -1;
                }
                s += len;
            }
            EmojiInfo info = new EmojiInfo();
            info.Content = new string(tmp);
            info.lens = ids;
            info.uvs = uvs;
            info.emojiCount = count;
            return info;
        }
        public static bool HaveMesh(char ch)
        {
            for (int j = 0; j < key_noMesh.Length; j++)
            {
                if (key_noMesh[j] == ch)
                {
                    return false;
                }
            }
            return true;
        }
        public string SubstringByTextElements(int index, int len)
        {
            if (noEmojiInfo.lens == null)
                return "";
            int max = noEmojiInfo.lens.Length;
            if (index >= max)
                return "";
            if (index+len>max)
            {
                len = max - index;
                if(len<=0)
                {
                    return "";
                }
            }
            return new StringInfo(target).SubstringByTextElements(index, len);
        }
        public void RmoveTextElements(int index, int len)
        {
            if (noEmojiInfo.lens == null)
                return;
            int all = noEmojiInfo.lens.Length;
            if (len >= all)
            {
                target = "";
                Analysis();
                return;
            }
            var si = new StringInfo(target);
            string frist = si.SubstringByTextElements(0, index);
            if(index+len>=all)
            {
                target =frist;
                Analysis();
                return;
            }
            string end = si.SubstringByTextElements(index+len);
            target = frist  + end;
            Analysis();
        }
        public int InsertTextElements(int index, string str)
        {
            if(noEmojiInfo.lens==null)
            {
                target = str;
                Analysis();
                return noEmojiInfo.lens.Length;
            }
            int len = noEmojiInfo.lens.Length;
            if(index>=len)
            {
                target = target + str;
            }
            else
            {
                var si = new StringInfo(target);
                string frist = si.SubstringByTextElements(0, index);
                string end = si.SubstringByTextElements(index);
                target = frist + str + end;
            }
            Analysis();
            return noEmojiInfo.lens.Length - len;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="richText"></param>
        public void Reset(string str,bool richText)
        {
            if (RichText == richText)
                if (str == target)
                    return;
            RichText = richText;
            target = str;
            Analysis();
            split = false;
        }
        public unsafe bool GetEmojiUV(HVertex* vertex, int start, int index)
        {
            if (noEmojiInfo.lens == null)
                return false;
            if (noEmojiInfo.uvs == null)
            {
                Debug.Log("error");
                return false;
            }
            if (index < 0)
            {
                Debug.Log(index);
                return false;
            }
            if (index >= noEmojiInfo.uvs.Length)
            {
                Debug.Log(index);
                return false;
            }
            int i = noEmojiInfo.uvs[index];
            if (i >= 0)
            {
                var u = EmojiMap.GetUV(noEmojiInfo.lens[index], i);
                vertex[start].uv = u.uv0;
                start++;
                vertex[start].uv = u.uv1;
                start++;
                vertex[start].uv = u.uv2;
                start++;
                vertex[start].uv = u.uv3;
                return true;
            }
            return false;
        }
        #region  删除富文本中的标签
        public struct LableInfo
        {
            public int Start;
            public int Length;
            public int Type;
            public int Lable;
            public string Text;
            public Color32 color;
            public int fontSize;
            public FontStyle style;
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

        static string[] LableStart = new string[] { sb, si, sSize, sColor, sMaterial, sQuad };

        static string[] LableEnd = new string[] { eb, ei, eSize, eColor, eMaterial, eQuad };
        #endregion
        static StringBuilder tmp;
        List<LableInfo> lables;
        /// <summary>
        /// 删除富文本中的标签
        /// </summary>
        /// <param name="str">富文本</param>
        /// <returns></returns>
        public string DeleteLabel(string str)
        {
            HaveLabel = false;
            if (tmp == null)
                tmp = new StringBuilder();
            else
                tmp.Clear();
            if (lables == null)
                lables = new List<LableInfo>();
            else lables.Clear();
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
                {
                    return str; 
                }
            }
            if (lables.Count == 0)
            {
                return str; 
            }
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
            if (ol > 0)
                tmp.Append(str, s, ol);
            HaveLabel = true;
            lable.Start = 0;
            lable.Length = 0;
            lable.Lable = -1;
            lables.Insert(0, lable);
            lable.Start = str.Length;
            lable.Length = 0;
            lable.Lable = -1;
            lables.Add(lable);
            return tmp.ToString();
        }
        static bool CheckLable(List<LableInfo> list, ref int index)
        {
            LableInfo tmp = list[index];
            index++;
            if (index >= list.Count)
                return false;
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
        static bool GetLable(string str, int index, ref LableInfo info)
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
                                if (c == equals)
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
                                else if (c == end)
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
        static bool Compare(string src, int index, string tar)
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
        #endregion
        public List<LableInfo> lableInfos;
        bool split = false;
        int baseSize;
        Color32 baseColor;
        FontStyle baseStyle;
        public void SplitLableString(int fontSize, Color32 color, FontStyle style)
        {
            if (split)//防止重复计算
                if (fontSize == baseSize)
                    if (color.a == baseColor.a)
                        if (color.r == baseColor.r)
                            if (color.g == baseColor.g)
                                if (color.b == baseColor.b)
                                    if (style == baseStyle)
                                        return;
            baseSize = fontSize;
            baseColor = color;
            baseStyle = style;
            if(HaveLabel)
            {
                if (lableInfos == null)
                    lableInfos = new List<LableInfo>();
                else lableInfos.Clear();
                LableInfo info = new LableInfo();
                info.fontSize = fontSize;
                info.color = color;
                info.style = style;
                AnalysisLabel(0, info);
                split = true;
            }
        }
        int AnalysisLabel(int index, LableInfo info)
        {
            LableInfo Buf = lables[index];
            Buf.fontSize = info.fontSize;
            Buf.style = info.style;
            Buf.color = info.color;
            int e = index + 1;
            switch(Buf.Lable)
            {
                case 0:
                    Buf.style = FontStyle.Bold;
                    break;
                case 1:
                    Buf.style = FontStyle.Italic;
                    break;
                case 2:
                    {
                        int c = 0;
                        int s = Buf.Start + 6;
                        string str = target.Substring(s, Buf.Length - 7);
                        if (int.TryParse(str, out c))
                        {
                            Buf.fontSize = c;
                        }
                    }
                    break;
                case 3:
                    {
                        int s = Buf.Start + 8;
                        string str = target.Substring(s, Buf.Length - 9);
                        Buf.color = str.HexToColor();
                    }
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
            for(int i=index;i<lables.Count;i++)
            {
                if (Buf.Lable == lables[e].Lable)
                {
                    int s = lables[index].Start + lables[index].Length;
                    Buf.Text = target.Substring(s, lables[e].Start - s);
                    lableInfos.Add(Buf);
                    return e;
                }
                else
                {
                    int s = lables[index].Start + lables[index].Length;
                    Buf.Text = target.Substring(s, lables[e].Start - s);
                    lableInfos.Add(Buf);
                }
                e = AnalysisLabel(index + 1, Buf);
                index = e;
                e++;
                if (e >= lables.Count)
                    break;
            }
            return e;
        }
    }
}
