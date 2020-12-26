using huqiang.Core.UIData;
using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Helper.HGUI
{
    public struct TextVertex
    {
        public Vector3 position;
        public Color32 color;
        public Vector2 uv;
        public int Index;
    }
    public class HText: HGraphics
    {
        static Font defFont;
        /// <summary>
        /// 默认字体
        /// </summary>
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
        [HideInInspector]
        public CustomFont customFont;
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
        protected void SaveToHText(Core.HGUI.HText ui, bool activeSelf,bool haveChild)
        {
            ui.Text = m_text;
            ui.m_pivot = m_pivot;
            ui._font = _font;
            ui.m_hof = m_hof;
            ui.m_vof = m_vof;
            ui.updateBounds = updateBounds;
            ui.ResizeForBestFit = m_resizeBestFit;
            ui.anchor = anchor;
            ui.m_fontSize = m_fontSize;
            ui.m_fontStyle = m_fontStyle;
            ui.m_richText = m_richText;
            ui.m_lineSpace = m_lineSpace;
            ui.m_align = m_align;
            ui.sizeFitter = sizeFitter;
            ui.OutLine = OutLine;
            ui.customFont = customFont;
            SaveToHGraphics(ui,activeSelf,haveChild);
        }
        public override Core.HGUI.UIElement ToHGUI2(bool activeSelf,bool haveChild=true)
        {
            if (STexture == null)
                STexture = UnityEngine.Resources.Load<Texture>("Emoji");
            Core.HGUI.HText htxt = new Core.HGUI.HText();
            SaveToHText(htxt,activeSelf,haveChild);
            return htxt;
        }
        public override void ToHGUI2(Core.HGUI.UIElement ui, bool activeSelf)
        {
            if (STexture == null)
                STexture = UnityEngine.Resources.Load<Texture>("Emoji");
            SaveToHText(ui as Core.HGUI.HText, activeSelf,false);
        }
        public void Reset()
        {
            STexture = UnityEngine.Resources.Load<Texture>("Emoji");
        }
    }
}
