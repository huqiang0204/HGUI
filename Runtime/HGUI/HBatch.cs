using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct ArrayInfo
    {
        public int Start;
        public int Length;
    }
    /// <summary>
    /// HGUI图像合批处理
    /// </summary>
    internal class HBatch
    {
        static int[] TriBuffer = new int[65536];
        static int[] IDBuffer = new int[32];
        static ArrayInfo[] Arrays = new ArrayInfo[8];
        /// <summary>
        /// 合批处理
        /// </summary>
        /// <param name="canvas">画布实例</param>
        /// <param name="pipeLine">UI流水线</param>
        public static void Batch(HCanvas canvas, HGUIElement[] pipeLine)
        {
            HGUIElement root = pipeLine[0];
            if (root.script != null)
            {
                canvas.MatCollector.Start();
                int c = root.childCount;
                int os = root.childOffset;
                float hw = canvas.m_sizeDelta.x * 0.5f;
                float hh = canvas.m_sizeDelta.y * 0.5f;
                for (int i = 0; i < c; i++)
                {
                    if (pipeLine[os].active)
                    {
                        Batch(pipeLine, os, canvas, new Vector4(-hw, -hh, hw, hh), Vector2.zero, new Vector2(hw, hh));
                    }
                    os++;
                }
                canvas.MatCollector.End();
            }
        }
        static void Batch(HGUIElement[] pipeLine, int index, HCanvas canvas, Vector4 clip, Vector2 center, Vector2 half)
        {
            if (!pipeLine[index].active)
                return;

            Vector3 o = pipeLine[index].Position;
            Vector3 scale = pipeLine[index].Scale;
            Quaternion q = pipeLine[index].Rotation;

            bool mask = false;
            var script = pipeLine[index].script;
            if (script != null)
            {
                mask = script.Mask;
                if (mask)//计算遮挡区域
                {
                    float x = script.SizeDelta.x;
                    float y = script.SizeDelta.y;
                    float px = script.Pivot.x;
                    float py = script.Pivot.y;
                    float lx = x * -px;
                    float rx = x + lx;
                    float dy = y * -py;
                    float ty = y + dy;
                    lx *= scale.x;
                    rx *= scale.x;
                    dy *= scale.y;
                    ty *= scale.y;
                    Vector4 v = new Vector4(o.x + lx, o.y + dy, o.x + rx, o.y + ty);
                    clip = CutRect(clip, v);
                    half.x = (clip.z - clip.x) * 0.5f;
                    half.y = (clip.w - clip.y) * 0.5f;
                    center.x = half.x + clip.x;
                    center.y = half.y + clip.y;
                }
                var graphics = script as HGraphics;
                if (graphics != null)//计算图形
                {
                    if (graphics.Shadow)
                        AddShadow(graphics, canvas, ref q, ref scale, ref o, ref clip, ref center, ref half);
                    var vs = canvas.vertex;
                    var vc = vs.Count;
                    int dc = graphics.vertInfo.DataCount;
                    if (dc > 0)
                    {
                        float px = (0.5f - script.Pivot.x) * script.m_sizeDelta.x;
                        float py = (0.5f - script.Pivot.y) * script.m_sizeDelta.y;
                        Vector2 uv2 = Vector2.zero;
                        unsafe
                        {
                            HVertex* hv = graphics.vertInfo.Addr;
                            for (int j = 0; j < dc; j++)
                            {
                                var tp = hv[j].position;//局部顶点
                                tp.z = 0;
                                tp.x += px;
                                tp.y += py;
                                tp.x *= scale.x;
                                tp.y *= scale.y;
                                var t = q * tp;
                                t += o;
                                t.z = 0;
                                uv2.x = (t.x - center.x) / half.x * 0.001f + 0.5f;
                                uv2.y = (t.y - center.y) / half.y * 0.001f + 0.5f;
                                vs.Add(t);
                                canvas.colors.Add(hv[j].color);
                                canvas.uv.Add(hv[j].uv);
                                canvas.uv2.Add(uv2);
                                canvas.uv3.Add(hv[j].uv3);
                                canvas.uv4.Add(hv[j].uv4);
                            }
                        }
                        AddTris(graphics, canvas, vc, dc);
                    }
                }
            }

            int c = pipeLine[index].childCount;
            int os = pipeLine[index].childOffset;
            for (int i = 0; i < c; i++)
            {
                Batch(pipeLine, os, canvas, clip, center, half);
                os++;
            }
        }
        static void AddTris(HGraphics graphics, HCanvas canvas, int vc, int dc)
        {
            int max = vc + dc;
            if (graphics.tris != null)
            {
                int tid = 0;
                var src = graphics.tris;
                int len = src.Length;
                if (len > 0)
                {
                    for (int k = 0; k < len; k++)
                    {
                        int index = TriBuffer[k] = src[k] + vc;
                        if (index >= max)
                            Debug.LogError("数组超标");
                    }
                    canvas.MatCollector.CombinationMaterial(graphics, graphics.MainTexture, TriBuffer, len, ref tid);
                }
                AddUV1(canvas, tid, ref graphics.vertInfo);
            }
            else if (graphics.trisInfo.DataCount > 0 | graphics.trisInfo1.DataCount > 0 | graphics.trisInfo2.DataCount > 0)
            {
                Arrays[0].Length = 0;
                Arrays[1].Length = 0;
                Arrays[2].Length = 0;
                int l = 0;
                int tc = graphics.trisInfo.DataCount;
                if (tc > 0)
                {
                    unsafe
                    {
                        int* ip = graphics.trisInfo.Addr;
                        for (int k = 0; k < tc; k++)
                        {
                            TriBuffer[k] = ip[k] + vc;
                        }
                    }
                    l = 1;
                }
                Arrays[0].Length = tc;
                int tc2 = graphics.trisInfo1.DataCount;
                if (tc2 > 0)
                {
                    int ks = tc;
                    unsafe
                    {
                        int* ip = graphics.trisInfo1.Addr;
                        for (int k = 0; k < tc2; k++)
                        {
                            TriBuffer[ks] = ip[k] + vc;
                            ks++;
                        }
                    }
                    Arrays[1].Start = tc;
                    Arrays[1].Length = tc2;
                    l = 2;
                }
                int tc3 = graphics.trisInfo2.DataCount;
                if (tc3 > 0)
                {
                    int ks = tc + tc2;
                    unsafe
                    {
                        int* ip = graphics.trisInfo2.Addr;
                        for (int k = 0; k < tc3; k++)
                        {
                            TriBuffer[ks] = ip[k] + vc;
                            ks++;
                        }
                    }
                    Arrays[2].Start = tc + tc2;
                    Arrays[2].Length = tc3;
                    l = 3;
                }
                canvas.MatCollector.CombinationMaterial(graphics, TriBuffer, Arrays, IDBuffer, l);
                unsafe
                {
                    HVertex* hv = graphics.vertInfo.Addr;
                    AddUV1(canvas, IDBuffer, hv, dc, l);
                }
            }
            else
            {
                AddUV1(canvas, 0, ref graphics.vertInfo);
            }
        }
        /// <summary>
        /// 添加阴影
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="canvas"></param>
        /// <param name="q"></param>
        /// <param name="scale"></param>
        /// <param name="o"></param>
        /// <param name="clip"></param>
        static void AddShadow(HGraphics graphics, HCanvas canvas, ref Quaternion q, ref Vector3 scale, ref Vector3 o, ref Vector4 clip, ref Vector2 center, ref Vector2 half)
        {
            var vs = canvas.vertex;
            var vc = vs.Count;
            int dc = graphics.vertInfo.DataCount;
            var os = graphics.shadowOffsset;
            if (dc > 0)
            {
                float px = (0.5f - graphics.Pivot.x) * graphics.SizeDelta.x + os.x;
                float py = (0.5f - graphics.Pivot.y) * graphics.SizeDelta.y + os.y;
                Vector2 uv2 = Vector2.zero;
                unsafe
                {
                    HVertex* vert = graphics.vertInfo.Addr;
                    for (int j = 0; j < dc; j++)
                    {
                        var tp = vert[j].position;//局部顶点
                        tp.x += px;
                        tp.y += py;
                        var t = q * tp;
                        t.x *= scale.x;
                        t.y *= scale.y;
                        t += o;
                        t.z = 0;
                        uv2.x = (t.x - center.x) / half.x * 0.001f + 0.5f;
                        uv2.y = (t.y - center.y) / half.y * 0.001f + 0.5f;
                        vs.Add(t);
                        canvas.colors.Add(graphics.shadowColor);
                        canvas.uv.Add(vert[j].uv);
                        canvas.uv2.Add(uv2);
                        canvas.uv3.Add(vert[j].uv3);
                        canvas.uv4.Add(vert[j].uv4);
                    }
                }
                AddTris(graphics, canvas, vc, dc);
            }
        }
        /// <summary>
        /// 添加UV
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="tid"></param>
        /// <param name="vertCount"></param>
        static void AddUV1(HCanvas canvas, int tid, ref BlockInfo<HVertex> dat)
        {
            Vector2 v = Vector2.zero;
            v.x = tid;
            v.x *= 0.1f;
            var dc = dat.DataCount;
            unsafe
            {
                var hv = dat.Addr;
                for (int i = 0; i < dc; i++)
                {
                    v.y = hv[i].fillColor;
                    canvas.uv1.Add(v);
                }
            }
        }
        static Vector2[] UV1 = new Vector2[4];
        unsafe static void AddUV1(HCanvas canvas, int[] ids, HVertex* vertices, int vc, int l)
        {
            for (int i = 0; i < l; i++)
            {
                float x = ids[i];
                UV1[i].x = x * 0.1f;
                UV1[i].y = 0;
            }
            for (int i = 0; i < vc; i++)
            {
                int p = vertices[i].picture;
                if (p < 0 | p > 3)
                    Debug.LogError("out of index");
                else
                {
                    UV1[p].y = vertices[i].fillColor;
                    canvas.uv1.Add(UV1[p]);
                }
            }

        }
        /// <summary>
        /// 矩形遮罩
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        static Vector4 CutRect(Vector4 v0, Vector4 v1)
        {
            if (v0.x < v1.x)
                v0.x = v1.x;
            if (v0.y < v1.y)
                v0.y = v1.y;
            if (v0.z > v1.z)
                v0.z = v1.z;
            if (v0.w > v1.w)
                v0.w = v1.w;
            return v0;
        }
    }
}
