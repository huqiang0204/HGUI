using huqiang.Core.HGUI;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace huqiang.UIComposite
{
    /// <summary>
    /// 文本输入框
    /// </summary>
    public class InputBox:Composite
    {
        static float KeySpeed = 420;
        static float MaxSpeed = 100;
        static float KeyPressTime;
        static List<HVertex> hs = new List<HVertex>();
        static List<int> tris = new List<int>();
        Color32 textColor;
        Color32 m_tipColor;
        Color32 PointColor = new Color32(255, 0, 0, 255);
        Color32 SelectionColor = new Color32(168, 206, 255, 64);
        int CharacterLimit;
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly;
        string m_TipString;
        string m_InputString;
        public InputType inputType = InputType.Standard;
        public LineType lineType = LineType.SingleLine;
        ContentType m_ctpye;
        bool Editing;
        StringEx FullString=new StringEx("",false);
        public char PasswordCharacter = '●';
        HImage Caret;
        public PressInfo startPress;
        public PressInfo endPress;
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object DataContext;
        /// <summary>
        /// 提示文本
        /// </summary>
        public string TipString { get { return m_TipString; } set { 
                m_TipString = value;
                SetShowText();
            } }
        /// <summary>
        /// 输入文本
        /// </summary>
        public string InputString { get { return FullString.FullString; } set {
                if (value == null)
                    value = "";
                FullString.Reset(value,false);
                SetShowText();
            } }
        /// <summary>
        /// 选中文本
        /// </summary>
        public string SelectString { get {
                if (Editing)
                    return GetSelectString();
                return "";
            } }
        /// <summary>
        /// 文本显示载体
        /// </summary>
        public TextBox TextCom;
        /// <summary>
        /// 文本类型
        /// </summary>
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
        /// <summary>
        /// 文本输入事件
        /// </summary>
        public InputBoxEvent InputEvent;
        /// <summary>
        /// 文本提交事件
        /// </summary>
        public Action<InputBox> OnSubmit;
        /// <summary>
        /// 文本输入完毕事件
        /// </summary>
        public Action<InputBox> OnDone;
        /// <summary>
        /// 输入内容改变事件
        /// </summary>
        public Action<InputBox> OnValueChanged;
        /// <summary>
        /// 验证新输入的字符
        /// </summary>
        public Func<InputBox, int, char, char> ValidateChar;
        public override void Initial(FakeStruct mod, UIElement element,UIInitializer initializer)
        {
            base.Initial(mod,element,initializer);
            var txt = TextCom = element.GetComponentInChildren<TextBox>();
            textColor = txt.m_color;
            unsafe
            {
                if(mod!=null)
                {
                    var ex = UIElementLoader.GetEventData(mod);
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
                        m_InputString = mod.buffer.GetData(tp->inputString) as string;
                    }
                    else
                    {
                        m_InputString = txt.Text;
                    }
                }
                else
                {
                    m_InputString = txt.Text;
                }
            }
            FullString.Reset(m_InputString,false);
            InputEvent = txt.RegEvent<InputBoxEvent>();
            InputEvent.Initial(null);
            InputEvent.input = this;
            Caret = txt.GetComponentInChildren<HImage>();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            TextCom.HorizontalOverflow = HorizontalWrapMode.Overflow;
#else
            TextCom.HorizontalOverflow = HorizontalWrapMode.Wrap;
#endif
            TextCom.VerticalOverflow = VerticalWrapMode.Overflow;
            SetShowText();
        }
        /// <summary>
        /// 鼠标按压
        /// </summary>
        /// <param name="action">用户事件</param>
        /// <param name="press">按压基本信息</param>
        public void OnMouseDown(UserAction action)
        {
            if (InputString != "" & InputString != null)
            {
                TextCom.CheckPoint(InputEvent, action, ref startPress);
                endPress = startPress;
            }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (!ReadOnly)
                Editing = true;
            Input.imeCompositionMode = IMECompositionMode.On;
#endif
            SetShowText();
        }
        /// <summary>
        /// 用户单击
        /// </summary>
        /// <param name="action">用户事件</param>
        public void OnClick(UserAction action)
        {
            endPress = startPress;
            if (!ReadOnly)
            {
                Input.imeCompositionMode = IMECompositionMode.On;
                Editing = true;
                if (!Keyboard.active)
                {
                    bool pass = contentType == ContentType.Password ? true : false;
                    bool multiLine = lineType == LineType.MultiLineNewline ? true : false;
                    Keyboard.OnInput(FullString.FullString, touchType, multiLine, pass, CharacterLimit);
                }
            }
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="action">用户事件</param>
        public void OnLostFocus(UserAction action)
        {
            Editing = false;
            TextCom.Text = FullString.FullString;
            SetShowText();
            if (OnDone != null)
                OnDone(this);
            Input.imeCompositionMode = IMECompositionMode.Auto;
        }
        /// <summary>
        /// 拖动选择
        /// </summary>
        /// <param name="action">用户事件</param>
        /// <param name="press">当前按压信息</param>
        public void OnDrag(UserAction action)
        {
            TextCom.CheckPoint(InputEvent, action, ref endPress);
        }
        string OnInputChanged(string input)
        {
            if (ReadOnly)
                return "";
            if (input == null | input == "")
                return "";
            input = CharOperation.Validate(characterValidation,input,input.Length);
            if (input == null | input == "")
                return "";
            if (CharacterLimit > 0)
            {
                int c = CharacterLimit - FullString.Length;
                if (c <= 0)
                {
                    TextCom.stringEx.Reset(FullString.FullString, false);
                    TextCom.Populate();
                    TextCom.m_vertexChange = true;
                    return ""; 
                }
                int[] o = StringInfo.ParseCombiningCharacters(input);
                if (o.Length > c)
                {
                    input = new StringInfo(input).SubstringByTextElements(0, o[c - 1]);
                }
            }
            
           
            DeleteSelectString();
            int len = FullString.Length;
            int index = TextCom.GetIndex(ref startPress);
            FullString.InsertTextElements(index, input);
            SetShowText();
            len = FullString.Length - len;
            TextCom.Populate();
            TextCom.MoveRight(ref startPress, len);
            endPress = startPress;
            return input;
        }
        void TouchInputChanged(string input)
        {
            if (Keyboard.InputChanged)
            {
                FullString.Reset(input, false);
                var sel = Keyboard.selection;
                int start = sel.start;
                int end = sel.start + sel.length;
                if (end != start)
                {
                    TextCom.GetPress(start,ref startPress);
                    TextCom.GetPress(end, ref endPress);
                }
                else 
                {
                    TextCom.GetPress(start, ref startPress);
                    endPress = startPress;
                }
                SetShowText();
                TextCom.Populate();
            }
            if (OnValueChanged != null)
                OnValueChanged(this);
        }
        string CompositionString;
        /// <summary>
        /// 应用当前可显示文本内容
        /// </summary>
        public void SetShowText()
        {
            var str = FullString.FullString;
            if (InputEvent.Focus)
            {
                if (TextCom == null)
                    return;
                CompositionString = Keyboard.CompositionString;
                TextCom.MainColor = textColor;
                if (contentType == ContentType.Password)
                {
                    int l = 0;
                    if (str != null & str != "")
                        l = str.Length;
                    if (CompositionString != null & CompositionString != "")
                        l += CompositionString.Length;
                    if (l > 0)
                        TextCom.m_text = new string(PasswordCharacter, l);
                    else TextCom.m_text = "";
                }
                else
                {
                    StringEx stringEx = TextCom.stringEx;
                    if (stringEx == null)
                        stringEx = TextCom.stringEx = new StringEx(str, false);
                    else stringEx.Reset(str, false);
                    if (CompositionString != null & CompositionString != "")
                    {
                        int index = TextCom.GetIndex(ref startPress);
                        stringEx.InsertTextElements(index, CompositionString);//str.Insert(ss, cs);
                    }
                    TextCom.m_text = stringEx.FullString;
                }
                TextCom.m_dirty = true;
            }
            else if (str != "" & str != null)
            {
                if (TextCom == null)
                    return;
                TextCom.MainColor = textColor;
                if (contentType == ContentType.Password)
                {
                    TextCom.Text = new string(PasswordCharacter, str.Length);
                }
                else
                {
                    TextCom.Text = str;
                }
            }
            else
            {
                TextCom.MainColor = m_tipColor;
                TextCom.Text = m_TipString;
            }
        }
        public void SelectAll()
        {
            startPress.Row = 0;
            startPress.Offset = 0;
            TextCom.GetEnd(ref endPress);
        }
        public string GetSelectString()
        {
            int si = TextCom.GetIndex(ref startPress);
            int ei = TextCom.GetIndex(ref endPress);
            if (si == ei)
            {
                return "";
            }
            else if (si > ei)
            {
                return FullString.SubstringByTextElements(ei, si-ei);
            }
            else
                return FullString.SubstringByTextElements(si, ei-si);
        }
        /// <summary>
        /// 删除选中文本内容
        /// </summary>
        /// <returns></returns>
        public bool DeleteSelectString()
        {
            int si = TextCom.GetIndex(ref startPress);
            int ei = TextCom.GetIndex(ref endPress);
            if (si > ei)
            {
                FullString.RmoveTextElements(ei, si-ei);
                startPress = endPress;
                SetShowText();
                TextCom.Populate();
                TextCom.Move(ref startPress);
                return true;
            }
            else if(si < ei)
            {
                FullString.RmoveTextElements(si, ei-si);
                endPress = startPress;
                SetShowText();
                TextCom.Populate();
                TextCom.Move(ref startPress);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除光标前面的字符
        /// </summary>
        /// <returns></returns>
        public bool DeleteLast()
        {
            if (DeleteSelectString())
                return true;
            int index = TextCom.GetIndex(ref startPress);
            if (index>0)
            {
                int row = startPress.Row;
                TextCom.MoveLeft(ref startPress);
                endPress = startPress;
                FullString.RmoveTextElements(index - 1, 1);
                SetShowText();
                TextCom.Populate();
                TextCom.Move(ref startPress);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除光标后面的字符
        /// </summary>
        /// <returns></returns>
        public bool DeleteNext()
        {
            if (DeleteSelectString())
                return true;
            int index = TextCom.GetIndex(ref startPress);
            if(index<0)
            {
                Debug.Log("error");
            }
            var lens = FullString.noEmojiInfo.lens;
            if (lens == null)
                return false;
            if (index <lens.Length)
            {
                FullString.RmoveTextElements(index, 1);
                SetShowText();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 光标向左移动
        /// </summary>
        public void PointerMoveLeft()
        {
            TextCom.MoveLeft(ref startPress);
            endPress = startPress;
        }
        /// <summary>
        /// 光标向右移动
        /// </summary>
        public void PointerMoveRight()
        {
            TextCom.MoveRight(ref startPress, 1);
            endPress = startPress;
        }
        /// <summary>
        /// 光标向上移动
        /// </summary>
        public void PointerMoveUp()
        {
            TextCom.MoveUp(ref startPress);
            endPress = startPress;
        }
        /// <summary>
        /// 光标向下移动
        /// </summary>
        public void PointerMoveDown()
        {
            TextCom.MoveDown(ref startPress);
            endPress = startPress;
        }
        /// <summary>
        /// 光标移动到文本开头
        /// </summary>
        public void PointerMoveStart()
        {
            startPress.Row = 0;
            startPress.Offset = 0;
            endPress.Row = 0;
            endPress.Offset = 0;
            TextCom.Move(ref startPress);
        }
        /// <summary>
        /// 光标移动到文本结尾
        /// </summary>
        public void PointerMoveEnd()
        {
            TextCom.MoveEnd(ref startPress);
            endPress = startPress;
        }
        /// <summary>
        /// 更新显示内容
        /// </summary>
        /// <param name="time">时间片</param>
        public override void Update(float time)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (InputEvent.Focus)
            {
                var state = KeyPressed();
                if (state == EditState.Continue)
                {
                    if (Keyboard.InputChanged)
                    {
                        OnInputChanged(Keyboard.InputString);
                    }
                    else if (CompositionString!=Keyboard.CompositionString)
                    {
                        SetShowText();
                    }
                }
                else if (state == EditState.Finish)
                {
                    if (!ReadOnly)
                        if (OnSubmit != null)
                            OnSubmit(this);
                }
                else if (state == EditState.NewLine)
                {
                    if(!ReadOnly)
                    {
                        if (lineType == LineType.SingleLine)
                        {
                            if (OnSubmit != null)
                                OnSubmit(this);
                        }
                        else OnInputChanged("\n");
                    }
                }
#else
      if (Editing)
      {
                        TouchInputChanged(Keyboard.TouchString);
                        if (Keyboard.status == TouchScreenKeyboard.Status.Done)
                        {
                            if (OnDone!= null)
                                OnDone(this);
                            return;
                        }
#endif
                UpdateCaret();
            }
            else
            {
                if (Caret != null)
                    Caret.activeSelf = false;
            }
        
        }
        float time;
        /// <summary>
        /// 更新光标
        /// </summary>
        void UpdateCaret()
        {
            if (Caret != null)
            {
                hs.Clear();
                tris.Clear();
                if (startPress.Row == endPress.Row&startPress.Offset==endPress.Offset)
                {
                    if (!ReadOnly)
                    {
                        time += UserAction.TimeSlice;
                        if (time > 1000f)
                        {
                            time = 0;
                        }
                        else if (time > 400f)
                        {
                            Caret.activeSelf = false;
                        }
                        else
                        {
                            Caret.activeSelf = true;
                            PressInfo press = startPress;
                            string cs = Keyboard.CompositionString;
                            if (cs != null)
                                press.Offset += cs.Length;
                            TextCom.GetPointer(tris, hs, ref PointColor, ref press);
                            Caret.LoadFromMesh(hs, tris);
                            TextCom.SetCursorPos(ref endPress);
                        }
                    }
                    else Caret.activeSelf = false;
                }
                else
                {
                    Caret.activeSelf = true;
                    int si = TextCom.GetIndex(ref startPress);
                    int ei = TextCom.GetIndex(ref endPress);
                    if (si > ei)
                        TextCom.GetSelectArea(tris, hs, ref SelectionColor, ref endPress, ref startPress);
                    else TextCom.GetSelectArea(tris, hs, ref SelectionColor, ref startPress, ref endPress);
                    Caret.LoadFromMesh(hs, tris);
                }
            }
        }
        void KeySpeedUp()
        {
            KeySpeed *= 0.8f;
            if (KeySpeed < MaxSpeed)
                KeySpeed = MaxSpeed;
            KeyPressTime = KeySpeed;
        }
        EditState KeyPressed()
        {
            KeyPressTime -= UserAction.TimeSlice;
            if (Keyboard.GetKey(KeyCode.Backspace))
            {
                if (!ReadOnly)
                    if (KeyPressTime <= 0)
                    {
                        DeleteLast();
                        KeySpeedUp();
                    }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.Delete))
            {
                if (!ReadOnly)
                    if (KeyPressTime <= 0)
                    {
                        DeleteNext();
                        KeySpeedUp();
                    }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.LeftArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveLeft();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.RightArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveRight();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.UpArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveUp();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.DownArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveDown();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            KeySpeed = 220f;
            if (Keyboard.GetKeyDown(KeyCode.Home))
            {
                PointerMoveStart();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.End))
            {
                PointerMoveEnd();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.A))
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    TextCom.GetEnd(ref endPress);
                    startPress.Row = 0;
                    startPress.Offset = 0;
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.X))//剪切
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    GUIUtility.systemCopyBuffer = GetSelectString();
                    if (!ReadOnly)
                        DeleteSelectString();
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.C))//复制
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    GUIUtility.systemCopyBuffer = GetSelectString();
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.V))//粘贴
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    OnInputChanged(Keyboard.systemCopyBuffer);
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.Return) | Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (lineType == LineType.MultiLineNewline)
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
    }
}
