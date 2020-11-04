using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data
{
    public unsafe struct SpriteDataS
    {
        /// <summary>
        /// 精灵名称
        /// </summary>
        public Int32 name;
        /// <summary>
        /// 精灵矩形
        /// </summary>
        public Rect rect;
        /// <summary>
        /// 精灵轴心
        /// </summary>
        public Vector2 pivot;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
        /// <summary>
        /// 此结构体尺寸
        /// </summary>
        public static int Size = sizeof(SpriteDataS);
        /// <summary>
        /// 此结构体元素个数
        /// </summary>
        public static int ElementSize = Size / 4;
    }
    /// <summary>
    /// 精灵数据
    /// </summary>
    public class SpriteData
    {
        struct SpriteInfo
        {
            public string Name;
            public Rect rect;
            /// <summary>
            /// 精灵轴心
            /// </summary>
            public Vector2 pivot;
            public Vector2[] uv;
        }
        struct TextureInfo
        {
            public string Name;
            public SpriteInfo[] sprites;
            public int width;
            public int height;
        }
        public string Name;
        TextureInfo[] infos;
        FakeStruct fakeStruct;
        /// <summary>
        /// 载入精灵数据包
        /// </summary>
        /// <param name="data">数据</param>
        public void LoadSpriteData(byte[] data)
        {
            fakeStruct = new DataBuffer(data).fakeStruct;
            Name = fakeStruct.GetData<string>(0);
            var fsa = fakeStruct.GetData<FakeStructArray>(1);
            if(fsa!=null)
            {
                int c = fsa.Length;
                infos = new TextureInfo[c];
                for (int i = 0; i < infos.Length; i++)
                {
                    infos[i].Name = fsa.GetData<string>(i, 0);
                    var arr = fsa.GetData<FakeStructArray>(i,1);
                    if(arr!=null)
                    {
                        infos[i].sprites = LoadSpriteData(arr);
                    }
                    infos[i].width = fsa[i,2];
                    infos[i].height = fsa[i, 3];
                }
            }
        }
        SpriteInfo[] LoadSpriteData(FakeStructArray array)
        {
            var db = array.buffer;
            SpriteInfo[] spr = new SpriteInfo[array.Length];
            for (int i = 0; i < spr.Length; i++)
            {
                unsafe
                {
                    SpriteDataS* sp = (SpriteDataS*)array[i];
                    spr[i].rect = sp->rect;
                    spr[i].pivot = sp->pivot;
                    spr[i].Name = db.GetData(sp->name) as string;
                    spr[i].uv = new Vector2[4];
                    spr[i].uv[0] = sp->uv0;
                    spr[i].uv[1] = sp->uv1;
                    spr[i].uv[2] = sp->uv2;
                    spr[i].uv[3] = sp->uv3;
                }
            }
            return spr;
        }
        public Vector2[] FindSpriteUV(string tName, string sName)
        {
            if (infos == null)
                return null;
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == tName)
                {
                    var spr = infos[i].sprites;
                    for (int j = 0; j < spr.Length; j++)
                    {
                        if(spr[j].Name==sName)
                        {
                            return spr[j].uv;
                        }
                    }
                }
            }
            return null;
        }
        public Vector2[][] FindSpriteUVs(string tName, string[] sName)
        {
            if (infos == null)
                return null;
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == tName)
                {
                    var spr = infos[i].sprites;
                    return FindSpriteUVs(spr,sName);
                }
            }
            return null;
        }
        Vector2[][] FindSpriteUVs(SpriteInfo[] spr, string[] sName)
        {
            Vector2[][] uvs = new Vector2[sName.Length][];
            for(int i=0;i<sName.Length;i++)
            {
                string name = sName[i];
                for (int j = 0; j < spr.Length; j++)
                {
                    if (spr[j].Name == name)
                    {
                        uvs[i] = spr[j].uv;
                        break;
                    }
                }
            }
            return uvs;
        }
    }
}
