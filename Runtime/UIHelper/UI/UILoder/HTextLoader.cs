using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Core.UIData;
using huqiang.Data;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public unsafe struct HTextData
    {
        public HGraphicsData graphicsData;
        public Int32 text;
        public Int32 font;
        public Vector2 pivot;
        public HorizontalWrapMode m_hof;
        public VerticalWrapMode m_vof;
        public TextAnchor anchor;
        public bool m_richText;
        public float m_lineSpace;
        public int m_fontSize;
        public bool m_align;
        public FontStyle m_fontStyle;
        public ContentSizeFitter sizeFitter;
        public float OutLine;
        public static int Size = sizeof(HTextData);
        public static int ElementSize = Size / 4;
    }
    public class HTextLoader : HGraphicsLoader
    {
        public FakeStructHelper TextHelper;
        public static List<Font> fonts = new List<Font>();
        public static Font FindFont(string str)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return UnityEngine.Resources.GetBuiltinResource<Font>(str + ".ttf");
            }
#endif
            if (fonts == null)
                return null;
            for (int i = 0; i < fonts.Count; i++)
            {
                if (str == fonts[i].name)
                    return fonts[i];
            }
            if (fonts.Count == 0)
                fonts.Add(Font.CreateDynamicFontFromOSFont("Arial", 16));
            return fonts[0];
        }
        protected string fontName;
        protected void LoadHText(FakeStruct fake, UIContext ui)
        {
            HTextData tmp;
            unsafe
            {
                tmp = *(HTextData*)fake.ip;
            }
            var tar = ui.GetUIData() as huqiang.Core.HGUI.HText;
            var buffer = fake.buffer;
            tar.m_text = buffer.GetData(tmp.text) as string;
            fontName = buffer.GetData(tmp.font) as string;
            if (fontName != null)
                tar._font = FindFont(fontName);
            else tar._font = null;
            tar.TextPivot = tmp.pivot;
            tar.m_hof = tmp.m_hof;
            tar.m_vof = tmp.m_vof;
            tar.TextAnchor = tmp.anchor;
            tar.m_richText = tmp.m_richText;
            tar.m_lineSpace = tmp.m_lineSpace;
            tar.m_fontSize = tmp.m_fontSize;
            tar.m_align = tmp.m_align;
            tar.m_fontStyle = tmp.m_fontStyle;
            tar.sizeFitter = tmp.sizeFitter;
            tar.OutLine = tmp.OutLine;
        }
        protected unsafe void SaveHText(FakeStruct fake, huqiang.Core.HGUI.HText ht)
        {
            var src = ht;
            HTextData* tar = (HTextData*)fake.ip;
            tar->text = fake.buffer.AddData(src.m_text);
            if (src._font != null)
                tar->font = fake.buffer.AddData(src._font.name);
            tar->pivot = src.TextPivot;
            tar->m_hof = src.m_hof;
            tar->m_vof = src.m_vof;
            tar->anchor = src.TextAnchor;
            tar->m_richText = src.m_richText;
            tar->m_lineSpace = src.m_lineSpace;
            tar->m_fontSize = src.m_fontSize;
            tar->m_align = src.m_align;
            tar->m_fontStyle = src.m_fontStyle;
            tar->sizeFitter = src.sizeFitter;
            tar->OutLine = src.OutLine;
        }
        public unsafe override void LoadToComponent(FakeStruct fake, Component com, FakeStruct main)
        {
            UIContext ui = com.GetComponent<UIContext>();
            if (ui == null)
            {
                Debug.Log(com.name);
                return; 
            }
            LoadElement(fake, ui);
            LoadHGraphics(fake, ui);
            LoadHText(fake, ui);
            //image.Initial(main,initializer as UIInitializer);
        }
        public unsafe override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as HText;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HTextData.ElementSize);
            SaveScript(fake.ip, src.Content);
            SaveHGraphics(fake, src.Content);
            SaveHText(fake, src.Content);
            return fake;
        }
        public static void CopyTo(HText ht, HText ht2)
        {
            var src = ht.Content;
            var tar = ht2.Content;
            tar.Text = src.Text;
            tar._font = src._font;
            tar.m_pivot = src.m_pivot;
            tar.m_hof = src.m_hof;
            tar.m_vof = src.m_vof;
            tar.anchor = src.anchor;
            tar.m_richText = src.m_richText;
            tar.m_lineSpace = src.m_lineSpace;
            tar.m_fontSize = src.m_fontSize;
            tar.m_align = src.m_align;
            tar.m_fontStyle = src.m_fontStyle;
            tar.sizeFitter = src.sizeFitter;
            tar.OutLine = src.OutLine;
        }
    }
}
