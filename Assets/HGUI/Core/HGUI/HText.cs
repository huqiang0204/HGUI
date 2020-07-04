using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class HText:HGraphics
    {
        static Font defFont;
        static char[] key_noMesh = new char[] { ' ' ,'\n', '\r' };
        static List<int> bufferA = new List<int>();
        static List<int> bufferB = new List<int>();
        static void CreateEmojiMesh(HText text)
        {
            if (text.verts == null)
                return;
            bufferA.Clear();
            bufferB.Clear();
            var emojis = text.emojiString.emojis;
            var str = text.emojiString.FilterString;
            var verts = text.verts;
            int c = verts.Length;
            if (c == 0)
            {
                text.vertices = null;
                text.tris = null;
                return;
            }

            HVertex[] hv = text.vertices;
            if (hv == null)
            {
                hv = new HVertex[c];
            }
            else if (hv.Length != c)
            {
                hv = new HVertex[c];
            }

            int e = c / 4;
            for (int i = 0; i < c; i++)
            {
                hv[i].position = verts[i].position;
                hv[i].color = verts[i].color;
                hv[i].uv = verts[i].uv0;
                hv[i].uv4.x = 1;
                hv[i].uv4.y = 1;
            }
            if(emojis.Count>0)
            {
                var info = emojis[0];
                Color col = Color.white;
                int p = 0;
                int si = 0;
                int len = str.Length;
                for (int i = 0; i < len; i++)
                {
                    bool yes = true;
                    
                    for(int j=0;j<key_noMesh.Length;j++)
                    {
                        if(key_noMesh[j]==str[i])
                        {
                            yes = false;
                            break;
                        }
                    }
                    if (yes)
                    {
                        if (i == info.pos)
                        {
                            int o = p * 4;
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
                            si++;
                            if (si < emojis.Count)
                                info = emojis[si];
                            int s = p * 4;
                            bufferB.Add(s);
                            bufferB.Add(s + 1);
                            bufferB.Add(s + 2);
                            bufferB.Add(s + 2);
                            bufferB.Add(s + 3);
                            bufferB.Add(s);
                        }
                        else
                        {
                            int s = p * 4;
                            bufferA.Add(s);
                            bufferA.Add(s + 1);
                            bufferA.Add(s + 2);
                            bufferA.Add(s + 2);
                            bufferA.Add(s + 3);
                            bufferA.Add(s);
                        }
                        p++;
                        if (p >= e)
                            break;
                    }
                }
                if(bufferB.Count>0)
                {
                    if (text.subTris == null)
                        text.subTris = new int[2][];
                    if(text.subTris[0]!=null)
                    {
                        if(text.subTris[0].Length==bufferA.Count)
                        {
                            bufferA.CopyTo(text.subTris[0]);
                        }else bufferA.ToArray();
                    }else text.subTris[0] = bufferA.ToArray();
                    if(text.subTris[1]!=null)
                    {
                        if(text.subTris[1].Length==bufferB.Count)
                        {
                            bufferB.CopyTo(text.subTris[1]);
                        }
                        else text.subTris[1] = bufferB.ToArray();
                    }
                    else text.subTris[1] = bufferB.ToArray();
                    text.tris = null;
                }
                else
                {
                    if(text.tris!=null)
                    {
                        if(text.tris.Length==bufferA.Count)
                        {
                            bufferA.CopyTo(text.tris);
                        }
                        else text.tris = bufferA.ToArray();
                    }
                    else
                    text.tris = bufferA.ToArray();
                    text.subTris = null;
                }
            }
            else
            {
                text.tris = CreateTri(c,text.tris);
                text.subTris = null;
            }
            text.vertices = hv;
        }
        static void OutLineVertex(HVertex[] buf, int start, HVertex[] src, float x,float y,ref Color32 color)
        {
            int l = src.Length;
            for(int i=0;i<l;i++)
            {
                buf[start] = src[i];
                buf[start].position.x += x;
                buf[start].position.y += y;
                buf[start].color = color;
                start++;
            }
        }
        static void OutLineTris(int[] buf, int start, int[] src, int offset)
        {
            for(int i=0;i<src.Length;i++)
            {
                buf[start] = src[i] + offset;
                start++;
            }
        }
        static void CreateOutLine(HText text)
        {
            HVertex[] buf = text.vertices;
            if (buf == null)
                return;
            int c = buf.Length;
            HVertex[] tmp = new HVertex[c * 5];
            float d = text.OutLine;
            OutLineVertex(tmp, 0, buf, d, d, ref text.shadowColor);
            OutLineVertex(tmp, c, buf, d, -d, ref text.shadowColor);
            OutLineVertex(tmp, c * 2, buf, -d, -d, ref text.shadowColor);
            OutLineVertex(tmp, c * 3, buf, -d, d, ref text.shadowColor);
            int s = c * 4;
            for (int i = 0; i < c; i++)
            {
                tmp[s] = buf[i];
                s++;
            }
            text.vertices = tmp;
            if (text.tris != null)
            {
                var src = text.tris;
                int l = src.Length;
                int[] tris = new int[l * 5];
                OutLineTris(tris, 0, src, 0);
                OutLineTris(tris, l, src, c);
                OutLineTris(tris, l * 2, src, c * 2);
                OutLineTris(tris, l * 3, src, c * 3);
                OutLineTris(tris, l * 4, src, c * 4);
                text.tris = tris;
            }
            else if (text.subTris != null)
            {
                var o = text.subTris;
                for (int j = 0; j < o.Length; j++)
                {
                    var src = o[j];
                    int l = src.Length;
                    int[] tris = new int[l * 5];
                    OutLineTris(tris, 0, src, 0);
                    OutLineTris(tris, l, src, c);
                    OutLineTris(tris, l * 2, src, c * 2);
                    OutLineTris(tris, l * 3, src, c * 3);
                    OutLineTris(tris, l * 4, src, c * 4);
                    o[j] = tris;
                }
            }
        }
        static int[] CreateTri(int len,int[] ori)
        {
            int c = len / 4;
            if (c < 0)
                return null;
            int max = c * 6;
            int[] tri = ori;
            if (tri == null)
                tri = new int[max];
            else if (tri.Length != max)
                tri = new int[max];
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
        [SerializeField]
        internal FontStyle m_fontStyle;
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
        public ContentSizeFitter sizeFitter;
        /// <summary>
        /// 慎用,顶点占用较多
        /// </summary>
        public float OutLine;
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
            var str = emojiString.FilterString;
            if(sizeFitter!=ContentSizeFitter.None)
            {
                if (marginType != MarginType.None)
                    Margin(this);
                GetGenerationSettings(ref m_sizeDelta, ref settings);
                var gen = Generator;
                if (sizeFitter==ContentSizeFitter.Horizoantal)
                {
                    m_sizeDelta.x = gen.GetPreferredWidth(str, settings);
                }
               else if(sizeFitter==ContentSizeFitter.Vertical)
                {
                    m_sizeDelta.y = gen.GetPreferredHeight(str, settings);
                }else if(sizeFitter == ContentSizeFitter.Both)
                {
                    float w = gen.GetPreferredWidth(str, settings);
                    if (w < m_sizeDelta.x)
                        m_sizeDelta.x = w;
                    m_sizeDelta.y = gen.GetPreferredHeight(str, settings);
                }
                Dock(this);
            }
            GetGenerationSettings(ref m_sizeDelta,ref settings);
            var g = Generator;
            g.Populate(str, settings);
            var v = g.verts;
            if (verts != null)
            {
                if (v.Count == verts.Length)
                {
                    v.CopyTo(verts,0);
                }
                else
                    verts = v.ToArray();
            }
            else verts = v.ToArray();
            var l = g.lines;
            if(uILines!=null)
            {
                if(uILines.Length==l.Count)
                {
                    l.CopyTo(uILines,0);
                }
                else
                {
                    uILines = l.ToArray();
                }
            }else uILines = l.ToArray();
            var c = g.characters;
            if(uIChars!=null)
            {
                if(uIChars.Length==c.Count)
                {
                    c.CopyTo(uIChars,0);
                }else uIChars = c.ToArray();
            }
            else
            uIChars = c.ToArray();
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
                if (OutLine > 0)
                    CreateOutLine(this);
                m_vertexChange = false;
            }
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
            float h = gen.GetPreferredHeight(new EmojiString(str).FilterString, settings);
            size.y = h;
        }
        public void GetPreferredWidth(ref Vector2 size, string str)
        {
            GetGenerationSettings(ref size, ref settings);
            var gen = Generator;
            float w = gen.GetPreferredWidth(new EmojiString(str).FilterString, settings);
            size.x = w;
        }
        public void GetPreferredSize(ref Vector2 size, string str)
        {
            GetGenerationSettings(ref size, ref settings);
            var gen = Generator;
            var fs = new EmojiString(str).FilterString;
            size.x = gen.GetPreferredWidth(fs, settings);
            size.y = gen.GetPreferredHeight(fs, settings);
        }
    }
    public enum ContentSizeFitter
    { 
       None,
       Horizoantal,
       Vertical,
       Both
    }

}
