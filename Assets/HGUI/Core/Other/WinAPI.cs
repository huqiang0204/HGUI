using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace huqiang
{
    public class WinAPI
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] FileDlg ofd);
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] FileDlg ofd);
#else
       public static bool GetOpenFileName([In, Out] FileDlg ofd) { return false; }
        public static bool GetSaveFileName([In, Out] FileDlg ofd) { return false; }
#endif
    }
}
