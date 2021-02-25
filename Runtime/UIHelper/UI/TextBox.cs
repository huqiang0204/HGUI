using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public class TextBox: UIContext
    {
        public huqiang.Core.HGUI.TextBox Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.TextBox();
            ContextID = Content.GetInstanceID();
        }
        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.TextBox();
            return Content;
        }
    }
}
