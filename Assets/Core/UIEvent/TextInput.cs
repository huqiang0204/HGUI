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
    public partial class TextInput:UserEvent
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
        static TextControll textControll = new TextControll();
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
                        textControll.DeleteLast();
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
                        textControll.DeleteNext();
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
                        textControll.PointerMoveLeft();
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
                        textControll.PointerMoveRight();
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
                        textControll.PointerMoveUp();
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
                        textControll.PointerMoveDown();
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
                textControll.PointerMoveStart();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.End))
            {
                textControll.PointerMoveEnd();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.A))
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        textControll.SelectAll();
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
                        string str = textControll.GetSelectString();
                        textControll.DeleteSelectString();
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
                        string str = InputEvent.SelectString;
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
                            InputEvent.OnInputChanged(Environment.NewLine);
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
            m_inputString = textControll.GetFullString();
            string str = textControll.GetShowString();
            if (str== "" )
            {
                TextCom.Chromatically = TipColor;
                TextCom.Text = m_TipString;
            }
            else
            {
                TextCom.Chromatically = textColor;
                TextCom.Text = str;
            }
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
                textControll.Context = TextCom;
                textControll.SetFullString(new EmojiString(m_inputString));
                textControll.ReCalcul();
                textControll.SetStartSelect(GetPressIndex(action,0));
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
                        if (action.CanPosition.y < RawPosition.y)
                            textControll.SetEndSelect(GetPressIndex(action, 0.2f));
                        else textControll.SetEndSelect(GetPressIndex(action, -0.8f));
                    }
                    else if(!entry)
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
                                textControll.PointerMoveUp();
                            else  textControll.PointerMoveDown();
                        }
                    }
                }
            base.OnDrag(action);
        }
        internal override void OnMouseWheel(UserAction action)
        {
            float oy = action.MouseWheelDelta;
            if (oy > 0)
                textControll.PointerMoveUp();
            else textControll.PointerMoveDown();
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
            if (action.CanPosition.y < RawPosition.y)
                textControll.SetEndSelect(GetPressIndex(action, 0.2f));
            else textControll.SetEndSelect(GetPressIndex(action, -0.8f));
            base.OnDragEnd(action);
        }
        void OnClick(UserEvent eventCall, UserAction action)
        {
            TextInput input = eventCall as TextInput;
            if (input == null)
                return;
            InputEvent = input;
            textControll.Context = TextCom;
            textControll.SetFullString(new EmojiString(m_inputString));
            textControll.ReCalcul();
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
            textControll.InsertString(str);
            textControll.ReCalcul();
            textControll.AdjustToPoint();
            textControll.AdjustStartLine();
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
        public float Percentage { get => textControll.Percentage;
            set => textControll.Percentage = value; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="action"></param>
        /// <param name="dir">从上往下0.2,从下网上为-0.8<</param>
        /// <returns>x=行,y=索引,z=行偏移</returns>
        Vector3Int GetPressIndex(UserAction action, float dir)
        {
            Vector3Int v3 = Vector3Int.zero;
            if (TextCom == null)
                return v3;
            if (m_inputString == "")
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
                        if (lines[i].topY + dir* lines[i].height < my)
                        {
                            r = i;
                            break;
                        }
                    }
                }
            }
            int count =0;
            if (r + 1 < lines.Count)
                count = lines[r + 1].startCharIdx - lines[r].startCharIdx;
            else count = uchars.Count - lines[r].startCharIdx;
            int s = lines[r].startCharIdx;
            float lx = uchars[s].cursorPos.x;
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
                        if (mx >= uchars[s].cursorPos.x)
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
            //int s = StartIndex;
            //int e = EndIndex;
            //if (e < s)
            //{
            //    int t = s;
            //    s = e;
            //    e = t;
            //}
            //int c = e - s;
            //vert.Clear();
            //tri.Clear();
            //int len = EndLine + 1;
            //for (int i = StartLine; i < len; i++)
            //{
            //    int os = lines[i].startCharIdx;
            //    int oe = uchars.Length;
            //    if (i < len - 1)
            //        oe = lines[i + 1].startCharIdx - 1;
            //    int state = CommonArea(s, e, ref os, ref oe);
            //    if (state == 2)//结束
            //        break;
            //    if (state == 1)//包含公共区域
            //    {
            //        float lx = uchars[os].cursorPos.x - uchars[os].charWidth * 0.5f;
            //        float rx = uchars[oe].cursorPos.x + uchars[oe].charWidth * 0.5f;
            //        float h = lines[i].height;
            //        float top = lines[i].topY;
            //        float down = top - h;
            //        int st = vert.Count;
            //        var v = new HVertex();
            //        v.position.x = lx;
            //        v.position.y = down;
            //        v.color = color;
            //        vert.Add(v);
            //        v.position.x = rx;
            //        v.position.y = down;
            //        v.color = color;
            //        vert.Add(v);
            //        v.position.x = lx;
            //        v.position.y = top;
            //        v.color = color;
            //        vert.Add(v);
            //        v.position.x = rx;
            //        v.position.y = top;
            //        v.color = color;
            //        vert.Add(v);
            //        tri.Add(st);
            //        tri.Add(st + 2);
            //        tri.Add(st + 3);
            //        tri.Add(st);
            //        tri.Add(st + 3);
            //        tri.Add(st + 1);
            //    }
            //}
        }
    }
}