using huqiang;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.HQGUI
{
    public class AsyncScript:MonoBehaviour
    {
        protected static ThreadMission thread = new ThreadMission("async");
        public virtual void CalculMesh(Vector3[] vertex,Vector2[] uv0,Vector2[] uv1,int[] tris)
        {

        }
        public virtual void MainUpdate()
        {

        }
        public virtual void SubUpdate()
        {

        }
        public virtual void ReSize()
        {
        }
        public Vector2 SizeDelta = new Vector2(100,100);
        private ScaleType lastScaleType;
        private AnchorType lastSizeType;
        private AnchorPointType lastAnchorType;
        private Margin lastmargin;
        public UserEvent userEvent;
    }
}
