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
        class MaterialInfo
        {
            public Vector4 clip = new Vector4(-10000, -10000, 10000, 10000);
            public Material material;
            public Texture texture;
            public int materialID;
            public int textureID;
            public string texName;
        }
        static Vector4 DefClip = new Vector4(-10000, -10000, 10000, 10000);
        public static Shader DefSahder { get; set; }
        static MaterialInfo[] Buffer = new MaterialInfo[4096];
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
        public static Material Find(int id)
        {
            return Find(id, DefClip);
        }
        public static Material Find(int id, Vector4 clip)
        {
            for (int i = 0; i < Max; i++)
            {
                if (Buffer[i].textureID == id)
                {
                    if(clip==Buffer[i].clip)
                    {
                        return Buffer[i].material;
                    }
                }
            }
            return null;
        }
        public static Material CreateMaterial(Texture texture)
        {
             var mat =new Material(DefSahder);
            MaterialInfo info = new MaterialInfo();
            info.material = mat;
            info.texture = texture;
            if (texture != null)
                info.textureID = texture.GetInstanceID();
            Buffer[Max] = info;
            Max++;
            return mat;
        }
        public static void RelaeseUnused()
        {
            
        }
    }
}
