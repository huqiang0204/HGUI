using huqiang.Core.Line;
using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    /// <summary>
    /// 画线UI,支持直线,弧线,贝塞尔曲线,二阶贝塞尔曲线
    /// </summary>
    public class HLine: UIContext
    {
        public huqiang.Core.HGUI.HLine Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.HLine();
            ContextID = Content.GetInstanceID();
        }
        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.HLine();
            return Content;
        }
    }
}
