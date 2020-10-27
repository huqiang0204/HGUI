using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIModel
{
    /// <summary>
    /// UI页面,位于UI最下层
    /// </summary>
    public class UIPage : UIBase
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public static Transform Root { get;private set; }
        /// <summary>
        /// 根元素
        /// </summary>
        public static UIElement UIRoot;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="Canvas">主画布</param>
        public static void Initial(Transform Canvas)
        {
            var page = new GameObject("Page");
            UIRoot = page.AddComponent<UIElement>();
            Root = page.transform;
            page.transform.SetParent(Canvas);
            Root.localPosition = Vector3.zero;
            Root.localScale = Vector3.one;
            Root.localRotation = Quaternion.identity;
        }
        /// <summary>
        /// 当前页面
        /// </summary>
        public static UIPage CurrentPage { get; private set; }
        /// <summary>
        /// 载入页面
        /// </summary>
        /// <typeparam name="T">页面类型</typeparam>
        /// <param name="dat">数据</param>
        /// <returns></returns>
        public static T LoadPage<T>(object dat = null) where T : UIPage, new()
        {
            var p = CurrentPage as T;
            if (p!=null)
            {
                CurrentPage.ChangeLanguage();
                CurrentPage.Show(dat);
                return p;
            }
            if (HCanvas.MainCanvas != null)//释放当前页面所有事件
                HCanvas.MainCanvas.ClearAllAction();
            if (CurrentPage != null)
            {
                CurrentPage.Dispose();
            }
            var t = new T();
            CurrentPage = t;
            t.Initial(Root, dat);
            t.ChangeLanguage();
            t.ReSize();
            return t;
        }
        /// <summary>
        /// 载入页面
        /// </summary>
        /// <param name="type">页面类型</param>
        /// <param name="dat">数据</param>
        /// <returns></returns>
        public static UIPage LoadPage(Type type, object dat = null)
        {
            if (typeof(UIPage).IsAssignableFrom(type))
            {
                if (CurrentPage != null)
                    if (CurrentPage.GetType() == type)
                    {
                        CurrentPage.ChangeLanguage();
                        CurrentPage.Show(dat);
                        return CurrentPage;
                    }
                if (HCanvas.MainCanvas != null)//释放当前页面所有事件
                    HCanvas.MainCanvas.ClearAllAction();
                if (CurrentPage != null)
                    CurrentPage.Dispose();
                var t = Activator.CreateInstance(type) as UIPage;
                CurrentPage = t;
                t.Initial(Root, dat);
                t.ChangeLanguage();
                t.ReSize();
                return t;
            }
            return null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public UIPage()
        {
            pops = new List<PopWindow>();
        }
        /// <summary>
        /// 当前显示的窗口
        /// </summary>
        public PopWindow currentPop { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parent">父坐标变换</param>
        /// <param name="dat">数据</param>
        public virtual void Initial(Transform parent, object dat = null)
        {
            Parent = parent;
            DataContext = dat;
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="dat">数据</param>
        public virtual void Show(object dat = null)
        {
        }
        /// <summary>
        /// 更新尺寸
        /// </summary>
        public override void ReSize() 
        {
            if (UIRoot != null)
                if(HCanvas.MainCanvas!=null)
                UIRoot.SizeDelta = HCanvas.MainCanvas.SizeDelta;
            base.ReSize(); 
            if (currentPop != null) 
                currentPop.ReSize();
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        public override void Dispose()
        {
            if (pops != null)
                for (int i = 0; i < pops.Count; i++)
                    pops[i].Dispose();
            pops.Clear();
            currentPop = null;
            base.Dispose();
        }
        /// <summary>
        /// 隐藏当前窗口
        /// </summary>
        public void HidePopWindow()
        {
            if (currentPop != null)
            {
                currentPop.Hide();
            }
            currentPop = null;
        }
        List<PopWindow> pops;
        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="obj">数据</param>
        /// <param name="parent">父坐标变换,为空则默认为当前页的父坐标变换</param>
        /// <returns></returns>
        protected T ShowPopWindow<T>(object obj = null, Transform parent = null) where T : PopWindow, new()
        {
            if (currentPop != null)
            {
                currentPop.Hide();
                currentPop = null;
            }
            for (int i = 0; i < pops.Count; i++)
                if (pops[i] is T)
                {
                    currentPop = pops[i];
                    pops[i].Show(obj);
                    if (pops[i].CurLan != LanName)
                        pops[i].ChangeLanguage();
                    return pops[i] as T;
                }
            var t = new T();
            pops.Add(t);
            currentPop = t;
            if (parent == null)
                t.Initial(Parent, this, obj);
            else t.Initial(parent, this, obj);
            t.ChangeLanguage();
            t.ReSize();
            return t;
        }
        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="obj">数据</param>
        /// <param name="parent">父坐标变换,为空则默认为当前页的父坐标变换</param>
        /// <returns></returns>
        protected object ShowPopWindow(Type type, object obj = null, Transform parent = null)
        {
            if (currentPop != null)
            { 
                currentPop.Hide(); 
                currentPop = null; 
            }
            for (int i = 0; i < pops.Count; i++)
                if (pops[i].GetType() == type)
                {
                    currentPop = pops[i];
                    pops[i].Show(obj);
                    if (pops[i].CurLan != LanName)
                        pops[i].ChangeLanguage();
                    return pops[i];
                }
            var t = Activator.CreateInstance(type) as PopWindow;
            pops.Add(t);
            currentPop = t;
            if (parent == null)
                t.Initial(Parent, this, obj);
            else t.Initial(parent, this, obj);
            t.ChangeLanguage();
            t.ReSize();
            return t;
        }
        /// <summary>
        /// 弹出一个窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="obj">数据</param>
        /// <returns></returns>
        public virtual T PopUpWindow<T>(object obj = null) where T : PopWindow, new()
        {
            return ShowPopWindow<T>(obj, null);
        }
        object PopUpWindow(Type type, object obj = null)
        {
            var pop = ShowPopWindow(type, obj, null) as PopWindow;
            return pop;
        }
        /// <summary>
        /// 释放掉当前未激活的弹窗
        /// </summary>
        public void ReleasePopWindow()
        {
            if (pops != null)
                for (int i = 0; i < pops.Count; i++)
                    if (pops[i] != currentPop)
                        pops[i].Dispose();
            pops.Clear();
            if (currentPop != null)
                pops.Add(currentPop);
        }
        /// <summary>
        /// 释放当前窗口
        /// </summary>
        /// <param name="window">窗口实例</param>
        public void ReleasePopWindow(PopWindow window)
        {
            pops.Remove(window);
            if (currentPop == window)
            {
                currentPop = null;
            }
            window.Dispose();
        }
        /// <summary>
        /// 移除窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        public void ReleasePopWindow<T>()
        {
            for (int i = 0; i < pops.Count; i++)
                if (pops[i] is T)
                {
                    pops[i].Dispose();
                    pops.RemoveAt(i);
                    break;
                }
            if (currentPop is T)
            {
                currentPop = null;
            }
        }
        /// <summary>
        /// 获取一个窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns></returns>
        public T GetPopWindow<T>() where T : PopWindow
        {
            for (int i = 0; i < pops.Count; i++)
                if (pops[i] is T)
                {
                    return pops[i] as T;
                }
            return null;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="time"></param>
        public override void Update(float time)
        {
            if (currentPop != null)
                currentPop.Update(time);
        }
        /// <summary>
        /// 更换语言
        /// </summary>
        /// <returns></returns>
        public override bool ChangeLanguage()
        {
            if (currentPop != null)
                currentPop.ChangeLanguage();
            return base.ChangeLanguage();
        }
    }
}