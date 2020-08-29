using System;
using System.Collections.Generic;
using huqiang;
using huqiang.Data;
using huqiang.UIEvent;
using huqiang.UIModel;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class HCanvas:UIElement
    {
        class TempBuffer
        {
            public int Max;
            public TempBuffer(int len=4096)
            {
                Max = len;
                PipeLine = new HGUIElement[len];
                scripts = new UIElement[len];
            }
            public HGUIElement[] PipeLine;
            public UIElement[] scripts;
            public List<Vector3> vertex = new List<Vector3>();
            public List<Vector2> uv = new List<Vector2>();
            public List<Vector2> uv1 = new List<Vector2>();
            public List<Vector2> uv2 = new List<Vector2>();
            public List<Vector2> uv3 = new List<Vector2>();
            public List<Vector2> uv4 = new List<Vector2>();
            public List<Color32> colors = new List<Color32>();
        }
        //protected static ThreadMission thread = new ThreadMission("UI");
        public Camera camera;
        public Vector2 DesignSize = new Vector2(1920, 1080);
        [Range(0.1f, 3)]
        public float PhysicalScale = 1;
        public float NearPlane = 0.01f;
        public Vector2 A = new Vector2(4,0.9f);//贝塞尔曲线起点
        public Vector2 B = new Vector2(6,1f);
        public Vector2 C = new Vector2(8,1.2f);
        public Vector2 D = new Vector2(10,1.3f);//贝塞尔曲线终点
        public RenderMode renderMode;
        public static HCanvas MainCanvas;
        //public bool SubBatch;//是否开启子线程合批处理,开启后画面会延迟一帧
        //LoopBuffer<HGUIElement[]> loopBuffer = new LoopBuffer<HGUIElement[]>(3);
        QueueBuffer<TempBuffer> Main,Sub;
        HGUIElement[] PipeLine = new HGUIElement[4096];
        UIElement[] scripts = new UIElement[4096];
        HText[] texts = new HText[2048];
        int point = 0;
        int max;
        int top_txt=0;
        public UserAction[] inputs;
        public bool PauseEvent;
        protected virtual void Start()
        {
            MainCanvas = this;
            Font.textureRebuilt += FontTextureRebuilt;
            Main = new QueueBuffer<TempBuffer>();
            Sub = new QueueBuffer<TempBuffer>();
        }
        bool ftr;
        void FontTextureRebuilt(Font font)
        {
            ftr = true;
        }
        protected virtual void OnDestroy()
        {
            Font.textureRebuilt -= FontTextureRebuilt;
        }
        /// <summary>
        /// 信息采集
        /// </summary>
        /// <param name="trans"></param>
        void Collection(Transform trans, int parent, int index)
        {
            PipeLine[index].parentIndex = parent;
            PipeLine[index].localPosition = trans.localPosition;
            PipeLine[index].localRotation = trans.localRotation;
            PipeLine[index].localScale = trans.localScale;
            PipeLine[index].trans = trans;
            var act = PipeLine[index].active = trans.gameObject.activeSelf;
            var script = trans.GetComponent<UIElement>();
            PipeLine[index].script = script;
            bool mask = false;
            if (script != null)
            {
                mask = script.Mask;
                script.PipelineIndex = index;
                scripts[max] = script;
                max++;
                var txt = script as HText;
                if (txt != null)
                {
                    texts[top_txt] = txt;
                    top_txt++;
                }
            }
            PipeLine[index].script = script;
            int c = trans.childCount;
            PipeLine[index].childCount = c;
            int s = point;
            point += c;
            PipeLine[index].childOffset = s;
            if (act)
                for (int i = 0; i < c; i++)
                {
                    Collection(trans.GetChild(i), index, s);
                    s++;
                }
        }
  
        protected virtual void Update()
        {
            MainCanvas = this;
            UserAction.Update();
            Keyboard.InfoCollection();
            DispatchUserAction();
            AnimationManage.Manage.Update();
            if (UIPage.CurrentPage != null)
                UIPage.CurrentPage.Update(UserAction.TimeSlice);
            UINotify.UpdateAll(UserAction.TimeSlice);
            //Keyboard.Dispatch();
            //InputCaret.UpdateCaret();
            CheckSize();
            ThreadMission.ExtcuteMain();
        }
        void CheckSize()
        {
            float w = m_sizeDelta.x;
            float h = m_sizeDelta.y;
            if (Scale.ScreenWidth != w | Scale.ScreenHeight != h)
            {
                Scale.ScreenWidth = w;
                Scale.ScreenHeight = h;
                if (UIPage.CurrentPage != null)
                    UIPage.CurrentPage.ReSize();
                if (UIMenu.CurrentMenu != null)
                    UIMenu.CurrentMenu.ReSize();
            }
        }
        private void LateUpdate()
        {
            point = 1;
            max = 0;
            top_txt = 0;
            Collection(transform, -1, 0);
            for (int i = 0; i < max; i++)
                scripts[i].MainUpdate();
            if (ftr)//纹理被改变了,需要重新计算
            {
                ftr = false;
                HText.DirtyAll();
            }
            for (int i = 0; i < top_txt; i++)
                texts[i].Populate();
            Batch();
            ApplyMeshRenderer();
            ApplyToCamera();
        }
        MeshFilter meshFilter;
        MeshRenderer renderer;
        void ApplyMeshRenderer()
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var mesh = meshFilter.mesh;
                if (mesh == null)
                {
                    mesh = new Mesh();
                    meshFilter.mesh = mesh;
                }
                mesh.Clear();
                if (vertex != null)
                {
                    mesh.SetVertices(vertex);
                    mesh.SetUVs(0, uv);
                    mesh.SetUVs(1, uv1);
                    mesh.SetUVs(2, uv2);
                    mesh.SetUVs(3, uv3);
                    mesh.SetUVs(4, uv4);
                    mesh.SetColors(colors);

                    var submesh= MatCollector.submesh;
                    if (submesh != null)
                    {
                        mesh.subMeshCount = submesh.Count;
                        for (int i = 0; i < submesh.Count; i++)
                            mesh.SetTriangles(submesh[i], i);
                    }
                }
            }
            if (renderer == null)
                renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.materials = MatCollector.GenerateMaterial();   //这里会产生一次GC
        }
        void ApplyToCamera()
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
            if(cam!=null)
            {
                int w = cam.pixelWidth;
                int h = cam.pixelHeight;


#if UNITY_IPHONE || UNITY_ANDROID
                float ss = Mathf.Sqrt(w * w + h * h);
#if UNITY_EDITOR
                ss /= 334;
#else
               ss/=Screen.dpi;
#endif
                float ps = 1;
                float r = (ss - A.x) / (D.x - A.x);
                if (r < 0)
                {
                    ps = A.y;
                }
                else if (r > 1)
                {
                    ps = D.y;
                }
                else
                {
                    ps = MathH.BezierPoint(r,ref A,ref B,ref C,ref D).y;
                }
                PhysicalScale = ps;
#else
                if(scaleType==ScaleType.FillX)
                {
                    PhysicalScale = w / DesignSize.x;
                }else if(scaleType==ScaleType.FillY)
                {
                    PhysicalScale = h / DesignSize.y;
                }
                float ps = PhysicalScale;
                if (ps < 0.01f)
                    ps = 0.01f;
#endif
                m_sizeDelta.x = w / ps;
                m_sizeDelta.y = h / ps;
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
#region 鼠标和触屏事件
        /// <summary>
        /// 派发用户输入指令信息
        /// </summary>
        void DispatchUserAction()
        {
#if UNITY_STANDALONE_WIN
            DispatchWin();
#elif UNITY_EDITOR
            DispatchMouse();
#elif UNITY_IPHONE || UNITY_ANDROID
            DispatchTouch();
#else
            DispatchMouse();
#endif
            if (PauseEvent)
                return;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] != null)
                {
#if DEBUG
                    try
                    {
#endif
                        if (inputs[i].IsActive)
                            inputs[i].Dispatch(PipeLine);
#if DEBUG
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.StackTrace);
                    }
