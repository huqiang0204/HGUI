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
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HText txt = target as HText;
        if (txt != null)
        {
            EditorGUILayout.LabelField("Text");
            var style = GUI.skin.textArea;
            style.wordWrap = true;
            txt.Text = EditorGUILayout.TextArea(txt.Text,style);
            txt.font = EditorGUILayout.ObjectField("Font", txt.font, typeof(Font), true) as Font;
        }
    }
}