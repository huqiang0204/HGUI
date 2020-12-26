using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct TextLineInfo
    {
        public int CharStart;
        public int CharEnd;
        public float Width;
        public float fontSize;
        public float High;
        public float topY;
        public float downY;
        public float Left;
        public float Right;
        public float minY;
        public float maxY;
        public int Row;
    }
    public struct CharacterInfoEx
    {
        public CharacterInfo info;
        public Color32 color;
        public int type;
        public float Left;
        public float Center;
        public float Right;
        public int VertexIndex;
        public int fontSize;
        public char ch;
    }
    public struct PopulateStringInfo
    {
        public char ch;
        public int fontSize;
        public FontStyle style;
        public int type;
        public int VertexIndex;
    }
    struct ContextText
    {
        public List<HText> buf;
        public Font font;
        public bool rebuild;
        //public int rebuildEnd;
    }
    public class HTextGenerator
    {
        /// <summary>
        /// 获取计算后的字符信息
        /// </summary>
        /// <param name="infos"></param>
        public static void GetPopulateInfo(List<PopulateStringInfo> infos)
        {
            infos.Clear();
            PopulateStringInfo info = new PopulateStringInfo();
            for(int i=0;i<MaxChar;i++)
            {
                info.ch = characters[i].ch;
                info.fontSize = characters[i].fontSize;
                info.style = characters[i].info.style;
                info.type = characters[i].type;
                info.VertexIndex = characters[i].VertexIndex;
                infos.Add(info);
            }
        }
        /// <summary>
        /// 默认行间隔
        /// </summary>
        public static float LineSpacing = 1.14f;
        /// <summary>
        /// 当前字符串计算的顶点
        /// </summary>
        public static List<HVertex> vertices = new List<HVertex>();
        /// <summary>
        /// 当前字符串计算的字体纹理三角形
        /// </summary>
        public static List<int> Triangle1 = new List<int>();
        /// <summary>
        /// 当前字符串计算的表情符纹理三角形
        /// </summary>
        public static List<int> Triangle2 = new List<int>();
        /// <summary>
        /// 当前字符串计算的自定义纹理三角形
        /// </summary>
        public static List<int> Triangle3 = new List<int>();
        static CharacterInfoEx[] characters = new CharacterInfoEx[8192];
        /// <summary>
        /// 总计字符数
        /// </summary>
        public static int MaxChar { get; private set; }
        static TextLineInfo[] lineInfos = new TextLineInfo[2048];
        /// <summary>
        /// 挡墙自定义字体信息
        /// </summary>
        public static CustomFont customFont;
        /// <summary>
        /// 复制文本行信息
        /// </summary>
        /// <param name="target"></param>
        public static void CopyLinesInfo(List<TextLineInfo> target)
        {
            for (int i = 0; i < MaxLine; i++)
                target.Add(lineInfos[i]);
        }
        /// <summary>
        /// 复制字符信息
        /// </summary>
        /// <param name="target"></param>
        public static void CopyCharsInfo(List<CharacterInfoEx> target)
        {
            for (int i = 0; i < MaxChar; i++)
                target.Add(characters[i]);
        }
        /// <summary>
        /// 当前计算的字符串最大行数
        /// </summary>
        public static int MaxLine { get; private set; }
        static TextLineInfo lineInfo = new TextLineInfo();
        /// <summary>
        /// 单次计算字符串长度不得超过8000
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        public static void Populate(StringEx str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return;
            Layout(str, ref setting);
            CreateVertex(str, ref setting);
        }
        static void Layout(StringEx str, ref TextGenerationSettings setting)
        {
            MaxChar = 0;
            MaxLine = 0;
            if (setting.richText)
            {
                if (str.HaveLabel)
                {
                    lineInfo = new TextLineInfo();
                    str.SplitLableString(setting.fontSize, setting.color, setting.fontStyle);
                    var info = str.lableInfos;
                    var cur = setting;
                    int s = 0;
                    for (int i = 0; i < info.Count; i++)
                    {
                        cur.fontSize = info[i].fontSize;
                        cur.color = info[i].color;
                        cur.fontStyle = info[i].style;
                        var son = new StringEx(info[i].Text, false);
                        LayoutRichText(son, ref cur, s);
                        s += son.Length;
                    }
                    int end = str.meshInfo.lens.Length - 1;
                    if (MaxLine > 0)
                    {
                        if (lineInfos[MaxLine - 1].CharEnd < end)
                        {
                            lineInfo.CharEnd = end;
                            lineInfos[MaxLine] = lineInfo;
                            MaxLine++;
                        }
                    }
                    else
                    {
                        lineInfo.CharEnd = end;
                        lineInfos[MaxLine] = lineInfo;
                        MaxLine++;
                    }
                }
                else
                {
                    LayoutText(str, ref setting);
                }
            }
            else
            {
                LayoutText(str, ref setting);
            }
        }
        static void LayoutRichText(StringEx str, ref TextGenerationSettings setting, int start)
        {
            int[] eles = str.noEmojiInfo.lens;
            if (eles == null)
                return;
            var font = setting.font;
            string full = str.FullString;
            string no = str.noEmoji;
            font.RequestCharactersInTexture(no, setting.fontSize, setting.fontStyle);

            CharacterInfoEx info = new CharacterInfoEx();
            info.color = setting.color;
            CharUV uV = new CharUV();
            int s = 0;
            float w = setting.generationExtents.x;
            float lw = lineInfo.Width;
            if (lineInfo.fontSize < setting.fontSize)
                lineInfo.fontSize = setting.fontSize;
            for (int i = 0; i < eles.Length; i++)
            {
                int len = eles[i];
                if (no[i] == '\n')//行结束
                {
                    font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    info.type = -3;
                    info.info.advance = 0;
                    lineInfo.CharEnd = start + i - 1;
                    lineInfos[MaxLine] = lineInfo;
                    MaxLine++;
                    lineInfo.CharStart = start + i;
                    lineInfo.minY = 0;
                    lineInfo.maxY = 0;
                    lineInfo.fontSize = setting.fontSize;
                    lineInfo.Width = 0;
                    lw = 0;
                    s++;
                }
                else
                {
                    if (len == 1)
                    {
                        if (no[i] == ' ')
                            info.type = -2;
                        else info.type = 0;
                        if (customFont != null)
                        {
                            if (customFont.GetCharacterInfo(no[i], ref info.info, setting.fontSize))
                            {
                                info.type = 1;
                            }
                            else
                                font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                        }
                        else
                            font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    }
                    else
                    {
                        font.GetCharacterInfo('@', out info.info, setting.fontSize, setting.fontStyle);
                        int index = EmojiMap.FindEmojiInfo(full, s, len, ref uV);
                        if (index >= 0)
                        {
                            info.type = -1;
                            info.info.uvBottomLeft = uV.uv3;
                            info.info.uvTopLeft = uV.uv0;
                            info.info.uvTopRight = uV.uv1;
                            info.info.uvBottomRight = uV.uv2;
                        }
                        else info.type = 0;
                    }
                    if (setting.horizontalOverflow == HorizontalWrapMode.Wrap)
                    {
                        if (lw + info.info.advance > w)
                        {
                            lineInfo.CharEnd = start + i - 1;
                            lineInfos[MaxLine] = lineInfo;
                            MaxLine++;
                            lineInfo.CharStart = start + i;
                            lineInfo.minY = 0;
                            lineInfo.maxY = 0;
                            lineInfo.fontSize = setting.fontSize;
                            lw = info.info.advance;
                        }
                        else lw += info.info.advance;
                    }
                    else lw += info.info.advance;
                    if (info.info.maxY > lineInfo.maxY)
                        lineInfo.maxY = info.info.maxY;
                    if (info.info.minY < lineInfo.minY)
                        lineInfo.minY = info.info.minY;
                    lineInfo.Width = lw;
                    s += len;
                }
                characters[MaxChar] = info;
                characters[MaxChar].fontSize = setting.fontSize;
                characters[MaxChar].ch = no[i];
                MaxChar++;
            }
        }
        /// <summary>
        /// 字符布局
        /// </summary>
        static void LayoutText(StringEx str, ref TextGenerationSettings setting)
        {
            int[] eles = str.noEmojiInfo.lens;
            if (eles == null)
                return;
            lineInfo = new TextLineInfo();
            var font = setting.font;
            string full = str.FullString;
            string no = str.noEmoji;
            font.RequestCharactersInTexture(no, setting.fontSize, setting.fontStyle);

            CharacterInfoEx info = new CharacterInfoEx();
            info.color = setting.color;
            CharUV uV = new CharUV();
            int s = 0;
            float w = setting.generationExtents.x;
            float lw = 0;
            lineInfo.fontSize = setting.fontSize;
            for (int i = 0; i < eles.Length; i++)
            {
                int len = eles[i];
                if (no[i] == '\n')//行结束
                {
                    font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    info.type = -3;
                    info.info.glyphWidth = 0;
                    info.info.advance = 0;
                    lineInfo.CharEnd = i - 1;
                    lineInfos[MaxLine] = lineInfo;
                    MaxLine++;
                    lineInfo.CharStart = i;
                    lineInfo.minY = 0;
                    lineInfo.maxY = 0;
                    lw = 0;
                    s++;
                }
                else
                {
                    if (len == 1)
                    {
                        if (no[i] == ' ')
                            info.type = -2;
                        else info.type = 0;
                        if (customFont != null)
                        {
                            if (customFont.GetCharacterInfo(no[i], ref info.info, setting.fontSize))
                            {
                                info.type = 1;
                            }
                            else
                                font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                        }
                        else
                            font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    }
                    else
                    {
                        font.GetCharacterInfo('@', out info.info, setting.fontSize, setting.fontStyle);
                        int index = EmojiMap.FindEmojiInfo(full, s, len, ref uV);
                        if (index >= 0)
                        {
                            info.type = -1;
                            info.info.uvBottomLeft = uV.uv3;
                            info.info.uvTopLeft = uV.uv0;
                            info.info.uvTopRight = uV.uv1;
                            info.info.uvBottomRight = uV.uv2;
                        }
                        else info.type = 0;
                    }
                    if (setting.horizontalOverflow == HorizontalWrapMode.Wrap)
                    {
                        if (lw + info.info.advance > w)
                        {
                            lineInfo.CharEnd = i - 1;
                            lineInfos[MaxLine] = lineInfo;
                            MaxLine++;
                            lineInfo.CharStart = i;
                            lineInfo.minY = 0;
                            lineInfo.maxY = 0;
                            lw = info.info.advance;
                        }
                        else lw += info.info.advance;
                    }
                    else lw += info.info.advance;
                    if (info.info.maxY > lineInfo.maxY)
                        lineInfo.maxY = info.info.maxY;
                    if (info.info.minY < lineInfo.minY)
                        lineInfo.minY = info.info.minY;
                    lineInfo.Width = lw;
                    s += len;
                }
                characters[MaxChar] = info;
                characters[MaxChar].fontSize = setting.fontSize;
                characters[MaxChar].ch = no[i];
                MaxChar++;
            }
            int end = eles.Length - 1;
            if (MaxLine > 0)
            {
                if (lineInfos[MaxLine - 1].CharEnd < end)
                {
                    lineInfo.CharEnd = end;
                    lineInfos[MaxLine] = lineInfo;
                    MaxLine++;
                }
            }
            else
            {
                lineInfo.CharEnd = end;
                lineInfos[MaxLine] = lineInfo;
                MaxLine++;
            }
        }
        static void LayoutText(String no, ref TextGenerationSettings setting)
        {
            MaxChar = 0;
            MaxLine = 0;
            lineInfo = new TextLineInfo();
            var font = setting.font;
            font.RequestCharactersInTexture(no, setting.fontSize, setting.fontStyle);
            CharacterInfoEx info = new CharacterInfoEx();
            info.color = setting.color;
            float w = setting.generationExtents.x;
            float lw = 0;
            lineInfo.fontSize = setting.fontSize;
            for (int i = 0; i < no.Length; i++)
            {
                if (no[i] == '\n')//行结束
                {
                    font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    info.type = 0;
                    lineInfo.CharEnd = i - 1;
                    lineInfos[MaxLine] = lineInfo;
                    MaxLine++;
                    lineInfo.CharStart = i;
                    lineInfo.minY = 0;
                    lineInfo.maxY = 0;
                    lw = 0;
                }
                else
                {
                    if (no[i] == ' ')
                        info.type = -2;
                    else info.type = 0;
                    if (customFont != null)
                    {
                        if (customFont.GetCharacterInfo(no[i], ref info.info, setting.fontSize))
                        {
                            info.type = 1;
                        }
                        else
                            font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    }
                    else
                        font.GetCharacterInfo(no[i], out info.info, setting.fontSize, setting.fontStyle);
                    if (setting.horizontalOverflow == HorizontalWrapMode.Wrap)
                    {
                        if (lw + info.info.advance > w)
                        {
                            lineInfo.CharEnd = i - 1;
                            lineInfos[MaxLine] = lineInfo;
                            MaxLine++;
                            lw = info.info.advance;
                            lineInfo.CharStart = i;
                            lineInfo.minY = 0;
                            lineInfo.minY = 0;
                        }
                        else lw += info.info.advance;
                    }
                    else lw += info.info.advance;
                    if (info.info.maxY > lineInfo.maxY)
                        lineInfo.maxY = info.info.maxY;
                    if (info.info.minY < lineInfo.minY)
                        lineInfo.minY = info.info.minY;
                    lineInfo.Width = lw;
                }
                characters[MaxChar] = info;
                characters[MaxChar].fontSize = setting.fontSize;
                characters[MaxChar].ch = no[i];
                MaxChar++;
            }
            int end = no.Length - 1;
            if (MaxLine > 0)
            {
                if (lineInfos[MaxLine - 1].CharEnd < end)
                {
                    lineInfo.CharEnd = end;
                    lineInfos[MaxLine] = lineInfo;
                    MaxLine++;
                }
            }
            else
            {
                lineInfo.CharEnd = end;
                lineInfos[MaxLine] = lineInfo;
                MaxLine++;
            }
        }
        /// <summary>
        /// 创建顶点
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        public static void CreateVertex(StringEx str, ref TextGenerationSettings setting)
        {
            vertices.Clear();
            Triangle1.Clear();
            Triangle2.Clear();
            Triangle3.Clear();
            if (str == null)
                return;
            for (int i = 0; i < MaxLine; i++)
            {
                lineInfos[i].Row = i;
                lineInfos[i].High = lineInfos[i].fontSize * setting.lineSpacing * LineSpacing;
            }
            switch (setting.textAnchor)
            {
                case TextAnchor.UpperLeft:
                    LayoutUpperLeft(ref setting);
                    break;
                case TextAnchor.MiddleLeft:
                    LayoutMiddleLeft(ref setting);
                    break;
                case TextAnchor.LowerLeft:
                    LayoutLowerLeft(ref setting);
                    break;
                case TextAnchor.UpperCenter:
                    LayoutUpperCenter(ref setting);
                    break;
                case TextAnchor.MiddleCenter:
                    LayoutMiddleCenter(ref setting);
                    break;
                case TextAnchor.LowerCenter:
                    LayoutLowerCenter(ref setting);
                    break;
                case TextAnchor.UpperRight:
                    LayoutUpperRight(ref setting);
                    break;
                case TextAnchor.MiddleRight:
                    LayoutMiddleRight(ref setting);
                    break;
                case TextAnchor.LowerRight:
                    LayoutLowerRight(ref setting);
                    break;
            }
        }
        static void LayoutUpperLeft(ref TextGenerationSettings setting)
        {
            float sx = setting.generationExtents.x * (setting.pivot.x - 1);
            float sy = setting.generationExtents.y * (1 - setting.pivot.y);
            float hy = setting.generationExtents.y * -0.5f;
            for (int i = 0; i < MaxLine; i++)
            {
                float h = lineInfos[i].High;
                if (setting.verticalOverflow == VerticalWrapMode.Truncate)
                {
                    if (sy - 0.5f * h < hy)
                        break;
                }
                LayoutLine(i, sx, sy);
                sy -= h;
            }
        }
        static void LayoutMiddleLeft(ref TextGenerationSettings setting)
        {
            float sx = setting.generationExtents.x * (setting.pivot.x - 1);
            if (setting.verticalOverflow == VerticalWrapMode.Overflow)
            {
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    hc += lineInfos[i].High;
                }
                float sy = hc * 0.5f;
                for (int i = 0; i < MaxLine; i++)
                {
                    LayoutLine(i, sx, sy);
                    sy -= lineInfos[i].High;
                }
            }
            else
            {
                float gh = setting.generationExtents.y;
                int count = MaxLine;
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    float h = lineInfos[i].High;
                    if (hc + h > gh)
                    {
                        count = i;
                        break;
                    }
                    hc += h;
                }
                float sy = hc * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    LayoutLine(i, sx, sy);
                    sy -= lineInfos[i].High;
                }
            }
        }
        static void LayoutLowerLeft(ref TextGenerationSettings setting)
        {
            float sx = setting.generationExtents.x * (setting.pivot.x - 1);
            float sy = setting.generationExtents.y * (1 - setting.pivot.y);
            float gh = setting.generationExtents.y;
            if (setting.verticalOverflow == VerticalWrapMode.Overflow)
            {
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    hc += lineInfos[i].High;
                }
                sy += hc - gh;
                for (int i = 0; i < MaxLine; i++)
                {
                    LayoutLine(i, sx, sy);
                    sy -= lineInfos[i].High;
                }
            }
            else
            {

                int count = MaxLine;
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    float h = lineInfos[i].High;
                    if (hc + h > gh)
                    {
                        count = i;
                        break;
                    }
                    hc += h;
                }
                sy += hc - gh;
                for (int i = 0; i < count; i++)
                {
                    LayoutLine(i, sx, sy);
                    sy -= lineInfos[i].High;
                }
            }
        }
        static void LayoutUpperCenter(ref TextGenerationSettings setting)
        {
            float sy = setting.generationExtents.y * (1 - setting.pivot.y);
            float hy = setting.generationExtents.y * -0.5f;
            for (int i = 0; i < MaxLine; i++)
            {
                float h = lineInfos[i].High;
                if (setting.verticalOverflow == VerticalWrapMode.Truncate)
                {
                    if (sy - 0.5f * h < hy)
                        break;
                }
                LayoutLine(i, lineInfos[i].Width * -0.5f, sy);
                sy -= h;
            }
        }
        static void LayoutMiddleCenter(ref TextGenerationSettings setting)
        {
            if (setting.verticalOverflow == VerticalWrapMode.Overflow)
            {
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    hc += lineInfos[i].High;
                }
                float sy = hc * 0.5f;
                for (int i = 0; i < MaxLine; i++)
                {
                    LayoutLine(i, lineInfos[i].Width * -0.5f, sy);
                    sy -= lineInfos[i].High;
                }
            }
            else
            {
                float gh = setting.generationExtents.y;
                int count = MaxLine;
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    float h = lineInfos[i].High;
                    if (hc + h > gh)
                    {
                        count = i;
                        break;
                    }
                    hc += h;
                }
                float sy = hc * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    LayoutLine(i, lineInfos[i].Width * -0.5f, sy);
                    sy -= lineInfos[i].High;
                }
            }
        }
        static void LayoutLowerCenter(ref TextGenerationSettings setting)
        {
            float sy = setting.generationExtents.y * (1 - setting.pivot.y);
            float gh = setting.generationExtents.y;
            if (setting.verticalOverflow == VerticalWrapMode.Overflow)
            {
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    hc += lineInfos[i].High;
                }
                sy += hc - gh;
                for (int i = 0; i < MaxLine; i++)
                {
                    LayoutLine(i, lineInfos[i].Width * -0.5f, sy);
                    sy -= lineInfos[i].High;
                }
            }
            else
            {
                int count = MaxLine;
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    float h = lineInfos[i].High;
                    if (hc + h > gh)
                    {
                        count = i;
                        break;
                    }
                    hc += h;
                }
                sy += hc - gh;
                for (int i = 0; i < count; i++)
                {
                    LayoutLine(i, lineInfos[i].Width * -0.5f, sy);
                    sy -= lineInfos[i].High;
                }
            }
        }
        static void LayoutUpperRight(ref TextGenerationSettings setting)
        {
            float sx = setting.generationExtents.x * 0.5f;
            float sy = setting.generationExtents.y * (1 - setting.pivot.y);
            float hy = setting.generationExtents.y * -0.5f;
            for (int i = 0; i < MaxLine; i++)
            {
                float h = lineInfos[i].High;
                if (setting.verticalOverflow == VerticalWrapMode.Truncate)
                {
                    if (sy - 0.5f * h < hy)
                        break;
                }
                LayoutLine(i, sx - lineInfos[i].Width, sy);
                sy -= h;
            }
        }
        static void LayoutMiddleRight(ref TextGenerationSettings setting)
        {
            float sx = setting.generationExtents.x * 0.5f;
            if (setting.verticalOverflow == VerticalWrapMode.Overflow)
            {
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    hc += lineInfos[i].High;
                }
                float sy = hc * 0.5f;
                for (int i = 0; i < MaxLine; i++)
                {
                    LayoutLine(i, sx - lineInfos[i].Width, sy);
                    sy -= lineInfos[i].High;
                }
            }
            else
            {
                float gh = setting.generationExtents.y;
                int count = MaxLine;
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    float h = lineInfos[i].High;
                    if (hc + h > gh)
                    {
                        count = i;
                        break;
                    }
                    hc += h;
                }
                float sy = hc * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    LayoutLine(i, sx - lineInfos[i].Width, sy);
                    sy -= lineInfos[i].High;
                }
            }
        }
        static void LayoutLowerRight(ref TextGenerationSettings setting)
        {
            float sx = setting.generationExtents.x * 0.5f;
            float sy = setting.generationExtents.y * (1 - setting.pivot.y);
            float gh = setting.generationExtents.y;
            if (setting.verticalOverflow == VerticalWrapMode.Overflow)
            {
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    hc += lineInfos[i].High;
                }
                sy += hc - gh;
                for (int i = 0; i < MaxLine; i++)
                {
                    LayoutLine(i, sx - lineInfos[i].Width, sy);
                    sy -= lineInfos[i].High;
                }
            }
            else
            {
                int count = MaxLine;
                float hc = 0;
                for (int i = 0; i < MaxLine; i++)
                {
                    float h = lineInfos[i].High;
                    if (hc + h > gh)
                    {
                        count = i;
                        break;
                    }
                    hc += h;
                }
                sy += hc - gh;
                for (int i = 0; i < count; i++)
                {
                    LayoutLine(i, sx - lineInfos[i].Width, sy);
                    sy -= lineInfos[i].High;
                }
            }
        }
        static void LayoutLine(int index, float startX, float startY)
        {
            var line = lineInfos[index];
            lineInfos[index].topY = startY;
            float down = lineInfos[index].downY = startY - line.High;
            float sy = down + line.minY + 0.24f * line.High;
            float sx = startX;
            HVertex vertex = new HVertex();

            for (int i = line.CharStart; i <= line.CharEnd; i++)
            {
                var ci = characters[i];
                float sw = ci.info.glyphWidth;
                float rx = sw * 0.5f;
                float cx = ci.info.minX + rx;
                rx *= 0.95f;//给光标留间隙
                float lx = -rx;
                lx += sx + cx;
                rx += sx + cx;
                if (ci.type > -2)//有网格
                {
                    int c = vertices.Count;
                    vertex.uv4.x = 1;
                    vertex.uv4.y = 1;
                    if (ci.type == -1)//表情符
                    {
                        vertex.picture = 1;
                        vertex.fillColor = 0.2f;
                        AddTris(Triangle2, c);
                    }
                    else if (ci.type == 1)//自定义字符
                    {
                        vertex.picture = 2;
                        vertex.fillColor = 0.2f;
                        AddTris(Triangle3, c);
                    }
                    else
                    {
                        vertex.picture = 0;
                        vertex.fillColor = 0.1f;
                        AddTris(Triangle1, c);
                    }

                    vertex.color = ci.color;

                    float dy = ci.info.minY - line.minY;
                    float ty = ci.info.maxY - line.minY;
                    ty *= 0.95f;
                    dy *= 0.95f;
                    ty += sy;
                    dy += sy;
                    characters[i].VertexIndex = vertices.Count / 4;
                    AddVertex(lx, rx, dy, ty, ref vertex, ref ci);
                    sx += ci.info.advance;
                }
                else
                {
                    sx += ci.info.advance;
                    characters[i].VertexIndex = -1;
                }
                characters[i].Left = lx;
                characters[i].Right = rx;
                characters[i].Center = (rx - lx) * 0.5f + lx;
            }
            lineInfos[index].Left = startX;
            lineInfos[index].Right = sx;
        }
        static void AddVertex(float lx, float rx, float dy, float ty, ref HVertex vertex, ref CharacterInfoEx info)
        {
            vertex.position.x = lx;
            vertex.position.y = dy;
            vertex.uv = info.info.uvBottomLeft;
            vertices.Add(vertex);

            vertex.position.x = lx;
            vertex.position.y = ty;
            vertex.uv = info.info.uvTopLeft;
            vertices.Add(vertex);

            vertex.position.x = rx;
            vertex.position.y = ty;
            vertex.uv = info.info.uvTopRight;
            vertices.Add(vertex);

            vertex.position.x = rx;
            vertex.position.y = dy;
            vertex.uv = info.info.uvBottomRight;
            vertices.Add(vertex);
        }
        /// <summary>
        /// 添加三角形索引
        /// </summary>
        /// <param name="tris"></param>
        /// <param name="s"></param>
        public static void AddTris(List<int> tris, int s)
        {
            tris.Add(s);
            tris.Add(s + 1);
            tris.Add(s + 2);
            tris.Add(s + 2);
            tris.Add(s + 3);
            tris.Add(s);
        }
        /// <summary>
        /// 获取文本的宽度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static float GetPreferredWidth(StringEx str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return setting.generationExtents.x;
            Layout(str, ref setting);
            float x = 0;
            for (int i = 0; i < MaxLine; i++)
            {
                if (x < lineInfos[i].Width)
                    x = lineInfos[i].Width;
            }
            return x;
        }
        /// <summary>
        /// 获取文本的高度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static float GetPreferredHeight(StringEx str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return setting.generationExtents.y;
            Layout(str, ref setting);
            float y = 0;
            for (int i = 0; i < MaxLine; i++)
            {
                y += lineInfos[i].fontSize * setting.lineSpacing * LineSpacing;
            }
            return y;
        }
        /// <summary>
        /// 获取文本的尺寸
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Vector2 GetPreferredSize(StringEx str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return new Vector2(setting.generationExtents.x, setting.generationExtents.y);
            Layout(str, ref setting);
            float x = 0;
            float y = 0;
            for (int i = 0; i < MaxLine; i++)
            {
                if (x < lineInfos[i].Width)
                    x = lineInfos[i].Width;
                y += lineInfos[i].fontSize * setting.lineSpacing * LineSpacing;
            }
            return new Vector2(x, y);
        }
        /// <summary>
        /// 获取文本的宽度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static float GetPreferredWidth(String str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return setting.generationExtents.x;
            LayoutText(str, ref setting);
            float x = 0;
            for (int i = 0; i < MaxLine; i++)
            {
                if (x < lineInfos[i].Width)
                    x = lineInfos[i].Width;
            }
            return x;
        }
        /// <summary>
        /// 获取文本的高度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static float GetPreferredHeight(String str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return setting.generationExtents.y;
            LayoutText(str, ref setting);
            float y = 0;
            for (int i = 0; i < MaxLine; i++)
            {
                y += lineInfos[i].fontSize * LineSpacing * setting.lineSpacing;
            }
            return y;
        }
        /// <summary>
        /// 获取文本尺寸
        /// </summary>
        /// <param name="str"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Vector2 GetPreferredSize(String str, ref TextGenerationSettings setting)
        {
            var font = setting.font;
            if (font == null)
                return new Vector2(setting.generationExtents.x, setting.generationExtents.y);
            LayoutText(str, ref setting);
            float x = 0;
            float y = 0;
            for (int i = 0; i < MaxLine; i++)
            {
                if (x < lineInfos[i].Width)
                    x = lineInfos[i].Width;
                y += lineInfos[i].fontSize * LineSpacing * setting.lineSpacing;
            }
            return new Vector2(x, y);
        }
        static ContextText[] TextBuffer = new ContextText[256];
        static int top = 0;
        /// <summary>
        /// 当纹理被重建后重建纹理UV
        /// </summary>
        public static void RebuildUV()
        {
            for (int i = 0; i < top; i++)
            {
                if (TextBuffer[i].rebuild)
                {
                //label:;
                    TextBuffer[i].rebuild = false;
                    RebuildUV(TextBuffer[i].font, TextBuffer[i].buf);
                    //if(TextBuffer[i].rebuild)
                    //    goto label;
                }
            }
        }
        static void RebuildUV(Font font, List<HText> texts)
        {
            CharacterInfo info = new CharacterInfo();
            for (int i = 0; i < texts.Count; i++)
            {
                RequestTexture(font, texts[i]);
                var pps = texts[i].populates;
                var vert = texts[i].vertInfo;
                for (int j = 0; j < pps.Count; j++)
                {
                    var pi = pps[j];
                    if (pi.type == 0)
                    {
                        if (font.GetCharacterInfo(pi.ch, out info, pi.fontSize, pi.style))
                        {
                            int index = pi.VertexIndex * 4;
                            unsafe
                            {
                                var hv = vert.Addr;
                                hv[index].uv = info.uvBottomLeft;
                                index++;
                                hv[index].uv = info.uvTopLeft;
                                index++;
                                hv[index].uv = info.uvTopRight;
                                index++;
                                hv[index].uv = info.uvBottomRight;
                            }
                        }
                        else 
                        {
                            return;
                        }
                    }
                }
            }
        }
        static void RequestTexture(Font font, HText text)
        {
            if (text.m_richText)
            {
                StringEx str = text.stringEx;
                if (str.HaveLabel)
                {
                    var info = str.lableInfos;
                    if (info != null)
                    {
                        for (int i = 0; i < info.Count; i++)
                        {
                            var son = new StringEx(info[i].Text, false);
                            font.RequestCharactersInTexture(son.noEmoji, info[i].fontSize, info[i].style);
                        }
                        return;
                    }
                }
            }
            font.RequestCharactersInTexture(text.stringEx.noEmoji,text.m_fontSize,text.m_fontStyle);
        }
        /// <summary>
        /// 添加当前帧文本与字体的关联
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        public static void AddContext(HText text ,Font font)
        {
            for (int i = 0; i < top; i++)
            {
                if (TextBuffer[i].font == font)
                {
                    TextBuffer[i].buf.Add(text);
                    return;
                }
            }
            TextBuffer[top].font = font;
            if (TextBuffer[top].buf == null)
                TextBuffer[top].buf = new List<HText>();
            TextBuffer[top].buf.Add(text);
            TextBuffer[top].rebuild = false;
            top++;
        }
        /// <summary>
        /// 当前帧字体绘制结束
        /// </summary>
        public static void End()
        {
            for (int i = 0; i < top; i++)
            { 
                TextBuffer[i].rebuild = false;
                TextBuffer[i].font = null;
                TextBuffer[i].buf.Clear();
            }
            top = 0;
        }
        /// <summary>
        /// 字体纹理重建
        /// </summary>
        /// <param name="font"></param>
        public static void FontTextureRebuilt(Font font)
        {
            for (int i = 0; i < top; i++)
                if (TextBuffer[i].font == font)
                {
                    TextBuffer[i].rebuild = true;
                }
        }
    }
}
