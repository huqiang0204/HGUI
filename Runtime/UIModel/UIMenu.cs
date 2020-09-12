using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

#if Hot
namespace huqiang.HotUIModel
#else
namespace huqiang.UIModel
#endif
{
    public class UIMenu : UIBase
    {
        public const int LeftTop = 0;
        public const int Top = 1;
        public const int RightTop = 2;
        public const int Right = 3;
        public const int RightDown = 4;
        public const int Down = 5;
        public const int LeftDown = 6;
        public const int Left = 7;
        public static Transform Root { get;private set; }
        public static UIElement UIRoot { get;private set; }
        public static UIMenu Instance { get; set; }
        public static void Initial(Transform Canvas)
        {
            var menu = new GameObject("Menu");
            var ele = UIRoot = menu.AddComponent<UIElement>();
            ele.marginType = MarginType.Margin;
            var act = ele.RegEvent<UserEvent>();
            act.PointerDown = (o, e) => { o.Context.gameObject.SetActive(false); };
            act.Penetrate = true;
            Root = menu.transform;
            menu.transform.SetParent(Canvas);
            Root.localPosition = Vector3.zero;
            Root.localScale = Vector3.one;
            Root.localRotation = Quaternion.identity;
            menu.SetActive(false);
        }
        public static void HideMenu()
        {
            if (Root != null)
                Root.gameObject.SetActive(false);
            if (CurrentMenu != null)
                CurrentMenu.Hide();
        }
        public override void ReSize()
        {
            if (UIRoot != null)
                if (HCanvas.MainCanvas != null)
                    UIRoot.SizeDelta = HCanvas.MainCanvas.SizeDelta;
            base.ReSize();
        }
        static List<UIMenu> menus=new List<UIMenu>();
        public static UIMenu CurrentMenu { get; private set; }
        /// <summary>
        /// 释放掉当前未激活的弹窗
        /// </summary>
        public static void ReleaseMenu()
        {
            int c = menus.Count - 1;
            for (; c >= 0; c--)
            {
                var p = menus[c];
                if (p.model == null)
                {
                    p.Dispose(); 
                }
                else
                if (!p.Main.gameObject.activeSelf)
                {
                    p.Dispose(); 
                }
            }
        }
        public static T ShowMenu<T>(UIBase context, Rect pos, int dic, object obj = null) where T : UIMenu, new()
        {
            UIRoot.gameObject.SetActive(true);
            if (CurrentMenu != null)
            { 
                CurrentMenu.Hide(); 
                CurrentMenu = null; 
            }
            for (int i = 0; i < menus.Count; i++)
                if (menus[i] is T)
                {
                    CurrentMenu = menus[i];
                    CurrentMenu.ChangeLanguage();
                    menus[i].Show(context, pos, dic, obj);
                    return menus[i] as T;
                }
            var t = new T();
            menus.Add(t);
            CurrentMenu = t;
            t.Initial(Root, context, obj);
            t.ChangeLanguage();
            t.Show(context, pos, dic, obj);
            t.ReSize();
            return t;
        }
        static Vector4 GetPointer(UIElement target)
        {
            var coord = UIElement.GetGlobaInfo(target.transform,false);
            Vector2 tsize = target.m_sizeDelta;
            float left = tsize.x * (-target.Pivot.x);
            float right = left + tsize.x;
            left *= coord.Scale.x;
            left += coord.Postion.x;

            right *= coord.Scale.x;
            right += coord.Postion.x;

            float down = tsize.y *(- target.Pivot.y);
            float top = down + tsize.y;
            down *= coord.Scale.y;
            down += coord.Postion.y;

            top *= coord.Scale.y;
            top += coord.Postion.y;
            return new Vector4(left,right,down,top);
        }
        public static T PopDown<T>(UIBase context, Rect pos, object obj = null) where T : UIMenu, new()
        {
            if (pos.x < 0)
            {
                return ShowMenu<T>(context, pos, RightDown, obj);
            }
            else return ShowMenu<T>(context, pos, LeftDown, obj);
        }
        public static T PopUp<T>(UIBase context, Rect pos, object obj = null) where T : UIMenu, new()
        {
            if (pos.x < 0)
            {
                return ShowMenu<T>(context, pos, RightTop, obj);
            }
            else return ShowMenu<T>(context,pos, LeftTop, obj);
        }
        public static T PopUpOrDown<T>(UIBase context, Rect pos, object obj = null) where T : UIMenu, new()
        {
            if (pos.y < 0)
            {
                return PopDown<T>(context, pos, obj);
            }
            else
            {
                return PopUp<T>(context, pos, obj);
            }
        }
        public static T PopLeft<T>(UIBase context, Rect pos, object obj = null) where T : UIMenu, new()
        {
            if (pos.y < 0)
            {
                return ShowMenu<T>(context, pos, LeftTop, obj);
            }
            else return ShowMenu<T>(context, pos, LeftDown, obj);
        }
        public static T PopRight<T>(UIBase context, Rect pos, object obj = null) where T : UIMenu, new()
        {
            if (pos.y < 0)
            {
                return ShowMenu<T>(context, pos, RightTop, obj);
            }
            else return ShowMenu<T>(context, pos, RightDown, obj);
        }
        public static T PopLeftOrRight<T>(UIBase context, Rect pos, object obj = null) where T : UIMenu, new()
        {
            if(pos.x<0)
            {
               return PopRight<T>(context,pos,obj);
            }
            else
            {
               return PopLeft<T>(context,pos, obj);
            }
        }
        public static T PopDown<T>(UIBase context, UIElement target,object obj = null) where T : UIMenu, new()
        {
            Vector4 v4 = GetPointer(target);
            if (v4.x < 0)
            {
                return ShowMenu<T>(context, new Rect(v4.x, v4.z,0,0), RightDown, obj);
            }
            else return ShowMenu<T>(context,new Rect(v4.y,v4.z,0,0),LeftDown, obj);
        }
        public static T PopUp<T>(UIBase context, UIElement target, object obj = null) where T : UIMenu ,new()
        {
            Vector4 v4 = GetPointer(target);
            if (v4.x < 0)
            {
                return ShowMenu<T>(context, new Rect(v4.x, v4.w,0,0), RightTop, obj);
            }
            else return ShowMenu<T>(context, new Rect(v4.y, v4.w,0,0), LeftTop, obj);
        }
        public static T PopUpOrDown<T>(UIBase context, UIElement target, object obj = null) where T : UIMenu, new()
        {
            var coord = UIElement.GetGlobaInfo(target.transform, false);
            Rect r = new Rect(coord.Postion.x, coord.Postion.y, target.m_sizeDelta.x * coord.Scale.x, target.m_sizeDelta.y * coord.Scale.y);
            if (coord.Postion.y< 0)
            {
                return ShowMenu<T>(context,  r, Top, obj);
            }
            else return ShowMenu<T>(context, r, Down, obj);
        }
        public static T PopLeft<T>(UIBase context, UIElement target, object obj = null) where T : UIMenu, new()
        {
            Vector4 v4 = GetPointer(target);
            if (v4.z < 0)
            {
                return ShowMenu<T>(context, new Rect(v4.x, v4.z, 0, 0), LeftTop, obj);
            }
            else return ShowMenu<T>(context, new Rect(v4.x, v4.w, 0, 0), LeftDown, obj);
        }
        public static T PopRight<T>(UIBase context, UIElement target, object obj = null) where T :UIMenu, new()
        {
            Vector4 v4 = GetPointer(target);
            if (v4.z < 0)
            {
                return ShowMenu<T>(context, new Rect(v4.y, v4.z,0,0), RightTop, obj);
            }
            else return ShowMenu<T>(context, new Rect(v4.y, v4.w,0,0), RightDown, obj);
        }
        public static T PopLeftOrRight<T>(UIBase context, UIElement target, object obj = null) where T : UIMenu, new()
        {
            var coord = UIElement.GetGlobaInfo(target.transform, false);
            Rect r = new Rect(coord.Postion.x, coord.Postion.y, target.m_sizeDelta.x * coord.Scale.x, target.m_sizeDelta.y * coord.Scale.y);
            if (coord.Postion.x < 0)
            {
                return ShowMenu<T>(context, r, Right, obj);
            }
            else return ShowMenu<T>(context, r, Left, obj);
        }
        public static T GetMenu<T>() where T : UIMenu
        {
            for (int i = 0; i < menus.Count; i++)
                if (menus[i] is T)
                    return menus[i] as T;
            return null;
        }
        protected UIBase Context;
        public virtual void Show(UIBase context, Rect pos, int dic, object obj = null)
        {
            Context = context;
            var ui = Main.GetComponent<UIElement>();
            var size = ui.SizeDelta;
            float hw = size.x * 0.5f;
            float hh = size.y * 0.5f;
            switch (dic)
            {
                case LeftTop:
                    pos.x -= hw;
                    pos.y += hh;
                    break;
                case Top:
                    pos.y += hh;
                    break;
                case RightTop:
                    pos.x += hw;
                    pos.y += hh;
                    break;
                case Right:
                    pos.x += hw;
                    break;
                case RightDown:
                    pos.x += hw;
                    pos.y -= hh;
                    break;
                case Down:
                    pos.y -= hh;
                    break;
                case LeftDown:
                    pos.x -= hw;
                    pos.y -= hh;
                    break;
                case Left:
                    pos.x -= hw;
                    break;
            }
            Main.transform.localPosition = new Vector3(pos.x,pos.y,0);
            if (Main != null)
                Main.SetActive(true);
        }
        public virtual void Hide()
        {
            if (Main != null)
                Main.SetActive(false);
        }
        public override void Dispose()
        {
            base.Dispose();
            menus.Remove(this);
        }
    }
}