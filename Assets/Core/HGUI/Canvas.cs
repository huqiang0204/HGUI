using System;
using huqiang;
using huqiang.UIEvent;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public sealed class Canvas:AsyncScript
    {
        public Camera camera;
        public RenderMode renderMode;
        GUIElement[] PipeLine = new GUIElement[4096];
        AsyncScript[] scripts = new AsyncScript[4096];
        int point = 0;
        int max;
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
            
            thread.AddSubMission((o) =>
            {
                int len = max;
                if (scripts != null)
                    for (int i = 0; i < len; i++)
                        scripts[i].SubUpdate();
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
                float near = cam.nearClipPlane;
                float s = 0.5f / (float)h;
                float o = MathH.Tan(cam.fieldOfView);
                s /= o;
                transform.localScale = new Vector3(s, s, s);
                transform.position = cam.transform.position + cam.transform.forward*near;
                transform.forward = cam.transform.forward;
            }
        }
    }
}
