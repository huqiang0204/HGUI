using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextControll
    {
        UILineInfo[] lines;//所有文本行
        UICharInfo[] uchars;
        /// <summary>
        /// 总计行数
        /// </summary>
        public int LineCount;
        /// <summary>
        /// 变动的行数，增加或减少
        /// </summary>
        public int LineChange = 0;
        /// <summary>
        /// 选中的开始行数
        /// </summary>
        public int StartLine;
        /// <summary>
        /// 选中的结束行数
        /// </summary>
        public int EndLine;
        /// <summary>
        /// 选中的开始索引
        /// </summary>
        public int StartIndex;
        /// <summary>
        /// 选中的结束索引
        /// </summary>
        public int EndIndex;
        int ShowStart;
        int ShowRow;
        /// <summary>
        /// 当前滚动偏移
        /// </summary>
        float ShowOffset;
        float PreferredHeight;
        float HeightChange;
        Vector2 BoxSize;
        int Style = 0;
        int LineOffset;
        EmojiString Text;
        public HText Context;
        public void SetFullString(EmojiString emojiString)
        {
            Text = emojiString;
            textChanged = true;
        }
        public void SetStartSelect(Vector3Int vector)
        {
            int s = lines[ShowStart].startCharIdx;
            StartIndex = s + vector.y;
            StartLine = ShowStart + vector.x;
            LineOffset = vector.z;
            Style = 0;
        }
        public void SetEndSelect(Vector3Int vector)
        {
            int s = lines[ShowStart].startCharIdx;
            EndIndex = s + vector.y;
            EndLine = ShowStart + vector.x;
            Style = 1;
        }
        public string GetFullString()
        {
            return Text.FullString;
        }
        /// <summary>
        /// 获取当前需要显示的字符串
        /// </summary>
        /// <returns></returns>
        public string GetShowString()
        {
            int s = lines[ShowStart].startCharIdx;
            int end = ShowStart + ShowRow + 1;
            int e = uchars.Length ;
            if (end < LineCount)
                e = lines[end].startCharIdx - 1;
            return Text.SubString(s, e - s);
        }
        /// <summary>
        /// 获取当前选中字符串
        /// </summary>
        /// <returns></returns>
        public string GetSelectString()
        {
            if (Style == 0)
                return "";
            int s = StartIndex;
            int e = EndIndex;
            if (s == e)
                return "";
            if (s > e)
            {
                int a = e;
                e = s;
                s = a;
            }
            return Text.SubString(s, e - s);
        }
        public void PointerMoveLeft()
        {
            if(StartIndex>0)
            {
                StartIndex--;
                if(StartIndex<lines[StartLine].startCharIdx)
                {
                    StartLine--;
                }
                LineOffset = StartIndex - lines[StartLine].startCharIdx;
                if (StartLine < ShowStart)
                {
                    ChangeShowLine(StartLine);
                }
            }
        }
        public void PointerMoveRight()
        {
            if (StartIndex < uchars.Length - 1)
            {
                StartIndex++;
                if (StartLine < lines.Length - 1)
                {
                    if (lines[StartLine + 1].startCharIdx <= StartIndex)
                    {
                        StartLine++;
                    }
                }
                LineOffset = StartIndex - lines[StartLine].startCharIdx;
                if (StartLine - ShowRow > ShowStart)
                {
                    ChangeShowLine(StartLine - ShowRow);
                }
            }
        }
        public void PointerMoveUp()
        {
            if (StartLine > 0)
            {
                int e = StartLine;
                StartLine--;
                int l = lines[e].startCharIdx - lines[StartLine].startCharIdx;
                if (LineOffset > l)
                    StartIndex = lines[StartLine].startCharIdx + l;
                else StartIndex = lines[StartLine].startCharIdx + LineOffset;
                if(StartLine<ShowStart)
                {
                    ChangeShowLine(StartLine);
                }
            }
        }
        public void PointerMoveDown()
        {
            if (StartLine < lines.Length - 1)
            {
                StartLine++;
                int l = uchars.Length - lines[StartLine].startCharIdx;
                if (StartLine < lines.Length - 1)
                    l = lines[StartLine + 1].startCharIdx - lines[StartLine].startCharIdx;
                if (LineOffset > l)
                    StartIndex = lines[StartLine].startCharIdx + l;
                else StartIndex = lines[StartLine].startCharIdx + LineOffset;
                if (StartLine - ShowRow > ShowStart)
                {
                    ChangeShowLine(StartLine - ShowRow);
                }
            }
        }
        public void PointerMoveStart()
        {
            Style = 0;
            StartIndex = 0;
        }
        public void PointerMoveEnd()
        {
            Style = 0;
            StartIndex= uchars.Length;
        }
        public void SelectAll()
        {
            Style = 1;
            StartIndex = 0;
            EndIndex = uchars.Length;
        }
        public void MoveToEnd()
        {
            ShowStart = LineCount - ShowRow;
            if (ShowStart < 0)
                ShowStart = 0;
        }
        public void AdjustToPoint()
        {
            int s = lines[ShowStart].startCharIdx;
            if (StartIndex < s)
            {
                for (int i = ShowStart; i >= 0; i--)
                {
                    if(lines[i].startCharIdx<=StartIndex)
                    {
                        ShowStart = i;
                        StartLine = i;
                        return;
                    }
                }
                ShowStart = 0;
                StartLine = 0;
                return;
            }
            int e = uchars.Length;
            int l = ShowStart + ShowRow + 1;
            if (l < LineCount)
                e = lines[l].startCharIdx - 1;
            if (StartIndex > e)
            {
                for (int i = ShowStart; i <LineCount; i++)
                {
                    if (lines[i].startCharIdx > StartIndex)
                    {
                        ShowStart = i - ShowRow;
                        StartLine = i - 1;
                        return;
                    }
                }
                ShowStart = LineCount - ShowRow;
                StartLine = LineCount - 1;
                return;
            }
        }
        public void AdjustStartLine()
        {
            int a = lines.Length - 1;
            for (int i = ShowStart; i < lines.Length; i++)
            {
                if (lines[i].startCharIdx > StartIndex)
                {
                    StartLine = i - 1;
                    break;
                }
            }
        }
        public void ChangeShowLine(int index)
        {
            ShowStart = index;
        }
        public void InsertString(string str)
        {
            DeleteSelectString();
            var es = new EmojiString(str);
            int c = es.Length;
            Text.Insert(StartIndex, es);
            StartIndex += c;
            textChanged = true;
        }
        public bool DeleteSelectString()
        {
            if (Style == 1)
            {
                Style = 0;
                int s = StartIndex;
                int e = EndIndex;
                if (s == e)
                    return false;
                Text.Remove(s, e - s);
                if (StartIndex < EndIndex)
                    StartIndex = EndIndex;
                textChanged = true;
                return true;
            }
            return false;
        }
        public bool DeleteLast()
        {
            if (DeleteSelectString())
                return true;
            if (StartIndex < 1)
                return false;
            StartIndex--;
            if (Text.Remove(StartIndex))
            {
                textChanged = true;
                return true;
            }
            return false;
        }
        public bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            if(Text.Remove(StartIndex))
            {
                textChanged = true;
                return true;
            }
            return false;
        }
        bool textChanged;
        /// <summary>
        /// 重新计算
        /// </summary>
        public void ReCalcul()
        {
            if(textChanged)
            {
                GetPreferredHeight();
                textChanged = false;
            }
        }
        /// <summary>
        /// 当前显示区域的百分比
        /// </summary>
        public float Percentage
        {
            get
            {
                float r = (float)ShowStart/ ((float)LineCount - (float)ShowRow);
                if (r < 0)
                    r = 0;
                else if (r > 1)
                    r = 1;
                return r;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                float a = (float)LineCount - (float)ShowRow;
                int c = (int)(a * value);
                ChangeShowLine(c);
            }
        }
        void GetPreferredHeight()
        {
            string str = Text.FilterString;
            TextGenerationSettings settings = new TextGenerationSettings();
            settings.resizeTextMinSize = 2;
            settings.resizeTextMaxSize = 40;
            settings.scaleFactor = 1;
            settings.textAnchor = TextAnchor.UpperLeft;
            settings.color = Color.white;
            settings.generationExtents = new Vector2(Context.SizeDelta.x, 0);
            settings.pivot = new Vector2(0.5f, 0.5f);
            settings.richText = false;
            settings.font = Context.Font;
            if (settings.font == null)
                settings.font = HText.DefaultFont;
            settings.fontSize = Context.m_fontSize;
            settings.fontStyle = FontStyle.Normal;
            settings.alignByGeometry = false;
            settings.updateBounds = false;
            settings.lineSpacing = Context.m_lineSpace;
            settings.horizontalOverflow = HorizontalWrapMode.Wrap;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            TextGenerator generator = HText.Generator;
            float h = generator.GetPreferredHeight(str, settings);
            HeightChange = PreferredHeight - h;
            PreferredHeight = h;
            lines= generator.lines.ToArray();
            uchars= generator.characters.ToArray();
            int lc = lines.Length;
            LineChange = lc - LineCount;
            LineCount = lc;
            float per = h / lc;
            ShowRow =(int)(Context.SizeDelta.y / per);
        }
        public int GetPressIndex()
        {
            return StartIndex - lines[ShowStart].startCharIdx;
        }
    }
}