#endif
                }
            }
            if (inputs.Length > 0)
                GestureEvent.Dispatch(new List<UserAction>(inputs));
        }
        /// <summary>
        /// 派发鼠标事件
        /// </summary>
        void DispatchMouse()
        {
            if (inputs == null)
            {
                inputs = new UserAction[1];
                inputs[0] = new UserAction(0);
            }
            var action = inputs[0];
            action.LoadMouse();
        }
        /// <summary>
        /// 派发Touch事件
        /// </summary>
        void DispatchTouch()
        {
            if (inputs == null)
            {
                inputs = new UserAction[10];
                for (int i = 0; i < 10; i++)
                    inputs[i] = new UserAction(i);
            }
            var touches = Input.touches;
            for (int i = 0; i < 10; i++)
            {
                if (touches != null)
                {
                    for (int j = 0; j < touches.Length; j++)
                    {
                        if (touches[j].fingerId == i)
                        {
                            inputs[i].LoadFinger(ref touches[j]);
                            inputs[i].IsActive = true;
                            goto label;
                        }
                    }
                }
                if (inputs[i].isPressed)
                {
                    inputs[i].isPressed = false;
                    inputs[i].IsLeftButtonUp = true;
                }
                else inputs[i].IsActive = false;
                label:;
            }
        }
        /// <summary>
        /// 派发鼠标和Touch混合事件
        /// </summary>
        void DispatchWin()
        {
            if (inputs == null)
            {
                inputs = new UserAction[10];
                for (int i = 0; i < 10; i++)
                    inputs[i] = new UserAction(i);
            }
            var touches = Input.touches;
            for (int i = 0; i < 10; i++)
            {
                int id = i;
                if (touches != null)
                {

                    for (int j = 0; j < touches.Length; j++)
                    {
                        if (touches[j].fingerId == id)
                        {
                            inputs[id].LoadFinger(ref touches[j]);
                            inputs[id].IsActive = true;
                            goto label;
                        }
                    }
                }
                if (touches.Length > 0 & inputs[id].isPressed)
                {
                    inputs[id].isPressed = false;
                    inputs[id].IsLeftButtonUp = true;
                }
                else inputs[id].IsActive = false;
                label:;
            }
            if (touches.Length == 0)
            {
                var action = inputs[0];
                action.LoadMouse();
            }
        }
        public void ClearAllAction()
        {
            if (inputs != null)
                for (int i = 0; i < inputs.Length; i++)
                    inputs[i].Clear();
        }
