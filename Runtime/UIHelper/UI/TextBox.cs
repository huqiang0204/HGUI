using huqiang.Core.UIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public class TextBox: HText
    {
        int startLine;
        float StartX;
        float EndX;
        float offsetX;
        float StartY;
        float EndY;
        float cw;
        float ch;
        public float PercentageX
        {
            get 
            {
                if (cw <= m_sizeDelta.x)
                    return 0;
                return offsetX / (cw - m_sizeDelta.x);
            }
            set 
            {
                if(cw>m_sizeDelta.x)
                {
                    if (value < 0)
                        value = 0;
                    else if (value > 1)
                        value = 1;
                    offsetX = value * (cw - m_sizeDelta.x);
                    m_vertexChange = true;
                }
            }
        }
        public float PercentageY {
            get
            {
                if (AllLine == 0)
                    return 0;
                if (AllLine <= ShowRow)
                    return 0;
                float a = startLine;
                float b = AllLine;
                float c = ShowRow;
                return a/ (b - c);
            }
            set
            {
                if (AllLine > 0)
                {
                    if (AllLine > ShowRow)
                    {
                        if (value < 0)
                            value = 0;
                        else if (value > 1)
                            value = 1;
                        int r = AllLine - ShowRow;
                        float s = value * r;
                        startLine = (int)s;
                        m_vertexChange = true;
                    }
                }
            }
        }
        public int StartLine { get => startLine;set {
                if (value < 0)
                    value = 0;
                else if (value+ ShowRow > AllLine)
                    value = AllLine - ShowRow;
                if(value!=startLine)
                {
                    startLine = value;
                    m_vertexChange = true;
                }
            } }
        public int AllLine { get; private set; }
        public int ShowRow { get; private set; }
        public float ContentWidth { get => cw; }
        public float ContentHeight { get => ch; }
        protected void SaveToTextBox(Core.HGUI.TextBox ui, bool activeSelf,bool haveChild)
        {
            ui.PercentageX = PercentageX;
            ui.PercentageY = PercentageY;
            ui.StartLine = startLine;
            SaveToHText(ui,activeSelf,haveChild);
        }
        public override Core.HGUI.UIElement ToHGUI2(bool activeSelf,bool haveChild=true)
        {
            Core.HGUI.TextBox htxt = new Core.HGUI.TextBox();
            SaveToTextBox(htxt,activeSelf,haveChild);
            return htxt;
        }
        public override void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        {
            SaveToTextBox(ui as Core.HGUI.TextBox, activeSelf,false);
        }
    }
}
