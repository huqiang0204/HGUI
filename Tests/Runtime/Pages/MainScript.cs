using huqiang;
using huqiang.Data;
using UnityEngine;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Net;
using huqiang.UIModel;
using huqiang.UIComposite;
using huqiang.Core.HGUI;

public class MainScript : MonoBehaviour
{
    public TextAsset baseUI;
    // Start is called before the first frame update
    protected  void Start()
    {
        KcpDataControll.Instance.Connection("192.168.0.134", 8899);
        KcpDataControll.Instance.OpenLog();

        var can = GetComponent<HGUIRender>().canvas;
        App.Initial(can);
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

    protected  void Update()
    {
        KcpDataControll.Instance.DispatchMessage();
    }
    protected  void OnDestroy()
    {
        App.Dispose();
    }
}
