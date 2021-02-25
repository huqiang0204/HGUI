using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.UIData
{
    public class UIElementLoader : UIDataLoader
    {
        public static Vector2 GetSize(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UIElementData*)fake.ip;
                return trans->m_sizeDelta;
            }
        }
        public static Vector2 GetPivot(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UIElementData*)fake.ip;
                return trans->Pivot;
            }
        }
        public static FakeStruct GetEventData(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UIElementData*)fake.ip;
                return fake.buffer.GetData((Int16)(trans->eve)) as FakeStruct;
            }
        }
        public static FakeStruct GetCompositeData(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UIElementData*)fake.ip;
                return fake.buffer.GetData((Int16)(trans->composite)) as FakeStruct;
            }
        }
        public static FakeStruct GetEx(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UIElementData*)fake.ip;
                return fake.buffer.GetData((Int16)(trans->ex)) as FakeStruct;
            }
        }
        public FakeStructHelper ElementHelper;
        public override unsafe FakeStruct SaveUI(Component com, DataBuffer buffer)
        {
            var src = com.GetComponent<Helper.HGUI.UIContext>();
            FakeStruct fake = new FakeStruct(buffer, UIElementData.ElementSize);
            SaveUIElement(com.transform, fake);
            return fake;
        }
        protected unsafe void SaveUIElement(Transform trans, FakeStruct fake)
        {
            var buffer = fake.buffer;
            UIElementData* tar = (UIElementData*)fake.ip;
            tar->type = fake.buffer.AddData("UIElement");
            tar->insID = trans.GetInstanceID();
            tar->activeSelf = trans.gameObject.activeSelf;
            tar->name = buffer.AddData(trans.name);
            tar->localPosition = trans.localPosition;
            tar->localScale = trans.localScale;
            tar->localRotation = trans.localRotation;
            var con = trans.GetComponent<huqiang.Helper.HGUI.UIContext>();
            if(con!=null)
            {
                var src = con.GetUIData();
                tar->m_sizeDelta = src.m_sizeDelta;
                tar->Pivot = src.Pivot;
                tar->scaleType = src.scaleType;
                tar->anchorType = src.anchorType;
                tar->anchorPointType = src.anchorPointType;
                tar->anchorOffset = src.anchorOffset;
                tar->marginType = src.marginType;
                tar->parentType = src.parentType;
                tar->margin = src.margin;
                tar->Mask = src.Mask;
                tar->eventType = src.eventType;
                tar->compositeType = src.compositeType;
                tar->type = fake.buffer.AddData(src.TypeName);
                tar->insID = src.GetInstanceID();
                tar->activeSelf = src.activeSelf;
            }
            var hleps = con.GetComponents<UIHelper>();
            if(hleps!=null)
            {
                for(int i=0;i<hleps.Length;i++)
                    hleps[i].ToBufferData(buffer, tar);
            }
            int c = trans.childCount;
            if (c > 0)
            {
                Int16[] buf = new short[c];
                for (int i = 0; i < c; i++)
                {
                    var son = trans.GetChild(i);
                    var ui = son.GetComponent<Helper.HGUI.UIContext>();
                    string tn = "UIElement";
                    if (ui == null)
                    {
                        Debug.LogWarning("没有UI元素:" + son.name);
                    }
                    else
                        tn = ui.GetType().Name;
                    var load = uiBuffer.FindDataLoader(tn);
                    if (load != null)
                    {
                        var fs = load.SaveUI(son, buffer);
                        if (fs == null)
                            Debug.LogError("Save Error:" + son.name);
                        buf[i] = (Int16)buffer.AddData(fs);
                    }
                    else Debug.LogError(tn + " type is null");
                }
                tar->child = buffer.AddData(buf);
            }
        }
        public override void LoadUI(HGUI.UIElement tar, FakeStruct fake, HGUI.UIInitializer initializer)
        {
            LoadUIElement(tar, fake, initializer);
        }
        protected unsafe void LoadUIElement(HGUI.UIElement tar, FakeStruct fake, HGUI.UIInitializer initializer)
        {
            UIElementData tmp = new UIElementData();
            unsafe
            {
                UIElementData* src = &tmp;
                ElementHelper.LoadData((byte*)src, fake.ip);
            }
            tar.mod = fake;
            var buffer = fake.buffer;
            tar.activeSelf = tmp.activeSelf;
            tar.name = buffer.GetData(tmp.name) as string;
            tar.localPosition = tmp.localPosition;
            tar.localScale = tmp.localScale;
            tar.localRotation = tmp.localRotation;
            tar.m_sizeDelta = tmp.m_sizeDelta;
            tar.Pivot = tmp.Pivot;
            tar.scaleType = tmp.scaleType;
            tar.anchorType = tmp.anchorType;
            tar.anchorPointType = tmp.anchorPointType;
            tar.anchorOffset = tmp.anchorOffset;
            tar.marginType = tmp.marginType;
            tar.parentType = tmp.parentType;
            tar.margin = tmp.margin;
            tar.Mask = tmp.Mask;
            tar.eventType = tmp.eventType;
            tar.compositeType = tmp.compositeType;
            tar.userEvent = null;
            tar.composite = null;
            tar.SizeChanged = null;
            Int16[] chi = buffer.GetData(tmp.child) as Int16[];
            if (chi != null)
                for (int i = 0; i < chi.Length; i++)
                {
                    var fs = buffer.GetData(chi[i]) as FakeStruct;
                    if (fs != null)
                    {
                        var son = uiBuffer.Clone(fs, initializer);
                        if (son != null)
                            son.SetParent(tar);
                    }
                    else Debug.LogError("child is null");
                }
#if UNITY_EDITOR
            if(Application.isPlaying)
#endif
                tar.Initial(fake, initializer);
            if (initializer != null)
            {
                initializer.Initialiezd(fake, tar);
                initializer.AddContext(tar, tmp.insID);
            }
        }
    }
}
