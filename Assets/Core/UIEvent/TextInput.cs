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
                        InputEvent.MoveUp();
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
                        InputEvent.MoveDown();
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
        static readonly char[] Separators = { ' ', '.', ',', '\t', '\r', '\n' };
        const string EmailCharacters = "!#$%&'*+-/=?^_`{|}~";
        static char Validate(CharacterValidation validat, string text, int pos, char ch)
        {
            if (validat == CharacterValidation.None)
                return ch;
            if (validat == CharacterValidation.Integer)
            {
                if (ch == '-')
                {
                    if (text == "")
                        return ch;
                    if (text.Length > 0)
                        return (char)0;
                }
                if (ch < '0' | ch > '9')
                    return (char)0;
                return ch;
            }
            else if (validat == CharacterValidation.Decimal)
            {
                if (ch >= '0' && ch <= '9')
                {
                    if (ch == '.')
                        if (text.IndexOf('.') < 0)
                            return ch;
                    return (char)0;
                }
                return ch;
            }
            else if (validat == CharacterValidation.Alphanumeric)
            {
                // All alphanumeric characters
                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.numberAndName)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.Name)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
            }
            else if (validat == CharacterValidation.EmailAddress)
            {

                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
                if (ch == '@' && text.IndexOf('@') == -1) return ch;
                if (EmailCharacters.IndexOf(ch) != -1) return ch;
                if (ch == '.')
                {
                    char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
                    char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
                    if (lastChar != '.' && nextChar != '.')
                        return ch;
                }
            }
            return (char)0;
        }
        #endregion

        string m_TipString = "";
        string m_inputString="";
        public string InputString { get { return m_inputString; }
            set {
                m_inputString = value;
                value = ValidateString(value);
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
            //m_inputString = textControll.GetFullString();
            //string str = textControll.GetShowString();
            //if (str== "" )
            //{
            //    TextCom.Chromatically = TipColor;
            //    TextCom.Text = m_TipString;
            //}
            //else
            //{
            //    TextCom.Chromatically = textColor;
            //    TextCom.Text = str;
            //}
        }
        public bool ReadOnly;
        Color textColor = Color.black;
        Color m_tipColor = new Color(0, 0, 0, 0.8f);
        public Color TipColor { get { return m_tipColor; } set { m_tipColor = value;} }
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
            Text.FullString = m_inputString;
            GetPreferredHeight();
        }
        internal override void OnClick(UserAction action)
        {
            InputEvent = this;
            //textControll.Context = TextCom;
            //textControll.SetFullString(new EmojiString(m_inputString));
            //textControll.ReCalcul();
            //textControll.MoveToEnd();
            //TextCom.Text = textControll.GetShowString();
            bool pass = InputEvent.contentType == ContentType.Password ? true : false;
            Keyboard.OnInput(m_inputString, InputEvent.touchType, InputEvent.multiLine, pass, CharacterLimit);
            InputCaret.SetParent(Context.transform);
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
            //string str = textControll.GetFullString();
            //if (CharacterLimit > 0)
            //{
            //    string fs = es.FilterString;
            //    if (fs.Length + str.Length > CharacterLimit)
            //    {
            //        int len = CharacterLimit - str.Length;
            //        if (len <= 0)
            //            return "";
            //        es.Remove(fs.Length - len, len);
            //    }
            //}
            //str = es.FullString;
            
            //if (Validate(characterValidation, textInfo.text, textInfo.startSelect, str[0]) == 0)
            //    return "";
            //if (ValidateChar != null)
            //    if (ValidateChar(this, textInfo.startSelect, str[0]) == 0)
            //        return "";
            //textControll.InsertString(str);
            //textControll.ReCalcul();
            //textControll.AdjustToPoint();
            //textControll.AdjustStartLine();
            SetShowText();
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
                if (Style == 0)
                {
                    SetPressPointer();
                }
                else
                {
                    List<HVertex> hs = new List<HVertex>();
                    List<int> tris = new List<int>();
                    GetSelectArea(SelectionColor, tris, hs);
                    InputCaret.ChangeCaret(hs.ToArray(),tris.ToArray());
                }
            }
        }
        public void SetPressPointer()
        {
            //int index = textControll.GetPressIndex();
            //if (index > -1)
            //{
            //    var uc = TextCom.uIChars;
            //    if (uc != null)
            //    {
            //        if (index < uc.Count)
            //        {
            //            int line = textControll.GetPressLine();
            //            float h = TextCom.uILines[line].height;
            //            var ch = TextCom.uIChars[index];
            //            float rx = ch.cursorPos.x - 0.5f;
            //            float lx = rx - 2f;
            //            float ty = ch.cursorPos.y;
            //            float dy = ty - h;
            //            InputCaret.ChangeCaret(lx, rx, ty, dy,PointColor);
            //        }
            //    }
            //}
        }
        public bool DeleteSelectString()
        {
            if (Style == 1)
            {
                Style = 0;
                int s = StartPress.Index;
                int e = EndPress.Index;
                if (s == e)
                    return false;
                Text.Remove(s, e - s);
                if (StartPress.Index > EndPress.Index)
                    StartPress.Index = EndPress.Index;
                //textChanged = true;
                return true;
            }
            return false;
        }
        public bool DeleteLast()
        {
            if (DeleteSelectString())
                return true;
            if (StartPress.Index < 1)
                return false;
            StartPress.Index--;
            if (Text.Remove(StartPress.Index))
            {
                //textChanged = true;
                return true;
            }
            return false;
        }
        public bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            if (Text.Remove(StartPress.Index))
            {
                //textChanged = true;
                return true;
            }
            return false;
        }
        public void InsertString(string str)
        {
            DeleteSelectString();
            var es = new EmojiString(str);
            int c = es.Length;
            Text.Insert(StartPress.Index, es);
            StartPress.Index += c;
            //textChanged = true;
        }
        public void PointerMoveLeft()
        {
            //if (StartIndex > 0)
            //{
            //    StartIndex--;
            //    if (StartIndex < lines[StartLine].startCharIdx)
            //    {
            //        StartLine--;
            //    }
            //    LineOffset = StartIndex - lines[StartLine].startCharIdx;
            //    if (StartLine < ShowStart)
            //    {
            //        ChangeShowLine(StartLine);
            //    }
            //}
        }
        public void PointerMoveRight()
        {
            //if (StartIndex < uchars.Length - 1)
            //{
            //    StartIndex++;
            //    if (StartLine < lines.Length - 1)
            //    {
            //        if (lines[StartLine + 1].startCharIdx <= StartIndex)
            //        {
            //            StartLine++;
            //        }
            //    }
            //    LineOffset = StartIndex - lines[StartLine].startCharIdx;
            //    if (StartLine - ShowRow > ShowStart)
            //    {
            //        ChangeShowLine(StartLine - ShowRow);
            //    }
            //}
        }
        public void PointerMoveStart()
        {
            Style = 0;
            StartPress.Row = 0;
            StartPress.Index = 0;
            StartPress.Offset = 0;
        }
        public void PointerMoveEnd()
        {
            Style = 0;
            if (cha != null)
                StartPress.Index = cha.Length - 1;
            if (StartPress.Index < 0)
                StartPress.Index = 0;
            if (lines != null)
            {
                if (lines.Length > 0)
                {
                    StartPress.Row = lines.Length - 1;
                    StartPress.Offset = lines[StartPress.Row].Count;
                }
            }
        }
    }
}