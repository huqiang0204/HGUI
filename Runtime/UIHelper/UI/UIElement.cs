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
    public class UIElement:UIContext
    {
        public huqiang.Core.HGUI.UIElement Content;
        [SerializeField]
        [HideInInspector]
        public int ContextID;
        public void Awake()
        {
            if (Content == null)
                Content = new Core.HGUI.UIElement();
            ContextID = Content.GetInstanceID();
        }
        public override Core.HGUI.UIElement GetUIData()
        {
            if (Content == null)
                Content = new Core.HGUI.UIElement();
            return Content;
        }
        /// <summary>
        /// 当UI尺寸被改变时,执行此委托
        /// </summary>
        public Action<UIElement> SizeChanged;
        protected void SaveToUIElement(Core.HGUI.UIElement ui, bool activeSelf, bool haveChild = true)
        {
            //ui.activeSelf = gameObject.activeSelf;
            //ui.anchorOffset = this.anchorOffset;
            //ui.anchorPointType = this.anchorPointType;
            //ui.anchorType = this.anchorType;
            //ui.compositeType = this.compositeType;
            //ui.DataContext = this.DataContext;
            //ui.eventType = this.eventType;
            //ui.margin = margin;
            //ui.marginType = this.marginType;
            //ui.Mask = this.Mask;
            //ui.m_sizeDelta = this.m_sizeDelta;
            //ui.name = name;
            //ui.parentType = this.parentType;
            //ui.Pivot = this.Pivot;
            //ui.scaleType = this.scaleType;
            //var trans = transform;
            //ui.localPosition = trans.localPosition;
            //ui.localRotation = trans.localRotation;
            //ui.localScale = trans.localScale;
            //if(haveChild)
            //{
            //    int c = trans.childCount;
            //    for (int i = 0; i < c; i++)
            //    {
            //        var son = trans.GetChild(i);
            //        if (activeSelf)
            //            if (!son.gameObject.activeSelf)
            //                continue;
            //        var h = son.GetComponent<UIElement>();
            //        if (h == null)
            //        {
            //            var st = ToHGUI2(son, activeSelf);
            //            st.SetParent(ui);
            //            st.localPosition = son.localPosition;
            //            st.localScale = son.localScale;
            //            st.localRotation = son.localRotation;
            //        }
            //        else
            //        {
            //            var st = h.ToHGUI2(activeSelf);
            //            st.SetParent(ui);
            //            st.localPosition = son.localPosition;
            //            st.localScale = son.localScale;
            //            st.localRotation = son.localRotation;
            //        }
            //    }
            //}
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
