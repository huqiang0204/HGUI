using System;
using System.Linq;
using System.Text;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;

namespace huqiang.UIModel
{
    public class HotConstructor<T, U> where T : class, new()
    {
        public HotMiddleware middle;
        public HotConstructor(Action<T, U, int> action)
        {
            middle = new HotMiddleware();
            middle.Context = this;
            middle.initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
            middle.creator = Create;
            middle.caller = Call;
            Invoke = action;
        }
        U u;
        public Action<T, U, int> Invoke;
        public object Create()
        {
            var t = new T();
            middle.initializer.Reset(t);
            return t;
        }
        public void Call(object obj, object dat, int index)
        {
            if (Invoke != null)
            {
                try
                {
                    u = (U)dat;
                }
                catch
                {
                }
                Invoke(obj as T, u, index);
            }
        }
    }
}
