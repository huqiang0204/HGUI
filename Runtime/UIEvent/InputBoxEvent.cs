using huqiang.Core.HGUI;
using huqiang.Core.Line;
using huqiang.Data;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class InputBoxEvent:UserEvent
    {
        public InputBox input;
        HText text;
        public string InputString;
        EmojiString Content=new EmojiString();
        List<UIVertex> verts = new List<UIVertex>();
        List<LineInfo> lines = new List<LineInfo>();
        List<UICharInfo> chars = new List<UICharInfo>();
        protected float overDistance = 500;
        protected float overTime = 0;
        internal override void Initial(FakeStruct mod)
        {
            AutoColor = false;
            text = Context as HText;
        }
        public void ChangeText(string str)
        {
            verts.Clear();
            lines.Clear();
            chars.Clear();
            Content.FullString = str;
            text.GetGenerationSettings(ref text.m_sizeDelta, ref HText.settings);
            HText.settings.richText = false;

            var g = HText.Generator;
            g.Populate(Content.FilterString, HText.settings);
            verts.AddRange(g.verts);
            chars.AddRange(g.characters);
            //ContentHeight = 0 ;
            LineInfo line = new LineInfo();
            if (g.lines.Count > 0)
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
                l = g.lines[g.lines.Count - 1];
                line.startCharIdx = l.startCharIdx;
                line.endIdx = chars.Count - 1;
                line.topY = l.topY;
                line.endY = l.topY- l.height;
                lines.Add(line);
                //ContentHeight = lines[0].topY - line.endY;
            }
        }
        public override void OnMouseDown(UserAction action)
        {
            PressInfo press = new PressInfo();
            CheckPointer(action,ref press);
            input.OnMouseDown(action,ref press);
            base.OnMouseDown(action);
        }
        internal override void OnClick(UserAction action)
        {
            input.OnClick(action);
            base.OnClick(action);
        }
        internal override void OnLostFocus(UserAction action)
        {
            input.OnLostFocus(action);
            base.OnLostFocus(action);
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
            {
                if (action.CanPosition != action.LastPosition)
                {
                    if (!entry)
                    {
                        float oy = action.CanPosition.y - GlobalPosition.y;
                        float py = GlobalScale.y * text.SizeDelta.y * 0.5f;
                        if (oy > 0)
                            oy -= py;
                        else oy += py;
                        if (oy > overDistance)
                            oy = overDistance;
                        float per = 5000 / oy;
                        if (per < 0)
                            per = -per;
                        overTime += UserAction.TimeSlice;
                        if (overTime >= per)
                        {
                            overTime -= per;
                            if (oy > 0)
                            {
                                if (TextOperation.ContentMoveUp())
                                {
                                    input.SetShowText();
                                }
                            }
                            else
                            {
                                if (TextOperation.ContentMoveDown())
                                {
                                    input.SetShowText();
                                }
                            }
                        }
                    }
                    PressInfo press = new PressInfo();
                    CheckPointer(action, ref press);
                    input.OnDrag(action, ref press);
                }
            }
            base.OnDrag(action);
        }
        internal override void OnMouseWheel(UserAction action)
        {
            float oy = action.MouseWheelDelta;
            if (oy > 0)
            {
                if (TextOperation.ContentMoveUp())
                {
                    input.SetShowText();
                }
            }
            else
            {
                if (TextOperation.ContentMoveDown())
                {
                    input.SetShowText();
                }
            }
            base.OnMouseWheel(action);
        }
        public void CheckPointer(UserAction action, ref PressInfo press)
        {
            if(lines.Count>0)
            {
                var offset = action.CanPosition;
                offset.x -= GlobalPosition.x;//全局坐标
                offset.y -= GlobalPosition.y;
                var q = Quaternion.Inverse(GlobalRotation);
                offset = q * offset;
                var scale = GlobalScale;//全局尺寸
                offset.x /= scale.x;
                offset.y /= scale.y;
                int end = Content.FilterString.Length;
                int row = 0;
                if (lines.Count > 0)
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (offset.y > lines[i].topY)
                        {
                            end = lines[i].startCharIdx;
                            break;
                        }
                        else row = i;
                    }
                }
                if (end > chars.Count)
                    end = chars.Count;
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
        }
        public void GetPointer(List<int> tri, List<HVertex> vert, ref Color32 color, ref PressInfo start)
        {
            if (start.Row < 0)
                return;
            if (start.Row >= lines.Count)
                return;
            int index = start.Index;
            if (index < 0)
                return;
            if (index >= chars.Count)
                return;
            int row = start.Row;
            for (int i = 0; i < lines.Count; i++)
            {
                if (index <= lines[i].endIdx)
                {
                    row = i;
                    break;
                }
            }
            float top = lines[row].topY;
            float down = lines[row].endY;
            float p;
            if (index > lines[row].endIdx)
            {
                index--;
                if (index < 0)
                    index = 0;
                p = chars[index].cursorPos.x + chars[index].charWidth;
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
            tri.Add(0);
            tri.Add(2);
            tri.Add(3);
            tri.Add(0);
            tri.Add(3);
            tri.Add(1);
        }
        public void SetCursorPos(ref PressInfo start)
        {
            if (start.Row < 0)
                return;
            if (start.Row >= lines.Count)
                return;
            int index = start.Index;
            if (index < 0)
                return;
            if (index >= chars.Count)
                return;
            int row = start.Row;
            for (int i = 0; i < lines.Count; i++)
            {
                if (index <= lines[i].endIdx)
                {
                    row = i;
                    break;
                }
            }
            float p;
            if (index > lines[row].endIdx)
            {
                index--;
                if (index < 0)
                    index = 0;
                p = chars[index].cursorPos.x + chars[index].charWidth;
            }
            else
            {
                p = chars[index].cursorPos.x;
            }
            float right = p + 1;
            float down = lines[row].endY;
            var gl = UIElement.GetGlobaInfo(text.transform, false);
            float rx = gl.Scale.x * right;
            float rd = gl.Scale.y * down;
            gl.Postion.x += rx;
            gl.Postion.y += rd;
            gl.Postion /= HCanvas.MainCanvas.PhysicalScale;
            gl.Postion.x += Screen.width / 2;
            gl.Postion.y += Screen.height / 2;
            Keyboard.CursorPos = new Vector2(gl.Postion.x, Screen.height - gl.Postion.y);
        }
        public void GetSelectArea(List<int> tri, List<HVertex> vert, ref Color32 color, ref PressInfo start, ref PressInfo end)
        {
            int sr = start.Row;
            int er = end.Row + 1;
            int st = 0;
            int c = lines.Count;
            if (c > er)
                c = er;
            if (sr < 0)
                sr = 0;
            for (int i = sr; i < c; i++)
            {
                LineInfo info = lines[i];
                int ls = info.startCharIdx;
                if (ls > end.Index)
                    break;
                int le = lines[i].endIdx;
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
                float top = info.topY;
                float down = info.endY;

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
    }
}
