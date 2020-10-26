using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace huqiang
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenDialogDir
    {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = null;
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class FileDlg
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }
    /// <summary>
    /// 文件选取器,使用win32 api,适用于win平台
    /// </summary>
    public class OpenFileDialog
    {
        FileDlg pth;
        public OpenFileDialog()
        {
            pth = new FileDlg();
            pth.structSize = Marshal.SizeOf(pth);
            pth.filter = "All |*.*;\0\0";
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = Environment.CurrentDirectory;  // default path  
            pth.title = "打开项目";
            pth.defExt = "txt";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        }
        public string filter { get => pth.filter; set => pth.filter = value + "\0\0"; }
        public string title { get => pth.title; set => pth.title = value; }
        public string detExt { get => pth.defExt; set => pth.defExt = value; }
        public string dirPath { get => pth.initialDir; set => pth.initialDir = value; }
        public string file { get => pth.file; }
        public bool Open()
        {
            return WinAPI.GetOpenFileName(pth);
        }
    }
    /// <summary>
    /// 文件存储选取器,使用win32 api,适用于win平台
    /// </summary>
    public class SaveFileDialog
    {
        FileDlg pth;
        public SaveFileDialog()
        {
            pth = new FileDlg();
            pth.structSize = Marshal.SizeOf(pth);
            pth.filter = "All |*.*;\0\0";
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = Environment.CurrentDirectory;  // default path  
            pth.title = "保存项目";
            pth.defExt = ".dat";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        }
        public string filter { get => pth.filter; set => pth.filter = value  +"\0\0"; }
        public string title { get => pth.title; set => pth.title = value; }
        public string detExt { get => pth.defExt; set => pth.defExt = value; }
        public string dirPath { get => pth.initialDir; set => pth.initialDir = value; }
        public string file { get => pth.file; }
        public bool Open()
        {
            return WinAPI.GetSaveFileName(pth);
        }
    }
}
