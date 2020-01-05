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
        #region static method
        protected static ThreadMission thread = new ThreadMission("async");
        public static Vector2[] Anchors = new[] { new Vector2(0.5f, 0.5f), new Vector2(0, 0.5f),new Vector2(1, 0.5f),
        new Vector2(0.5f, 1),new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)};
        public static void Scaling(AsyncScript script, ScaleType type, Vector2 pSize, Vector2 ds)
        {
            var rect = script.transform;
            switch (type)
            {
                case ScaleType.None:
                    break;
                case ScaleType.FillX:
                    float sx = pSize.x / ds.x;
                    rect.localScale = new Vector3(sx, sx, sx);
                    break;
                case ScaleType.FillY:
                    float sy = pSize.y / ds.y;
                    rect.localScale = new Vector3(sy, sy, sy);
                    break;
                case ScaleType.FillXY:
                    sx = pSize.x / ds.x;
                    sy = pSize.y / ds.y;
                    if (sx < sy)
                        rect.localScale = new Vector3(sx, sx, sx);
                    else rect.localScale = new Vector3(sy, sy, sy);
                    break;
                case ScaleType.Cover:
                    sx = pSize.x / ds.x;
                    sy = pSize.y / ds.y;
                    if (sx < sy)
                        rect.localScale = new Vector3(sy, sy, sy);
                    else rect.localScale = new Vector3(sx, sx, sx);
                    break;
            }
        }
        public static void Anchor(AsyncScript script, Vector2 pivot, Vector2 offset)
        {
            Vector2 p;
            Vector2 pp = new Vector2(0.5f, 0.5f);
            var rect = script.transform;
            if (rect.parent != null)
            {
                var t = rect.parent.GetComponent<AsyncScript>();
                p = t.SizeDelta;
                pp = t.Pivot;
            }
            else { p = new Vector2(Screen.width, Screen.height); }
            rect.localScale = Vector3.one;
            float sx = p.x * (pivot.x - 0.5f);
            float sy = p.y * (pivot.y - 0.5f);
            float ox = sx + offset.x;
            float oy = sy + offset.y;
            rect.localPosition = new Vector3(ox, oy, 0);
        }
        public static void AnchorEx(AsyncScript script, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
        {
            Vector2 pivot = Anchors[(int)type];
            float ox = (p.x - 1) * psize.x;//原点x
            float oy = (p.y - 1) * psize.y;//原点y
            float tx = ox + pivot.x * psize.x;//锚点x
            float ty = oy + pivot.y * psize.y;//锚点y
            offset.x += tx;//偏移点x
            offset.y += ty;//偏移点y
            script.transform.localPosition = new Vector3(offset.x, offset.y, 0);
        }
        public static void AlignmentEx(AsyncScript script, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
        {
            Vector2 pivot = Anchors[(int)type];
            float ox = (p.x - 1) * psize.x;//原点x
            float oy = (p.y - 1) * psize.y;//原点y
            float tx = ox + pivot.x * psize.x;//锚点x
            float ty = oy + pivot.y * psize.y;//锚点y
            float x = offset.x + tx;
            float y = offset.y + ty;
            switch (type)
            {
                case AnchorPointType.Left:
                    x += script.SizeDelta.x * 0.5f;
                    break;
                case AnchorPointType.Right:
                    x -= script.SizeDelta.x * 0.5f;
                    break;
                case AnchorPointType.Top:
                    y -= script.SizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.Down:
                    y += script.SizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.LeftDown:
                    x += script.SizeDelta.x * 0.5f;
                    y += script.SizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.LeftTop:
                    x += script.SizeDelta.x * 0.5f;
                    y -= script.SizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.RightDown:
                    x -= script.SizeDelta.x * 0.5f;
                    y += script.SizeDelta.y * 0.5f;
                    break;
                case AnchorPointType.RightTop:
                    x -= script.SizeDelta.x * 0.5f;
                    y -= script.SizeDelta.y * 0.5f;
                    break;
            }
            script.transform.localPosition = new Vector3(x, y, 0);
        }
        public static void MarginEx(AsyncScript script, Margin margin, Vector2 parentPivot, Vector2 parentSize)
        {
            var rect = script.transform;
            float w = parentSize.x - margin.left - margin.right;
            float h = parentSize.y - margin.top - margin.down;
            var m_pivot = script.Pivot;
            float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
            float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
            float sx = rect.localScale.x;
            float sy = rect.localScale.y;
            script.SizeDelta = new Vector2(w / sx, h / sy);
            rect.localPosition = new Vector3(ox, oy, 0);
        }
        public static void MarginX(AsyncScript script, Margin margin, Vector2 parentPivot, Vector2 parentSize)
        {
            var rect = script.transform;
            float w = parentSize.x - margin.left - margin.right;
            var m_pivot = script.Pivot;
            float ox = w * m_pivot.x - parentPivot.x * parentSize.x + margin.left;
            float sx = rect.localScale.x;
            float y = script.SizeDelta.y;
            script.SizeDelta = new Vector2(w / sx, y);
            float py = rect.localPosition.y;
            rect.localPosition = new Vector3(ox, py, 0);
        }
        public static void MarginY(AsyncScript script, Margin margin, Vector2 parentPivot, Vector2 parentSize)
        {
            var rect = script.transform;
            float h = parentSize.y - margin.top - margin.down;
            var m_pivot = script.Pivot;
            float oy = h * m_pivot.y - parentPivot.y * parentSize.y + margin.down;
            float sy = rect.localScale.y;
            float x = script.SizeDelta.x;
            script.SizeDelta = new Vector2(x, h / sy);
            float px = rect.localPosition.x;
            rect.localPosition = new Vector3(px, oy, 0);
        }
        public static void Resize(AsyncScript script,bool child =true)
        {
            Transform rect = script.transform;
            Vector3 loclpos = rect.localPosition;
            Vector2 psize = Vector2.zero;
            var pp = Anchors[0];
            if (script.parentType == ParentType.Tranfrom)
            {
                var p = rect.parent;
                if(p!=null)
                {
                    var t = p.GetComponent<AsyncScript>();
                    if (t != null)
                    {
                        psize = t.SizeDelta;
                        pp = t.Pivot;
                    }
                }
            }
            else
            {
                var t = rect.root.GetComponent<AsyncScript>();
                if (t != null)
                    psize = t.SizeDelta;
            }

            if (script.DesignSize.x == 0)
                script.DesignSize.x = 1;
            if (script.DesignSize.y == 0)
                script.DesignSize.y = 1;
            Scaling(script, script.scaleType, psize, script.DesignSize);
            switch (script.anchorType)
            {
                case AnchorType.None:
                    break;
                case AnchorType.Anchor:
                    AnchorEx(script, script.anchorPointType, script.anchorOffset, pp, psize);
                    break;
                case AnchorType.Alignment:
                    AlignmentEx(script, script.anchorPointType, script.anchorOffset, pp, psize);
                    break;
            }
            switch (script.marginType)
            {
                case MarginType.None:
                    break;
                case MarginType.Margin:
                    var mar = script.margin;
                    if (script.parentType == ParentType.BangsScreen)
                        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                            mar.top += 88;
                    MarginEx(script, mar, pp, psize);
                    break;
                case MarginType.MarginRatio:
                    mar = new Margin();
                    mar.left = script.margin.left * psize.x;
                    mar.right = script.margin.right * psize.x;
                    mar.top = script.margin.top * psize.y;
                    if (script.parentType == ParentType.BangsScreen)
                        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                            mar.top += 88;
                    mar.down = script.margin.down * psize.y;
                    MarginEx(script, mar, pp, psize);
                    break;
                case MarginType.MarginX:
                    mar = script.margin;
                    if (script.parentType == ParentType.BangsScreen)
                        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                            mar.top += 88;
                    MarginX(script, mar, pp, psize);
                    break;
                case MarginType.MarginY:
                    mar = script.margin;
                    if (script.parentType == ParentType.BangsScreen)
                        if (Scale.ScreenHeight / Scale.ScreenWidth > 2f)
                            mar.top += 88;
                    MarginY(script, mar, pp, psize);
                    break;
                case MarginType.MarginRatioX:
                    break;
                case MarginType.MarginRatioY:
                    break;
            }
            if (child)
                for (int i = 0; i < rect.childCount; i++)
                {
                    var ss = rect.GetChild(i).GetComponent<AsyncScript>();
                    if (ss != null)
                        Resize(ss,child);
                }
            script.ReSized();
        }
        #endregion
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
        public virtual void ReSized()
        {
            if (SizeChanged != null)
                SizeChanged(this);
        }

        public bool Mask;
        public UserEvent userEvent;
        internal int PipelineIndex;
        public virtual Color32 Chromatically { get; set; }
        public T RegEvent<T>() where T : UserEvent, new()
        {
            var t = new T();
            t.Context = this;
            t.Initial(null);
            userEvent = t;
            t.g_color = Chromatically;
            return t;
        }
        public object RegEvent(Type type, FakeStruct fake)
        {
            UserEvent u = Activator.CreateInstance(type) as UserEvent;
            u.Context = this;
            u.Initi(fake);
            userEvent = u;
            u.g_color = Chromatically;
            return u;
        }
        public Action<AsyncScript> SizeChanged;
    }
}
