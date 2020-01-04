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
            Quaternion q = quate * pipeLine[index].localRotation;

            bool mask = false;
            if (root.script != null)
            {
                mask = root.script.Mask;
                if (mask)//计算遮挡区域
                {
                    float x = root.script.SizeDelta.x;
                    float y = root.script.SizeDelta.y;
                    float hx = x * 0.5f;
                    hx *= s.x;
                    float hy = y * 0.5f;
                    hy *= s.y;
                    Vector4 v = new Vector4(o.x - hx, o.y - hy, o.x + hx, o.y + hy);
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
                var graphics = root.script as HGraphics;
                if (graphics != null)//计算图形
                {
                    var vs = canvas.vertex;
                    var vc = vs.Count;
                    var vert = graphics.vertex;
                    if (vert != null)
                    {
                        Vector2[] uv2 = new Vector2[vert.Length];
                        for (int j = 0; j < vert.Length; j++)
                        {
                            var t = q * vert[j];
                            t.x *= s.x;
                            t.y *= s.y;
                            t += o;
                            t.z = 0;
                            vs.Add( t);
                            uv2[j].x = (t.x + 10000) / 20000;
                            uv2[j].y = (t.y + 10000) / 20000;
                        }
                        canvas.uv2.AddRange(uv2);
                        if (graphics.Colors == null)
                        {
                            var col = graphics.m_color;
                            for (int j = 0; j < vert.Length; j++)
                            {
                                canvas.colors.Add(col);
                            }
                        }
                        else
                        {
                            if (graphics.Colors.Length == 0)
                            {
                                var col = graphics.m_color;
                                for (int j = 0; j < vert.Length; j++)
                                {
                                    canvas.colors.Add(col);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < graphics.Colors.Length; j++)
                                    canvas.colors.Add(graphics.Colors[j]);
                            }
                        }
                        canvas.uv.AddRange(graphics.uv);
                        int tid = 0;
                        if (graphics.tris != null)
                        {
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
                            }
                        }
                        Vector2[] uv1 = new Vector2[vert.Length];
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
                }
            }
            s.x *= scale.x;
            s.y *= scale.y;
            int c = root.childCount;
            int os = root.childOffset;
            for (int i = 0; i < c; i++)
            {
                Batch(pipeLine, os, canvas, o, s, q, clip);
                os++;
            }
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
