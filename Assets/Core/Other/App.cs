using huqiang.Data;
using UnityEngine;
using huqiang.Core.HGUI;

namespace huqiang
{
    public class App
    {
        static void InitialInput()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            IME.Initial();
#endif     
        }
        static void CreateUI() 
        {
            var page = new GameObject("page");
            UIPage.Root = page.transform;
            page.transform.SetParent(UIRoot);
            UIPage.Root.localPosition = Vector3.zero;
            UIPage.Root.localScale = Vector3.one;
            UIPage.Root.localRotation = Quaternion.identity;

            var buff = new GameObject("buffer");
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
            InitialInput();
            UIRoot = uiRoot;
            CreateUI();
        }
        public static void Dispose()
        {
            ThreadMission.DisposeAll();
            RecordManager.ReleaseAll();
            ElementAsset.bundles.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
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