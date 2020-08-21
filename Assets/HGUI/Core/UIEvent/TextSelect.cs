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
    struct LineInfo
    {
        public float top;
        public int StartIndex;
        public int Count;
        public float y;
    }
    public class TextSelect:UserEvent
    {
        protected static TextGenerationSettings settings;
        public HText TextCom;
        protected EmojiString Text = new EmojiString();
        protected float overDistance = 500;
        protected float overTime = 0;
        internal LineInfo[] lines;
        protected UICharInfo[] cha;
        protected UILineInfo[] showLines;
        protected UICharInfo[] showChars;
        /// <summary>
        /// 总计行数
        /// </summary>
        public int LineCount;
        /// <summary>
        /// 变动的行数，增加或减少
        /// </summary>
        public int LineChange = 0;
        public float PreferredHeight;
        float HeightChange;
        protected int ShowStart;
        protected int ShowRow;
        protected bool Focus;
        protected bool ShowChanged;
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
            Focus = true;
            if (TextCom != null)
            {
                TextOperation.ChangeText(TextCom,Text);
                TextOperation.SetStartPointer(this,action);
                InputCaret.SetParent(TextCom.transform);
                InputCaret.Active();
                ShowChanged = true;
            }
            base.OnMouseDown(action);
        }
        protected override void OnDrag(UserAction action)
        {
            if (Pressed)
                if (TextCom != null)
                {
                    Style = 2;
                    if (action.Motion != Vector2.zero)
                    {
                        TextOperation.Drag(this, action);
                        ShowChanged = true;
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
                        float per = 5000 / oy;
                        if (per < 0)
                            per = -per;
                        overTime += UserAction.TimeSlice;
                        if (overTime >= per)
                        {
                            overTime -= per;
                            if (oy > 0)
                            {
                                if(TextOperation.ContentMoveUp())
                                {
                                    TextCom.Text = TextOperation.GetShowContent();
                                    TextCom.Populate();
                                    ShowChanged = true;
                                }
                            }
                            else
                            {
                                if(TextOperation.ContentMoveDown())
                                {
                                    TextCom.Text = TextOperation.GetShowContent();
                                    TextCom.Populate();
                                    ShowChanged = true;
                                }
                            }
                            TextOperation.Drag(this, action);
                        }
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
                    TextCom.Text = TextOperation.GetShowContent();
                    TextCom.Populate();
                    ShowChanged = true;
                }
            }
            else 
            {
                if (TextOperation.ContentMoveDown())
                {
                    TextCom.Text = TextOperation.GetShowContent();
                    TextCom.Populate();
                    ShowChanged = true;
                }
            }
            base.OnMouseWheel(action);
        }
        internal override void OnClick(UserAction action)
        {
            Style = 0;
            InputCaret.Hide();
            if (Click != null)
                Click(this, action);
        }
        internal override void OnLostFocus(UserAction eventCall)
        {
            Style = 0;
            InputCaret.Hide();
            Focus = false;
        }
        protected void SetSetting()
        {
            if (TextCom == null)
                return;
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
        }
        protected void GetPreferredHeight()
        {
            SetSetting();
            string str = Text.FilterString;
            TextGenerator generator = HText.Generator;
            float h = generator.GetPreferredHeight(str, settings);
            HeightChange = PreferredHeight - h;
            PreferredHeight = h;
            int lc = generator.lineCount;
            LineCount = lc;
            float per = h / lc;
            ShowRow = (int)(Context.SizeDelta.y / per);
        }
        internal override void Update()
        {
            switch(Style)
            {
                case 0:
                    InputCaret.Hide();
                    break;
                case 2:
                    if(ShowChanged)
                    {
                        InputCaret.Active();
                        ShowChanged = false;
                        List<HVertex> hs = new List<HVertex>();
                        List<int> tris = new List<int>();
                        TextOperation.GetSelectArea(SelectionColor, tris, hs);
                        InputCaret.ChangeCaret(hs.ToArray(), tris.ToArray());
                    }
                    break;
            }
           if(Focus)
            {
                if (Keyboard.GetKeyDown(KeyCode.C) & Keyboard.GetKey(KeyCode.LeftControl))
                    GUIUtility.systemCopyBuffer = TextOperation.GetSelectString();
                if (Keyboard.GetKeyDown(KeyCode.A) & Keyboard.GetKey(KeyCode.LeftControl))
                {
                    Style = 2;
                    TextOperation.SelectAll();
                    ShowChanged = true;
                }
            }
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
                ShowStart = c;
                TextCom.Text = TextOperation.GetShowContent();
                TextCom.Populate();
                ShowChanged = true;
            }
        }
        bool IsSelectLine(int row)
        {
            int s = StartPress.Row;
            if (row == s)
                return true;
            int e = EndPress.Row;
            if (row == e)
                return true;
            if(s>e)
            {
                int t = s;
                s = e;
                e = t;
            }
            if (row > s & row < e)
                return true;
            return false;
        }
        public int StartIndex
        {
            get 
            {
                if (lines == null)
                    return 0;
                return lines[StartPress.Row].StartIndex + StartPress.Offset; 
            }
            set { SetIndex(ref StartPress, value); }
        }
        public int EndIndex
        {
            get { return lines[EndPress.Row].StartIndex + EndPress.Offset; }
            set { SetIndex(ref EndPress, value); }
        }
        void SetIndex(ref PressInfo press,int index)
        {
            if (index <= 0)
            {
                press.Row = 0;
                press.Offset = 0;
            }
            else if (index > cha.Length)
            {
                press.Row = lines.Length - 1;
                press.Offset = lines[StartPress.Row].Count;
            }
            else
            {
                int c = lines.Length - 1;
                for (int i = c; i >= 0; i--)
                {
                    if (lines[i].StartIndex < index)
                    {
                        press.Row = i;
                        press.Offset = index - lines[i].StartIndex;
                        return;
                    }
                }
                press.Row = 0;
                press.Offset = index;
            }
        }
    }
}
