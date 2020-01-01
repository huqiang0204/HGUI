using Assets.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HText), true)]
[CanEditMultipleObjects]
public class HTextEditor:Editor
{
    Vector3 pos;
    Vector3 scale;
    Vector3 angle;
    string str;
    private void OnEnable()
    {
        HText img = target as HText;
        if (img != null)
        {
            str = img.Text;
        }
            
    }
    private void OnSceneGUI()
    {
        HText img = target as HText;
        if (img != null)
        {
            bool changed = false;
            if (pos != img.transform.localPosition)
                changed = true;
            else if (scale != img.transform.localScale)
                changed = true;
            else if (angle != img.transform.localEulerAngles)
                changed = true;
            pos = img.transform.localPosition;
            scale = img.transform.localScale;
            angle = img.transform.localEulerAngles;
            if (changed)
            {
                var can = FindHCanvas(img.transform);
                if (can != null)
                    can.Refresh();
            }
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
                var can = FindHCanvas(txt.transform);
                if (can != null)
                    can.Refresh();
            }
        }
    }
    HCanvas FindHCanvas(Transform trans)
    {
        if (trans == null)
            return null;
        var can = trans.GetComponent<HCanvas>();
        if (can == null)
            return FindHCanvas(trans.parent);
        return can;
    }
}