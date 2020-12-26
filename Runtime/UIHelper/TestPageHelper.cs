using huqiang;
using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System.IO;
using UnityEngine;


/// <summary>
/// 挂载在Canvas下
/// </summary>
public class TestPageHelper:UIHelper
{
    public Transform TargetCanvas;
    public virtual void LoadBundle()
    {
#if UNITY_EDITOR
        if (ElementAsset.bundles.Count == 0)
        {
            //var dic = Application.dataPath + "/StreamingAssets";
            var dic = Application.streamingAssetsPath;
            if (Directory.Exists(dic))
            {
                var bs = Directory.GetFiles(dic, "*.unity3d");
                for (int i = 0; i < bs.Length; i++)
                {
                    ElementAsset.bundles.Add(AssetBundle.LoadFromFile(bs[i]));
                }
            }
        } 
#endif
    }
    public string AssetName = "baseUI";
    private void Start()
    {
        Initital();
        LoadTestPage();
    }
    public void Initital()
    {
        LoadBundle();
        HGUIManager.Initial();
        var db = HGUIManager.GetPrefab(TargetCanvas);
        HGUIManager.prefabAssets.Clear();
        HGUIManager.LoadModels(db, AssetName);
        var c = transform.childCount;
        for (int i = 0; i < c; i++)
            GameObject.Destroy(transform.GetChild(i).gameObject);
    }
    
    public virtual void LoadTestPage()
    {
    }

    private void Update()
    {
        OnUpdate();
    }
    public virtual void OnUpdate()
    {
    }
    public virtual void OnDestroy()
    {
        App.Dispose();
        AssetBundle.UnloadAllAssetBundles(true);
        ElementAsset.bundles.Clear();
    }
}