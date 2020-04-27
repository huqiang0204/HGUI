using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
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
        public EventType eventType;
        public CompositeType compositeType;
        public bool Mask;
        public static int Size = sizeof(UIElementData);
        public static int ElementSize = Size / 4;
    }
    public class UIElementLoader:DataLoader
    {
        protected unsafe void LoadScript(byte* ip, UIElement tar)
        {
            var src = (UIElementData*)ip;
            tar.m_sizeDelta = src->m_sizeDelta;
            tar.Pivot = src->Pivot;
            tar.scaleType = src->scaleType;
            tar.anchorType = src->anchorType;
            tar.anchorPointType = src->anchorPointType;
            tar.anchorOffset = src->anchorOffset;
            tar.marginType = src->marginType;
            tar.parentType = src->parentType;
            tar.margin = src->margin;
            tar.Mask = src->Mask;
            tar.eventType = src->eventType;
            tar.compositeType = src->compositeType;
            tar.userEvent = null;
            tar.composite = null;
            tar.SizeChanged = null;
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
            LoadScript(fake.ip, ui);
            ui.Initial(main);
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
    }
}
