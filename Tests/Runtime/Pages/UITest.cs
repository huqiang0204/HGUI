using huqiang.Data;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;
using huqiang;
using Assets.Scripts;
using huqiang.UIModel;
using huqiang.Core.HGUI;
using huqiang.Core.UIData;

public class UITest : TestPageHelper
{
    public HGUIRender GUIRender;
    public Font font;
    public override void LoadTestPage()
    {
        UIElement.DisposeAll();
        HCanvas.RegCanvas(GUIRender);
        UISystem.PhysicalScale =1f;
        font.RequestCharactersInTexture("ABCDEF", 512);//OPQUVWXYZ
        HTextLoader.fonts.Clear();
        HTextLoader.fonts.Add(font);
        //HCanvas can = new HCanvas();
        //can.DesignSize = GUIRender.DesignSize;
        //can.SizeDelta = GUIRender.DesignSize;
        //can.name = GUIRender.name;
        //GUIRender.canvas = can;
        HCanvas.CurrentCanvas = GUIRender.canvas;
        App.Initial(GUIRender.canvas);
        Application.targetFrameRate = 1000;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
#if UNITY_EDITOR
        UIPage.LoadPage<StartPage>();
#else
        //ElementAsset.LoadAssetsAsync("base.unity3d",(o,e)=> { UIPage.LoadPage<ChatPage>(); });
#endif
        DataGrid.CursorX = DockPanelLine.CursorX = Resources.Load<Texture2D>("StretchWX");
        DataGrid.CursorY = DockPanelLine.CursorY = Resources.Load<Texture2D>("StretchWY");
    }
    public override void OnUpdate()
    {
        //if (IME.CompStringChanged)
        //    Debug.Log(IME.CompString);
        //if (IME.InputDone)
        //    Debug.Log(IME.ResultString);
    }
}