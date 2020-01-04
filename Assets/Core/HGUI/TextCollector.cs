using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class TextCollector
    {
        struct TextInfo
        {
            public HText[] texts;
            public int max;
        }
        TextInfo[] buffer = new TextInfo[64];
        int point;
        int max;
        public void AddText(HText text)
        {
            var buf = buffer[point].texts;
            if (buf == null)
            {
                buf = new HText[1024];
                buffer[point].texts = buf;
            }
            int top = buffer[point].max;
            buf[top] = text;
            buffer[point].max++;
        }
        public void Clear()
        {
            for (int i = 0; i <= max; i++)
            {
                int c = buffer[i].max;
                var buf = buffer[i];
                for (int j = 0; j < c; j++)
                    buf.texts[j] = null;
                buffer[i].max = 0;
            }
            max = 1;
            point = 0;
        }
        public void Next()
        {
            point++;
            max = point+1;
        }
        public void Back()
        {
            point--;
        }
        public void GenerateTexture()
        {
            for (int i = 0; i < max; i++)
            {
                int c = buffer[i].max;
                var buf = buffer[i].texts;
                bool dirty = false;
                for (int j = 0; j < c; j++)
                {
                    if (buf[j].m_dirty)
                    {
                        dirty = true;
                        break;
                    }
                }
                if (dirty)
                {
                    for (int j = 0; j < c; j++)
                        buf[j].Populate();
                    var tex = buf[0].Font.material.mainTexture;
                    var id = tex.GetInstanceID();
                    for (int j = 0; j < c; j++)
                    {
                        buf[j].texIds[0] = id;
                        buf[j].textures[0] = tex;
                    }
                }
            }
        }
    }
}
