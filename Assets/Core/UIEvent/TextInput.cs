using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Text;
using UGUI;
using UnityEngine;

namespace huqiang.UIEvent
{
    public unsafe struct TextInputData
    {
        public Color inputColor;
        public Color tipColor;
        public Color pointColor;
        public Color selectColor;
        public Int32 inputString;
        public Int32 tipString;
        public static int Size = sizeof(TextInputData);
        public static int ElementSize = Size / 4;
    }
    public partial class TextInput:UserEvent
    {
        public enum ContentType
        {
            Standard,
            Autocorrected,
            IntegerNumber,
            DecimalNumber,
            Alphanumeric,
            Name,
            NumberAndName,
            EmailAddress,
            Password,
            Pin,
            Custom
        }
        public enum InputType
        {
            Standard,
            AutoCorrect,
            Password,
        }
        public enum LineType
        {
            SingleLine,
            MultiLineSubmit,
            MultiLineNewline
        }
        public enum CharacterValidation
        {
            None,
            Integer,
            Decimal,
            Alphanumeric,
            Name,
            numberAndName,
            EmailAddress,
            Custom
        }
        static TextControll textControll = new TextControll();
        string m_TipString = "";
        string m_inputString="";
        public string InputString { get { return m_inputString; }
            set {
                m_inputString = value;
                value = ValidateString(value);
                //textInfo.buffer.FullString = value;
                //textInfo.text = value;
                //textInfo.startSelect = 0;
                //textInfo.endSelect = -1;
                //SetShowText();
                textChanged = true;
                selectChanged = true;
            } }
        public string TipString
        {
            get { return m_TipString; }
            set
            {
                m_TipString = value;
                SetShowText();
            }
        }
        void SetShowText()
        {
            //if (textInfo.text == ""| textInfo.text==null)
            //{
            //    TextCom.Chromatically = TipColor;
            //    TextCom.Text = m_TipString;
            //}
            //else
            //{
            //    TextCom.Chromatically = textColor;
            //    TextCom.Text = textInfo.ShowString.FullString;
            //}
        }
        public bool ReadOnly;
        Color textColor=Color.black;
        Color m_tipColor = new Color(0, 0, 0, 0.8f);
        public Color TipColor { get { return m_tipColor; } set { m_tipColor = value;} }
        public Color PointColor = Color.white;
        public Color SelectionColor = new Color(0.65882f, 0.8078f, 1, 0.2f);
        public Func<TextInput, int, char, char> ValidateChar;
        public Action<TextInput> OnValueChanged;
        public Action<TextInput> OnSubmit;
        public Action<TextInput> OnDone;
        public Action<TextInput> LineChanged;
        public Action<TextInput, UserAction> OnSelectChanged;
        public Action<TextInput, UserAction> OnSelectEnd;
        public InputType inputType = InputType.Standard;
        public LineType lineType = LineType.MultiLineNewline;
        ContentType m_ctpye;
        bool multiLine = true;
        public ContentType contentType
        {
            get { return m_ctpye; }
            set
            {
                m_ctpye = value;
                switch (value)
                {
                    case ContentType.Standard:
                        {
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.Autocorrected:
                        {
                            inputType = InputType.AutoCorrect;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.IntegerNumber:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NumberPad;
                            characterValidation = CharacterValidation.Integer;
                            break;
                        }
                    case ContentType.DecimalNumber:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NumbersAndPunctuation;
                            characterValidation = CharacterValidation.Decimal;
                            break;
                        }
                    case ContentType.Alphanumeric:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.ASCIICapable;
                            characterValidation = CharacterValidation.Alphanumeric;
                            break;
                        }
                    case ContentType.Name:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NamePhonePad;
                            characterValidation = CharacterValidation.Name;
                            break;
                        }
                    case ContentType.NumberAndName:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NamePhonePad;
                            characterValidation = CharacterValidation.numberAndName;
                            break;
                        }
                    case ContentType.EmailAddress:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.EmailAddress;
                            characterValidation = CharacterValidation.EmailAddress;
                            break;
                        }
                    case ContentType.Password:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Password;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.Pin:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Password;
                            touchType = TouchScreenKeyboardType.NumberPad;
                            characterValidation = CharacterValidation.Integer;
                            break;
                        }
                    default:
                        {
                            // Includes Custom type. Nothing should be enforced.
                            break;
                        }
                }
            }
        }
        public CharacterValidation characterValidation = CharacterValidation.None;
        public TouchScreenKeyboardType touchType = TouchScreenKeyboardType.Default;
        public int CharacterLimit = 0;
        float overDistance = 500;
        float overTime = 0;
        public TextInput()
        {
            Click = OnClick;
            LostFocus = OnLostFocus;
        }
        internal override void Initial(FakeStruct mod)
        {
            var txt = TextCom = Context as HText;
            textColor = txt.m_color;
            unsafe
            {
                var ex = mod.buffer.GetData(((TransfromData*)mod.ip)->ex) as FakeStruct;
                if (ex != null)
                {
                    TextInputData* tp = (TextInputData*)ex.ip;
                    textColor = tp->inputColor;
                    m_tipColor = tp->tipColor;
                    PointColor = tp->pointColor;
                    SelectionColor = tp->selectColor;
                    m_TipString = mod.buffer.GetData(tp->tipString) as string;
                    InputString = mod.buffer.GetData(tp->inputString) as string;
                }
                else InputString = txt.Text;
            }
            AutoColor = false;
        }
        public HText TextCom { get; private set; }
        public override void OnMouseDown(UserAction action)
        {
            overTime = 0;
            if (TextCom != null)
            {
                textControll.SetStartSelect(this,action);
                Editing = true;
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
                        textControll.SetEndSelect(this,action);
                        //textInfo.CaretStyle = 2;
                        //int end = textInfo.endSelect;
                        //textInfo.endSelect = GetPressIndex(textInfo, this, action, ref textInfo.endDock) + textInfo.StartIndex;
                        //if (end != textInfo.endSelect)
                        //{
                        //    Selected();
                        //    if (OnSelectChanged != null)
                        //        OnSelectChanged(this, action);
                        //    selectChanged = true;
                        //}
                    }else if(!entry)
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
                            if(oy>0)
                            {
                                textControll.PointerMoveUp();
                            }
                            else
                            {
                                textControll.PointerMoveDown();
                            }
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
                textControll.PointerMoveUp();
            }
            else
            {
                textControll.PointerMoveDown();
            }
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
            textControll.SetEndSelect(this,action);
            base.OnDragEnd(action);
        }
        void OnClick(UserEvent eventCall, UserAction action)
        {
            TextInput input = eventCall as TextInput;
            if (input == null)
                return;
            InputEvent = input;
            textControll.SetFullString(new EmojiString(m_inputString));
            textControll.ReCalcul(input.TextCom);
            textControll.MoveToEnd();
            TextCom.Text = textControll.GetShowString();
            bool pass = InputEvent.contentType == ContentType.Password ? true : false;
            Keyboard.OnInput(m_inputString, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
            InputCaret.SetParent(Context.transform);
        }
        void OnLostFocus(UserEvent eventCall, UserAction action)
        {
            TextInput text = eventCall as TextInput;
            if (text == InputEvent)
            {
                if (InputEvent.OnDone != null)
                    InputEvent.OnDone(InputEvent);
                InputEvent = null;
            }
            Editing = false;
            SetShowText();

            InputCaret.Hide();
            Keyboard.EndInput();
        }
        string ValidateString(string input)
        {
            if (CharacterLimit > 0)
                if (input.Length > CharacterLimit)
                    input = input.Substring(0,CharacterLimit);
            if (characterValidation == CharacterValidation.None)
                return input;
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<input.Length;i++)
            {
                if (Validate(characterValidation, sb.ToString(), i, input[i]) != 0)
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }
        string OnInputChanged(string input)
        {
            if (input == "")
                return "";
            EmojiString es = new EmojiString(input);
            string str = textControll.GetFullString();
            if (CharacterLimit > 0)
            {
                string fs = es.FilterString;
                if (fs.Length + str.Length > CharacterLimit)
                {
                    int len = CharacterLimit - str.Length;
                    if (len <= 0)
                        return "";
                    es.Remove(fs.Length - len, len);
                }
            }
            str = es.FullString;
            //if (Validate(characterValidation, textInfo.text, textInfo.startSelect, str[0]) == 0)
            //    return "";
            //if (ValidateChar != null)
            //    if (ValidateChar(this, textInfo.startSelect, str[0]) == 0)
            //        return "";
            //DeleteSelected(textInfo);
            //textInfo.buffer.Insert(textInfo.startSelect,es);
            //textInfo.startSelect += es.FilterString.Length;
            //if (OnValueChanged != null)
            //    OnValueChanged(this);
            //textInfo.text = textInfo.buffer.FullString;
            //SetShowText();
            //textInfo.CaretStyle = 1;
            //selectChanged = true;
            //textChanged = true;
            return input;
        }
        string TouchInputChanged(string input)
        {
            if (input == "")
                return "";
            //textInfo.buffer= new EmojiString(input);
            //if (OnValueChanged != null)
            //    OnValueChanged(this);
            //textInfo.text = textInfo.buffer.FullString;
            //SetShowText();
            //textInfo.CaretStyle = 1;
            //ChangePoint(textInfo,this);
            selectChanged = true;
            textChanged = true;
            return input;
        }
        public bool Editing;
        public static void SetCurrentInput(TextInput input, UserAction action)
        {
            if (input == null)
                return;
            if (InputEvent == input)
                return;
            if (InputEvent != null)
               InputEvent.LostFocus(InputEvent, action);
            InputEvent = input;
            InputEvent.Editing = true;
        }

        bool textChanged;
        bool selectChanged;
        bool lineChanged;
        public string SelectString { get => textControll.GetSelectString(); }

        void Refresh()
        {
            var te = TextCom;
            if (te != null)
            {
                //if(textChanged)
                //{
                //    textInfo.fontSize = TextCom.m_fontSize;
                //    textChanged = false;
                //    GetPreferredHeight(TextCom,textInfo);
                //    textInfo.StartLine += textInfo.LineChange;
                //    textInfo.EndLine += textInfo.LineChange;
                //    lineChanged = true;
                //    var lines = textInfo.fullLines;
                //    if (lines != null)
                //    {
                //        int i = lines.Length - 1;
                //        int start = textInfo.startSelect;
                //        for (; i >= 0; i--)
                //        {
                //            int t = lines[i].startCharIdx;
                //            if (t <= start)
                //            {
                //                textInfo.lineIndex = start - t;
                //                break;
                //            }
                //        }
                //    }
                //}
                //if(lineChanged)
                //{
                //    lineChanged = false;
                //    FilterPopulate(TextCom, textInfo);
                //    SetShowText();
                //}
                //if(selectChanged)
                //{
                //    textInfo.fontSize = TextCom.m_fontSize;
                //    textInfo.caretColor = PointColor;
                //    textInfo.areaColor = SelectionColor;
                //    selectChanged = false;
                //    if (textInfo.CaretStyle == 2)
                //        FilterChoiceArea(TextCom, textInfo);
                //    else ChangePoint(textInfo,this);
                //    InputCaret.ChangeCaret(textInfo);
                //}  
            }
        }
        public void SizeChanged()
        {
            textChanged = true;
            selectChanged = true;
        }
        public float Percentage { get => textControll.Percentage;
            set => textControll.Percentage = value; }
    }
}