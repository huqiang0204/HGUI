using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    internal class HBatch
    {
        public static void Batch(HCanvas canvas, GUIElement[] pipeLine)
        {
            GUIElement root = pipeLine[0];
            if (root.script != null)
            {
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os, canvas, Vector3.zero, Vector3.one, Quaternion.identity);
                    os++;
                }
            }
        }
        static void Batch(GUIElement[] pipeLine, int index, HCanvas canvas, Vector3 pos, Vector3 scale, Quaternion quate)
        {
            GUIElement root = pipeLine[index];
            Vector3 p = quate * pipeLine[index].localPosition;
            Vector3 o = Vector3.zero;
            o.x = p.x * scale.x;
            o.y = p.y * scale.y;
            o.z = p.z * scale.z;
            o += pos;
            Vector3 s = pipeLine[index].localScale;
            Quaternion q = quate * pipeLine[index].localRotation;
     
            if (root.active)
            {
                if (root.script != null)
                {
                    var graphics = root.script as HGraphics;
                    var vs = canvas.vertex;
                    var vc = vs.Count;
                    var vert = graphics.vertex;
                    if (vert != null)
                    {
                        for (int j = 0; j < vert.Length; j++)
                        {
                            var t = q * vert[j];
                            t.x *= s.x;
                            t.y *= s.y;
                            vs.Add(o + t);
                        }
                        if (graphics.Colors == null)
                        {
                            var col = graphics.Color;
                            for (int j = 0; j < vert.Length; j++)
                            {
                                canvas.colors.Add(col);
                            }
                        }
                        else
                        {
                            if (graphics.Colors.Length == 0)
                            {
                                var col = graphics.Color;
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
                                canvas.submesh.Add(tmp);
                                canvas.AddMaterial(graphics.GetMaterial(0, canvas));
                            }
                        }
                    }
                }
                s.x *= scale.x;
                s.y *= scale.y;
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os, canvas, o, s, q);
                    os++;
                }
            }
        }
        public static void MainBatch(HCanvas canvas,GUIElement [] pipeLine)
        {

        }

        public static int AddTexture(Texture txt, int id, bool force = false)
        {

            return 0;
        }
    }
}
