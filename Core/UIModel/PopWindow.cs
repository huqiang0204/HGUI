using System;
using UnityEngine;

namespace huqiang.UIModel
{
    public class PopWindow : UIBase
    {
        public Func<bool> Back { get; set; }
        protected UIPage mainPage;
        public virtual void Initial(Transform parent, UIPage page, object obj = null)
        {
            base.Initial(parent, page, obj);
            mainPage = page;
            if (model != null)
                if (page != null)
                    Main.transform.SetParent(parent);
        }
        public virtual void Show(object obj = null)
        {
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

