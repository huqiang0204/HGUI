using huqiang;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class AsyncScript:MonoBehaviour
    {
        protected static ThreadMission thread = new ThreadMission("async");
        private void Reset()
        {
            Initial();
        }
        public virtual void Initial()
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
        internal int PipelineIndex;
        public bool Mask;
    }
}
