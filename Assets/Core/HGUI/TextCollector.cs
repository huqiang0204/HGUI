using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Core.HGUI
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
            buf[buffer[point].max] = text;
            buffer[point].max++;
        }
        public void Clear()
        {
            for (int i = 0; i < max; i++)
            {
                int c = buffer[i].max;
                var buf = buffer[i];
                for (int j = 0; j < c; j++)
                    buf.texts[j] = null;
                buffer[i].max = 0;
            }
            max = 0;
            point = 0;
        }
        public void Next()
        {
            point++;
            max = point;
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
                       dirty= true;
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
