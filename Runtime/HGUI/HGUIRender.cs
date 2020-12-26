using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// HGUI呈现器
    /// </summary>
    public class HGUIRender:MonoBehaviour
    {
        internal static List<HGUIRender> AllCanvas = new List<HGUIRender>();
        [HideInInspector]
        public string uid;
        [HideInInspector]
        public int ContextID;
        public string GetGuid()
        {
            if (uid == null|uid=="")
                uid = Guid.NewGuid().ToString();
            return uid;
        }
        [HideInInspector]
        public Transform trans;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        [HideInInspector]
        public HCanvas canvas;

        public ScaleType scaleType;
        /// <summary>
        /// 目标相机,如果为空则使用主相机
        /// </summary>
        public Camera camera;
        /// <summary>
        /// 默认设计尺寸
        /// </summary>
        public Vector2 DesignSize = new Vector2(1920, 1080);
        /// <summary>
        /// 距离相机镜头的距离
        /// </summary>
        public float NearPlane = 0f;
        public RenderMode renderMode;
        /// <summary>
        /// 暂停用户事件
        /// </summary>
        public bool PauseEvent;
        /// <summary>
        /// 暂停所有更新
        /// </summary>
        public bool Pause;

        public int renderQueue = 3100;
        public void Awake()
        {
            HCanvas.RegCanvas(this);
            AllCanvas.Add(this);
        }
        public virtual void Start()
        {
            trans = transform;
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }
        protected virtual void OnDestroy()
        {
            AllCanvas.Remove(this);
            HCanvas.ReleaseCanvas(this);
            canvas = null;
            ContextID = 0;
        }
        public virtual void UpdateMesh()
        {
            if (canvas != null)
            {
#if UNITY_EDITOR
                canvas.WorldPosition = transform.position;
#endif
                canvas.localPosition = Vector3.zero;
                canvas.UpdateMesh();
                ApplyToCamera();
            }
        }
        public void Batch()
        {
            if (canvas != null)
            {
                canvas.Pause = Pause;
                canvas.PauseEvent = PauseEvent;
                canvas.renderQueue = renderQueue;
                canvas.Batch();
            }
        }
        public void Apply()
        {
            if (canvas != null)
            {
                canvas.ApplyMeshRenderer(meshFilter, meshRenderer);
            }
        }
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
                float ps = UISystem.PhysicalScale;
                if (ps < 0.01f)
                    ps = 0.01f;
                canvas.m_sizeDelta.x = w / ps;
                canvas.m_sizeDelta.y = h / ps;
                float near = cam.nearClipPlane + NearPlane;
                if (cam.orthographic)
                {
                    float os = cam.orthographicSize * 2;
                    float s = os / (float)h;
                    s *= ps;
                    canvas.localScale = trans.localScale = new Vector3(s, s, s);
                    Vector3 pos = cam.transform.position;
                    Vector3 forward = cam.transform.forward;
                    trans.position = pos + forward * near;
                    trans.forward = forward;
                }
                else
                {
                    float s = 2 / (float)h;
                    float o = MathH.Tan(cam.fieldOfView) / near;
                    s /= o;
                    s *= ps;
                    canvas.localScale = trans.localScale = new Vector3(s, s, s);
                    Vector3 pos = cam.transform.position;
                    Vector3 forward = cam.transform.forward;
                    trans.position = pos + forward * near;
                    trans.forward = forward;
                }
                trans.rotation = cam.transform.rotation;
            }
        }
        #region
#if UNITY_EDITOR
        public huqiang.Helper.HGUI.HCanvas OldUI;
        public void Refresh()
        {
            if (Application.isPlaying)
            {
                return;
            }
            trans = transform;
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            var can = UIElement.FindInstance(ContextID) as HCanvas;
            if (can == null)
            {
                if (OldUI != null)
                {
                    var oh = OldUI.GetComponent<huqiang.Helper.HGUI.HCanvas>();
                    if (oh != null)
                    {
                        NearPlane = oh.NearPlane;
                        UIElement.DisposeAll();
                        can = oh.ToHGUI2(true) as HCanvas;
                        ContextID = can.id;
                    }
                }
            }
            if (can != null)
            {
                canvas = can;
                canvas.localPosition = Vector3.zero;
                canvas.WorldPosition = transform.position;
                can.UpdateMesh();
                HTextGenerator.RebuildUV();
                HTextGenerator.End();
                can.Batch();
                can.ApplyMeshRenderer(meshFilter, meshRenderer);
                ApplyToCamera();
            }
        }
#endif
        #endregion
    }
}
