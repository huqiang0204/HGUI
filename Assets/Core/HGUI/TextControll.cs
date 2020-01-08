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
        public void SetFullString(EmojiString emojiString)
        {
            Text = emojiString;
        }
        int GetPressIndex(UserEvent callBack, UserAction action)
        {
            if (uchars == null)
                return 0;
            var pos = callBack.GlobalPosition;
            var scale = callBack.GlobalScale;
            float mx = action.CanPosition.x - pos.x;
            mx *= scale.x;
            float my = action.CanPosition.y - pos.y;
            my *= scale.y;
            my += ShowOffset;
            int r = 0;//行
            int count = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (my > lines[i].topY)
                {
                    if (i == 0)
                    {
                        count = uchars.Length;
                    }
                    else
                    {
                        r = i - 1;
                        count = lines[i].startCharIdx - lines[r].startCharIdx;
                    }
                }
            }
            int s = lines[r].startCharIdx;
            float lx = uchars[s].cursorPos.x;
            if (mx < lx)//最左边
            {
                return s;
            }
            else
            {
                int e = s + count - 1;
                float rx = uchars[e].cursorPos.x;
                if (mx > rx)//最右边
                {
                    return e;
                }
                else
                {
                    s++;
                    for (int i = 1; i < count; i++)
                    {
                        if (mx >= uchars[s].cursorPos.x)
                        {
                            lx = uchars[s - 1].cursorPos.x;
                            rx = uchars[s].cursorPos.x;
                            if (mx - lx > rx - mx)
                            {
                                return s;
                            }
                            else
                            {
                                return s - 1;
                            }
                        }
                        s++;
                    }
                }
            }
            return 0;
        }
        public void SetStartSelect(UserEvent userEvent, UserAction action)
        {
            StartIndex = GetPressIndex(userEvent,action);
            Style = 0;
        }
        public void SetEndSelect(UserEvent userEvent, UserAction action)
        {
            EndIndex = GetPressIndex(userEvent,action);
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
        int CommonArea(int s1, int e1, ref int s2, ref int e2)
        {
            if (s1 > e2)
                return 0;
            if (s2 > e1)
                return 2;
            if (s2 < s1)
                s2 = s1;
            if (e2 > e1)
                e2 = e1;
            return 1;
        }
        /// <summary>
        /// 获取当前选中的区域
        /// </summary>
        /// <param name="color"></param>
        /// <param name="tri"></param>
        /// <param name="vert"></param>
        public void GetSelectArea(Color32 color, List<int> tri, List<HVertex> vert)
        {
            if (uchars == null)
                return;
            int s = StartIndex;
            int e = EndIndex;
            if (e < s)
            {
                int t = s;
                s = e;
                e = t;
            }
            int c = e - s;
            vert.Clear();
            tri.Clear();
            int len = EndLine + 1;
            for (int i = StartLine; i < len; i++)
            {
                int os = lines[i].startCharIdx;
                int oe = uchars.Length;
                if (i < len - 1)
                    oe = lines[i + 1].startCharIdx - 1;
                int state = CommonArea(s, e, ref os, ref oe);
                if (state == 2)//结束
                    break;
                if (state == 1)//包含公共区域
                {
                    float lx = uchars[os].cursorPos.x - uchars[os].charWidth * 0.5f;
                    float rx = uchars[oe].cursorPos.x + uchars[oe].charWidth * 0.5f;
                    float h = lines[i].height;
                    float top = lines[i].topY;
                    float down = top - h;
                    int st = vert.Count;
                    var v = new HVertex();
                    v.position.x = lx;
                    v.position.y = down;
                    v.color = color;
                    vert.Add(v);
                    v.position.x = rx;
                    v.position.y = down;
                    v.color = color;
                    vert.Add(v);
                    v.position.x = lx;
                    v.position.y = top;
                    v.color = color;
                    vert.Add(v);
                    v.position.x = rx;
                    v.position.y = top;
                    v.color = color;
                    vert.Add(v);
                    tri.Add(st);
                    tri.Add(st + 2);
                    tri.Add(st + 3);
                    tri.Add(st);
                    tri.Add(st + 3);
                    tri.Add(st + 1);
                }
            }

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
            if(StartIndex<uchars.Length-1)
            {
                StartIndex++;
                if (StartLine < lines.Length - 1)
                {
                    if(lines[StartLine+1].startCharIdx<=StartIndex)
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
                else StartLine = lines[StartLine].startCharIdx + LineOffset;
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
                else StartLine = lines[StartLine].startCharIdx + LineOffset;
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
                        return;
                    }
                }
                ShowStart = 0;
                return;
            }
            int e = uchars.Length;
            int l = ShowStart + ShowRow + 1;
            if (l < LineCount)
                e = lines[l].startCharIdx - 1;
            if (StartIndex > e)
            {
                for (int i = ShowStart; i <LineCount; i--)
                {
                    if (lines[i].startCharIdx > StartIndex)
                    {
                        ShowStart = i - ShowRow;
                        return;
                    }
                }
                ShowStart = LineCount - ShowRow;
            }
        }
        public void ChangeShowLine(int index)
        {
            if (ShowStart != index)
            {

            }
            ShowStart = index;
        }
        public void InsertString(string str)
        {
            DeleteSelectString();
            var es = new EmojiString(str);
            int c = es.Length;
            Text.Insert(StartIndex, es);
            StartIndex += c;
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
            return Text.Remove(StartIndex);
        }
        public bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            return Text.Remove(StartIndex);
        }
        /// <summary>
        /// 重新计算
        /// </summary>
        public void ReCalcul(HText text)
        {
            GetPreferredHeight(text);
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
        void GetPreferredHeight(HText text)
        {
            string str = Text.FilterString;
            TextGenerationSettings settings = new TextGenerationSettings();
            settings.resizeTextMinSize = 2;
            settings.resizeTextMaxSize = 40;
            settings.scaleFactor = 1;
            settings.textAnchor = TextAnchor.UpperLeft;
            settings.color = Color.white;
            settings.generationExtents = new Vector2(text.SizeDelta.x, 0);
            settings.pivot = new Vector2(0.5f, 0.5f);
            settings.richText = true;
            settings.font = text.Font;
            if (settings.font == null)
                settings.font = HText.DefaultFont;
            settings.fontSize = text.m_fontSize;
            settings.fontStyle = FontStyle.Normal;
            settings.alignByGeometry = false;
            settings.updateBounds = false;
            settings.lineSpacing = text.m_lineSpace;
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
            ShowRow =(int)(text.SizeDelta.y / per);
        }
    }
}
