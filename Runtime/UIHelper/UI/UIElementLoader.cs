using huqiang.Core.UIData;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public unsafe struct UIElementData
    {
        public Vector2 m_sizeDelta;
        public Vector2 Pivot;
        public ScaleType scaleType;
        public AnchorType anchorType;
        public AnchorPointType anchorPointType;
        public Vector2 anchorOffset;
        public MarginType marginType;
        public ParentType parentType;
        public Margin margin;
        public HEventType eventType;
        public CompositeType compositeType;
        public bool Mask;

        public static int Size = sizeof(UIElementData);
        public static int ElementSize = Size / 4;
    }
    public class UIElementLoader:DataLoader
    {
        public FakeStructHelper ElementHelper;
        protected  void LoadElement(FakeStruct fake, UIElement tar)
        {
            //tar.Clear();
            UIElementData tmp;
            //if (ElementHelper == null)
            //{
            unsafe
            {
                tmp = *(UIElementData*)fake.ip;
            }
            //}
            //else
            //{
            //    tmp = new UIElementData();
            //    unsafe
            //    {
            //        UIElementData* src = &tmp;
            //        ElementHelper.LoadData((byte*)src, fake.ip);
            //    }
            //}
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
            //if (tar.userEvent != null)
            //    tar.userEvent.RemoveFocus();
            //tar.userEvent = null;
            //tar.composite = null;
            //tar.SizeChanged = null;
        }
        protected unsafe void SaveScript(byte* ip, UIElement src)
        {
            UIElementData* tar = (UIElementData*)ip;
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
        }
        public unsafe override void LoadToComponent(FakeStruct fake, Component com, FakeStruct main)
        {
            var ui = com.GetComponent<UIElement>();
            ui.composite = null;
            ui.userEvent = null;
            ui.mod = fake;
            LoadElement(fake, ui);
            //ui.Initial(main,initializer as UIInitializer);
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as UIElement;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, UIElementData.ElementSize);
            SaveScript(fake.ip, src);
            return fake;
        }

        public static Vector2 GetSize(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return trans->size;
            }
        }
        public static Vector3 GetScale(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return trans->localScale;
            }
        }
        public static Vector2 GetPivot(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return trans->pivot;
            }
        }
        public static Vector3 GetEuler(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return trans->localEulerAngles;
            }
        }
        public static FakeStruct GetEventData(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return fake.buffer.GetData((Int16)(trans->eve)) as FakeStruct;
            }
        }
        public static FakeStruct GetCompositeData(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return fake.buffer.GetData((Int16)(trans->composite)) as FakeStruct;
            }
        }
        public static FakeStruct GetEx(FakeStruct fake)
        {
            unsafe
            {
                var trans = (UITransfromData*)fake.ip;
                return fake.buffer.GetData((Int16)(trans->ex)) as FakeStruct;
            }
        }
    }
}