#endregion
#region UI绘制与合批
        void Batch()
        {
            int len = max;
            if (scripts != null)
            {
                for (int i = 0; i < len; i++)
                {
                    var grap = scripts[i] as HGraphics;
                    if (grap != null)
                        grap.UpdateMesh();
                }
            }
            ClearMesh();
            HBatch.Batch(this, PipeLine);
        }
        void SubBatch(object obj)
        {
            var m = Sub.Dequeue();
            if(m!=null)
            {
                PipeLine = m.PipeLine;
                scripts = m.scripts;
                vertex = m.vertex;
                uv = m.uv;
                uv1 = m.uv1;
                uv2 = m.uv2;
                uv3 = m.uv3;
                colors = m.colors;
                Batch();
                Main.Enqueue(m);
            }
        }
        internal List<Vector3> vertex = new List<Vector3>();
        internal List<Vector2> uv = new List<Vector2>();
        internal List<Vector2> uv1 = new List<Vector2>();
        internal List<Vector2> uv2 = new List<Vector2>();
        internal List<Vector2> uv3 = new List<Vector2>();
        internal List<Vector2> uv4 = new List<Vector2>();
        internal List<Color32> colors = new List<Color32>();

        internal MaterialCollector MatCollector = new MaterialCollector();
        void ClearMesh()
        {
            vertex.Clear();
            uv.Clear();
            uv1.Clear();
            uv2.Clear();
            uv3.Clear();
            uv4.Clear();
            colors.Clear();
        }
#endregion
#region 编辑器状态刷新网格
#if UNITY_EDITOR
        public void Refresh()
        {
            if (Application.isPlaying)
            {
                return;
            }
            MainCanvas = this;
            point = 1;
            max = 0;
            top_txt = 0;
            Collection(transform, -1, 0);
            int len = max;
            for (int i = 0; i < len; i++)
            {
                var grap = scripts[i] as HGraphics;
                if (grap != null)
                {
                    grap.m_dirty = true;
                    grap.m_vertexChange = true;
                }
            }
            for (int i = 0; i < max; i++)
                Resize(scripts[i], false);
            for (int i = 0; i < max; i++)
                scripts[i].MainUpdate();
            for (int i = 0; i < top_txt; i++)
                texts[i].Populate();
            Batch();
            ApplyToCamera();
            ApplyShareMesh();
        }
        void ApplyShareMesh()
        {
            var mf = GetComponent<MeshFilter>();
            if (mf != null)
            {
                var mesh = mf.sharedMesh;
                if (mesh == null)
                {
                    mesh = new Mesh();
                    mf.mesh = mesh;
                }
                mesh.Clear();
                if (vertex != null)
                {
                    mesh.SetVertices(vertex);
                    mesh.SetUVs(0, uv);
                    mesh.SetUVs(1, uv1);
                    mesh.SetUVs(2, uv2);
                    mesh.SetUVs(3, uv3);
                    mesh.SetUVs(4, uv4);
                    mesh.SetColors(colors);
                    var submesh = MatCollector.submesh;
                    if (submesh != null)
                    {
                        mesh.subMeshCount = submesh.Count;
                        for (int i = 0; i < submesh.Count; i++)
                            mesh.SetTriangles(submesh[i], i);
                    }
                }
            }
            var mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                //这里会产生一次GC
                mr.sharedMaterials = MatCollector.GenerateMaterial();
            }
               
        }
#endif
#endregion
    }
}
