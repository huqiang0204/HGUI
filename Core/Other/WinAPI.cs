using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang
{
    public class WinAPI
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] FileDlg ofd);
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] FileDlg ofd);
  
        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);
        public static string GetPathFromWindowsExplorer(string dialogtitle = "Select file path strength.")
        {
            try
            {
                OpenDialogDir ofn2 = new OpenDialogDir();
                ofn2.pszDisplayName = new string(new char[2048]);
                // 存放目录路径缓冲区  
                ofn2.lpszTitle = dialogtitle; // 标题  
                ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框  
                IntPtr pidlPtr = SHBrowseForFolder(ofn2);

                char[] charArray = new char[2048];

                for (int i = 0; i < 2048; i++)
                {
                    charArray[i] = '\0';
                }

                SHGetPathFromIDList(pidlPtr, charArray);
                string res = new string(charArray);
                res = res.Substring(0, res.IndexOf('\0'));
                return res;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return string.Empty;
        }

#else
       public static bool GetOpenFileName([In, Out] FileDlg ofd) { return false; }
        public static bool GetSaveFileName([In, Out] FileDlg ofd) { return false; }
#endif
    }
}
