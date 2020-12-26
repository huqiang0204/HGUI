using huqiang.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.UIData
{
    /// <summary>
    /// 自定义文字
    /// </summary>
    public class CustomFont
    {
        struct UVRect
        {
            public char cha;
            /// <summary>
            /// 左下
            /// </summary>
            public Vector2 uv0;
            /// <summary>
            /// 左上
            /// </summary>
            public Vector2 uv1;
            /// <summary>
            /// 右上
            /// </summary>
            public Vector2 uv2;
            /// <summary>
            /// 右下
            /// </summary>
            public Vector2 uv3;
            /// <summary>
            /// 比例
            /// </summary>
            public Vector2 Scale;
            public Vector2 Pivot;
        }
        float width, height;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t2d">包含字符精灵的纹理</param>
        public CustomFont(Texture t2d)
        {
            texture = t2d;
            width = t2d.width;
            height = t2d.height;
        }
        public Texture texture { get; private set; }
        List<UVRect> cash = new List<UVRect>();
        /// <summary>
        /// 添加字符图
        /// </summary>
        /// <param name="key">字符</param>
        /// <param name="rect">精灵矩形</param>
        public void AddCharMap(char key, Rect rect,float fontSize)
        {
            float x0 = rect.x / width;
            float y0 = rect.y / height;
            float x1 = (rect.x + rect.width) / width;
            float y1 = (rect.y + rect.height) / height;
            UVRect uv = new UVRect();
            uv.cha = key;
            uv.uv0.x = x0;
            uv.uv0.y = y0;
            uv.uv1.x = x0;
            uv.uv1.y = y1;
            uv.uv2.x = x1;
            uv.uv2.y = y1;
            uv.uv3.x = x1;
            uv.uv3.y = y0;
            uv.Scale.x = rect.width / fontSize;
            uv.Scale.y = rect.height / fontSize;
            uv.Pivot.x = 0.5f;
            uv.Pivot.y = 0.5f;
            cash.Add(uv);
        }
        
        /// <summary>
        /// 添加字符图
        /// </summary>
        /// <param name="key">字符</param>
        /// <param name="spi">精灵矩形</param>
        /// <param name="fontSize">此精灵的参考尺寸</param>
        public void AddCharMap(char key, ref SpriteInfo spi, float fontSize)
        {
            UVRect uv = new UVRect();
            uv.cha = key;
            uv.uv0 = spi.uv[0];
            uv.uv1 = spi.uv[1];
            uv.uv2 = spi.uv[2];
            uv.uv3 = spi.uv[3];
            uv.Scale.x = spi.rect.width / fontSize;
            uv.Scale.y = spi.rect.height / fontSize;
            uv.Pivot = spi.pivot;
            cash.Add(uv);
        }
        /// <summary>
        /// 添加字符图
        /// </summary>
        /// <param name="key">字符</param>
        /// <param name="sprite">精灵</param>
        ///     /// <param name="fontSize">此精灵的参考尺寸</param>
        public void AddCharMap(char key, Sprite sprite, float fontSize)
        {
            var rect = sprite.rect;
            float x0 = rect.x / width;
            float y0 = rect.y / height;
            float x1 = (rect.x + rect.width) / width;
            float y1 = (rect.y + rect.height) / height;
            var t = sprite.uv;
            UVRect uv = new UVRect();
            uv.cha = key;
            uv.uv0.x = x0;
            uv.uv0.y = y0;
            uv.uv1.x = x0;
            uv.uv1.y = y1;
            uv.uv2.x = x1;
            uv.uv2.y = y1;
            uv.uv3.x = x1;
            uv.uv3.y = y0;
            uv.Scale.x = rect.width / fontSize;
            uv.Scale.y = rect.height / fontSize;
            uv.Pivot.x = sprite.pivot.x / rect.width;
            uv.Pivot.y = sprite.pivot.y / rect.height;
            cash.Add(uv);
        }
        public bool GetCharacterInfo(char cha, ref CharacterInfo info, int fontSize)
        {
            for (int i = 0; i < cash.Count; i++)
            {
                if (cha == cash[i].cha)
                {
                    float w = cash[i].Scale.x * fontSize;
                    float h = cash[i].Scale.y * fontSize;
                    info.minX = 0;
                    info.glyphWidth = info.advance = info.maxX = (int)w;
                    float oy = 0.5f - cash[i].Pivot.y;
                    oy *= h;
                    info.minY = (int)oy;
                    info.maxY = (int)(oy + h);
                    info.glyphHeight = (int)h;
                    info.uvBottomLeft = cash[i].uv0;
                    info.uvTopLeft = cash[i].uv1;
                    info.uvTopRight = cash[i].uv2;
                    info.uvBottomRight = cash[i].uv3;
                    return true;
                }
            }
            return false;
        }
    }
}
