﻿using huqiang.Helper.HGUI;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextBox), true)]
[CanEditMultipleObjects]
public class TextBoxEditor : UIElementEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextBox tb = target as TextBox;
        if (tb != null)
        {
            var box = tb.Content;
            GUILayout.BeginHorizontal();
            GUILayout.Label("PercentageX");
            var value = GUILayout.HorizontalSlider(box.PercentageX, 0, 1);
            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;
            box.PercentageX = value;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("PercentageY");
            value = GUILayout.HorizontalSlider(box.PercentageY, 0, 1);
            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;
            box.PercentageY = value;
            GUILayout.EndHorizontal();
            if (GUI.changed)
            {
                box.m_vertexChange = true;
                //var tar = huqiang.Core.HGUI.UIElement.FindInstance(box.ContextID);
                //if (tar != null)
                //    box.ToHGUI2(tar, false);
            }
        }
    }
}
