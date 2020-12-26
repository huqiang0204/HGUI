using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using huqiang.Data;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public unsafe struct HGraphicsData
    {
        public UIElementData element;
        public StringPoint shader;
        public StringPoint asset;
        public StringPoint MainTexture;
        public StringPoint STexture;
        public StringPoint TTexture;
        public StringPoint FTexture;
        public Color32 color;
        public Vector4 uvRect;
        public bool Shadow;
        public Vector2 shadowOffsset;
        public Color32 shadowColor;
        public static int Size = sizeof(HGraphicsData);
        public static int ElementSize = Size / 4;
    }
    public class HGraphicsLoader:UIElementLoader
    {
        protected string shader;
        protected string asset;
        protected string MainTexture;
        protected string STexture;
        protected string TTexture;
        protected string FTexture;
        public FakeStructHelper GraphicsHelper;
        protected void LoadHGraphics(FakeStruct fake, HGraphics tar)
        {
            HGraphicsData tmp;
            //if (GraphicsHelper == null)
            //{
                unsafe
                {
                    tmp = *(HGraphicsData*)fake.ip;
                }
            //}
            //else
            //{
            //    tmp = new HGraphicsData();
            //    unsafe
            //    {
            //        HGraphicsData* src = &tmp;
            //        GraphicsHelper.LoadData((byte*)src, fake.ip);
            //    }
            //}
            var buffer = fake.buffer;
            asset = buffer.GetData(tmp.asset) as string;
            MainTexture = buffer.GetData(tmp.MainTexture) as string;
            if (MainTexture != null)
                tar.MainTexture = ElementAsset.FindTexture(asset, MainTexture);
            else tar.MainTexture = null;
            STexture = buffer.GetData(tmp.STexture) as string;
            if (STexture != null)
                tar.STexture = ElementAsset.FindTexture(asset, STexture);
            else tar.STexture = null;
            TTexture = buffer.GetData(tmp.TTexture) as string;
            if (TTexture != null)
                tar.TTexture = ElementAsset.FindTexture(asset, TTexture);
            else TTexture = null;
            FTexture = buffer.GetData(tmp.FTexture) as string;
            if (FTexture != null)
                tar.FTexture = ElementAsset.FindTexture(asset, FTexture);
            else FTexture = null;
            shader = buffer.GetData(tmp.shader) as string;
            if (shader != null)
                tar.Material = new Material(Shader.Find(shader));
            else tar.Material = null;
            tar.m_color = tmp.color;
            tar.uvrect = tmp.uvRect;
            tar.Shadow = tmp.Shadow;
            tar.shadowOffsset = tmp.shadowOffsset;
            tar.shadowColor = tmp.shadowColor;
            //tar.tris = null;
        }
        protected unsafe void SaveHGraphics(FakeStruct fake, HGraphics src)
        {
            HGraphicsData* tar = (HGraphicsData*)fake.ip;
            var buffer = fake.buffer;
            var tex = src.MainTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                    tar->asset = buffer.AddData(an);
                tar->MainTexture = buffer.AddData(tex.name);
            }
            tex = src.STexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                    tar->asset = buffer.AddData(an);
                tar->STexture = buffer.AddData(tex.name);
            }
            tex = src.TTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                    tar->asset = buffer.AddData(an);
                tar->TTexture = buffer.AddData(tex.name);
            }
            tex = src.FTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                    tar->asset = buffer.AddData(an);
                tar->FTexture = buffer.AddData(tex.name);
            }
            if (src.m_material != null)
                tar->shader = buffer.AddData(src.m_material.shader.name);
            tar->color = src.m_color;
            tar->uvRect = src.uvrect;
            tar->Shadow = src.Shadow;
            tar->shadowOffsset = src.shadowOffsset;
            tar->shadowColor = src.shadowColor;
        }
        public unsafe override void LoadToComponent(FakeStruct fake, Component com, FakeStruct main)
        {
            var hg = com.GetComponent<HGraphics>();
            if (hg == null)
                return;
            hg.mod = fake;
            LoadElement(fake, hg);
            LoadHGraphics(fake, hg);
            //hg.Initial(main,initializer as UIInitializer);
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
