using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class HText : HGraphics
    {
        public override string TypeName { get => "HText"; }
        public static List<HText> TextBuffer = new List<HText>();
        /// <summary>
        /// 顶点缓存
        /// </summary>
        public static BlockBuffer<HVertex> VertexBuffer = new BlockBuffer<HVertex>(32, 1024);
        /// <summary>
        /// 临时顶点缓存
        /// </summary>
        public static BlockBuffer<TextVertex> PopulateBuffer = new BlockBuffer<TextVertex>(32, 1024);
        static Font defFont;

        /// <summary>
        /// 将所有文本设置为被污染
        /// </summary>
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
        static void OutLineVertex(ref BlockInfo<HVertex> buf, int start, ref BlockInfo<HVertex> src, float x, float y, ref Color32 color)
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
        /// <summary>
        /// 创建出线
        /// </summary>
        /// <param name="text">文本实体</param>
        protected static void CreateOutLine(HText text)
        {
            int c = text.vertInfo.DataCount;
            if (c == 0)
                return;
            BlockInfo<HVertex> tmp = VertexBuffer.RegNew(c * 5);
            tmp.DataCount = c * 5;
            float d = text.OutLine;
            OutLineVertex(ref tmp, 0, ref text.vertInfo, d, d, ref text.shadowColor);
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
            if (text.trisInfo1.DataCount > 0)
            {
                int l = text.trisInfo1.DataCount;
                var tris = trisBuffer.RegNew(l * 5);
                tris.DataCount = l * 5;
                OutLineTris(ref tris, 0, ref text.trisInfo1, 0);
                OutLineTris(ref tris, l, ref text.trisInfo1, c);
                OutLineTris(ref tris, l * 2, ref text.trisInfo1, c * 2);
                OutLineTris(ref tris, l * 3, ref text.trisInfo1, c * 3);
                OutLineTris(ref tris, l * 4, ref text.trisInfo1, c * 4);
                text.trisInfo1.Release();
                text.trisInfo1 = tris;
            }
            if (text.trisInfo2.DataCount > 0)
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
        /// <summary>
        /// 默认字体
        /// </summary>
        public static Font DefaultFont
        {
            get
            {
                if (defFont == null)
                {
                    defFont = HTextLoader.FindFont("Arial"); //Font.CreateDynamicFontFromOSFont("Arial", 16);
                    //var t2d = defFont.material.mainTexture as Texture2D;
                    //t2d.Resize(1024, 1024);
                }
                return defFont;
            }
            set
            {
                defFont = value;
            }
        }

        [TextArea(3, 10)]
        [SerializeField]
        internal string m_text;
        public virtual string Text
        {
            get => m_text;
            set
            {
                m_text = value;
                m_dirty = true;
            }
        }

        [SerializeField]
        internal Font _font;
        public Font Font
        {
            get
            {
                if (_font == null)
                    return DefaultFont;
                return _font;
            }
            set
            {
                _font = value;
            }
        }
        [HideInInspector]
        public CustomFont customFont;
        internal Vector2 m_pivot = new Vector2(0.5f, 0.5f);
        public Vector2 TextPivot
        {
            get => m_pivot;
            set
            {
                m_pivot = value;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal HorizontalWrapMode m_hof;
        public HorizontalWrapMode HorizontalOverflow
        {
            get => m_hof;
            set
            {
                m_hof = value;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal VerticalWrapMode m_vof;
        public VerticalWrapMode VerticalOverflow
        {
            get => m_vof;
            set
            {
                m_vof = value;
                m_dirty = true;
            }
        }
        public bool updateBounds;
        private int resizeTextMaxSize = 40;
        private int resizeTextMinSize = 10;
        private bool generateOutOfBounds = false;
        bool m_resizeBestFit;
        public bool EmojiColor;
        public bool ResizeForBestFit
        {
            get => m_resizeBestFit;
            set
            {
                m_resizeBestFit = true;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal TextAnchor anchor;
        public TextAnchor TextAnchor
        {
            get => anchor;
            set
            {
                anchor = value;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal FontStyle m_fontStyle;
        public FontStyle FontStyle
        {
            get => m_fontStyle;
            set
            {
                m_fontStyle = value;
                m_dirty = true;
            }
        }
        float scaleFactor = 1;
        [SerializeField]
        internal bool m_richText;
        public bool RichText
        {
            get => m_richText;
            set
            {
                m_richText = value;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal float m_lineSpace = 1;
        public float LineSpacing
        {
            get => m_lineSpace;
            set
            {
                m_lineSpace = value;
                m_dirty = true;
            }
        }
        [SerializeField]
        internal int m_fontSize = 14;
        public int FontSize
        {
            get => m_fontSize;
            set
            {
                m_fontSize = value;
                m_dirty = true;
            }
        }
        internal bool m_align;
        public bool AlignByGeometry
        {
            get => m_align;
            set
            {
                m_align = value;
                m_dirty = true;
            }
        }
        public ContentSizeFitter sizeFitter;
        /// <summary>
        /// 慎用,顶点占用较多
        /// </summary>
        public float OutLine;
        public static TextGenerationSettings settings;
        /// <summary>
        /// 配置文本生成器的参数
        /// </summary>
        /// <param name="size">参考尺寸</param>
        /// <param name="sett">参数载体</param>
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

        internal StringEx stringEx;
        protected static void ApplyTris(HText text, List<int> a, List<int> b, List<int> c)
        {
            if (text.trisInfo.Size > 0)
                text.trisInfo.Release();
            int ic = a.Count;
            if (ic > 0)
            {
                text.trisInfo = trisBuffer.RegNew(ic);
                text.trisInfo.DataCount = ic;
                unsafe
                {
                    int* ip = text.trisInfo.Addr;
                    for (int i = 0; i < ic; i++)
                        ip[i] = a[i];
                }
            }
            else
            {
                text.trisInfo.DataCount = 0;
            }

            ic = b.Count;
            if (ic > 0)
            {
                if (text.trisInfo1.Size > 0)
                    text.trisInfo1.Release();
                text.trisInfo1 = trisBuffer.RegNew(ic);
                text.trisInfo1.DataCount = ic;
                unsafe
                {
                    int* ip = text.trisInfo1.Addr;
                    for (int i = 0; i < ic; i++)
                        ip[i] = b[i];
                }
            }
            else
            {
                text.trisInfo1.DataCount = 0;
            }
            if (c == null)
                return;
            ic = c.Count;
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
                        ip[i] = c[i];
                }
            }
            else
            {
                text.trisInfo2.DataCount = 0;
            }
        }
        internal List<PopulateStringInfo> populates = new List<PopulateStringInfo>();
        /// <summary>
        /// 文本计算
        /// </summary>
        public virtual void Populate()
        {
            HTextGenerator.AddContext(this, Font);
            if (m_dirty | m_colorChanged)
            {
                if (stringEx == null)
                    stringEx = new StringEx(m_text, m_richText);
                else stringEx.Reset(m_text, m_richText);
                string str = stringEx.noEmoji;
                HTextGenerator.customFont = customFont;
                if (sizeFitter != ContentSizeFitter.None)
                {
                    if (marginType != MarginType.None)
                        Margin(this);
                    GetGenerationSettings(ref m_sizeDelta, ref settings);
                    if (sizeFitter == ContentSizeFitter.Horizoantal)
                    {
                        m_sizeDelta.x = HTextGenerator.GetPreferredWidth(stringEx, ref settings);
                    }
                    else if (sizeFitter == ContentSizeFitter.Vertical)
                    {
                        m_sizeDelta.y = HTextGenerator.GetPreferredHeight(stringEx, ref settings);
                    }
                    else if (sizeFitter == ContentSizeFitter.Both)
                    {
                        m_sizeDelta = HTextGenerator.GetPreferredSize(stringEx, ref settings);
                    }
                    Dock(this);
                    settings.generationExtents = m_sizeDelta;
                    HTextGenerator.CreateVertex(stringEx, ref settings);
                }
                else
                {
                    GetGenerationSettings(ref m_sizeDelta, ref settings);
                    HTextGenerator.Populate(stringEx, ref settings);
                }
                int c = HTextGenerator.vertices.Count;
                if (c == 0)
                {
                    vertInfo.DataCount = 0;
                    trisInfo.DataCount = 0;
                    trisInfo1.DataCount = 0;
                    trisInfo2.DataCount = 0;
                    return;
                }
                if (vertInfo.Size == 0)
                {
                    vertInfo = VertexBuffer.RegNew(c);
                }
                else
                if (vertInfo.Size < c | vertInfo.Size > c + 32)
                {
                    vertInfo.Release();
                    vertInfo = VertexBuffer.RegNew(c);
                }
                var vs = HTextGenerator.vertices;
                unsafe
                {
                    HVertex* hv = vertInfo.Addr;
                    for (int i = 0; i < c; i++)
                    {
                        hv[i] = vs[i];
                    }
                }
                tris = null;
                vertInfo.DataCount = c;
                ApplyTris(this, HTextGenerator.Triangle1, HTextGenerator.Triangle2, HTextGenerator.Triangle3);
                m_dirty = false;
                m_vertexChange = true;
                //fillColors[0] = true;
                m_colorChanged = false;
                MainTexture = Font.material.mainTexture;
                if (customFont != null)
                    TTexture = customFont.texture;
                else TTexture = null;
                HTextGenerator.GetPopulateInfo(populates);
            }
        }
        /// <summary>
        /// 更新网格
        /// </summary>
        public override void UpdateMesh()
        {
            if (m_vertexChange)
            {
                if (OutLine > 0)
                    CreateOutLine(this);
                m_vertexChange = false;
                m_colorChanged = false;
            }
        }
        /// <summary>
        /// 获取文字预设高度
        /// </summary>
        /// <param name="size"></param>
        /// <param name="str"></param>
        public void GetPreferredHeight(ref Vector2 size, string str)
        {
            GetGenerationSettings(ref size, ref settings);
            float h = HTextGenerator.GetPreferredHeight(str, ref settings);
            size.y = h;
        }
        /// <summary>
        /// 获取文字预设宽度
        /// </summary>
        /// <param name="size"></param>
        /// <param name="str"></param>
        public void GetPreferredWidth(ref Vector2 size, string str)
        {
            GetGenerationSettings(ref size, ref settings);
            float w = HTextGenerator.GetPreferredWidth(str, ref settings);
            size.x = w;
        }
        /// <summary>
        /// 获取文字预设尺寸
        /// </summary>
        /// <param name="size"></param>
        /// <param name="str"></param>
        public void GetPreferredSize(ref Vector2 size, string str)
        {
            GetGenerationSettings(ref size, ref settings);
            size = HTextGenerator.GetPreferredSize(str, ref settings);
        }
        public HText()
        {
            TextBuffer.Add(this);
        }
        public override void Dispose()
        {
            base.Dispose();
            TextBuffer.Remove(this);
            customFont = null;
        }
        public override void Clear()
        {
            customFont = null;
        }
        /// <summary>
        /// UI尺寸本改变,设置污染
        /// </summary>
        public override void ReSized()
        {
            base.ReSized();
            m_dirty = true;
        }
    }
}
