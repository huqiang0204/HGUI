using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using huqiang.Data;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public unsafe struct HGraphicsData
    {
        public AsyncScriptData scriptData;
        public Int32 shader;
        public Int32 asset;
        public Int32 MainTexture;
        public Int32 STexture;
        public Int32 TTexture;
        public Int32 FTexture;
        public Color color;
        public static int Size = sizeof(HGraphicsData);
        public static int ElementSize = Size / 4;
    }
    public class HGraphicsLoader:AsyncScriptLoader
    {
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            var hg = com as HGraphics;
            LoadScript(fake.ip, hg);
            LoadHGraphics(fake, hg);
        }
        protected unsafe void LoadHGraphics(FakeStruct fake, HGraphics tar)
        {
            HGraphicsData* src = (HGraphicsData*)fake.ip;
            string asset = fake.GetData<string>(src->asset);
            if(asset!=null)
            {
                string str = fake.GetData<string>(src->MainTexture);
                if (str != null)
                    tar.MainTexture = ElementAsset.FindTexture(asset, str);
                str = fake.GetData<string>(src->STexture);
                if (str != null)
                    tar.STexture = ElementAsset.FindTexture(asset, str);
                str = fake.GetData<string>(src->TTexture);
                if (str != null)
                    tar.TTexture = ElementAsset.FindTexture(asset, str);
                str = fake.GetData<string>(src->FTexture);
                if (str != null)
                    tar.FTexture = ElementAsset.FindTexture(asset, str);
            }
            string shader = fake.GetData<string>(src->shader);
            if (shader != null)
                tar.Material = new Material(Shader.Find(shader));
            tar.m_color = src->color;
        }
        protected unsafe void SaveHGraphics(FakeStruct fake, HGraphics src)
        {
            HGraphicsData* tar = (HGraphicsData*)fake.ip;
            var tex = src.MainTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->MainTexture = fake.buffer.AddData(tex.name);
                }
            }
            tex = src.STexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->STexture = fake.buffer.AddData(tex.name);
                }
            }
            tex = src.TTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->TTexture = fake.buffer.AddData(tex.name);
                }
            }
            tex = src.FTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->FTexture = fake.buffer.AddData(tex.name);
                }
            }
            if (src.m_material != null)
                tar->shader = fake.buffer.AddData(src.m_material.shader.name);
            tar->color = src.m_color;
        }
        public unsafe override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as HGraphics;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HGraphicsData.ElementSize);
            SaveScript(fake.ip, src);
            SaveHGraphics(fake,src);
            return fake;
        }
      
    }
}
