using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public unsafe struct AsyncScriptData
    {
        public Vector2 m_sizeDelta;
        public Vector2 Pivot;
        public Vector2 DesignSize;
        public ScaleType scaleType;
        public AnchorType anchorType;
        public AnchorPointType anchorPointType;
        public Vector2 anchorOffset;
        public MarginType marginType;
        public ParentType parentType;
        public Margin margin;
        public EventType eventType;
        public bool Mask;
        public static int Size = sizeof(AsyncScriptData);
        public static int ElementSize = Size / 4;
    }
    public class AsyncScriptLoader:DataLoader
    {
        protected unsafe void LoadScript(byte* ip, AsyncScript tar)
        {
            var src = (AsyncScriptData*)ip;
            tar.m_sizeDelta = src->m_sizeDelta;
            tar.Pivot = src->Pivot;
            tar.DesignSize = src->DesignSize;
            tar.scaleType = src->scaleType;
            tar.anchorType = src->anchorType;
            tar.anchorPointType = src->anchorPointType;
            tar.anchorOffset = src->anchorOffset;
            tar.marginType = src->marginType;
            tar.parentType = src->parentType;
            tar.margin = src->margin;
            tar.Mask = src->Mask;
            tar.eventType = src->eventType;
        }
        protected unsafe void SaveScript(byte* ip, AsyncScript src)
        {
            AsyncScriptData* tar = (AsyncScriptData*)ip;
            tar->m_sizeDelta = src.m_sizeDelta;
            tar->Pivot = src.Pivot;
            tar->DesignSize = src.DesignSize;
            tar->scaleType = src.scaleType;
            tar->anchorType = src.anchorType;
            tar->anchorPointType = src.anchorPointType;
            tar->anchorOffset = src.anchorOffset;
            tar->marginType = src.marginType;
            tar->parentType = src.parentType;
            tar->margin = src.margin;
            tar->Mask = src.Mask;
            tar->eventType = src.eventType;
        }
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            LoadScript(fake.ip, com.GetComponent<AsyncScript>());
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as AsyncScript;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, AsyncScriptData.ElementSize);
            SaveScript(fake.ip, src);
            return fake;
        }
    }
}
