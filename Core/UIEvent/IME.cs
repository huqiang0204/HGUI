using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace huqiang
{
#if UNITY_STANDALONE_WIN ||UNITY_EDITOR
    public class IME
    {
        private const int WH_KEYBOARD_LL = 13; //全局键盘钩子
        private const int WM_KEYDOWN = 0x0100; //键盘按下
        private const int WM_KEYUP = 0x0101; //键盘抬起
        /// <summary>
        /// 安装钩子
        /// </summary>
        /// <param name="idHook">钩子类型</param>
        /// <param name="lpfn">函数指针</param>
        /// <param name="hMod">包含钩子函数的模块(EXE、DLL)句柄; 一般是 HInstance; 如果是当前线程这里可以是 0</param>
        /// <param name="dwThreadId">关联的线程; 可用 GetCurrentThreadId 获取当前线程; 0 表示是系统级钩子</param>
        /// <returns>返回钩子的句柄; 0 表示失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        //钩子类型 idHook 选项:
        //WH_MSGFILTER       = -1; {线程级; 截获用户与控件交互的消息}
        //WH_JOURNALRECORD   = 0;  {系统级; 记录所有消息队列从消息队列送出的输入消息, 在消息从队列中清除时发生; 可用于宏记录}
        //WH_JOURNALPLAYBACK = 1;  {系统级; 回放由 WH_JOURNALRECORD 记录的消息, 也就是将这些消息重新送入消息队列}
        //WH_KEYBOARD        = 2;  {系统级或线程级; 截获键盘消息}
        //WH_GETMESSAGE      = 3;  {系统级或线程级; 截获从消息队列送出的消息}
        //WH_CALLWNDPROC     = 4;  {系统级或线程级; 截获发送到目标窗口的消息, 在 SendMessage 调用时发生}
        //WH_CBT             = 5;  {系统级或线程级; 截获系统基本消息, 譬如: 窗口的创建、激活、关闭、最大最小化、移动等等}
        //WH_SYSMSGFILTER    = 6;  {系统级; 截获系统范围内用户与控件交互的消息}
        //WH_MOUSE           = 7;  {系统级或线程级; 截获鼠标消息}
        //WH_HARDWARE        = 8;  {系统级或线程级; 截获非标准硬件(非鼠标、键盘)的消息}
        //WH_DEBUG           = 9;  {系统级或线程级; 在其他钩子调用前调用, 用于调试钩子}
        //WH_SHELL           = 10; {系统级或线程级; 截获发向外壳应用程序的消息}
        //WH_FOREGROUNDIDLE  = 11; {系统级或线程级; 在程序前台线程空闲时调用}
        //WH_CALLWNDPROCRET  = 12; {系统级或线程级; 截获目标窗口处理完毕的消息, 在 SendMessage 调用后发生}
        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="hhk">钩子的句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        public static bool haveMainWindow = false;
        public  static IntPtr mainWindowHandle = IntPtr.Zero;
        private static IntPtr _hookID = IntPtr.Zero;
        public int processId = 0;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        public  IntPtr GetMainWindowHandle(int processId)
        {
            if (!haveMainWindow)
            {
                mainWindowHandle = IntPtr.Zero;
                this.processId = processId;
                EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(this.EnumWindowsCallback);
                EnumWindows(callback, IntPtr.Zero);
                GC.KeepAlive(callback);

                haveMainWindow = true;
            }
            return mainWindowHandle;
        }

        private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
        {
            int num;
            GetWindowThreadProcessId(new HandleRef(this, handle), out num);
            if ((num == this.processId) && this.IsMainWindow(handle))
            {
                mainWindowHandle = handle;
                return false;
            }
            return true;
        }

        private bool IsMainWindow(IntPtr handle)
        {
            return (!(GetWindow(new HandleRef(this, handle), 4) != IntPtr.Zero) && IsWindowVisible(new HandleRef(this, handle)));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HandleRef hWnd);

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        public static extern int ImmSetCompositionStringW(IntPtr himc, int dwIndex, IntPtr lpComp, int dw, int lpRead, int dw2);

        public const int WM_IME_SETCONTEXT = 0x0281;
        public const int WM_IME_CHAR = 0x0286;
        public const int WM_CHAR = 0x0102;
        public const int WM_IME_COMPOSITION = 0x010F;
        const int GCS_COMPATTR = 0x10;
        const int GCS_COMPCLAUSE = 0x20;
        const int GCS_COMPREADATTR= 0x2;
        const int GCS_COMPREADCLAUSE = 0x4;
        const int GCS_COMPREADSTR = 0x1;
        const int GCS_COMPSTR = 0x8;
        const int GCS_CURSORPOS = 0x80;
        const int GCS_DELTASTART = 0x100;
        const int GCS_RESULTCLAUSE = 0x1000;
        const int GCS_RESULTREADCLAUSE = 0x400;
        const int GCS_RESULTREADSTR = 0x200;
        const int GCS_RESULTSTR = 0x0800;
        public const int SCS_SETRECONVERTSTRING = 0x00010000;
        public const int SCS_QUERYRECONVERTSTRING = 0x00020000;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECONVERTSTRING
        {
            public uint dwSize;
            public uint dwVersion;
            public uint dwStrLen;
            public uint dwStrOffset;
            public uint dwCompStrLen;
            public uint dwCompStrOffset;
            public uint dwTargetStrLen;
            public uint dwTargetStrOffset;
        }
        public static string CompString;
        public static string LastCompString;
        public static string ResultString;
        public static string LastResultString;
        public static bool Inputing;
        public static bool CompStringChanged;
        public static bool InputDone;
        static IntPtr hIMC= IntPtr.Zero;
        public static void Update()
        {
            CompStringChanged = false;
            ResultString = "";
            InputDone = false;
            int strLen = ImmGetCompositionStringW(hIMC, GCS_COMPSTR, null, 0);
            if (strLen > 0)
            {
                byte[] buffer = new byte[strLen];
                ImmGetCompositionStringW(hIMC, GCS_COMPSTR, buffer, strLen);
                CompString = Encoding.Unicode.GetString(buffer, 0, strLen);
                Inputing = true;
                if(LastCompString!=CompString)
                {
                    CompStringChanged = true;
                    LastCompString = CompString;
                }
                return;
            }
            else 
            {
                LastCompString = CompString = ""; 
            }
            if (Inputing)
            {
                strLen = ImmGetCompositionStringW(hIMC, GCS_RESULTSTR, null, 0);
                if (strLen > 0)
                {
                    byte[] buffer = new byte[strLen];
                    ImmGetCompositionStringW(hIMC, GCS_RESULTSTR, buffer, strLen);
                    string str = Encoding.Unicode.GetString(buffer, 0, strLen);
                    str = str.Replace("\b","");
                    LastResultString = ResultString = str;
                }
                else 
                { 
                    LastResultString = ResultString = ""; 
                }
                Inputing = false;
                InputDone = true;
                return;
            }
        }
        static void SetIMEString(string str)
        {
            unsafe
            {
                uint len = 0;
                RECONVERTSTRING* reconv = (RECONVERTSTRING*)Marshal.AllocHGlobal(
                  sizeof(RECONVERTSTRING) + Encoding.Unicode.GetByteCount(str) + 1);

                char* paragraph = (char*)((byte*)reconv + sizeof(RECONVERTSTRING));

                reconv->dwSize
                  = (uint)sizeof(RECONVERTSTRING) + (uint)Encoding.Unicode.GetByteCount(str) + 1;
                reconv->dwVersion = 0;
                reconv->dwStrLen = (uint)str.Length;
                reconv->dwStrOffset = (uint)sizeof(RECONVERTSTRING);

                reconv->dwCompStrLen = 0;
                reconv->dwCompStrOffset = len * sizeof(char);

                reconv->dwTargetStrLen = 0;
                reconv->dwTargetStrOffset = len * sizeof(char);

                for (int i = 0; i < str.Length; i++)
                {
                    paragraph[i] = str[i];
                }
                ImmSetCompositionStringW(hIMC, SCS_SETRECONVERTSTRING, (IntPtr)reconv,
      sizeof(RECONVERTSTRING) + Encoding.Unicode.GetByteCount(str) + 1, 0, 0);
            }
           
        }
        /// <summary>
        /// 处理函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns>
        /// 如果返回1，则结束消息，这个消息到此为止，不再传递；
        /// 如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者；
        /// </returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //键盘按下时
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode > 64 & vkCode < 91)
                    vkCode += 32;
                KeyCode key = (KeyCode)vkCode;
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    UnityEngine.Debug.Log(key);
                }
                else if(wParam==(IntPtr)WM_KEYUP)
                {
                    UnityEngine.Debug.Log(key);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        public static void Initial()
        {
             var pro = Process.GetCurrentProcess();
             mainWindowHandle = new IME().GetMainWindowHandle(pro.Id);
             hIMC = ImmGetContext(mainWindowHandle);
            //using (ProcessModule curModule = pro.MainModule)
            //{
            //    _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, GetModuleHandle(curModule.ModuleName), 0);
            //}
        }
        public static void Dispose()
        {
            ImmReleaseContext(mainWindowHandle, hIMC);
            //UnhookWindowsHookEx(_hookID);
        }
    }
#endif
}
