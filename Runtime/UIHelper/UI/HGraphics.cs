using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public class HGraphics: UIContext
    {
        public huqiang.Core.HGUI.HGraphics Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.HGraphics();
            ContextID = Content.GetInstanceID();
        }
        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.HGraphics();
            return Content;
        }
    }
}
