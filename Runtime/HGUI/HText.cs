using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct TextVertex
    {
        public Vector3 position;
        public Color32 color;
        public Vector2 uv;
        public int Index;
    }
    public class HText:HGraphics
    {
        static List<HText> TextBuffer = new List<HText>();
        public static BlockBuffer<HVertex> VertexBuffer = new BlockBuffer<HVertex>(32, 1024);
        public static BlockBuffer<TextVertex> PopulateBuffer = new BlockBuffer<TextVertex>(32, 1024);
        static Font defFont;
        protected static char[] key_noMesh = new char[] { ' ' ,'\n' };//排除\r
        static List<int> bufferA = new List<int>();
        static List<int> bufferB = new List<int>();
        public static void DirtyAll()
        {
            int c = TextBuffer.Count;
            for (int i = c - 1; i >= 0; i--)
            {
                var ht = TextBuffer[i];
                if (ht == null)
                    TextBuffer.RemoveAt(i);
                else ht.m_dirty = true;
            }
        }
        static void AddTris(List<int> tris, int s)
        {
            tris.Add(s);
            tris.Add(s + 1);
            tris.Add(s + 2);
            tris.Add(s + 2);
            tris.Add(s + 3);
            tris.Add(s);
        }
        protected static void CreateEmojiMesh(HText text)
        {
            if (text.TmpVerts.DataCount == 0)
            {
                text.vertInfo.DataCount = 0;
                text.trisInfo.DataCount = 0;
                text.trisInfo2.DataCount = 0;
                return;
            }
            bufferA.Clear();
            bufferB.Clear();
            var emojis = text.emojiString.emojis;
            var str = text.emojiString.FilterString;
            var verts = text.TmpVerts;
            int c = verts.DataCount;
            text.tris = null;
            if (text.vertInfo.Size == 0)
            {
                text.vertInfo = VertexBuffer.RegNew(c);
            }
            else
            if (text.vertInfo.Size < c | text.vertInfo.Size > c + 32)
            {
                text.vertInfo.Release();
                text.vertInfo = VertexBuffer.RegNew(c);
            }
            var emoji = text.emojiString;
            EmojiInfo info = null;
            if (emoji.emojis.Count > 0)
                info = emoji.emojis[0];
            int index = 0;
            int e = c / 4;
            int ac = 0;
            Color32 col = Color.white;
            unsafe
            {
                HVertex* hv = text.vertInfo.Addr;
                TextVertex* src = verts.Addr;
                for (int i = 0; i < e; i++)
                {
                    int s = i * 4;
                    int ss = ac;
                    int ti = src[s].Index;
                    for (int j = 0; j < 4; j++)
                    {
                        hv[ss].position = src[s].position;
                        hv[ss].color = src[s].color;
                        hv[ss].uv = src[s].uv;
                        hv[ss].uv4.x = 1;
                        hv[ss].uv4.y = 1;
                        hv[ss].picture = 0;
                        s++;
                        ss++;
                    }
                    if (info != null)
                    {
                        if (ti > info.pos)
                        {
                            info = null;
                            for (int j = index; j < emoji.emojis.Count; j++)
                            {
                                if (emoji.emojis[j].pos >= ti)
                                {
                                    index = j;
                                    info = emoji.emojis[j];
                                    break;
                                }
                            }
                        }
                        ss = ac;
                        if (info != null)
                        {
                            if (ti == info.pos)
                            {
                                AddTris(bufferB, ac);
                                hv[ss].uv = info.uv[0];
                                hv[ss].color = col;
                                hv[ss].picture = 1;
                                ss++;
                                hv[ss].uv = info.uv[1];
                                hv[ss].color = col;
                                hv[ss].picture = 1;
                                ss++;
                                hv[ss].uv = info.uv[2];
                                hv[ss].color = col;
                                hv[ss].picture = 1;
                                ss++;
                                hv[ss].uv = info.uv[3];
                                hv[ss].color = col;
                                hv[ss].picture = 1;
                            }
                            else
                            {
                                AddTris(bufferA, ac);
                            }
                        }
                        else
                        {
                            AddTris(bufferA, ac);
                        }
                    }
                    else
                    {
                        AddTris(bufferA, ac);
                    }
                    ac += 4;
                }
            }
            text.vertInfo.DataCount = ac;
            ApplyTris(text);
        }
        static void ApplyTris(HText text)
        {
            if (text.trisInfo.Size > 0)
                text.trisInfo.Release();
            int ic = bufferA.Count;
            if (ic > 0)
            {
                text.trisInfo = trisBuffer.RegNew(ic);
                text.trisInfo.DataCount = ic;
                unsafe
                {
                    int* ip = text.trisInfo.Addr;
                    for (int i = 0; i < ic; i++)
                        ip[i] = bufferA[i];
                }
            }
            else
            {
                text.trisInfo.DataCount = 0;
            }

            ic = bufferB.Count;
            if (ic > 0)
            {
                if (text.trisInfo2.Size > 0)
                    text.trisInfo2.Release();
                text.trisInfo2 = trisBuffer.RegNew(ic);
                text.trisInfo2.DataCount = ic;
                unsafe
                {
                    int* ip = text.trisInfo2.Addr;
                    for (int i = 0; i < ic; i++)
                        ip[i] = bufferB[i];
                }
            }
            else
            {
                text.trisInfo2.DataCount = 0;
            }
        }
        static void OutLineVertex(ref BlockInfo<HVertex> buf, int start, ref BlockInfo<HVertex> src, float x,float y,ref Color32 color)
        {
            int l = src.DataCount;
            unsafe
            {
                HVertex* tar = buf.Addr;
                HVertex* ori = src.Addr;
                for (int i = 0; i < l; i++)
                {
                    tar[start] = ori[i];
                    tar[start].position.x += x;
                    tar[start].position.y += y;
                    tar[start].color = color;
                    start++;
                }
            }
         
        }
        static void OutLineTris(ref BlockInfo<int> buf, int start, ref BlockInfo<int> src, int offset)
        {
            unsafe
            {
                int* tar = buf.Addr;
                int* ori = src.Addr;
                for (int i = 0; i < src.DataCount; i++)
                {
                    tar[start] = ori[i] + offset;
                    start++;
                }
            }
       
        }
        protected static void CreateOutLine(HText text)
        {
            int c = text.vertInfo.DataCount;
            if (c == 0)
                return;
            BlockInfo<HVertex> tmp = VertexBuffer.RegNew(c*5);
            tmp.DataCount = c * 5;
            float d = text.OutLine;
            OutLineVertex(ref tmp, 0,ref text.vertInfo, d, d, ref text.shadowColor);
            OutLineVertex(ref tmp, c, ref text.vertInfo, d, -d, ref text.shadowColor);
            OutLineVertex(ref tmp, c * 2, ref text.vertInfo, -d, -d, ref text.shadowColor);
            OutLineVertex(ref tmp, c * 3, ref text.vertInfo, -d, d, ref text.shadowColor);
            unsafe
            {
                HVertex* tar = tmp.Addr;
                HVertex* ori = text.vertInfo.Addr;
                int s = c * 4;
                for (int i = 0; i < c; i++)
                {
                    tar[s] = ori[i];
                    s++;
                }
            }

            text.vertInfo.Release();
            text.vertInfo = tmp;
            if (text.trisInfo.DataCount>0)
            {
                int l = text.trisInfo.DataCount;
                var tris = trisBuffer.RegNew(l*5);
                tris.DataCount = l*5;
                OutLineTris(ref tris, 0,ref text.trisInfo, 0);
                OutLineTris(ref tris, l, ref text.trisInfo, c);
                OutLineTris(ref tris, l * 2, ref text.trisInfo, c * 2);
                OutLineTris(ref tris, l * 3, ref text.trisInfo, c * 3);
                OutLineTris(ref tris, l * 4, ref text.trisInfo, c * 4);
                text.trisInfo.Release();
                text.trisInfo = tris;
            }
            if (text.trisInfo2.DataCount>0)
            {
                int l = text.trisInfo2.DataCount;
                var tris = trisBuffer.RegNew(l * 5);
                tris.DataCount = l * 5;
                OutLineTris(ref tris, 0, ref text.trisInfo2, 0);
                OutLineTris(ref tris, l, ref text.trisInfo2, c);
                OutLineTris(ref tris, l * 2, ref text.trisInfo2, c * 2);
                OutLineTris(ref tris, l * 3, ref text.trisInfo2, c * 3);
                OutLineTris(ref tris, l * 4, ref text.trisInfo2, c * 4);
                text.trisInfo2.Release();
                text.trisInfo2 = tris;
            }
        }
        static void CreateTri(int len,ref BlockInfo<int> block)
        {
            int c = len / 4;
            if (c < 0)
            {
                block.DataCount = 0;
                return;
            }
            int max = c * 6;
            if(block.Size==0)
            {
                block = trisBuffer.RegNew(max);
            }
            else if(block.Size<max|block.Size>max+48)
            {
                block.Release();
                block= trisBuffer.RegNew(max);
            }
            block.DataCount = max;
            unsafe
            {
                int* tri = block.Addr;
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
            }
           
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
        internal EmojiString emojiString = new EmojiString();
        
        [SerializeField]
        internal Font _font;
        public Font Font {
            get { 
                if (_font == null) 
                    return DefaultFont;
                return _font; 
            }
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

        internal BlockInfo<TextVertex> TmpVerts;
        protected void GetTempVertex(IList<UIVertex> v, ref BlockInfo<TextVertex> vert, string filterStr)
        {
            int o = 0;
            unsafe
            {
                TextVertex* hv = vert.Addr;
                for (int i = 0; i < filterStr.Length; i++)
                {
                    var ch = filterStr[i];
                    bool mesh = true;
                    for (int j = 0; j < key_noMesh.Length; j++)
                    {
                        if (key_noMesh[j] == ch)
                        {
                            mesh = false;
                            break;
                        }
                    }
                    if (mesh)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            hv[o].position = v[o].position;
                            hv[o].uv = v[o].uv0;
                            hv[o].color = v[o].color;
                            hv[o].Index = i;
                            o++;
                        }
                        if (o >= v.Count)
                            break;
                    }
                }
            }
            vert.DataCount = o;
        }
        public virtual void Populate()
        {
            if (!m_dirty)
                return;
            emojiString.FullString = m_text;
            var str = emojiString.FilterString;
            if (sizeFitter != ContentSizeFitter.None)
            {
                if (marginType != MarginType.None)
                    Margin(this);
                GetGenerationSettings(ref m_sizeDelta, ref settings);
                var gen = Generator;
                if (sizeFitter == ContentSizeFitter.Horizoantal)
                {
                    m_sizeDelta.x = gen.GetPreferredWidth(str, settings);
                }
                else if (sizeFitter == ContentSizeFitter.Vertical)
                {
                    m_sizeDelta.y = gen.GetPreferredHeight(str, settings);
                }
                else if (sizeFitter == ContentSizeFitter.Both)
                {
                    float w = gen.GetPreferredWidth(str, settings);
                    if (w < m_sizeDelta.x)
                        m_sizeDelta.x = w;
                    m_sizeDelta.y = gen.GetPreferredHeight(str, settings);
                }
                Dock(this);
            }
            GetGenerationSettings(ref m_sizeDelta, ref settings);
            var g = Generator;
            g.Populate(str, settings);
            var v = g.verts;
            int c = g.characterCountVisible * 4;
            if (c == 0)
            {
                TmpVerts.DataCount = 0;
                trisInfo.DataCount = 0;
                trisInfo2.DataCount = 0;
                return;
            }
            else
            if (c > TmpVerts.Size | TmpVerts.Size > c + 32)
            {
                TmpVerts.Release();
                TmpVerts = PopulateBuffer.RegNew(c);
            }
            if (m_richText)
            {
                emojiString.FullString = RichTextHelper.DeleteLabel(m_text);
                str = emojiString.FilterString;
            }
            GetTempVertex(v, ref TmpVerts, str);
            m_dirty = false;
            m_vertexChange = true;
            fillColors[0] = true;
            m_colorChanged = false;
            MainTexture = Font.material.mainTexture;
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
        private void Awake()
        {
            TextBuffer.Add(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            TextBuffer.Remove(this);
            TmpVerts.Release();
        }
        public override void ReSized()
        {
            base.ReSized();
            m_dirty = true;
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
