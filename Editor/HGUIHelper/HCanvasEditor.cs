using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.Helper.HGUI;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HCanvas), true)]
[CanEditMultipleObjects]
public class HCanvasEditor:Editor
{
    public static HCanvas Instance;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        HCanvas ele = target as HCanvas;
        if (GUILayout.Button("Clear All AssetBundle"))
        {
            AssetBundle.UnloadAllAssetBundles(true);
            ElementAsset.bundles.Clear();
        }
        if (GUILayout.Button("Create New"))
        {
            Create(ele.AssetName + ".bytes", ele.dicpath, ele.gameObject);
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Clone New"))
        {
            if (ele.NewBytesUI != null)
                CloneNew(ele.CloneName, ele.NewBytesUI.bytes, ele.transform);
        }
        if (GUILayout.Button("Clone New All"))
        {
            if (ele.NewBytesUI != null)
                CloneNewAll(ele.NewBytesUI.bytes, ele.transform);
        }
        if (GUILayout.Button("Clone Old"))
        {
            if (ele.OldBytesUI != null)
                Clone(ele.CloneName, ele.OldBytesUI.bytes, ele.transform);
        }
        if (GUILayout.Button("Clone Old All"))
        {
            if (ele.OldBytesUI != null)
                CloneAll(ele.OldBytesUI.bytes, ele.transform);
        }
        serializedObject.ApplyModifiedProperties();
    }
    static void LoadBundle()
    {
        if (ElementAsset.bundles.Count == 0)
        {
            var dic = Application.dataPath + "/StreamingAssets";
            if (Directory.Exists(dic))
            {
                var bs = Directory.GetFiles(dic, "*.unity3d");
                for (int i = 0; i < bs.Length; i++)
                {
                    ElementAsset.bundles.Add(AssetBundle.LoadFromFile(bs[i]));
                }
            }
        }
    }
    static void Create(string Assetname, string dicpath, GameObject gameObject)
    {
        if (Assetname == null)
            return;
        if (Assetname == "")
            return;
        LoadBundle();
        Assetname = Assetname.Replace(" ", "");
        var dc = dicpath;
        if (dc == null | dc == "")
        {
            dc = Application.dataPath + "/AssetsBundle/";
            if (!Directory.Exists(dc))
                Directory.CreateDirectory(dc);
        }
        dc += Assetname;
        huqiang.Core.HGUI.HGUIManager.Initial();
        huqiang.Core.HGUI.HGUIManager.SavePrefab(gameObject.transform, dc);
        Debug.Log("create done path:" + dc);
    }
    static void Clone(string CloneName, byte[] ui, Transform root)
    {
        if (ui != null)
        {
            if (CloneName != null)
                if (CloneName != "")
                {
                    HGUIManager.Initial(root);
                    HGUIManager.LoadModels(ui, "assTest");
                    Initializer initializer = new Initializer();
                    var go = HGUIManager.Clone(HGUIManager.FindModel("assTest", CloneName),initializer);
                    if (go != null)
                    {
                        var trans = go.transform;
                        trans.SetParent(root);
                        trans.localScale = Vector3.one;
                        trans.localScale = Vector3.one;
                        trans.localRotation = Quaternion.identity;
                    }
                }
        }
    }
    static void CloneAll(byte[] ui, Transform root)
    {
        if (ui != null)
        {
            HGUIManager.Initial(root);
            var fake = HGUIManager.LoadModels(ui, "assTest").models;
            var child = HGUIManager.GetAllChild(fake);
            if (child != null)
            {
                Initializer initializer = new Initializer();
                for (int i = 0; i < child.Length; i++)
                {
                    var go = HGUIManager.Clone(child[i],initializer);
                    if (go != null)
                    {
                        var trans = go.transform;
                        trans.SetParent(root);
                        trans.localPosition = Vector3.zero;
                        trans.localScale = Vector3.one;
                        trans.localRotation = Quaternion.identity;
                    }
                }
            }
        }
    }
    static void CloneNew(string CloneName, byte[] ui, Transform root)
    {
        if (ui != null)
        {
            if (CloneName != null)
                if (CloneName != "")
                {
                    huqiang.Core.HGUI.HGUIManager.Initial();
                    huqiang.Core.HGUI.HGUIManager.LoadModels(ui, "assTest");
                    var ele = huqiang.Core.HGUI.HGUIManager.Clone(huqiang.Core.HGUI.HGUIManager.FindModel("assTest", CloneName));
                    if (ele != null)
                    {
                        Initializer ini = new Initializer();
                        var go =  CreateGameObject(ele ,ini);
                        ini.Done();
                        var trans = go.transform;
                        trans.SetParent(root);
                        trans.localScale = Vector3.one;
                        trans.localScale = Vector3.one;
                        trans.localRotation = Quaternion.identity;
                    }
                }
        }
    }
    static void CloneNewAll(byte[] ui, Transform root)
    {
        if (ui != null)
        {
            huqiang.Core.HGUI.HGUIManager.Initial();
            var fake = huqiang.Core.HGUI.HGUIManager.LoadModels(ui, "assTest").models;
            var child = huqiang.Core.HGUI.HGUIManager.GetAllChild(fake);
            if (child != null)
            {
                for (int i = 0; i < child.Length; i++)
                {
                    var ele = huqiang.Core.HGUI.HGUIManager.Clone(child[i]);
                    if (ele != null)
                    {
                        Initializer ini = new Initializer();
                        var go = CreateGameObject(ele, ini);
                        ini.Done();
                        var trans = go.transform;
                        trans.SetParent(root);
                        trans.localPosition = Vector3.zero;
                        trans.localScale = Vector3.one;
                        trans.localRotation = Quaternion.identity;
                    }
                }
            }
        }
    }
    static GameObject CreateGameObject(huqiang.Core.HGUI.UIElement ui,Initializer ini)
    {
        GameObject go = new GameObject();
        go.name = ui.name;
        switch (ui.TypeName)
        {
            case huqiang.Core.HGUI.UIType.UIElement:
                var tar = go.AddComponent<UIElement>();
                tar.Content = ui;
                break;
            case huqiang.Core.HGUI.UIType.HImage:
                var img = go.AddComponent<HImage>();
                img.Content = ui as huqiang.Core.HGUI.HImage;
                break;
            case huqiang.Core.HGUI.UIType.HText:
                var txt = go.AddComponent<HText>();
                txt.Content = ui as huqiang.Core.HGUI.HText;
                break;
            case huqiang.Core.HGUI.UIType.TextBox:
                var tb = go.AddComponent<TextBox>();
                tb.Content = ui as huqiang.Core.HGUI.TextBox;
                break;
            case huqiang.Core.HGUI.UIType.HLine:
                var line = go.AddComponent<HLine>();
                line.Content = ui as huqiang.Core.HGUI.HLine;
                break;
            case huqiang.Core.HGUI.UIType.HGraphics:
                var gra = go.AddComponent<HGraphics>();
                gra.Content = ui as huqiang.Core.HGUI.HGraphics;
                break;
            case huqiang.Core.HGUI.UIType.HCanvas:
                var can = go.AddComponent<HCanvas>();
                can.Content = ui as huqiang.Core.HGUI.HCanvas;
                break;
        }
        ui.Context = go.transform;
        unsafe
        {
            var src = (huqiang.Core.UIData.UIElementData*)ui.mod.ip;
            ini.AddContext(ui.Context, src->insID);
            LoadHelper(go, ui.mod.buffer, src->ex, ini);
            LoadHelper(go, ui.mod.buffer, src->composite, ini);
            LoadHelper(go, ui.mod.buffer, src->ex, ini);
        }
      
        if(ui.parent!=null)
        {
            ui.Context.SetParent(ui.parent.Context);
            ui.Context.localPosition = ui.localPosition;
            ui.Context.localRotation = ui.localRotation;
            ui.Context.localScale = ui.localScale;
        }
        var c = ui.child.Count;
        for (int i = 0; i < c; i++)
        {
            CreateGameObject(ui.child[i], ini);
        }
        return go;
    }
    static void LoadHelper(GameObject com, DataBuffer buff, int v, Initializer initializer)
    {
        int type = v >> 16;
        string str = buff.GetData(type) as string;
        if (str != null)
        {
            var ex = buff.GetData(v & 0xffff) as FakeStruct;
            if (ex != null)
            {
                var tps = typeof(UIHelper).Assembly.GetTypes();
                if (tps != null)
                {
                    for (int i = 0; i < tps.Length; i++)
                    {
                        if (tps[i].Name == str)
                        {
                            var help = com.AddComponent(tps[i]) as UIHelper;
                            help.LoadFromBuffer(ex, initializer);
                            return;
                        }
                    }
                }
            }
        }
    }
}
