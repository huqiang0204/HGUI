using huqiang.Data;
using huqiang.UI;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;
using huqiang;
using Assets.Scripts;

public class UITest : TestPageHelper
{
    public override void LoadTestPage()
    {
        Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
#if UNITY_EDITOR
        UIPage.LoadPage<ChatPage>();
#else
        ElementAsset.LoadAssetsAsync("base.unity3d",(o,e)=> { UIPage.LoadPage<AniTestPage>(); });
#endif
    }
}