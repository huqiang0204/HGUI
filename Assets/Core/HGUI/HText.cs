﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class HText:HGraphics
    {
        static Font defFont;
        public static Font DefaultFont
        {
            get
            {
                if (defFont == null)
                {
                    defFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
                }
                return defFont;
            }
            set
            {
                defFont = value;
            }
        }
        static TextGenerator shareGenerator;
        public static TextGenerator Generator { get {
                if (shareGenerator == null)
                    shareGenerator = new TextGenerator();
                return shareGenerator;
            } }
        [TextArea(3,10)][SerializeField]
        internal string m_text;
        public string Text {
            get => m_text;
            set {
                m_text = value;
                m_dirty = true;
            } }
        EmojiString emojiString = new EmojiString();
        UILineInfo[] lines;
        UIVertex[] verts;
        [SerializeField]
        internal Font _font;
        public Font Font { get => _font;
            set {
                _font = value;
            } }
        internal Vector2 m_pivot = new Vector2(0.5f,0.5f);
        public Vector2 TextPivot {
            get => m_pivot;
            set {
                m_pivot = value;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal HorizontalWrapMode m_hof;
        public HorizontalWrapMode HorizontalOverflow {
            get => m_hof;
            set {
                m_hof = value;
                m_dirty = true;
            } }
        [SerializeField]
        internal VerticalWrapMode m_vof;
        public VerticalWrapMode VerticalOverflow {
            get => m_vof;
            set {
                m_vof = value;
                m_dirty = true;
            } }
        public bool updateBounds;
        private int resizeTextMaxSize = 40;
        private int resizeTextMinSize = 10;
        private bool generateOutOfBounds = false;
        bool m_resizeBestFit;
        public bool ResizeForBestFit {
            get => m_resizeBestFit;
            set {
                m_resizeBestFit = true;
                m_dirty = true;
            } }
        [SerializeField]
        internal TextAnchor anchor;
        public TextAnchor TextAnchor {
            get => anchor;
            set {
                anchor = value;
                m_dirty = true;
            } }
        FontStyle m_fontStyle;
        public FontStyle FontStyle {
            get => m_fontStyle;
            set {
                m_fontStyle = value;
                m_dirty = true;
            } }
        float scaleFactor = 1;
        [SerializeField]
        internal bool m_richText;
        public bool RichText {
            get => m_richText;
            set {
                m_richText = value;
                m_dirty = true;
            } }
        [SerializeField]
        internal float m_lineSpace=1;
        public float LineSpacing {
            get => m_lineSpace;
            set {
                m_lineSpace = value;
                m_dirty = true;
            } }
        [SerializeField]
        internal int m_fontSize = 14;
        public int FontSize {
            get => m_fontSize;
            set {
                m_fontSize = value;
                m_dirty = true;
            } }
        internal bool m_align;
        public bool AlignByGeometry {
            get => m_align;
            set {
                m_align = value;
                m_dirty = true;
            } }

        public static TextGenerationSettings settings;
        public void GetGenerationSettings(ref Vector2 size, ref TextGenerationSettings sett)
        {
            var font = Font;
            sett.font = font;
            sett.pivot = TextPivot;
            sett.generationExtents = size;
            sett.horizontalOverflow = m_hof;
            sett.verticalOverflow = m_vof;
            sett.resizeTextMaxSize = resizeTextMaxSize;
            sett.resizeTextMinSize = resizeTextMinSize;
            sett.generateOutOfBounds = generateOutOfBounds;
            sett.resizeTextForBestFit = m_resizeBestFit;
            sett.textAnchor = anchor;
            sett.fontStyle = m_fontStyle;
            sett.scaleFactor = scaleFactor;
            sett.richText = m_richText;
            sett.lineSpacing = m_lineSpace;
            sett.fontSize = m_fontSize;
            sett.color = m_color;
            sett.alignByGeometry = m_align;
        }
        internal UILineInfo[] uILines;
        internal UICharInfo[] uIChars;
        public void Populate()
        {
            emojiString.FullString = m_text;
            GetGenerationSettings(ref m_sizeDelta,ref settings);
            var g = Generator;
            g.Populate(emojiString.FilterString, settings);
            verts = g.verts.ToArray();
            uILines = g.lines.ToArray();
            uIChars = g.characters.ToArray();
            m_dirty = false;
            m_vertexChange = true;
            fillColors[0] = true;
            m_colorChanged = false;
        }
        public override void UpdateMesh()
        {
            if (m_vertexChange)
            {
                CreateEmojiMesh(this);
                m_vertexChange = false;
            }
        }
        static void CreateEmojiMesh(HText text)
        {
            if (text.verts == null)
                return;
            var emojis = text.emojiString.emojis;
            var verts = text.verts;
            int c = verts.Length;
            if (c == 0)
            {
                text.vertices = null;
                text.tris = null;
                return;
            }
            HVertex[] hv = new HVertex[c];
           
            int e = c / 4;
           
            for (int i = 0; i < c; i++)
            {
                hv[i].position = verts[i].position;
                hv[i].color = verts[i].color;
                hv[i].uv = verts[i].uv0;
            }
            int b = emojis.Count;
            for (int i=0; i < emojis.Count; i++)
            {
                if (emojis[i].pos > e)
                {
                    b = i;
                    break;
                }
            }
            if (b > 0)
            {
                int be = b;
                b  *= 6;
                int all = e * 6;
                int a = all - b;
                int[] triA = new int[a];
                int[] triB = new int[b];

                int[] offset = new int[c];
                var info = emojis[0];
                int si = 1;
                int ap = 0;
                int bp = 0;
                Color col = Color.white;
                col.a = text.m_color.a;
                for (int i = 0; i < e; i++)
                {
                    if (i == info.pos)
                    {
                        int o = i * 4;
                        hv[o].uv = info.uv[0];
                        hv[o].color = col;
                        hv[o].picture = 1;
                        o++;
                        hv[o].uv = info.uv[1];
                        hv[o].color = col;
                        hv[o].picture = 1;
                        o++;
                        hv[o].uv = info.uv[2];
                        hv[o].color = col;
                        hv[o].picture = 1;
                        o++;
                        hv[o].uv = info.uv[3];
                        hv[o].color = col;
                        hv[o].picture = 1;
                        if (si < be)
                            info = emojis[si];
                        si++;
                        int p = i * 4;
                        triB[bp] = p;
                        bp++;
                        triB[bp] = p + 1;
                        bp++;
                        triB[bp] = p + 2;
                        bp++;
                        triB[bp] = p + 2;
                        bp++;
                        triB[bp] = p + 3;
                        bp++;
                        triB[bp] = p;
                        bp++;
                    }
                    else
                    {
                        int p = i * 4;
                        triA[ap] = p;
                        ap++;
                        triA[ap] = p + 1;
                        ap++;
                        triA[ap] = p + 2;
                        ap++;
                        triA[ap] = p + 2;
                        ap++;
                        triA[ap] = p + 3;
                        ap++;
                        triA[ap] = p;
                        ap++;
                    }
                }
                if (text.subTris == null)
                    text.subTris = new int[2][];
                text.subTris[0] = triA;
                text.subTris[1] = triB;
                text.tris = null;
            }
            else
            {
                text.tris = CreateTri(c);
                text.subTris = null;
            }
            text.vertices = hv;
        }
        static int[] CreateTri(int len)
        {
            int c = len / 4;
            if (c < 0)
                return null;
            int max = c * 6;
            int[] tri = new int[max];
            for (int i = 0; i < c; i++)
            {
                int p = i * 4;
                int s = i * 6;
                tri[s] = p;
                s++;
                tri[s] = p + 1;
                s++;
                tri[s] = p + 2;
                s++;
                tri[s] = p + 2;
                s++;
                tri[s] = p + 3;
                s++;
                tri[s] = p;
            }
            return tri;
        }
        public void Reset()
        {
            STexture = UnityEngine.Resources.Load<Texture>("Emoji");
#if UNITY_EDITOR
            if (_font == null)
                if (!Application.isPlaying)
                {
                    _font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
#endif
        }
        public void GetPreferredHeight(ref Vector2 size, string str)
        {
            GetGenerationSettings(ref size, ref settings);
            var gen = Generator;
            float h = gen.GetPreferredHeight(str, settings);
            size.y = h;
            if (gen.lineCount == 1)
            {
                var cha = gen.characters[gen.characterCountVisible];
                size.x = cha.cursorPos.x + cha.charWidth * 1.1f + size.x * 0.5f;
            }
        }
    }
}
