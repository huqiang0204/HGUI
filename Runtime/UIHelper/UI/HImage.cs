using huqiang.Core.UIData;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public class HImage:UIContext
    {
        public huqiang.Core.HGUI.HImage Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.HImage();
            ContextID = Content.GetInstanceID();
        }
        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.HImage();
            return Content;
        }
    }
}
