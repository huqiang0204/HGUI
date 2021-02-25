using huqiang.UIEvent;
using huqiang.UIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    [Serializable]
    /// <summary>
    /// ui画布
    /// </summary>
    public class HCanvas : UIElement
    {
        public static List<HCanvas> AllCanvas = new List<HCanvas>();
        public static void RegCanvas(HGUIRender render)
        {
            for (int i = 0; i < AllCanvas.Count; i++)
            {
                if (AllCanvas[i].render == render)
                {
                    render.canvas = AllCanvas[i];
                    return;
                }
            }
            var can = render.canvas;
            if (can == null)
            { 
                can = new HCanvas();
                can.m_sizeDelta = render.DesignSize;
                can.DesignSize = render.DesignSize;
                can.Pause = render.Pause;
                can.name = render.name;
            }
            can.render = render;
            render.canvas = can;
            AllCanvas.Add(can);
        }
        public static void ReleaseCanvas(HGUIRender render)
        {
            for (int i = 0; i < AllCanvas.Count; i++)
            {
                var can = AllCanvas[i];
                if (can.render == render)
                {
                    can.Dispose();
                    return;
                }
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            AllCanvas.Remove(this);
        }
        public override string TypeName =>UIType.HCanvas;
        [HideInInspector]
        [NonSerialized]
        public HGUIRender render;
#if UNITY_EDITOR
        public Vector3 WorldPosition;
#endif
        /// <summary>
        /// 主画布实例
        /// </summary>
        public static HCanvas CurrentCanvas;
        /// <summary>
        /// 默认设计尺寸
        /// </summary>
        public Vector2 DesignSize = new Vector2(1920, 1080);
        /// <summary>
        /// UI元素流水线缓存
        /// </summary>
        HGUIElement[] PipeLine = new HGUIElement[4096];
        /// <summary>
        /// ui元素脚本缓存
        /// </summary>
        UIElement[] scripts = new UIElement[4096];
        /// <summary>
        /// 文本元素缓存
        /// </summary>
        HText[] texts = new HText[2048];
        int point = 0;
        int max;
        int top_txt = 0;
        [NonSerialized]
        /// <summary>
        /// 用户输入事件
        /// </summary>
        public UserAction[] inputs;
        /// <summary>
        /// 暂停用户事件
        /// </summary>
        public bool PauseEvent;
        /// <summary>
        /// 暂停所有更新
        /// </summary>
        public bool Pause;
        public int renderQueue = 3100;
        public float PhysicalScale = 1;
        public HCanvas()
        {
            CurrentCanvas = this;
        }
        /// <summary>
        /// 信息采集
        /// </summary>
        /// <param name="script"></param>
        void Collection(UIElement script, int parent, int index)
        {
            var act = PipeLine[index].active = script.activeSelf;
            if (!act)
                return;
            PipeLine[index].parentIndex = parent;
            var lp = PipeLine[index].localPosition = script.localPosition;
            var lr = PipeLine[index].localRotation = script.localRotation;
            var ls = PipeLine[index].localScale = script.localScale;
            if (parent >= 0)
            {
                var ps = PipeLine[parent].Scale;
                lp.x *= ps.x;
                lp.y *= ps.y;
                lp.z *= ps.z;
                PipeLine[index].Position = PipeLine[parent].Position + PipeLine[parent].Rotation * lp;
                PipeLine[index].Rotation = PipeLine[parent].Rotation * lr;
                PipeLine[index].Scale = ps;
                PipeLine[index].Scale.x *= ls.x;
                PipeLine[index].Scale.y *= ls.y;
                PipeLine[index].Scale.z *= ls.z;
            }
            else//HCanvas
            {
                PipeLine[index].Position = Vector3.zero;
                PipeLine[index].Rotation = Quaternion.identity;
                PipeLine[index].Scale = Vector3.one;
            }
            PipeLine[index].script = script;
            if (script != null)
            {
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
            int c = script.child.Count;
            PipeLine[index].childCount = c;
            int s = point;
            point += c;
            PipeLine[index].childOffset = s;
            for (int i = 0; i < c; i++)
            {
                Collection(script.child[i], index, s);
                s++;
            }
        }
        /// <summary>
        /// 更新内容包含:UI流水线采集,UI MainUpdate函数执行,UI Populate函数执行,文本更新,合批处理,应用网格
        /// </summary>
        public void UpdateMesh()
        {
            if (Pause)
                return;
            MatCollector.renderQueue = renderQueue;
            LateFrame++;
            point = 1;
            max = 0;
            top_txt = 0;
            Collection(this, -1, 0);
            for (int i = 1; i < max; i++)//跳过HCanvas
            {
                var scr = scripts[i];
                if (scr.LateFrame != LateFrame)
                {
                    scr.LateFrame = LateFrame;
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    { 
                        var grap = scr as HGraphics;
                        if (grap != null)
                            grap.m_dirty = true;
                    }
#endif
                    scr.MainUpdate();
                }
                else
                {
                    Debug.Log("脚本重复更新");
                }
            }
            Collection(this, -1, 0);
            for (int i = 0; i < top_txt; i++)
            {
                texts[i].Populate();
            }
        }
        public void ApplyMeshRenderer(MeshFilter meshFilter, MeshRenderer renderer)
        {
            if (meshFilter == null)
                return;
            if (meshFilter != null)
            {
                Mesh mesh;
#if UNITY_EDITOR
                if (Application.isPlaying)
                    mesh = meshFilter.mesh;
                else mesh = meshFilter.sharedMesh;
#else
               mesh = meshFilter.mesh;
#endif
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

                    var submesh = MatCollector.submesh;
                    if (submesh != null)
                    {
                        mesh.subMeshCount = submesh.Count;
                        for (int i = 0; i < submesh.Count; i++)
                        {
                            var tri = submesh[i];
                            //for (int k = 0; k < tri.Length; k++)
                            //    if (tri[k] > vertex.Count)
                            //    {
                            //        Debug.LogError(k + ":" + tri[k]);
                            //        tri[k] = 0;
                            //    }
                            mesh.SetTriangles(tri, i);
                        }
                    }
                }
            }
            if (renderer != null)
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                    renderer.materials = MatCollector.GenerateMaterial();   //这里会产生一次GC
                else renderer.sharedMaterials = MatCollector.GenerateMaterial();   //这里会产生一次GC
#else
                renderer.materials = MatCollector.GenerateMaterial();   //这里会产生一次GC
#endif
            }
        }

        #region 鼠标和触屏事件
#if UNITY_EDITOR
        public void DispatchUserAction(Touch[] touches)
        {
            DispatchTouch(touches);
            if (PauseEvent)
                return;
            PhysicalScale = UISystem.PhysicalScale;
            if (render != null)
                if (render.renderMode == RenderMode.WorldSpace)
                    PhysicalScale = 1;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] != null)
                {
                    try
                    {
                        inputs[i].PhysicalScale = PhysicalScale;
                        if (inputs[i].IsActive)
                            inputs[i].Dispatch(PipeLine);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.StackTrace);
                    }
                }
            }
            if (inputs.Length > 0)
                GestureEvent.Dispatch(inputs);
        }
