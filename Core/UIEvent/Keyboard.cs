using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIEvent
{
    public enum EditState
    {
        Done,
        Continue,
        NewLine,
        Finish
    }
    public class Keyboard
    {
        public enum TouchOpertaion
        {
            None,
            Insert,
            Delete
        }
        static KeyCode[] keys;
        public static List<KeyCode> KeyPress;
        public static List<KeyCode> KeyDowns;
        public static List<KeyCode> KeyUps;
        public static string InputString;
        public static string TempString;
        public static bool TempStringChanged;
        public static string CorrectionInput;
        public static string TouchString = "";
        public static string CorrectionTouch;
        public static bool InputChanged;
        static TouchScreenKeyboard m_touch;
        public static bool _touch = false;
        public static IMECompositionMode iME;
        public static string systemCopyBuffer;
        public static void InfoCollection()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            IME.Update();
            TempStringChanged = IME.CompStringChanged;
#endif     
            systemCopyBuffer = GUIUtility.systemCopyBuffer;
            if (keys == null)
            {
                if (Application.platform == RuntimePlatform.Android |
                Application.platform == RuntimePlatform.IPhonePlayer |
                Application.platform == RuntimePlatform.WSAPlayerARM |
                Application.platform == RuntimePlatform.WSAPlayerX64 |
                Application.platform == RuntimePlatform.WSAPlayerX86)
                    _touch = true;
                keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
                KeyPress = new List<KeyCode>();
                KeyUps = new List<KeyCode>();
                KeyDowns = new List<KeyCode>();
                Input.imeCompositionMode = IMECompositionMode.On;
            }
            KeyPress.Clear();
            KeyUps.Clear();
            KeyDowns.Clear();
            for(int i=0;i<keys.Length;i++)
            {
                var key = keys[i];
                if (Input.GetKey(key))
                    KeyDowns.Add(key);
                if (Input.GetKeyDown(key))
                    KeyPress.Add(key);
                if (Input.GetKeyUp(key))
                    KeyUps.Add(key);
            }
            if(_touch)
            {
                if(m_touch!=null)
                {
                    if (m_touch.active)
                    {
#if !UNITY_STANDALONE_WIN
                        targetDisplay = m_touch.targetDisplay;
                        type = m_touch.type;
#endif
                        selection = m_touch.selection;
                        string str = m_touch.text;
                        InputChanged = true;
                        TouchString = str;
                        canGetSelection = m_touch.canGetSelection;
                        status = m_touch.status;
                    }
                    else status = TouchScreenKeyboard.Status.LostFocus;
                    active = m_touch.active;
                }
            }
            else
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (IME.Inputing)
                {
                    TempString = IME.CompString;
                }
                else if(IME.InputDone)
                {
                    TempString = "";
                    InputString = IME.ResultString;
                    InputChanged = true;
                }
                else
                {
                    if (InputString != Input.inputString)
                        InputChanged = true;
                    else InputChanged = false;
                    InputString = Input.inputString;
                }
#else
                if (InputString != Input.inputString)
                    InputChanged = true;
                else InputChanged = false;
                InputString = Input.inputString;
#endif

            }
            iME = Input.imeCompositionMode;
        }
        public static void OnInput(string str, TouchScreenKeyboardType type,bool multiLine,bool passward,int limit)
        {
            if(_touch)
            {
                m_touch = TouchScreenKeyboard.Open(str, type, true, multiLine, passward);
                m_touch.characterLimit = limit;
            }
        }
        public static void EndInput()
        {
            if(_touch)
            {
                if (m_touch != null)
                    m_touch.active = false;
            }
        }
        public static int targetDisplay { get; set; }
        public  static TouchScreenKeyboardType type { get; private set; }
        public static RangeInt selection { get; set; }
        public static bool canSetSelection { get; private set; }
        public static bool active { get; set; }
        public static bool canGetSelection { get; private set; }
        public static TouchScreenKeyboard.Status status { get; private set; }
        public static bool GetKey(KeyCode key)
        {
            if (KeyDowns.Contains(key))
                return true;
            return false;
        }
        public static bool GetKeyDown(KeyCode key)
        {
            if (KeyPress.Contains(key))
                return true;
            return false;
        }
        public static bool Nokey()
        {
            if (KeyPress.Count > 0)
                return false;
            if (KeyDowns.Count > 0)
                return false;
            if (KeyUps.Count > 0)
                return false;
            return true;
        }
        public static bool GetKeyUp(KeyCode key)
        {
            if (KeyUps.Contains(key))
                return true;
            return false;
        }

        /// <summary>
        /// 每秒5次
        /// </summary>
        static float KeySpeed = 220;
        static float MaxSpeed = 30;
        static float KeyPressTime;
        public static TextInput InputEvent { get;  set; }
        public static void Dispatch()
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
                            if (InputChanged)
                            {
                                InputEvent.OnInputChanged(InputString);
                            }else if(TempStringChanged)
                            {
                               InputEvent.SetShowText();
                            }
                        }
                        else if (state == EditState.Finish)
                        {
                            if (InputEvent.OnSubmit != null)
                                InputEvent.OnSubmit(InputEvent);
                        }
                        else if (state == EditState.NewLine)
                        {
                            if (InputEvent.lineType == LineType.SingleLine)
                            {
                                if (InputEvent.OnSubmit != null)
                                    InputEvent.OnSubmit(InputEvent);
                            }
                            else InputEvent.OnInputChanged("\r\n");
                        }
