using System;
using huqiang;
using UnityEngine;

namespace Assets.Core.HQGUI
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
        void Collection(Transform trans, int parent)
        {
            PipeLine[point].parentIndex = parent;
            PipeLine[point].localPos = trans.localPosition;
            PipeLine[point].localRot = trans.localRotation;
            PipeLine[point].localScale = trans.localScale;
            PipeLine[point].trans = trans;
            var script= trans.GetComponent<AsyncScript>();
            if(script!=null)
            {
                scripts[max] = script;
                max++;
            }
            PipeLine[point].script = script;
            int c = trans.childCount;
            PipeLine[point].childCount = c;
            int p = point;
            point++;
            for (int i = 0; i < c; i++)
                Collection(trans.GetChild(i), p);
        }
        private void Update()
        {
            point = 0;
            max = 0;
            Collection(transform, -1);
            CheckSize();
            for (int i = 0; i < scripts.Length; i++)
                scripts[i].MainUpdate();
            thread.AddSubMission((o) => {
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
            }
        }
    }
}
