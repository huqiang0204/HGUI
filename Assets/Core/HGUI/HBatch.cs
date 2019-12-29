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
                for (int i = 0; i < 4096; i++)
                    Table[i] = 0;
                Max = 0;
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os, canvas, Vector3.zero, Vector3.one, Quaternion.identity);
                    os++;
                }
                canvas.CompeleteSub();
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
                                int tid = 0;
                                if(Combination(graphics.MainTexture,graphics.TextureID,ref tid))
                                {
                                    canvas.CombinationMesh(tmp);
                                }
                                else
                                {
                                    canvas.CompeleteSub();
                                    canvas.CombinationMesh(tmp);
                                    canvas.AddMaterial(graphics.material);
                                }
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
        struct TextureInfo
        {
            public Texture texture;
            public int ID;
        }
        static TextureInfo[] textures = new TextureInfo[4096];
        static int[] Table = new Int32[4096];

        static int Max = 0;
        static bool Combination(Texture txt, int id, ref int index, bool not = false)
        {
            if(not)
            {
                index = 0;
                int c = Table[Max];
                if (c > 0)
                    Max++;
                int s = Max * 4;
                textures[s].texture = txt;
                textures[s].ID = id;
                Table[Max] = 1;
                return false;
            }
            else
            {
                int c = Table[Max];
                int s = Max * 4;
                for (int i = 0; i < c; i++)
                {
                    if (textures[s].ID == id)
                    {
                        index = i;
                        return true;
                    }
                    s++;
                }
                if(c<4)
                {
                    s = Max * 4+ Table[Max];
                    textures[s].texture = txt;
                    textures[s].ID = id;
                    Table[Max]++;
                    return true;
                }
                else
                {
                    index = 0;
                    Max++;
                    s = Max * 4;
                    textures[s].texture = txt;
                    textures[s].ID = id;
                    Table[Max] = 1;
                    return false;
                }
            }
        }
    }
}
