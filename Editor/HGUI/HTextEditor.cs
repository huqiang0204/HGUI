using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[HGUIEditor(typeof(HText))]
public class HtextEditor:UIEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUI.changed)
        {
            (Target as HText).m_dirty = true;
        }
    }
}

