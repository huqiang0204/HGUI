using huqiang.Core.HGUI;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextBox), true)]
[CanEditMultipleObjects]
public class TextBoxEditor : HTextEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TextBox box = target as TextBox;
        if (box != null)
        {
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
            }
        }
    }
}
