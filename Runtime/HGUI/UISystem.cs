using huqiang.Core.UIData;
using huqiang.UIEvent;
using huqiang.UIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class UISystem:MonoBehaviour
    {
        static bool Changed;
        static float _physicalScale = 1;
        public static UISystem Instance;
        protected static ThreadMission thread;// = new ThreadMission("UI");
        public static float PhysicalScale
        {
            get => _physicalScale;
            set
            {
                _physicalScale = value;
                Changed = true;
            }
        }
        public ScaleType scaleType;
        /// <summary>
        /// 物理尺寸缩放的贝塞尔曲线,x=英寸,y=比例
        /// </summary>
        public Vector2 A = new Vector2(4, 0.9f);//贝塞尔曲线起点
        public Vector2 B = new Vector2(6, 1f);
        public Vector2 C = new Vector2(8, 1.2f);
        public Vector2 D = new Vector2(10, 1.3f);//贝塞尔曲线终点
        public void Awake()
        {
            Instance = this;
        }
        public void Start()
        {
            Font.textureRebuilt += HTextGenerator.FontTextureRebuilt;
        }
        public void OnDestroy()
        {
            Font.textureRebuilt -= HTextGenerator.FontTextureRebuilt;
            HTextLoader.fonts.Clear();
        }
        public virtual void Update()
        {
            AnimationManage.Manage.Update();
            if (UIPage.CurrentPage != null)
                UIPage.CurrentPage.Update(UserAction.TimeSlice);
            UINotify.UpdateAll(UserAction.TimeSlice);
            UserAction.Update();
            Keyboard.InfoCollection();
            DispatchUserAction();
            CheckSize();
            ThreadMission.ExtcuteMain();
        }
        List<float> distance = new List<float>();
        void DispatchUserAction()
        {
#if UNITY_EDITOR
            if (GestureSimulation)
                UpdateTouch();
#endif
            var cvs = HGUIRender.AllCanvas;
            if (cvs.Count > 1)
            {
                distance.Clear();
                var trans = Camera.main.transform.position;
                int c = cvs.Count;
                for (int i = 0; i < c; i++)//计算UI与相机的距离
                {
                    var pos = cvs[i].transform.position;
                    if (cvs[i].camera != null)
                    {
                        var p = cvs[i].camera.transform.position;
                        float x = pos.x - p.x;
                        float y = pos.y - p.y;
                        float z = pos.z - p.z;
                        distance.Add(x * x + y * y + z * z);
                    }
                    else//主相机
                    {
                        float x = pos.x - trans.x;
                        float y = pos.y - trans.y;
                        float z = pos.z - trans.z;
                        distance.Add(x * x + y * y + z * z);
                    }
                }
                for (int i = 0; i < c; i++)//根据距离进行画布排序
                {
                    float d = distance[i];
                    int t = i;
                    for (int j = i + 1; j < c; j++)
                    {
                        if (distance[j] < d)
                        {
                            d = distance[j];
                            t = j;
                        }
                    }
                    distance[t] = distance[i];
                    distance[i] = d;
                    var h = cvs[i];
                    cvs[i] = cvs[t];
                    cvs[t] = h;
                }
                for (int i = 0; i < c; i++)//派发用户事件，距离最近的优先派发
                {
                    HCanvas.CurrentCanvas = cvs[i].canvas;
#if UNITY_EDITOR
                    if (GestureSimulation)
                        cvs[i].canvas.DispatchUserAction(touches);
                    else
#endif
                        cvs[i].canvas.DispatchUserAction();
                }
            }
            else if (cvs.Count > 0)
            {
                HCanvas.CurrentCanvas = cvs[0].canvas;
                if (cvs[0].canvas != null)
                {
#if UNITY_EDITOR
                    if (GestureSimulation)
                        cvs[0].canvas.DispatchUserAction(touches); 
                    else
#endif
                        cvs[0].canvas.DispatchUserAction();
                }
            }
        }
        int ScreenWidth = 0;
        int ScreenHeight = 0;
        void CheckSize()
        {
            int w = Screen.width;
            int h = Screen.height;
            if (ScreenWidth != w | ScreenHeight != h|Changed)
            {
                ScreenWidth = w;
                ScreenHeight = h;
                Changed = false;
                OverCamera();
                var cvs = HGUIRender.AllCanvas;
                for (int i = 0; i < cvs.Count; i++)
                {
                    if (cvs[i].renderMode != RenderMode.WorldSpace)
                    {
                        cvs[i].ApplyToCamera();
                        UIElement.ResizeChild(cvs[i].canvas);
                    }
                }
                if (UIPage.CurrentPage != null)
                    UIPage.CurrentPage.ReSize();
                if (UINotify.CurrentNotify != null)
                    UINotify.CurrentNotify.ReSize();
                if (UIMenu.CurrentMenu != null)
                    UIMenu.CurrentMenu.ReSize();
            }
        }
        /// <summary>
        /// 暂停所有更新
        /// </summary>
        public bool Pause;
        public bool SubBatch;
        int mainFrame = 0;
        int subFrame = 0;
        public virtual void LateUpdate()
        {
            if (Pause)
                return;
            if(SubBatch)
            {
                if (subFrame == mainFrame)
                {
                    var cvs = HGUIRender.AllCanvas;
                    for (int i = 0; i < cvs.Count; i++)
                        cvs[i].UpdateMesh();
                    HTextGenerator.RebuildUV();
                    HTextGenerator.End();
                    for (int i = 0; i < cvs.Count; i++)
                    {
                        cvs[i].Apply();
                    }
                    mainFrame++;
                    if (thread == null)
                        thread = ThreadMission.CreateMission("UI");
                    thread.AddSubMission((o) =>
                    {
                        try
                        {
                            var tmp = HGUIRender.AllCanvas;
                            for (int i = 0; i < tmp.Count; i++)
                            {
                                tmp[i].Batch();
                            }
                        }catch(Exception ex)
                        {
                            Debug.LogError(ex.StackTrace);
                        }
                        subFrame++;
                    }, this);
                }
            }
            else
            {
                var cvs = HGUIRender.AllCanvas;
                for (int i = 0; i < cvs.Count; i++)
                    cvs[i].UpdateMesh();
                HTextGenerator.RebuildUV();
                HTextGenerator.End();
                for (int i = 0; i < cvs.Count; i++)
                {
                    cvs[i].Batch();
                    cvs[i].Apply();
                }
            }
        }
        /// <summary>
        /// 默认设计尺寸
        /// </summary>
        public Vector2 DesignSize = new Vector2(1920, 1080);
        public void OverCamera()
        {
            var cam = Camera.main;
            if(cam==null)
            {
                _physicalScale = 1;
                return;
            }
            float w = cam.pixelWidth;
            float h = cam.pixelHeight;

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
                    ps = MathH.BezierPoint(r, ref A,ref B,ref C,ref D).y;
                }
                _physicalScale = h / DesignSize.y;
#else
            if (scaleType == ScaleType.FillX)
            {
                _physicalScale = w / DesignSize.x;
            }
            else if (scaleType == ScaleType.FillY)
            {
                _physicalScale = h / DesignSize.y;
            }
#endif
        }
        public void OnApplicationFocus(bool focus)
        {
            if (focus)
                HText.DirtyAll();
        }
