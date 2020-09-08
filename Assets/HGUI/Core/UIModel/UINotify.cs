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
    public class UINotify : UIBase
    {
        public static Transform Root { get; set; }
        public static UIElement UIRoot { get; private set; }
        public static UINotify Instance { get; private set; }
        public static void Initial(Transform Canvas)
        {
            var menu = new GameObject("Notify");
            var ele = UIRoot = menu.AddComponent<UIElement>();
            ele.marginType = MarginType.Margin;
            Root = menu.transform;
            menu.transform.SetParent(Canvas);
            Root.localPosition = Vector3.zero;
            Root.localScale = Vector3.one;
            Root.localRotation = Quaternion.identity;
        }
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
        public static UINotify CurrentNotify { get; private set; }
        public static T ShowNotify<T>(UIBase context, object obj = null) where T : UINotify, new()
        {
            UIRoot.gameObject.SetActive(true);
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
            t.Initial(Root, context, obj);
            t.ChangeLanguage();
            t.Show(context, obj);
            t.ReSize();
            return t;
        }
        public UINotify()
        {
            Instance = this;
        }
        public virtual void Show(UIBase context, object dat = null)
        {
            if (Main != null)
                Main.SetActive(true);
        }
        public override void ReSize()
        {
            if (UIRoot != null)
                if (HCanvas.MainCanvas != null)
                    UIRoot.SizeDelta = HCanvas.MainCanvas.SizeDelta;
            base.ReSize();
        }
        public virtual void Hide()
        {
            if (Main != null)
                Main.SetActive(false);
        }

    }
}

