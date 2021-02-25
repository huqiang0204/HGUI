using System;
using System.Collections.Generic;
using huqiang;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using huqiang.UIModel;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    /// <summary>
    /// ui画布
    /// </summary>
    public class HCanvas: UIContext
    {
        public static HCanvas MainCanvas;
        [SerializeField]
        public huqiang.Core.HGUI.HCanvas Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.HCanvas();
            ContextID = Content.GetInstanceID();
        }
        /// <summary>
        /// 目标相机,如果为空则使用主相机
        /// </summary>
        public Camera camera;
        ///// <summary>
        ///// 默认设计尺寸
        ///// </summary>
        //public Vector2 DesignSize = new Vector2(1920, 1080);
        /// <summary>
        /// 距离相机镜头的距离
        /// </summary>
        public float NearPlane = 0f;
        public RenderMode renderMode;
        ///// <summary>
        ///// 暂停用户事件
        ///// </summary>
        //public bool PauseEvent;
        ///// <summary>
        ///// 暂停所有更新
        ///// </summary>
        //public bool Pause;
        //public int renderQueue = 3100;
#if UNITY_EDITOR
        public TextAsset NewBytesUI;
        public TextAsset OldBytesUI;
        public string AssetName = "baseUI";
        public string dicpath;
        public string CloneName;
        public void ApplyToCamera()
        {
            switch (renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    OverCamera(Camera.main);
                    break;
                case RenderMode.ScreenSpaceCamera:
                    OverCamera(camera);
                    break;
                case RenderMode.WorldSpace:
                    break;
            }
        }
        void OverCamera(Camera cam)
        {
            if (cam != null)
            {
                float w = cam.pixelWidth;
                float h = cam.pixelHeight;
                float ps = huqiang.Core.HGUI.UISystem.PhysicalScale;
                if (ps < 0.01f)
                    ps = 0.01f;
                Content.m_sizeDelta.x = w / ps;
                Content.m_sizeDelta.y = h / ps;

                float near = cam.nearClipPlane + NearPlane;
                if (cam.orthographic)
                {
                    float os = cam.orthographicSize * 2;
                    float s = os / (float)h;
                    s *= ps;
                    transform.localScale = new Vector3(s, s, s);
                    Vector3 pos = cam.transform.position;
                    Vector3 forward = cam.transform.forward;
                    transform.position = pos + forward * near;
                    transform.forward = forward;
                }
                else
                {
                    float s = 2 / (float)h;
                    float o = MathH.Tan(cam.fieldOfView) / near;
                    s /= o;
                    s *= ps;
                    transform.localScale = new Vector3(s, s, s);
                    Vector3 pos = cam.transform.position;
                    Vector3 forward = cam.transform.forward;
                    transform.position = pos + forward * near;
                    transform.forward = forward;
                }
                transform.rotation = cam.transform.rotation;
            }
        }

        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.HCanvas();
            return Content;
        }
#endif
        //protected void SaveToHCanvas(Core.HGUI.HCanvas ui, bool activeSelf, bool haveChild)
        //{
        //    //ui.DesignSize = DesignSize;
        //    //ui.PauseEvent = PauseEvent;
        //    //ui.Pause = Pause;
        //    //ui.renderQueue = renderQueue;
        //    //SaveToUIElement(ui, activeSelf, haveChild);
        //}
        //public override Core.HGUI.UIElement ToHGUI2(bool activeSelf = false,bool haveChild=true)
        //{
        //    //Core.HGUI.HCanvas can = new Core.HGUI.HCanvas();
        //    //SaveToHCanvas(can, activeSelf, haveChild);
        //    return Content;
        //}
        //public override void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        //{
        //    //SaveToHCanvas(ui as Core.HGUI.HCanvas, activeSelf, false);
        //}
    }
}
