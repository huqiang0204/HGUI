using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Game.HotFix
{
    public class HotFix
    {
        static HotFix ins;
        ILRuntime.Runtime.Enviorment.AppDomain _app;
        public static HotFix Instance { get { if (ins == null) ins = new HotFix(); return ins; } }
        ILType mainScript;
        IMethod start;
        IMethod Update;
        IMethod Cmd;
        IMethod FullCmd;
        IMethod Resize;
        IMethod Dispose;
        byte[] dll;
        MemoryStream dllStream;
        MemoryStream pdbStream;
        private HotFix()
        {
            _app = new ILRuntime.Runtime.Enviorment.AppDomain();
            _app.DebugService.StartDebugService(7000);
            RegDelegate();

        }
        public void Load(byte[] dat, byte[] pdb = null)
        {
            if (dllStream != null)
                dllStream.Dispose();
            if (pdbStream != null)
                pdbStream.Dispose();
            dll = dat;
            dllStream = new MemoryStream(dat);
           if (pdb != null)
            {
                pdbStream = new MemoryStream(pdb);
                _app.LoadAssembly(dllStream, pdbStream, new ILRuntime.Mono.Cecil.Cil.PortablePdbReaderProvider());
            }
            else
                _app.LoadAssembly(dllStream);
            mainScript = _app.GetType("Main") as ILType;
            start = mainScript.GetMethod("Start");
            Update = mainScript.GetMethod("Update");
            Resize = mainScript.GetMethod("Resize");
            Cmd = mainScript.GetMethod("Cmd");
            FullCmd = mainScript.GetMethod("FullCmd");
            Dispose = mainScript.GetMethod("Dispose");
        }
        void RegDelegate()
        {
            _app.DelegateManager.RegisterMethodDelegate<object>();
            _app.DelegateManager.RegisterMethodDelegate<object, object, int>();
            _app.DelegateManager.RegisterMethodDelegate<object, object, float>();
            _app.DelegateManager.RegisterMethodDelegate<UIElement>();
            _app.DelegateManager.RegisterMethodDelegate<UserEvent, UserAction>();
            _app.DelegateManager.RegisterMethodDelegate<UserEvent, UserAction, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<TextInput>();
            _app.DelegateManager.RegisterMethodDelegate<TextInput, UserAction>();
            _app.DelegateManager.RegisterMethodDelegate<GestureEvent>();

            _app.DelegateManager.RegisterMethodDelegate<UIRocker>();
            _app.DelegateManager.RegisterMethodDelegate<UISlider>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollItem>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollX>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollX, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollY, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<DragContent>();
            _app.DelegateManager.RegisterMethodDelegate<DragContent, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<GridScroll>();
            _app.DelegateManager.RegisterMethodDelegate<GridScroll, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<DropdownEx, object>();
            _app.DelegateManager.RegisterMethodDelegate<ScrollYExtand, Vector2>();
            _app.DelegateManager.RegisterMethodDelegate<ObjectLinker, LinkerMod>();
            _app.DelegateManager.RegisterMethodDelegate<UIContainer>();
            _app.DelegateManager.RegisterMethodDelegate<OptionGroup, UserAction>();
            _app.DelegateManager.RegisterMethodDelegate<TreeView, TreeViewItem>();
            _app.DelegateManager.RegisterMethodDelegate<UIPalette>();

            _app.DelegateManager.RegisterMethodDelegate<AsyncOperation>();

            _app.DelegateManager.RegisterFunctionDelegate<object>();
            _app.DelegateManager.RegisterFunctionDelegate<TextInput, int, char, char>();

        }
        public void Start(Transform uiRoot, string Cmd, object data)
        {
            if (start != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, start.Name, mainScript, uiRoot, Cmd, data);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }

        }
        public void RuntimeUpdate(float time)
        {
            if (Update != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Update.Name, mainScript, time);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void RuntimeReSize()
        {
            if (Resize != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Resize.Name, mainScript);
                }
                catch (Exception ex)
                {
                   Debug.Log(ex.StackTrace);
                }
            }
        }
        public void RuntimeCmd(string cmd, string obj)
        {
            if (Cmd != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Cmd.Name, mainScript, cmd, obj);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void RuntimeFullCmd(DataBuffer data)
        {
            if (Cmd != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, FullCmd.Name, mainScript, data);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void RuntimeDispose()
        {
            if (Dispose != null)
            {
                try
                {
                    _app.Invoke(mainScript.FullName, Dispose.Name, mainScript);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.StackTrace);
                }
            }
        }
        public void Clear()
        {
           
        }
    }
}
