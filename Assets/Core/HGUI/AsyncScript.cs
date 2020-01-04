using huqiang;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class AsyncScript:MonoBehaviour
    {
        protected static ThreadMission thread = new ThreadMission("async");
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
        public Vector2 Pivot = new Vector2(0.5f, 0.5f);
        internal ScaleType scaleType;
        internal AnchorType sizeType;
        internal AnchorPointType anchorType;
        internal Margin margin;
        public bool Mask;
        public UserEvent userEvent;
        internal int PipelineIndex;
        public virtual Color Chromatically { get; set; }
        public T RegEvent<T>() where T : UserEvent, new()
        {
            var t = new T();
            t.Context = this;
            t.Initial();
            userEvent = t;
            t.g_color = Chromatically;
            return t;
        }
        public object RegEvent(Type type)
        {
            UserEvent u = Activator.CreateInstance(type) as UserEvent;
            u.Context = this;
            u.Initial();
            userEvent = u;
            u.g_color = Chromatically;
            return u;
        }
        public Action<AsyncScript> SizeChanged;
    }
}
