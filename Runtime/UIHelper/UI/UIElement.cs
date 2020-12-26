using huqiang;
using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    [DisallowMultipleComponent]
    /// <summary>
    /// UI基本元素组件
    /// </summary>
    public class UIElement:MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
        public bool expand;
#endif
        #region static method
        public static Vector2[] Anchors = new[] { new Vector2(0.5f, 0.5f), new Vector2(0, 0.5f),new Vector2(1, 0.5f),
        new Vector2(0.5f, 1),new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)};
        public static void Scaling(UIElement script, ScaleType type, Vector2 pSize, Vector2 ds)
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
        public static void AnchorEx(UIElement script, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
        {
            Vector2 pivot = Anchors[(int)type];
            float x = psize.x;
            float y = psize.y;
            float px = p.x;
            float py = p.y;

            float lx = x * -px;
            float dy = y * -py;

            float tx = lx + pivot.x * psize.x;//锚点x
            float ty = dy + pivot.y * psize.y;//锚点y
            offset.x += tx;//偏移点x
            offset.y += ty;//偏移点y
            script.transform.localPosition = new Vector3(offset.x, offset.y, 0);
        }
        public static void AlignmentEx(UIElement script, AnchorPointType type, Vector2 offset, Vector2 p, Vector2 psize)
        {
            Vector2 pivot = Anchors[(int)type];
            float ax = psize.x;
            float ay = psize.y;
            float apx = p.x;
            float apy = p.y;
            float alx = ax * -apx;
            float ady = ay * -apy;
            //float aox = ax * apx;//原点x
            //float aoy = ay * apy;//原点y

            float x = script.SizeDelta.x;
            float y = script.SizeDelta.y;
            float px = script.Pivot.x;
            float py = script.Pivot.y;
            float lx = x * -px;
            float dy = y * -py;

            //float ox = x * px;//原点x
            //float oy = y * py;//原点y

            switch (type)
            {
                case AnchorPointType.Left:
                    x = alx - lx;
                    y = (ady + ay * 0.5f) - (dy + y * 0.5f);
                    break;
                case AnchorPointType.Right:
                    x = (ax + alx) - (x + lx);
                    y = (ady + ay * 0.5f) - (dy + y * 0.5f);
                    break;
                case AnchorPointType.Top:
                    x = (alx + ax * 0.5f) - (lx + x * 0.5f);
                    y = (ay + ady) - (y + dy);
                    break;
                case AnchorPointType.Down:
                    x = (alx + ax * 0.5f) - (lx + x * 0.5f);
                    y = ady - dy;
                    break;
                case AnchorPointType.LeftDown:
                    x = alx - lx;
                    y = ady - dy;
                    break;
                case AnchorPointType.LeftTop:
                    x = alx - lx;
                    y = (ay + ady) - (y + dy);
                    break;
                case AnchorPointType.RightDown:
                    x = (ax + alx) - (x + lx);
                    y = ady - dy;
                    break;
                case AnchorPointType.RightTop:
                    x = (ax + alx) - (x + lx);
                    y = (ay + ady) - (y + dy);
                    break;
                default:
                    x = (alx + ax * 0.5f) - (lx + x * 0.5f);
                    y = (ady + ay * 0.5f) - (dy + y * 0.5f);
                    break;
            }
            x += offset.x;
            y += offset.y;
            script.transform.localPosition = new Vector3(x, y, 0);
        }
        public static void MarginEx(UIElement script, Margin margin, Vector2 parentPivot, Vector2 parentSize)
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
        public static void MarginX(UIElement script, Margin margin, Vector2 parentPivot, Vector2 parentSize)
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
        public static void MarginY(UIElement script, Margin margin, Vector2 parentPivot, Vector2 parentSize)
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
        public static void Resize(UIElement script,bool child = true)
        {
            Transform rect = script.transform;
            Vector2 psize = Vector2.zero;
            Vector2 v = script.m_sizeDelta;
            var pp = Anchors[0];
            if (script.parentType == ParentType.Tranfrom)
            {
                var p = rect.parent;
                if(p!=null)
                {
                    var t = p.GetComponent<UIElement>();
                    if (t != null)
                    {
                        psize = t.SizeDelta;
                        pp = t.Pivot;
                    }
                }
            }
            else
            {
                var t = rect.root.GetComponent<UIElement>();
                if (t != null)
                    psize = t.SizeDelta;
            }
            switch (script.marginType)
            {
                case MarginType.None:
                    break;
                case MarginType.Margin:
                    MarginEx(script, script.margin, pp, psize);
                    break;
                case MarginType.MarginRatio:
                    var mar = new Margin();
                    mar.left = script.margin.left * psize.x;
                    mar.right = script.margin.right * psize.x;
                    mar.top = script.margin.top * psize.y;
                    mar.down = script.margin.down * psize.y;
                    MarginEx(script, mar, pp, psize);
                    break;
                case MarginType.MarginX:
                    MarginX(script, script.margin, pp, psize);
                    break;
                case MarginType.MarginY:
                    MarginY(script, script.margin, pp, psize);
                    break;
                case MarginType.MarginRatioX:
                    mar = new Margin();
                    mar.left = script.margin.left * psize.x;
                    mar.right = script.margin.right * psize.x;
                    MarginX(script, mar, pp, psize);
                    break;
                case MarginType.MarginRatioY:
                    mar = new Margin();
                    mar.top = script.margin.top * psize.y;
                    mar.down = script.margin.down * psize.y;
                    MarginY(script, mar, pp, psize);
                    break;
                case MarginType.Size:
                    script.m_sizeDelta.x = psize.x - script.margin.left;
                    script.m_sizeDelta.y = psize.y - script.margin.down;
                    break;
                case MarginType.Ratio:
                    script.m_sizeDelta.x = psize.x * (1 - script.margin.left);
                    script.m_sizeDelta.y = psize.y * (1 - script.margin.down);
                    break;
                case MarginType.SizeX:
                    script.m_sizeDelta.x = psize.x - script.margin.left;
                    break;
                case MarginType.SizeY:
                    script.m_sizeDelta.y = psize.y - script.margin.down;
                    break;
                case MarginType.RatioX:
                    script.m_sizeDelta.x = psize.x * (1 - script.margin.left);
                    break;
                case MarginType.RatioY:
                    script.m_sizeDelta.y = psize.y * (1 - script.margin.down);
                    break;
            }
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
            if (script.scaleType != ScaleType.None)
                Scaling(script, script.scaleType, psize, script.m_sizeDelta);
            if (child)
                ResizeChild(rect, child);
            if (script.scaleType != ScaleType.None | script.anchorType != AnchorType.None | script.marginType != MarginType.None)
            {
                ResizeChild(rect, false);
            }
        }
        public static void ResizeChild(Transform trans, bool child = true)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                var son = trans.GetChild(i);
                var ss =son.GetComponent<UIElement>();
                if (ss != null)
                    Resize(ss, child);
                else if (child)
                    ResizeChild(son,child);
            }
        }
        public static void ResizeChild(UIElement script, bool child = true)
        {
            var rect = script.transform;
            for (int i = 0; i < rect.childCount; i++)
            {
                var ss = rect.GetChild(i).GetComponent<UIElement>();
                if (ss != null)
                    Resize(ss, child);
            }
        }
        public static void Margin(UIElement script)
        {
            Transform rect = script.transform;
            Vector3 loclpos = rect.localPosition;
            Vector2 psize = Vector2.zero;
            Vector2 v = script.m_sizeDelta;
            var pp = Anchors[0];
            if (script.parentType == ParentType.Tranfrom)
            {
                var p = rect.parent;
                if (p != null)
                {
                    var t = p.GetComponent<UIElement>();
                    if (t != null)
                    {
                        psize = t.SizeDelta;
                        pp = t.Pivot;
                    }
                }
            }
            else
            {
                var t = rect.root.GetComponent<UIElement>();
                if (t != null)
                    psize = t.SizeDelta;
            }
            switch (script.marginType)
            {
                case MarginType.None:
                    break;
                case MarginType.Margin:
                    var mar = script.margin;
                    MarginEx(script, mar, pp, psize);
                    break;
                case MarginType.MarginRatio:
                    mar = new Margin();
                    mar.left = script.margin.left * psize.x;
                    mar.right = script.margin.right * psize.x;
                    mar.top = script.margin.top * psize.y;
                    mar.down = script.margin.down * psize.y;
                    MarginEx(script, mar, pp, psize);
                    break;
                case MarginType.MarginX:
                    mar = script.margin;
                    MarginX(script, mar, pp, psize);
                    break;
                case MarginType.MarginY:
                    mar = script.margin;
                    MarginY(script, mar, pp, psize);
                    break;
                case MarginType.MarginRatioX:
                    mar = new Margin();
                    mar.left = script.margin.left * psize.x;
                    mar.right = script.margin.right * psize.x;
                    MarginX(script, mar, pp, psize);
                    break;
                case MarginType.MarginRatioY:
                    mar = new Margin();
                    mar.top = script.margin.top * psize.y;
                    mar.down = script.margin.down * psize.y;
                    MarginY(script, mar, pp, psize);
                    break;
                case MarginType.Size:
                    script.m_sizeDelta.x = psize.x - script.margin.left;
                    script.m_sizeDelta.y = psize.y - script.margin.down;
                    break;
                case MarginType.Ratio:
                    script.m_sizeDelta.x = psize.x * (1 - script.margin.left);
                    script.m_sizeDelta.y = psize.y * (1 - script.margin.down);
                    break;
                case MarginType.SizeX:
                    script.m_sizeDelta.x = psize.x - script.margin.left;
                    break;
                case MarginType.SizeY:
                    script.m_sizeDelta.y = psize.y - script.margin.down;
                    break;
                case MarginType.RatioX:
                    script.m_sizeDelta.x = psize.x * (1 - script.margin.left);
                    break;
                case MarginType.RatioY:
                    script.m_sizeDelta.y = psize.y * (1 - script.margin.down);
                    break;
            }
        }
        public static void Dock(UIElement script)
        {
            Transform rect = script.transform;
            Vector3 loclpos = rect.localPosition;
            Vector2 psize = Vector2.zero;
            Vector2 v = script.m_sizeDelta;
            var pp = Anchors[0];
            if (script.parentType == ParentType.Tranfrom)
            {
                var p = rect.parent;
                if (p != null)
                {
                    var t = p.GetComponent<UIElement>();
                    if (t != null)
                    {
                        psize = t.SizeDelta;
                        pp = t.Pivot;
                    }
                }
            }
            else
            {
                var t = rect.root.GetComponent<UIElement>();
                if (t != null)
                    psize = t.SizeDelta;
            }
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
        }
        public static Vector2 GetSize(UIElement parent,FakeStruct ele)
        {
            unsafe
            {
                Vector2 psize = Vector2.zero;
                UIElementData* up = (UIElementData*)ele.ip;
                switch(up->parentType)
                {
                    case ParentType.Screen:
                        psize = HCanvas.MainCanvas.m_sizeDelta;
                        break;
                    case ParentType.Tranfrom:
                        if(parent!=null)
                            psize = parent.m_sizeDelta;
                        break;
                }
                switch(up->marginType)
                {
                    case MarginType.None:
                        return up->m_sizeDelta;
                    case MarginType.Margin:
                        psize.x -= up->margin.left + up->margin.right;
                        psize.y -= up->margin.top + up->margin.down;
                        return psize;
                    case MarginType.MarginRatio:
                        psize.x *=(1- up->margin.left - up->margin.right);
                        psize.y *=(1-up->margin.top - up->margin.down);
                        return psize;
                    case MarginType.MarginX:
                        psize.x -= up->margin.left + up->margin.right;
                        psize.y = up->m_sizeDelta.y;
                        return psize;
                    case MarginType.MarginY:
                        psize.x = up->m_sizeDelta.x;
                        psize.y -= up->margin.top + up->margin.down;
                        return psize;
                    case MarginType.MarginRatioX:
                        psize.x *= (1 - up->margin.left - up->margin.right);
                        psize.y = up->m_sizeDelta.y;
                        return psize;
                    case MarginType.MarginRatioY:
                        psize.x = up->m_sizeDelta.x;
                        psize.y *= (1 - up->margin.top - up->margin.down);
                        return psize;
                    case MarginType.Size:
                        psize.x -= up->margin.left;
                        psize.y -= up->margin.down;
                        return psize;
                    case MarginType.Ratio:
                        psize.x *= (1 - up->margin.left);
                        psize.y *= (1 - up->margin.down);
                        return psize;
                    case MarginType.SizeX:
                        psize.x -= up->margin.left;
                        psize.y = up->m_sizeDelta.y;
                        return psize;
                    case MarginType.SizeY:
                        psize.x = up->m_sizeDelta.x;
                        psize.y -= up->margin.down;
                        return psize;
                    case MarginType.RatioX:
                        psize.x *= (1 - up->margin.left);
                        psize.y = up->m_sizeDelta.y;
                        return psize;
                    case MarginType.RatioY:
                        psize.x = up->m_sizeDelta.x;
                        psize.y *= (1 - up->margin.down);
                        return psize;
                }
                return up->m_sizeDelta;
            }
        }
        #endregion
        [SerializeField]
        internal Vector2 m_sizeDelta = new Vector2(100,100);
        public virtual Vector2 SizeDelta { get => m_sizeDelta; set => m_sizeDelta = value; }
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        /// <summary>
        /// 轴心
        /// </summary>
        public Vector2 Pivot = new Vector2(0.5f, 0.5f);
        //public Vector2 DesignSize;
        /// <summary>
        /// 缩放类型
        /// </summary>
        public ScaleType scaleType;
        /// <summary>
        /// 停靠类型
        /// </summary>
        public AnchorType anchorType;
        /// <summary>
        /// 停靠的点类型
        /// </summary>
        public AnchorPointType anchorPointType;
        /// <summary>
        /// 停靠的偏移位置
        /// </summary>
        public Vector2 anchorOffset;
        /// <summary>
        /// 相对与父物体的剔除尺寸的类型
        /// </summary>
        public MarginType marginType;
        /// <summary>
        /// 父物体类型
        /// </summary>
        public ParentType parentType;
        /// <summary>
        /// 相对与父物体的剔除尺寸
        /// </summary>
        public Margin margin;
        /// <summary>
        /// 用户事件类型
        /// </summary>
        public HEventType eventType;
        /// <summary>
        /// 复合型UI类型
        /// </summary>
        public CompositeType compositeType;
        /// <summary>
        /// 是否开启遮罩
        /// </summary>
        public bool Mask;
        /// <summary>
        /// 用户事件
        /// </summary>
        public UserEvent userEvent;
        /// <summary>
        /// 复合ui组件实体
        /// </summary>
        public Composite composite;
        /// <summary>
        /// 数据模型
        /// </summary>
        public FakeStruct mod;
        /// <summary>
        /// 联系上下文
        /// </summary>
        public object DataContext;
        /// <summary>
        /// 主颜色
        /// </summary>
        public virtual Color32 MainColor { get; set; }
        /// <summary>
        /// 当UI尺寸被改变时,执行此委托
        /// </summary>
        public Action<UIElement> SizeChanged;
        protected void SaveToUIElement(Core.HGUI.UIElement ui, bool activeSelf, bool haveChild = true)
        {
#if UNITY_EDITOR
            ui.Target = this;
            ui.expand = expand;
#endif
            ui.activeSelf = gameObject.activeSelf;
            ui.anchorOffset = this.anchorOffset;
            ui.anchorPointType = this.anchorPointType;
            ui.anchorType = this.anchorType;
            ui.compositeType = this.compositeType;
            ui.DataContext = this.DataContext;
            ui.eventType = this.eventType;
            ui.margin = margin;
            ui.marginType = this.marginType;
            ui.Mask = this.Mask;
            ui.m_sizeDelta = this.m_sizeDelta;
            ui.name = name;
            ui.parentType = this.parentType;
            ui.Pivot = this.Pivot;
            ui.scaleType = this.scaleType;
            var trans = transform;
            ui.localPosition = trans.localPosition;
            ui.localRotation = trans.localRotation;
            ui.localScale = trans.localScale;
            if(haveChild)
            {
                int c = trans.childCount;
                for (int i = 0; i < c; i++)
                {
                    var son = trans.GetChild(i);
                    if (activeSelf)
                        if (!son.gameObject.activeSelf)
                            continue;
                    var h = son.GetComponent<UIElement>();
                    if (h == null)
                    {
                        var st = ToHGUI2(son, activeSelf);
                        st.SetParent(ui);
                        st.localPosition = son.localPosition;
                        st.localScale = son.localScale;
                        st.localRotation = son.localRotation;
                    }
                    else
                    {
                        var st = h.ToHGUI2(activeSelf);
                        st.SetParent(ui);
                        st.localPosition = son.localPosition;
                        st.localScale = son.localScale;
                        st.localRotation = son.localRotation;
                    }
                }
            }
            ContextID = ui.id;
        }
        public virtual Core.HGUI.UIElement ToHGUI2(bool activeSelf,bool haveChild = true)
        {
            Core.HGUI.UIElement ui = new Core.HGUI.UIElement();
            SaveToUIElement(ui, activeSelf,haveChild);
            return ui;
        }
        public virtual void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        {
            SaveToUIElement(ui, activeSelf, false);
        }
        public static Core.HGUI.UIElement ToHGUI2(Transform trans, bool activeSelf)
        {
            Core.HGUI.UIElement ui = new Core.HGUI.UIElement();
            ui.activeSelf = trans.gameObject.activeSelf;
            ui.localPosition = trans.localPosition;
            ui.localRotation = trans.localRotation;
            ui.localScale = trans.localScale;
            ui.name = trans.name;
            int c = trans.childCount;
            for (int i = 0; i < c; i++)
            {
                var son = trans.GetChild(i);
                if (activeSelf)
                    if (!son.gameObject.activeSelf)
                        continue;
                var h = son.GetComponent<UIElement>();
                if (h == null)
                {
                    var st = ToHGUI2(son,activeSelf);
                    st.SetParent(ui);
                    st.localPosition = son.localPosition;
                    st.localScale = son.localScale;
                    st.localRotation = son.localRotation;
                }
                else
                {
                    var st = h.ToHGUI2(activeSelf);
                    st.SetParent(ui);
                    st.localPosition = son.localPosition;
                    st.localScale = son.localScale;
                    st.localRotation = son.localRotation;
                }
            }
            return ui;
        }
        public virtual void OnDestroy()
        {
            var ele = huqiang.Core.HGUI.UIElement.FindInstance(ContextID);
            if(ele!=null)
            {
                ele.SetParent(null);
                ele.child.Clear();
                ele.Dispose();
            }
            ContextID = 0;
        }
        #region ///为了更强的兼容模式使用此字段操作
        public bool activeSelf { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
        public Vector3 localScale { get => transform.localScale; set => transform.localScale = value; }
        public Vector3 localPosition { get => transform.localPosition; set => transform.localPosition = value; }
        public Quaternion localRotation { get => transform.localRotation; set => transform.localRotation = value; }
        public int childCount { get => transform.childCount; }
        public UIElement parent
        {
            get
            {
                if (transform.parent != null)
                    return transform.parent.GetComponent<UIElement>();
                return null;
            }
            set
            {
                if (value == null)
                    transform.SetParent(null);
                else transform.SetParent(value.transform);
            }
        }
        public UIElement root
        {
            get
            {
                return transform.root.GetComponent<UIElement>();
            }
        }
        public void SetParent(UIElement ele)
        {
            if (ele == null)
                transform.SetParent(null);
            else transform.SetParent(ele.transform);
        }
        public void SetAsFirstSibling()
        {
            transform.SetAsFirstSibling();
        }
        public UIElement GetChild(int index)
        {
            return transform.GetChild(index).GetComponent<UIElement>();
        }
        public UIElement Find(string name)
        {
            var trans = transform.Find(name);
            if (trans != null)
                return trans.GetComponent<UIElement>();
            return null;
        }
        #endregion
    }
}
