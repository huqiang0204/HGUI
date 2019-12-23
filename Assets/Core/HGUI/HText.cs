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
        static TextGenerator shareGenerator;
        static TextGenerator generator { get {
                if (shareGenerator == null)
                    shareGenerator = new TextGenerator();
                return shareGenerator;
            } }
        bool _textChanged;
        internal string _text;
        public string Text {
            get => _text;
            set { _text = value; } }
        EmojiString emojiString = new EmojiString();
        IList<UILineInfo> lines;
        IList<UICharInfo> characters;
        IList<UIVertex> verts;
        public Font font { get; set; }
        public Vector2 pivot { get; set; }
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
        public bool updateBounds;
        public int resizeTextMaxSize;
        public int resizeTextMinSize;
        public bool generateOutOfBounds;
        public bool resizeTextForBestFit=false;
        public TextAnchor textAnchor;
        public FontStyle fontStyle;
        public float scaleFactor = 1;
        public bool richText;
        public float lineSpacing;
        public int fontSize;
        public bool alignByGeometry;
        public override void MainUpdate()
        {
            base.MainUpdate();
            if(_textChanged)
            {
                emojiString.FullString = _text;
                TextGenerationSettings settings = new TextGenerationSettings();
                settings.font = font;
                settings.pivot = pivot;
                settings.generationExtents = SizeDelta;
                settings.horizontalOverflow = horizontalOverflow;
                settings.verticalOverflow = verticalOverflow;
                settings.resizeTextMaxSize = fontSize;
                settings.resizeTextMinSize = fontSize;
                settings.generateOutOfBounds = generateOutOfBounds;
                settings.resizeTextForBestFit = resizeTextForBestFit;
                settings.textAnchor = textAnchor;
                settings.fontStyle = fontStyle;
                settings.scaleFactor = scaleFactor;
                settings.richText = richText;
                settings.lineSpacing = lineSpacing;
                settings.fontSize = fontSize;
                settings.color = color;
                settings.alignByGeometry = alignByGeometry;
                var g = generator;
                g.Populate(emojiString.FilterString,settings);
                lines = g.lines;
                verts = g.verts;
                characters = g.characters;
                _textChanged = false;
                _vertexChange = true;
            }
        }
        public override void UpdateMesh()
        {
            if (_vertexChange)
            {
               
                _vertexChange = false;
            }
        }
    }
}
