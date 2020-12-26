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
        /// <summary>
        /// 当前帧所有按下键的列表
        /// </summary>
        public static List<KeyCode> KeyPress;
        /// <summary>
        /// 当前帧所有按下状态的键的列表
        /// </summary>
        public static List<KeyCode> KeyDowns;
        /// <summary>
        /// 当前帧所有弹起键的列表
        /// </summary>
        public static List<KeyCode> KeyUps;
        /// <summary>
        /// 输入字符串
        /// </summary>
        public static string InputString;
        /// <summary>
        /// IME临时字符串
        /// </summary>
        public static string TempString;
        /// <summary>
        /// Unity获取到的IME字符串
        /// </summary>
        public static string CompositionString;
        /// <summary>
        /// IME临时字符串当前帧改变了
        /// </summary>
        public static bool TempStringChanged;
        /// <summary>
        /// 受输入规则限定后的字符串
        /// </summary>
        public static string CorrectionInput;
        /// <summary>
        /// 移动端输入字符串
        /// </summary>
        public static string TouchString = "";
        /// <summary>
        /// 移动端受输入规则限定后的字符串
        /// </summary>
        public static string CorrectionTouch;
        /// <summary>
        /// 输入字符串在当前帧改变了
        /// </summary>
        public static bool InputChanged;
        static TouchScreenKeyboard m_touch;
        /// <summary>
        /// 软键盘
        /// </summary>
        public static bool _touch = false;
        /// <summary>
        /// 系统粘贴板
        /// </summary>
        public static string systemCopyBuffer;
        /// <summary>
        /// 输入法位置
        /// </summary>
        public static Vector2 CursorPos;
        /// <summary>
        /// 键盘信息收集
        /// </summary>
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
                Application.platform == RuntimePlatform.IPhonePlayer 
                | Application.platform == RuntimePlatform.WSAPlayerARM 
                | Application.platform == RuntimePlatform.WSAPlayerX64 
                | Application.platform == RuntimePlatform.WSAPlayerX86
                )
                    _touch = true;
                keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
                KeyPress = new List<KeyCode>();
                KeyUps = new List<KeyCode>();
                KeyDowns = new List<KeyCode>();
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
                CompositionString = Input.compositionString;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (IME.Inputing)
                {
                    TempString = IME.CompString;
                    Input.compositionCursorPos = CursorPos;
                }
                else if(IME.InputDone)
                {
                    TempString = "";
                    InputString = IME.ResultString;
                    InputChanged = true;
                    CompositionString = "";
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
        }
        /// <summary>
        /// 弹出软键盘输入
        /// </summary>
        /// <param name="str">初始字符串</param>
        /// <param name="type">键盘类型</param>
        /// <param name="multiLine">允许多行</param>
        /// <param name="passward">密码输入</param>
        /// <param name="limit">字符个数限制,为0则不限制</param>
        public static void OnInput(string str, TouchScreenKeyboardType type,bool multiLine,bool passward,int limit)
        {
            if(_touch)
            {
                m_touch = TouchScreenKeyboard.Open(str, type, true, multiLine, passward);
                m_touch.characterLimit = limit;
            }
        }
        /// <summary>
        /// 结束输入
        /// </summary>
        public static void EndInput()
        {
            if(_touch)
            {
                if (m_touch != null)
                    m_touch.active = false;
            }
        }
        public static int targetDisplay { get; set; }
        /// <summary>
        /// 触摸键盘类型
        /// </summary>
        public  static TouchScreenKeyboardType type { get; private set; }
        public static RangeInt selection { get; set; }
        public static bool canSetSelection { get; private set; }
        public static bool active { get; set; }
        public static bool canGetSelection { get; private set; }
        /// <summary>
        /// 触摸键盘状态
        /// </summary>
        public static TouchScreenKeyboard.Status status { get; private set; }
        /// <summary>
        /// 获取键的当前帧按压状态
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool GetKey(KeyCode key)
        {
            if (KeyDowns.Contains(key))
                return true;
            return false;
        }
        /// <summary>
        /// 获取键的当前帧是否按下
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool GetKeyDown(KeyCode key)
        {
            if (KeyPress.Contains(key))
                return true;
            return false;
        }
        /// <summary>
        /// 当前帧是否没有键输入
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 获取键的当前帧是否弹起
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool GetKeyUp(KeyCode key)
        {
            if (KeyUps.Contains(key))
                return true;
            return false;
        }
    }
}
