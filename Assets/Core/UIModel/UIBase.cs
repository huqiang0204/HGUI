using Assets.Core.HGUI;
using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBase
{
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
    public Transform Main { get; protected set; }
    public FakeStruct model { get; protected set; }
    protected UIBase UIParent;
    public T LoadUI<T>(string asset, string name) where T : class, new()
    {
        model = HGUIManager.FindModel(asset, name);
        T t = new T();
        Main = HGUIManager.GameBuffer.Clone(model, t).transform;
        Main.SetParent(Parent);
        Main.localPosition = Vector3.zero;
        Main.localScale = Vector3.one;
        return t;
    }
    public virtual void Initial(Transform parent, UIBase ui, object obj = null)
    {
        DataContext = obj;
        UIParent = ui;
        Parent = parent;
        if (parent != null)
            if (Main != null)
                Main.SetParent(parent);
    }
    public virtual void Dispose()
    {
        if (Main != null)
        {
            HGUIManager.GameBuffer.RecycleGameObject(Main.gameObject);
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
    public virtual void Cmd(string cmd, object dat)
    {
    }
    public virtual void Cmd(DataBuffer dat)
    {
    }
    public virtual void ReSize()
    {
        
    }
    public virtual void Update(float time)
    {
    }
    public virtual void ChangeLanguage()
    {

    }
}
