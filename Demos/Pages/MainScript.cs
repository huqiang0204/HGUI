using huqiang;
using huqiang.Data;
using UnityEngine;
using huqiang.Core.HGUI;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Net;
using huqiang.UIModel;

public class MainScript : HCanvas
{
    public TextAsset baseUI;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        App.Initial(transform);
        HGUIManager.LoadModels(baseUI.bytes, "baseUI");
#if UNITY_EDITOR
        AssetBundle.UnloadAllAssetBundles(true);
#endif
        //ElementAsset.LoadAssetsAsync("base.unity3d").PlayOver = (o, e) =>
        //{
        //    UIPage.LoadPage<ChatPage>();
        //};
        UIPage.LoadPage<StartPage>();
        KcpDataControll.Instance.Connection("192.168.0.144", 8899);
    }
   protected override void OnDestroy()
    {
        base.OnDestroy();
        App.Dispose();
    }
}
