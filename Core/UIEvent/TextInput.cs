using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.Data2D;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace huqiang.UIEvent
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
    public unsafe struct TextInputData
    {
        public Color inputColor;
        public Color tipColor;
        public Color pointColor;
        public Color selectColor;
        public Int32 inputString;
        public Int32 tipString;
        public int CharacterLimit;
        public bool ReadyOnly;
        public ContentType contentType;
        public LineType lineType;
        public static int Size = sizeof(TextInputData);
        public static int ElementSize = Size / 4;
    }
    public class TextInput:TextSelect
    {
        string m_TipString = "";
        public string InputString { get { return Text.FullString; }
            set {
                value = ValidateString(value);
                Text.FullString = value;
                GetPreferredHeight();
                PointerMoveEnd();
                SetShowText();
            } }
        public string TipString
        {
            get { return m_TipString; }
            set
            {
                m_TipString = value;
            }
        }
        void SetShowText()
        {
            var str = InputString + Keyboard.TempString;
            if (Editing | (str != "" & str != null))
            {
                if (TextCom == null)
                    return;
                str = TextOperation.GetShowContent();//GetShowString();
                TextCom.MainColor = textColor;
                if (contentType == ContentType.Password)
                    TextCom.Text = new string('*', str.Length);
                else TextCom.Text = str;
            }
            else
            {
                TextCom.MainColor = TipColor;
                TextCom.Text = m_TipString;
                InputCaret.CaretStyle = 0;
            }
        }
        public bool ReadOnly;
        bool lineChanged;
        bool textChanged;
        Color textColor = Color.black;
        Color m_tipColor = new Color(0, 0, 0, 0.8f);
        public Color TipColor { get { return m_tipColor; } set { m_tipColor = value;} }
        public Func<TextInput, int, char, char> ValidateChar;
        public Action<TextInput> OnValueChanged;
        public Action<TextInput> OnSubmit;
        public Action<TextInput> OnDone;
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
        CharacterValidation characterValidation = CharacterValidation.None;
        TouchScreenKeyboardType touchType = TouchScreenKeyboardType.Default;
        public int CharacterLimit = 0;
        int pressOffset;
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
                    CharacterLimit = tp->CharacterLimit;
                    ReadOnly = tp->ReadyOnly;
                    contentType = tp->contentType;
                    lineType = tp->lineType;
                    m_TipString = mod.buffer.GetData(tp->tipString) as string;
                    var str = mod.buffer.GetData(tp->inputString) as string;
                    str = ValidateString(str);
                    Text.FullString = str;
                }
                else {
                    string str = txt.Text;
                    str = ValidateString(str);
                    Text.FullString = str;
                }
            }
            AutoColor = false;
            GetPreferredHeight();
        }
        public override void OnMouseDown(UserAction action)
        {
            base.OnMouseDown(action);
            InputCaret.Styles = 3;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            Editing = true;
            SetShowText();
#endif
        }
        internal override void OnClick(UserAction action)
        {
            if (Keyboard.InputEvent != this)
            {
                Keyboard.InputEvent = this;
                bool pass = contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(Text.FullString, touchType, multiLine, pass, CharacterLimit);
                InputCaret.SetParent(Context.transform);
                pressOffset = StartPress.Offset;
                Editing = true;
            }
            else
            if(!Keyboard.active)
            {
                bool pass = contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(Text.FullString, touchType, multiLine, pass, CharacterLimit);
            }
            Style = 1;
            if (Click != null)
                Click(this,action);
        }
        internal override void OnLostFocus(UserAction action)
        {
            if (this == Keyboard.InputEvent)
            {
                Keyboard.InputEvent = null;
            }
            if (OnDone != null)
                OnDone(this);
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
                if (CharOperation.Validate(characterValidation, sb.ToString(), i, input[i]) != 0)
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }
        internal string OnInputChanged(string input)
        {
            if (input == null | input == "")
                return "";
            EmojiString es = new EmojiString(input);
            string str = Text.FullString;
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
            int s = StartIndex;
            if (CharOperation.Validate(characterValidation, Text.FullString, s, str[0]) == 0)
                return "";
            if (ValidateChar != null)
                if (ValidateChar(this, s, str[0]) == 0)
                    return "";
            InsertString(str);
            return input;
        }
        string TouchInputChanged(string input)
        {
            if(Keyboard.InputChanged)
            {
                Text.FullString = input;
                GetPreferredHeight();
            }
            if (Keyboard.selection.length > 0)
            {
                TextOperation.SetStartPressIndex(Keyboard.selection.start);
                TextOperation.SetEndPressIndex(Keyboard.selection.end);
                ShowChanged = true;
            }
            else
            {
                bool b = false;
                TextOperation.SetPressIndex(Keyboard.selection.start,ref b);
            }
            if (OnValueChanged != null)
                OnValueChanged(this);
            SetShowText();
            return input;
        }
        public bool Editing;
        public bool DeleteSelectString()
        {
            if(TextOperation.DeleteSelectString())
            {
                ShowChanged = true;
                SetShowText();
                return true;
            }
            return false;
        }
        public bool DeleteLast()
        {
            if(TextOperation.DeleteLast())
            {
                ShowChanged = true;
                SetShowText();
                return true;
            }
            return false;
        }
        public bool DeleteNext()
        {
            if (TextOperation.DeleteNext())
            { 
                ShowChanged = true;
                SetShowText();
                return true;
            }
            return false;
        }
        public void InsertString(string str)
        {
            Style = 1;
            var es = new EmojiString(str);
            TextOperation.DeleteSelectString();
            TextOperation.InsertContent(es);
            ShowChanged = true;
            SetShowText();
        }
        public void PointerMoveLeft()
        {
            bool lc = false;
            if (TextOperation.PointerMoveLeft(ref lc))
            {
                ShowChanged = true;
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveRight()
        {
            bool lc = false;
            if (TextOperation.PointerMoveRight(ref lc))
            {
                ShowChanged = true;
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveUp()
        {
            bool lc = false;
            if (TextOperation.PointerMoveUp(ref lc))
            {
                ShowChanged = true;
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveDown()
        {
            bool lc = false;
            if (TextOperation.PointerMoveDown(ref lc))
            {
                ShowChanged = true;
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveStart()
        {
            bool lc = false;
            if (TextOperation.SetPressIndex(0, ref lc))
            {
                ShowChanged = true;
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveEnd()
        {
            bool lc = false;
            if (TextOperation.SetPressIndex(999999999, ref lc))
            {
                ShowChanged = true;
                if (lc)
                    SetShowText();
            }
        }
        internal override void Update()
        {
            base.Update();
        }
        public static void Clear()
        {
            InputCaret.Hide();
        }
    }
}