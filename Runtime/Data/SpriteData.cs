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
        /// 纹理名称
        /// </summary>
        public Vector2 txtSize;
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
        class SpriteCategory
        {
            public string txtName;
            public List<Sprite> sprites = new List<Sprite>();
        }
        List<SpriteCategory> lsc = new List<SpriteCategory>();
        /// <summary>
        /// 添加精灵
        /// </summary>
        /// <param name="sprite">精灵</param>
        public void AddSprite(Sprite sprite)
        {
            if (sprite == null)
                return;
            string tname = sprite.texture.name;
            for (int i = 0; i < lsc.Count; i++)
            {
                if (tname == lsc[i].txtName)
                {
                    lsc[i].sprites.Add(sprite);
                    return;
                }
            }
            SpriteCategory category = new SpriteCategory();
            category.txtName = tname;
            category.sprites.Add(sprite);
            lsc.Add(category);
        }
        FakeStructArray SaveCategory(DataBuffer buffer)
        {
            FakeStructArray array = new FakeStructArray(buffer, 2, lsc.Count);
            for (int i=0;i<lsc.Count;i++)
            {
                array.SetData(i,0,lsc[i].txtName);
                array.SetData(i,1,SaveSprites(buffer,lsc[i].sprites));
            }
            return array;
        }
        unsafe FakeStructArray SaveSprites(DataBuffer buffer,List<Sprite> sprites)
        {
            FakeStructArray array = new FakeStructArray(buffer, SpriteDataS.ElementSize, sprites.Count);
            float tx = sprites[0].texture.width;
            float ty = sprites[0].texture.height;
            for (int i = 0; i < sprites.Count; i++)
            {
                var sprite = sprites[i];
                string name = sprite.name;
                SpriteDataS* sp = (SpriteDataS*)array[i];
                sp->name = buffer.AddData(name);
                sp->txtSize.x = tx;
                sp->txtSize.y = ty;
                var sr = sp->rect = sprite.rect;
                sp->pivot = sprite.pivot;
                float w = sprite.texture.width;
                float h = sprite.texture.width;
                float x = sr.x;
                float rx = sr.width + x;
                float y = sr.y;
                ty = sr.height + y;
                x /= w;
                rx /= w;
                y /= h;
                ty /= h;
                sp->uv0.x = x;
                sp->uv0.y = y;
                sp->uv1.x = x;
                sp->uv1.y = ty;
                sp->uv2.x = rx;
                sp->uv2.y = ty;
                sp->uv3.x = rx;
                sp->uv3.y = y;
            }
            return array;
        }
        /// <summary>
        /// 保存精灵信息为二进制数据
        /// </summary>
        /// <param name="name">数据包名</param>
        /// <param name="path">文件路径</param>
        public void Save(string name,string path)
        {
            DataBuffer buffer = new DataBuffer(4096);
            var fs = buffer.fakeStruct = new FakeStruct(buffer, 2);
            fs.SetData(0, name);
            fs.SetData(1, SaveCategory(buffer));
            byte[] dat = buffer.ToBytes();
            File.WriteAllBytes(path,dat);
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void Clear()
        {
            lsc.Clear();
        }
        FakeStruct fakeStruct;
        /// <summary>
        /// 载入精灵数据包
        /// </summary>
        /// <param name="data">数据</param>
        public void LoadSpriteData(byte[] data)
        {
            fakeStruct = new DataBuffer(data).fakeStruct;
        }
    }
}
