using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.Helper.HGUI;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HCanvas), true)]
[CanEditMultipleObjects]
public class HCanvasEditor:UIElementEditor
{
    public static void Refresh(HCanvas canvas)
    {
        if (canvas == null)
            return;
        var can = canvas;
        UIElement.ResizeChild(can);
        Debug.LogError("需要数据同步");
        //huqiang.Core.HGUI.HGUIRender render = can.GetComponent<huqiang.Core.HGUI.HGUIRender>();
        //if (render != null)
        //{
        //    HGUIRenderEditor.Refresh(render, can);
        //}
    }

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
                    var go = HGUIManager.GameBuffer.Clone(HGUIManager.FindModel("assTest", CloneName));
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
                UIInitializer initializer = new UIInitializer();
                for (int i = 0; i < child.Length; i++)
                {
                    var go = HGUIManager.GameBuffer.Clone(child[i],initializer);
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
}
