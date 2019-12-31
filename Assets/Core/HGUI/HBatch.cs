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
    internal class MaterialCollector
    {
        public TextureInfo[] textures;
        public MaterialInfo[] materials;
        int[] table;
        int max = 0;
        internal List<int[]> submesh = new List<int[]>();
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
            submesh.Clear();
        }
        /// <summary>
        /// 添加自定义材质球,无法合批
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="matID"></param>
        /// <returns></returns>
        void CombinationMaterial(Material mat, int matID)
        {
            max++;
            table[max] = 1;
            materials[max].material = mat;
            materials[max].ID = matID;
        }
        /// <summary>
        /// 组合默认材质球
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="texID"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        bool CombinationMaterial(Texture texture, int texID, ref int offset)
        {
            if (materials[max].ID == 0)//材质相同
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
            materials[max].material = null;
            materials[max].ID = 0;
            int o = max * 4;
            textures[o].texture = texture;
            textures[o].ID = texID;
            return false;
        }
        public void CombinationMaterial(HGraphics graphics, int[] tris, ref int offset)
        {
            int id = graphics.MatID;
            if (id == 0)//使用默认材质球
            {
                if (CombinationMaterial(graphics.textures[0], graphics.texIds[0], ref offset))
                {
                    CombinationMesh(tris);
                }
                else
                {
                    CompeleteSub();
                    CombinationMesh(tris);
                }
            }
            else//使用自定义材质球
            {
                CombinationMaterial(graphics.Material, id);
            }
        }
        public void CombinationMaterial(HGraphics graphics,int[][] trisArray,int[] offsets)
        {
            int id = graphics.MatID;
            if (id == 0)//使用默认材质球
            {
                if (trisArray != null)
                {
                    int c = trisArray.Length;
                    for (int i = 0; i < c; i++)
                    {
                        if (CombinationMaterial(graphics.textures[i], graphics.texIds[i], ref offsets[i]))
                        {
                            CombinationMesh(trisArray[i]);
                        }
                        else
                        {
                            CompeleteSub();
                            CombinationMesh(trisArray[i]);
                        }
                    }
                }
            }
            else//使用自定义材质球
            {
                CombinationMaterial(graphics.Material, id);
            }
        }
        List<int[]> tmpMesh = new List<int[]>();
        public void CombinationMesh(int[] sub)
        {
            tmpMesh.Add(sub);
        }
        public void CompeleteSub()
        {
            int c = tmpMesh.Count;
            int all = 0;
            for (int i = 0; i < c; i++)
                all += tmpMesh[i].Length;
            int[] buf = new int[all];
            int s = 0;
            for (int i = 0; i < c; i++)
            {
                var t = tmpMesh[i];
                for (int j = 0; j < t.Length; j++)
                {
                    buf[s] = t[j];
                    s++;
                }
            }
            submesh.Add(buf);
        }
        public void End()
        {
            if(tmpMesh.Count>0)
            {
                CompeleteSub();
            }
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
                canvas.Collector.Start();
                int c = root.childCount;
                int os = root.childOffset;
                for (int i = 0; i < c; i++)
                {
                    Batch(pipeLine, os, canvas, Vector3.zero, Vector3.one, Quaternion.identity);
                    os++;
                }
                canvas.Collector.End();
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
                                canvas.Collector.CombinationMaterial(graphics,tmp,ref tid);
                            }
                        }else if(graphics.subTris!=null)
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
                                canvas.Collector.CombinationMaterial(graphics, buf, ids);
                            }
                        }
                        Vector2[]  uv1 = new Vector2[vert.Length];
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
    }
}
