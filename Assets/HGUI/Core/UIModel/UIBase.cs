using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
#if Hot
namespace huqiang.HotUIModel
#else
namespace huqiang.UIModel
#endif
{
    public class UIBase
    {
        public static INIReader Lan = new INIReader();
        static int point;
        static UIBase[] buff = new UIBase[1024];
        public static T GetUI<T>() where T : UIBase
        {
            for (int i = 0; i < point; i++)
                if (buff[i] is T)
                    return buff[i] as T;
            return null;
        }
        public static List<T> GetUIs<T>() where T : UIBase
        {
            List<T> tmp = new List<T>();
            for (int i = 0; i < point; i++)
                if (buff[i] is T)
                    tmp.Add(buff[i] as T);
            return tmp;
        }
        public static void ClearUI()
        {
            for (int i = 0; i < point; i++)
                buff[i] = null;
            point = 0;
        }
        int Index;
        public UIBase()
        {
            Index = point;
            buff[point] = this;
            point++;
        }
        public object DataContext;
        public Transform Parent { get; protected set; }
        public GameObject Main { get; protected set; }
        public FakeStruct model { get; protected set; }
        protected UIBase UIParent;
        public static TempReflection ObjectFields(Type type)
        {
            var fs = type.GetFields();
            TempReflection temp = new TempReflection();
            temp.Top = fs.Length;
            ReflectionModel[] reflections = new ReflectionModel[temp.Top];
            for (int i = 0; i < fs.Length; i++)
            {
                ReflectionModel r = new ReflectionModel();
                r.field = fs[i];
                r.FieldType = fs[i].FieldType;
                r.name = fs[i].Name;
                reflections[i] = r;
            }
            temp.All = reflections;
            return temp;
        }
        string uiName;
        protected UIInitializer uiInitializer;
        public T LoadUI<T>(string asset, string name) where T : class, new()
        {
            uiName = name;
            model = HGUIManager.FindModel(asset, name);
            T t = new T();
            uiInitializer = new UIInitializer(ObjectFields(typeof(T)));
            uiInitializer.Reset(t);
            Main = HGUIManager.GameBuffer.Clone(model, uiInitializer);
            var trans = Main.transform;
            trans.SetParent(Parent);
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            return t;
        }
        public virtual void Initial(Transform parent, UIBase ui, object obj = null)
        {
            DataContext = obj;
            UIParent = ui;
            Parent = parent;
            if (parent != null)
                if (Main != null)
                    Main.transform.SetParent(parent);
        }
        public virtual void Dispose()
        {
            if (Main != null)
            {
                HGUIManager.GameBuffer.RecycleGameObject(Main);
            }
            point--;
            if (buff[point] != null)
                buff[point].Index = Index;
            buff[Index] = buff[point];
            buff[point] = null;
        }
        public virtual void Save()
        {
        }
        public virtual void Recovery()
        {
        }
        public virtual object CollectionData()
        {
            return null;
        }
        public virtual void Cmd(Msg msg, object dat)
        {
        }
        public virtual void Cmd(int cmd, object dat)
        {
        }
        public virtual void Cmd(DataBuffer dat)
        {
        }
        public virtual void ReSize()
        {
            if (Main != null)
            {
                var ele = Main.GetComponent<UIElement>();
                if (ele != null)
                    UIElement.Resize(ele);
            }
        }
        public virtual void Update(float time)
        {
        }
        public virtual void ChangeLanguage()
        {
            if (Lan == null)
                return;
            if (uiName != null)
            {
                var sec = Lan.FindSection(uiName);
                if (sec != null)
                {
                    if (uiInitializer != null)
                    {
                        uiInitializer.ChangeLanguage(sec);
                    }
                }
            }
        }
    }
}