#if UNITY_EDITOR
        /// <summary>
        /// 手势模拟，用于没有触控屏的电脑，进行调试
        /// </summary>
        public bool GestureSimulation;
        int pressIndex = -1;
        Touch[] touches;
        private void OnGUI()
        {
            if (touches != null)
                for (int i = 0; i < 10; i++)
                {
                    if (touches[i].fingerId >= 0)
                        DrawButton(ref touches[i]);
                }
        }
        void UpdateTouch()
        {
            if (touches == null)
            {
                touches = new Touch[10];
                for (int i = 0; i < 10; i++)
                    touches[i].fingerId = -1;
            }
            for (int i = 0; i < 10; i++)
            {
                if (touches[i].fingerId >= 0)
                    if (touches[i].phase == TouchPhase.Ended)
                        touches[i].fingerId = -1;
            }
            if (Input.GetMouseButton(0))
            {
                MoveTouch();
            }
            if (Input.GetMouseButtonDown(0))
            {
                PressTouch();
            }
            if (Input.GetMouseButtonUp(0))
            {
                pressIndex = -1;
            }
            if (Input.GetMouseButtonDown(1))
            {
                UpTouch();
            }
        }
        void PressTouch()
        {
            Vector2 pos = Input.mousePosition;
            for (int i = 0; i < 10; i++)
            {
                if (touches[i].fingerId >= 0)
                {
                    float x = pos.x - touches[i].position.x;
                    float y = pos.y - touches[i].position.y;
                    if (x * x + y * y < 900)//在圆内
                    {
                        pressIndex = i;
                        touches[i].phase = TouchPhase.Began;
                        return;
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (touches[i].fingerId < 0) 
                {
                    touches[i].fingerId = i;
                    pressIndex = i;
                    touches[i].phase = TouchPhase.Began;
                    touches[i].position = Input.mousePosition;
                    return;
                }
            }
        }
        void UpTouch()
        {
            Vector2 pos = Input.mousePosition;
            for (int i = 0; i < 10; i++)
            {
                if (touches[i].fingerId >= 0)
                {
                    float x = pos.x - touches[i].position.x;
                    float y = pos.y - touches[i].position.y;
                    if (x * x + y * y < 900)//在圆内
                    {
                        touches[i].phase = TouchPhase.Ended;
                        return;
                    }
                }
            }
        }
        void MoveTouch()
        {
            Vector2 pos = Input.mousePosition;
            for(int i=0;i<10;i++)
            {
                if(i==pressIndex)
                {
                    if(touches[i].position==pos)
                    {
                        touches[i].phase = TouchPhase.Stationary;
                    }
                    else
                    {
                        touches[i].position = pos;
                        touches[i].phase = TouchPhase.Moved;
                    }
                }
                else
                {
                    touches[i].phase = TouchPhase.Stationary;
                }
            }
        }
        void DrawButton(ref Touch touch)
        {
            Rect rect = new Rect();
            var pos = touch.position;
            pos.y = Screen.height - pos.y-30;
            pos.x -= 30;
            rect.position = pos;
            rect.size = new Vector2(60,60);
            GUI.Button(rect,touch.fingerId.ToString());
        }
#endif
    }
}
