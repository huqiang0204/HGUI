using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct PressInfo
    {
        public int Row;
        public int Offset;
    }
    [Serializable]
    public class TextBox:HText
    {
        public override string TypeName { get => UIType.TextBox; }
        static List<int> TrisA = new List<int>();
        static List<int> TrisB = new List<int>();
        static List<int> TrisC = new List<int>();
        /// <summary>
        /// 不会产生网格的字符
        /// </summary>
        protected static char[] key_noMesh = new char[] { ' ', '\n' };//排除\r
        List<HVertex> verts = new List<HVertex>();
        List<TextLineInfo> lines = new List<TextLineInfo>();
        List<CharacterInfoEx> chars = new List<CharacterInfoEx>();
        List<TextLineInfo> showlines = new List<TextLineInfo>();
        List<HVertex> showverts = new List<HVertex>();
        /// <summary>
        /// 开启此项配合Mask效果较好,但是Mask会增加合批材质
        /// </summary>
        public bool SmoothX = true;
        public bool SmoothY = true;
        int startLine;
        int endLine;
        int topLine;
        float StartX;
        float StartY;
        /// <summary>
        /// 最左边的顶点
        /// </summary>
        float vertxstart;
        /// <summary>
        /// 文字最大宽度
        /// </summary>
        float cw;
        /// <summary>
        /// 文字总计高度
        /// </summary>
        float ch;
        public int ShowStartChar { get; private set; }
        public float PercentageX
        {
            get
            {
                if (cw <= m_sizeDelta.x)
                    return 0;
                float r = cw - m_sizeDelta.x;
                return StartX / r;
            }
            set
            {
                if (cw <= m_sizeDelta.x)
                {
                    if (StartX == 0)
                        return;
                    StartX = 0;
                    m_vertexChange = true;
                    return;
                }
                float r = cw - m_sizeDelta.x;
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                value *= r;
                if (StartX == value)
                    return;
                StartX = value;
                m_vertexChange = true;
            }
        }
        public float PercentageY
        {
            get
            {
                if (ch <= m_sizeDelta.y)
                    return 0;
                float r = ch - m_sizeDelta.y;
                return StartY / r;
            }
            set
            {
                if (ch <= m_sizeDelta.y)
                {
                    if (StartY == 0)
                        return;
                    StartY = 0;
                    m_vertexChange = true;
                    return;
                }
                float r = ch - m_sizeDelta.y;
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                value *= r;
                if (StartY == value)
                    return;
                StartY = value;
                m_vertexChange = true;
            }
        }
        public int StartLine
        {
            get => startLine; set
            {
                if (value < 0)
                {
                    if (startLine == 0)
                        return;
                    startLine = 0;
                    StartY = 0;
                    PopulateVertex();
                    return;
                }
                if (value > topLine)
                    value = topLine;
                if (startLine == value)
                    return;
                startLine = value;
                float sy = lines[0].topY - lines[startLine].topY + 1;
                if (sy + m_sizeDelta.y > ch)
                {
                    sy = ch - m_sizeDelta.y;
                    if (sy < 0)
                        sy = 0;
                }
                if (sy == StartY)
                    return;
                StartY = sy;
                PopulateVertex();
            }
        }
        public int TopLine
        {
            get => topLine;
        }
        public int AllLine { get; private set; }
        public float ContentWidth { get => cw; }
        public float ContentHeight { get => ch; }
        void LayoutAnalysis()
        {
            vertxstart = cw = m_sizeDelta.x;
            ch = m_sizeDelta.y;
            float h = 0;
            int c = lines.Count - 1;
            topLine = 0;
            bool top = false;
            for (int i = c; i >= 0; i--)
            {
                if (lines[i].Width > cw)
                    cw = lines[i].Width;
                if (lines[i].Left < vertxstart)
                    vertxstart = lines[i].Left;
                h += lines[i].High;
                if (!top)
                    if (h >= ch)
                    {
                        topLine = i + 1;
                        top = true;
                    }
            }
            if (h > ch)
                ch = h;
        }
        void GetShowContent()
        {
            showlines.Clear();
            showverts.Clear();
            TrisA.Clear();
            TrisB.Clear();
            TrisC.Clear();
            bool frist = false;
            if (lines.Count > 0)
            {
                float sx = vertxstart;
                float ox = StartX + vertxstart - m_sizeDelta.x * -0.5f;
                float sy = lines[0].topY;
                float ey = StartY + m_sizeDelta.y;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (SmoothY)
                    {
                        float dy = sy - lines[i].downY;
                        if (dy >= StartY)
                        {
                            if (!frist)
                            {
                                startLine = i;
                                frist = true;
                            }
                            dy = sy - lines[i].topY;
                            if (dy > ey)//超过下边界
                            {
                                break;
                            }
                            endLine = i;
                            GetShowLine(i, ox);
                        }
                    }
                    else
                    {
                        float dy = sy - lines[i].topY + 1;
                        if (dy >= StartY)
                        {
                            if (!frist)
                            {
                                startLine = i;
                                frist = true;
                            }
                            dy = sy - lines[i].downY;
                            if (dy > ey)//超过下边界
                            {
                                break;
                            }
                            endLine = i;
                            GetShowLine(i, ox);
                        }
                    }
                }
            }
        }
        void GetShowLine(int index, float ox)
        {
            showlines.Add(lines[index]);
            for (int j = lines[index].CharStart; j <= lines[index].CharEnd; j++)
            {
                if (SmoothX)
                {
                    if (chars[j].Right - vertxstart > StartX)
                    {
                        if (chars[j].Left - vertxstart > StartX + m_sizeDelta.x)//超过右边界
                            break;
                        int vi = chars[j].VertexIndex;
                        if (vi >= 0)
                        {
                            GetShowChar(j, vi, ox);
                        }
                    }
                }
                else
                {
                    if (chars[j].Left - vertxstart > StartX)
                    {
                        if (chars[j].Right - vertxstart > StartX + m_sizeDelta.x)//超过右边界
                            break;
                        int vi = chars[j].VertexIndex;
                        if (vi >= 0)
                        {
                            GetShowChar(j, vi, ox);
                        }
                    }
                }
            }
        }
        void GetShowChar(int index, int vi, float ox)
        {
            HVertex vertex = new HVertex();
            int c = showverts.Count;
            int t = chars[index].type;
            if (t == -1)//表情符
            {
                HTextGenerator.AddTris(TrisB, c);
            }
            else if (t == 1)
            {
                HTextGenerator.AddTris(TrisC, c);
            }
            else
            {
                HTextGenerator.AddTris(TrisA, c);
            }
            vi *= 4;
            vertex = verts[vi];
            vertex.position.x -= ox;
            vertex.position.y += StartY;
            showverts.Add(vertex);
            vi++;
            vertex = verts[vi];
            vertex.position.x -= ox;
            vertex.position.y += StartY;
            showverts.Add(vertex);
            vi++;
            vertex = verts[vi];
            vertex.position.x -= ox;
            vertex.position.y += StartY;
            showverts.Add(vertex);
            vi++;
            vertex = verts[vi];
            vertex.position.x -= ox;
            vertex.position.y += StartY;
            showverts.Add(vertex);
        }
        public override void Populate()
        {
            HTextGenerator.AddContext(this, Font);
            if (m_dirty | m_colorChanged)
            {
                if (stringEx == null)
                    stringEx = new StringEx(m_text, m_richText);
                else stringEx.Reset(m_text, m_richText);
                GetGenerationSettings(ref m_sizeDelta, ref settings);
                settings.verticalOverflow = VerticalWrapMode.Overflow;

                verts.Clear();
                lines.Clear();
                chars.Clear();
                HTextGenerator.customFont = customFont;
                HTextGenerator.Populate(stringEx, ref settings);
                HTextGenerator.CopyLinesInfo(lines);
                HTextGenerator.CopyCharsInfo(chars);
                verts.AddRange(HTextGenerator.vertices);
                LayoutAnalysis();
                AllLine = lines.Count;

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
        void PopulateVertex()
        {
            GetShowContent();
            int c = showverts.Count;
            if (c == 0)
            {
                vertInfo.DataCount = 0;
                trisInfo.DataCount = 0;
                trisInfo1.DataCount = 0;
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
            var vs = showverts;
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
            ApplyTris(this, TrisA, TrisB, null);
        }
        public override void UpdateMesh()
        {
            if (m_vertexChange)
            {
                PopulateVertex();
                if (OutLine > 0)
                    CreateOutLine(this);
                m_vertexChange = false;
            }
        }
        public void CheckPoint(UserEvent user, UserAction action, ref PressInfo press)
        {
            if (showlines.Count > 0)
            {
                float ox = StartX + vertxstart - m_sizeDelta.x * -0.5f;
                var offset = action.CanPosition;
                offset.x -= user.GlobalPosition.x;//全局坐标
                offset.y -= user.GlobalPosition.y;
                var q = Quaternion.Inverse(user.GlobalRotation);
                offset = q * offset;
                var scale = user.GlobalScale;//全局尺寸
                offset.x /= scale.x;
                offset.x += ox;
                offset.y /= scale.y;
                offset.y -= StartY;
                int row = 0;
                if (showlines.Count > 0)
                {
                    for (int i = 0; i < showlines.Count; i++)
                    {
                        if (offset.y > showlines[i].topY)
                        {
                            break;
                        }
                        else row = i;
                    }
                }
                int start = showlines[row].CharStart;
                int col = showlines[row].CharEnd - start + 1;
                if (chars[showlines[row].CharEnd].type == -3)//行尾是换行符
                    col--;
                for (int i = start; i < showlines[row].CharEnd + 1; i++)
                {
                    if (offset.x < chars[i].Center)
                    {
                        col = i - start;
                        break;
                    }
                }
                press.Row = showlines[row].Row;
                press.Offset = col;
            }
        }
        /// <summary>
        /// 获取光标的显示网格
        /// </summary>
        /// <param name="tri">三角形列表</param>
        /// <param name="vert">顶点列表</param>
        /// <param name="color">填充颜色</param>
        /// <param name="start">按压信息</param>
        public void GetPointer(List<int> tri, List<HVertex> vert, ref Color32 color, ref PressInfo start)
        {
            int index = GetIndex(ref start);
            if (lines.Count == 0)
            {
                if (index == 0)
                {
                    float h = m_fontSize * HTextGenerator.LineSpacing;
                    float lx = -1.5f;
                    float ty = h * 0.5f;
                    switch (TextAnchor)
                    {
                        case TextAnchor.UpperLeft:
                            lx = m_sizeDelta.x * -0.5f;
                            ty = m_sizeDelta.y * 0.5f;
                            break;
                        case TextAnchor.MiddleLeft:
                            lx = m_sizeDelta.x * -0.5f;
                            break;
                        case TextAnchor.LowerLeft:
                            lx = m_sizeDelta.x * -0.5f;
                            ty = m_sizeDelta.y * -0.5f + h;
                            break;
                    }
                    float rx = lx + 3;
                    float dy = ty - h;
                    var hv = new HVertex();
                    hv.position.x = lx;
                    hv.position.y = dy;
                    hv.color = color;
                    vert.Add(hv);
                    hv.position.x = rx;
                    hv.position.y = dy;
                    hv.color = color;
                    vert.Add(hv);
                    hv.position.x = lx;
                    hv.position.y = ty;
                    hv.color = color;
                    vert.Add(hv);
                    hv.position.x = rx;
                    hv.position.y = ty;
                    hv.color = color;
                    vert.Add(hv);
                    tri.Add(0);
                    tri.Add(2);
                    tri.Add(3);
                    tri.Add(0);
                    tri.Add(3);
                    tri.Add(1);
                }
            }
            if (start.Row < 0)
                return;
            if (start.Row >= lines.Count)
                return;
            if (index < 0)
                return;
            int row = start.Row;
            float ox = StartX + vertxstart - m_sizeDelta.x * -0.5f;
            float top = lines[row].topY + StartY;
            float down = lines[row].downY + StartY;
            float p;
            if (index > lines[row].CharEnd)
            {
                index--;
                if (index < 0)
                    index = 0;
                p = chars[index].Right;
            }
            else
            {
                p = chars[index].Left;
            }
            p -= ox;
            float left = p - 1.5f;
            float right = p + 1.5f;
            var v = new HVertex();
            v.position.x = left;
            v.position.y = down;
            v.color = color;
            vert.Add(v);
            v.position.x = right;
            v.position.y = down;
            v.color = color;
            vert.Add(v);
            v.position.x = left;
            v.position.y = top;
            v.color = color;
            vert.Add(v);
            v.position.x = right;
            v.position.y = top;
            v.color = color;
            vert.Add(v);
            tri.Add(0);
            tri.Add(2);
            tri.Add(3);
            tri.Add(0);
            tri.Add(3);
            tri.Add(1);
        }
        /// <summary>
        /// 获取选中区域的网格
        /// </summary>
        /// <param name="tri">三角形列表</param>
        /// <param name="vert">顶点列表</param>
        /// <param name="color">填充颜色</param>
        /// <param name="start">开始按压位置信息</param>
        /// <param name="end">结束按压位置信息</param>
        public void GetSelectArea(List<int> tri, List<HVertex> vert, ref Color32 color, ref PressInfo start, ref PressInfo end)
        {
            if (lines.Count == 0)
            {
                return;
            }
            int sr = start.Row;
            int er = end.Row + 1;
            int st = 0;
            int c = showlines.Count;
            if (c > er)
                c = er;
            if (sr < 0)
                sr = 0;
            if (sr < showlines[0].Row)
                sr = showlines[0].Row;
            float ox = StartX + vertxstart - m_sizeDelta.x * -0.5f;
            int startIndex = GetIndex(ref start);
            int endIndex = GetIndex(ref end);
            for (int i = 0; i < c; i++)
            {
                if (sr >= lines.Count)
                    break;
                TextLineInfo info = lines[sr];
                int ls = info.CharStart;
                if (ls > endIndex)
                    break;
                int le = lines[sr].CharEnd;
                if (le <= startIndex)
                    continue;
                int si = startIndex;
                int ei = endIndex;
                float left;
                if (si < ls)
                    left = chars[ls].Left;
                else left = chars[si].Left;
                float right;
                if (ei > le)
                    right = chars[le].Right;
                else right = chars[ei].Left;
                left -= ox;
                right -= ox;
                float top = info.topY + StartY;
                float down = info.downY + StartY;

                var v = new HVertex();
                v.position.x = left;
                v.position.y = down;
                v.color = color;
                vert.Add(v);
                v.position.x = right;
                v.position.y = down;
                v.color = color;
                vert.Add(v);
                v.position.x = left;
                v.position.y = top;
                v.color = color;
                vert.Add(v);
                v.position.x = right;
                v.position.y = top;
                v.color = color;
                vert.Add(v);
                tri.Add(st);
                tri.Add(st + 2);
                tri.Add(st + 3);
                tri.Add(st);
                tri.Add(st + 3);
                tri.Add(st + 1);
                st += 4;
                sr++;
            }
        }
        /// <summary>
        /// 光标向上移动
        /// </summary>
        /// <param name="press">当前光标位置</param>
        public void MoveUp(ref PressInfo press)
        {
            CorrectionPress(ref press);
            if (press.Row > 0)
            {
                press.Row--;
                int Index = GetIndex(ref press);
                if (press.Row <= startLine)
                {
                    StartY = lines[0].topY - lines[press.Row].topY;
                    if (StartY < 0)
                        StartY = 0;
                    JumpToChar(press.Row, Index - lines[press.Row].CharStart);
                    PopulateVertex();
                }
                else
                {
                    if (JumpToChar(press.Row, Index - lines[press.Row].CharStart))
                        PopulateVertex();
                }
            }
        }
        /// <summary>
        /// 光标向下移动
        /// </summary>
        /// <param name="press">当前光标位置</param>
        public void MoveDown(ref PressInfo press)
        {
            CorrectionPress(ref press);
            if (press.Row < lines.Count - 1)
            {
                press.Row++;
                int Index = GetIndex(ref press);
                if (press.Row >= endLine)
                {
                    StartY = lines[0].topY - lines[press.Row].downY - m_sizeDelta.y;
                    if (StartY < 0)
                        StartY = 0;
                    JumpToChar(press.Row, Index - lines[press.Row].CharStart);
                    PopulateVertex();
                }
                else
                {
                    if (JumpToChar(press.Row, Index - lines[press.Row].CharStart))
                        PopulateVertex();
                }
            }
        }
        /// <summary>
        /// 光标向左移动
        /// </summary>
        /// <param name="press">当前光标位置</param>
        public void MoveLeft(ref PressInfo press)
        {
            CorrectionPress(ref press);
            int e = lines[press.Row].CharEnd - lines[press.Row].CharStart;
            if (press.Offset > e)
                press.Offset = e + 1;
            press.Offset--;
            if (press.Offset <= 0)
            {
                if (press.Row > 0)
                {
                    press.Row--;
                    press.Offset = lines[press.Row].CharEnd - lines[press.Row].CharStart + 1;
                }
                else press.Offset = 0;
            }
            Move(ref press);
        }
        /// <summary>
        /// 光标向右移动
        /// </summary>
        /// <param name="press">当前光标位置</param>
        /// <param name="count">字符个数</param>
        public void MoveRight(ref PressInfo press, int count)
        {
            CorrectionPress(ref press);
            press.Offset += count;
            if (press.Row >= lines.Count)
                press.Row = lines.Count - 1;
            int e = lines[press.Row].CharEnd - lines[press.Row].CharStart;
            if (press.Offset > e)
            {
                int index = lines[press.Row].CharStart + press.Offset;
                for (int i = press.Row; i < lines.Count; i++)
                {
                    if (lines[i].CharEnd >= index)
                    {
                        press.Row = i;
                        press.Offset = index - lines[i].CharStart;
                        Move(ref press);
                        return;
                    }
                }
                press.Row = lines.Count - 1;
                press.Offset = lines[press.Row].CharEnd + 1;
            }
            Move(ref press);
        }
        /// <summary>
        /// 光标移动到字符串尾部
        /// </summary>
        /// <param name="press">当前光标位置</param>
        public void MoveEnd(ref PressInfo press)
        {
            press.Row = lines.Count - 1;
            if (press.Row < 0)
            {
                press.Row = 0;
                return;
            }
            press.Offset = lines[press.Row].CharEnd + 1;
            Move(ref press);
        }
        /// <summary>
        /// 获取字符串尾部信息
        /// </summary>
        /// <param name="press"></param>
        public void GetEnd(ref PressInfo press)
        {
            if (lines.Count == 0)
            {
                press.Row = 0;
                press.Offset = 0;
                return;
            }
            press.Row = lines.Count - 1;
            press.Offset = lines[press.Row].CharEnd + 1;
        }
        bool JumpToChar(int row, int offset)
        {
            float ox = StartX + vertxstart - m_sizeDelta.x * -0.5f;
            int index = lines[row].CharStart + offset;
            float x;
            if (index > lines[row].CharEnd)//右边
            {
                index = lines[row].CharEnd;
                x = chars[index].Right;
            }
            else
            {
                x = chars[index].Left;
            }
            x -= ox;
            float hw = m_sizeDelta.x * 0.5f;
            if (x > hw)
            {
                StartX += x - hw + 1;
                return true;
            }
            else if (x < -hw)
            {
                StartX = x - m_sizeDelta.x;
                if (StartX < 0)
                    StartX = 0;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 光标移动到指定按压信息
        /// </summary>
        /// <param name="press">按压信息</param>
        public void Move(ref PressInfo press)
        {
            if (press.Row >= lines.Count)
                return;
            int index = lines[press.Row].CharStart + press.Offset;
            float sy = lines[0].topY - lines[press.Row].downY;
            float sx;
            int e = lines[press.Row].CharEnd;
            if (index > e)//右边
            {
                sx = chars[e].Right - vertxstart;
            }
            else
            {
                sx = chars[index].Left - vertxstart;
            }
            bool a = false;
            if (sx < StartX)
            {
                StartX = sx - 1;
                a = true;
            }
            else if (sx > StartX + m_sizeDelta.x)
            {
                StartX = sx - m_sizeDelta.x;
                a = true;
            }
            if (StartX + m_sizeDelta.x > cw)
            {
                StartX = cw - m_sizeDelta.x;
                a = true;
            }
            if (StartX < 0)
                StartX = 0;
            if (sy < StartY)
            {
                StartY = sy - 1;
                a = true;
            }
            else if (sy > StartY + m_sizeDelta.y)
            {
                StartY = sy - m_sizeDelta.y;
                a = true;
            }
            if (StartY + m_sizeDelta.y > ch)
            {
                StartY = ch - m_sizeDelta.y;
                a = true;
            }
            if (StartY < 0)
                StartY = 0;
            if (a)
                PopulateVertex();
        }
        /// <summary>
        /// 使用按压信息获取字符索引
        /// </summary>
        /// <param name="press">按压信息</param>
        /// <returns></returns>
        public int GetIndex(ref PressInfo press)
        {
            if (press.Row < 0)
                return 0;
            if (press.Row >= lines.Count)
                return chars.Count;
            int index = lines[press.Row].CharStart + press.Offset;
            if (index > lines[press.Row].CharEnd + 1)
                index = lines[press.Row].CharEnd + 1;
            if (index <= chars.Count)
                return index;
            return chars.Count;
        }
        /// <summary>
        /// 使用索引获取按压信息
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="press">按压信息</param>
        public void GetPress(int index, ref PressInfo press)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (index <= lines[i].CharEnd + 1)
                {
                    press.Row = i;
                    press.Offset = index - lines[i].CharStart;
                    return;
                }
            }
            press.Row = 0;
            press.Offset = 0;
        }

        /// <summary>
        /// 设置输入法显示位置,win平台
        /// </summary>
        /// <param name="start">光标按压位置</param>
        public void SetCursorPos(ref PressInfo start)
        {
            if (start.Row < 0)
                return;
            if (start.Row >= lines.Count)
                return;
            int index = GetIndex(ref start);
            if (index < 0)
                return;
            if (index >= chars.Count)
                return;
            int row = start.Row;
            for (int i = 0; i < lines.Count; i++)
            {
                if (index <= lines[i].CharEnd)
                {
                    row = i;
                    break;
                }
            }
            float p;
            if (index > lines[row].CharEnd)
            {
                index--;
                if (index < 0)
                    index = 0;
                p = chars[index].Right;
            }
            else
            {
                p = chars[index].Left;
            }
            float right = p + 1;
            float down = lines[row].downY;
            var gl = GetGlobaInfo(this, false);
            float rx = gl.Scale.x * right;
            float rd = gl.Scale.y * down;
            gl.Postion.x += rx;
            gl.Postion.y += rd;
            gl.Postion *= UISystem.PhysicalScale;
            gl.Postion.x += Screen.width / 2;
            gl.Postion.y += Screen.height / 2;
            Keyboard.CursorPos = new Vector2(gl.Postion.x, Screen.height - gl.Postion.y);
        }
        void CorrectionPress(ref PressInfo press)
        {
            if(lines.Count==0)
            {
                press.Row = 0;
                press.Offset = 0;
                return;
            }
            if (press.Row >= lines.Count)
                press.Row = lines.Count - 1;
            if (press.Row < 0)
                press.Row = 0;
        }
    }
}
