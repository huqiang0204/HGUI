using huqiang.Core.HGUI;
using huqiang.Data;
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
        #region enum
        enum EditState
        {
            Done,
            Continue,
            NewLine,
            Finish
        }
        #endregion
        #region static
        /// <summary>
        /// 每秒5次
        /// </summary>
        static float KeySpeed = 220;
        static float MaxSpeed = 30;
        static float KeyPressTime;
        public static TextInput InputEvent { get; private set; }
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
                {
                    if (!InputEvent.Pressed)
                    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                        var state = KeyPressed();
                        if (state == EditState.Continue)
                        {
                            if (Keyboard.InputChanged)
                            {
                                if (Keyboard.InputString == "")
                                    return;
                                if (Keyboard.Nokey())
                                    InputEvent.OnInputChanged(IME.CurrentCompStr());
                                else
                                    InputEvent.OnInputChanged(Keyboard.InputString);
                            }
                        }
                        else if (state == EditState.Finish)
                        {
                            if (InputEvent.OnSubmit != null)
                                InputEvent.OnSubmit(InputEvent);
                        }
                        else if (state == EditState.NewLine)
                        {
                            InputEvent.OnInputChanged("\n");
                        }
#else
                        InputEvent.TouchInputChanged(Keyboard.TouchString);
                        if (Keyboard.status == TouchScreenKeyboard.Status.Done)
                        {
                            if (InputEvent.OnSubmit != null)
                                InputEvent.OnSubmit(InputEvent);
                            InputEvent.Refresh();
                            InputEvent = null;
                            return;
                        }
#endif
                    }
                }
                InputEvent.Refresh();
            }
        }
#endregion

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
            string str = GetShowString();
            if (str == ""&! Editing)
            {
                TextCom.MainColor = TipColor;
                TextCom.Text = m_TipString;
                InputCaret.CaretStyle = 0;
            }
            else
            {
                if (TextCom == null)
                    return;
                TextCom.MainColor = textColor;
                if (contentType == ContentType.Password)
                    TextCom.Text = new string('*',str.Length);
                else TextCom.Text = str;
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
            if(Text.FullString =="")
            {
                StartPress.Row = 0;
                StartPress.Offset = 0;
            }
        }
        internal override void OnClick(UserAction action)
        {
            if (InputEvent != this)
            {
                InputEvent = this;
                bool pass = InputEvent.contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(Text.FullString, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
                InputCaret.SetParent(Context.transform);
                pressOffset = StartPress.Offset;
                Editing = true;
            }else if(!Keyboard.active)
            {
                bool pass = InputEvent.contentType == ContentType.Password ? true : false;
                Keyboard.OnInput(Text.FullString, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
            }
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
                StartIndex = Keyboard.selection.start;
                EndIndex = Keyboard.selection.end;
                Style = 2;
                ShowChanged = true;
            }
            else
            {
                StartIndex = Keyboard.selection.start;
                SetShowStart();
                Style = 1;
            }
            if (OnValueChanged != null)
                OnValueChanged(this);
            SetShowText();
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
            InputCaret.CaretStyle = Style;
            var te = TextCom;
            if (te != null)
            {
                if (textChanged)
                {
                    textChanged = false;
                    GetPreferredHeight();
                }
                if (lineChanged)
                {
                    lineChanged = false;
                    SetShowText();
                    TextCom.Populate();
                    ShowChanged = true;
                }
                if(Style == 2)
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
                }else if(ShowChanged)
                {
                    ShowChanged = false;
                    SetShowText();
                }
            }
        }
        public void SetPressPointer()
        {
            if (TextCom == null)
                return;
            int line = StartPress.Row - ShowStart;
            if(line>=0)
            {
                var ul = TextCom.uILines;
                int c = ul.Length;
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
                int s = StartIndex;
                int e = EndIndex;
                if(e<s)
                {
                    StartIndex = e;
                    int t = s;
                    s = e;
                    e = t;
                }
                else if (s == e)
                    return false;
                Text.Remove(s, e - s);
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
            if (StartIndex < 1)
                return false;
            StartIndex--;
            if (Text.Remove(StartIndex))
            {
                textChanged = true;
                lineChanged = true;
                SetShowStart();
                return true;
            }
            return false;
        }
        public bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            Style = 1;
            if (Text.Remove(StartIndex))
            {
                if (StartIndex >= Text.FilterString.Length)
                {
                    if(StartPress.Row>0)
                    {
                        LineCount--;
                        StartPress.Row--;
                        StartPress.Offset = lines[StartPress.Row].Count;
                    }
                }
                SetShowStart();
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
            Text.Insert(StartIndex, es);
            GetPreferredHeight();
            StartIndex += c;
            pressOffset = StartPress.Offset;
            textChanged = true;
            lineChanged = true;
            SetShowStart();
        }
        public void PointerMoveLeft()
        {
            Style = 1;
            if (StartIndex > 0)
            {
                int c = StartPress.Row;
                StartIndex--;
                if (c != StartPress.Row)
                {
                    lineChanged = true;
                    SetShowStart();
                }
                pressOffset = StartPress.Offset;
            }
        }
        public void PointerMoveRight()
        {
            Style = 1;
            if (StartIndex<cha.Length-1)
            {
                int c = StartPress.Row;
                StartIndex++;
                if (c != StartPress.Row)
                {
                    lineChanged = true;
                    SetShowStart();
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
                var c = pressOffset;
                if (c > lines[StartPress.Row].Count)
                    c = lines[StartPress.Row].Count;
                StartPress.Offset = c;
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
            int l = LineCount;
            if (StartPress.Row < l - 1)
            {
                StartPress.Row++;
                var c = pressOffset;
                if (c > lines[StartPress.Row].Count)
                    c = lines[StartPress.Row].Count;
                StartPress.Offset = c;
                SetShowStart();
                lineChanged = true;
            }
        }
        public void PointerMoveStart()
        {
            Style = 1;
            if (StartPress.Row != 0)
                lineChanged = true;
            StartPress.Row = 0;
            StartPress.Offset = 0;
            ShowStart = 0;
        }
        public void PointerMoveEnd()
        {
            Style = 1;
            if (cha != null)
                StartIndex= cha.Length - 1;
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
        internal override void Update()
        {
        }
        void SetShowStart()
        {
            if (ShowStart > StartPress.Row)
                ShowStart = StartPress.Row;
            else if (ShowStart + ShowRow <= StartPress.Row)
                ShowStart = StartPress.Row - ShowRow + 1;
            if (ShowStart + ShowRow > LineCount)
                ShowStart = LineCount - ShowRow;
            if (ShowStart < 0)
                ShowStart = 0;
            ShowChanged = true;
        }
        public static void Clear()
        {
            InputEvent = null;
            InputCaret.Hide();
        }
    }
}