#else
                        InputEvent.TouchInputChanged(Keyboard.TouchString);
                        if (Keyboard.status == TouchScreenKeyboard.Status.Done)
                        {
                            if (InputEvent.OnSubmit != null)
                                InputEvent.OnSubmit(InputEvent);
                            InputEvent = null;
                            return;
                        }
#endif
                    }
                }
            }
        }
        static EditState KeyPressed()
        {
            KeyPressTime -= UserAction.TimeSlice;
            if (GetKey(KeyCode.Backspace))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.DeleteLast();
                    }
                    KeySpeed *= 0.8f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (GetKey(KeyCode.Delete))
            {
                if (KeyPressTime <= 0)
                {
                    if (InputEvent != null)
                    {
                        InputEvent.DeleteNext();
                    }
                    KeySpeed *= 0.7f;
                    if (KeySpeed < MaxSpeed)
                        KeySpeed = MaxSpeed;
                    KeyPressTime = KeySpeed;
                }
                return EditState.Done;
            }
            if (GetKey(KeyCode.LeftArrow))
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
            if (GetKey(KeyCode.RightArrow))
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
            if (GetKey(KeyCode.UpArrow))
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
            if (GetKey(KeyCode.DownArrow))
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
            if (GetKeyDown(KeyCode.Home))
            {
                InputEvent.PointerMoveStart();
                return EditState.Done;
            }
            if (GetKeyDown(KeyCode.End))
            {
                InputEvent.PointerMoveEnd();
                return EditState.Done;
            }
            if (GetKeyDown(KeyCode.A))
            {
                if (GetKey(KeyCode.LeftControl) | GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        TextOperation.SelectAll();
                    }
                    return EditState.Done;
                }
            }
            if (GetKeyDown(KeyCode.X))//剪切
            {
                if (GetKey(KeyCode.LeftControl) | GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        string str = TextOperation.GetSelectString();
                        InputEvent.DeleteSelectString();
                        GUIUtility.systemCopyBuffer = str;
                    }
                    return EditState.Done;
                }
            }
            if (GetKeyDown(KeyCode.C))//复制
            {
                if (GetKey(KeyCode.LeftControl) | GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        string str = TextOperation.GetSelectString();
                        GUIUtility.systemCopyBuffer = str;
                    }
                    return EditState.Done;
                }
            }
            if (GetKeyDown(KeyCode.V))//粘贴
            {
                if (GetKey(KeyCode.LeftControl) | GetKey(KeyCode.RightControl))
                {
                    if (InputEvent != null)
                    {
                        InputEvent.OnInputChanged(systemCopyBuffer);
                    }
                    return EditState.Done;
                }
            }
            if (GetKeyDown(KeyCode.Return) | GetKeyDown(KeyCode.KeypadEnter))
            {
                if (InputEvent.lineType == LineType.MultiLineNewline)
                {
                    if (GetKey(KeyCode.RightControl))
                        return EditState.Finish;
                    return EditState.NewLine;
                }
                else return EditState.Finish;
            }
            if (GetKeyDown(KeyCode.Escape))
            {
                return EditState.Finish;
            }
            return EditState.Continue;
        }
    }
}
