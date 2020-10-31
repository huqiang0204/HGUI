using huqiang;
using huqiang.Data;
using UnityEngine;
using huqiang.Core.HGUI;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Net;
using huqiang.UIModel;
using huqiang.UIComposite;

public class MainScript : HCanvas
{
    public TextAsset baseUI;
    // Start is called before the first frame update
    protected override void Start()
    {
        KcpDataControll.Instance.Connection("192.168.0.134", 8899);
        KcpDataControll.Instance.OpenLog();
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
        DataGrid.CursorX = DockPanelLine.CursorX = Resources.Load<Texture2D>("StretchWX");
        DataGrid.CursorY = DockPanelLine.CursorY = Resources.Load<Texture2D>("StretchWY");
    }

    protected override void Update()
    {
        base.Update();
        KcpDataControll.Instance.DispatchMessage();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        App.Dispose();
    }
}
