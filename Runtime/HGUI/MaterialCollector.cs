using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    internal struct TextureInfo
    {
        public Texture texture;
        public bool fillcolor;//需要替换颜色
        public int ID;
    }
    internal struct MaterialInfo
    {
        public Vector4 clip;
        public Material material;
        public int ID;
    }
    /// <summary>
    /// 材质球收集器
    /// </summary>
    internal class MaterialCollector
    {
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
        public MaterialCollector(int length = 1024)
        {
            materials = new MaterialInfo[length];
            table = new int[length];
            textures = new TextureInfo[length * 4];
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
            materials[0].clip.x = -10000;
            materials[0].clip.y = -10000;
            materials[0].clip.z = 10000;
            materials[0].clip.w = 10000;
        }
        /// <summary>
        /// 添加自定义材质球,无法合批
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="matID"></param>
        /// <returns></returns>
        void CombinationMaterial(Material mat, int matID, ref Vector4 clip)
        {
            max++;
            table[max] = 1;
            materials[max].material = mat;
            materials[max].ID = matID;
            materials[max].clip = clip;
        }
        /// <summary>
        /// 组合默认材质球
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="texID"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        bool CombinationMaterial(Texture texture, int texID, bool fillcolor, ref int offset, ref Vector4 clip,bool mask)
        {
            if (max < 0)
                goto label;
            if(!mask)
            {
                if (materials[max].ID == 0)//材质相同
                {
                    if (materials[max].clip != clip)
                        goto label;
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
                        textures[s].fillcolor = fillcolor;
                        offset = c;
                        return true;
                    }
                }
            }
        label:;
            max++;
            offset = 0;
            table[max] = 1;
            materials[max].material = null;
            materials[max].ID = 0;
            materials[max].clip = clip;
            int o = max * 4;
            textures[o].texture = texture;
            textures[o].ID = texID;
            textures[o].fillcolor = fillcolor;
            return false;
        }
        /// <summary>
        /// 组合材质球
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="tris"></param>
        /// <param name="len"></param>
        /// <param name="offset"></param>
        /// <param name="clip"></param>
        public void CombinationMaterial(HGraphics graphics, int[] tris, int len, ref int offset, ref Vector4 clip)
        {
            int id = graphics.MatID;
            if (id == 0)//使用默认材质球
            {
                bool mask = graphics.Mask;
                if (CombinationMaterial(graphics.textures[0], graphics.texIds[0], graphics.fillColors[0], ref offset, ref clip,mask))
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
                CombinationMaterial(graphics.Material, id, ref clip);
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
        /// <param name="clip"></param>
        public void CombinationMaterial(HGraphics graphics, int[] trisArray,ArrayInfo[] address, int[] offsets, int len, ref Vector4 clip)
        {
            int id = graphics.MatID;
            if (id == 0)//使用默认材质球
            {
                if (trisArray != null)
                {
                    bool mask = graphics.Mask;
                    for (int i = 0; i < len; i++)
                    {
                        if (CombinationMaterial(graphics.textures[i], graphics.texIds[i],graphics.fillColors[i], ref offsets[i], ref clip, mask))
                        {
                            int s = address[i].Start;
                            int l = address[i].Length;
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
                            int l = address[i].Length;
                            for (int j = 0; j < l; j++)
                            {
                                tmpMesh.Add(trisArray[s]);
                                s++;
                            }
                        }
                        mask = false;
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
                CombinationMaterial(graphics.Material, id, ref clip);
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
        static string[] tc = new string[] { "_MainTex" , "_STex","_TTex" , "_FTex" };
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
                    Vector4 v = Vector4.zero;
                    int s = i * 4;
                    for (int j = 0; j < 4; j++)
                    {
                        if (j < c)
                            mat.SetTexture(tc[j], textures[s].texture);
                        else mat.SetTexture(tc[j], null);
                        s++;
                    }
                    s = i * 4;
                    if (c > 0)
                    {
                        if (textures[s].fillcolor)
                            v.x = 1;
                        if (c > 1)
                        {
                            s++;
                            if (textures[s].fillcolor)
                                v.y = 1;
                            if (c > 2)
                            {
                                s++;
                                if (textures[s].fillcolor)
                                    v.z = 1;
                                if (c > 3)
                                {
                                    s++;
                                    if (textures[s].fillcolor)
                                        v.w = 1;
                                }
                            }
                        }
                    }
                    mat.SetVector("_FillColor", v);
                }
                mat.SetVector("_Rect", materials[i].clip);
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
