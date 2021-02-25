using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(HGUIRender), true)]
public class HGUIRenderEditor:Editor
{
    public void OnEnable()
    {
        render = target as HGUIRender;
        if(Application.isPlaying)
        {
            UIHierarchy.ChangeRoot(render.GetGuid(), render.canvas);
        }
        else
        {
            if (render.OldUI != null)
            {
                render.canvas = render.OldUI.Content;
                UIHierarchy.ChangeRoot(render.GetGuid(), render.canvas); 
            }
        }
    }
    public void OnDisable()
    {
        
    }
    static HCanvas canvas;
    static HGUIRender render;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        render = target as HGUIRender;
        if (GUILayout.Button("Test"))
        {
            Test();
        }
        if (GUI.changed)
        {
            UIHierarchy.ChangeRoot(render.GetGuid(), render.canvas);
        }
    }
    static void Test()
    {
        TA ta = new TA();
        ta.list = new List<string>();
        var ptr = UnsafeOperation.GetObjectAddr(ta);
        var obj = UnsafeOperation.GetObject(ptr + 16);
    }
    static object Create()
    {
       return Vector3Int.one;
    }
    class TA
    {
        public List<string> list;
    }
    class TB
    {
        public TA ta;
    }
}