using huqiang;
using huqiang.Core.HGUI;
using huqiang.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestPageHelper), true)]
public class TestPageHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        TestPageHelper ele = target as TestPageHelper;
        if (GUILayout.Button("Clear All AssetBundle"))
        {
            AssetBundle.UnloadAllAssetBundles(true);
            ElementAsset.bundles.Clear();
        }
        if (GUILayout.Button("Create"))
        {
            Create(ele.AssetName+".bytes", ele.dicpath, ele.gameObject);
        }
        if (GUILayout.Button("Clone"))
        {
            if (ele.bytesUI != null)
                Clone(ele.CloneName, ele.bytesUI.bytes, ele.transform);
        }
        if (GUILayout.Button("CloneAll"))
        {
            if (ele.bytesUI != null)
                CloneAll(ele.bytesUI.bytes, ele.transform);
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
        HGUIManager.Initial(gameObject.transform);
        HGUIManager.SavePrefab(gameObject.transform, dc);
        Debug.Log("create done path:"+dc);
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
                    var go = HGUIManager.GameBuffer.Clone(HGUIManager.FindModel("assTest",CloneName));
                    if(go!=null)
                    {
                        var trans = go.transform;
                        trans.SetParent(root);
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
                for (int i = 0; i < child.Length; i++)
                {
                    var go = HGUIManager.GameBuffer.Clone(child[i]);
                    if (go != null)
                    {
                        var trans = go.transform;
                        trans.SetParent(root);
                    }
                }
            }
        }
    }
}