using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public unsafe struct AsyncScriptData
    {
        public Vector2 SizeDelta;
        public Vector2 Pivot;
        public ScaleType scaleType;
        public AnchorType sizeType;
        public AnchorPointType anchorType;
        public Margin margin;
        public static int Size = sizeof(AsyncScriptData);
        public static int ElementSize = Size / 4;
    }
    public class AsyncScriptLoader:DataLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            LoadScript(fake.ip, com as AsyncScript);
        }
        protected unsafe void LoadScript(byte* ip,AsyncScript tar)
        {
            var src = (AsyncScriptData*)ip;
            tar.SizeDelta = src->SizeDelta;
            tar.Pivot = src->Pivot;
            tar.scaleType = src->scaleType;
            tar.sizeType = src->sizeType;
            tar.anchorType = src->anchorType;
            tar.margin = src->margin;
        }
        protected unsafe void SaveScript(byte* ip, AsyncScript src)
        {
            AsyncScriptData* tar = (AsyncScriptData*)ip;
            tar->SizeDelta = src.SizeDelta;
            tar->Pivot = src.Pivot;
            tar->scaleType = src.scaleType;
            tar->sizeType = src.sizeType;
            tar->anchorType = src.anchorType;
            tar->margin = src.margin;
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
