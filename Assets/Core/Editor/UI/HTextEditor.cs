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
            txt.Text = EditorGUILayout.TextField("Text",txt.Text);
            txt.font = EditorGUILayout.ObjectField("Sprite", txt.font, typeof(Font), true) as Font;
        }
    }
}