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
        protected static TextGenerationSettings settings;
        public HText TextCom;
        protected EmojiString Text = new EmojiString();
        protected string ShowContent;
        protected float overDistance = 500;
        protected float overTime = 0;
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
                InputCaret.SetParent(TextCom.transform);
                InputCaret.Styles = 2;
                TextOperation.ChangeText(TextCom,Text);
                TextOperation.SetStartPointer(this,action);
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
                                    ShowContent = TextCom.Text = TextOperation.GetShowContent();
                                    TextCom.Populate();
                                    ShowChanged = true;
                                }
                            }
                            else
                            {
                                if(TextOperation.ContentMoveDown())
                                {
                                    ShowContent = TextCom.Text = TextOperation.GetShowContent();
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
            ShowStart = TextOperation.ShowStart;
            ShowContent = TextOperation.GetShowContent();
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
           if(Focus)
            {
                if (Keyboard.GetKeyDown(KeyCode.C) & Keyboard.GetKey(KeyCode.LeftControl))
                    GUIUtility.systemCopyBuffer = TextOperation.GetSelectString();
                if (Keyboard.GetKeyDown(KeyCode.A) & Keyboard.GetKey(KeyCode.LeftControl))
                {
                    TextOperation.SelectAll();
                    ShowChanged = true;
                }
            }
            if (ShowChanged)
            {
                InputCaret.ChangeCaret();
                ShowChanged = false;
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
    }
}
