﻿using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIEvent
{
    public struct PressInfo
    {
        public int Row;
        public int Offset;
        public int Index;
    }
    struct LineInfo
    {
        public float top;
        public int StartIndex;
        public int Count;
        public float y;
    }
    public class TextSelect:UserEvent
    {
        public HText TextCom;
        protected EmojiString Text = new EmojiString();
        protected float overDistance = 500;
        protected float overTime = 0;
        LineInfo[] lines;
        UICharInfo[] cha;
        /// <summary>
        /// 总计行数
        /// </summary>
        public int LineCount;
        /// <summary>
        /// 变动的行数，增加或减少
        /// </summary>
        public int LineChange = 0;
        float PreferredHeight;
        float HeightChange;
        int ShowStart;
        int ShowRow;
        protected PressInfo StartPress;
        protected PressInfo EndPress;
        public int Style = 0;
        public Color32 PointColor = Color.white;
        public Color32 SelectionColor = new Color(0.65882f, 0.8078f, 1, 0.2f);
        internal override void Initial(FakeStruct mod)
        {
            TextCom = Context as HText;
            AutoColor = false;
            unsafe
            {
                var ex = mod.buffer.GetData(((TransfromData*)mod.ip)->ex) as FakeStruct;
                if (ex != null)
                {
                    TextInputData* tp = (TextInputData*)ex.ip;
                    PointColor = tp->pointColor;
                    SelectionColor = tp->selectColor;
                }
            }
            Text.FullString = TextCom.Text;
            GetPreferredHeight();
        }
        public override void OnMouseDown(UserAction action)
        {
            if (TextCom != null)
            {
                StartPress = GetPressIndex(action, Vector2.zero);
                InputCaret.SetParent(TextCom.transform);
                InputCaret.Active();
            }
            base.OnMouseDown(action);
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
                if (TextCom != null)
                {
                    Style = 1;
                    if (action.Motion != Vector2.zero)
                    {
                        EndPress = GetPressIndex(action, action.CanPosition - RawPosition);
                    }
                    else if (!entry)
                    {
                        float oy = action.CanPosition.y - GlobalPosition.y;
                        float py = GlobalScale.y * TextCom.SizeDelta.y * 0.5f;
                        if (oy > 0)
                            oy -= py;
                        else oy += py;
                        if (oy > overDistance)
                            oy = overDistance;
                        float per = 50000 / oy;
                        if (per < 0)
                            per = -per;
                        overTime += UserAction.TimeSlice;
                        if (overTime >= per)
                        {
                            overTime -= per;
                            if (oy > 0)
                                MoveUp();
                            else MoveDown();
                        }
                    }
                }
            base.OnDrag(action);
        }
        internal override void OnMouseWheel(UserAction action)
        {
            float oy = action.MouseWheelDelta;
            //if (oy > 0)
            //    textControll.PointerMoveUp();
            //else textControll.PointerMoveDown();
            base.OnMouseWheel(action);
        }
        internal override void OnDragEnd(UserAction action)
        {
            if(TextCom!=null)
            {
                Style = 1;
                long r = action.EventTicks - PressTime;
                if (r <= ClickTime)
                {
                    float x = action.CanPosition.x;
                    float y = action.CanPosition.y;
                    x -= RawPosition.x;
                    x *= x;
                    y -= RawPosition.y;
                    y *= y;
                    x += y;
                    if (x < ClickArea)
                        return;
                }
                EndPress = GetPressIndex(action, action.CanPosition - RawPosition);
            }
            base.OnDragEnd(action);
        }
        void OnClick(UserEvent eventCall, UserAction action)
        {
            Style = 0;
            InputCaret.Hide();
        }
        void OnLostFocus(UserEvent eventCall, UserAction action)
        {
            Style = 0;
            InputCaret.Hide();
        }
        protected void GetPreferredHeight()
        {
            string str = Text.FilterString;
            TextGenerationSettings settings = new TextGenerationSettings();
            settings.resizeTextMinSize = 2;
            settings.resizeTextMaxSize = 40;
            settings.scaleFactor = 1;
            settings.textAnchor = TextAnchor.UpperLeft;
            settings.color = Color.white;
            settings.generationExtents = new Vector2(Context.SizeDelta.x, Context.SizeDelta.y);
            settings.pivot = new Vector2(0.5f, 1);
            settings.richText = false;
            settings.font = TextCom.Font;
            if (settings.font == null)
                settings.font = HText.DefaultFont;
            settings.fontSize = TextCom.m_fontSize;
            settings.fontStyle = FontStyle.Normal;
            settings.alignByGeometry = false;
            settings.updateBounds = false;
            settings.lineSpacing = TextCom.m_lineSpace;
            settings.horizontalOverflow = HorizontalWrapMode.Wrap;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            TextGenerator generator = HText.Generator;
            float h = generator.GetPreferredHeight(str, settings);
            HeightChange = PreferredHeight - h;
            PreferredHeight = h;
      
            cha = generator.characters.ToArray();
            var tmp = generator.lines;
            lines = new LineInfo[tmp.Count];
            int len = lines.Length - 1;
            for (int i = 0; i < len; i++)
            {
                lines[i].StartIndex = tmp[i].startCharIdx;
                lines[i].Count = tmp[i + 1].startCharIdx - tmp[i].startCharIdx;
                lines[i].y = tmp[i].topY - (tmp[i].height + tmp[i].leading) * 0.5f;
            }
            lines[len].StartIndex = tmp[len].startCharIdx;
            lines[len].Count = cha.Length - tmp[len].startCharIdx;
            lines[len].y = tmp[len].topY - (tmp[len].height + tmp[len].leading) * 0.5f;
            int lc = lines.Length;
            LineChange = lc - LineCount;
            LineCount = lc;
            float per = h / lc;
            ShowRow = (int)(Context.SizeDelta.y / per);
            for(int i=0;i<cha.Length;i++)
                cha[i].cursorPos.x += cha[i].charWidth * 0.5f;
        }
        protected PressInfo GetPressIndex(UserAction action, Vector2 dir)
        {
            PressInfo info = new PressInfo();
            if (TextCom == null)
                return info;
            Vector3 pos = GlobalPosition;//全局坐标
            var offset = pos;
            offset.x -= action.CanPosition.x;
            offset.y -= action.CanPosition.y;
            var q = Quaternion.Inverse(GlobalRotation);
            offset = q * offset;
            var scale = GlobalScale;//全局尺寸
            offset.x /= scale.x;
            offset.y /= scale.y;
            float ox = -offset.x;
            float oy = -offset.y - TextCom.SizeDelta.y * 0.5f;
            int r = GetPressLine(oy,dir.y);
            info.Row = r;
            int os = GetPressOffset(r,ox,dir.x);
            info.Offset = os;
            if (os >= lines[r].Count)
                os--;
            info.Index= lines[r].StartIndex+ os;
            return info;
        }
        int GetPressLine(float y,float dir)
        {
            int r = ShowStart;
            if (y < lines[ShowStart].y)
            {
                int end = ShowStart + ShowRow;
                if (end > lines.Length)
                    end = lines.Length;
                if (y < lines[end].y)
                    return end - 1;
                float oy = 1000000;
                int index = ShowStart;
                for (int i = ShowStart; i < end ; i++)
                {
                    float ty = lines[i].y - y;
                    if (ty < 0)
                    {
                        ty = -ty;
                        if (oy < ty)
                            index = i - 1;
                        if (index < 0)
                            index = 0;
                        break;
                    }
                    else 
                    { 
                        oy = ty;
                        index = i;
                    }
                }
                if (dir == 0)
                    return index;
                else if(dir<0)//向下
                {
                    if (lines[index].y < y)
                        index--;
                    if (index < 0)
                        index = 0;
                    return index;
                }
                else//向上
                {
                    if (lines[index].y < y)
                        index++;
                    if (index > end)
                        index = end;
                    return index;
                }
            }
            return r;
        }
        int GetPressOffset(int line,float x,float dir)
        {
            int s = lines[line].StartIndex;
            int c = lines[line].Count;
            int e = s + c - 1;
            if (x < cha[s].cursorPos.x)
                return 0;
            if (x > cha[e].cursorPos.x + cha[e].charWidth)
                return c;
            float ox = 1000000;
            int index = 0;
            for (int i = 0; i < c ; i++)
            {
                float tx = x - cha[s].cursorPos.x;
                if (tx < 0)
                {
                    tx = -tx;
                    if (tx < ox)
                        index++;
                    if (index > c)
                        index = c;
                    break;
                }
                else
                {
                    ox = tx;
                    index = i;
                }
                s++;
            }
           if (dir < 0)//向左
            {
                if (cha[index].cursorPos.x < x)
                    index ++;
                if (index > c)
                    index = c;
            }
            else if(dir > 0)//向右
            {
                if (cha[index].cursorPos.x < x)
                    index++;
                if (index > c)
                    index = c;
            }
            return index;
        }
        internal override void Update()
        {
            base.Update();
            switch(Style)
            {
                case 0:
                    InputCaret.Hide();
                    break;
                case 1:
                    InputCaret.CaretStyle = 2;
                    List<HVertex> hs = new List<HVertex>();
                    List<int> tris = new List<int>();
                    GetSelectArea(SelectionColor, tris, hs);
                    InputCaret.ChangeCaret(hs.ToArray(), tris.ToArray());
                    break;
            }
        }
        protected void MoveUp()
        {

        }
        protected void MoveDown()
        {

        }
        public float Percentage
        {
            get
            {
                float r = (float)ShowStart / ((float)LineCount - (float)ShowRow);
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
                //ChangeShowLine(c);
            }
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
        Vector2Int GetShowSelect()
        {
            Vector2Int vector = Vector2Int.zero;
            vector.x = StartPress.Index;
            vector.y = EndPress.Index;
            if (vector.x > vector.y)
            {
                int t = vector.x;
                vector.x = vector.y;
                vector.y = t;
            }
            int s = lines[ShowStart].StartIndex;
            vector.x -= s;
            vector.y -= s;
            return vector;
        }
        int GetStartLine()
        {
            if (StartPress.Row > EndPress.Row)
                return EndPress.Row;
            else return StartPress.Row;
        }
        /// <summary>
        /// 获取当前选中的区域
        /// </summary>
        /// <param name="color"></param>
        /// <param name="tri"></param>
        /// <param name="vert"></param>
        public void GetSelectArea(Color32 color, List<int> tri, List<HVertex> vert)
        {
            if (TextCom == null)
                return;
            var ss = GetShowSelect();
            int s = ss.x;
            int e = ss.y;
            int c = TextCom.uIChars.Count;
            int state = CommonArea(0, c, ref s, ref e);
            if (state == 1)
                if (s != e)
                {
                    vert.Clear();
                    tri.Clear();
                    int sl = GetStartLine();
                    if (sl < 0)
                        sl = 0;
                    var lines = TextCom.uILines;
                    int len = lines.Count;
                    var uchars = TextCom.uIChars;
                    int clen = uchars.Count;
                    for (int i = sl; i < len; i++)
                    {
                        int os = lines[i].startCharIdx;
                        int oe = clen;
                        if (i < len - 1)
                            oe = lines[i + 1].startCharIdx - 1;
                        state = CommonArea(s, e, ref os, ref oe);
                        if (state == 2)//结束
                            break;
                        if (state == 1)//包含公共区域
                        {
                            float lx = uchars[os].cursorPos.x ;
                            float rx = uchars[oe].cursorPos.x + uchars[oe].charWidth;
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
        }
    }
}
