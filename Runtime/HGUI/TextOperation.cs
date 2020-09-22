using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct PressInfo
    {
        public int Row;
        public int Offset;
        public int Index;
    }
    public struct LineInfo
    {
        public int startCharIdx;
        public int endIdx;
        public float topY;
        public float endY;
    }
    public class TextOperation
    {
        public static HText Target;
        public static PressInfo StartPress;
        public static PressInfo EndPress;

        public static int SelectCount;
        static List<UIVertex> verts = new List<UIVertex>();
        static List<LineInfo> lines = new List<LineInfo>();
        static List<UICharInfo> chars = new List<UICharInfo>();
        public static EmojiString Content;
        static int VisibleCount;
        public static int ShowStart;//当期文本框显示的内容起始行
        public static int ShowRow;//当前文本框可以显示的内容行数
        public static float ContentHeight;
        static float StartY;
        public static ContentType contentType;
        public static void ChangeText(HText text, EmojiString str)
        {
            verts.Clear();
            lines.Clear();
            chars.Clear();
            Target = text;
            Content = str;
            text.GetGenerationSettings(ref text.m_sizeDelta,ref HText.settings);
            var g = HText.Generator;
            string fs = Content.FilterString;
            if (contentType == ContentType.Password)
            {
                ContentHeight = g.GetPreferredHeight(new string('●', fs.Length), HText.settings);
            }
            else
            { 
                ContentHeight = g.GetPreferredHeight(fs, HText.settings); 
            }
            VisibleCount = g.characterCountVisible;
            if (g.lines.Count > 0)
            {
                float per = ContentHeight / g.lines.Count;
                ShowRow = (int)(text.m_sizeDelta.y / per);
            }
            else ShowRow = (int)text.m_sizeDelta.y / text.FontSize;
            StartY = ContentHeight * 0.5f - text.m_sizeDelta.y * 0.5f;
            verts.AddRange(g.verts);
            chars.AddRange(g.characters);
            LineInfo line = new LineInfo();
            if(g.lines.Count>0)
            {
                var l = g.lines[0];
                for (int i = 1; i < g.lines.Count; i++)
                {
                    var n = g.lines[i];
                    line.startCharIdx = l.startCharIdx;
                    line.endIdx = n.startCharIdx - 1;
                    line.topY = l.topY;
                    line.endY = n.topY;
                    l = n;
                    lines.Add(line);
                }
                l = g.lines[g.lines.Count-1];
                line.startCharIdx = l.startCharIdx;
                line.endIdx = chars.Count - 1;
                line.topY = l.topY;
                line.endY = g.lines[0].topY - ContentHeight - g.lines[g.lines.Count-1].leading;
                lines.Add(line);
            }
            if (StartPress.Index > fs.Length)
            {
                SetPressIndex(fs.Length, ref StartPress);
                EndPress = StartPress;
            }
            SetShowStart(ShowStart);
        }
        public static void SetStartPointer(UserEvent user, UserAction action)
        {
            PointerDown(user,action,ref StartPress);
            EndPress = StartPress;
        }
        public static void Drag(UserEvent user, UserAction action)
        {
            PointerDown(user, action, ref EndPress);
        }
        public static void PointerDown(UserEvent user, UserAction action,ref PressInfo press)
        {
            var offset = action.CanPosition;
            offset.x -= user.GlobalPosition.x;//全局坐标
            offset.y -= user.GlobalPosition.y;
            var q = Quaternion.Inverse(user.GlobalRotation);
            offset = q * offset;
            var scale =user.GlobalScale;//全局尺寸
            offset.x /= scale.x;
            offset.y /= scale.y;
            float oy = lines[0].topY - lines[ShowStart].topY;
            offset.y -= oy;
            offset.y += StartY;
            int row = ShowStart;
            int end = VisibleCount;
            if(lines.Count>0)
            {
                for (int i = ShowStart; i < lines.Count; i++)
                {
                    if (offset.y > lines[i].topY)
                    {
                        end = lines[i].startCharIdx;
                        break; 
                    }
                    else row = i;
                }
            }
            int start = lines[row].startCharIdx;
            int col = end - start;
            for (int i = start; i < end; i++)
            {
                if (offset.x < chars[i].cursorPos.x + chars[i].charWidth * 0.5f)
                {
                    col = i - start;
                    break;
                }
            }
            press.Row = row;
            press.Offset = col;
            press.Index = start + col;
        }
        public static bool ContentMoveUp()
        {
            if (ShowStart > 0)
            { 
                ShowStart--;
                return true;
            }
            return false;
        }
        public static bool ContentMoveDown()
        {
            if(ShowStart + ShowRow < lines.Count)
            {
                ShowStart++;
                return true;
            }
            return false;
        }
        public static string GetShowContent()
        {
            int s = lines[ShowStart].startCharIdx;
            int end = ShowStart + ShowRow;
            int e = VisibleCount;
            if(end < lines.Count)
                e = lines[end].endIdx + 1;
            string str = Content.SubString(s, e - s);
            string cs = Keyboard.CompositionString;
            if (cs != null & cs != "")
            {
                int ss = StartPress.Index - s;
                if (ss >= 0 & ss <= e - s)
                {
                    str = str.Insert(ss, cs);
                }
            }
            return str;
        }
        public static void SelectAll()
        {
            StartPress.Index = 0;
            StartPress.Offset = 0;
            StartPress.Row = 0;
            EndPress.Row = lines.Count - 1;
            EndPress.Index = chars.Count - 1;
            EndPress.Offset = EndPress.Index - lines[EndPress.Row].startCharIdx;
        }
        public static string GetSelectString()
        {
            if (EndPress.Index == StartPress.Index)
                return "";
            if (EndPress.Index < StartPress.Index)
                return Content.SubString(EndPress.Index, StartPress.Index - EndPress.Index);
            else return Content.SubString(StartPress.Index,EndPress.Index-StartPress.Index);
        }
        public static bool DeleteSelectString()
        {
            if (StartPress.Index == EndPress.Index)
                return false;
            if (EndPress.Index < StartPress.Index)
            {
                int c = StartPress.Index - EndPress.Index;
                Content.Remove(EndPress.Index, c);
                StartPress = EndPress;
            }
            else 
            {
                 int c = EndPress.Index - StartPress.Index;
                Content.Remove(StartPress.Index, c);
                EndPress = StartPress;
            }
            ChangeText(Target,Content);
            if (StartPress.Row < ShowStart)
            {
                ShowStart = StartPress.Row;
            }
            else if (StartPress.Row >= ShowStart + ShowRow)
            {
                ShowStart = StartPress.Row - ShowRow + 1;
            }
            return true;
        }
        static bool SetPressIndex(int index,ref PressInfo press)
        {
            if (index < 0)
                index = 0;
            if (lines.Count == 0)
                return false;
            if (index == press.Index)
                return false;
            if (index >= chars.Count)
            {
                index = chars.Count - 1;
                if (index == press.Index)
                    return false;
                press.Index = index;
                press.Row = lines.Count - 1;
                press.Offset = index - lines[press.Row].startCharIdx;
                return true;
            }
            for (int i = 0; i < lines.Count; i++)
            {
                if (index <= lines[i].endIdx)
                {
                    press.Index = index;
                    press.Row = i;
                    press.Offset = index - lines[i].startCharIdx;
                    break;
                }
            }
            return true;
        }
        public static bool SetPressIndex(int index,ref bool LineChanged)
        {
            if(SetPressIndex(index,ref StartPress))
            {
                EndPress = StartPress;
                if (StartPress.Row < ShowStart)
                {
                    ShowStart = StartPress.Row;
                    LineChanged = true;
                }
                else
                if (StartPress.Row >= ShowStart + ShowRow)
                {
                    ShowStart = StartPress.Row - ShowRow + 1;
                    LineChanged = true;
                }
                return true;
            }
            return false;
        }
        public static void SetStartPressIndex(int index)
        {
            SetPressIndex(index, ref StartPress);
        }
        public static void SetEndPressIndex(int index)
        {
            SetPressIndex(index, ref EndPress);
        }
        public static bool PointerMoveUp(ref bool LineChanged)
        {
            if(StartPress.Row>0)
            {
                StartPress.Row--;
                EndPress.Row = StartPress.Row;
                int index = lines[StartPress.Row].startCharIdx + StartPress.Offset;
                if (index > lines[StartPress.Row].endIdx)
                    index = lines[StartPress.Row].endIdx;
                EndPress.Index = StartPress.Index = index;
                if(StartPress.Row<ShowStart)
                {
                    ShowStart = StartPress.Row;
                    LineChanged = true;
                }
                return true;
            }
            return false;
        }
        public static bool PointerMoveDown(ref bool LineChanged)
        {
            if (StartPress.Row < lines.Count - 1)
            {
                StartPress.Row++;
                EndPress.Row = StartPress.Row;
                int index = lines[StartPress.Row].startCharIdx + StartPress.Offset;
                if (index > lines[StartPress.Row].endIdx)
                    index = lines[StartPress.Row].endIdx;
                EndPress.Index = StartPress.Index = index;
                if(StartPress.Row>=ShowStart+ShowRow)
                {
                    ShowStart = StartPress.Row - ShowRow+1;
                    LineChanged = true;
                }
                return true;
            }
            return false;
        }
        public static bool PointerMoveLeft(ref bool LineChanged)
        {
            if (StartPress.Index > 0)
            {
                StartPress.Index--;
                int r = StartPress.Row;
                if (StartPress.Index < lines[r].startCharIdx)
                    StartPress.Row--;
                StartPress.Offset = StartPress.Index - lines[StartPress.Row].startCharIdx;
                EndPress = StartPress;
                if (StartPress.Row < ShowStart)
                {
                    ShowStart = StartPress.Row;
                    LineChanged = true;
                }
                return true;
            }
            return false;
        }
        public static bool PointerMoveRight(ref bool LineChanged)
        {
            if (StartPress.Index < chars.Count - 1)
            {
                StartPress.Index++;
                int r = StartPress.Row;
                if (StartPress.Index > lines[r].endIdx)
                    StartPress.Row++;
                StartPress.Offset = StartPress.Index - lines[StartPress.Row].startCharIdx;
                EndPress = StartPress;
                if (StartPress.Row >= ShowStart + ShowRow)
                {
                    ShowStart = StartPress.Row - ShowRow + 1;
                    LineChanged = true;
                }
                return true;
            }
            return false;
        }
        public static bool DeleteLast()
        {
            if (DeleteSelectString())
                return true;
            bool b= false;
            if(StartPress.Index>0)
            {
                Content.Remove(StartPress.Index - 1,1);
                int index = StartPress.Index - 1;
                int lc = lines.Count;
                ChangeText(Target, Content);
                SetPressIndex(index,ref b);
                int cc = lines.Count;
                int oc = lc - cc;
                ShowStart -= oc;
                if (ShowStart < 0)
                    ShowStart = 0;
                return true;
            }
            return false;
        }
        public static bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            bool b = false;
            if (StartPress.Index<chars.Count - 1)
            {
                Content.Remove(StartPress.Index, 1);
                int index = StartPress.Index;
                int lc = lines.Count;
                ChangeText(Target, Content);
                SetPressIndex(index,ref b);
                int cc = lines.Count;
                int oc = lc - cc;
                ShowStart -= oc;
                if (ShowStart < 0)
                    ShowStart = 0;
                return true;
            }
            return false;
        }
        public static void InsertContent(EmojiString con)
        {
            Content.Insert(StartPress.Index,con);
            int index = StartPress.Index + con.Length;
            ChangeText(Target, Content);
            bool b = false;
            SetPressIndex(index, ref b);
        }
        public static void GetSelectArea(List<int> tri, List<HVertex> vert, Color32 aColor, Color32 pColor)
        {
            if (EndPress.Index == StartPress.Index)
            {
                if (StartPress.Row < ShowStart)
                    return;
                if (StartPress.Row > ShowStart + ShowRow)
                    return;
                float oy = StartY - (lines[0].topY - lines[ShowStart].topY);
                float top = lines[StartPress.Row].topY - oy;
                float down = lines[StartPress.Row].endY - oy;
                int index = StartPress.Index;
                float p;
                if (StartPress.Index > lines[StartPress.Row].endIdx)
                {
                    index--;
                    if (index < 0)
                        index = 0;
                    p = chars[index].cursorPos.x+ chars[index].charWidth;
                }
                else
                {
                    p = chars[index].cursorPos.x;
                }
                float left = p - 1;
                float right = p + 1;
                var v = new HVertex();
                v.position.x = left;
                v.position.y = down;
                v.color = pColor;
                vert.Add(v);
                v.position.x = right;
                v.position.y = down;
                v.color = pColor;
                vert.Add(v);
                v.position.x = left;
                v.position.y = top;
                v.color = pColor;
                vert.Add(v);
                v.position.x = right;
                v.position.y = top;
                v.color = pColor;
                vert.Add(v);
                tri.Add(0);
                tri.Add(2);
                tri.Add( 3);
                tri.Add(0);
                tri.Add(3);
                tri.Add(1);
                return; 
            }
            if (EndPress.Index < StartPress.Index)
                GetSelectArea( tri, vert, ref aColor, ref EndPress, ref StartPress);
            else GetSelectArea(tri, vert, ref aColor, ref StartPress, ref EndPress);
        }
        static void GetSelectArea(List<int> tri, List<HVertex> vert, ref Color32 color, ref PressInfo start, ref PressInfo end)
        {
            int ss = ShowStart;
            int sr = start.Row;
            int er = end.Row;
            if (ss > er)
                return;
            if (sr > ss + ShowRow)
                return;
            float oy = StartY - (lines[0].topY - lines[ShowStart].topY);
            int st = 0;
            for (int i = 0; i < ShowRow; i++)
            {
                if (ss >= lines.Count)
                    break;
                LineInfo info = lines[ss];
                int ls = info.startCharIdx;
                if (ls > end.Index)
                    break;
                int le = lines[ss].endIdx;
                ss++;
                if (le <= start.Index)
                    continue;
                int si = start.Index;
                int ei = end.Index;
                float left;
                if (si < ls)
                    left = chars[ls].cursorPos.x;
                else left = chars[si].cursorPos.x;
                float right;
                if (ei > le)
                    right = chars[le].cursorPos.x + chars[le].charWidth;
                else right = chars[ei].cursorPos.x;
                float top = info.topY - oy;
                float down = info.endY - oy;

                var v = new HVertex();
                v.position.x = left;
                v.position.y = down;
                v.color = color;
                vert.Add(v);
                v.position.x = right;
                v.position.y = down;
                v.color = color;
                vert.Add(v);
                v.position.x = left;
                v.position.y = top;
                v.color = color;
                vert.Add(v);
                v.position.x = right;
                v.position.y = top;
                v.color = color;
                vert.Add(v);
                tri.Add(st);
                tri.Add(st + 2);
                tri.Add(st + 3);
                tri.Add(st);
                tri.Add(st + 3);
                tri.Add(st + 1);
                st += 4;
            }
        }
        public static int Style { get => StartPress.Index == EndPress.Index ? 1 : 2; }
        public static void SetShowStart(int start)
        {
            ShowStart = start;
            if (lines.Count <= ShowStart + ShowRow)
                ShowStart = lines.Count - ShowRow;
            if (ShowStart < 0)
                ShowStart = 0;
        }
        public static void SetPress(ref PressInfo press)
        {
            int row = press.Row + ShowStart;
            //if (row >= lines.Count)
            //    row = lines.Count - 1;
            StartPress.Index = lines[row].startCharIdx + press.Offset;
            StartPress.Row = row;
            StartPress.Offset = press.Offset;
            EndPress = StartPress;
        }
        public static void SetStartPress(ref PressInfo press)
        {
            int row = press.Row + ShowStart;
            StartPress.Index = lines[row].startCharIdx + press.Offset;
            StartPress.Row = row;
            StartPress.Offset = press.Offset;
        }
        public static void SetEndPress(ref PressInfo press)
        {
            int row = press.Row + ShowStart;
            EndPress.Index = lines[row].startCharIdx + press.Offset;
            EndPress.Row = row;
            EndPress.Offset = press.Offset;
        }
        public static PressInfo GetStartPress()
        {
            PressInfo p = new PressInfo();
            if (EndPress.Index < StartPress.Index)
                p = EndPress;
            else p = StartPress;
            p.Row -= ShowStart;
            p.Index -= lines[ShowStart].startCharIdx;
            if (Keyboard.CompositionString != null)
                p.Index += Keyboard.CompositionString.Length;
            return p;
        }
        public static PressInfo GetEndPress()
        {
            PressInfo p = new PressInfo();
            if (StartPress.Index > EndPress.Index)
                p = StartPress;
            else p = EndPress;
            p.Row -= ShowStart;
            p.Index -= lines[ShowStart].startCharIdx;
            return p;
        }
    }
}
