using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HCanvasEditorUpdate
{
    public enum PlayModeState
    {
        Playing,
        Paused,
        Stop,
        PlayingOrWillChangePlayMode
    }
    class DataContext
    {
        public string guid;
        public UIElement Root;
    }
    static HCanvasEditorUpdate()
    {
        EditorApplication.update += Update;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        if(EditorApplication.isCompiling)
            UIElement.DisposeAll();
    }
    static float time;
    static void Update()
    {
        if (Application.isPlaying)
            return;
        if (EditorApplication.isCompiling)
        { 
            UIElement.DisposeAll();
        }
        time += Time.deltaTime;
        if(time>1)
        {
            time = 0;
            var sys = SceneAsset.FindObjectsOfType(typeof(UISystem));
            if (sys != null)
            {
                if (sys.Length > 0)
                    (sys[0] as UISystem).OverCamera();
            }
            var objs = SceneAsset.FindObjectsOfType(typeof(UIHelper));
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    var obj = objs[i] as UIHelper;
                    if (obj != null)
                        obj.Refresh();
                }
            }
            objs = SceneAsset.FindObjectsOfType(typeof(HGUIRender));
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    var obj = objs[i] as HGUIRender;
                    if (obj != null)
                    {
                        UIElement.ResizeChild(obj.canvas);
                        if (obj.OldUI != null)
                            huqiang.Helper.HGUI.UIElement.Resize(obj.OldUI);
                        var render = obj.GetComponent<huqiang.Core.HGUI.HGUIRender>();
                        if (render != null)
                        {
                            render.Refresh();
                        }
                    }
                }
            }
        }
    }
    static void OnHierarchyChanged()
    {
        if (!Application.isPlaying)
            UIElement.DisposeAll();
    }
    static List<DataContext> CanvasCash = new List<DataContext>();
    public static HCanvas GetData(HGUIRender render)
    {
        string guid = render.GetGuid();
        for(int i=0;i<CanvasCash.Count;i++)
        {
            if(guid==CanvasCash[i].guid)
            {
                return CanvasCash[i].Root as HCanvas;
            }
        }
        string dic = Environment.CurrentDirectory + "/hgui2";
        string path = dic + "/" + render.GetGuid() + ".bytes";
        if (File.Exists(path))
        {
            byte[] dat = File.ReadAllBytes(path);
            huqiang.Core.HGUI.UIElement.DisposeAll();
            if (HGUIManager.GameBuffer == null)
                HGUIManager.Initial();
            if (render.canvas == null)
                render.canvas = new HCanvas();
            CloneAll(new DataBuffer(dat), render.canvas);
            DataContext dc = new DataContext();
            dc.guid = guid;
            dc.Root = render.canvas;
            CanvasCash.Add(dc);
            return render.canvas;
        }
        return null;
    }
    static void CloneAll(DataBuffer db, UIElement root)
    {
        root.child.Clear();
        var fake = HGUIManager.LoadModels(db, "assTest").models;
        var child = HGUIManager.GetAllChild(fake);
        if (child != null)
        {
            for (int i = 0; i < child.Length; i++)
            {
                var go = HGUIManager.Clone(child[i]);
                if (go != null)
                {
                    go.SetParent(root);
                    go.localScale = Vector3.one;
                }
            }
        }
    }
}