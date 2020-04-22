using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    internal class HBatch
    {
        public static void Batch(HCanvas canvas, HGUIElement[] pipeLine)
        {
            HGUIElement root = pipeLine[0];
            if (root.script != null)
            {
                canvas.MatCollector.Start();
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    if(pipeLine[os].active)
                        Batch(pipeLine, os, canvas, Vector3.zero, Vector3.one, Quaternion.identity, new Vector4(0, 0, 1, 1));
                    os++;
                }
                canvas.MatCollector.End();
            }
        }
        static void Batch(HGUIElement[] pipeLine, int index, HCanvas canvas, Vector3 pos, Vector3 scale, Quaternion quate,Vector4 clip)
        {
            if (!pipeLine[index].active)
                return;
            HGUIElement root = pipeLine[index];
            Vector3 p = quate * pipeLine[index].localPosition;
            Vector3 o = Vector3.zero;
            o.x = p.x * scale.x;
            o.y = p.y * scale.y;
            o.z = p.z * scale.z;
            o += pos;
            Vector3 s = pipeLine[index].localScale;
            scale.x *= s.x;
            scale.y *= s.y;
            Quaternion q = quate * pipeLine[index].localRotation;

            bool mask = false;
            var script = root.script;
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
                    lx *= s.x;
                    rx *= s.x;
                    dy *= s.y;
                    ty *= s.y;
                    Vector4 v = new Vector4(o.x + lx, o.y + dy, o.x + rx, o.y + ty);
                    v.x += 10000;
                    v.x /= 20000;
                    v.y += 10000;
                    v.y /= 20000;
                    v.z += 10000;
                    v.z /= 20000;
                    v.w += 10000;
                    v.w /= 20000;
                    clip = CutRect(clip, v);
                }
                var graphics = script as HGraphics;
                if (graphics != null)//计算图形
                {
                    if (graphics.Shadow)
                        AddShadow(graphics,canvas,ref q,ref scale,ref o,ref clip);
                    var vs = canvas.vertex;
                    var vc = vs.Count;
                    var vert = graphics.vertices;
                    if (vert != null)
                    {
                        float px = (0.5f - script.Pivot.x) * script.SizeDelta.x;
                        float py = (0.5f - script.Pivot.y) * script.SizeDelta.y;  
                        Vector2 uv2 = Vector2.zero;
                        for (int j = 0; j < vert.Length; j++)
                        {
                            var tp = vert[j].position;//局部顶点
                            tp.x += px;
                            tp.y += py;
                            var t = q * tp;
                            t.x *= scale.x;
                            t.y *= scale.y;
                            t += o;
                            t.z = 0;
                            uv2.x = (t.x + 10000) / 20000;
                            uv2.y = (t.y + 10000) / 20000;
                            vs.Add(t);
                            canvas.colors.Add(vert[j].color);
                            canvas.uv.Add(vert[j].uv);
                            canvas.uv2.Add(uv2);
                            canvas.uv3.Add(vert[j].uv3);
                            canvas.uv4.Add(vert[j].uv4);
                        }
                        if (graphics.tris != null)
                        {
                            int tid = 0;
                            var src = graphics.tris;
                            if (src.Length > 0)
                            {
                                int[] tmp = new int[src.Length];
                                for (int k = 0; k < tmp.Length; k++)
                                {
                                    tmp[k] = src[k] + vc;
                                }
                                canvas.MatCollector.CombinationMaterial(graphics, tmp, ref tid, ref clip);
                            }
                            AddUV1(canvas,tid,vert.Length);
                        }
                        else if (graphics.subTris != null)
                        {
                            var subs = graphics.subTris;
                            int l = subs.Length;
                            if (l > 0)
                            {
                                int[] ids = new int[l];
                                int[][] buf = new int[l][];
                                for (int i = 0; i < l; i++)
                                {
                                    var src = subs[i];
                                    int[] tmp = new int[src.Length];
                                    for (int k = 0; k < tmp.Length; k++)
                                    {
                                        tmp[k] = src[k] + vc;
                                    }
                                    buf[i] = tmp;
                                }
                                canvas.MatCollector.CombinationMaterial(graphics, buf, ids, ref clip);
                                AddUV1(canvas,ids,vert);
                            }
                            else
                            {
                                AddUV1(canvas, 0, vert.Length);
                            }
                        }
                        else
                        {
                            AddUV1(canvas, 0, vert.Length);
                        }
                    }
                }
            }
        
            int c = root.childCount;
            int os = root.childOffset;
            for (int i = 0; i < c; i++)
            {
                Batch(pipeLine, os, canvas, o, scale, q, clip);
                os++;
            }
        }
        static void AddShadow(HGraphics graphics,HCanvas canvas, ref Quaternion q,ref Vector3 scale,ref Vector3 o, ref Vector4 clip)
        {
            var vs = canvas.vertex;
            var vc = vs.Count;
            var vert = graphics.vertices;
            var os = graphics.shadowOffsset;
            if (vert != null)
            {
                float px = (0.5f - graphics.Pivot.x) * graphics.SizeDelta.x+os.x;
                float py = (0.5f - graphics.Pivot.y) * graphics.SizeDelta.y+os.y;
                Vector2 uv2 = Vector2.zero;
                for (int j = 0; j < vert.Length; j++)
                {
                    var tp = vert[j].position;//局部顶点
                    tp.x += px;
                    tp.y += py;
                    var t = q * tp;
                    t.x *= scale.x;
                    t.y *= scale.y;
                    t += o;
                    t.z = 0;
                    uv2.x = (t.x + 10000) / 20000;
                    uv2.y = (t.y + 10000) / 20000;
                    vs.Add(t);
                    canvas.colors.Add(graphics.shadowColor);
                    canvas.uv.Add(vert[j].uv);
                    canvas.uv2.Add(uv2);
                    canvas.uv3.Add(vert[j].uv3);
                    canvas.uv4.Add(vert[j].uv4);
                }
                if (graphics.tris != null)
                {
                    int tid = 0;
                    var src = graphics.tris;
                    if (src.Length > 0)
                    {
                        int[] tmp = new int[src.Length];
                        for (int k = 0; k < tmp.Length; k++)
                        {
                            tmp[k] = src[k] + vc;
                        }
                        canvas.MatCollector.CombinationMaterial(graphics, tmp, ref tid, ref clip);
                    }
                    AddUV1(canvas, tid, vert.Length);
                }
                else if (graphics.subTris != null)
                {
                    var subs = graphics.subTris;
                    int l = subs.Length;
                    if (l > 0)
                    {
                        int[] ids = new int[l];
                        int[][] buf = new int[l][];
                        for (int i = 0; i < l; i++)
                        {
                            var src = subs[i];
                            int[] tmp = new int[src.Length];
                            for (int k = 0; k < tmp.Length; k++)
                            {
                                tmp[k] = src[k] + vc;
                            }
                            buf[i] = tmp;
                        }
                        canvas.MatCollector.CombinationMaterial(graphics, buf, ids, ref clip);
                        AddUV1(canvas, ids, vert);
                    }
                    else
                    {
                        AddUV1(canvas, 0, vert.Length);
                    }
                }
                else
                {
                    AddUV1(canvas, 0, vert.Length);
                }
            }
        }
        static void AddUV1(HCanvas canvas, int tid, int vertCount)
        {
            Vector2[] uv1 = new Vector2[vertCount];
            switch (tid)
            {
                case 1:
                    for (int i = 0; i < uv1.Length; i++)
                        uv1[i].y = 1;
                    break;
                case 2:
                    for (int i = 0; i < uv1.Length; i++)
                        uv1[i].x = 1;
                    break;
                case 3:
                    for (int i = 0; i < uv1.Length; i++)
                    {
                        uv1[i].x = 1;
                        uv1[i].y = 1;
                    }
                    break;
            }
            canvas.uv1.AddRange(uv1);
        }
        static Vector2[] UV1 = new Vector2[4];
        static void AddUV1(HCanvas canvas, int[] ids, HVertex[] vertices)
        {
            int len = vertices.Length;
            Vector2[] uv1 = new Vector2[len];
            for (int i = 0; i < ids.Length; i++)
            {
                switch (ids[i])
                {
                    case 0:
                        UV1[i].x = 0;
                        UV1[i].y = 0;
                        break;
                    case 1:
                        UV1[i].x = 0;
                        UV1[i].y = 1;
                        break;
                    case 2:
                        UV1[i].x = 1;
                        UV1[i].y = 0;
                        break;
                    case 3:
                        UV1[i].x = 1;
                        UV1[i].y = 1;
                        break;
                }
            }
            for (int i = 0; i < len; i++)
                uv1[i] = UV1[vertices[i].picture];
            canvas.uv1.AddRange(uv1);
        }
        static Vector4 CutRect(Vector4 v0,Vector4 v1)
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
