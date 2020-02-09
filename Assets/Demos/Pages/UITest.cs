using huqiang.Data;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;
using huqiang;
using Assets.Scripts;
using huqiang.Communication;

public class UITest : TestPageHelper
{
    public override void LoadTestPage()
    {
        RemoteLog.Instance.Connection("192.168.0.144",8899);
        Debug.Log(Scale.ScreenSize);
        Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
#if UNITY_EDITOR
        UIPage.LoadPage<StartPage>();
#else
        //ElementAsset.LoadAssetsAsync("base.unity3d",(o,e)=> { UIPage.LoadPage<ChatPage>(); });
#endif
    }
    public override void OnUpdate()
    {
        Resize();
    }
    void Resize()
    {
        float w = Screen.width;
        float h = Screen.height;
        if (Scale.ScreenWidth != w | Scale.ScreenHeight != h)
        {
            Scale.ScreenWidth = w;
            Scale.ScreenHeight = h;
            if (UIPage.CurrentPage != null)
                UIPage.CurrentPage.ReSize();
        }
    }
}