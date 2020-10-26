using huqiang.Core.Line;
using huqiang.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// 画线UI,支持直线,弧线,贝塞尔曲线,二阶贝塞尔曲线
    /// </summary>
    public class HLine:HGraphics
    {
        List<Beeline> beelines;
        List<ArcLine> arcLines;
        List<BzierLine> bzierLines;
        List<BzierLine2> bzierLines2;
        /// <summary>
        /// 添加一条直线
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(ref Beeline line)
        {
            if (beelines == null)
                beelines = new List<Beeline>();
            beelines.Add(line);
            m_dirty = true;
        }
        /// <summary>
        /// 添加一条弧线
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(ref ArcLine line)
        {
            if (arcLines == null)
                arcLines = new List<ArcLine>();
            if (line.Precision <= 0)
                line.Precision = 0.01f;
            arcLines.Add(line);
            m_dirty = true;
        }
        /// <summary>
        /// 添加一条贝塞尔曲线
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(ref BzierLine line)
        {
            if (bzierLines == null)
                bzierLines = new List<BzierLine>();
            if (line.Precision <= 0)
                line.Precision = 0.01f;
            bzierLines.Add(line);
            m_dirty = true;
        }
        /// <summary>
        /// 添加一条二阶贝塞尔曲线
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(ref BzierLine2 line)
        {
            if (bzierLines2 == null)
                bzierLines2 = new List<BzierLine2>();
            if (line.Precision <= 0)
                line.Precision = 0.01f;
            bzierLines2.Add(line);
            m_dirty = true;
        }
        public void AddParabola(ref ParabolaLine line)
        {

        }
        /// <summary>
        /// 清除所有线段
        /// </summary>
        public override void Clear()
        {
            if (beelines != null)
                beelines.Clear();
            if (arcLines != null)
                arcLines.Clear();
            if (bzierLines != null)
               bzierLines.Clear();
            if (bzierLines2 != null)
                bzierLines2.Clear();
            vertInfo.DataCount = 0;
            trisInfo.DataCount = 0;
        }
        int GetSize()
        {
            int c = 0;
            if (beelines != null)
                c = beelines.Count * 4;
            if(arcLines!=null)
            {
                for(int i=0;i<arcLines.Count;i++)
                {
                    float f = arcLines[i].Precision;
                    int a =(int)( 1 / f)+2;
                    c += a * 4;
                }
            }
            if(bzierLines!=null)
            {
                for (int i = 0; i < bzierLines.Count; i++)
                {
                    float f = bzierLines[i].Precision;
                    int a = (int)(1 / f) + 2;
                    c += a * 4;
                }
            }
            if(bzierLines2!=null)
            {
                for (int i = 0; i < bzierLines2.Count; i++)
                {
                    float f = bzierLines2[i].Precision;
                    int a = (int)(1 / f) + 2;
                    c += a * 4;
                }
            }
            return c;
        }
        /// <summary>
        /// 更新所有线段网格
        /// </summary>
        public override void UpdateMesh()
        {
            if(m_dirty)
            {
                int c = GetSize();
                if (c > vertInfo.Size | c+32 < vertInfo.Size)
                {
                    vertInfo.Release();
                    vertInfo = HGUIMesh.blockBuffer.RegNew(c);
                    int tc = c / 4 * 6;
                    trisInfo.Release();
                    trisInfo = trisBuffer.RegNew(tc);
                }
                vertInfo.DataCount = 0;
                trisInfo.DataCount = 0;
                m_dirty = false;
                if(beelines!=null)
                {
                    for (int i = 0; i < beelines.Count; i++)
                    {
                        var tmp = beelines[i];
                        CreateBeeLine(ref tmp); 
                    }
                }
                if(arcLines!=null)
                {
                    for (int i = 0; i < arcLines.Count; i++)
                    {
                        var tmp = arcLines[i];
                        CreateArcLine(ref tmp);
                    }
                }
                if(bzierLines!=null)
                {
                    for (int i = 0; i < bzierLines.Count; i++)
                    {
                        var tmp = bzierLines[i];
                        CreateBzier(ref tmp);
                    }
                }
                if(bzierLines2!=null)
                {
                    for (int i = 0; i < bzierLines2.Count; i++)
                    {
                        var tmp = bzierLines2[i];
                        CreateBzier2(ref tmp);
                    }
                }
            }
        }
        static void CreateVert(ref Vector2 start, ref Vector2 end, ref BlockInfo<HVertex> vert, ref BlockInfo<int> tris,ref Color32 color, float lineWidth)
        {
            float vx = end.x - start.x;
            float vy = end.y - start.y;
            float r = Mathf.Sqrt(lineWidth * lineWidth / (vx * vx + vy * vy));
            float nx = vx * r;
            float ny = vy * r;
            int index = vert.DataCount;
            unsafe
            {
                HVertex* p =vert.Addr;
                p[index].position.x = start.x + ny;
                p[index].position.y = start.y - nx;
                p[index].position.z = 0;
                p[index].color = color;
                p[index].picture = 0;
                index++;
                p[index].position.x = start.x - ny;
                p[index].position.y = start.y + nx;
                p[index].position.z = 0;
                p[index].color = color;
                p[index].picture = 0;
                index++;
                p[index].position.x = end.x - ny;
                p[index].position.y = end.y + nx;
                p[index].position.z = 0;
                p[index].color = color;
                p[index].picture = 0;
                index++;
                p[index].position.x = end.x + ny;
                p[index].position.y = end.y - nx;
                p[index].position.z = 0;
                p[index].color = color;
                p[index].picture = 0;
            }
            index = vert.DataCount;
            int t = tris.DataCount;
            unsafe
            {
                int* p = tris.Addr;
                p[t] = index;
                t++;
                p[t] = index + 1;
                t++;
                p[t] = index + 3;
                t++;
                p[t] = index + 3;
                t++;
                p[t] = index + 1;
                t++;
                p[t] = index+ 2;
                t++;
            }
            vert.DataCount += 4;
            tris.DataCount += 6;
        }
        void CreateBeeLine(ref Beeline beeline)
        {
            CreateVert(ref beeline.Start, ref beeline.End, ref vertInfo,ref trisInfo, ref beeline.lineBase.Color,beeline.lineBase.Width);
        }
        void CreateArcLine(ref ArcLine line)
        {
            int Part =(int)(1 / line.Precision);
            float p = line.Angle / Part;
            Part++;
            float sp = 0 - line.Angle / 2;
            Quaternion q = Quaternion.Euler(0, 0, line.Dic);
            bool start = false;
            Vector2 s = Vector2.zero;
            Vector2 frist=Vector2.zero;
            for (int i = 0; i < Part; i++)
            {
                float a = sp;
                if (a < 0)
                    a += 360;
                else if (sp > 360)
                    a -= 360;
                Vector2 v = MathH.Tan2(a);
                v.x *= line.Radius;
                v.y *= line.Radius;
                v.x *= line.Scale.x;
                v.y *= line.Scale.y;
                Vector2 e = q * v;
                if(start)
                {
                    CreateVert(ref s, ref e, ref vertInfo, ref trisInfo, ref line.lineBase.Color, line.lineBase.Width);
                }
                s = e;
                sp += p;
                start = true;
                if (i == 0)
                    frist = e;
            }
            if(line.Closed)
            {
                CreateVert(ref s, ref frist, ref vertInfo, ref trisInfo, ref line.lineBase.Color,line.lineBase.Width);
            }
        }
        void CreateBzier(ref BzierLine line)
        {
            int Part = (int)(1 / line.Precision);
            float t = line.Precision;
            Vector2 s = line.A;
            for (int i = 0; i < Part; i++)
            {
                var e = MathH.BezierPoint(t,ref line.A,ref line.B,ref line.C);
                CreateVert(ref s, ref e, ref vertInfo, ref trisInfo, ref line.lineBase.Color, line.lineBase.Width);
                s = e;
                t += line.Precision;
            }
            CreateVert(ref s, ref line.C, ref vertInfo, ref trisInfo, ref line.lineBase.Color, line.lineBase.Width);
        }
        void CreateBzier2(ref BzierLine2 line)
        {
            int Part = (int)(1 / line.Precision);
            float t = line.Precision;
            Vector2 s = line.A;
            for (int i = 0; i < Part; i++)
            {
                var e = MathH.BezierPoint(t, ref line.A, ref line.B, ref line.C,ref line.D);
                CreateVert(ref s, ref e, ref vertInfo, ref trisInfo, ref line.lineBase.Color,line.lineBase.Width);
                s = e;
                t += line.Precision;
            }
            CreateVert(ref s, ref line.D, ref vertInfo, ref trisInfo, ref line.lineBase.Color, line.lineBase.Width);
        }
    }
}
