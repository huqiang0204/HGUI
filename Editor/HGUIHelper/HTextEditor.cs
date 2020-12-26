using System;
using System.Collections.Generic;
using huqiang;
using huqiang.Helper.HGUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HText), true)]
[CanEditMultipleObjects]
public class HTextEditor:UIElementEditor
{
    string str;
    public override void OnEnable()
    {
        base.OnEnable();
        HText img = target as HText;
        if (img != null)
        {
            var can = FindHCanvas(img.transform);
            //if (can != null)
            //    can.Refresh();
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HText txt = target as HText;
        if (txt != null)
        {
            bool changed = false;
            if (str != txt.Text)
            {
                str = txt.Text;
                txt.Text = str;
                changed = true;
            }
            str = txt.Text;
            if(GUI.changed |changed)
            {
                txt.m_dirty = true;
                var tar = huqiang.Core.HGUI.UIElement.FindInstance(txt.ContextID);
                if (tar != null)
                    txt.ToHGUI2(tar, false);
            }
        }
    }
    protected HCanvas FindHCanvas(Transform trans)
    {
        if (trans == null)
            return null;
        var can = trans.GetComponent<HCanvas>();
        if (can == null)
            return FindHCanvas(trans.parent);
        return can;
    }
}