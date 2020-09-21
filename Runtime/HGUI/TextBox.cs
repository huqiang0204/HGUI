using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextBox:HText
    {
        List<UIVertex> verts = new List<UIVertex>();
        List<LineInfo> lines = new List<LineInfo>();
        List<UICharInfo> chars = new List<UICharInfo>();
        public float PercentageX
        {
            get 
            {
                if (ContentWidth <= m_sizeDelta.x)
                    return 0;
                return offsetX / (ContentWidth - m_sizeDelta.x);
            }
            set 
            {
                if(ContentWidth>m_sizeDelta.x)
                {
                    if (value < 0)
                        value = 0;
                    else if (value > 1)
                        value = 1;
                    offsetX = value * (ContentWidth - m_sizeDelta.x);
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
                float a = StartLine;
                float b = AllLine;
                float c = ShowRow;
                return a/ (b - c);
            }
            set
            {
                if(AllLine>0)
                {
                    if(AllLine>ShowRow)
                    {
                        if (value < 0)
                            value = 0;
                        else if (value > 1)
                            value = 1;
                        int r = AllLine - ShowRow;
                        float s = value * r;
                        StartLine = (int)s;
                        m_vertexChange = true;
                    }
                }
            }
        }
        public int StartIndex { get; set; }
        public int StartLine { get; set; }
        public int AllLine { get; private set; }
        public int ShowRow { get; private set; }
        float StartX;
        float EndX;
        float offsetX;
        float ContentWidth;
        float ContentHeight;
        public void Apply()
        {
            if(m_dirty)
            {
                GetGenerationSettings(ref m_sizeDelta, ref settings);
                var g = Generator;
                g.Populate(m_text, settings);
            }
        }
        public override void Populate()
        {
            GetGenerationSettings(ref m_sizeDelta, ref settings);
            emojiString.FullString = m_text;
            var str = emojiString.FilterString;
            var g = Generator;
            g.Populate(str, settings);
            verts.Clear();
            lines.Clear();
            chars.Clear();
            verts.AddRange(g.verts);
            chars.AddRange(g.characters);
            LineInfo line = new LineInfo();
            int c = g.lineCount;
            AllLine = c;
            ContentWidth = m_sizeDelta.x;
            EndX = m_sizeDelta.x * 0.5f;
            StartX = -EndX;
            if (c > 0)
            {
                int s = c - 1;
                ContentHeight = g.lines[0].topY - g.lines[s].topY + g.lines[s].height + g.lines[s].leading;
                int t = c - 1;
                float per = ContentHeight / g.lines.Count;
                ShowRow = (int)(m_sizeDelta.y / per);
                var l = g.lines[0];
                int vc = 0;
                for (int i = 1; i < c ; i++)
                {
                    var n = g.lines[i];
                    line.startCharIdx = l.startCharIdx;
                    line.vertIndex = vc;
                    line.endIdx = n.startCharIdx - 1;
                    int tc = 0;
                    for (int j = line.startCharIdx; j < n.startCharIdx; j++)
                        if (chars[j].charWidth > 0)
                            tc++;
                    line.visibleCount = tc;
                    vc += tc;
                    line.topY = l.topY;
                    line.endY = n.topY;
                    l = n;
                    lines.Add(line);
                    float lx = chars[line.startCharIdx].cursorPos.x;
                    float rx = chars[line.endIdx].cursorPos.x;
                    float w = rx - lx + chars[line.endIdx].charWidth;
                    if (w > ContentWidth)
                    { 
                        ContentWidth = w;
                        StartX = lx;
                        EndX = lx + w;
                    }
                }
                l = g.lines[t];
                line.startCharIdx = l.startCharIdx;
                line.endIdx = g.characterCountVisible - 1;
                int otc = 0;
                for (int j = line.startCharIdx; j < g.characterCountVisible; j++)
                    if (chars[j].charWidth > 0)
                        otc++;
                line.vertIndex = vc;
                line.visibleCount = otc;
                line.topY = l.topY;
                line.endY = g.lines[0].topY - ContentHeight - g.lines[g.lines.Count - 1].leading;
                lines.Add(line);
                float olx = chars[line.startCharIdx].cursorPos.x;
                float orx = chars[line.endIdx].cursorPos.x;
                float ow = orx - olx + chars[line.endIdx].charWidth;
                if (ow > ContentWidth)
                {
                    ContentWidth = ow;
                    StartX = olx;
                    EndX = olx + ow;
                }
            }
            else
            {
                ShowRow = (int)m_sizeDelta.y / FontSize;
                ContentHeight = 0;
            }
            if (StartLine + ShowRow >= AllLine)
            {
                StartLine = AllLine - ShowRow;
                if (StartLine <= 0)
                    StartLine = 0;
            }
            if (ContentWidth < m_sizeDelta.x)
            {
                offsetX = 0;
            }
            else if(offsetX+m_sizeDelta.x>ContentWidth)
            {
                offsetX = ContentWidth - m_sizeDelta.x;
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
                int s = lines[StartLine].startCharIdx;
                int e = StartLine + ShowRow - 1;
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
                if(ContentWidth>m_sizeDelta.x)
                {
                    ox = offsetX + StartX + m_sizeDelta.x * 0.5f;
                }
                if (ContentHeight > m_sizeDelta.y)
                    oy = lines[0].topY - lines[StartLine].topY;
                float rx = m_sizeDelta.x * 0.5f+m_fontSize;
                float lx = -rx;
                int ac = 0;
                int max = verts.Count;
                unsafe
                {
                    TextVertex* hv = TmpVerts.Addr;
                    c = e - s;
                    int l = StartLine;
                    int ol = ShowRow;
                    if (ol > lines.Count)
                        ol = lines.Count;
                    for (int i = 0; i < ol; i++)
                    {
                        int os = lines[l].vertIndex * 4;
                        int oc = lines[l].visibleCount;
                        for (int k = 0; k < oc; k++)
                        {
                            var ax = verts[os].position.x;
                            var bx = verts[os + 1].position.x;
                            float cx = (bx - ax) * 0.5f + ax - ox;
                            if (cx < lx)
                            {
                                os += 4;
                            }
                            else if (cx > rx)
                            {
                                break;
                            }
                            else
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    hv[ac].position = verts[os].position;
                                    hv[ac].position.y += oy;
                                    hv[ac].position.x -= ox;
                                    hv[ac].uv = verts[os].uv0;
                                    hv[ac].color = verts[os].color;
                                    ac++;
                                    os++;
                                }
                            }
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
        public void GetShowContent()
        {

        }
    }
}
