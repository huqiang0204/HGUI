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
    public class HLine: HGraphics
    {
        public override Core.HGUI.UIElement ToHGUI2(bool activeSelf,bool haveChild=true)
        {
            Core.HGUI.HLine line = new Core.HGUI.HLine();
            SaveToHGraphics(line,activeSelf,haveChild);
            return line;
        }
        public override void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        {
            SaveToHGraphics(ui as Core.HGUI.HLine, activeSelf,false);
        }
    }
}
