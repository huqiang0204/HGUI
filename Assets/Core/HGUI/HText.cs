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
                    };
                }
                return defFont;
            }
            set
            {
                defFont = value;
            }
        }
        static Texture _emoji;
        public static Texture emojiTexture
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
        Color[] colors;
        EmojiString emojiString = new EmojiString();
        IList<UILineInfo> lines;
        IList<UICharInfo> characters;
        IList<UIVertex> verts;
        internal int[] secondTris;
        internal Font _font;
        public Font font { get => _font;
            set {
                _font = value;
            } }
        internal Vector2 _pivot = new Vector2(0.5f,0.5f);
        public Vector2 pivot { get => _pivot; set { _pivot = value; } }
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
        public bool updateBounds;
        private int resizeTextMaxSize = 40;
        private int resizeTextMinSize = 10;
        private bool generateOutOfBounds;
        public bool resizeForBestFit = false;
        public TextAnchor textAnchor;
        public FontStyle fontStyle;
        public float scaleFactor = 1;
        public bool richText;
        public float lineSpacing;
        public int fontSize =14;
        public bool alignByGeometry;
        public Material emojiMaterial;
        public override void Initial()
        {
            font = DefaultFont;
            emojiMaterial = new Material(defShader);
            emojiMaterial.SetTexture("_MainTex",emojiTexture);
        }
        public override void MainUpdate()
        {
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
                settings.resizeTextForBestFit = resizeForBestFit;
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
                text.secondTris = null;
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
            text.colors = colors;
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
                text.tris = triA;
                text.secondTris = triB;
            }
            else
            {
                text.tris = CreateTri(c);
                text.secondTris = null;
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
#if UNITY_EDITOR
        public void Test()
        {
            _textChanged = true;
            MainUpdate();
            UpdateMesh();
            var mesh = GetComponent<MeshFilter>();
            if (mesh != null)
            {
                mesh.sharedMesh.triangles = null;
                mesh.sharedMesh.vertices = null;
                mesh.sharedMesh.uv = null;

                mesh.sharedMesh.vertices = vertex;
                mesh.sharedMesh.uv = uv;
                mesh.sharedMesh.subMeshCount = 2;
                mesh.sharedMesh.SetTriangles(tris,0);
                mesh.sharedMesh.SetTriangles(secondTris,1);
            }
            var mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                //material.SetTexture("_MainTex",_font.material);
                mr.materials = new Material[] { font.material,emojiMaterial};
            }
        }
#endif
    }
}
