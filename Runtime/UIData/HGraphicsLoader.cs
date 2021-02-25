using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using UnityEngine;

namespace huqiang.Core.UIData
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
        protected void LoadHGraphics(HGraphics tar,FakeStruct fake)
        {
            HGraphicsData tmp = new HGraphicsData();
            unsafe
            {
                HGraphicsData* src = &tmp;
                GraphicsHelper.LoadData((byte*)src, fake.ip);
                var buffer = fake.buffer;
                asset = buffer.GetData(src->asset) as string;
                MainTexture = buffer.GetData(src->MainTexture) as string;
                if (MainTexture != null)
                    tar.MainTexture = ElementAsset.FindTexture(asset, MainTexture);
                else tar.MainTexture = null;
                STexture = buffer.GetData(src->STexture) as string;
                if (STexture != null)
                    tar.STexture = ElementAsset.FindTexture(asset, STexture);
                else tar.STexture = null;
                TTexture = buffer.GetData(src->TTexture) as string;
                if (TTexture != null)
                    tar.TTexture = ElementAsset.FindTexture(asset, TTexture);
                else TTexture = null;
                FTexture = buffer.GetData(src->FTexture) as string;
                if (FTexture != null)
                    tar.FTexture = ElementAsset.FindTexture(asset, FTexture);
                else FTexture = null;
                shader = buffer.GetData(src->shader) as string;
                if (shader != null)
                    tar.Material = new Material(Shader.Find(shader));
                else tar.Material = null;
            }
            tar.m_color = tmp.color;
            tar.uvrect = tmp.uvRect;
            tar.Shadow = tmp.Shadow;
            tar.shadowOffsset = tmp.shadowOffsset;
            tar.shadowColor = tmp.shadowColor;
            tar.tris = null;
        }
        protected unsafe void SaveHGraphics(HGraphics src, FakeStruct fake)
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
        public unsafe override void LoadUI(UIElement com, FakeStruct fake, UIInitializer initializer)
        {
            var hg = com as HGraphics;
            if (hg == null)
                return;
            LoadHGraphics(hg, fake);
            LoadUIElement(hg, fake, initializer);
        }
        protected unsafe void SaveHGraphics(Helper.HGUI.HGraphics hg, FakeStruct fake)
        {
            var src = hg.Content;
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
        public unsafe override FakeStruct SaveUI(Component com, DataBuffer buffer)
        {
            var src = com.GetComponent<Helper.HGUI.UIContext>();
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HGraphicsData.ElementSize);
            var con = src.GetUIData() as HGraphics;
            SaveUIElement(com.transform, fake);
            SaveHGraphics(con, fake);
            return fake;
        }
    }
}
