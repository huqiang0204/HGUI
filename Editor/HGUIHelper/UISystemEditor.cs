using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UISystem), true)]
public class UISystemEditor:Editor
{
    public void OnEnable()
    {
        UISystem.Instance = target as UISystem;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Slider("PhysicalScale", UISystem.PhysicalScale,0.3f,3);
        if(GUI.changed)
        {
            (target as UISystem).OverCamera();
        }
    }
}