#endif
        /// <summary>
        /// 派发用户输入指令信息
        /// </summary>
        public void DispatchUserAction()
        {
#if UNITY_STANDALONE_WIN
            DispatchWin(Input.touches);
#elif UNITY_EDITOR
            DispatchMouse();
#elif UNITY_IPHONE || UNITY_ANDROID
            DispatchTouch(Input.touches);
#else
            DispatchMouse();
#endif
            if (PauseEvent)
                return;
            PhysicalScale = UISystem.PhysicalScale;
            if (render != null)
                if (render.renderMode == RenderMode.WorldSpace)
                    PhysicalScale = 1;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] != null)
                {
#if DEBUG
                    try
                    {
#endif
                        inputs[i].PhysicalScale = PhysicalScale;
                        if (inputs[i].IsActive)
                            inputs[i].Dispatch(PipeLine);
#if DEBUG
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.StackTrace);
                    }
#endif
                }
            }
            if (inputs.Length > 0)
                GestureEvent.Dispatch(inputs);
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
        void DispatchTouch(Touch[] touches)
        {
            if (inputs == null)
            {
                inputs = new UserAction[10];
                for (int i = 0; i < 10; i++)
                    inputs[i] = new UserAction(i);
            }
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
        void DispatchWin(Touch[] touches)
        {
            if (inputs == null)
            {
                inputs = new UserAction[10];
                for (int i = 0; i < 10; i++)
                    inputs[i] = new UserAction(i);
            }
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
        /// <summary>
        /// 当窗口失去焦点时停止所有更新用来节省cpu性能
        /// </summary>
        /// <param name="focus"></param>
        public void OnApplicationFocus(bool focus)
        {
            Pause = !focus;
        }
#endregion
#region UI绘制与合批
        public void Batch()
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
            vertex.Clear();
            uv.Clear();
            uv1.Clear();
            uv2.Clear();
            uv3.Clear();
            uv4.Clear();
            colors.Clear();
            HBatch.Batch(this, PipeLine);
        }
        [NonSerialized]
        internal List<Vector3> vertex = new List<Vector3>();
        [NonSerialized]
        internal List<Vector2> uv = new List<Vector2>();
        [NonSerialized]
        /// <summary>
        /// picture index
        /// </summary>
        internal List<Vector2> uv1 = new List<Vector2>();
        [NonSerialized]
        /// <summary>
        /// cut rect
        /// </summary>
        internal List<Vector2> uv2 = new List<Vector2>();
        [NonSerialized]
        /// <summary>
        /// uv tiling
        /// </summary>
        internal List<Vector2> uv3 = new List<Vector2>();
        [NonSerialized]
        /// <summary>
        /// uv offset
        /// </summary>
        internal List<Vector2> uv4 = new List<Vector2>();
        [NonSerialized]
        internal List<Color32> colors = new List<Color32>();
        [NonSerialized]

        internal MaterialCollector MatCollector = new MaterialCollector();
        /// <summary>
        /// 将屏幕坐标转换为画布坐标
        /// </summary>
        /// <param name="mPos">屏幕坐标</param>
        /// <returns></returns>
        public Vector2 ScreenToCanvasPos(Vector2 mPos)
        {
            if (render == null)
                return new Vector2(-100000, -100000);
            if (render.renderMode == RenderMode.WorldSpace)
            {
                Vector3 a = new Vector3(-10000f, -10000f, 0);
                Vector3 b = new Vector3(0, 10000f, 0);
                Vector3 c = new Vector3(10000, -10000, 0);
                var trans = render.trans;
                var pos = trans.position;
                var qt = trans.rotation;
                a = qt * a + pos;
                b = qt * b + pos;
                c = qt * c + pos;//得到世界坐标的三角面
                var cam = Camera.main;
                var v = cam.ScreenToWorldPoint(mPos);
                var f = cam.transform.forward;
                Vector3 p = Vector3.zero;
                if (huqiang.Physics.IntersectTriangle(ref v, ref f, ref a, ref b, ref c, ref p))
                {
                    var iq = Quaternion.Inverse(qt);
                    p -= pos;
                    p = iq * p;
                    var ls = trans.localScale;
                    p.x /= ls.x;
                    p.y /= ls.y;
                    return new Vector2(p.x, p.y);
                }
                return new Vector2(-100000, -100000);
            }
            else
            {
                float w = Screen.width;
                w *= 0.5f;
                float h = Screen.height;
                h *= 0.5f;
                var cPos = Vector2.zero;
                cPos.x = mPos.x - w;
                cPos.y = mPos.y - h;
                float ps = UISystem.PhysicalScale;
                cPos.x /= ps;
                cPos.y /= ps;
                return cPos;
            }
        }
#endregion
    }
}
