using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    /// <summary>
    /// 材质管理器
    /// </summary>
    public class MaterialManager
    {
        static int Max;
        static Shader shader;
        public static int renderQueue = 3100;
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
        static List<Material> materials = new List<Material>();
        static int point;   
        public static void Reset()
        {
            point = 0;
        }
        /// <summary>
        /// 获取下一个材质球,如果没有则重新创建
        /// </summary>
        /// <returns></returns>
        internal static Material GetNextMaterial()
        {
            Material mat;
            if(point == materials.Count)
            {
                mat = new Material(DefShader);
                materials.Add(mat);
            }
            else
            {
                mat = materials[point];
                if (mat == null)
                {
                    mat = new Material(DefShader);
                    materials[point] = mat;
                }
            }

            mat.renderQueue = renderQueue;
            point++;
            return mat;
        }
    }
}
