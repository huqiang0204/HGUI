using huqiang.Core.HGUI;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.UIData
{
    public enum ContentSizeFitter
    {
        None,
        Horizoantal,
        Vertical,
        Both
    }
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
            //#if UNITY_EDITOR
            //            if (!Application.isPlaying)
            //            {
            //                return UnityEngine.Resources.GetBuiltinResource<Font>(str + ".ttf");
            //            }
            //#endif
            int c = fonts.Count - 1;
            for (int i = c; i>=0; i--)
            {
                if (fonts[i] == null)
                { 
                    fonts.RemoveAt(i); 
                }else
                {
                    if (str == fonts[i].name)
                        return fonts[i];
                }
            }
            for (int i = c; i >= 0; i--)
            {
                if (fonts[i] == null)
                {
                    fonts.RemoveAt(i);
                }
                else
                {
                    if ("Arial" == fonts[i].name)
                        return fonts[i];
                }
            }
            var def = Font.CreateDynamicFontFromOSFont("Arial", 16);
            def.RequestCharactersInTexture("ABCDEFGUVWXYZ", 512);
            fonts.Add(def);
            return def;
        }
        protected string fontName;
        protected void LoadHText(HGUI.HText tar, FakeStruct fake)
        {
            HTextData tmp = new HTextData();
            unsafe
            {
                HTextData* src = &tmp;
                TextHelper.LoadData((byte*)src, fake.ip);

                var buffer = fake.buffer;
                tar.m_text = buffer.GetData(src->text) as string;
                fontName = buffer.GetData(src->font) as string;
                if (fontName != null)
                    tar._font = FindFont(fontName);
                else tar._font = null;
            }
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
        protected unsafe void SaveHText(HGUI.HText src, FakeStruct fake)
        {
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
        public override void LoadUI(HGUI.UIElement com, FakeStruct fake, UIInitializer initializer)
        {
            HGUI.HText image = com as HGUI.HText;
            if (image == null)
                return;
            LoadHGraphics(image, fake);
            LoadHText(image, fake);
            LoadUIElement(image, fake, initializer);

        }
        public override FakeStruct SaveUI(Component com, DataBuffer buffer)
        {
            var src = com.GetComponent<Helper.HGUI.UIContext>();
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HTextData.ElementSize);
            var con = src.GetUIData() as HText;
            SaveHGraphics(con, fake);
            SaveHText(con, fake);
            SaveUIElement(com.transform, fake);
            return fake;
        }
        public static void CopyTo(HText src, HText tar)
        {
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
