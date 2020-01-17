using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
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
    public class TextInput:TextSelect
    {
        #region enum
        enum EditState
        {
            Done,
            Continue,
            NewLine,
            Finish
        }
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
        #endregion
        #region static
        /// <summary>
        /// 每秒5次
        /// </summary>
        static float KeySpeed = 220;
        static float MaxSpeed = 30;
        static float KeyPressTime;
        static TextInput InputEvent;
        static EditState KeyPressed()
        {
            KeyPressTime -= UserAction.TimeSlice;
            if (Keyboard.GetKey(KeyCode.Backspace))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.DeleteLast();
                        InputEvent.SetShowText();
                    }
                    KeySpeed *= 0.8f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.Delete))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.DeleteNext();
                        InputEvent.SetShowText();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.LeftArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.PointerMoveLeft();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.RightArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.PointerMoveRight();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.UpArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.PointerMoveUp();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.DownArrow))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.PointerMoveDown();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            KeySpeed = 220f;
            if (Keyboard.GetKeyDown(KeyCode.Home))
            {
                InputEvent.PointerMoveStart();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.End))
            {
                InputEvent.PointerMoveEnd();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.A))
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        InputEvent.SelectAll();
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.X))//剪切
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        string str = InputEvent.GetSelectString();
                        InputEvent.DeleteSelectString();
                        GUIUtility.systemCopyBuffer = str;
                        InputEvent.SetShowText();
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.C))//复制
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        string str = InputEvent.GetSelectString();
                        GUIUtility.systemCopyBuffer = str;
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.V))//粘贴
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        InputEvent.OnInputChanged(Keyboard.systemCopyBuffer);
                    }
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.Return) | Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (InputEvent.lineType == LineType.MultiLineNewline)
                {
                    if (Keyboard.GetKey(KeyCode.RightControl))
                        return EditState.Finish;
                    return EditState.NewLine;
                }
                else return EditState.Finish;
            }
            if (Keyboard.GetKeyDown(KeyCode.Escape))
            {
                return EditState.Finish;
            }
            return EditState.Continue;
        }
        internal static void Dispatch()
        {
            if (InputEvent != null)
            {
                if (!InputEvent.ReadOnly)
                    if (!InputEvent.Pressed)
                    {
                        var state = KeyPressed();
                        if (state == EditState.Continue)
                        {
                            if (Keyboard.InputChanged)
                            {
                                if (Keyboard.InputString == "")
                                    return;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                                if (Keyboard.Nokey())
                                    InputEvent.OnInputChanged(IME.CurrentCompStr());
                                else
                                    InputEvent.OnInputChanged(Keyboard.InputString);
#else
                                   InputEvent.TouchInputChanged(Keyboard.InputString);
#endif
                            }
                        }
                        else if (state == EditState.Finish)
                        {
                            if (InputEvent.OnSubmit != null)
                                InputEvent.OnSubmit(InputEvent);
                        }
                        else if (state == EditState.NewLine)
                        {
                            InputEvent.OnInputChanged("\r");
                        }
                    }
                InputEvent.Refresh();
            }
        }
        #endregion

        string m_TipString = "";
        string m_inputString="";
        public string InputString { get { return Text.FullString; }
            set {
                value = ValidateString(value);
                Text.FullString = value;
                m_inputString = value;
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
            string str = GetShowString();
            if (str == "")
            {
                TextCom.Chromatically = TipColor;
                TextCom.Text = m_TipString;
                InputCaret.CaretStyle = 0;
            }
            else
            {
                TextCom.Chromatically = textColor;
                TextCom.Text = str;
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
        public CharacterValidation characterValidation = CharacterValidation.None;
        public TouchScreenKeyboardType touchType = TouchScreenKeyboardType.Default;
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
                    m_TipString = mod.buffer.GetData(tp->tipString) as string;
                    var str = mod.buffer.GetData(tp->inputString) as string;
                    str = ValidateString(str);
                    Text.FullString = str;
                    m_inputString = str;
                }
                else {
                    string str = txt.Text;
                    str = ValidateString(str);
                    Text.FullString = str;
                    m_inputString = str;
                }
            }
            AutoColor = false;
            Text.FullString = m_inputString;
            GetPreferredHeight();
        }
        public override void OnMouseDown(UserAction action)
        {
            base.OnMouseDown(action);
            if(m_inputString=="")
            {
                StartPress.Row = 0;
                StartPress.Index = 0;
                StartPress.Offset = 0;
            }
        }
        internal override void OnClick(UserAction action)
        {
            InputEvent = this;
            bool pass = InputEvent.contentType == ContentType.Password ? true : false;
            Keyboard.OnInput(m_inputString, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
            InputCaret.SetParent(Context.transform);
            pressOffset = StartPress.Offset;
            Editing = true;
            Style = 1;
        }
        internal override void OnLostFocus(UserAction action)
        {
            if (this == InputEvent)
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
                if (CharOperation.Validate(characterValidation, sb.ToString(), i, input[i]) != 0)
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }
        string OnInputChanged(string input)
        {
            if (input == "")
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
            if (CharOperation.Validate(characterValidation, Text.FullString, StartPress.Index, str[0]) == 0)
                return "";
            if (ValidateChar != null)
                if (ValidateChar(this, StartPress.Index, str[0]) == 0)
                    return "";
            InsertString(str);
            return input;
        }
        string TouchInputChanged(string input)
        {
            if (input == "")
                return "";
            //textInfo.buffer = new EmojiString(input);
            //if (OnValueChanged != null)
            //    OnValueChanged(this);
            //textInfo.text = textInfo.buffer.FullString;
            //SetShowText();
            //textInfo.CaretStyle = 1;
            //ChangePoint(textInfo, this);
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
        void Refresh()
        {
            var te = TextCom;
            if (te != null)
            {
                if (textChanged)
                {
                    textChanged = false;
                    GetPreferredHeight();
                    PointerChange(StartPress.Index);
                }
                if (lineChanged)
                {
                    lineChanged = false;
                    SetShowText();
                    TextCom.Populate();
                    ShowChanged = true;
                }
                if (Style == 1)
                {
                    SetPressPointer();
                }
                else if(Style==2)
                {
                    if(ShowChanged)
                    {
                        ShowChanged = false;
                        InputCaret.Active();
                        List<HVertex> hs = new List<HVertex>();
                        List<int> tris = new List<int>();
                        GetSelectArea(SelectionColor, tris, hs);
                        InputCaret.ChangeCaret(hs.ToArray(), tris.ToArray());
                    }
                }
            }
        }
        public void SetPressPointer()
        {
            int line = StartPress.Row - ShowStart;
            if(line>=0)
            {
                var ul = TextCom.uILines;
                int c = ul.Count;
                if (line < c)
                {
                    bool right = false;
                    int os = StartPress.Offset;
                    if (lines[StartPress.Row].Count == os)
                    {
                        right = true;
                        os--;
                    }
                    int index = ul[line].startCharIdx + os;
                    float h = TextCom.uILines[line].height;
                    var ch = TextCom.uIChars[index];
                    float rx = ch.cursorPos.x - 0.5f;
                    if (right)
                        rx += ch.charWidth + 1;
                    float lx = rx - 2f;
                    float ty = ch.cursorPos.y;
                    float dy = ty - h;
                    InputCaret.ChangeCaret(lx, rx, ty, dy, PointColor);
                }
            }
        }
        public bool DeleteSelectString()
        {
            if (Style == 2)
            {
                Style = 1;
                int s = StartPress.Index;
                int e = EndPress.Index;
                if (s == e)
                    return false;
                Text.Remove(s, e - s);
                if (StartPress.Index > EndPress.Index)
                    StartPress.Index = EndPress.Index;
                lineChanged = true;
                textChanged = true;
                return true;
            }
            return false;
        }
        public bool DeleteLast()
        {
            if (DeleteSelectString())
                return true;
            Style = 1;
            if (StartPress.Index < 1)
                return false;
            StartPress.Index--;
            if (Text.Remove(StartPress.Index))
            {
                textChanged = true;
                lineChanged = true;
                return true;
            }
            return false;
        }
        public bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            Style = 1;
            if (Text.Remove(StartPress.Index))
            {
                textChanged = true;
                lineChanged = true;
                return true;
            }
            return false;
        }
        public void InsertString(string str)
        {
            Style = 1;
            DeleteSelectString();
            var es = new EmojiString(str);
            int c = es.Length;
            Text.Insert(StartPress.Index, es);
            StartPress.Index += c;
            textChanged = true;
            lineChanged = true;
        }
        public void PointerMoveLeft()
        {
            Style = 1;
            if (StartPress.Index > 0)
            {
                StartPress.Index--;
                StartPress.Offset--;
                pressOffset = StartPress.Offset;
                int c = lines[StartPress.Row].StartIndex;
                if (StartPress.Index<c)
                {
                    StartPress.Row--;
                    c = lines[StartPress.Row].StartIndex;
                   pressOffset = StartPress.Offset = StartPress.Index - c;
                   if(StartPress.Row<ShowStart)
                    {
                        ShowStart = StartPress.Row;
                        lineChanged = true;
                        ShowChanged = true;
                    }
                }
                pressOffset = StartPress.Offset;
            }
        }
        public void PointerMoveRight()
        {
            Style = 1;
            if (StartPress.Row < lines.Length - 1 | StartPress.Offset < lines[StartPress.Row].Count)
            {
                StartPress.Offset++;
                if (StartPress.Offset != lines[StartPress.Row].Count)
                    StartPress.Index++;
                if (StartPress.Offset > lines[StartPress.Row].Count)
                {
                    StartPress.Row++;
                    pressOffset = StartPress.Offset = 0;
                    if (ShowStart + ShowRow < lines.Length)
                    {
                        ShowStart++;
                        lineChanged = true;
                        ShowChanged = true;
                    }
                }
                pressOffset = StartPress.Offset;
            }
        }
        public void PointerMoveUp()
        {
            Style = 1;
            if (StartPress.Row > 0)
            {
                StartPress.Row--;
                var c =pressOffset;
                if (c > lines[StartPress.Row].Count)
                    c = lines[StartPress.Row].Count;
                StartPress.Offset = c;
                StartPress.Index = lines[StartPress.Row].StartIndex + c;
                if(StartPress.Row<ShowStart)
                {
                    ShowStart = StartPress.Row;
                    ShowChanged = true;
                    lineChanged = true;
                }
            }
        }
        public void PointerMoveDown()
        {
            Style = 1;
            int l = lines.Length - 1;
            if (StartPress.Row < l)
            {
                StartPress.Row++;
                var c = pressOffset;
                if (c > lines[StartPress.Row].Count)
                    c = lines[StartPress.Row].Count;
                StartPress.Offset = c;
                StartPress.Index = lines[StartPress.Row].StartIndex + c;
                if(ShowStart+ShowRow<StartPress.Row+1)
                {
                    ShowStart = StartPress.Row - ShowRow+1;
                    ShowChanged = true;
                    lineChanged = true;
                }
            }
        }
        public void PointerMoveStart()
        {
            Style = 1;
            if (StartPress.Row != 0)
                lineChanged = true;
            StartPress.Row = 0;
            StartPress.Index = 0;
            StartPress.Offset = 0;
            ShowStart = 0;
        }
        public void PointerMoveEnd()
        {
            Style = 1;
            if (cha != null)
                StartPress.Index = cha.Length - 1;
            if (StartPress.Index < 0)
                StartPress.Index = 0;
            if (lines != null)
            {
                if (lines.Length > 0)
                {
                    var c = lines.Length - ShowRow;
                    if (c < 0)
                        c = 0;
                    if (StartPress.Row != c)
                        lineChanged = true;
                    StartPress.Row = c;
                    StartPress.Offset = lines[StartPress.Row].Count;
                    ShowStart = StartPress.Row - ShowRow + 1;
                    if (ShowStart <0)
                    {
                        ShowStart = 0;
                    }
                }
            }
        }
        public void PointerChange(int index)
        {
            if (index < 0)
                index = 0;
            if (index >= cha.Length)
                index = cha.Length - 1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartIndex + lines[i].Count >= index)
                {
                    StartPress.Index = index;
                    StartPress.Row = i;
                    pressOffset = StartPress.Offset = index - lines[i].StartIndex;
                    break;
                }
            }
            if (StartPress.Row < ShowStart)
            {
                ShowStart = StartPress.Row;
            }else
            if (ShowStart + ShowRow < StartPress.Row +1)
            {
                ShowStart = StartPress.Row - ShowRow + 1;
            }else if(ShowStart+ShowRow>LineCount)
            {
                ShowStart = LineCount - ShowRow;
                if (ShowStart < 0)
                    ShowStart = 0;
            }
        }
        internal override void Update()
        {
        }
    }
}