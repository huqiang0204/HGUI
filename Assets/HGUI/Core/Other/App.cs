using huqiang.Data;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.UIModel;
using huqiang.UIEvent;

namespace huqiang
{
    public class App
    {
        static void CreateUI() 
        {
            UIPage.Initial(UIRoot);
            UIMenu.Initial(UIRoot);
            UINotify.Initial(UIRoot);

            var buff = new GameObject("Buffer");
            buff.transform.SetParent(UIRoot);
            buff.SetActive(false);
            buff.transform.localScale = Vector3.one;
            HGUIManager.Initial(buff.transform);
        }
        public static Transform UIRoot;
        public static void Initial(Transform uiRoot)
        {
            ThreadMission.SetMianId();
            Scale.Initial();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            IME.Initial();
#endif     
            UIRoot = uiRoot;
            CreateUI();
        }
        public static void Dispose()
        {
            ThreadMission.DisposeAll();
            RecordManager.ReleaseAll();
            ElementAsset.bundles.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            IME.Dispose();
#endif     
        }
        public static void Hide()
        {
            if (UIRoot != null)
                UIRoot.gameObject.SetActive(false);
        }
        public static void Show()
        {
            if (UIRoot != null)
                UIRoot.gameObject.SetActive(true);
        }
    }
}