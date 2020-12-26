using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            var can = UIElement.FindInstance(render.ContextID);
            if (can != null)
            {
                can.name = render.name;
                UIHierarchy.ChangeRoot(render.GetGuid(), can);
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
        //render.PauseEvent = EditorGUILayout.Toggle("Pause Event", render.PauseEvent);
        //render.Pause = EditorGUILayout.Toggle("Pause", render.Pause);
        //render.renderQueue = EditorGUILayout.IntField("Render Queue", render.renderQueue);
        if (GUI.changed)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            render.canvas = canvas;
            render.Refresh();
        }
        //if (GUI.changed)
        //    Refresh(target as HGUIRender,t);
    }
    public static void Refresh(HGUIRender render, huqiang.Helper.HGUI.HCanvas oldUI)
    {
        var can = HCanvas.FindInstance(oldUI.ContextID);
        if(can==null)
        {
            HGUIManager.Initial();
            var db = HGUIManager.GetPrefab(oldUI.transform);
            string dic = Environment.CurrentDirectory + "/hgui2";
            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);
            string path = dic + "/layout.bytes";
            File.WriteAllBytes(path, db.ToBytes());
            huqiang.Core.HGUI.UIElement.DisposeAll();
            CloneAll(db, render.canvas);
        }
        else
        {
            render.canvas = can as HCanvas;
        }
        UIHierarchy.ChangeRoot(render.GetGuid(), render.canvas);
        render.Refresh();
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
    //public static void Refresh()
    //{
    //    if (render != null)
    //    {
    //        if(canvas==null)
    //        {
    //            string dic = Environment.CurrentDirectory + "/hgui2";
    //            string path = dic + "/" + render.GetGuid() + ".bytes";
    //            if (File.Exists(path))
    //            {
    //                byte[] dat = File.ReadAllBytes(path);
    //                if (!Application.isPlaying)
    //                    huqiang.Core.HGUI.UIElement.DisposeAll();
    //                if (HGUIManager.GameBuffer == null)
    //                    HGUIManager.Initial();
    //                CloneAll(new DataBuffer(dat), render.canvas);
    //                canvas = render.canvas;
    //                UIHierarchy.ChangeRoot(render.GetGuid(), render.canvas);
    //            }
    //        }
    //        if (canvas != null)
    //        {
    //            render.canvas = canvas;
    //            render.Refresh();
    //        }
    //    }
    //}
}