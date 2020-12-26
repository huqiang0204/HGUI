using huqiang.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UIInspector : EditorWindow
{
    [MenuItem("HGUI/UIInspector")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        UIInspector window = (UIInspector)EditorWindow.GetWindow(typeof(UIInspector));
        window.Show();
        Instance = window;
    }
    static UIInspector Instance;
    static UIElement Target;
    static UIEditor editor;
    public static void ChangeUI(UIElement ui)
    {
        if(editor!=null)
        {
            editor.OnDisable();
        }
        Target = ui;
        if (Instance != null)
            Instance.Repaint();
        var tc = typeof(UIEditor).Assembly.GetTypes();
        var t = ui.GetType();
        for (int i = 0; i < 32; i++)
        {
            editor = FindEditor(tc, t.Name);
            if (editor == null)
            {
                t = t.BaseType; 
                if(t.Name== "UIElement")
                {
                    break;
                }
            }
        }
        if(editor==null)
        {
            editor = new UIEditor();
        }
        editor.Target = ui;
        editor.OnEnable();
    }
    static UIEditor FindEditor(Type[] types, string name)
    {
        for (int i = 0; i < types.Length; i++)
        {
            var t = types[i];
            var e = t.GetCustomAttribute<HGUIEditor>();
            if (e != null)
            {
                if (name == e.Target.Name)
                {
                    return Activator.CreateInstance(t) as UIEditor;
                }
            }
        }
        return null;
    }
    Vector2 scroll;
    void OnGUI()
    {
        Instance = this;
        if(editor!=null)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            editor.OnInspectorGUI();
            editor.DrawHelper();
            EditorGUILayout.EndScrollView();
        }
    }
    public virtual void OnEnable()
    {
#if UNITY_2019_1_OR_NEWER
        SceneView.duringSceneGui += DuringSceneGui;
#else
        SceneView.onSceneGUIDelegate += DuringSceneGui;
#endif
    }
    public virtual void OnDisable()
    {
#if UNITY_2019_1_OR_NEWER
        SceneView.duringSceneGui -= DuringSceneGui;
#else
        SceneView.onSceneGUIDelegate -= DuringSceneGui;
#endif
    }
    void DuringSceneGui(SceneView view)
    {
        DrawBorder(Target as UIElement);
    }
    public void DrawBorder(UIElement txt)
    {
        if (txt == null)
            return;
        Handles.color = Color.red;
        Vector3[] verts = new Vector3[8];
        var coord = UIElement.GetGlobaInfo(txt);
        var p = coord.Postion;
        var can = (txt.root as HCanvas);
        if (can != null)
            p += can.WorldPosition;
        var size = txt.SizeDelta;
        size.x *= coord.Scale.x;
        size.y *= coord.Scale.y;
        var bor = new Border(size, txt.Pivot);
        var q = coord.quaternion;
        var lt = p + q * new Vector3(bor.left, bor.top, 0);
        var rt = p + q * new Vector3(bor.right, bor.top, 0);
        var rd = p + q * new Vector3(bor.right, bor.down, 0);
        var ld = p + q * new Vector3(bor.left, bor.down, 0);
        verts[0] = lt;
        verts[1] = rt;
        verts[2] = rt;
        verts[3] = rd;
        verts[4] = rd;
        verts[5] = ld;
        verts[6] = ld;
        verts[7] = lt;
        Handles.DrawLines(verts);
    }
}