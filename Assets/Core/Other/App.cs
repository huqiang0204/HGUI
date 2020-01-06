using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

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
            ModelManagerUI.CycleBuffer = buff.transform;
        }
        public static Transform UIRoot;
        static ThreadMission mission;
        public static void Initial(Transform uiRoot)
        {
            ThreadMission.SetMianId();
            Scale.Initial();
            InitialInput();
            UIRoot = uiRoot;
            CreateUI();
        }
        public static float AllTime;
        public static float FrameTime = 33;
        static float time;
        public static void Update()
        {
            Scale.MainUpdate();
            UserAction.Update();
            RenderForm.LoadAction();
            InputCaret.UpdateCaret();
            Keyboard.DispatchEvent();
            RenderForm.ApplyAll();
            ThreadMission.ExtcuteMain();
            ModelManagerUI.RecycleGameObject();
            AnimationManage.Manage.Update();
            UIPage.MainRefresh(UserAction.TimeSlice);
            AllTime += Time.deltaTime;
            mission.AddSubMission(SubThread, null);
        }
        static void SubThread(object obj)
        {
            RenderForm.DispatchAction();
            Resize();
            UIPage.Refresh(UserAction.TimeSlice);
            UINotify.Refresh(UserAction.TimeSlice);
            UIAnimation.Manage.Update();
            RenderForm.VertexCalculationAll();
        }
        static void Resize()
        {
            if(Scale.ScreenChanged())
            {
                Vector2 v = new Vector2(Scale.LayoutWidth, Scale.LayoutHeight);
                //UIPage.Root.data.sizeDelta = v;
                //if (Scale.DpiScale)
                //{
                //    var dr = Scale.DpiRatio;
                //    UIPage.Root.data.localScale = new Vector3(dr, dr, dr);
                //}
                //else UIPage.Root.data.localScale = Vector3.one;
                //UIPage.Root.data.sizeDelta = new Vector2(Scale.ScreenWidth, Scale.ScreenHeight);
                //UIPage.Root.IsChanged = true;
                if (UIPage.CurrentPage != null)
                    UIPage.CurrentPage.ReSize();
                if (UIMenu.Instance != null)
                    UIMenu.Instance.ReSize();
                if (UINotify.Instance != null)
                    UINotify.Instance.ReSize();
            }
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
            
        }
        public static void Show()
        {
            
        }
    }
}