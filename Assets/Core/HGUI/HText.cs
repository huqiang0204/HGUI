using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UGUI;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HText:HGraphics
    {
        internal static Shader DefFontShader
        {
            get
            {
                if (shader == null)
                    shader = Shader.Find("Custom/FontDef");
                return shader;
            }
        }
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
                }
                return defFont;
            }
            set
            {
                defFont = value;
            }
        }
        Material m_TextMaterial;
        Material DefaultTextMaterial
        {
            get {
                if (m_TextMaterial == null)
                {
                    m_TextMaterial = new Material(DefFontShader);
                }
                return m_TextMaterial;
            }
        }
        Material mainMaterial;
        static Texture m_emoji;
        public static Texture EmojiTexture
        {
            get
            {
                if (m_emoji == null)
                    m_emoji = Resources.Load<Texture>("emoji");
                return m_emoji;
            }
            set
            {
                m_emoji = value;
            }
        }
        static Material m_EmojiMaterial;
        static Material DefaultEmojiMaterial {
            get {
                if (m_EmojiMaterial == null)
                {
                    m_EmojiMaterial = new Material(DefShader);
                    m_EmojiMaterial.SetTexture("_MainTex", EmojiTexture);
                }
                return m_EmojiMaterial;
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
        UILineInfo[] lines;
        UICharInfo[] characters;
        UIVertex[] verts;
        [SerializeField]
        internal Font _font;
        public Font Font { get => _font;
            set {
                _font = value;
            } }
        internal Vector2 m_pivot = new Vector2(0.5f,0.5f);
        public Vector2 Pivot {
            get => m_pivot;
            set {
                m_pivot = value;
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
        [SerializeField]
        internal int m_fontSize = 14;
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
                var font = Font;
                emojiString.FullString = m_text;
                settings.font = font;
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
                settings.color = m_color;
                settings.alignByGeometry = m_align;
                var g = Generator;
                g.Populate(emojiString.FilterString,settings);
                verts = g.verts.ToArray();
                m_dirty = false;
                m_vertexChange = true;
                if (material != null)
                {
                    material.mainTexture = font.material.mainTexture;
                    mainMaterial = material;
                }
                else
                {
                    DefaultTextMaterial.SetTexture("_MainTex", font.material.mainTexture);
                    mainMaterial = m_TextMaterial;
                }
            }
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
            var emojis = text.emojiString.emojis;
            var verts = text.verts;
            int c = verts.Length;
            if (c == 0)
            {
                text.tris = null;
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
            text.tris = CreateTri(c);
            int ec = emojis.Count;
            for (int i = 0; i < ec; i++)
            {
                EmojiInfo info = emojis[i];
                int o = info.pos * 4;
                uv[o] = info.uv[0];
                colors[o].a = 0;
                o++;
                uv[o] = info.uv[1];
                colors[o].a = 0;
                o++;
                uv[o] = info.uv[2];
                colors[o].a = 0;
                o++;
                uv[o] = info.uv[3];
                colors[o].a = 0;
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
                return mainMaterial;
            }
            else if (index == 1)
            {
                return m_EmojiMaterial;
            }
            return null;
        }
        public override bool CompareMaterial(HGraphics graphics)
        {
            HText text = graphics as HText;
            if (text == null)
                return false;
            if (text.mainMaterial == mainMaterial)
                return true;
            return false;
        }
    }
}
