using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextBox:HText
    {
        List<TextVertex> verts = new List<TextVertex>();
        List<LineInfo> lines = new List<LineInfo>();
        List<UICharInfo> chars = new List<UICharInfo>();
        int startLine;
        float StartX;
        float EndX;
        float offsetX;
        float StartY;
        float EndY;
        float cw;
        float ch;
        public float PercentageX
        {
            get 
            {
                if (cw <= m_sizeDelta.x)
                    return 0;
                return offsetX / (cw - m_sizeDelta.x);
            }
            set 
            {
                if(cw>m_sizeDelta.x)
                {
                    if (value < 0)
                        value = 0;
                    else if (value > 1)
                        value = 1;
                    offsetX = value * (cw - m_sizeDelta.x);
                    m_vertexChange = true;
                }
            }
        }
        public float PercentageY {
            get
            {
                if (AllLine == 0)
                    return 0;
                if (AllLine <= ShowRow)
                    return 0;
                float a = startLine;
                float b = AllLine;
                float c = ShowRow;
                return a/ (b - c);
            }
            set
            {
                if (AllLine > 0)
                {
                    if (AllLine > ShowRow)
                    {
                        if (value < 0)
                            value = 0;
                        else if (value > 1)
                            value = 1;
                        int r = AllLine - ShowRow;
                        float s = value * r;
                        startLine = (int)s;
                        m_vertexChange = true;
                    }
                }
            }
        }
        public int StartLine { get => startLine;set {
                if (value < 0)
                    value = 0;
                else if (value+ ShowRow > AllLine)
                    value = AllLine - ShowRow;
                if(value!=startLine)
                {
                    startLine = value;
                    m_vertexChange = true;
                }
            } }
        public int AllLine { get; private set; }
        public int ShowRow { get; private set; }
        public float ContentWidth { get => cw; }
        public float ContentHeight { get => ch; }
        public void Apply()
        {
            if(m_dirty)
            {
                GetGenerationSettings(ref m_sizeDelta, ref settings);
                var g = Generator;
                g.Populate(m_text, settings);
            }
        }
        protected void GetTempVertex(IList<UIVertex> v, List<TextVertex> vert, string filterStr)
        {
            TextVertex tv = new TextVertex();
            int o = 0;
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
                        tv.position = v[o].position;
                        tv.uv = v[o].uv0;
                        tv.color = v[o].color;
                        tv.Index = i;
                        vert.Add(tv);
                        o++;
                    }
                    if (o >= v.Count)
                        break;
                }
            }
        }
        public override void Populate()
        {
            GetGenerationSettings(ref m_sizeDelta, ref settings);
            settings.horizontalOverflow = HorizontalWrapMode.Overflow;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            emojiString.FullString = m_text;
            string str = emojiString.FilterString;
            var g = Generator;
            g.Populate(str, settings);
            verts.Clear();
            lines.Clear();
            chars.Clear();
            chars.AddRange(g.characters);
            LineInfo line = new LineInfo();
            int c = g.lineCount;
            AllLine = c;
            cw = m_sizeDelta.x;
            EndX = m_sizeDelta.x * 0.5f;
            StartX = -EndX;
            if (g.characterCountVisible > 0)
            {
                if (m_richText)
                {
                    emojiString.FullString = RichTextHelper.DeleteLabel(m_text);
                    str = emojiString.FilterString;
                }
                GetTempVertex(g.verts,verts,str);
                int s = c - 1;
                StartY = g.lines[0].topY;
                EndY = g.lines[s].topY - g.lines[s].height - g.lines[s].leading;
                ch = StartY- EndY;
                float per = ch / g.lines.Count;
                ShowRow = (int)(m_sizeDelta.y / per);
                var l = g.lines[0];
                for (int i = 1; i < c ; i++)
                {
                    var n = g.lines[i];
                    line.startCharIdx = l.startCharIdx;
                    line.endIdx = n.startCharIdx - 1;
                    line.topY = l.topY;
                    line.endY = n.topY;
                    l = n;
                    lines.Add(line);
                    float lx = chars[line.startCharIdx].cursorPos.x;
                    float rx = chars[line.endIdx].cursorPos.x;
                    float w = rx - lx + chars[line.endIdx].charWidth;
                    if (w > cw)
                    { 
                        cw = w;
                        StartX = lx;
                        EndX = lx + w;
                    }
                }
                l = g.lines[s];
                line.startCharIdx = l.startCharIdx;
                line.endIdx = g.characterCountVisible - 1;
                line.topY = l.topY;
                line.endY = EndY;
                lines.Add(line);
                float olx = chars[line.startCharIdx].cursorPos.x;
                float orx = chars[line.endIdx].cursorPos.x;
                float ow = orx - olx + chars[line.endIdx].charWidth;
                if (ow > cw)
                {
                    cw = ow;
                    StartX = olx;
                    EndX = olx + ow;
                }
            }
            else
            {
                TmpVerts.DataCount = 0;
                trisInfo.DataCount = 0;
                trisInfo2.DataCount = 0;
                ShowRow = (int)m_sizeDelta.y / FontSize;
                ch = 0;
                StartY = 0;
                EndY = 0;
            }
            if (startLine + ShowRow >= AllLine)
            {
                startLine = AllLine - ShowRow;
                if (startLine <= 0)
                    startLine = 0;
            }
            if (cw < m_sizeDelta.x)
            {
                offsetX = 0;
            }
            else if(offsetX+m_sizeDelta.x>cw)
            {
                offsetX = cw - m_sizeDelta.x;
            }
            m_dirty = false;
            m_vertexChange = true;
            fillColors[0] = true;
            m_colorChanged = false;
            MainTexture = Font.material.mainTexture;
        }
        void PopulateVertex()
        {
            int c = verts.Count;
            if (c == 0)
            {
                TmpVerts.DataCount = 0;
                trisInfo.DataCount = 0;
                trisInfo2.DataCount = 0;
                return;
            }
            else
            {
                int s = lines[startLine].startCharIdx;
                int e = startLine + ShowRow - 1;
                if (e >= lines.Count)
                    e = lines.Count - 1;
                e = lines[e].endIdx + 1;
                c = (e - s) * 4;
                if (c > TmpVerts.Size | TmpVerts.Size > c + 32)
                {
                    TmpVerts.Release();
                    TmpVerts = PopulateBuffer.RegNew(c);
                }
                float oy = 0;
                float ox = offsetX;
                if(cw>m_sizeDelta.x)
                {
                    ox = offsetX + StartX + m_sizeDelta.x * 0.5f;
                }
                if (ch > m_sizeDelta.y)
                {
                    oy = lines[startLine].topY - StartY+ StartY - m_sizeDelta.y * 0.5f ;
                }
                float rx = m_sizeDelta.x * 0.5f + m_fontSize;
                float lx = -rx;
                int ac = 0;
                int max = verts.Count;
                var chs = chars;
                unsafe
                {
                    TextVertex* hv = TmpVerts.Addr;
                    c = e - s;
                    int l = startLine;
                    int ol = ShowRow;
                    if (ol > lines.Count)
                        ol = lines.Count;
                    int next = 0;
                    for (int i = 0; i < ol; i++)
                    {
                        int start = lines[l].startCharIdx;
                        int oc = lines[l].endIdx - start + 1;
                        for(int k=0;k<verts.Count;k+=4)
                        {
                            if(verts[k].Index>=start)
                            {
                                next = k;
                                break;
                            }
                        }
                        for (int k = 0; k < oc; k++)
                        {
                            if (next >= verts.Count)
                                break;
                            var ax = verts[next].position.x;
                            var bx = verts[next + 1].position.x;
                            float cx = (bx - ax) * 0.5f + ax - ox;
                            if (cx < lx)
                            {
                                next += 4;
                            }
                            else if (cx > rx)
                            {
                                break;
                            }
                            else
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    hv[ac] = verts[next];
                                    hv[ac].position.y -= oy;
                                    hv[ac].position.x -= ox;
                                    ac++;
                                    next++;
                                }
                            }
                            start++;
                        }
                        l++;
                    }
                }
                TmpVerts.DataCount = ac;
            }
        }
        public override void UpdateMesh()
        {
            if (m_vertexChange)
            {
                PopulateVertex();
                CreateEmojiMesh(this);
                if (OutLine > 0)
                    CreateOutLine(this);
                m_vertexChange = false;
            }
        }
    }
}
