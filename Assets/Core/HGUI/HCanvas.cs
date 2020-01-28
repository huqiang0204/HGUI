using System;
using System.Collections.Generic;
using huqiang;
using huqiang.UIEvent;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class HCanvas:UIElement
    {
        public Camera camera;
        public RenderMode renderMode;
        HGUIElement[] PipeLine = new HGUIElement[4096];
        UIElement[] scripts = new UIElement[4096];
        int point = 0;
        int max;
        public UserAction[] inputs;
        public bool PauseEvent;
        public UserAction.InputType inputType = UserAction.InputType.OnlyMouse;
        private void Start()
        {
            Font.textureRebuilt += (o) => {
                TxtCollector.GenerateTexture(true);
            };
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
            PipeLine[index].active = trans.gameObject.activeSelf;
            var script= trans.GetComponent<UIElement>();
            PipeLine[index].script = script;
            bool mask = false;
            if (script != null)
            {
                mask = script.Mask;
                script.PipelineIndex = index;
                scripts[max] = script;
                max++;
                if (mask)
                    TxtCollector.Next();
                var txt = script as HText;
                if (txt != null)
                    TxtCollector.AddText(txt);
            }
            PipeLine[index].script = script;
            int c = trans.childCount;
            PipeLine[index].childCount = c;
            int s = point;
            point += c;
            PipeLine[index].childOffset = s;
            for (int i = 0; i < c; i++)
            {
                Collection(trans.GetChild(i), index, s);
                s++;
            }
            if(mask)
                TxtCollector.Back();
        }
  
        private void Update()
        {
            CheckSize();
            UserAction.Update();
            Keyboard.InfoCollection();
            DispatchUserAction();
            InputCaret.UpdateCaret();
            if (UIPage.CurrentPage != null)
                UIPage.CurrentPage.Update(UserAction.TimeSlice);
            MainMission();
            ThreadMission.ExtcuteMain();
      
        }
        private void LateUpdate()
        {
           TxtCollector.GenerateTexture(false);
            SubMission(null);
            ApplyMeshRenderer();
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
                    mesh.vertices = vertex.ToArray();
                    mesh.uv = uv.ToArray();
                    mesh.uv2 = uv1.ToArray();
                    mesh.uv3 = uv2.ToArray();
                    mesh.uv4 = uv3.ToArray();
                    mesh.uv5 = uv4.ToArray();
                    mesh.colors32 = colors.ToArray();
                   
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
                renderer.materials = MatCollector.GenerateMaterial();
        }
        void CheckSize()
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
                m_sizeDelta.x = w;
                m_sizeDelta.y = h;
                if(cam.orthographic)
                {
                    float os = cam.orthographicSize * 2;
                    float s = os / (float)h;
                    transform.localScale = new Vector3(s, s, s);
                    Vector3 pos = cam.transform.position;
                    Vector3 forward = cam.transform.forward;
                    transform.position = pos + forward;
                    transform.forward = forward;
                }
                else
                {
                    float near = cam.nearClipPlane + 0.01f;
                    float s = 2 / (float)h;
                    float o = MathH.Tan(cam.fieldOfView) / near;
                    s /= o;
                    transform.localScale = new Vector3(s, s, s);
                    Vector3 pos = cam.transform.position;
                    Vector3 forward = cam.transform.forward;
                    transform.position = pos + forward * near;
                    transform.forward = forward;
                }
            }
        }
        #region 鼠标和触屏事件
        /// <summary>
        /// 派发用户输入指令信息
        /// </summary>
        void DispatchUserAction()
        {
            if (inputType == UserAction.InputType.OnlyMouse)
            {
                DispatchMouse();
            }
            else if (inputType == UserAction.InputType.OnlyTouch)
            {
                DispatchTouch();
            }
            else
            {
                DispatchWin();
            }
            if(PauseEvent)
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
        void ClearAllAction()
        {
            if (inputs != null)
                for (int i = 0; i < inputs.Length; i++)
                    inputs[i].Clear();
        }
        #endregion
        #region UI绘制与合批
        void MainMission()
        {
            point = 1;
            max = 0;
            TxtCollector.Clear();
            Collection(transform, -1, 0);
            for (int i = 0; i < max; i++)
            {
                var scr = scripts[i];
                if (scr.userEvent != null)
                    scr.userEvent.Update();
                if (scr.composite != null)
                    scr.composite.Update(UserAction.TimeSlice);
                scripts[i].MainUpdate();
            }
            TextInput.Dispatch();
            InputCaret.UpdateCaret();
        }
        void SubMission(object obj)
        {
            int len = max;
            if (scripts != null)
            {
                //for (int i = 0; i < len; i++)
                //    scripts[i].SubUpdate();
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
        internal TextCollector TxtCollector = new TextCollector();
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
            point = 1;
            max = 0;
            TxtCollector.Clear();
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
            TxtCollector.GenerateTexture();
            SubMission(null);
            CheckSize();
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
                    mesh.vertices = vertex.ToArray();
                    mesh.uv = uv.ToArray();
                    mesh.uv2 = uv1.ToArray();
                    mesh.uv3 = uv2.ToArray();
                    mesh.uv4 = uv3.ToArray();
                    mesh.uv5 = uv4.ToArray();
                    mesh.colors32 = colors.ToArray();
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
                mr.sharedMaterials = MatCollector.GenerateMaterial();
        }
#endif
        #endregion
    }
}
