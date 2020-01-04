using huqiang;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class AsyncScript:MonoBehaviour
    {
        protected static ThreadMission thread = new ThreadMission("async");

        public Vector2 SizeDelta = new Vector2(100, 100);
        public Vector2 Pivot = new Vector2(0.5f, 0.5f);
        public Vector2 DesignSize;
        public ScaleType scaleType;
        public AnchorType anchorType;
        public AnchorPointType anchorPointType;
        public Vector2 anchorOffset;
        public MarginType marginType;
        public ParentType parentType;
        public Margin margin;
        public virtual void MainUpdate()
        {
        }
        public virtual void SubUpdate()
        {

        }
        public virtual void ReSize()
        {
        }

        public bool Mask;
        public UserEvent userEvent;
        internal int PipelineIndex;
        public virtual Color Chromatically { get; set; }
        public T RegEvent<T>() where T : UserEvent, new()
        {
            var t = new T();
            t.Context = this;
            t.Initial(null);
            userEvent = t;
            t.g_color = Chromatically;
            return t;
        }
        public object RegEvent(Type type,FakeStruct fake)
        {
            UserEvent u = Activator.CreateInstance(type) as UserEvent;
            u.Context = this;
            u.Initial(fake);
            userEvent = u;
            u.g_color = Chromatically;
            return u;
        }
        public Action<AsyncScript> SizeChanged;
    }
}
