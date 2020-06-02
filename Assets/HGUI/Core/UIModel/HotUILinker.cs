using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIComposite;
using System;

namespace huqiang.UIModel
{
    public class HotUILinker<T, U> where T : class, new() where U : class, new()
    {
        public ObjectLinker middle;
        public Action<T, U, int> ItemUpdate;
        public Func<U, float> CalculItemHigh;
        UIInitializer initializer;
        FakeStruct model;
        UIContainer con;
        public HotUILinker(UIContainer container, string mod)
        {
            con = container;
            model = HGUIManager.FindChild(container.model, mod);
            middle = new ObjectLinker(container);
            middle.ItemUpdate = MItemUpdate;
            middle.CalculItemHigh = MCalculItemHigh;
            middle.ItemCreate = MItemCreate;
            initializer = new UIInitializer(TempReflection.ObjectFields(typeof(T)));
        }
        void MItemUpdate(object obj, object dat, int index)
        {
            if (ItemUpdate != null)
                ItemUpdate(obj as T, dat as U, index);
        }
        float MCalculItemHigh(object dat)
        {
            if (CalculItemHigh != null)
                return CalculItemHigh(dat as U);
            return 0;
        }
        void MItemCreate(ObjectLinker obj, LinkerMod mod)
        {
            T t = new T();
            initializer.Reset(t);
            mod.main = HGUIManager.GameBuffer.Clone(model, initializer);
            mod.UI = t;
        }
        public void InsertData(U dat)
        {
            con.InsertData(middle, dat);
        }
        public void AddData(U dat)
        {
            con.AddData(middle, dat);
        }
        public void AddAndMove(U dat, float h)
        {
            if (h <= 0)
                h = 60;
            con.AddAndMove(middle, dat, h);
        }
        public void AddAndMoveEnd(U dat, float h)
        {
            if (h <= 0)
                h = 60;
            con.AddAndMoveEnd(middle, dat, h);
        }
    }
}

