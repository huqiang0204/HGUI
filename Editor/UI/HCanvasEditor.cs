using huqiang.Core.HGUI;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HCanvas), true)]
[CanEditMultipleObjects]
public class HCanvasEditor:UIElementEditor
{
    //public override void OnSceneGUI()
    //{
    //    base.OnSceneGUI();
    //    Refresh(target as HCanvas);
    //}

    public static HCanvas Instance;
    public override void OnEnable()
    {
        UIHierarchy.ChangeRoot((target as HCanvas).transform);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
            Refresh(target as HCanvas);
    }
    void Refresh(HCanvas canvas)
    {
        if (canvas == null)
            return;

        canvas.Refresh();
    }
    
}
