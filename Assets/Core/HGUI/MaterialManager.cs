using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.HGUI
{
    /// <summary>
    /// 材质管理器
    /// </summary>
    public class MaterialManager
    {
        public static Shader DefSahder { get; set; }
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
        public static void RelaeseUnused()
        {
            
        }
    }
}
