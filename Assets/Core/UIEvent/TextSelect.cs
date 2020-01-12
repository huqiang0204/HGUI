using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIEvent
{
    public class TextSelect:UserEvent
    {
        public HText TextCom;
        EmojiString Text;
        protected float overDistance = 500;
        protected float overTime = 0;
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
        float PreferredHeight;
        float HeightChange;
        int ShowStart;
        int ShowRow;
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
        }
        public override void OnMouseDown(UserAction action)
        {
            if (TextCom != null)
            {
                //textControll.Context = TextCom;
                //textControll.SetFullString(new EmojiString(m_inputString));
                //textControll.ReCalcul();
                //textControll.SetStartSelect(GetPressIndex(action, 0));
            }
            base.OnMouseDown(action);
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
                if (TextCom != null)
                {
                    if (action.Motion != Vector2.zero)
                    {
                        //if (action.CanPosition.y < RawPosition.y)
                        //    textControll.SetEndSelect(GetPressIndex(action, 0.2f));
                        //else textControll.SetEndSelect(GetPressIndex(action, -0.8f));
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
                            //if (oy > 0)
                            //    textControll.PointerMoveUp();
                            //else textControll.PointerMoveDown();
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
            //if (action.CanPosition.y < RawPosition.y)
            //    textControll.SetEndSelect(GetPressIndex(action, 0.2f));
            //else textControll.SetEndSelect(GetPressIndex(action, -0.8f));
            base.OnDragEnd(action);
        }
        void OnClick(UserEvent eventCall, UserAction action)
        {
           

        }
        void OnLostFocus(UserEvent eventCall, UserAction action)
        {
            InputCaret.Hide();
        }
        Vector3Int GetPressIndex(UserAction action, float dir)
        {
            Vector3Int v3 = Vector3Int.zero;
            if (TextCom == null)
                return v3;
            var lines = TextCom.uILines;
            if (lines == null)
                return v3;
            var uchars = TextCom.uIChars;
            var pos = GlobalPosition;
            var scale = GlobalScale;
            float mx = action.CanPosition.x - pos.x;
            mx *= scale.x;
            float my = action.CanPosition.y - pos.y;
            my *= scale.y;
            int len = lines.Count;
            int end = len - 1;
            int r = 0;//行
            v3.x = r;
            if (my < lines[0].topY)
            {
                if (my < lines[end].topY)
                {
                    r = end;
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        if (lines[i].topY + dir * lines[i].height < my)
                        {
                            r = i;
                            break;
                        }
                    }
                }
            }
            int count = 0;
            if (r + 1 < lines.Count)
                count = lines[r + 1].startCharIdx - lines[r].startCharIdx;
            else count = uchars.Count - lines[r].startCharIdx;
            int s = lines[r].startCharIdx;
            float lx = uchars[s].cursorPos.x;
            v3.y = s;
            if (mx < lx)//最左边
            {
                v3.y = s;
                return v3;
            }
            else
            {
                int e = s + count - 1;
                float rx = uchars[e].cursorPos.x;
                if (mx > rx)//最右边
                {
                    v3.y = e;
                    v3.z = count - 1;
                    return v3;
                }
                else
                {
                    s++;
                    for (int i = 1; i < count; i++)
                    {
                        if (mx < uchars[s].cursorPos.x)
                        {
                            lx = uchars[s - 1].cursorPos.x;
                            rx = uchars[s].cursorPos.x;
                            if (mx - lx > rx - mx)
                            {
                                v3.y = s;
                                v3.z = i;
                                return v3;
                            }
                            else
                            {
                                v3.y = s - 1;
                                v3.z = i - 1;
                                return v3;
                            }
                        }
                        s++;
                    }
                }
            }
            return v3;
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
            settings.generationExtents = new Vector2(Context.SizeDelta.x, Context.SizeDelta.y);
            settings.pivot = new Vector2(0.5f, 0);
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
            lines = generator.lines.ToArray();
            uchars = generator.characters.ToArray();
            int lc = lines.Length;
            LineChange = lc - LineCount;
            LineCount = lc;
            float per = h / lc;
            ShowRow = (int)(Context.SizeDelta.y / per);
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
                    InputCaret.CaretStyle = 1;
                    break;
                case 2:
                    InputCaret.CaretStyle = 2;
                    break;
            }
        }
        protected void PointerMoveLeft()
        {

        }
        protected void PointerMoveRight()
        {

        }
        protected void PointerMoveUp()
        {

        }
        protected void PointerMoveDown()
        {

        }
   
    }
}
