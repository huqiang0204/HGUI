using System;
using UnityEngine;
#if Hot
namespace huqiang.HotUIModel
#else
namespace huqiang.UIModel
#endif
{
    public class PopWindow : UIBase
    {
        public Func<bool> Back { get; set; }
        public UIBase UIContext;
        public virtual void Show(object obj = null)
        {
            if (Main != null)
                Main.gameObject.SetActive(true);
        }
        public virtual void Show(UIBase context, object obj = null)
        {
            UIContext = context;
            if (Main != null)
                Main.gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            if (Main != null)
                Main.gameObject.SetActive(false);
        }
        public virtual bool Handling(string cmd, object dat)
        {
            return false;
        }
    }
}

