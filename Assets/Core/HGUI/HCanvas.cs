using System;
using System.Collections.Generic;
using huqiang;
using huqiang.UI;
using huqiang.UIEvent;
using UnityEngine;

namespace Assets.Core.HGUI
{
    [RequireComponent(typeof( MeshFilter),typeof(MeshRenderer))]
    public sealed class HCanvas:AsyncScript
    {
        //static List<Canvas> canvases = new List<Canvas>();
        public Camera camera;
        public RenderMode renderMode;
        GUIElement[] PipeLine = new GUIElement[4096];
        AsyncScript[] scripts = new AsyncScript[4096];
        int point = 0;
        int max;
        public UserAction[] inputs;
        public bool PauseEvent;
        public UserAction.InputType inputType = UserAction.InputType.OnlyMouse;

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
            var script= trans.GetComponent<AsyncScript>();
            if(script!=null)
            {
                scripts[max] = script;
                max++;
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
        }
  
        private void Update()
        {
            point = 1;
            max = 0;
            Collection(transform, -1, 0);
            CheckSize();
            for (int i = 0; i < max; i++)
                scripts[i].MainUpdate();
            UserAction.Update();
            DispatchUserAction();
            thread.AddSubMission((o) =>
            {
                int len = max;
                if (scripts != null)
                    for (int i = 0; i < len; i++)
                        scripts[i].SubUpdate();
                HBatch.Batch(this, PipeLine);
            }, null);
        }
        void CheckSize()
        {
            switch(renderMode)
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
                SizeDelta.x = w;
                SizeDelta.y = h;
                if(cam.orthographic)
                {

                }
                else
                {
                    float near = cam.nearClipPlane;
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

        /// <summary>
        /// 分线程UpDate
        /// </summary>
        public override void SubUpdate()
        {
            
        }
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
                            UserEvent.DispatchEvent(inputs[i], PipeLine);
#if DEBUG
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.StackTrace);
                    }
#endif
                }
            }
            //if (inputs.Length > 1)
            //    GestureEvent.Dispatch(new List<UserAction>(inputs));
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
    }
}
