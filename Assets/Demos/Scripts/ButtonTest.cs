using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : TestPageHelper
{
    public override void LoadTestPage()
    {
        //Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
#if UNITY_EDITOR
        UIPage.LoadPage<ButtonPage>();
#else
        ElementAsset.LoadAssetsAsync("base.unity3d",(o,e)=> { UIPage.LoadPage<AniTestPage>(); });
#endif
    }
}
