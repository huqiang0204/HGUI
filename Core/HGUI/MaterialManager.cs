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
        internal static Shader DefShader
        {
            get
            {
                if (shader == null)
                    shader = Shader.Find("Custom/UIDef");//Custom/UIDef
                return shader;
            }
        }
        static List<Material> materials = new List<Material>();
        static int point;   
        public static void Reset()
        {
            point = 0;
        }
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
            point++;
            return mat;
        }
    }
}
