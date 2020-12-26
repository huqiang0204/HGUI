using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    internal struct TextureInfo
    {
        public Texture texture;
        //public bool fillcolor;//需要替换颜色
        //public int ID;
    }
    internal struct MaterialInfo
    {
        public Material material;
        public int ID;
    }
    /// <summary>
    /// 材质球收集器
    /// </summary>
    internal class MaterialCollector
    {
        static int mLen = 8;
        static Texture[] tmp = new Texture[4];
        /// <summary>
        /// 纹理信息缓存
        /// </summary>
        public TextureInfo[] textures;
        /// <summary>
        /// 材质球缓存
        /// </summary>
        public MaterialInfo[] materials;
        int[] table;
        int max = -1;
        /// <summary>
        /// 子三角形列表
        /// </summary>
        internal List<int[]> submesh = new List<int[]>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="length">缓存大小</param>
        public MaterialCollector(int length = 256)
        {
            materials = new MaterialInfo[length];
            table = new int[length];
            textures = new TextureInfo[length * mLen];
        }
        /// <summary>
        /// 开始新一轮的收集
        /// </summary>
        public void Start()
        {
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = 0;
                materials[i].material = null;
                materials[i].ID = 0;
            }
            tmpMesh.Clear();
            submesh.Clear();
            max = -1;
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
        bool CombinationMaterial(Texture texture, ref int offset)
        {
            if (max < 0)
                goto label;
            if (materials[max].ID == 0)//材质相同
            {
                int c = table[max];//获取当前材质的纹理数量
                int s = max * mLen;//计算材质的起始位置
                for (int i = 0; i < c; i++)
                {
                    if (textures[s].texture == texture)//如果纹理相等
                    {
                        offset = i;
                        return true;
                    }
                    s++;
                }
                if (c < mLen)//如果纹理未填满
                {
                    table[max]++;
                    textures[s].texture = texture;
                    //textures[s].fillcolor = fillcolor;
                    offset = c;
                    return true;
                }
            }
        label:;
            max++;
            offset = 0;
            table[max] = 1;
            materials[max].material = null;
            int o = max * mLen;
            textures[o].texture = texture;
            //textures[o].fillcolor = fillcolor;
            return false;
        }
        /// <summary>
        /// 组合材质球
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="t2d"></param>
        /// <param name="tris"></param>
        /// <param name="len"></param>
        /// <param name="offset"></param>
        public void CombinationMaterial(HGraphics graphics, Texture t2d, int[] tris, int len, ref int offset)
        {
            int id = graphics.MatID;
            if (id == 0)//使用默认材质球
            {
                if (CombinationMaterial(t2d, ref offset))
                {
                    for (int i = 0; i < len; i++)
                        tmpMesh.Add(tris[i]);
                }
                else
                {
                    if (max > 0)
                        CompeleteSub();
                    for (int i = 0; i < len; i++)
                        tmpMesh.Add(tris[i]);
                }
            }
            else//使用自定义材质球
            {
                if (max > -1)
                    CompeleteSub();
                for (int i = 0; i < len; i++)
                    tmpMesh.Add(tris[i]);
                CombinationMaterial(graphics.Material, id);
            }
        }
        /// <summary>
        /// 组合材质球
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="trisArray"></param>
        /// <param name="address"></param>
        /// <param name="offsets"></param>
        /// <param name="len"></param>
        public void CombinationMaterial(HGraphics graphics, int[] trisArray, ArrayInfo[] address, int[] offsets, int len)
        {
            tmp[0] = graphics.MainTexture;
            tmp[1] = graphics.STexture;
            tmp[2] = graphics.TTexture;
            tmp[3] = graphics.FTexture;
            int id = graphics.MatID;
            if (id == 0)//使用默认材质球
            {
                if (trisArray != null)
                {
                    for (int i = 0; i < len; i++)
                    {
                        int l = address[i].Length;
                        if (l > 0)
                        {
                            if (CombinationMaterial(tmp[i], ref offsets[i]))
                            {
                                int s = address[i].Start;
                                for (int j = 0; j < l; j++)
                                {
                                    tmpMesh.Add(trisArray[s]);
                                    s++;
                                }
                            }
                            else
                            {
                                if (max > 0)
                                    CompeleteSub();
                                int s = address[i].Start;
                                for (int j = 0; j < l; j++)
                                {
                                    tmpMesh.Add(trisArray[s]);
                                    s++;
                                }
                            }
                        }
                    }
                }
            }
            else//使用自定义材质球
            {
                if (max > -1)
                    CompeleteSub();
                for (int i = 0; i < len; i++)
                {
                    int s = address[i].Start;
                    int l = address[i].Length;
                    for (int j = 0; j < l; j++)
                    {
                        tmpMesh.Add(trisArray[s]);
                        s++;
                    }
                }
                CombinationMaterial(graphics.Material, id);
            }
        }
        List<int> tmpMesh = new List<int>();
        /// <summary>
        /// 子网格闭合
        /// </summary>
        public void CompeleteSub()
        {
            submesh.Add(tmpMesh.ToArray());
            tmpMesh.Clear();
        }
        /// <summary>
        /// 此轮收集完毕
        /// </summary>
        public void End()
        {
            if (tmpMesh.Count > 0)
            {
                CompeleteSub();
            }
        }
        static string[] tc = new string[] { "_MainTex", "_t1", "_t2", "_t3", "_t4", "_t5", "_t6", "_t7" };
        public int Length { get => max + 1; }
        /// <summary>
        /// 生成材质球数组, 这里会产生一次GC
        /// </summary>
        /// <returns></returns>
        public Material[] GenerateMaterial()
        {
            Reset();
            int len = max + 1;
            Material[] mats = new Material[len];
            for (int i = 0; i < len; i++)
            {
                int c = table[i];
                var mat = materials[i].material;
                if (mat == null)//如果为空,则使用默认材质球
                {
                    mat = GetNextMaterial();
                    int s = i * mLen;
                    for (int j = 0; j < mLen; j++)
                    {
                        if (j < c)
                            mat.SetTexture(tc[j], textures[s].texture);
                        else mat.SetTexture(tc[j], null);
                        s++;
                    }
                }
                mats[i] = mat;
            }
            return mats;
        }
        static Shader shader;
        public int renderQueue = 3100;
        /// <summary>
        /// 默认着色器
        /// </summary>
        internal static Shader DefShader
        {
            get
            {
                if (shader == null)
                    shader = Shader.Find("HGUI/UIDef");//Custom/UIDef
                return shader;
            }
        }
        List<Material> matList = new List<Material>();
        int point;
        public void Reset()
        {
            point = 0;
        }
        /// <summary>
        /// 获取下一个材质球,如果没有则重新创建
        /// </summary>
        /// <returns></returns>
        internal Material GetNextMaterial()
        {
            Material mat;
            if (point == matList.Count)
            {
                mat = new Material(DefShader);
                matList.Add(mat);
            }
            else
            {
                mat = matList[point];
                if (mat == null)
                {
                    mat = new Material(DefShader);
                    matList[point] = mat;
                }
            }

            mat.renderQueue = renderQueue;
            point++;
            return mat;
        }
    }
}
