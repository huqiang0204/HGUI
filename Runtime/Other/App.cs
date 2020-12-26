using huqiang.Data;
using UnityEngine;
using huqiang.Core.HGUI;
using huqiang.UIModel;
using huqiang.UIEvent;

namespace huqiang
{
    /// <summary>
    /// 应用管理类
    /// </summary>
    public class App
    {
        static void CreateUI() 
        {
            UIPage.Initial(UIRoot);
            UIMenu.Initial(UIRoot);
            UINotify.Initial(UIRoot);

            var buff = new UIElement();
            buff.name = "Buffer";
            buff.SetParent(UIRoot);
            buff.activeSelf = false;
            buff.localScale = Vector3.one;
            HGUIManager.Initial();
        }
        /// <summary>
        /// 根节点
        /// </summary>
        public static UIElement UIRoot;
        /// <summary>
        /// 初始化UI布局
        /// </summary>
        /// <param name="uiRoot"></param>
        public static void Initial(UIElement uiRoot)
        {
            ThreadMission.SetMianId();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            IME.Initial();
#endif     
            UIRoot = uiRoot;
            CreateUI();
        }
        /// <summary>
        /// 释放资源:包含ThreadMission,RecordManager,ElementAsset,AssetBundle
        /// </summary>
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
        /// <summary>
        /// 隐藏
        /// </summary>
        public static void Hide()
        {
            if (UIRoot != null)
                UIRoot.activeSelf = false;
        }
        /// <summary>
        /// 显示
        /// </summary>
        public static void Show()
        {
            if (UIRoot != null)
                UIRoot.activeSelf = true;
        }
    }
}