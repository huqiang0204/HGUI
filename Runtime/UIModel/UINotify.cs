﻿using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIModel
{
    /// <summary>
    /// 通知UI,位于所有UI的顶层
    /// </summary>
    public class UINotify : UIBase
    {
        /// <summary>
        /// 根元素
        /// </summary>
        public static UIElement UIRoot { get; private set; }
        /// <summary>
        /// 当前通知栏
        /// </summary>
        public static UINotify Instance { get; private set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="Canvas">主画布</param>
        public static void Initial(UIElement Canvas)
        {
            var menu = new UIElement();
            menu.name = "Notify";
            UIRoot = menu;
            //menu.marginType = MarginType.Margin;
            menu.SetParent(Canvas);
            menu.localPosition = Vector3.zero;
            menu.localScale = Vector3.one;
            menu.localRotation = Quaternion.identity;
        }
        /// <summary>
        /// 更新所有通知栏
        /// </summary>
        /// <param name="time">时间片</param>
        public static void UpdateAll(float time)
        {
            for (int i = 0; i < notifys.Count; i++)
            {
                if (notifys[i].Main != null)
                {
                    if (notifys[i].Main.activeSelf)
                        notifys[i].Update(time);
                }
            }
        }
        static List<UINotify> notifys = new List<UINotify>();
        /// <summary>
        /// 当前通知栏
        /// </summary>
        public static UINotify CurrentNotify { get; private set; }
        /// <summary>
        /// 显示某个通知栏
        /// </summary>
        /// <typeparam name="T">UI类型</typeparam>
        /// <param name="context">联系上下文</param>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static T ShowNotify<T>(UIBase context, object obj = null) where T : UINotify, new()
        {
            UIRoot.activeSelf = true;
            for (int i = 0; i < notifys.Count; i++)
                if (notifys[i] is T)
                {
                    CurrentNotify = notifys[i];
                    CurrentNotify.ChangeLanguage();
                    notifys[i].Show(context, obj);
                    return notifys[i] as T;
                }
            var t = new T();
            notifys.Add(t);
            CurrentNotify = t;
            t.Initial(UIRoot, context, obj);
            t.ChangeLanguage();
            t.Show(context, obj);
            t.ReSize();
            return t;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public UINotify()
        {
            Instance = this;
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="context">联系上下文</param>
        /// <param name="dat">数据对象</param>
        public virtual void Show(UIBase context, object dat = null)
        {
            if (Main != null)
                Main.activeSelf = true;
        }
        /// <summary>
        /// 更新尺寸
        /// </summary>
        public override void ReSize()
        {
            if (UIRoot != null)
                if (HCanvas.CurrentCanvas != null)
                    UIRoot.SizeDelta = HCanvas.CurrentCanvas.SizeDelta;
            base.ReSize();
        }
        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide()
        {
            if (Main != null)
                Main.activeSelf = false;
        }
    }
}

