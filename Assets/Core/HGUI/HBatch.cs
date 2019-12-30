using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    internal struct TextureInfo
    {
        public Texture texture;
        public int ID;
    }
    internal struct MaterialInfo
    {
        public Vector4 clip;
        public Material material;
        public int ID;
    }
    class MaterialCollector
    {
        public TextureInfo[] textures;
        public MaterialInfo[] materials;
        int[] table;
        int max = 0;
        
        public MaterialCollector(int length=1024)
        {
            materials = new MaterialInfo[length];
            table = new int[length];
            textures = new TextureInfo[length*4];
        }
        public void Start()
        {
            for(int i=0;i<table.Length;i++)
            {
                table[i] = 0;
                materials[i].material = null;
                materials[i].ID = -1;
            }
            tmpMesh.Clear();
        }
        public bool CombinationMaterial(Material mat, int matID, Texture texture, int texID, ref int offset)
        {
            if (materials[max].ID == matID)//材质相同
            {
                int c = table[max];//获取当前材质的纹理数量
                int s = max * 4;//计算材质的起始位置
                for (int i = 0; i < c; i++)
                {
                    if (textures[s].ID == texID)//如果纹理相等
                    {
                        offset = i;
                        return true;
                    }
                    s++;
                }
                if (c < 4)//如果4张纹理未填满
                {
                    table[max]++;
                    textures[s].texture = texture;
                    textures[s].ID = texID;
                    offset = c;
                    return true;
                }
            }
            max++;
            offset = 0;
            table[max] = 1;
            materials[max].material = mat;
            materials[max].ID = matID;
            int o = max * 4;
            textures[o].texture = texture;
            textures[o].ID = texID;
            return false;
        }
        List<int[]> tmpMesh = new List<int[]>();
        public void CombinationMesh(int[] sub)
        {
            tmpMesh.Add(sub);
        }
        public void End()
        {

        }
        public Material[] GenerateMaterial()
        {
            int len = max + 1;
            Material[] mats = new Material[len];
            for (int i = 0; i <len; i++)
            {
                int c = table[i];
                var mat = materials[i].material;
                if (mat == null)//如果为空,则使用默认材质球
                {
                    mat = new Material(HGraphics.DefShader);
                    int s = i * 4;
                    if (c > 0)
                    {
                        mat.SetTexture("_MainTex", textures[s].texture);
                        if (c > 1)
                        {
                            s++;
                            mat.SetTexture("_STex", textures[s].texture);
                            if (c > 2)
                            {
                                s++;
                                mat.SetTexture("_TTex", textures[s].texture);
                                if (c > 3)
                                {
                                    s++;
                                    mat.SetTexture("_FTex", textures[s].texture);
                                }
                            }
                        }
                    }
                }
                mat.SetVector("_ClipRect",materials[i].clip);
                mats[i] = mat;
            }
            return mats;
        }
    }
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
                        Vector2[] uv2 = new Vector2[vert.Length];
                        for (int j = 0; j < vert.Length; j++)
                        {
                            var t = q * vert[j];
                            t.x *= s.x;
                            t.y *= s.y;
                            vs.Add(o + t);
                            uv2[j].x = (t.x + 10000) / 20000;
                            uv2[j].y = (t.y + 10000) / 20000;
                        }
                        canvas.uv2.AddRange(uv2);
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
