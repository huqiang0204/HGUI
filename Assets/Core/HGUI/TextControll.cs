using huqiang.Core.HGUI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class TextControll
    {
        UILineInfo[] lines;//所有文本行
        UIVertex[] fullVertex;//所有文本顶点
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
        Vector2 BoxSize;
        int Style = 0;
        int LineOffset;
        EmojiString Text;
        public void SetFullString(TextGenerator generator,EmojiString emojiString)
        {
            uchars = generator.characters.ToArray();
            lines = generator.lines.ToArray();
            fullVertex = generator.verts.ToArray();
            Text = emojiString;
            LineCount = lines.Length;
        }
        public int GetPressIndex(UserEvent callBack, UserAction action)
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
            int count = 1;
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
                int e = s + count;
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
        /// <summary>
        /// 获取当前需要显示的字符串
        /// </summary>
        /// <returns></returns>
        public string GetShowString()
        {
            int s = lines[ShowStart].startCharIdx;
            int end = ShowStart + ShowRow + 1;
            int e = uchars.Length;
            if (end < LineCount)
                e = lines[end].startCharIdx - 1;
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
        public void MoveLeft()
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
                    ChangeLine(StartLine);
                }
            }
        }
        public void MoveRight()
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
                    ChangeLine(StartLine - ShowRow);
                }
            }
        }
        public void MoveUp()
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
                    ChangeLine(StartLine);
                }
            }
        }
        public void MoveDown()
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
                    ChangeLine(StartLine - ShowRow);
                }
            }
        }
        public void ChangeLine(int index)
        {
            if(ShowStart!=index)
            {

            }
            StartLine = index;
        }
        public void AddString(string str)
        {

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
                ChangeLine(c);
            }
        }
    }
}
