﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UGUI;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HText:HGraphics
    {
        static Texture fontTexture;
        public static Font defFont;
        static Font DefaultFont
        {
            get
            {
                if (defFont == null)
                {
                    defFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
                    fontTexture = defFont.material.mainTexture;
                    Font.textureRebuilt += (o) => {
                        fontTexture = defFont.material.mainTexture;
                        DefaultTextMaterial.SetTexture("_MainTex",fontTexture);
                    };
                }
                return defFont;
            }
            set
            {
                defFont = value;
            }
        }
        static Material _TextMaterial;
        static Material DefaultTextMaterial
        {
            get {
                if (_TextMaterial == null)
                {
                    _TextMaterial = new Material(DefShader);
                    _TextMaterial.SetTexture("_MainTex", fontTexture);
                }
                _TextMaterial.SetTexture("_MainTex", fontTexture);
                return _TextMaterial;
            }
        }
        static Texture _emoji;
        public static Texture EmojiTexture
        {
            get
            {
                if (_emoji == null)
                    _emoji = Resources.Load<Texture>("emoji");
                return _emoji;
            }
            set
            {
                _emoji = value;
            }
        }
        static Material _EmojiMaterial;
        static Material DefaultEmojiMaterial {
            get {
                if (_EmojiMaterial == null)
                {
                    _EmojiMaterial = new Material(DefShader);
                    _EmojiMaterial.SetTexture("_MainTex", EmojiTexture);
                }
                return _EmojiMaterial;
            } }
        static TextGenerator shareGenerator;
        static TextGenerator Generator { get {
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
        IList<UILineInfo> lines;
        IList<UICharInfo> characters;
        IList<UIVertex> verts;
        [SerializeField]
        internal Font _font;
        public Font Font { get => _font;
            set {
                _font = value;
            } }
        internal Vector2 _pivot = new Vector2(0.5f,0.5f);
        public Vector2 Pivot {
            get => _pivot;
            set {
                _pivot = value;
                m_dirty = true;
            }
        }
        HorizontalWrapMode m_hof;
        public HorizontalWrapMode HorizontalOverflow {
            get => m_hof;
            set {
                m_hof = value;
                m_dirty = true;
            } }
        VerticalWrapMode m_vof;
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
        TextAnchor anchor;
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
        bool m_richText;
        public bool RichText {
            get => m_richText;
            set {
                m_richText = value;
                m_dirty = true;
            } }

        internal float m_lineSpace;
        public float LineSpacing {
            get => m_lineSpace;
            set {
                m_lineSpace = value;
                m_dirty = true;
            } }
        int m_fontSize = 14;
        public int FontSize {
            get => m_fontSize;
            set {
                m_fontSize = value;
                m_dirty = true;
            } }
        bool m_align;
        public bool AlignByGeometry {
            get => m_align;
            set {
                m_align = value;
                m_dirty = true;
            } }
        public Material emojiMaterial;
        public override void Initial()
        {
            Font = DefaultFont;
        }
        static TextGenerationSettings settings;
        public override void MainUpdate()
        {
            if(m_dirty)
            {
                emojiString.FullString = m_text;
                settings.font = Font;
                settings.pivot = Pivot;
                settings.generationExtents = SizeDelta;
                settings.horizontalOverflow = m_hof;
                settings.verticalOverflow = m_vof;
                settings.resizeTextMaxSize = resizeTextMaxSize;
                settings.resizeTextMinSize = resizeTextMinSize;
                settings.generateOutOfBounds = generateOutOfBounds;
                settings.resizeTextForBestFit = m_resizeBestFit;
                settings.textAnchor = anchor;
                settings.fontStyle = m_fontStyle;
                settings.scaleFactor = scaleFactor;
                settings.richText = m_richText;
                settings.lineSpacing = m_lineSpace;
                settings.fontSize = m_fontSize;
                settings.color = Color;
                settings.alignByGeometry = m_align;
                var g = Generator;
                g.Populate(emojiString.FilterString,settings);
                lines = g.lines;
                verts = g.verts;
                characters = g.characters;
                m_dirty = false;
                _vertexChange = true;
                fontTexture = Font.material.mainTexture;
            }
        }
        public override void UpdateMesh()
        {
            if (_vertexChange)
            {
                CreateEmojiMesh(this);
                _vertexChange = false;
            }
        }
        static void CreateEmojiMesh(HText text)
        {
            var emojis = text.emojiString.emojis;
            var verts = text.verts;
            int c = verts.Count;
            if (c == 0)
            {
                text.tris = null;
                text.SubMesh = null;
                text.Colors = null;
                return;
            }
            Vector3[] vertex = new Vector3[c];
            Color[] colors = new Color[c];
            Vector2[] uv = new Vector2[c];
            int e = c / 4;
            int all = e * 6;
            int b = emojis.Count * 6;
            int a = all - b;
            int[] triA = new int[a];
            int[] triB = new int[b];
            for (int i = 0; i < c; i++)
            {
                vertex[i] = verts[i].position;
                colors[i] = verts[i].color;
                uv[i] = verts[i].uv0;
            }
            text.vertex = vertex;
            text.uv = uv;
            text.Colors = colors;
            int ec = emojis.Count;
            if(ec>0)
            {
                var info = emojis[0];
                int si = 1;
                int ap = 0;
                int bp = 0;
                for (int i = 0; i < e; i++)
                {
                    if(i==info.pos)
                    {
                        int o = i * 4;
                        uv[o] = info.uv[0];
                        o++;
                        uv[o] = info.uv[1];
                        o++;
                        uv[o] = info.uv[2];
                        o++;
                        uv[o] = info.uv[3];
                        if (si < ec)
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
                if (text.SubMesh == null)
                    text.SubMesh = new int[2][];
                text.SubMesh[0] = triA;
                text.SubMesh[1] = triB;
            }
            else
            {
                text.tris = CreateTri(c);
                text.SubMesh = null;
            }
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
        internal override Material GetMaterial(int index, HCanvas canvas)
        {
            if (index == 0)
            {
                if(material==null)
                {
                    return DefaultTextMaterial;
                }
                return material;
            }
            else if (index == 1)
            {
                if(emojiMaterial==null)
                {
                    return DefaultEmojiMaterial;
                }
                return emojiMaterial;
            }
            return null;
        }
    }
}
